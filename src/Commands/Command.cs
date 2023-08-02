using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VentLib.Options.Processors;
using VentLib.Utilities.Extensions;

namespace VentLib.Commands;

public class Command
{
    public string[] Aliases = Array.Empty<string>();
    public CommandFlag Flags;
    
    internal MethodInfo? Trampoline;
    internal object? Instance;
    internal bool CommandReceiver;

    public List<Command> SubCommands = new();

    public void Execute(CommandContext context)
    {
        MethodInfo? method = Trampoline;
        if (method == null) return;
        if (CommandReceiver)
        {
            method.Invoke(Instance, new object?[] { context.Source, context });
            return;
        }

        ParameterInfo[] parameters = method.GetParameters();
        Type[] parameterTypes = parameters.Select(p => p.ParameterType).ToArray();
        if (parameterTypes.Length == 0) method.Invoke(Instance, null);
        else if (parameterTypes.Length == 1) method.Invoke(Instance, new object?[] { context.Source });
        else if (parameterTypes.Contains(typeof(CommandContext)) && parameters.Length == 2) method.Invoke(Instance, new object?[] { context.Source, context });
        else
        {
            Type[] conversionTypes = parameterTypes[1..];
            List<object> values = new() { context.Source };
            if (parameterTypes.Contains(typeof(CommandContext)))
            {
                conversionTypes = parameterTypes[2..];
                values.Add(context);
            }
            
            for (int index = 0; index < conversionTypes.Length; index++)
            {
                Type conversionType = conversionTypes[index];
                string arg = index != conversionTypes.Length - 1 ? context.Args[index] : context.Args[index..].Fuse(" ");
                values.Add(ValueTypeProcessors.ReadFromLine(arg, conversionType));
            }
            
            method.Invoke(Instance, values.ToArray());
        }
    }
}