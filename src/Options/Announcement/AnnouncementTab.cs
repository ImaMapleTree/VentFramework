using System.Collections.Generic;
using UnityEngine;
using VentLib.Options.Announcement.Interfaces;
using VentLib.Utilities.Optionals;
using Random = UnityEngine.Random;

namespace VentLib.Options.Announcement;

public class AnnouncementTab : IAnnouncementTab
{
    private int uniqueId = Random.RandomRange(0, int.MaxValue);
    private string title;
    private string subtitle;

    public Optional<Color> backgroundColor = Optional<Color>.Null();
    public Optional<Color> selectedColor = Optional<Color>.Null();
    
    private List<AnnouncementOption> announcementOptions = new();

    public AnnouncementTab(string title, string subtitle, Optional<Color>? backgroundColor = null, Optional<Color>? selectedColor = null)
    {
        this.title = title;
        this.subtitle = subtitle;
        if (backgroundColor != null) this.backgroundColor = backgroundColor;
        if (selectedColor != null) this.selectedColor = selectedColor;
    }

    public string Title() => title;

    public string Subtitle() => subtitle;

    public int UniqueId() => uniqueId;

    public void AddOption(AnnouncementOption option) => announcementOptions.Add(option);

    public void RemoveOption(AnnouncementOption option) => announcementOptions.Remove(option);

    public List<AnnouncementOption> GetOptions() => announcementOptions;
}