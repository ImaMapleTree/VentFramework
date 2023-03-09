using System.Collections.Generic;
using HarmonyLib;
using VentLib.Logging;
using VentLib.Options.Announcement.Interfaces;
using VentLib.Utilities.Extensions;

namespace VentLib.Options.Announcement.Patches;

[HarmonyPatch(typeof(AnnouncementPanel), nameof(AnnouncementPanel.SetUp))]
internal class AnnouncementPanelPatch
{
    private static bool Prefix(AnnouncementPanel __instance, [HarmonyArgument(0)] Assets.InnerNet.Announcement announcement)
    {
        if (!announcement.Title.Contains("MODDED")) return true;
        Assets.InnerNet.Announcement modified = announcement.Clone();
        
        IAnnouncementTab? tab = AnnouncementOptionController.Tabs.GetValueOrDefault(modified.Number);
        if (tab == null)
        {
            VentLogger.Warn($"Could not load tab \"{announcement.Title}\" ({announcement.Number})");
            return true;
        }
        
        modified.Title = "Setting Up";
        __instance.SetUp(modified);
        /*tab.backgroundColor.IfPresent(color => __instance.ReadColor = color);
        tab.selectedColor.IfPresent(color => __instance.SelectedColor = color);*/
        __instance.DateText.text = tab.Subtitle();
        return false;
    }
}