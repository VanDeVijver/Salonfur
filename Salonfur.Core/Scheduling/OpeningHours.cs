namespace Salonfur.Core.Scheduling;

public static class OpeningHours
{
    public static readonly IReadOnlyDictionary<DayOfWeek, (TimeOnly Open, TimeOnly Close)> Week =
        new Dictionary<DayOfWeek, (TimeOnly, TimeOnly)>
        {
            [DayOfWeek.Thursday] = (new(9, 0), new(18, 0)),
            [DayOfWeek.Friday] = (new(9, 0), new(18, 0)),
            [DayOfWeek.Saturday] = (new(9, 0), new(12, 0)),
        };

    public static (TimeOnly Open, TimeOnly Close)? For(DateOnly date) =>
        Week.TryGetValue(date.DayOfWeek, out var hours) ? hours : null;
}
