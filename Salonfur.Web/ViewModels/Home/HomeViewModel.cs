using Salonfur.Core.Entities;

namespace Salonfur.Web.ViewModels.Home;

public class HomeViewModel
{
    public IReadOnlyList<Service> Services { get; set; } = [];
    public IEnumerable<Service> BookableServices => Services.Where(s => s.IsBookable);
    public BookingFormViewModel Booking { get; set; } = new();
    public DateOnly MinDate { get; set; }
    public DateOnly MaxDate { get; set; }
}
