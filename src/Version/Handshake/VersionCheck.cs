using System.Linq;
using HarmonyLib;
using VentLib.Logging;
using VentLib.RPC;
using VentLib.RPC.Attributes;
using VentLib.Utilities;
using VentLib.Version.Handshake.Patches;

namespace VentLib.Version.Handshake;

public class VersionCheck
{
    // This is the sender version of this Rpc. In order to fully utilize it you must make your own handler.
    [VentRPC(VentCall.VersionCheck, RpcActors.Host, RpcActors.NonHosts)]
    public static void RequestVersion()
    {
        SendVersion(VersionControl.Instance.Version ?? new NoVersion());
    }

    [VentRPC(VentCall.VersionCheck, RpcActors.NonHosts, RpcActors.LastSender)]
    public static void SendVersion(Version version)
    {
        PlayerControl? lastSender = Vents.GetLastSender((uint)VentCall.VersionCheck);
        VentLogger.Info($"Received Version: \"{version}\" from Player {lastSender?.Data?.PlayerName}", "VentLib");
        VersionControl vc = VersionControl.Instance;

        if (lastSender != null)
            PlayerJoinPatch.WaitSet.Remove(lastSender.GetClientId());
        
        HandshakeResult action = vc.HandshakeFilter!.Invoke(version);
        vc.VersionHandles
            .Where(h => h.Item1.HasFlag(action is HandshakeResult.PassDoNothing ? ReceiveExecutionFlag.OnSuccessfulHandshake : ReceiveExecutionFlag.OnFailedHandshake))
            .Do(h => h.Item2.Invoke(version, lastSender!));
        
        HandleAction(action, lastSender);
    }

    private static void HandleAction(HandshakeResult action, PlayerControl? player)
    {
        if (player == null) return;
        switch (action)
        {
            case HandshakeResult.DisableRPC:
                Vents.BlockClient(Vents.RootAssemby, player.GetClientId());
                break;
            case HandshakeResult.Kick:
                AmongUsClient.Instance.KickPlayer(player.GetClientId(), false);
                break;
            case HandshakeResult.FailDoNothing:
            case HandshakeResult.PassDoNothing:
            default:
                break;
        }
        
    }
}