using WarhammerMinimal.Abilities;
using WarhammerMinimal.Models;
using WarhammerMinimal.UI;

namespace WarhammerMinimal.Logic;

public partial class Game
{
    private GameState _state = new();

    internal GameState State
    {
        get => _state;
        set => _state = value;
    }

    internal void SetState(GameState state) => _state = state;

    private const string SavePath = "savegame.json";

    public void Run()
    {
        if (File.Exists(SavePath) && Renderer.PromptYesNo("Найдено сохранение. Загрузить? (y/n): "))
            Load();
        else
            NewGame();

        while (true)
        {
            Console.Clear();
            Renderer.Render(_state);

            if (CheckGameOver()) break;

            Console.WriteLine($"\nХод команды: {_state.CurrentTeam}, ход №{_state.TurnNumber}");
            Console.WriteLine("Действия: [m] move | [a] attack | [s] ability | [e] end turn | [q] save & quit");
            Console.Write("> ");
            var cmd = Console.ReadLine()?.Trim().ToLower();

            try
            {
                switch (cmd)
                {
                    case "m": DoMove(); break;
                    case "a": DoAttack(); break;
                    case "s": DoAbility(); break;
                    case "e": EndTurn(); break;
                    case "q": Save(); Console.WriteLine("Сохранено. Выход."); return;
                    default: Console.WriteLine("Неизвестная команда."); Renderer.Pause(); break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                Renderer.Pause();
            }

            if (_state.Units.Where(u => u.IsAlive && u.Team == _state.CurrentTeam).All(u => u.Ap == 0))
            {
                Console.WriteLine("Все юниты исчерпали AP — ход переходит к противнику.");
                Renderer.Pause();
                EndTurn();
            }
        }

        Console.WriteLine("\nИгра окончена. Нажмите любую клавишу...");
        Console.ReadKey();
        File.Delete(SavePath);
    }

    private void NewGame()
    {
        _state = new GameState { Width = 10, Height = 8 };
        for (int y = 0; y < _state.Height; y++)
        {
            var row = new List<TileKind>();
            for (int x = 0; x < _state.Width; x++) row.Add(TileKind.Empty);
            _state.Tiles.Add(row);
        }

        _state.Units.Add(new Unit
        {
            Name = "Space Marine",
            Team = Team.Imperium,
            X = 1,
            Y = 1,
            Hp = 20,
            MaxHp = 20,
            Damage = 5,
            MoveRange = 2,
            AttackRange = 1,
            ActiveAbilities = new() { Skills.Rage, Skills.BolterFire },
            PassiveAbilities = new() { Skills.Toughness }
        });
        _state.Units.Add(new Unit
        {
            Name = "Apothecary",
            Team = Team.Imperium,
            X = 1,
            Y = 6,
            Hp = 15,
            MaxHp = 15,
            Damage = 3,
            MoveRange = 2,
            AttackRange = 1,
            ActiveAbilities = new() { Skills.Heal, Skills.Narthecium },
            PassiveAbilities = new() { Skills.Regeneration }
        });
        _state.Units.Add(new Unit
        {
            Name = "Chaos Marine",
            Team = Team.Chaos,
            X = 8,
            Y = 6,
            Hp = 18,
            MaxHp = 18,
            Damage = 5,
            MoveRange = 2,
            AttackRange = 1,
            ActiveAbilities = new() { Skills.Rage, Skills.Chainsword },
            PassiveAbilities = new() { Skills.Zeal }
        });
        _state.Units.Add(new Unit
        {
            Name = "Sorcerer",
            Team = Team.Chaos,
            X = 8,
            Y = 1,
            Hp = 14,
            MaxHp = 14,
            Damage = 4,
            MoveRange = 2,
            AttackRange = 2,
            ActiveAbilities = new() { Skills.Smite, Skills.WarpCurse },
            PassiveAbilities = new() { Skills.Toughness }
        });

        var rng = new Random();
        GenerateObstacles(rng);

        _state.CurrentTeam = Team.Imperium;
        _state.TurnNumber = 1;

        Console.WriteLine("Новая игра создана!");
        Renderer.Pause();
    }

    internal void EndTurn()
    {
        _state.CurrentTeam = _state.CurrentTeam == Team.Imperium ? Team.Chaos : Team.Imperium;
        if (_state.CurrentTeam == Team.Imperium) _state.TurnNumber++;

        foreach (var u in _state.Units.Where(x => x.IsAlive && x.Team == _state.CurrentTeam))
        {
            u.Ap = u.MaxAp;
            ApplyPassivesAtTurnStart(u);
        }
    }

    internal void ApplyPassivesAtTurnStart(Unit u)
    {
        if (u.PassiveAbilities.Contains(Skills.Regeneration))
        {
            int healed = Math.Min(2, u.MaxHp - u.Hp);
            u.Hp += healed;
            if (healed > 0) Console.WriteLine($"[Пассивка] {u.Name} регенерирует {healed} HP.");
        }
        if (u.RageTurnsLeft > 0) u.RageTurnsLeft--;
        if (u.CurseTurnsLeft > 0) u.CurseTurnsLeft--;
    }

    private bool CheckGameOver()
    {
        bool impAlive = _state.Units.Any(u => u.IsAlive && u.Team == Team.Imperium);
        bool chAlive = _state.Units.Any(u => u.IsAlive && u.Team == Team.Chaos);
        if (!impAlive) { Console.WriteLine("\nПобеда Хаоса!"); return true; }
        if (!chAlive) { Console.WriteLine("\nПобеда Империума!"); return true; }
        return false;
    }

    internal bool InBounds(int x, int y) => x >= 0 && y >= 0 && x < _state.Width && y < _state.Height;

    internal Unit? SelectOwnUnit()
    {
        var own = _state.Units
            .Where(u => u.IsAlive && u.Team == _state.CurrentTeam && u.Ap > 0)
            .ToList();

        Console.WriteLine("Ваши юниты (с AP):");
        for (int i = 0; i < own.Count; i++)
            Console.WriteLine($"  {i}) {own[i].Name} @({own[i].X},{own[i].Y}) HP {own[i].Hp} AP {own[i].Ap}/{own[i].MaxAp}");
        Console.Write("Выбор: ");
        int idx = int.Parse(Console.ReadLine()!);
        if (idx < 0 || idx >= own.Count) { Console.WriteLine("Неверно."); return null; }
        return own[idx];
    }

    private Unit? SelectEnemyInRange(Unit attacker, int range)
    {
        var enemies = _state.Units
            .Where(u => u.IsAlive && u.Team != attacker.Team &&
                        Math.Abs(u.X - attacker.X) + Math.Abs(u.Y - attacker.Y) <= range)
            .ToList();
        if (enemies.Count == 0) { Console.WriteLine("Целей в радиусе нет."); Renderer.Pause(); return null; }
        Console.WriteLine("Цели:");
        for (int i = 0; i < enemies.Count; i++)
            Console.WriteLine($"  {i}) {enemies[i].Name} @({enemies[i].X},{enemies[i].Y}) HP {enemies[i].Hp}");
        Console.Write("Выбор: ");
        int idx = int.Parse(Console.ReadLine()!);
        if (idx < 0 || idx >= enemies.Count) { Console.WriteLine("Неверно."); return null; }
        return enemies[idx];
    }
}