namespace Salonfur.Web.ViewModels.Albums;

public record AlbumViewModel(int Id, string Title, string? Description, IReadOnlyList<int> PhotoIds);
