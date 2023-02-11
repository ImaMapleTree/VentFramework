using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using VentLib.Logging;
using VentLib.Options.Interfaces;
using VentLib.Utilities;
using VentLib.Utilities.Extensions;

namespace VentLib.Options.OptionElement;

public partial class Option
{
    public string Name { get; internal set; } = null!;
    public string? Entry { get; internal set; }
    public Color Color { get; internal set; } = Color.white;
    public string ColorName => Color.Colorize(Name);
    public AbstractOptionTab? Tab { get => tab; set => SetTab(value!); }
    private AbstractOptionTab? tab;
    private uint tabCallbackId;

    internal string Key => Entry ?? Name;

    public Option? Parent;
    public readonly List<Option> SubOptions = new();
    public List<OptionValue> Values { get; set; } = new();
    internal Func<object, bool>? ShowOptionsPredicate; 
    
    private readonly List<Action<OptionChangeEvent>> eventHandlers = new();
    internal int DefaultIndex = -1;
    // ReSharper disable once InconsistentNaming
    internal int CurrentIndex;

    public object GetValue() => (IsIndexValid(CurrentIndex) ? Values[CurrentIndex] : OptionValue.Null).Value;
    public string GetValueAsString() => (IsIndexValid(CurrentIndex) ? Values[CurrentIndex] : OptionValue.Null).ToString()!;

    public void SetValue(int index)
    {
        object oldValue = GetValue();
        if (!IsIndexValid(index))
            VentLogger.Error("Attempted to change option to an illegal index. Ignoring change", "Options");
        else CurrentIndex = index;
        if (SaveOnChange) Save();
        object newValue = GetValue();
        eventHandlers.Do(h => h.Invoke(new OptionChangeEvent(this, oldValue, newValue, OptionChangeType.ManualSet)));
    }

    public void Increment()
    {
        object oldValue = GetValue();
        if (!IsIndexValid(++CurrentIndex)) CurrentIndex = 0;
        if (!IsIndexValid(CurrentIndex)) VentLogger.Error("Called increment on an option with no values.", "Options");
        if (SaveOnChange) Save();
        object newValue = GetValue();
        VentLogger.Fatal($"Incremented New Value: {newValue}");
        VentLogger.Fatal($"Incremented New Value: {newValue.GetType()}");
        VentLogger.Fatal($"Values: {Values.StrJoin()}");
        eventHandlers.Do(h => h.Invoke(new OptionChangeEvent(this, oldValue, newValue, OptionChangeType.Increment)));
    }

    public void Decrement()
    {
        object oldValue = GetValue();
        if (!IsIndexValid(--CurrentIndex)) CurrentIndex = Values.Count - 1;
        if (!IsIndexValid(CurrentIndex)) VentLogger.Error("Called decrement on an option with no values.", "Options");
        if (SaveOnChange) Save();
        object newValue = GetValue();
        VentLogger.Fatal($"Decremented New Value: {newValue}");
        eventHandlers.Do(h => h.Invoke(new OptionChangeEvent(this, oldValue, newValue, OptionChangeType.Decrement)));
    }

    // ReSharper disable once ParameterHidesMember
    public void SetTab(AbstractOptionTab? tab)
    {
        this.tab?.TabLoadCallbacks.Remove(tabCallbackId);
        this.tab?.RemoveOption(this);

        this.tab = tab;
        if (tab == null) return;
        
        tabCallbackId = tab.TabLoadCallbacks.Add(_ =>
        {
            if (StringOption == null) return;
            VentLogger.Fatal($"Tab Switch Callback: {tab}");
            SkipRender = false;
            OptionManager.NewRegisters.Add(this);
        });
        this.tab?.AddOption(this);
    }

    public void AddEventHandler(Action<OptionChangeEvent> onChangeHandler)
    {
        eventHandlers.Add(onChangeHandler);
    }

    public bool MatchesPredicate()
    {
        if (ShowOptionsPredicate == null) return true;
        try
        {
            return ShowOptionsPredicate(GetValue());
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    private bool IsIndexValid(int index) => index >= 0 && index < Values.Count;
}