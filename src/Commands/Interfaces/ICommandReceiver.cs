namespace VentLib.Commands.Interfaces;

public interface ICommandReceiver
{
    void Receive(PlayerControl source, CommandContext context);
}