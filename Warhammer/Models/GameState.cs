namespace WarhammerMinimal.Models;

public class GameState
{
    public int Width { get; set; } = 10;
    public int Height { get; set; } = 8;
    public List<List<TileKind>> Tiles { get; set; } = new();
    public List<Unit> Units { get; set; } = new();
    public Team CurrentTeam { get; set; } = Team.Imperium;
    public int TurnNumber { get; set; } = 1;
}