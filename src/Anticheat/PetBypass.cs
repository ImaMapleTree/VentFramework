using VentLib.RPC;
using VentLib.Utilities;

namespace VentLib.Anticheat;

/// <summary>
/// Utility command which bypasses the anti-cheat when setting other player's pets. This should only be used
/// for host only circumstances because otherwise you can just create your own RPCs.
/// </summary>
public static class PetBypass
{
    /// <summary>
    /// Sets the target player's pet to the given pet id
    /// <br></br>
    /// <b>=======[ WARNING ]=======</b> <br></br>
    /// If applyNow is enabled the anticheat may kick when the player shapeshifts.
    /// This will / will not happen based on how your game is setup prior to this call.
    /// The anticheat will not kick for shapeshifting if the target player is made a shapeshifter prior to this call.
    /// </summary>
    /// <param name="player">Target player to set pet for</param>
    /// <param name="petId">ID of pet</param>
    /// <param name="applyNow">Whether to immediately apply the pet change in game (forces the player to shapeshift for a second)</param>
    public static void SetPet(PlayerControl player, string petId, bool applyNow = false)
    {
        if (player.AmOwner) {
            player.SetPet(petId);
            return;
        }

        var outfit = player.Data.Outfits[PlayerOutfitType.Default];
        outfit.PetId = petId;
        GeneralRPC.SendGameData(player.GetClientId());
        
        if (!applyNow) return;
        ShapeshiftBypass.Shapeshift(player, player, false);
        Async.ScheduleInStep(() => ShapeshiftBypass.RevertShapeshift(player, false), NetUtils.DeriveDelay(0.2f));
    }
}