using System.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Salonfur.Core.Data;
using Salonfur.Core.Entities;

namespace Salonfur.Core.Scheduling;

public record DayAvailability(IReadOnlyList<TimeOnly> Slots, string? Message);

public class BookingService(AppDbContext db)
{
    public const int SlotStepMinutes = 30;
    public const int MaxDaysAhead = 120;

    public static readonly TimeZoneInfo Zone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Brussels");

    public static DateTime LocalNow => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Zone);
    public static DateOnly Today => DateOnly.FromDateTime(LocalNow);

    public async Task<DayAvailability> GetAvailabilityAsync(DateOnly date, int durationMinutes)
    {
        var now = LocalNow;
        var today = DateOnly.FromDateTime(now);

        if (date < today)
            return new([], "Deze dag ligt in het verleden.");
        if (date > today.AddDays(MaxDaysAhead))
            return new([], $"Je kan maximaal {MaxDaysAhead} dagen vooruit boeken.");

        var hours = OpeningHours.For(date);
        if (hours is null)
            return new([], "Op deze dag is de salon gesloten. We zijn open op donderdag, vrijdag en zaterdag.");

        var closed = await db.ClosedPeriods.AsNoTracking()
            .FirstOrDefaultAsync(c => c.StartDate <= date && c.EndDate >= date);
        if (closed is not null)
            return new([], "De salon is gesloten op deze dag" + (string.IsNullOrWhiteSpace(closed.Reason) ? "." : $" ({closed.Reason})."));

        var booked = await db.Reservations.AsNoTracking()
            .Where(r => r.Date == date)
            .Select(r => new { r.StartTime, r.DurationMinutes })
            .ToListAsync();

        var (open, close) = hours.Value;
        var duration = TimeSpan.FromMinutes(durationMinutes);
        var slots = new List<TimeOnly>();

        for (var start = open.ToTimeSpan(); start + duration <= close.ToTimeSpan(); start += TimeSpan.FromMinutes(SlotStepMinutes))
        {
            if (date == today && start <= now.TimeOfDay)
                continue;

            var end = start + duration;
            var overlaps = booked.Any(b =>
                start < b.StartTime.ToTimeSpan() + TimeSpan.FromMinutes(b.DurationMinutes) && b.StartTime.ToTimeSpan() < end);
            if (!overlaps)
                slots.Add(TimeOnly.FromTimeSpan(start));
        }

        return new(slots, slots.Count == 0 ? "Deze dag is volzet voor deze beurt. Probeer een andere dag." : null);
    }

    // Returns false when the slot is no longer free (e.g. someone booked it a moment earlier).
    public async Task<bool> TryBookAsync(Reservation reservation)
    {
        try
        {
            await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            var availability = await GetAvailabilityAsync(reservation.Date, reservation.DurationMinutes);
            if (!availability.Slots.Contains(reservation.StartTime))
                return false;

            reservation.CreatedAtUtc = DateTime.UtcNow;
            db.Reservations.Add(reservation);
            await db.SaveChangesAsync();
            await tx.CommitAsync();
            return true;
        }
        catch (Exception ex) when (IsSerializationFailure(ex))
        {
            db.ChangeTracker.Clear();
            return false;
        }
    }

    private static bool IsSerializationFailure(Exception ex) =>
        (ex as PostgresException ?? ex.InnerException as PostgresException)?.SqlState == PostgresErrorCodes.SerializationFailure;
}
