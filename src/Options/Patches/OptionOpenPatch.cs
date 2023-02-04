using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace VentLib.Options.Patches;

[HarmonyPriority(Priority.First)]
[HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Start))]
internal static class OptionOpenPatch
{
    internal static void Postfix(GameOptionsMenu __instance)
    {
        if (!OptionClosePatch.Closed) return;
        var template = Object.FindObjectsOfType<StringOption>().FirstOrDefault();
        if (template == null) return;
        Option.StringOption = template;
        OptionClosePatch.Closed = false;
    }
}