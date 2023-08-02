using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using VentLib.Localization;
using VentLib.Options.Game.Interfaces;
using VentLib.Options.Interfaces;
using VentLib.Options.IO;
using VentLib.Ranges;
using VentLib.Utilities.Optionals;
// ReSharper disable MethodOverloadWithOptionalParameter

namespace VentLib.Options.Game;

public class GameOptionBuilder : IOptionBuilder<GameOptionBuilder>
{
    protected GameOption Option = new();
    
    public static GameOptionBuilder From(Option option, BuilderFlags flags = BuilderFlags.KeepNone) => (GameOptionBuilder)OptionHelpers.OptionToBuilder(option, typeof(GameOptionBuilder), flags);
    
    public GameOptionBuilder Name(string name)
    {
        Option.name = name;
        return this;
    }

    public GameOptionBuilder Key(string key)
    {
        Option.Key = key;
        return this;
    }

    public GameOptionBuilder Description(string description)
    {
        Option.Description = Optional<string>.NonNull(description);
        return this;
    }

    public GameOptionBuilder LocaleName(string qualifier)
    {
        Option.name = Localizer.Get(Assembly.GetCallingAssembly()).Translate(qualifier, translationCreationOption: TranslationCreationOption.CreateIfNull);
        Option.Key ??= qualifier;
        return this;
    }

    public GameOptionBuilder IOSettings(Func<IOSettings, IOSettings> settings)
    {
        Option.IOSettings = settings(Option.IOSettings);
        return this;
    }
    
    public GameOptionBuilder IOSettings(Action<IOSettings> settings)
    {
        settings(Option.IOSettings);
        return this;
    }
    

    /// <summary>
    /// Introduces a condition for when to show sub-options. The predicate takes in an object which is the current
    /// value of the option and should return a bool with true indicating the sub options should be shown, and false indicating otherwise.
    /// </summary>
    /// <param name="subOptionPredicate">Predicate to determine if sub options should be shown</param>
    /// <returns></returns>
    public GameOptionBuilder ShowSubOptionPredicate(Func<object, bool> subOptionPredicate)
    {
        Option.Children.SetPredicate(subOptionPredicate);
        return this;
    }

    public GameOptionBuilder Values(IEnumerable<object> values)
    {
        Option.Values.AddRange(values.Select(v => new OptionValue(v)));
        return this;
    }

    public GameOptionBuilder Values(int defaultIndex, params object[] values)
    {
        Option.DefaultIndex = defaultIndex;
        Option.Values.AddRange(values.Select(v => new OptionValue(v)));
        return this;
    }
    
    public GameOptionBuilder Values(int defaultIndex, Type valueType, params object[] values)
    {
        Option.DefaultIndex = defaultIndex;
        Option.Values.AddRange(values.Select(v => new OptionValue(v)));
        Option.ValueType = valueType;
        return this;
    }

    public GameOptionBuilder Values(IEnumerable<OptionValue> values)
    {
        Option.Values.AddRange(values);
        return this;
    }

    public GameOptionBuilder Value(Func<GameOptionValueBuilder, OptionValue> valueBuilder)
    {
        Option.Values.Add(valueBuilder(new GameOptionValueBuilder()));
        return this;
    }

    public GameOptionBuilder Value(object value)
    {
        Option.Values.Add(new OptionValue(value));
        return this;
    }

    public GameOptionBuilder AddFloatRange(float start, float stop, float step)
    {
        return AddFloatRange(start, stop, step, 0);
    }

    public GameOptionBuilder AddIntRange(int start, int stop, int step)
    {
        return AddIntRange(start, stop, step, 0);
    }

    public GameOptionBuilder Bind(Action<object> changeConsumer)
    {
        BindEvent(ve => changeConsumer(ve.NewValue()));
        return this;
    }

    public GameOptionBuilder BindEvent(Action<IOptionEvent> changeConsumer)
    {
        Option.RegisterEventHandler(changeConsumer);
        return this;
    }
    
