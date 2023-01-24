using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using VentLib.Localization;

namespace VentLib.Commands.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class CommandAttribute: Attribute
{
    public string[] Aliases = null!;
    public readonly CommandUser User = CommandUser.Everyone;
    public readonly HideCommand HideCommand = HideCommand.Always;
    public readonly bool CaseSensitive;

    private readonly string[] _aliases;
    private readonly string[]? _localeAliases;

    internal readonly List<CommandAttribute> Subcommands = new();
    internal bool IsSubcommand;

    public CommandAttribute(params string[] aliases)
    {
        _aliases = aliases;
    }
    
    public CommandAttribute(string[]? localeAliases, params string[] aliases)
    {
        _localeAliases = localeAliases;
        _aliases = aliases;
    }

    public CommandAttribute(string[] aliases, string[]? localeAliases = null, CommandUser user = CommandUser.Everyone, HideCommand hideCommand = HideCommand.Always, bool caseSensitive = false)
    {
        _localeAliases = localeAliases;
        _aliases = aliases;
        
        User = user;
        HideCommand = hideCommand;
        CaseSensitive = caseSensitive;
    }

    internal void Generate(Assembly assembly)
    {
        Aliases = _localeAliases == null ? _aliases : _aliases.AddRangeToArray(_localeAliases.SelectMany(a => Localizer.GetAll(a, assembly.GetName().Name)).ToArray());
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