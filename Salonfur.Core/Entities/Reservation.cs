using System.ComponentModel.DataAnnotations;
using Salonfur.Core.Enums;

namespace Salonfur.Core.Entities;

public class Reservation
{
    public int Id { get; set; }

    // Local salon time (Europe/Brussels).
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }

    // Copied from the service at booking time, so later edits to a service don't move existing bookings.
    public int DurationMinutes { get; set; }

    public int ServiceId { get; set; }
    public Service Service { get; set; } = null!;

    [MaxLength(60)]
    public string DogName { get; set; } = "";

    [MaxLength(80)]
    public string? Breed { get; set; }

    public DogSize Size { get; set; }

    public bool FirstVisit { get; set; }

    [MaxLength(100)]
    public string CustomerName { get; set; } = "";

    [MaxLength(30)]
    public string Phone { get; set; } = "";

    [MaxLength(200)]
    public string? Email { get; set; }

    [MaxLength(1000)]
    public string? Remarks { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public TimeOnly EndTime => StartTime.AddMinutes(DurationMinutes);
}
