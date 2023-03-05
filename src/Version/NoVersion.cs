using Hazel;

namespace VentLib.Version;

public sealed class NoVersion: Version
{
    public override Version Read(MessageReader reader)
    {
        return new NoVersion();
    }

    protected override void WriteInfo(MessageWriter writer) { }

    public override string ToSimpleName() => "";

    public override string ToString() => "NoVersion";
}