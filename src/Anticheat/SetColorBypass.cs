using VentLib.RPC;

namespace VentLib.Anticheat;

public class SetColorBypass
{
    public static void SetColor(PlayerControl player, int colorId, int clientId = -1)
    {
        var outfit = player.Data.Outfits[PlayerOutfitType.Default];
        outfit.ColorId = colorId;
        GeneralRPC.SendGameData(clientId);
    }
}