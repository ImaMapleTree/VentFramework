using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MonoMod.RuntimeDetour;
using VentLib.Localization.Attributes;

namespace VentLib.Localization;

public class ReflectionLoader
{
    public static void RegisterClass(Type cls, LocalizedAttribute? parent = null)
    {
        LocalizedAttribute? parentAttribute = cls.GetCustomAttribute<LocalizedAttribute>();
        if (parentAttribute != null)
        {
            if (parentAttribute.Key != null)
            {
                parentAttribute.Group ??= parentAttribute.Key;
                parentAttribute.Key = null;
            }

            if (parent != null)
            {
                parentAttribute.Subgroup = parentAttribute.GetPath();
                parentAttribute.Group = parent.GetPath();
            }
            LocalizedAttribute.Attributes.Add(parentAttribute, parentAttribute.Source = new ReflectionObject(cls, ReflectionType.Class));
        }

        List<FieldInfo> staticFields = cls.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy).ToList();
        FieldInfo? overrideField = staticFields.FirstOrDefault(f => f.GetCustomAttribute<SubgroupProvider>() != null);
        if (overrideField != null && parentAttribute != null) parentAttribute.Subgroup = (string?)overrideField.GetValue(null);

        List<PropertyInfo> properties = cls.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).ToList();
        PropertyInfo? overrideProperty = properties.FirstOrDefault(f => f.GetCustomAttribute<SubgroupProvider>() != null);
        if (overrideProperty != null)
        {
            Hook _ = new(overrideProperty.GetGetMethod(true), new Func<Func<object, string>, object, string>((getter, self) => PropertyInfoHook(getter, self, parentAttribute)));
        }

        staticFields.Do(f => RegisterField(f, ReflectionType.StaticField, parentAttribute));
        cls.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Do(f => RegisterField(f, ReflectionType.InstanceField, parentAttribute));
        cls.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Do(f => RegisterProperty(f, parentAttribute, cls.Assembly));
        cls.GetNestedTypes(BindingFlags.Default | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic).Do(clz => RegisterClass(clz, parentAttribute));
    }

    public static void RegisterField(FieldInfo field, ReflectionType reflectionType, LocalizedAttribute? parent)
    {
        LocalizedAttribute? attribute = field.GetCustomAttribute<LocalizedAttribute>();
        if (attribute == null) return;
        if (parent != null)
            attribute.GroupSupplier = parent.GetPath;
        
        LocalizedAttribute.Attributes.Add(attribute, attribute.Source = new ReflectionObject(field, reflectionType));
    }

    public static void RegisterProperty(PropertyInfo property, LocalizedAttribute? parent, Assembly assembly)
    {
        LocalizedAttribute? attribute = property.GetCustomAttribute<LocalizedAttribute>();
        if (attribute == null) return;

        string assemblyName = Assembly.GetCallingAssembly().GetName().Name!;
        Hook _ = new(property.GetGetMethod(true), new Func<Func<object, string>, object, string>((getter, self) => PropertyModifyHook(getter, self, attribute, assemblyName, attribute.Key == null)));
        if (parent != null)
            attribute.GroupSupplier = parent.GetPath;
        
        LocalizedAttribute.Attributes.Add(attribute, attribute.Source = new ReflectionObject(property, ReflectionType.Property));
    }

    public static string PropertyInfoHook(Func<object, string> getter, object self, LocalizedAttribute? parent)
    {
        string value = getter(self);
        if (parent == null) return value;
        parent.Subgroup = value;
        return value;
    }

    public static string PropertyModifyHook(Func<object, string> getter, object self, LocalizedAttribute target, string assemblyName, bool setKey)
    {
        // DO NOT REMOVE BELOW. THIS IS REQUIRED TO POSSIBLY INVOKE ANY REQS
        string value = getter(self);
        if (setKey) target.Key = value;
        return Localizer.Get(target.GetPath(), assemblyName);
    }
}