using VentLib.Options.GUI.Tabs;

namespace VentLib.Options.GUI.Interfaces;

public interface ITabEvent : IControllerEvent
{
    IGameOptionTab Source();
}