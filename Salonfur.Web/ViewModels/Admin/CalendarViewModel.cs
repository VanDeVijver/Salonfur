namespace Salonfur.Web.ViewModels.Admin;

public class CalendarViewModel
{
    // FullCalendar day numbers (0 = Sunday) on which the salon is never open.
    public IReadOnlyList<int> HiddenDays { get; set; } = [];
    public string SlotMinTime { get; set; } = "09:00";
    public string SlotMaxTime { get; set; } = "18:00";
}
