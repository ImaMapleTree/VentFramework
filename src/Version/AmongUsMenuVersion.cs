using Hazel;

namespace VentLib.Version;


/// <summary>
/// Version Representing AUM
/// </summary>
public class AmongUsMenuVersion: Version
{
    public override Version Read(MessageReader reader)
    {
        return new AmongUsMenuVersion();
    }

    protected override void WriteInfo(MessageWriter writer)
    {
    }

    public override string ToSimpleName()
    {
        return "Among Us Menu (Hacking Menu)";
    }
}