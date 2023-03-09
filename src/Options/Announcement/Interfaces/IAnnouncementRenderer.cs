using System.Collections.Generic;

namespace VentLib.Options.Announcement.Interfaces;

public interface IAnnouncementRenderer
{
    List<AnnouncementButton> Render(List<AnnouncementOption> options, ButtonHelper buttonHelper);
}