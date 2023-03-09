using HarmonyLib;

namespace VentLib.Options.Announcement.Patches;


[HarmonyPatch(typeof(AnnouncementPopUp), nameof(AnnouncementPopUp.ShowIfNew))]
internal static class AnnouncmentLoadPatch
{
    private static void Prefix(AnnouncementPopUp __instance)
    {
        AnnouncementPopUp.UpdateState = AnnouncementPopUp.AnnounceState.Success;
    }
}