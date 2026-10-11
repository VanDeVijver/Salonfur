using Salonfur.Core.Entities;

namespace Salonfur.Web.ViewModels.Services;

public class ServiceListViewModel
{
    public IReadOnlyList<Service> Services { get; set; } = [];
}
