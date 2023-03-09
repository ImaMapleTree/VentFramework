using System.Collections.Generic;
using System.Linq;
using AmongUs.Data;
using UnityEngine;
using VentLib.Options.Announcement.Interfaces;
using VentLib.Utilities.Extensions;

namespace VentLib.Options.Announcement;

public class AnnouncementOptionController
{
    internal static readonly Dictionary<int, IAnnouncementTab> Tabs = new();
    private static List<AnnouncementButton> buttons = new();
    private static IAnnouncementRenderer _renderer = new AnnouncementRenderer();
    
    public static void AddTab(IAnnouncementTab tab)
    {
        Tabs[tab.UniqueId()] = tab;
        DataManager.Player.Announcements.AddAnnouncement(CreateAnnouncement(tab));
    }

    public static void RemoveTab(IAnnouncementTab tab)
    {
        Tabs.Remove(tab.UniqueId());
        var announcements = DataManager.Player.Announcements.AllAnnouncements.ToArray()
            .Where(a => a.Number != tab.UniqueId());
        DataManager.Player.Announcements.AllAnnouncements.Clear();
        announcements.ForEach(DataManager.Player.Announcements.AddAnnouncement);
    }

    // 0.207f per new line
    private const float NewLineHeight = 0.01f;
    internal static void Render(AnnouncementPopUp popup, int tabId)
    {
        buttons.ForEach(b => b.Destroy());
        buttons.Clear();
        IAnnouncementTab? tab = Tabs.GetValueOrDefault(tabId);
        if (tab == null) return;
        popup.Title.text = tab.Title();
        popup.SubTitle.text = tab.Subtitle();
        popup.DateString.text = "";
        buttons.AddRange(_renderer.Render(tab.GetOptions(), new ButtonHelper(popup)));

        float max = buttons.Count == 0 ? 0 : buttons.Max(b => b.Position.y);
        popup.AnnouncementBodyText.text = "‎\n".Repeat(Mathf.CeilToInt(max / NewLineHeight)-30);
    }
    
    
    private static Assets.InnerNet.Announcement CreateAnnouncement(IAnnouncementTab tab)
    {
        return new Assets.InnerNet.Announcement
        {
            Id = tab.UniqueId().ToString(),
            Number = tab.UniqueId(),
            Language = 0,
            Title = "MODDED",
            SubTitle = tab.Subtitle(),
            ShortTitle = tab.Title(),
            Text = "",
            Date = "11/17/2023",
            PinState = false
        };
    }
}