using System.ComponentModel.DataAnnotations;

namespace Salonfur.Core.Entities;

public class Service
{
    public int Id { get; set; }

    [MaxLength(60)]
    public string Name { get; set; } = "";

    [MaxLength(300)]
    public string Description { get; set; } = "";

    [MaxLength(40)]
    public string? Tag { get; set; }

    // Shown as "<PriceLabel> <Price>", e.g. "vanaf € 35" or "per kwartier".
    [MaxLength(20)]
    public string PriceLabel { get; set; } = "vanaf";

    [MaxLength(30)]
    public string Price { get; set; } = "";

    public int DurationMinutes { get; set; }

    // Extras such as "ontklitten" are shown on the site but cannot be booked on their own.
    public bool IsBookable { get; set; } = true;

    public bool IsFeatured { get; set; }

    public int SortOrder { get; set; }

    public List<Reservation> Reservations { get; set; } = [];
}
