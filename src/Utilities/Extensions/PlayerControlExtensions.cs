using VentLib.Version;

namespace VentLib.Utilities.Extensions;

public static class PlayerControlExtensions
{
    public static bool IsHost(this PlayerControl player) => player != null && player.PlayerId == 0;
    
    public static bool IsModded(this PlayerControl player) => VersionControl.Instance.GetPlayerVersion(player.PlayerId) is not NoVersion;
}