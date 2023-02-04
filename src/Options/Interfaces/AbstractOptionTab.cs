using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using VentLib.Utilities.Collections;

namespace VentLib.Options.Interfaces;

public abstract class AbstractOptionTab
{
    internal UuidList<Action<AbstractOptionTab>> TabLoadCallbacks = new();
    internal UuidList<Action<AbstractOptionTab>> TabUnloadCallbacks = new();
    
    public abstract Transform GetTransform();

    public abstract void AddOption(Option option);

    public abstract void RemoveOption(Option option);

    public abstract List<Option> GetOptions();

    public abstract void SetOptions(List<Option> options);

    public void OnLoad(Action<AbstractOptionTab> callback)
    {
        TabLoadCallbacks.Add(callback);
    }

    public void OnUnload(Action<AbstractOptionTab> callback)
    {
        TabUnloadCallbacks.Add(callback);
    }

    public virtual void Load()
    {
        TabLoadCallbacks.Do(callback => callback(this));
    }
    
    public virtual void Unload()
    {
        TabUnloadCallbacks.Do(callback => callback(this));
    }
}