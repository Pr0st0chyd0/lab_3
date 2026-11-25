using WarhammerMinimal.Models;

namespace WarhammerMinimal.Logic;

public partial class Game
{
    private const double ObstacleDensity = 0.15;
    internal void GenerateObstacles(Random rng)
    {
        int w = _state.Width, h = _state.Height;

        var forbidden = new HashSet<(int, int)>();
        foreach (var u in _state.Units)
        {
            for (int dy = -1; dy <= 1; dy++)
                for (int dx = -1; dx <= 1; dx++)
                {
                    int nx = u.X + dx, ny = u.Y + dy;
                    if (nx >= 0 && ny >= 0 && nx < w && ny < h)
                        forbidden.Add((nx, ny));
                }
        }

        var candidates = new List<(int x, int y)>();
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
                if (!forbidden.Contains((x, y)))
                    candidates.Add((x, y));

        Shuffle(candidates, rng);
        int target = (int)Math.Round(candidates.Count * ObstacleDensity);
        int placed = 0;

        foreach (var (x, y) in candidates)
        {
            if (placed >= target) break;

            _state.Tiles[y][x] = TileKind.Obstacle;

            if (!AllUnitsConnected())
            {
                _state.Tiles[y][x] = TileKind.Empty;
            }
            else
            {
                placed++;
            }
        }
    }

    internal bool AllUnitsConnected()
    {
        if (_state.Units.Count == 0) return true;

        var start = (_state.Units[0].X, _state.Units[0].Y);
        var reachable = BfsFreeCells(start);

        foreach (var u in _state.Units)
        {
            if (_state.Tiles[u.Y][u.X] == TileKind.Obstacle) return false;
            if (!reachable.Contains((u.X, u.Y))) return false;
        }
        return true;
    }

    internal HashSet<(int, int)> BfsFreeCells((int x, int y) start)
    {
        var visited = new HashSet<(int, int)>();
        var queue = new Queue<(int x, int y)>();
        if (_state.Tiles[start.y][start.x] == TileKind.Obstacle) return visited;

        queue.Enqueue(start);
        visited.Add(start);

        var dirs = new (int dx, int dy)[] { (0, -1), (0, 1), (-1, 0), (1, 0) };
        while (queue.Count > 0)
        {
            var (x, y) = queue.Dequeue();
            foreach (var (dx, dy) in dirs)
            {
                int nx = x + dx, ny = y + dy;
                if (nx < 0 || ny < 0 || nx >= _state.Width || ny >= _state.Height) continue;
                if (visited.Contains((nx, ny))) continue;
                if (_state.Tiles[ny][nx] == TileKind.Obstacle) continue;
                visited.Add((nx, ny));
                queue.Enqueue((nx, ny));
            }
        }
        return visited;
    }

    internal static void Shuffle<T>(IList<T> list, Random rng)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}