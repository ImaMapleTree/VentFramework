using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace VentLib.Commands.Patches;

[HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
internal static class AddChatPatch
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal static void Prefix(ChatController __instance, PlayerControl sourcePlayer, string chatText)
    {
        if (!chatText.StartsWith("/")) return;
        CommandRunner.Instance.Run(new CommandContext(sourcePlayer, chatText[1..]));
    }
}