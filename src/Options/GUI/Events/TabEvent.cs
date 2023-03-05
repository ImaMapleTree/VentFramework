using VentLib.Options.GUI.Interfaces;
using VentLib.Options.GUI.Tabs;

namespace VentLib.Options.GUI.Events;

public class TabEvent : ITabEvent
{
    private readonly IGameOptionTab source;
    
    public TabEvent(IGameOptionTab tab)
    {
        source = tab;
    }

    public IGameOptionTab Source() => source;
}