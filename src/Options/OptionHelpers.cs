using System;
using System.Collections.Generic;
using System.Linq;
using VentLib.Options.Interfaces;
using VentLib.Utilities.Extensions;

namespace VentLib.Options;

public class OptionHelpers
{
    public static List<Option> GetChildren(Option option, bool recursive = true)
    {
        List<Option> children = option.Children;
        if (!recursive) return children;
        return children.SelectMany(child => new[] {child}.Concat(GetChildren(child, recursive))).ToList();
    }

    public static IOptionBuilder OptionToBuilder(Option option, Type builderClass, BuilderFlags flags = BuilderFlags.KeepNone)
    {
        IOptionBuilder builder = ((IOptionBuilder)Activator.CreateInstance(builderClass)!)
            .Name(option.Name());
        if (flags.HasFlag(BuilderFlags.KeepValues))
            builder.Values(option.Values);
        if (flags.HasFlag(BuilderFlags.KeepSubOptions))
            option.Children.ForEach(opt => builder.SubOption(_ => opt));
        if (flags.HasFlag(BuilderFlags.KeepSubOptionPredicate))
            builder.ShowSubOptionPredicate(option.Children.Predicate);
        if (flags.HasFlag(BuilderFlags.KeepAttributes))
            option.Attributes.ForEach(kv => builder.Attribute(kv.Key, kv.Value));
        if (flags.HasFlag(BuilderFlags.KeepEventHandlers))
            option.EventHandlers.ForEach(ev => builder.BindEvent(ev));
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