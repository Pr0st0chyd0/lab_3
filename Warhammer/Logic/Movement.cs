using WarhammerMinimal.Models;
using WarhammerMinimal.UI;

namespace WarhammerMinimal.Logic;

public partial class Game
{
    private void DoMove()
    {
        var u = SelectOwnUnit();
        if (u == null) return;

        var reachable = ComputeReachable(u);

        if (reachable.Count == 0)
        {
            Console.WriteLine("Нет доступных клеток для перемещения.");
            Renderer.Pause();
            return;
        }

        int cx = u.X, cy = u.Y;

        while (true)
        {
            Console.Clear();
            Renderer.RenderMoveMode(_state, u, reachable, cx, cy);
            Console.WriteLine("\nСтрелки — курсор, Enter — подтвердить, Esc — отмена.");
            var key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow: if (cy > 0) cy--; break;
                case ConsoleKey.DownArrow: if (cy < _state.Height - 1) cy++; break;
                case ConsoleKey.LeftArrow: if (cx > 0) cx--; break;
                case ConsoleKey.RightArrow: if (cx < _state.Width - 1) cx++; break;
                case ConsoleKey.Escape: return;
                case ConsoleKey.Enter:
                    if (cx == u.X && cy == u.Y) { Console.WriteLine("Курсор на исходной позиции."); Renderer.Pause(); continue; }
                    if (!reachable.Contains((cx, cy))) { Console.WriteLine("Клетка недоступна."); Renderer.Pause(); continue; }
                    u.X = cx; u.Y = cy;
                    u.Ap -= 1;
                    Console.WriteLine($"Перемещение в ({cx},{cy}). AP {u.Name}: {u.Ap}/{u.MaxAp}");
                    Renderer.Pause();
                    return;
            }
        }
    }

    internal HashSet<(int, int)> ComputeReachable(Unit u)
    {
        var visited = new HashSet<(int, int)>();
        var result = new HashSet<(int, int)>();
        var queue = new Queue<(int x, int y, int dist)>();
        queue.Enqueue((u.X, u.Y, 0));
        visited.Add((u.X, u.Y));

        var dirs = new (int dx, int dy)[] { (0, -1), (0, 1), (-1, 0), (1, 0) };

        while (queue.Count > 0)
        {
            var (x, y, dist) = queue.Dequeue();
            if (dist >= u.MoveRange) continue;

            foreach (var (dx, dy) in dirs)
            {
                int nx = x + dx, ny = y + dy;
                if (!InBounds(nx, ny)) continue;
                if (visited.Contains((nx, ny))) continue;
                if (_state.Tiles[ny][nx] == TileKind.Obstacle) continue;

                if (_state.Units.Any(o => o.IsAlive && o.X == nx && o.Y == ny)) continue;

                visited.Add((nx, ny));
                result.Add((nx, ny));
                queue.Enqueue((nx, ny, dist + 1));
            }
        }

        return result;
    }
}