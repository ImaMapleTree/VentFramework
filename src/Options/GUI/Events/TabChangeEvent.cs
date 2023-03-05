using VentLib.Options.GUI.Tabs;

namespace VentLib.Options.GUI.Events;

public class TabChangeEvent : TabEvent
{
    public readonly IGameOptionTab Original;
    
    public TabChangeEvent(IGameOptionTab oldTab, IGameOptionTab newTab) : base(newTab)
    {
        Original = oldTab;
    }
}