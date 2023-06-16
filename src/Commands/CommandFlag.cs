using System;

namespace VentLib.Commands;

[Flags]
public enum CommandFlag
{
    None = 0,
    HostOnly = 1,
    ModdedOnly = 2,
    LobbyOnly = 4,
    InGameOnly = 8,
    CaseSensitive = 16
}