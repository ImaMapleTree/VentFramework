using System;
using System.Collections.Generic;
using VentLib.Networking.Handshake;

namespace VentLib.Version;

public class VersionControl
{
    public static VersionControl Instance = null!;
    public Version? Version;
    internal bool Handshake = true;
    internal float ResponseTimer = 1f;
    internal Func<Version, HandshakeResult>? HandshakeFilter;
    internal readonly List<(ReceiveExecutionFlag, Action<Version, PlayerControl>)> VersionHandles = new();
    internal readonly HashSet<int> PassedClients = new(); 

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

    public bool IsModdedClient(int clientId) => PassedClients.Contains(clientId);
}

[Flags]
public enum ReceiveExecutionFlag
{
    OnSuccessfulHandshake = 1,
    OnFailedHandshake = 2,
    Always = 3
}