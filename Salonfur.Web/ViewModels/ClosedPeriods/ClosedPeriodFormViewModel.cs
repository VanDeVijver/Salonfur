using System.ComponentModel.DataAnnotations;

namespace Salonfur.Web.ViewModels.ClosedPeriods;

public class ClosedPeriodFormViewModel : IValidatableObject
{
    [Required(ErrorMessage = "Kies een begindatum.")]
    [Display(Name = "Van")]
    public DateOnly? StartDate { get; set; }

    [Required(ErrorMessage = "Kies een einddatum.")]
    [Display(Name = "Tot en met")]
    public DateOnly? EndDate { get; set; }

    [StringLength(100)]
    [Display(Name = "Reden (optioneel, zichtbaar voor klanten)")]
    public string? Reason { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartDate is not null && EndDate is not null && EndDate < StartDate)
            yield return new ValidationResult("De einddatum ligt voor de begindatum.", [nameof(EndDate)]);
    }
}
