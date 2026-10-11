using Salonfur.Core.Scheduling;

namespace Salonfur.Web;

// Fixed business details shown on the public site.
public static class SalonInfo
{
    public const string Street = "Bavegemstraat 4";
    public const string City = "9860 Oosterzele";
    public const string RouteUrl = "https://www.google.com/maps/dir/?api=1&destination=Bavegemstraat+4,+9860+Oosterzele";
    public const string MapEmbedUrl = "https://www.openstreetmap.org/export/embed.html?bbox=3.8189%2C50.9536%2C3.8389%2C50.9607&layer=mapnik&marker=50.95716%2C3.82888";
    public const string ReviewUrl = "https://g.page/r/caot11kfielqeae/review";
    public const string InstagramUrl = "https://www.instagram.com/salon.fur/";

    public static readonly (string Display, string Href)[] Phones =
    [
        ("+32 496 03 60 70", "tel:+32496036070"),
        ("+49 6036 070", "tel:+496036070"),
    ];

    private static readonly (DayOfWeek Day, string Name)[] Days =
    [
        (DayOfWeek.Monday, "Maandag"), (DayOfWeek.Tuesday, "Dinsdag"), (DayOfWeek.Wednesday, "Woensdag"),
        (DayOfWeek.Thursday, "Donderdag"), (DayOfWeek.Friday, "Vrijdag"), (DayOfWeek.Saturday, "Zaterdag"),
        (DayOfWeek.Sunday, "Zondag"),
    ];

    public static IEnumerable<(string Day, string Hours)> HoursTable =>
        Days.Select(d => OpeningHours.Week.TryGetValue(d.Day, out var h)
            ? (d.Name, $"{h.Open:H\\:mm} – {h.Close:H\\:mm}")
            : (d.Name, "Gesloten"));
}
