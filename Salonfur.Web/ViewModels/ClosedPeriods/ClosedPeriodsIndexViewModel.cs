using Salonfur.Core.Entities;

namespace Salonfur.Web.ViewModels.ClosedPeriods;

public class ClosedPeriodsIndexViewModel
{
    public IReadOnlyList<ClosedPeriod> Upcoming { get; set; } = [];
    public ClosedPeriodFormViewModel Form { get; set; } = new();
}
