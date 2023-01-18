using System.Linq;
using InnerNet;

namespace VentLib.Utilities;

public static class NetUtils
{
    public static ClientData? GetClient(this PlayerControl player)
    {
        var client = AmongUsClient.Instance.allClients.ToArray().FirstOrDefault(cd => cd.Character.PlayerId == player.PlayerId);
        return client;
    }
    public static int GetClientId(this PlayerControl player)
    {
        var client = player.GetClient();
        return client?.Id ?? -1;
    }
    
    // ReSharper disable once CompareOfFloatsByEqualityOperator
    public static float DeriveDelay(float flatDelay = float.MinValue) => AmongUsClient.Instance.Ping * 0.0003f + (flatDelay == float.MinValue ? 0.4f : flatDelay);
}