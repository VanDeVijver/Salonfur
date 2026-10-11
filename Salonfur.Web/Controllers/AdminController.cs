using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Salonfur.Core.Data;
using Salonfur.Core.Scheduling;
using Salonfur.Web.ViewModels.Admin;

namespace Salonfur.Web.Controllers;

[Authorize(Roles = Seeder.AdminRole)]
[Route("admin")]
public class AdminController(AppDbContext db) : Controller
{
    [HttpGet("")]
    public IActionResult Index() => View(new CalendarViewModel
    {
        HiddenDays = Enum.GetValues<DayOfWeek>().Where(d => !OpeningHours.Week.ContainsKey(d)).Select(d => (int)d).ToList(),
        SlotMinTime = OpeningHours.Week.Values.Min(h => h.Open).ToString("HH:mm"),
        SlotMaxTime = OpeningHours.Week.Values.Max(h => h.Close).ToString("HH:mm"),
    });

    // Event feed for FullCalendar. start/end arrive as ISO strings; only the date part matters.
    [HttpGet("agenda")]
    public async Task<IActionResult> Events(string start, string end)
    {
        if (!DateOnly.TryParse(start.AsSpan(0, Math.Min(10, start.Length)), out var from) ||
            !DateOnly.TryParse(end.AsSpan(0, Math.Min(10, end.Length)), out var to))
            return BadRequest();

        var reservations = await db.Reservations.AsNoTracking()
            .Where(r => r.Date >= from && r.Date < to)
            .Select(r => new { r.Id, r.Date, r.StartTime, r.DurationMinutes, r.DogName, Service = r.Service.Name })
            .ToListAsync();

        var closed = await db.ClosedPeriods.AsNoTracking()
            .Where(c => c.EndDate >= from && c.StartDate < to)
            .ToListAsync();

        var events = reservations.Select(r => (object)new
        {
            id = r.Id.ToString(),
            title = $"{r.DogName} · {r.Service}",
            start = r.Date.ToDateTime(r.StartTime).ToString("s"),
            end = r.Date.ToDateTime(r.StartTime.AddMinutes(r.DurationMinutes)).ToString("s"),
            url = Url.Action(nameof(Details), new { id = r.Id }),
        }).Concat(closed.Select(c => (object)new
        {
            id = "closed-" + c.Id,
            title = string.IsNullOrWhiteSpace(c.Reason) ? "Gesloten" : $"Gesloten: {c.Reason}",
            start = c.StartDate.ToString("yyyy-MM-dd"),
            end = c.EndDate.AddDays(1).ToString("yyyy-MM-dd"),
            allDay = true,
            display = "background",
            classNames = new[] { "closed-day" },
        }));

        return Json(events);
    }

    [HttpGet("reservaties/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var r = await db.Reservations.AsNoTracking().Include(x => x.Service).FirstOrDefaultAsync(x => x.Id == id);
        if (r is null)
            return NotFound();

        return View(new ReservationDetailsViewModel
        {
            Id = r.Id,
            Date = r.Date,
            StartTime = r.StartTime,
            EndTime = r.EndTime,
            ServiceName = r.Service.Name,
            DogName = r.DogName,
            Breed = r.Breed,
            Size = r.Size,
            FirstVisit = r.FirstVisit,
            CustomerName = r.CustomerName,
            Phone = r.Phone,
            Email = r.Email,
            Remarks = r.Remarks,
            CreatedAtLocal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(r.CreatedAtUtc, DateTimeKind.Utc), BookingService.Zone),
        });
    }

    [HttpPost("reservaties/{id:int}/verwijderen")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await db.Reservations.Where(r => r.Id == id).ExecuteDeleteAsync();
        TempData["Message"] = deleted > 0 ? "De afspraak is verwijderd." : "Deze afspraak bestond niet meer.";
        return RedirectToAction(nameof(Index));
    }
}
