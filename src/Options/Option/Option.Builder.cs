using System;
using HarmonyLib;

namespace VentLib.Options;

public partial class Option
{
    public OptionBuilder ToBuilder(BuilderFlags flags = BuilderFlags.KeepNone)
    {
        var builder = new OptionBuilder()
            .Name(Name)
            .Entry(Entry!)
            .Tab(Tab!)
            .Color(Color);
        if (flags.HasFlag(BuilderFlags.KeepValues))
            builder.Values(Values);
        if (flags.HasFlag(BuilderFlags.KeepSubOptions))
            SubOptions.Do(opt => builder.SubOption(_ => opt));
        if (flags.HasFlag(BuilderFlags.KeepSubOptionPredicate))
            builder.ShowSubOptionPredicate(ShowOptionsPredicate!);
        if (flags.HasFlag(BuilderFlags.KeepAttributes))
            Attributes.Do(kv => builder.Attribute(kv.Key, kv.Value));
        if (flags.HasFlag(BuilderFlags.KeepEventHandlers))
            eventHandlers.Do(ev => builder.BindEvent(ev));
        return builder;
    }
}

[Flags]
public enum BuilderFlags
{
    KeepValues = 1,
    KeepSubOptions = 2,
    KeepSubOptionPredicate = 4,
    KeepAttributes = 8,
    KeepEventHandlers = 16,
    KeepAll = 17,
    KeepNone = 32
}