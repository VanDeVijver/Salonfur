using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Salonfur.Core.Data;
using Salonfur.Core.Entities;
using Salonfur.Web.ViewModels.Albums;

namespace Salonfur.Web.Controllers;

[Authorize(Roles = Seeder.AdminRole)]
[Route("admin/albums")]
public class AlbumsController(AppDbContext db) : Controller
{
    private const long MaxPhotoBytes = 10 * 1024 * 1024;
    private const long MaxUploadBytes = 100 * 1024 * 1024;
    private static readonly HashSet<string> AllowedTypes = ["image/jpeg", "image/png", "image/webp"];

    [HttpGet("")]
    public async Task<IActionResult> Index() => View(new AlbumsIndexViewModel { Albums = await LoadAsync(db) });

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind(Prefix = "Form")] AlbumFormViewModel form)
    {
        if (!ModelState.IsValid)
            return View(nameof(Index), new AlbumsIndexViewModel { Albums = await LoadAsync(db), Form = form });

        var album = new Album
        {
            Title = form.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(form.Description) ? null : form.Description.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
        };
        db.Albums.Add(album);
        await db.SaveChangesAsync();
        TempData["Message"] = $"\"{album.Title}\" is aangemaakt. Voeg nu foto's toe.";
        return RedirectToAction(nameof(Details), new { id = album.Id });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var album = (await LoadAsync(db, id)).FirstOrDefault();
        return album is null ? NotFound() : View(album);
    }

    [HttpPost("{id:int}/fotos")]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(MaxUploadBytes)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxUploadBytes)]
    public async Task<IActionResult> Upload(int id, List<IFormFile> files)
    {
        if (!await db.Albums.AnyAsync(a => a.Id == id))
            return NotFound();

        var skipped = new List<string>();
        foreach (var file in files)
        {
            if (file.Length == 0 || file.Length > MaxPhotoBytes || !AllowedTypes.Contains(file.ContentType))
            {
                skipped.Add(file.FileName);
                continue;
            }

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            db.Photos.Add(new Photo { AlbumId = id, ContentType = file.ContentType, Data = ms.ToArray(), CreatedAtUtc = DateTime.UtcNow });
        }
        await db.SaveChangesAsync();

        TempData["Message"] = $"{files.Count - skipped.Count} foto('s) toegevoegd."
            + (skipped.Count == 0 ? "" : $" Overgeslagen (enkel JPG, PNG of WebP tot 10 MB): {string.Join(", ", skipped)}.");
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost("{id:int}/verwijderen")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await db.Albums.Where(a => a.Id == id).ExecuteDeleteAsync();
        TempData["Message"] = "Het album is verwijderd.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("fotos/{photoId:int}/verwijderen")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePhoto(int photoId)
    {
        var albumId = await db.Photos.Where(p => p.Id == photoId).Select(p => (int?)p.AlbumId).FirstOrDefaultAsync();
        if (albumId is null)
            return NotFound();

        await db.Photos.Where(p => p.Id == photoId).ExecuteDeleteAsync();
        TempData["Message"] = "De foto is verwijderd.";
        return RedirectToAction(nameof(Details), new { id = albumId });
    }

    // Public. A photo never changes after upload, so browsers may cache it for good.
    [AllowAnonymous]
    [HttpGet("/fotos/{photoId:int}")]
    [ResponseCache(Duration = 31536000)]
    public async Task<IActionResult> Photo(int photoId)
    {
        var photo = await db.Photos.AsNoTracking().Where(p => p.Id == photoId)
            .Select(p => new { p.ContentType, p.Data }).FirstOrDefaultAsync();
        if (photo is null)
            return NotFound();

        Response.Headers.XContentTypeOptions = "nosniff";
        return File(photo.Data, photo.ContentType);
    }

    // Projects to photo ids only, so the image bytes stay in the database.
    public static async Task<IReadOnlyList<AlbumViewModel>> LoadAsync(AppDbContext db, int? id = null) =>
        await db.Albums.AsNoTracking()
            .Where(a => id == null || a.Id == id)
            .OrderByDescending(a => a.CreatedAtUtc)
            .Select(a => new AlbumViewModel(a.Id, a.Title, a.Description, a.Photos.OrderBy(p => p.Id).Select(p => p.Id).ToList()))
            .ToListAsync();
}
