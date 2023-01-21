using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using VentLib.Localization;

namespace VentLib.Commands.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class CommandAttribute: Attribute
{
    public readonly string[] Aliases;
    public readonly CommandUser User = CommandUser.Everyone;
    public readonly HideCommand HideCommand = HideCommand.Always;
    public readonly bool CaseSensitive;

    internal readonly List<CommandAttribute> Subcommands = new();

    public CommandAttribute(params string[] aliases)
    {
        Aliases = aliases;
    }
    
    public CommandAttribute(string[]? localeAliases, params string[] aliases)
    {
        Aliases = localeAliases == null ? aliases : aliases.AddRangeToArray(localeAliases.Select(a => Localizer.Get(a)).ToArray());
    }

    public CommandAttribute(string[] aliases, string[]? localeAliases = null, CommandUser user = CommandUser.Everyone, HideCommand hideCommand = HideCommand.Always, bool caseSensitive = false)
    {
        Aliases = localeAliases == null ? aliases : aliases.AddRangeToArray(localeAliases.Select(a => Localizer.Get(a)).ToArray());
        User = user;
        HideCommand = hideCommand;
        CaseSensitive = caseSensitive;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((CommandAttribute)obj);
    }

    private bool Equals(CommandAttribute other)
    {
        return base.Equals(other) && Aliases.Equals(other.Aliases) && User == other.User && HideCommand == other.HideCommand && CaseSensitive == other.CaseSensitive && Equals(Subcommands, other.Subcommands);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Aliases, (int)User, (int)HideCommand, CaseSensitive, Subcommands);
    }
}