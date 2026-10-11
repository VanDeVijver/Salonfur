using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Salonfur.Core.Data;
using Salonfur.Core.Entities;
using Salonfur.Web.ViewModels.Services;

namespace Salonfur.Web.Controllers;

[Authorize(Roles = Seeder.AdminRole)]
[Route("admin/diensten")]
public class ServicesController(AppDbContext db) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index() => View(new ServiceListViewModel
    {
        Services = await db.Services.AsNoTracking().OrderBy(s => s.SortOrder).ThenBy(s => s.Name).ToListAsync(),
    });

    [HttpGet("nieuw")]
    public async Task<IActionResult> Create() =>
        View("Form", new ServiceFormViewModel { SortOrder = (await db.Services.MaxAsync(s => (int?)s.SortOrder) ?? 0) + 1 });

    [HttpPost("nieuw")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ServiceFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View("Form", model);

        var service = new Service();
        Apply(model, service);
        db.Services.Add(service);
        await db.SaveChangesAsync();
        TempData["Message"] = $"\"{service.Name}\" is toegevoegd.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("{id:int}/bewerken")]
    public async Task<IActionResult> Edit(int id)
    {
        var s = await db.Services.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (s is null)
            return NotFound();

        return View("Form", new ServiceFormViewModel
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description,
            Tag = s.Tag,
            PriceLabel = s.PriceLabel,
            Price = s.Price,
            DurationMinutes = s.DurationMinutes,
            IsBookable = s.IsBookable,
            IsFeatured = s.IsFeatured,
            SortOrder = s.SortOrder,
        });
    }

    [HttpPost("{id:int}/bewerken")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ServiceFormViewModel model)
    {
        model.Id = id;
        if (!ModelState.IsValid)
            return View("Form", model);

        var service = await db.Services.FindAsync(id);
        if (service is null)
            return NotFound();

        Apply(model, service);
        await db.SaveChangesAsync();
        TempData["Message"] = $"\"{service.Name}\" is bijgewerkt.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("{id:int}/verwijderen")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var service = await db.Services.FindAsync(id);
        if (service is null)
            return NotFound();

        if (await db.Reservations.AnyAsync(r => r.ServiceId == id))
        {
            TempData["Message"] = $"\"{service.Name}\" heeft afspraken en kan niet verwijderd worden. Zet online boeken uit om de dienst te verbergen in het formulier.";
            return RedirectToAction(nameof(Index));
        }

        db.Services.Remove(service);
        await db.SaveChangesAsync();
        TempData["Message"] = $"\"{service.Name}\" is verwijderd.";
        return RedirectToAction(nameof(Index));
    }

    private static void Apply(ServiceFormViewModel model, Service service)
    {
        service.Name = model.Name.Trim();
        service.Description = model.Description.Trim();
        service.Tag = string.IsNullOrWhiteSpace(model.Tag) ? null : model.Tag.Trim();
        service.PriceLabel = model.PriceLabel.Trim();
        service.Price = model.Price.Trim();
        service.DurationMinutes = model.DurationMinutes;
        service.IsBookable = model.IsBookable;
        service.IsFeatured = model.IsFeatured;
        service.SortOrder = model.SortOrder;
    }
}
