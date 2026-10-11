using System.ComponentModel.DataAnnotations;

namespace Salonfur.Web.ViewModels.Services;

public class ServiceFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Vul een naam in.")]
    [StringLength(60)]
    [Display(Name = "Naam")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Vul een korte beschrijving in.")]
    [StringLength(300)]
    [Display(Name = "Beschrijving")]
    public string Description { get; set; } = "";

    [StringLength(40)]
    [Display(Name = "Label (bv. \"meest gevraagd\")")]
    public string? Tag { get; set; }

    [Required(ErrorMessage = "Vul een woord in, bv. \"vanaf\".")]
    [StringLength(20)]
    [Display(Name = "Voor de prijs (bv. \"vanaf\")")]
    public string PriceLabel { get; set; } = "vanaf";

    [Required(ErrorMessage = "Vul een prijs in.")]
    [StringLength(30)]
    [Display(Name = "Prijs (bv. \"€ 35\")")]
    public string Price { get; set; } = "";

    [Range(15, 480, ErrorMessage = "Kies een duur tussen 15 en 480 minuten.")]
    [Display(Name = "Duur in minuten")]
    public int DurationMinutes { get; set; } = 60;

    [Display(Name = "Klanten kunnen deze beurt online boeken")]
    public bool IsBookable { get; set; } = true;

    [Display(Name = "Uitgelicht op de website")]
    public bool IsFeatured { get; set; }

    [Range(0, 999)]
    [Display(Name = "Volgorde")]
    public int SortOrder { get; set; }
}
