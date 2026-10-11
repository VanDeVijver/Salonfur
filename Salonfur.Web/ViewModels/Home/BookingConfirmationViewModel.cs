namespace Salonfur.Web.ViewModels.Home;

public class BookingConfirmationViewModel
{
    public string DogName { get; set; } = "";
    public string ServiceName { get; set; } = "";
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
}
