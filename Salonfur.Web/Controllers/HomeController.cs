using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Salonfur.Core.Data;
using Salonfur.Core.Entities;
using Salonfur.Core.Scheduling;
using Salonfur.Web.ViewModels.Home;
using Salonfur.Web.ViewModels.Shared;

namespace Salonfur.Web.Controllers;

public class HomeController(AppDbContext db, BookingService booking) : Controller
{
    private const string ConfirmationKey = "BookingConfirmation";

    public async Task<IActionResult> Index() => View(await BuildHomeAsync(new BookingFormViewModel()));

    [HttpGet("/afspraak/tijden")]
    public async Task<IActionResult> Slots(DateOnly date, int serviceId)
    {
        var service = await db.Services.AsNoTracking().FirstOrDefaultAsync(s => s.Id == serviceId && s.IsBookable);
        if (service is null)
            return Json(new { slots = Array.Empty<string>(), message = "Kies eerst een beurt." });

        var availability = await booking.GetAvailabilityAsync(date, service.DurationMinutes);
        return Json(new { slots = availability.Slots.Select(t => t.ToString("HH:mm")), message = availability.Message });
    }

    [HttpPost("/afspraak")]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("booking")]
    public async Task<IActionResult> Book([Bind(Prefix = "Booking")] BookingFormViewModel form)
    {
        // Honeypot filled in: act as if it worked, store nothing.
        if (!string.IsNullOrEmpty(form.Website))
            return RedirectToAction(nameof(Index));

        var service = form.ServiceId is null ? null
            : await db.Services.AsNoTracking().FirstOrDefaultAsync(s => s.Id == form.ServiceId && s.IsBookable);
        if (form.ServiceId is not null && service is null)
            ModelState.AddModelError("Booking.ServiceId", "Deze beurt kan niet online geboekt worden.");

        if (!ModelState.IsValid || service is null)
            return View(nameof(Index), await BuildHomeAsync(form));

        var reservation = new Reservation
        {
            Date = form.Date!.Value,
            StartTime = form.StartTime!.Value,
            DurationMinutes = service.DurationMinutes,
            ServiceId = service.Id,
            DogName = form.DogName.Trim(),
            Breed = form.Breed?.Trim(),
            Size = form.Size!.Value,
            FirstVisit = form.FirstVisit,
            CustomerName = form.CustomerName.Trim(),
            Phone = form.Phone.Trim(),
            Email = form.Email?.Trim(),
            Remarks = form.Remarks?.Trim(),
        };

        if (!await booking.TryBookAsync(reservation))
        {
            ModelState.AddModelError("Booking.StartTime", "Dit tijdstip is niet (meer) vrij. Kies een ander moment.");
            form.StartTime = null;
            return View(nameof(Index), await BuildHomeAsync(form));
        }

        TempData[ConfirmationKey] = JsonSerializer.Serialize(new BookingConfirmationViewModel
        {
            DogName = reservation.DogName,
            ServiceName = service.Name,
            Date = reservation.Date,
            StartTime = reservation.StartTime,
        });
        return RedirectToAction(nameof(Confirmation));
    }

    [HttpGet("/afspraak/bevestigd")]
    public IActionResult Confirmation()
    {
        if (TempData[ConfirmationKey] is not string json)
            return RedirectToAction(nameof(Index));

        return View(JsonSerializer.Deserialize<BookingConfirmationViewModel>(json));
    }

    [HttpGet("/privacy")]
    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult HttpStatus(int id)
    {
        Response.StatusCode = id;
        return View("Error", new ErrorViewModel { StatusCode = id, RequestId = HttpContext.TraceIdentifier });
    }

    private async Task<HomeViewModel> BuildHomeAsync(BookingFormViewModel form)
    {
        var today = BookingService.Today;
        return new HomeViewModel
        {
            Services = await db.Services.AsNoTracking().OrderBy(s => s.SortOrder).ThenBy(s => s.Name).ToListAsync(),
            Booking = form,
            MinDate = today,
            MaxDate = today.AddDays(BookingService.MaxDaysAhead),
        };
    }
}
