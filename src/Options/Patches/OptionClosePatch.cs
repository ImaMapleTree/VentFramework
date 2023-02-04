using System.Linq;
using HarmonyLib;
using VentLib.Utilities;

namespace VentLib.Options.Patches;

[HarmonyPriority(Priority.First)]
[HarmonyPatch(typeof(ControllerManager), nameof(ControllerManager.CloseOverlayMenu))]
internal class OptionClosePatch
{
    internal static bool Closed;
    internal static void Prefix(ControllerManager __instance, [HarmonyArgument(0)] string menuName)
    {
        if (menuName != "PlayerOptionsMenu(Clone)") return;
        Closed = true;
        Async.Schedule(() =>
        {
            OptionManager.AllOptions.Do(o => o.SkipRender = false);
            OptionManager.Managers.Values.SelectMany(m => m.Options).Do(o =>
            {
                o.SkipRender = false;
                OptionManager.NewRegisters.Add(o);
            });
        }, 0.5f);
        
    }
}