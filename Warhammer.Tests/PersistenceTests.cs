using System.Text.Json;
using WarhammerMinimal.Models;
using Xunit;

namespace WarhammerMinimal.Tests;

public class PersistenceTests
{
    [Fact]
    public void Serialize_Deserialize_RoundTripsFullState()
    {
        var s = TestHelpers.MakeState(6, 4);
        var u = TestHelpers.MakeUnit(Team.Imperium, x: 2, y: 1, hp: 7, dmg: 4,
                                     actives: new[] { "rage" }, passives: new[] { "toughness" });
        u.Ap = 1;
        u.RageTurnsLeft = 1;
        u.CurseTurnsLeft = 2;
        s.Units.Add(u);
        s.CurrentTeam = Team.Chaos;
        s.TurnNumber = 5;

        var json = JsonSerializer.Serialize(s);
        var back = JsonSerializer.Deserialize<GameState>(json)!;

        Assert.Equal(s.Width, back.Width);
        Assert.Equal(s.Height, back.Height);
        Assert.Equal(s.CurrentTeam, back.CurrentTeam);
        Assert.Equal(s.TurnNumber, back.TurnNumber);
        Assert.Single(back.Units);

        var bu = back.Units[0];
        Assert.Equal(u.Id, bu.Id);
        Assert.Equal(u.X, bu.X);
        Assert.Equal(u.Y, bu.Y);
        Assert.Equal(u.Hp, bu.Hp);
        Assert.Equal(u.Ap, bu.Ap);
        Assert.Equal(u.RageTurnsLeft, bu.RageTurnsLeft);
        Assert.Equal(u.CurseTurnsLeft, bu.CurseTurnsLeft);
        Assert.Equal(u.ActiveAbilities, bu.ActiveAbilities);
        Assert.Equal(u.PassiveAbilities, bu.PassiveAbilities);
    }

    [Fact]
    public void IsAlive_IsNotSerialized()
    {
        var u = TestHelpers.MakeUnit(hp: 5);
        var json = JsonSerializer.Serialize(u);

        Assert.DoesNotContain("IsAlive", json);
        Assert.DoesNotContain("\"IsAlive\"", json);
    }

    [Fact]
    public void IsAlive_ComputedFromHp_AfterDeserialize()
    {
        var s = TestHelpers.MakeState(3, 3);
        var dead = TestHelpers.MakeUnit(Team.Chaos);
        dead.Hp = 0;
        s.Units.Add(dead);

        var json = JsonSerializer.Serialize(s);
        var back = JsonSerializer.Deserialize<GameState>(json)!;

        Assert.False(back.Units[0].IsAlive);

        var alive = TestHelpers.MakeUnit(Team.Chaos);
        alive.Hp = 1;
        var json2 = JsonSerializer.Serialize(new GameState { Units = new List<Unit> { alive } });
        var back2 = JsonSerializer.Deserialize<GameState>(json2)!;
        Assert.True(back2.Units[0].IsAlive);
    }
}