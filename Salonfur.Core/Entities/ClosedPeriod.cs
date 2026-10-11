using System.ComponentModel.DataAnnotations;

namespace Salonfur.Core.Entities;

public class ClosedPeriod
{
    public int Id { get; set; }

    public DateOnly StartDate { get; set; }

    // Inclusive.
    public DateOnly EndDate { get; set; }

    [MaxLength(100)]
    public string? Reason { get; set; }
}
