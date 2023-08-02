namespace VentLib.Options.Game.Interfaces;

public interface ITabEvent : IControllerEvent
{
    IGameOptionTab Source();
}