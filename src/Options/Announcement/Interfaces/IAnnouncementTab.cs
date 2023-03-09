using System.Collections.Generic;

namespace VentLib.Options.Announcement.Interfaces;

public interface IAnnouncementTab
{
    string Title();

    string Subtitle();

    int UniqueId();

    void AddOption(AnnouncementOption option);

    void RemoveOption(AnnouncementOption option);

    List<AnnouncementOption> GetOptions();
}