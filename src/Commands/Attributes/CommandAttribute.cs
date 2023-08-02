using System;
using System.Reflection;
using HarmonyLib;
using VentLib.Commands.Interfaces;
using VentLib.Utilities;
using VentLib.Utilities.Extensions;

namespace VentLib.Commands.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class CommandAttribute: Attribute
{
    private CommandFlag flags;
    private string[] aliases;

    public CommandAttribute(CommandFlag flags, params string[] aliases)
    {
        this.flags = flags;
        this.aliases = aliases;
    }
    
    public CommandAttribute(params string[] aliases) : this(CommandFlag.None, aliases)
    {
    }
    
    
    /// <summary>
    /// Entry point into the saga. The flow is a bit complicated but basically.
    ///
    /// 1) If tyoe is nested => <see cref="RegisterNestedType"/>
    /// 2) If type has attribute => <see cref="CommandAttribute.RegisterType"/>
    /// 3) Otherwise, get all nested methods and register individually
    /// </summary>
    public static void Register(CommandRunner commandRunner, Type type)
    {
        CommandAttribute? commandAttribute = type.GetCustomAttribute<CommandAttribute>();
        if (type.IsNested) RegisterNestedType(commandRunner, type);
        else if (commandAttribute != null) commandAttribute.RegisterType(commandRunner, type, null);
        else type.GetMethods(AccessFlags.AllAccessFlags).ForEach(m => RegisterFloatingMethod(commandRunner, type, m));
    }

    /// <summary>
    /// Continues the register descent.. if this type is an attribute type then it immediately
    /// otherwise continue the registration descent.
    ///
    /// This method works because we can guarantee that at some point in the chain there was a method marked with the attribute
    /// </summary>
    private static void Register(CommandRunner commandRunner, Type type, Command? parentCommand)
    {
        CommandAttribute? commandAttribute = type.GetCustomAttribute<CommandAttribute>();
        if (commandAttribute != null)
        {
            commandAttribute.RegisterType(commandRunner, type, parentCommand);
            return;
        }
        
        type.GetNestedTypes(AccessFlags.AllAccessFlags).ForEach(t => Register(commandRunner, t, parentCommand));
        type.GetMethods(AccessFlags.AllAccessFlags).ForEach(t => t.GetCustomAttribute<CommandAttribute>()?.RegisterMethod(commandRunner, t, parentCommand?.Instance, parentCommand));
    }


    /// <summary>
    /// Specifically handles the registering of a nested type from the entry point. If the nested type has a parent with the attribute, immediately stops execution,
    /// otherwise checks if this nested type is an attribute holder, if so registers it
    /// otherwise, registers the methods
    /// </summary>
    private static void RegisterNestedType(CommandRunner commandRunner, Type nestedType)
    {
        Type? parentType = nestedType.DeclaringType;
        while (parentType != null)
        {
            if (parentType.GetCustomAttribute<CommandAttribute>() != null) return;
            parentType = parentType.DeclaringType;
        }

        CommandAttribute? commandAttribute = nestedType.GetCustomAttribute<CommandAttribute>();
        if (commandAttribute != null) commandAttribute.RegisterType(commandRunner, nestedType, null);
        else nestedType.GetMethods(AccessFlags.AllAccessFlags).ForEach(m => RegisterFloatingMethod(commandRunner, nestedType, m));
    }

    /// <summary>
    /// Registers a standalone method
    /// </summary>
    private static void RegisterFloatingMethod(CommandRunner commandRunner, Type parentType, MethodInfo method)
    {
        CommandAttribute? commandAttribute = method.GetCustomAttribute<CommandAttribute>();
        if (commandAttribute == null) return;
        object? instance = parentType.IsAssignableTo(typeof(ICommandReceiver)) ? AccessTools.CreateInstance(parentType) : null;
        commandAttribute.RegisterMethod(commandRunner, method, instance, null);
    }
    
    /// <summary>
    /// Main registry for a type, after registering, checks nested types + methods for further registration
    /// </summary>
    private void RegisterType(CommandRunner runner, Type type, Command? parentCommand)
    {
        object? instance = type.IsAssignableTo(typeof(ICommandReceiver)) ? AccessTools.CreateInstance(type) : null;
        Command command = new()
        {
            Aliases = aliases,
            Flags = flags,
            Instance = instance,
            CommandReceiver = instance != null,
            Trampoline = instance != null ? AccessTools.Method(type, "Receive", new[] { typeof(PlayerControl), typeof(CommandContext) }) : null
        };
        if (parentCommand == null) runner.Register(command);
        else parentCommand.SubCommands.Add(command);
        
        type.GetNestedTypes(AccessFlags.AllAccessFlags).ForEach(t => Register(runner, t, command));
        type.GetMethods(AccessFlags.AllAccessFlags).ForEach(m => m.GetCustomAttribute<CommandAttribute>()?.RegisterMethod(runner, m, instance, command));
    }

    /// <summary>
    /// Registers a method
    /// </summary>
    private void RegisterMethod(CommandRunner runner, MethodInfo method, object? instance, Command? parentCommand)
    {
        if (instance == null && !method.IsStatic) throw new ArgumentException($"Error registering {method.Name} in {method.DeclaringType}. Methods marked as [Command] must be static in classes that don't implement ICommandReceiver");
        Command command = new()
        {
            Aliases = aliases,
            Flags = flags,
            Instance = instance,
            CommandReceiver = false,
            Trampoline = method
        };
        
        if (parentCommand == null) runner.Register(command);
        else parentCommand.SubCommands.Add(command);
    }
}