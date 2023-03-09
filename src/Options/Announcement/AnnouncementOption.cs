using UnityEngine;
using VentLib.Logging;
using VentLib.Options.Announcement.Interfaces;
using VentLib.Options.Game.Events;
using VentLib.Utilities;
using VentLib.Utilities.Optionals;

namespace VentLib.Options.Announcement;

public class AnnouncementOption : Option
{
    public Color Color = Color.white;
    public IAnnouncementTab? Tab
    {
        get => tab;
        set
        {
            tab?.RemoveOption(this);
            tab = value;
            tab?.AddOption(this);
        }
    }

    private IAnnouncementTab? tab;

    public string Name(bool colorize = true)
    {
        if (!colorize || Color == Color.white) return base.Name();
        return Color.Colorize(base.Name());
    }
    
    internal void Increment()
    {
        Optional<object> oldValue = Value.Map(v => v.Value);

        SetValue(EnforceIndexConstraint(Index.OrElse(DefaultIndex) + 1, true), false);

        object newValue = GetValue();
        VentLogger.Info($"New Value: {newValue}");

        OptionValueIncrementEvent incrementEvent = new(this, oldValue, newValue);
        EventHandlers.ForEach(eh => eh(incrementEvent));
    }
}