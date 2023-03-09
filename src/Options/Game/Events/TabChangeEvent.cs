using VentLib.Options.Game.Interfaces;
using VentLib.Options.Game.Tabs;

namespace VentLib.Options.Game.Events;

public class TabChangeEvent : TabEvent
{
    public readonly IGameOptionTab Original;
    
    public TabChangeEvent(IGameOptionTab oldTab, IGameOptionTab newTab) : base(newTab)
    {
        Original = oldTab;
    }
}