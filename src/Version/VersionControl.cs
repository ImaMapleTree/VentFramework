using System;
using System.Collections.Generic;
using System.Linq;
using VentLib.Networking.Handshake;
using VentLib.Utilities;

namespace VentLib.Version;

public class VersionControl
{
    public Version? Version;
    
    internal static VersionControl Instance = null!;
    internal bool Handshake = true;
    internal float ResponseTimer = 3f;
    internal Func<Version, HandshakeResult>? HandshakeFilter;
    internal readonly List<(ReceiveExecutionFlag, Action<Version, PlayerControl>)> VersionHandles = new();
    internal readonly HashSet<int> PassedClients = new();

    internal Dictionary<byte, Version> PlayerVersions = new();

    public VersionControl()
    {
        Instance = this;
        VersionHandles.Add((ReceiveExecutionFlag.Always, (version, player) =>
        {
            if (player != null) PlayerVersions[player.PlayerId] = version;
        }));
    }

    public static VersionControl For(IVersionEmitter emitter)
    {
        VersionControl vc = Instance;
        vc.HandshakeFilter = emitter.HandshakeFilter;
        vc.Version = emitter.Version();
        return vc;
    }
    
    public void AddVersionReceiver(Action<Version, PlayerControl> receiver, ReceiveExecutionFlag executionFlag = ReceiveExecutionFlag.Always)
    {
        VersionHandles.Insert(0, (executionFlag, receiver));
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

    public List<PlayerControl> GetModdedPlayers() => PlayerControl.AllPlayerControls.ToArray()
        .Where(p => PassedClients.Contains(p.GetClientId())).ToList();

    public Version GetPlayerVersion(byte playerId) => PlayerVersions.GetValueOrDefault(playerId, new NoVersion());
    
}

[Flags]
public enum ReceiveExecutionFlag
{
    OnSuccessfulHandshake = 1,
    OnFailedHandshake = 2,
    Always = 3
}