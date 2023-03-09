using UnityEngine;

namespace VentLib.Options.Announcement;

public class ButtonHelper
{
    public AnnouncementPopUp Source { get; }
    
    public ButtonHelper(AnnouncementPopUp popUp)
    {
        Source = popUp;
    }

    public AnnouncementButton Create(AnnouncementOption option)
    {
        AnnouncementPanel panel = Object.Instantiate(Source.AnnouncementPanelPrefab, Source.TextScroller.Inner);
        panel.PassiveButton.ClickMask = Source.TextScroller.Hitbox;
        return new AnnouncementButton(panel, option);
    }
}