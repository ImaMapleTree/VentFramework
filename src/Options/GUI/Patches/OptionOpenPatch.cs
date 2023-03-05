using System.Linq;
using HarmonyLib;
using UnityEngine;
using VentLib.Utilities.Optionals;

namespace VentLib.Options.GUI.Patches;

[HarmonyPriority(Priority.First)]
[HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Start))]
internal static class OptionOpenPatch
{
    private static UnityOptional<StringOption> template = UnityOptional<StringOption>.Null();

    internal static void Postfix(GameOptionsMenu __instance)
    {
        if (template.Exists()) return;
        template = UnityOptional<StringOption>.Of(Object.FindObjectsOfType<StringOption>().FirstOrDefault());
        
        if (template.Exists() && GameOptionController.Enabled) GameOptionController.HandleOpen();
    }
}