    public GameOptionBuilder BindEvent(Action<IOptionValueEvent> changeConsumer)
    {
        Option.RegisterEventHandler(ce =>
        {
            if (ce is IOptionValueEvent ove) changeConsumer(ove);
        });
        return this;
    }

    public GameOptionBuilder BindInt(Action<int> changeConsumer)
    {
        BindEvent(ve => changeConsumer((int)ve.NewValue()));
        return this;
    }

    public GameOptionBuilder BindBool(Action<bool> changeConsumer)
    {
        BindEvent(ve => changeConsumer((bool)ve.NewValue()));
        return this;
    }
    
    public GameOptionBuilder BindFloat(Action<float> changeConsumer)
    {
        BindEvent(ve => changeConsumer((float)ve.NewValue()));
        return this;
    }
    
    public GameOptionBuilder BindString(Action<string> changeConsumer)
    {
        BindEvent(ve => changeConsumer((string)ve.NewValue()));
        return this;
    }

    public GameOptionBuilder SubOption(Func<GameOptionBuilder, Option> subOption)
    {
        Option sub = subOption(new GameOptionBuilder());
        Option.AddChild(sub);
        return this;
    }

    public GameOptionBuilder Attribute(string key, object value)
    {
        Option.Attributes[key] = value;
        return this;
    }

    public void ClearValues()
    {
        Option.Values.Clear();
    }

    public void ClearAttributes()
    {
        Option.Attributes.Clear();
    }

    public GameOptionBuilder Clone()
    {
        return From(Option, BuilderFlags.KeepValues | BuilderFlags.KeepSubOptions | BuilderFlags.KeepSubOptionPredicate | BuilderFlags.KeepAttributes);
    }

    public TR Build<TR>() where TR : Option
    {
        return (TR) (Option) Build();
    }

    public TR BuildAndRegister<TR>(Assembly? assembly = null) where TR : Option
    {
        return (TR) (Option) BuildAndRegister();
    }
    
    public TR BuildAndRegister<TR>(OptionManager manager) where TR : Option
    {
        return (TR) (Option) BuildAndRegister(manager);
    }

    public GameOption Build()
    {
        return Option;
    }

    public GameOption BuildAndRegister(Assembly? assembly = null, OptionLoadMode loadMode = OptionLoadMode.LoadOrCreate)
    {
        assembly ??= Assembly.GetCallingAssembly();
        Option.Register(assembly, loadMode);
        return Option;
    }
    
    public GameOption BuildAndRegister(OptionManager manager, OptionLoadMode loadMode = OptionLoadMode.LoadOrCreate)
    {
        Option.Register(manager, loadMode);
        return Option;
    }
    

    public GameOptionBuilder Tab(IGameOptionTab tab)
    {
        Option.Tab = tab;
        return this;
    }
    
    public GameOptionBuilder Color(Color color)
    {
        Option.Color = color;
        return this;
    }

    public GameOptionBuilder IsHeader(bool isHeader)
    {
        Option.IsHeader = isHeader;
        return this;
    }

    public GameOptionBuilder IsTitle(bool isTitle)
    {
        Option.IsTitle = isTitle;
        return this;
    }
    
    public GameOptionBuilder AddFloatRange(float start, float stop, float step, int defaultIndex = 0, string suffix = "")
    {
        var values = new FloatRangeGen(start, stop, step).AsEnumerable().Select(v => new GameOptionValueBuilder().Value(v).Suffix(suffix).Build());
        Option.DefaultIndex = defaultIndex;
        Option.Values.AddRange(values);
        return this;
    }

    public GameOptionBuilder AddIntRange(int start, int stop, int step = 1, int defaultIndex = 0, string suffix = "")
    {
        var values = new IntRangeGen(start, stop, step).AsEnumerable()
            .Select(v => new GameOptionValueBuilder().Value(v).Suffix(suffix).Build());
        Option.DefaultIndex = defaultIndex;
        Option.Values.AddRange(values);
        return this;
    }
}