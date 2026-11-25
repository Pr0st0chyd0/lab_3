using WarhammerMinimal.Abilities;
using WarhammerMinimal.Models;
using WarhammerMinimal.UI;

namespace WarhammerMinimal.Logic;

public partial class Game
{
    private void DoAttack()
    {
        var attacker = SelectOwnUnit();
        if (attacker == null) return;
        var target = SelectEnemyInRange(attacker, attacker.AttackRange);
        if (target == null) return;

        int dmg = ComputeDamage(attacker, target);
        target.Hp -= dmg;
        Console.WriteLine($"{attacker.Name} наносит {dmg} урона {target.Name}. HP: {Math.Max(0, target.Hp)}");
        if (!target.IsAlive) Console.WriteLine($"{target.Name} уничтожен!");

        attacker.Ap -= 1;
        Console.WriteLine($"AP {attacker.Name}: {attacker.Ap}/{attacker.MaxAp}");
        Renderer.Pause();
    }

    internal int ComputeDamage(Unit attacker, Unit target, int? baseDamage = null, int bonusDamage = 0)
    {
        int dmg = baseDamage ?? attacker.Damage;
        if (attacker.RageTurnsLeft > 0) dmg += 3;
        if (attacker.PassiveAbilities.Contains(Skills.Zeal) && attacker.Hp < attacker.MaxHp / 2) dmg += 1;
        dmg += bonusDamage;
        if (target.CurseTurnsLeft > 0) dmg += 2;
        if (target.PassiveAbilities.Contains(Skills.Toughness)) dmg = Math.Max(1, dmg - 1);
        return Math.Max(1, dmg);
    }
}