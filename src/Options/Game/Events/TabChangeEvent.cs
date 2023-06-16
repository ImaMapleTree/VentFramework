using VentLib.Options.Game.Interfaces;

namespace VentLib.Options.Game.Events;

public class TabChangeEvent : TabEvent
{
    public readonly IGameOptionTab Original;
    
    public TabChangeEvent(IGameOptionTab oldTab, IGameOptionTab newTab) : base(newTab)
    {
        Original = oldTab;
    }
}