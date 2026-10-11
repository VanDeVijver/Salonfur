using Salonfur.Core.Enums;

namespace Salonfur.Web.ViewModels.Admin;

public class ReservationDetailsViewModel
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string ServiceName { get; set; } = "";
    public string DogName { get; set; } = "";
    public string? Breed { get; set; }
    public DogSize Size { get; set; }
    public bool FirstVisit { get; set; }
    public string CustomerName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string? Email { get; set; }
    public string? Remarks { get; set; }
    public DateTime CreatedAtLocal { get; set; }
}
