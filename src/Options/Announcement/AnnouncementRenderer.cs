using System.Collections.Generic;
using UnityEngine;
using VentLib.Options.Announcement.Interfaces;

namespace VentLib.Options.Announcement;

public class AnnouncementRenderer : IAnnouncementRenderer
{
    public List<AnnouncementButton> Render(List<AnnouncementOption> options, ButtonHelper buttonHelper)
    {
        List<AnnouncementButton> buttons = new();
        int row = -1;
        for (int index = 0; index < options.Count; index++)
        {
            if (index % 3 == 0) row++;
            AnnouncementOption option = options[index];
            AnnouncementButton button = buttonHelper.Create(option);
            button.SetLocalPosition(new Vector3(-4.25f, 0.5f));
            button += new Vector3(index % 3 * 2f, row * -0.7f);
            buttons.Add(button);
        }

        return buttons;
    }
}