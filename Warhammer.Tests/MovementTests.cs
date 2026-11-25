using WarhammerMinimal.Models;
using Xunit;

namespace WarhammerMinimal.Tests;

public class MovementTests
{
    [Fact]
    public void OpenField_Range2_ReachesManhattanDiamond()
    {
        var s = TestHelpers.MakeState(10, 8);
        var u = TestHelpers.MakeUnit(Team.Imperium, x: 5, y: 5, move: 2);
        s.Units.Add(u);
        var g = TestHelpers.MakeGame(s);

        var r = g.ComputeReachable(u);

        Assert.Equal(12, r.Count);
        Assert.Contains((5, 3), r);
        Assert.Contains((7, 5), r);
        Assert.Contains((5, 7), r);
        Assert.Contains((3, 5), r);
        Assert.DoesNotContain((5, 5), r);
        Assert.DoesNotContain((8, 5), r);
    }

    [Fact]
    public void Obstacle_BlocksDirectPath_OnRange2()
    {
        var s = TestHelpers.MakeState(10, 8);
        var u = TestHelpers.MakeUnit(Team.Imperium, x: 1, y: 1, move: 2);
        s.Units.Add(u);
        TestHelpers.PutObstacle(s, 2, 1);
        var g = TestHelpers.MakeGame(s);

        var r = g.ComputeReachable(u);

        Assert.DoesNotContain((3, 1), r);
        Assert.DoesNotContain((3, 0), r);
        Assert.Contains((1, 0), r);
        Assert.Contains((1, 2), r);
        Assert.Contains((0, 1), r);
    }

    [Fact]
    public void Obstacle_CanBeBypassed_WithinRange()
    {
        var s = TestHelpers.MakeState(10, 8);
        var u = TestHelpers.MakeUnit(Team.Imperium, x: 1, y: 1, move: 2);
        s.Units.Add(u);
        TestHelpers.PutObstacle(s, 2, 1);
        var g = TestHelpers.MakeGame(s);

        var r = g.ComputeReachable(u);

        Assert.Contains((2, 0), r);
    }

    [Fact]
    public void Unit_BlocksPassage()
    {
        var s = TestHelpers.MakeState(10, 8);
        var u = TestHelpers.MakeUnit(Team.Imperium, x: 1, y: 1, move: 2);
        var blocker = TestHelpers.MakeUnit(Team.Imperium, x: 2, y: 1);
        s.Units.Add(u);
        s.Units.Add(blocker);
        var g = TestHelpers.MakeGame(s);

        var r = g.ComputeReachable(u);

        Assert.DoesNotContain((2, 1), r);
        Assert.DoesNotContain((3, 1), r);
    }

    [Fact]
    public void NoReachableCells_WhenSurroundedByObstacles()
    {
        var s = TestHelpers.MakeState(5, 5);
        var u = TestHelpers.MakeUnit(Team.Imperium, x: 2, y: 2, move: 2);
        s.Units.Add(u);
        TestHelpers.PutObstacle(s, 2, 1);
        TestHelpers.PutObstacle(s, 2, 3);
        TestHelpers.PutObstacle(s, 1, 2);
        TestHelpers.PutObstacle(s, 3, 2);
        var g = TestHelpers.MakeGame(s);

        var r = g.ComputeReachable(u);

        Assert.Empty(r);
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(-1, 0)]
    [InlineData(0, 1)]
    [InlineData(0, -1)]
    public void AdjacentCells_AreReachable(int dx, int dy)
    {
        var s = TestHelpers.MakeState(5, 5);
        var u = TestHelpers.MakeUnit(Team.Imperium, x: 2, y: 2, move: 1);
        s.Units.Add(u);
        var g = TestHelpers.MakeGame(s);

        var r = g.ComputeReachable(u);

        Assert.Contains((2 + dx, 2 + dy), r);
    }
}