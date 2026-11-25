using WarhammerMinimal;
using WarhammerMinimal.Abilities;
using WarhammerMinimal.Models;
using Xunit;

namespace WarhammerMinimal.Tests;

public class TurnTests
{
    [Fact]
    public void EndTurn_SwitchesTeam()
    {
        var s = TestHelpers.MakeState();
        s.Units.Add(TestHelpers.MakeUnit(Team.Imperium));
        s.Units.Add(TestHelpers.MakeUnit(Team.Chaos));
        s.CurrentTeam = Team.Imperium;
        var g = TestHelpers.MakeGame(s);

        g.EndTurn();

        Assert.Equal(Team.Chaos, g.State.CurrentTeam);
    }

    [Fact]
    public void EndTurn_RestoresAp_ForNewTeamOnly()
    {
        var s = TestHelpers.MakeState();
        var imp = TestHelpers.MakeUnit(Team.Imperium);
        var ch = TestHelpers.MakeUnit(Team.Chaos);
        imp.Ap = 0;
        ch.Ap = 0;
        s.Units.Add(imp);
        s.Units.Add(ch);
        s.CurrentTeam = Team.Imperium;
        var g = TestHelpers.MakeGame(s);

        g.EndTurn();

        Assert.Equal(2, ch.Ap);
        Assert.Equal(0, imp.Ap);
    }

    [Fact]
    public void EndTurn_IncrementsTurnNumber_OnFullCycle()
    {
        var s = TestHelpers.MakeState();
        s.Units.Add(TestHelpers.MakeUnit(Team.Imperium));
        s.Units.Add(TestHelpers.MakeUnit(Team.Chaos));
        s.CurrentTeam = Team.Imperium;
        s.TurnNumber = 1;
        var g = TestHelpers.MakeGame(s);

        g.EndTurn();
        Assert.Equal(1, g.State.TurnNumber);

        g.EndTurn();
        Assert.Equal(2, g.State.TurnNumber);
    }

    [Fact]
    public void RageTicks_OnOwnerTurnStart()
    {
        var s = TestHelpers.MakeState();
        var imp = TestHelpers.MakeUnit(Team.Imperium);
        imp.RageTurnsLeft = 2;
        s.Units.Add(imp);
        s.Units.Add(TestHelpers.MakeUnit(Team.Chaos));
        s.CurrentTeam = Team.Chaos;
        var g = TestHelpers.MakeGame(s);

        g.EndTurn();

        Assert.Equal(1, imp.RageTurnsLeft);
    }

    [Fact]
    public void Regeneration_HealsTwo_AtTurnStart()
    {
        var s = TestHelpers.MakeState();
        var imp = TestHelpers.MakeUnit(Team.Imperium, hp: 10, passives: new[] { Skills.Regeneration });
        imp.Hp = 5;
        s.Units.Add(imp);
        s.Units.Add(TestHelpers.MakeUnit(Team.Chaos));
        s.CurrentTeam = Team.Chaos;
        var g = TestHelpers.MakeGame(s);

        g.EndTurn();

        Assert.Equal(7, imp.Hp);
    }

    [Fact]
    public void Regeneration_DoesNotExceedMaxHp()
    {
        var s = TestHelpers.MakeState();
        var imp = TestHelpers.MakeUnit(Team.Imperium, hp: 10, passives: new[] { Skills.Regeneration });
        imp.Hp = 9;
        s.Units.Add(imp);
        s.Units.Add(TestHelpers.MakeUnit(Team.Chaos));
        s.CurrentTeam = Team.Chaos;
        var g = TestHelpers.MakeGame(s);

        g.EndTurn();

        Assert.Equal(10, imp.Hp);
    }
}