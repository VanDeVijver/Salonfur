namespace Salonfur.Web.ViewModels.Albums;

public class AlbumsIndexViewModel
{
    public IReadOnlyList<AlbumViewModel> Albums { get; set; } = [];
    public AlbumFormViewModel Form { get; set; } = new();
}
