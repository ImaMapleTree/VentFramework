using VentLib.Options.Game.Tabs;

namespace VentLib.Options.Game.Interfaces;

public interface ITabEvent : IControllerEvent
{
    IGameOptionTab Source();
}