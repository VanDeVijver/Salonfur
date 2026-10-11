using System.ComponentModel.DataAnnotations;

namespace Salonfur.Web.ViewModels.Account;

public class LoginViewModel
{
    [Required(ErrorMessage = "Vul je e-mailadres in.")]
    [EmailAddress(ErrorMessage = "Dit is geen geldig e-mailadres.")]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Vul je wachtwoord in.")]
    [DataType(DataType.Password)]
    [Display(Name = "Wachtwoord")]
    public string Password { get; set; } = "";

    [Display(Name = "Ingelogd blijven")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}
