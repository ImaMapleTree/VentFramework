using VentLib.Options.Game.Interfaces;

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