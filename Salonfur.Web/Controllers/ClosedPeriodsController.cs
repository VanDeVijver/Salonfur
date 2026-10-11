using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Salonfur.Core.Data;
using Salonfur.Core.Entities;
using Salonfur.Core.Scheduling;
using Salonfur.Web.ViewModels.ClosedPeriods;

namespace Salonfur.Web.Controllers;

[Authorize(Roles = Seeder.AdminRole)]
[Route("admin/sluitingen")]
public class ClosedPeriodsController(AppDbContext db) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index() => View(await BuildAsync(new ClosedPeriodFormViewModel()));

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind(Prefix = "Form")] ClosedPeriodFormViewModel form)
    {
        if (!ModelState.IsValid)
            return View(nameof(Index), await BuildAsync(form));

        var period = new ClosedPeriod
        {
            StartDate = form.StartDate!.Value,
            EndDate = form.EndDate!.Value,
            Reason = string.IsNullOrWhiteSpace(form.Reason) ? null : form.Reason.Trim(),
        };
        db.ClosedPeriods.Add(period);
        await db.SaveChangesAsync();

        var existing = await db.Reservations.CountAsync(r => r.Date >= period.StartDate && r.Date <= period.EndDate);
        TempData["Message"] = existing == 0
            ? "De sluiting is toegevoegd."
            : $"De sluiting is toegevoegd. Let op: er {(existing == 1 ? "staat al 1 afspraak" : $"staan al {existing} afspraken")} in deze periode. Die blijven staan; neem contact op met de klant.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("{id:int}/verwijderen")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await db.ClosedPeriods.Where(c => c.Id == id).ExecuteDeleteAsync();
        TempData["Message"] = "De sluiting is verwijderd.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<ClosedPeriodsIndexViewModel> BuildAsync(ClosedPeriodFormViewModel form)
    {
        var today = BookingService.Today;
        return new ClosedPeriodsIndexViewModel
        {
            Upcoming = await db.ClosedPeriods.AsNoTracking().Where(c => c.EndDate >= today).OrderBy(c => c.StartDate).ToListAsync(),
            Form = form,
        };
    }
}
