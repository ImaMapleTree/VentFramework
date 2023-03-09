namespace VentLib.Utilities.Extensions;

public static class AnnouncementExtensions
{
    public static Assets.InnerNet.Announcement Clone(this Assets.InnerNet.Announcement announcement)
    {
        return new Assets.InnerNet.Announcement
        {
            Id = announcement.Id,
            Language = announcement.Language,
            Number = announcement.Number,
            Title = announcement.Title,
            SubTitle = announcement.SubTitle,
            ShortTitle = announcement.ShortTitle,
            PinState = announcement.PinState,
            Text = announcement.Text,
            Date = announcement.Date
        };
    }
}