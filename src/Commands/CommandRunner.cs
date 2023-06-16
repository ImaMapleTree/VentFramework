using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VentLib.Commands.Attributes;
using VentLib.Utilities.Extensions;

namespace VentLib.Commands;

public class CommandRunner
{
    public static CommandRunner Instance { get; private set; } = null!;
    internal Dictionary<string, List<Command>> Commands = new();
    internal Dictionary<string, List<Command>> CaseSensitiveCommands = new();

    internal CommandRunner()
    {
        Instance = this;
    }

    public void Register(Assembly assembly)
    {
        assembly.GetTypes().ForEach(t => CommandAttribute.Register(this, t));
    }

    internal void Register(Command command)
    {
        Dictionary<string, List<Command>> commandDictionary = command.Flags.HasFlag(CommandFlag.CaseSensitive) ? CaseSensitiveCommands : Commands;
        command.Aliases.ForEach(alias => commandDictionary.GetOrCompute(alias, () => new List<Command>()).Add(command));
    }

    internal void Execute(CommandContext context)
    {
        List<Command>? matches = Commands!.GetValueOrDefault(context.Alias);
        matches ??= CaseSensitiveCommands!.GetValueOrDefault(context.Alias);
        matches?.ForEach(m => ExecuteLowestCommands(m, context));
    }
    
    private bool ExecuteLowestCommands(Command source, CommandContext context)
    {
        if (source.Flags.HasFlag(CommandFlag.HostOnly) && !context.Source.IsHost()) return false;
        if (source.Flags.HasFlag(CommandFlag.ModdedOnly) && !context.Source.IsModded()) return false;
        if (source.Flags.HasFlag(CommandFlag.LobbyOnly) && LobbyBehaviour.Instance == null) return false;
        if (source.Flags.HasFlag(CommandFlag.InGameOnly) && LobbyBehaviour.Instance != null) return false;

        if (context.Alias == null) return false;
        HashSet<string> aliases = source.Aliases.ToHashSet();
        string alias = context.Alias;
        if (!source.Flags.HasFlag(CommandFlag.CaseSensitive))
        {
            aliases = aliases.Select(s => s.ToLowerInvariant()).ToHashSet();
            alias = alias.ToLowerInvariant();
        }

        if (!aliases.Contains(alias)) return false;
        
        bool higherExecution = source.SubCommands.Any(sc => ExecuteLowestCommands(sc, context.Subcommand()));
        if (higherExecution) return false;

        source.Execute(context);
        return true;
    }
}