using System;
using System.Collections.Generic;

namespace VentLib.Localization.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
public class LocalizedAttribute: Attribute, IComparable<LocalizedAttribute>
{
    internal static Dictionary<LocalizedAttribute, ReflectionObject> Attributes = new();

    public string? Key;
    public string? Group;
    public string? Subgroup;

    internal ReflectionObject? Source;
    internal Func<string?>? GroupSupplier;

    public LocalizedAttribute() { }

    public LocalizedAttribute(string key, string? group = null)
    {
        Key = key;
        Group = group;
    }

    internal string GetPath()
    {
        string? group = GroupSupplier != null 
            ? GroupSupplier() ?? Group 
            : Group;
        
        string subgroup = Subgroup != null ? "." + Subgroup : "";
        string key = Key != null ? "." + Key : "";

        return $"{group}{subgroup}{key}".TrimStart('.');
        /*if (Key == null && group == null) throw new FormatException($"Invalid Attribute for: {Source?.Object}. Localization Attribute must contain either a key or value");
        if (Key == null) return group + subgroup;
        return group == null ? Key : $"{group}{subgroup}.{Key}";*/
    }

    public int CompareTo(LocalizedAttribute? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        var groupComparison = string.Compare(Group, other.Group, StringComparison.Ordinal);
        if (groupComparison != 0) return groupComparison;
        var keyComparison = string.Compare(Key, other.Key, StringComparison.Ordinal);
        return keyComparison;
    }
}