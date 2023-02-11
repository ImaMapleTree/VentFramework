using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using VentLib.Options.Meta;
using VentLib.Utilities.Extensions;

namespace VentLib.Options.OptionElement;

public partial class Option
{
    internal OptionManager? manager;
    /// <summary>
    /// Used for plugins to store their own information within options, in TOHTOR this is used to store whether or not an option is a header
    /// </summary>
    public Dictionary<string, object> Attributes = new();
    public bool SaveOnChange = true;

    public int Level {
        get {
            if (level != -1) return level;
            int lvl = 0;
            var parent = Parent;
            while (parent != null) { parent = parent.Parent; lvl++; }
            return level = lvl;
        }
        internal set => level = value;
    }

    private int level = -1;

    public string Qualifier() => Parent == null ? Key : Parent.Qualifier() + "." + Key;

    internal void LoadOrCreate(OptionStub? stub)
    {
        if (stub == null)
        {
            eventHandlers.Do(h => h(new OptionChangeEvent(this, null!, GetValue(), OptionChangeType.ManualSet)));
            return;
        }
        int lIndex = Values.FindIndex(v => v.Value.ToString()!.Equals(stub.Value));
        if (lIndex == -1) lIndex = DefaultIndex;
        SetValue(lIndex);
    }
    
    public void Register(Assembly? assembly = null, bool isRendered = true)
    {
        assembly ??= Assembly.GetCallingAssembly();
        manager = OptionManager.Managers.GetValueOrDefault(assembly);
        if (manager == null) {
            OptionManager.Load(assembly);
            manager = OptionManager.Managers.GetValueOrDefault(assembly);
            if (manager == null)
                throw new NullReferenceException("Cannot Register Option - Option Manager not loaded for assembly");
        }

        NoRender = !isRendered;
        manager.LoadAndAdd(this, isRendered: isRendered);
        SubOptions.SelectMany(o => o.GetOptionsRecursive()).Do(o2 => manager.LoadAndAdd(o2, true));
    }

    public void Delete()
    {
        OptionManager.AllOptions.Remove(this);
        manager?.Options.Remove(this);
        tab?.RemoveOption(this);
        if (Behaviour != null)
        {
            Behaviour.gameObject.SetActive(false);
            Behaviour.Destroy();
        }
        SubOptions.Do(opt => opt.Delete());
    }

    internal List<Option> GetOptionsRecursive()
    {
        List<Option> options = new() { this };
        options.AddRange(SubOptions.SelectMany(sub => sub.GetOptionsRecursive()));
        return options;
    }

    public void Save() => manager?.Save();
}