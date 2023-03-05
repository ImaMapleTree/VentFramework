using System.Text.Json.Serialization;

namespace VentLib.Lobbies;

public class ModdedLobby
{
    [JsonPropertyName("game_id")]
    public int GameId { get; set; }
    
    [JsonPropertyName("version")]
    public string Version { get; set; } = null!;

    [JsonPropertyName("mod_name")]
    public string Mod { get; set; } = null!;
    
    [JsonPropertyName("host")]
    public string Host { get; set; } = null!;
    
    [JsonPropertyName("created")]
    public ulong CreationTime { get; set; }

    public override string ToString()
    {
        return $"ModdedLobby({nameof(GameId)}: {GameId}, {nameof(Version)}: {Version}, {nameof(CreationTime)}: {CreationTime})";
    }
}
