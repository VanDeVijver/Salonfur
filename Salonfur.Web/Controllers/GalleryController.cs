using Microsoft.AspNetCore.Mvc;
using Salonfur.Core.Data;

namespace Salonfur.Web.Controllers;

public class GalleryController(AppDbContext db) : Controller
{
    [HttpGet("/albums")]
    public async Task<IActionResult> Index() =>
        View((await AlbumsController.LoadAsync(db)).Where(a => a.PhotoIds.Count > 0).ToList());

    [HttpGet("/albums/{id:int}")]
    public async Task<IActionResult> Album(int id)
    {
        var album = (await AlbumsController.LoadAsync(db, id)).FirstOrDefault();
        return album is null || album.PhotoIds.Count == 0 ? NotFound() : View(album);
    }
}
