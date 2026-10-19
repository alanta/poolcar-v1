using PoolCar.Web.Clients;
using PoolCar.Web.Reserving;
using Shouldly;

namespace PoolCar.Web.Tests;

public class ComparableCarsTests
{
    private static readonly FleetCar Original = Car("ORIG01", "Kia", "Niro", 400);

    [Fact]
    public void Rank_PutsSameModelFirst_ThenAtLeastTheRange_ThenLessRange()
    {
        var free = new[]
        {
            Car("LESS01", "Volvo", "EX30", 350),
            Car("MORE01", "Volvo", "EX30", 450),
            Car("NIRO01", "Kia", "Niro", 300),
            Car("SAME01", "Skoda", "Enyaq", 400),
            Car("MORE02", "Skoda", "Enyaq", 500),
        };

        var ranked = ComparableCars.Rank(Original, free, max: 10);

        ranked.Select(c => c.Plate).ShouldBe(["NIRO01", "MORE02", "MORE01", "SAME01", "LESS01"]);
    }

    [Fact]
    public void Rank_BreaksTiesByPlate()
    {
        var ranked = ComparableCars.Rank(Original, [Car("BB0002", "Volvo", "EX30", 450), Car("AA0001", "Volvo", "EX30", 450)]);

        ranked.Select(c => c.Plate).ShouldBe(["AA0001", "BB0002"]);
    }

    [Fact]
    public void Rank_ReturnsAtMostThree()
    {
        var free = Enumerable.Range(1, 5).Select(i => Car($"CAR{i:000}", "Volvo", "EX30", 400 + i));

        ComparableCars.Rank(Original, free).Count.ShouldBe(ComparableCars.MaxSuggestions);
    }

    [Fact]
    public void Rank_SkipsTheOriginal_OtherCategories_AndOtherOffices()
    {
        var free = new[]
        {
            Original,
            Car("KLEIN1", "Renault", "5", 400, category: "klein"),
            Car("AMSTD1", "Kia", "Niro", 400, office: "amsterdam"),
        };

        ComparableCars.Rank(Original, free).ShouldBeEmpty();
    }

    private static FleetCar Car(string plate, string make, string model, int rangeKm, string category = "middel", string office = "utrecht") =>
        new(plate, make, model, "", category, office, "Grijs", 5, rangeKm, 150, new DateOnly(2024, 1, 1), null);
}
