using WarhammerMinimal;
using WarhammerMinimal.Logic;
using WarhammerMinimal.Models;

namespace WarhammerMinimal.Tests;

internal static class TestHelpers
{
    public static GameState MakeState(int w = 10, int h = 8)
    {
        var s = new GameState { Width = w, Height = h };
        for (int y = 0; y < h; y++)
        {
            var row = new List<TileKind>();
            for (int x = 0; x < w; x++) row.Add(TileKind.Empty);
            s.Tiles.Add(row);
        }
        return s;
    }

    public static Unit MakeUnit(
        Team team = Team.Imperium,
        int x = 0, int y = 0,
        int hp = 10, int dmg = 5,
        int move = 2, int attackRange = 1,
        IEnumerable<string>? actives = null,
        IEnumerable<string>? passives = null)
    {
        return new Unit
        {
            Name = $"{team}Unit",
            Team = team,
            X = x,
            Y = y,
            Hp = hp,
            MaxHp = hp,
            Damage = dmg,
            MoveRange = move,
            AttackRange = attackRange,
            MaxAp = 2,
            Ap = 2,
            ActiveAbilities = actives?.ToList() ?? new(),
            PassiveAbilities = passives?.ToList() ?? new()
        };
    }

    public static Game MakeGame(GameState state)
    {
        var g = new Game();
        g.SetState(state);
        return g;
    }

    public static void PutObstacle(GameState s, int x, int y) => s.Tiles[y][x] = TileKind.Obstacle;
}