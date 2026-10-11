namespace Salonfur.Web.ViewModels.Shared;

public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public int StatusCode { get; set; } = 500;
    public bool IsNotFound => StatusCode == 404;
}
