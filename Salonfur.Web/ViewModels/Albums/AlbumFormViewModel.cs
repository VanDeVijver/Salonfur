using System.ComponentModel.DataAnnotations;

namespace Salonfur.Web.ViewModels.Albums;

public class AlbumFormViewModel
{
    [Required(ErrorMessage = "Geef het album een titel.")]
    [StringLength(80)]
    [Display(Name = "Titel")]
    public string Title { get; set; } = "";

    [StringLength(300)]
    [Display(Name = "Beschrijving")]
    public string? Description { get; set; }
}
