using PoolCar.Web.Clients;

namespace PoolCar.Web.Reserving;

/// <summary>Picks free cars to offer instead of a car that can't be reserved.</summary>
public static class ComparableCars
{
    public const int MaxSuggestions = 3;

    /// <summary>The free cars of the same category and office as <paramref name="original"/>: the same model first,
    /// then cars with at least its range, then cars with less range; the longest range first within each group.</summary>
    public static IReadOnlyList<FleetCar> Rank(FleetCar original, IEnumerable<FleetCar> free, int max = MaxSuggestions) =>
    [
        .. free
            .Where(c => !string.Equals(c.Plate, original.Plate, StringComparison.OrdinalIgnoreCase)
                && c.Category == original.Category
                && string.Equals(c.Office, original.Office, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(c => IsSameModel(c, original))
            .ThenByDescending(c => c.RangeKm >= original.RangeKm)
            .ThenByDescending(c => c.RangeKm)
            .ThenBy(c => c.Plate, StringComparer.Ordinal)
            .Take(max),
    ];

    private static bool IsSameModel(FleetCar car, FleetCar original) =>
        string.Equals(car.Make, original.Make, StringComparison.OrdinalIgnoreCase)
        && string.Equals(car.Model, original.Model, StringComparison.OrdinalIgnoreCase);
}
