using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using VentLib.Utilities.Extensions;
using VentLib.Utilities.Harmony.Attributes;

namespace VentLib.Commands.Patches;

[HarmonyPriority(Priority.First)]
[HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
internal static class AddChatPatch
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal static void Prefix(ChatController __instance, PlayerControl sourcePlayer, string chatText)
    {
        if (!AmongUsClient.Instance.AmHost) return;
        if (!chatText.StartsWith("/")) return;
        if (sourcePlayer.IsHost()) return;
        CommandRunner.Instance.Execute(new CommandContext(sourcePlayer, chatText[1..]));
    }
    
    [QuickPrefix(typeof(PlayerControl), nameof(PlayerControl.RpcSendChat), Priority.First)]
    internal static void HostCommandCheck(PlayerControl __instance, string chatText)
    {
        if (!AmongUsClient.Instance.AmHost) return;
        if (!chatText.StartsWith("/")) return;
        if (!__instance.IsHost()) return;
        CommandRunner.Instance.Execute(new CommandContext(__instance, chatText[1..]));
    }
}