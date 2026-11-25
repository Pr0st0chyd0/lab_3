using WarhammerMinimal.Abilities;
using WarhammerMinimal.Models;
using Xunit;

namespace WarhammerMinimal.Tests;

public class CombatTests
{
    [Fact]
    public void BaseDamage_EqualsAttackerDamage_NoModifiers()
    {
        var atk = TestHelpers.MakeUnit(dmg: 5);
        var def = TestHelpers.MakeUnit(team: Team.Chaos, hp: 20);
        var g = TestHelpers.MakeGame(TestHelpers.MakeState());

        Assert.Equal(5, g.ComputeDamage(atk, def));
    }

    [Fact]
    public void Rage_AddsThreeDamage()
    {
        var atk = TestHelpers.MakeUnit(dmg: 5);
        atk.RageTurnsLeft = 1;
        var def = TestHelpers.MakeUnit(team: Team.Chaos);
        var g = TestHelpers.MakeGame(TestHelpers.MakeState());

        Assert.Equal(8, g.ComputeDamage(atk, def));
    }

    [Fact]
    public void Zeal_AddsOne_WhenHpBelowHalf()
    {
        var atk = TestHelpers.MakeUnit(dmg: 5, passives: new[] { Skills.Zeal });
        atk.Hp = atk.MaxHp / 2 - 1;
        var def = TestHelpers.MakeUnit(team: Team.Chaos);
        var g = TestHelpers.MakeGame(TestHelpers.MakeState());

        Assert.Equal(6, g.ComputeDamage(atk, def));
    }

    [Fact]
    public void Zeal_NoBonus_WhenHpAtHalf()
    {
        var atk = TestHelpers.MakeUnit(dmg: 5, hp: 10, passives: new[] { Skills.Zeal });
        atk.Hp = 5;
        var def = TestHelpers.MakeUnit(team: Team.Chaos);
        var g = TestHelpers.MakeGame(TestHelpers.MakeState());

        Assert.Equal(5, g.ComputeDamage(atk, def));
    }

    [Fact]
    public void Toughness_ReducesByOne()
    {
        var atk = TestHelpers.MakeUnit(dmg: 5);
        var def = TestHelpers.MakeUnit(team: Team.Chaos, passives: new[] { Skills.Toughness });
        var g = TestHelpers.MakeGame(TestHelpers.MakeState());

        Assert.Equal(4, g.ComputeDamage(atk, def));
    }

    [Fact]
    public void Toughness_NeverDropsBelowOne()
    {
        var atk = TestHelpers.MakeUnit(dmg: 1);
        var def = TestHelpers.MakeUnit(team: Team.Chaos, passives: new[] { Skills.Toughness });
        var g = TestHelpers.MakeGame(TestHelpers.MakeState());

        Assert.Equal(1, g.ComputeDamage(atk, def));
    }

    [Fact]
    public void Curse_AddsTwo_OnTarget()
    {
        var atk = TestHelpers.MakeUnit(dmg: 5);
        var def = TestHelpers.MakeUnit(team: Team.Chaos);
        def.CurseTurnsLeft = 2;
        var g = TestHelpers.MakeGame(TestHelpers.MakeState());

        Assert.Equal(7, g.ComputeDamage(atk, def));
    }

    [Fact]
    public void BonusDamage_IsAdded()
    {
        var atk = TestHelpers.MakeUnit(dmg: 5);
        var def = TestHelpers.MakeUnit(team: Team.Chaos);
        var g = TestHelpers.MakeGame(TestHelpers.MakeState());

        Assert.Equal(9, g.ComputeDamage(atk, def, bonusDamage: 4));
    }

    [Fact]
    public void BaseDamageOverride_IsUsed()
    {
        var atk = TestHelpers.MakeUnit(dmg: 5);
        var def = TestHelpers.MakeUnit(team: Team.Chaos);
        var g = TestHelpers.MakeGame(TestHelpers.MakeState());

        Assert.Equal(3, g.ComputeDamage(atk, def, baseDamage: 3));
    }

    [Fact]
    public void AllModifiers_Stack()
    {
        var atk = TestHelpers.MakeUnit(dmg: 5, hp: 10, passives: new[] { Skills.Zeal });
        atk.Hp = 1;
        atk.RageTurnsLeft = 2;
        var def = TestHelpers.MakeUnit(team: Team.Chaos, passives: new[] { Skills.Toughness });
        def.CurseTurnsLeft = 1;
        var g = TestHelpers.MakeGame(TestHelpers.MakeState());

        Assert.Equal(14, g.ComputeDamage(atk, def, bonusDamage: 4));
    }
}