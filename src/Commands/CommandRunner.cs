using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using VentLib.Commands.Attributes;
using VentLib.Commands.Interfaces;
using VentLib.Logging;

namespace VentLib.Commands;

public class CommandRunner
{
    internal static CommandRunner Instance = null!;
    internal Dictionary<CommandAttribute, object?> Registered = new();

    internal CommandRunner()
    {
        Instance = this;
    }

    public void Run(CommandContext context)
    {
        string lowerAlias = context.Alias.ToLower();
        List<CommandAttribute> commandsToBeRun = Registered.Keys.Where(cmd => 
            !cmd.IsSubcommand && (cmd.Aliases.Contains(context.Alias) || !cmd.CaseSensitive && 
                cmd.Aliases.Any(str => str.ToLower().Equals(lowerAlias)))).ToList();
        
        while (commandsToBeRun.Count > 0)
        {
            var context1 = context;
            commandsToBeRun
                .Where(attr => attr.User is CommandUser.Everyone || (AmongUsClient.Instance.AmHost && PlayerControl.LocalPlayer.PlayerId == context1.Source.PlayerId))
                .Select(cmd => Registered[cmd])
                .Do(obj =>
                {
                    if (obj == null) return;
                    if (obj is ICommandReceiver recv) recv.Receive(context1.Source, context1);
                    else
                    {
                        // lol this is so lazy
                        object[] lazyArray = (object[])obj;
                        AutocastAndInvoke((MethodInfo)lazyArray[1], lazyArray[0], context1);
                    }
                });

            context = context.Subcommand();
            commandsToBeRun = commandsToBeRun
                .Where(cmd => cmd.Subcommands.Count > 0)
                .Where(attr => attr.User is CommandUser.Everyone || (AmongUsClient.Instance.AmHost && PlayerControl.LocalPlayer.PlayerId == context1.Source.PlayerId))
                .SelectMany(cmd => cmd.Subcommands)
                .Where(cmd => cmd.Aliases.Contains(context.Alias) || !cmd.CaseSensitive && cmd.Aliases.Any(str => str.ToLower().Equals(lowerAlias))).ToList();
        }
    }

    internal void Register(Assembly assembly)
    {
        assembly.GetTypes().Do(t =>
        {
            CommandAttribute? commandAttribute = t.GetCustomAttribute<CommandAttribute>();
            if (commandAttribute == null) return;
            RegisterType(t, commandAttribute);
        });
    }

    private void AutocastAndInvoke(MethodInfo method, object instance, CommandContext cmdContext)
    {
        ParameterInfo[] parameters = method.GetParameters();
        switch (parameters.Length)
        {
            case 0:
                method.Invoke(instance, null);
                break;
            case 1:
                method.Invoke(instance, new object[] { cmdContext.Source });
                break;
            case 2:
                method.Invoke(instance, new object[] { cmdContext.Source, cmdContext });
                break;
            default:
                object[] args = parameters.Select((p, i) =>
                {
                    if (i <= 1) return null;
                    Type type = p.ParameterType;
                    try {
                        return Convert.ChangeType(cmdContext.Args[i-2], type);
                    } catch {
                        cmdContext.Errored = true;
                        cmdContext.ErroredParameters.Add(i);
                        return type.IsValueType ? Activator.CreateInstance(type) : null;
                    }
                }).ToArray()!;
                args[0] = cmdContext.Source;
                args[1] = cmdContext;
                method.Invoke(instance, args);
                break;
        }
    }

    private void RegisterType(Type type, CommandAttribute attribute)
    { 
        ConstructorInfo? constructor = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, Array.Empty<Type>());
        object? instance = constructor?.Invoke(null);
        
        type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic)
           .Where(m => m.GetCustomAttribute<CommandAttribute>() != null).Do(marked => {
               CommandAttribute subAttribute = marked.GetCustomAttribute<CommandAttribute>()!;
               subAttribute.IsSubcommand = true;
               if (marked.IsStatic)
               {
                   subAttribute.Generate(type.Assembly);
                   attribute.Subcommands.Add(subAttribute);
                   Registered[subAttribute] = new object?[] { null, marked };
               }
               else if (instance == null)
                    VentLogger.Error($"Could not initialize subcommand method: {marked}. Parent method must have a default no-args constructor.");
               else
               {
                   subAttribute.Generate(type.Assembly);
                   attribute.Subcommands.Add(subAttribute);
                   Registered[subAttribute] = new[] { instance, marked };
               }
           });
        
        type.GetNestedTypes(BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance).Do(subtype =>
        {
            CommandAttribute? subAttribute = subtype.GetCustomAttribute<CommandAttribute>();
            if (subAttribute == null) return;
            subAttribute.IsSubcommand = true;
            attribute.Subcommands.Add(subAttribute);
            RegisterType(subtype, subAttribute);
        });
        
        
        attribute.Generate(type.Assembly);
        bool isReceiver = type.IsAssignableTo(typeof(ICommandReceiver));
        if (!isReceiver) {
            Registered[attribute] = null;
            return;
        }
        
        if (constructor == null)
        {
            VentLogger.Error($"Could not initialize Receiver class: {type}. Classes marked with the Command Attribute and that implement ICommandReceiver should have a default no-args constructor.");
            Registered[attribute] = null;
            return;
        }

        Registered[attribute] = instance;
    }
}