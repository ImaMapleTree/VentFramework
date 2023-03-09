using VentLib.Options.Game.Interfaces;
using VentLib.Options.Game.Tabs;

namespace VentLib.Options.Game.Events;

public class TabEvent : ITabEvent
{
    private readonly IGameOptionTab source;
    
    public TabEvent(IGameOptionTab tab)
    {
        source = tab;
    }

    public IGameOptionTab Source() => source;
}