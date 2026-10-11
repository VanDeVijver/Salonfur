using System.ComponentModel.DataAnnotations;
using Salonfur.Core.Enums;

namespace Salonfur.Web.ViewModels.Home;

public class BookingFormViewModel
{
    [Required(ErrorMessage = "Vul de naam van je hond in.")]
    [StringLength(60)]
    [Display(Name = "Naam van je hond")]
    public string DogName { get; set; } = "";

    [StringLength(80)]
    [Display(Name = "Ras")]
    public string? Breed { get; set; }

    [Required(ErrorMessage = "Kies de grootte van je hond.")]
    public DogSize? Size { get; set; } = DogSize.Klein;

    [Required(ErrorMessage = "Kies een beurt.")]
    public int? ServiceId { get; set; }

    [Required(ErrorMessage = "Kies een dag.")]
    [Display(Name = "Dag")]
    public DateOnly? Date { get; set; }

    [Required(ErrorMessage = "Kies een tijdstip.")]
    public TimeOnly? StartTime { get; set; }

    [Display(Name = "Mijn hond komt voor het eerst naar salon.fur")]
    public bool FirstVisit { get; set; }

    [Required(ErrorMessage = "Vul je naam in.")]
    [StringLength(100)]
    [Display(Name = "Jouw naam")]
    public string CustomerName { get; set; } = "";

    [Required(ErrorMessage = "Vul je gsm-nummer in.")]
    [Phone(ErrorMessage = "Dit is geen geldig telefoonnummer.")]
    [StringLength(30)]
    [Display(Name = "GSM")]
    public string Phone { get; set; } = "";

    [EmailAddress(ErrorMessage = "Dit is geen geldig e-mailadres.")]
    [StringLength(200)]
    [Display(Name = "E-mail (optioneel)")]
    public string? Email { get; set; }

    [StringLength(1000)]
    [Display(Name = "Opmerkingen")]
    public string? Remarks { get; set; }

    // Honeypot: hidden from people, bots tend to fill it in.
    public string? Website { get; set; }
}
