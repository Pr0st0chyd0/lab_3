using WarhammerMinimal.Models;
using Xunit;

namespace WarhammerMinimal.Tests;

public class ObstacleGeneratorTests
{
    private static GameState NewGameStateWithFourUnits()
    {
        var s = TestHelpers.MakeState(10, 8);
        s.Units.Add(TestHelpers.MakeUnit(Team.Imperium, x: 1, y: 1));
        s.Units.Add(TestHelpers.MakeUnit(Team.Imperium, x: 1, y: 6));
        s.Units.Add(TestHelpers.MakeUnit(Team.Chaos, x: 8, y: 1));
        s.Units.Add(TestHelpers.MakeUnit(Team.Chaos, x: 8, y: 6));
        return s;
    }

    [Fact]
    public void Obstacles_NeverOnUnitStartPositions()
    {
        for (int seed = 0; seed < 50; seed++)
        {
            var s = NewGameStateWithFourUnits();
            var g = TestHelpers.MakeGame(s);
            g.GenerateObstacles(new Random(seed));

            foreach (var u in s.Units)
                Assert.NotEqual(TileKind.Obstacle, s.Tiles[u.Y][u.X]);
        }
    }

    [Fact]
    public void Obstacles_KeepAllUnitsConnected()
    {
        for (int seed = 0; seed < 50; seed++)
        {
            var s = NewGameStateWithFourUnits();
            var g = TestHelpers.MakeGame(s);
            g.GenerateObstacles(new Random(seed));

            Assert.True(g.AllUnitsConnected(), $"Seed {seed}: карта разрезана");
        }
    }

    [Fact]
    public void Obstacles_DensityIsReasonable()
    {
        var s = NewGameStateWithFourUnits();
        var g = TestHelpers.MakeGame(s);
        g.GenerateObstacles(new Random(42));

        int count = s.Tiles.Sum(row => row.Count(t => t == TileKind.Obstacle));
        Assert.InRange(count, 3, 15);
    }

    [Fact]
    public void SameSeed_ProducesSameLayout()
    {
        var s1 = NewGameStateWithFourUnits();
        TestHelpers.MakeGame(s1).GenerateObstacles(new Random(7));

        var s2 = NewGameStateWithFourUnits();
        TestHelpers.MakeGame(s2).GenerateObstacles(new Random(7));

        for (int y = 0; y < s1.Height; y++)
            for (int x = 0; x < s1.Width; x++)
                Assert.Equal(s1.Tiles[y][x], s2.Tiles[y][x]);
    }
}