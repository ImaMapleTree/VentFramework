using System;

namespace VentLib.Lobbies;

public enum LobbyStatus
{
    Open,
    InGame,
    Unknown,
    Closed
}

public static class LobbyStatusMethods
{
    public static string ServerString(this LobbyStatus lobbyStatus)
    {
        return lobbyStatus switch
        {
            LobbyStatus.Open => "OPEN",
            LobbyStatus.InGame => "IN-GAME",
            LobbyStatus.Unknown => "UNKNOWN",
            LobbyStatus.Closed => "CLOSED",
            _ => throw new ArgumentOutOfRangeException(nameof(lobbyStatus), lobbyStatus, null)
        };
    }
}