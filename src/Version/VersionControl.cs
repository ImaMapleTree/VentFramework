using System;
using System.Collections.Generic;
using VentLib.Version.Handshake;

namespace VentLib.Version;

public class VersionControl
{
    public static VersionControl Instance = null!;
    public Version? Version;
    internal Func<Version, HandshakeResult>? HandshakeFilter;
    internal List<(ReceiveExecutionFlag, Action<Version, PlayerControl>)> VersionHandles = new();
    internal bool Handshake = true;
    internal float ResponseTimer = 1f;

    public VersionControl()
    {
        Instance = this;
    }

    public void AddVersionReceiver(Action<Version, PlayerControl> receiver, ReceiveExecutionFlag executionFlag = ReceiveExecutionFlag.Always)
    {
        VersionHandles.Add((executionFlag, receiver));
    }
    
    public Version For(IVersionEmitter emitter)
    {
        HandshakeFilter = emitter.HandshakeFilter;
        return Version = emitter.Version();
    }

    public void DisableHandshake(bool disable = true)
    {
        Handshake = !disable;
    }

    public void SetResponseTimer(float timer)
    {
        ResponseTimer = timer;
    }
}

[Flags]
public enum ReceiveExecutionFlag
{
    OnSuccessfulHandshake = 1,
    OnFailedHandshake = 2,
    Always = 3
}