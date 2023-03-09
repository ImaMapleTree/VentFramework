using HarmonyLib;

namespace VentLib.Options.Announcement.Patches;


[HarmonyPatch(typeof(AnnouncementPopUp), nameof(AnnouncementPopUp.UpdateAnnouncementText))]
internal static class AnnouncementChangePatch
{
    private static void Postfix(AnnouncementPopUp __instance, [HarmonyArgument(0)] int id)
    {
        if (AnnouncementPopUp.UpdateState is not AnnouncementPopUp.AnnounceState.Cached) return;
        AnnouncementOptionController.Render(__instance, id);
    }
}