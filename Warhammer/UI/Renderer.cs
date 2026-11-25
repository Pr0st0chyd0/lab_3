using WarhammerMinimal.Models;

namespace WarhammerMinimal.UI;

public static class Renderer
{
    public static void Render(GameState state)
    {
        Console.WriteLine($"Поле {state.Width}x{state.Height}");
        for (int y = 0; y < state.Height; y++)
        {
            for (int x = 0; x < state.Width; x++)
            {
                var u = state.Units.FirstOrDefault(u => u.IsAlive && u.X == x && u.Y == y);
                if (u != null)
                {
                    char c = u.Team == Team.Imperium ? 'I' : 'C';
                    c = u.Team == state.CurrentTeam ? char.ToUpper(c) : char.ToLower(c);
                    Console.Write($" {c} ");
                }
                else if (state.Tiles[y][x] == TileKind.Obstacle) Console.Write(" # ");
                else Console.Write(" . ");
            }
            Console.WriteLine();
        }

        Console.WriteLine("\nЮниты:");
        foreach (Unit u in state.Units.Where(u => u.IsAlive))
        {
            string statuses = "";
            if (u.RageTurnsLeft > 0) statuses += $" [ярость:{u.RageTurnsLeft}]";
            if (u.CurseTurnsLeft > 0) statuses += $" [проклят:{u.CurseTurnsLeft}]";
            Console.WriteLine($"  [{u.Id.Substring(0, 4)}] {u.Name} ({u.Team}) " +
                              $"HP {u.Hp}/{u.MaxHp} AP {u.Ap}/{u.MaxAp} " +
                              $"@({u.X},{u.Y}) dmg={u.Damage}{statuses}");
        }
    }

    public static void RenderMoveMode(GameState state, Unit u, HashSet<(int, int)> reachable, int cx, int cy)
    {
        Console.WriteLine($"Перемещение: {u.Name} @({u.X},{u.Y}), дальность {u.MoveRange}, AP {u.Ap}");
        for (int y = 0; y < state.Height; y++)
        {
            for (int x = 0; x < state.Width; x++)
            {
                bool isCursor = (x == cx && y == cy);
                var other = state.Units.FirstOrDefault(o => o.IsAlive && o.X == x && o.Y == y);
                if (other != null)
                {
                    char c = other.Id == u.Id ? '@' : (other.Team == Team.Imperium ? 'i' : 'c');
                    if (isCursor) Console.BackgroundColor = ConsoleColor.DarkYellow;
                    else if (other.Id == u.Id) Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($" {c} ");
                    Console.ResetColor();
                }
                else if (isCursor)
                {
                    bool ok = reachable.Contains((x, y));
                    Console.BackgroundColor = ok ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed;
                    Console.Write("   ");
                    Console.ResetColor();
                }
                else if (reachable.Contains((x, y)))
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write(" * ");
                    Console.ResetColor();
                }
                else if (state.Tiles[y][x] == TileKind.Obstacle) Console.Write(" # ");
                else Console.Write(" . ");
            }
            Console.WriteLine();
        }
    }

    public static void Pause()
    {
        Console.WriteLine("Нажмите Enter...");
        Console.ReadLine();
    }

    public static bool PromptYesNo(string msg)
    {
        Console.Write(msg);
        var s = Console.ReadLine()?.Trim().ToLower();
        return s == "y" || s == "yes" || s == "д" || s == "да";
    }
}