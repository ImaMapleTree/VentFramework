using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using VentLib.Utilities;
using VentLib.Utilities.Extensions;

namespace VentLib.Localization.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Interface)]
public class LocalizedAttribute : Attribute
{
    internal static Dictionary<Type, string> ClassQualifiers = new();

    public string Qualifier;
    public bool IgnoreNesting;
    [Obsolete("Only use when needing to force a translation change.")]
    public bool ForceOverride;

    internal Type DeclaringType = null!;
    
    public LocalizedAttribute(string qualifier, bool ignoreNesting = false)
    {
        Qualifier = qualifier;
        IgnoreNesting = ignoreNesting;
    }

    public void Register(Localizer localizer, Type type)
    {
        Register(localizer, type, new[] { this });
    }

    public void Register(Localizer localizer, Type type, LocalizedAttribute[] parentAttributes)
    {
        ClassQualifiers[type] = Qualifier;
        DeclaringType = type;

        type.GetNestedTypes(AccessFlags.AllAccessFlags)
            .Where(t => t.GetCustomAttribute<LocalizedAttribute>() != null)
            .ForEach(t =>
            {
                LocalizedAttribute attribute = t.GetCustomAttribute<LocalizedAttribute>()!;
                attribute.Register(localizer, t, parentAttributes.AddItem(attribute).ToArray());
            });
        
        type.GetFields(AccessFlags.StaticAccessFlags)
            .Where(field => field.GetCustomAttribute<LocalizedAttribute>() != null)
            .ForEach(field => field.GetCustomAttribute<LocalizedAttribute>()!.Inject(localizer, parentAttributes, field));
    }

    public void Inject(Localizer localizer, LocalizedAttribute[] parentAttributes, FieldInfo containingField)
    {
        string qualifier = "";
        foreach (LocalizedAttribute parentAttribute in parentAttributes)
        {
            string classQualifier = ClassQualifiers.GetValueOrDefault(parentAttribute.DeclaringType, "");
            
            if (qualifier == "" || parentAttribute.IgnoreNesting)
                qualifier = classQualifier;
            else if (classQualifier != "")
                qualifier = qualifier + "." + classQualifier;
            
        }

        LocalizedAttribute fieldAttribute = containingField.GetCustomAttribute<LocalizedAttribute>()!;
        if (fieldAttribute.IgnoreNesting) qualifier = fieldAttribute.Qualifier;
        else qualifier = qualifier + "." + fieldAttribute.Qualifier;
        
        string? defaultValue = containingField.GetValue(null) as string;
#pragma warning disable CS0618 // Type or member is obsolete
        string translation = localizer.Translate(qualifier, defaultValue ?? $"<{qualifier}>", false, fieldAttribute.ForceOverride ? TranslationCreationOption.ForceSave : TranslationCreationOption.CreateIfNull);
#pragma warning restore CS0618 // Type or member is obsolete
        containingField.SetValue(null, translation);
    }
}
