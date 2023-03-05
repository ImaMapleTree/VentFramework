using VentLib.Networking.Handshake;

namespace VentLib.Version;

public interface IVersionEmitter
{
    public Version Version();

    public HandshakeResult HandshakeFilter(Version version)
    {
        return HandshakeResult.PassDoNothing;
    }
}