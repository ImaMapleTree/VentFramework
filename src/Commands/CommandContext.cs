using System;
using System.Collections.Generic;
using HarmonyLib;

namespace VentLib.Commands;

public struct CommandContext
{
    public string OriginalMessage;
    public string? Group;
    public string Alias;
    public string[] Args;
    public bool Errored;
    public List<int> ErroredParameters;

    internal PlayerControl Source;
    
    internal CommandContext(PlayerControl source, string message)
    {
        OriginalMessage = message;
        string[] split = message.Split(" ");
        Alias = split[0];
        Args = split.Length > 1 ? split[1..] : Array.Empty<string>();
        Group = null;
        Errored = false;
        ErroredParameters = new List<int>();
        Source = source;
    }

    internal CommandContext Subcommand()
    {
        return new CommandContext
        {
            OriginalMessage = OriginalMessage,
            Group = Alias,
            Alias = Args.Length > 0 ? Args[0] : null!,
            Args = Args.Length > 1  ? Args[1..] : Array.Empty<string>(),
            Errored = false,
            ErroredParameters = new List<int>(),
            Source = Source
        };
    }

    public string Join(string delimiter = " ") => Args.Join(delimiter: delimiter);
}