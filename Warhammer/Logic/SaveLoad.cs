using System.Text.Json;
using WarhammerMinimal.Models;
using WarhammerMinimal.UI;

namespace WarhammerMinimal.Logic;

public partial class Game
{
    private void Save()
    {
        var opts = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(SavePath, JsonSerializer.Serialize(_state, opts));
    }

    private void Load()
    {
        var json = File.ReadAllText(SavePath);
        _state = JsonSerializer.Deserialize<GameState>(json) ?? new GameState();
        Console.WriteLine("Загружено.");
        Renderer.Pause();
    }
}