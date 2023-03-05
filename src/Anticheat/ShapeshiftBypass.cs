using VentLib.Utilities;

namespace VentLib.Anticheat;

public class ShapeshiftBypass
{
    public static void Shapeshift(PlayerControl player, PlayerControl target, bool animate)
    {
        //player.RpcSetRole(RoleTypes.Shapeshifter);
        Async.Schedule(() => player.RpcShapeshift(target, animate), NetUtils.DeriveDelay(0.1f));
    }

    public static void RevertShapeshift(PlayerControl player, bool animate)
    {
        //player.RpcSetRole(RoleTypes.Shapeshifter);
        Async.Schedule(() => player.RpcRevertShapeshift(animate), NetUtils.DeriveDelay(0.1f));
    }
}