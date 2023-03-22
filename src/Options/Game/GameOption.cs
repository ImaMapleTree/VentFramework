using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using VentLib.Options.Game.Events;
using VentLib.Options.Game.Interfaces;
using VentLib.Options.Game.Tabs;
using VentLib.Utilities;
using VentLib.Utilities.Extensions;
using VentLib.Utilities.Optionals;

namespace VentLib.Options.Game;

public class GameOption : Option
{
    public Color Color = Color.white;
    public bool IsHeader;
    public IGameOptionTab? Tab
    {
        get => tab;
        set
        {
            tab?.RemoveOption(this);
            tab = value;
            tab?.AddOption(this);
        }
    }

    private IGameOptionTab? tab;

    public int Level => Parent.Map(p => ((GameOption)p).Level).OrElse(0) + 1;

    internal UnityOptional<StringOption> Behaviour = new();

    public string Name(bool colorize = true)
    {
        if (!colorize || Color == Color.white) return base.Name();
        return Color.Colorize(base.Name());
    }

    internal List<GameOption> GetDisplayedMembers()
    {
        return new[] { this }.Concat(Children.GetConditionally(GetValue())
            .Cast<GameOption>()
            .SelectMany(child => child.GetDisplayedMembers()))
            .ToList();
    }

    internal void HideMembers()
    {
        Behaviour.IfPresent(behaviour => behaviour.gameObject.SetActive(false));
        Children.Cast<GameOption>().ForEach(child => child.HideMembers());
    }

    public void Increment()
    {
        Optional<object> oldValue = Value.Map(v => v.Value);

        SetValue(EnforceIndexConstraint(Index.OrElse(DefaultIndex) + 1, true), false);
        Behaviour.IfPresent(b => b.ValueText.text = GetValueText());

        object newValue = GetValue();

        OptionValueIncrementEvent incrementEvent = new(this, oldValue, newValue);
        EventHandlers.ForEach(eh => eh(incrementEvent));
    }

    public void Decrement()
    {
        Optional<object> oldValue = Value.Map(v => v.Value);
        
        SetValue(EnforceIndexConstraint(Index.OrElse(DefaultIndex) - 1, true), false);
        Behaviour.IfPresent(b => b.ValueText.text = GetValueText());
        
        object newValue = GetValue();
        
        OptionValueDecrementEvent decrementEvent = new(this, oldValue, newValue);
        EventHandlers.ForEach(eh => eh(decrementEvent));
    }

    internal void BindPlusMinusButtons(GameOptionProperties properties)
    {
        PassiveButton plusButton = properties.PlusButton.GetComponent<PassiveButton>();
        PassiveButton minusButton = properties.MinusButton.GetComponent<PassiveButton>();

        plusButton.OnClick = new Button.ButtonClickedEvent();
        plusButton.OnClick.AddListener((Action)Increment);

        minusButton.OnClick = new Button.ButtonClickedEvent();
        minusButton.OnClick.AddListener((Action)Decrement);
    }
}