using System.Text.Json.Serialization;

namespace WarhammerMinimal.Models;

public class Unit
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "";
    public Team Team { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Hp { get; set; }
    public int MaxHp { get; set; }
    public int Damage { get; set; }
    public int MoveRange { get; set; } = 2;
    public int AttackRange { get; set; } = 1;

    public int MaxAp { get; set; } = 2;
    public int Ap { get; set; } = 2;

    public List<string> ActiveAbilities { get; set; } = new();
    public List<string> PassiveAbilities { get; set; } = new();

    public int RageTurnsLeft { get; set; } = 0;
    public int CurseTurnsLeft { get; set; } = 0;

    [JsonIgnore] public bool IsAlive => Hp > 0;
}