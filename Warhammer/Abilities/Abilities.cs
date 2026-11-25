namespace WarhammerMinimal.Abilities;

public static class Skills
{
    public const string Rage = "rage";
    public const string Heal = "heal";
    public const string Smite = "smite";
    public const string BolterFire = "bolter";
    public const string Narthecium = "narthecium";
    public const string Chainsword = "chainsword";
    public const string WarpCurse = "curse";

    public const string Toughness = "toughness";
    public const string Zeal = "zeal";
    public const string Regeneration = "regen";

    public static string Describe(string id) => id switch
    {
        Rage => "Ярость: +3 урона на 2 хода (актив.)",
        Heal => "Исцеление: восстановить 5 HP (актив.)",
        Smite => "Кара: атака на дистанции до 3, +2 урона (актив.)",
        BolterFire => "Огонь болтера: 2 выстрела по цели в радиусе 2, каждый по 3 урона (актив.)",
        Narthecium => "Нартециум: +3 HP всем союзникам в радиусе 2 (актив.)",
        Chainsword => "Цепной меч: удар в упор, +4 урона (актив.)",
        WarpCurse => "Проклятие Варпа: цель получает +2 урона от всех атак на 2 хода (актив.)",
        Toughness => "Стойкость: -1 получаемый урон (пассив.)",
        Zeal => "Рвение: +1 урон при HP < 50% (пассив.)",
        Regeneration => "Регенерация: +2 HP в начале хода (пассив.)",
        _ => id
    };

    public static int ApCost(string id) => id switch
    {
        Smite => 2,
        WarpCurse => 2,
        _ => 1
    };
}