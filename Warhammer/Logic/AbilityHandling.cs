using WarhammerMinimal.Abilities;
using WarhammerMinimal.UI;

namespace WarhammerMinimal.Logic;

public partial class Game
{
    private void DoAbility()
    {
        var u = SelectOwnUnit();
        if (u == null) return;
        if (u.ActiveAbilities.Count == 0) { Console.WriteLine("Нет активных способностей."); Renderer.Pause(); return; }

        Console.WriteLine($"AP: {u.Ap}/{u.MaxAp}. Способности:");
        for (int i = 0; i < u.ActiveAbilities.Count; i++)
        {
            int cost = Skills.ApCost(u.ActiveAbilities[i]);
            string afford = cost > u.Ap ? " [не хватает AP]" : "";
            Console.WriteLine($"  {i}) ({cost} AP) {Skills.Describe(u.ActiveAbilities[i])}{afford}");
        }
        Console.Write("Выбор: ");
        int idx = int.Parse(Console.ReadLine()!);
        if (idx < 0 || idx >= u.ActiveAbilities.Count) throw new Exception("Неверный выбор.");
        string ab = u.ActiveAbilities[idx];

        int apCost = Skills.ApCost(ab);
        if (u.Ap < apCost)
        {
            Console.WriteLine($"Не хватает AP: нужно {apCost}, есть {u.Ap}.");
            Renderer.Pause();
            return;
        }

        switch (ab)
        {
            case Skills.Rage:
                u.RageTurnsLeft = 2;
                Console.WriteLine($"{u.Name} впадает в ярость!");
                break;

            case Skills.Heal:
                {
                    int healed = Math.Min(5, u.MaxHp - u.Hp);
                    u.Hp += healed;
                    Console.WriteLine($"{u.Name} лечится на {healed} HP.");
                    break;
                }

            case Skills.Smite:
                {
                    var target = SelectEnemyInRange(u, 3);
                    if (target == null) return;
                    int smiteDmg = ComputeDamage(u, target) + 2;
                    target.Hp -= smiteDmg;
                    Console.WriteLine($"Кара наносит {smiteDmg} урона {target.Name}.");
                    if (!target.IsAlive) Console.WriteLine($"{target.Name} уничтожен!");
                    break;
                }

            case Skills.BolterFire:
                {
                    var target = SelectEnemyInRange(u, 2);
                    if (target == null) return;
                    int totalDmg = 0;
                    for (int shot = 0; shot < 2 && target.IsAlive; shot++)
                    {
                        int dmg = ComputeDamage(u, target, baseDamage: 3);
                        target.Hp -= dmg;
                        totalDmg += dmg;
                        Console.WriteLine($"Выстрел {shot + 1}: {dmg} урона {target.Name} (HP {Math.Max(0, target.Hp)})");
                    }
                    Console.WriteLine($"Итого {totalDmg} урона.");
                    if (!target.IsAlive) Console.WriteLine($"{target.Name} уничтожен!");
                    break;
                }

            case Skills.Narthecium:
                {
                    var allies = _state.Units
                        .Where(a => a.IsAlive && a.Team == u.Team &&
                                    Math.Abs(a.X - u.X) + Math.Abs(a.Y - u.Y) <= 2)
                        .ToList();
                    int healedTotal = 0;
                    foreach (var a in allies)
                    {
                        int h = Math.Min(3, a.MaxHp - a.Hp);
                        a.Hp += h;
                        healedTotal += h;
                        Console.WriteLine($"{a.Name}: +{h} HP (HP {a.Hp}/{a.MaxHp})");
                    }
                    Console.WriteLine($"Нартециум восстановил {healedTotal} HP у {allies.Count} юнит(ов).");
                    break;
                }

            case Skills.Chainsword:
                {
                    var target = SelectEnemyInRange(u, 1);
                    if (target == null) return;
                    int dmg = ComputeDamage(u, target, bonusDamage: 4);
                    target.Hp -= dmg;
                    Console.WriteLine($"Цепной меч: {dmg} урона {target.Name} (HP {Math.Max(0, target.Hp)})");
                    if (!target.IsAlive) Console.WriteLine($"{target.Name} уничтожен!");
                    break;
                }

            case Skills.WarpCurse:
                {
                    var target = SelectEnemyInRange(u, 3);
                    if (target == null) return;
                    target.CurseTurnsLeft = 2;
                    Console.WriteLine($"{target.Name} проклят Варпом! +2 урона на 2 хода.");
                    break;
                }
        }

        u.Ap -= apCost;
        Console.WriteLine($"Способность применена. AP {u.Name}: {u.Ap}/{u.MaxAp}");
        Renderer.Pause();
    }
}