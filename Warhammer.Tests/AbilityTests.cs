using WarhammerMinimal.Abilities;
using Xunit;

namespace WarhammerMinimal.Tests;

public class AbilityTests
{
    [Theory]
    [InlineData(Skills.Smite, 2)]
    [InlineData(Skills.WarpCurse, 2)]
    [InlineData(Skills.Rage, 1)]
    [InlineData(Skills.Heal, 1)]
    [InlineData(Skills.BolterFire, 1)]
    [InlineData(Skills.Narthecium, 1)]
    [InlineData(Skills.Chainsword, 1)]
    public void ApCost_MatchesSpec(string id, int expected)
    {
        Assert.Equal(expected, Skills.ApCost(id));
    }

    [Fact]
    public void Describe_ReturnsHumanReadable()
    {
        Assert.Contains("Ярость", Skills.Describe(Skills.Rage));
        Assert.Contains("Стойкость", Skills.Describe(Skills.Toughness));
    }
}