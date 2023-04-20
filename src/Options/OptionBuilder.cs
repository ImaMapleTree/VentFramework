using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VentLib.Localization;
using VentLib.Options.Interfaces;
using VentLib.Options.IO;
using VentLib.Ranges;
using VentLib.Utilities.Optionals;

namespace VentLib.Options;

public class OptionBuilder : IOptionBuilder<OptionBuilder>
{
    protected Option Option = new();
    
    public static OptionBuilder From(Option option, BuilderFlags flags = BuilderFlags.KeepNone) => (OptionBuilder)OptionHelpers.OptionToBuilder(option, typeof(OptionBuilder), flags);
    
    public OptionBuilder Name(string name)
    {
        Option.name = name;
        return this;
    }

    public OptionBuilder Key(string key)
    {
        Option.Key = key;
        return this;
    }

    public OptionBuilder Description(string description)
    {
        Option.Description = Optional<string>.NonNull(description);
        return this;
    }

    public OptionBuilder LocaleName(string qualifier)
    {
        Option.name = Localizer.Get(Assembly.GetCallingAssembly()).Translate(qualifier);
        Option.Key ??= qualifier;
        return this;
    }

    public OptionBuilder IOSettings(Func<IOSettings, IOSettings> settings)
    {
        Option.IOSettings = settings(Option.IOSettings);
        return this;
    }
    
    public OptionBuilder IOSettings(Action<IOSettings> settings)
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
    public OptionBuilder ShowSubOptionPredicate(Func<object, bool> subOptionPredicate)
    {
        Option.Children.SetPredicate(subOptionPredicate);
        return this;
    }

    public OptionBuilder Values(IEnumerable<object> values)
    {
        Option.Values.AddRange(values.Select(v => new OptionValue(v)));
        return this;
    }

    public OptionBuilder Values(int defaultIndex, params object[] values)
    {
        Option.DefaultIndex = defaultIndex;
        Option.Values.AddRange(values.Select(v => new OptionValue(v)));
        return this;
    }
    
    public OptionBuilder Values(int defaultIndex, Type valueType, params object[] values)
    {
        Option.DefaultIndex = defaultIndex;
        Option.Values.AddRange(values.Select(v => new OptionValue(v)));
        Option.ValueType = valueType;
        return this;
    }

    public OptionBuilder Values(IEnumerable<OptionValue> values)
    {
        Option.Values.AddRange(values);
        return this;
    }

    public OptionBuilder Value(Func<OptionValue.OptionValueBuilder, OptionValue> valueBuilder)
    {
        Option.Values.Add(valueBuilder(new OptionValue.OptionValueBuilder()));
        return this;
    }

    public OptionBuilder Value(object value)
    {
        Option.Values.Add(new OptionValue(value));
        return this;
    }

    public OptionBuilder AddFloatRange(float start, float stop, float step)
    {
        return AddFloatRange(start, stop, step, 0);
    }

    public OptionBuilder AddIntRange(int start, int stop, int step)
    {
        return AddIntRange(start, stop, step, 0);
    }

    public OptionBuilder AddFloatRange(float start, float stop, float step, int defaultIndex = 0)
    {
        var values = new FloatRangeGen(start, stop, step).AsEnumerable()
            .Select(v => new OptionValue(v));
        Option.DefaultIndex = defaultIndex;
        Option.Values.AddRange(values);
        return this;
    }

    public OptionBuilder AddIntRange(int start, int stop, int step = 1, int defaultIndex = 0)
    {
        var values = new IntRangeGen(start, stop, step).AsEnumerable()
            .Select(v => new OptionValue(v));
        Option.DefaultIndex = defaultIndex;
        Option.Values.AddRange(values);
        return this;
    }

    public OptionBuilder Bind(Action<object> changeConsumer)
    {
        BindEvent(ve => changeConsumer(ve.NewValue()));
        return this;
    }

    public OptionBuilder BindEvent(Action<IOptionEvent> changeConsumer)
    {
        Option.RegisterEventHandler(changeConsumer);
        return this;
    }
    
    public OptionBuilder BindEvent(Action<IOptionValueEvent> changeConsumer)
    {
        Option.RegisterEventHandler(ce =>
        {
            if (ce is IOptionValueEvent ve) changeConsumer(ve);
        });
        return this;
    }

    public OptionBuilder BindInt(Action<int> changeConsumer)
    {
        BindEvent(ve => changeConsumer((int)ve.NewValue()));
        return this;
    }

    public OptionBuilder BindBool(Action<bool> changeConsumer)
    {
        BindEvent(ve => changeConsumer((bool)ve.NewValue()));
        return this;
    }
    
    public OptionBuilder BindFloat(Action<float> changeConsumer)
    {
        BindEvent(ve => changeConsumer((float)ve.NewValue()));
        return this;
    }
    
    public OptionBuilder BindString(Action<string> changeConsumer)
    {
        BindEvent(ve => changeConsumer((string)ve.NewValue()));
        return this;
    }
    

    public OptionBuilder SubOption(Func<OptionBuilder, Option> subOption)
    {
        Option sub = subOption(new OptionBuilder());
        Option.AddChild(sub);
        return this;
    }

    public OptionBuilder Attribute(string key, object value)
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

    public OptionBuilder Clone()
    {
        return From(Option, BuilderFlags.KeepValues | BuilderFlags.KeepSubOptions | BuilderFlags.KeepSubOptionPredicate | BuilderFlags.KeepAttributes);
    }

    public TR Build<TR>() where TR : Option
    {
        return (TR) Build();
    }

    public TR BuildAndRegister<TR>(Assembly? assembly = null) where TR : Option
    {
        return (TR) BuildAndRegister();
    }

    public TR BuildAndRegister<TR>(OptionManager manager) where TR : Option
    {
        return (TR) BuildAndRegister(manager);
    }

    public Option Build()
    {
        return Option;
    }
    
    public Option BuildAndRegister(Assembly? assembly = null, OptionLoadMode loadMode = OptionLoadMode.LoadOrCreate)
    {
        assembly ??= Assembly.GetCallingAssembly();
        Option.Register(assembly, loadMode);
        return Option;
    }
    
    public Option BuildAndRegister(OptionManager manager, OptionLoadMode loadMode = OptionLoadMode.LoadOrCreate)
    {
        Option.Register(manager, loadMode);
        return Option;
    }
}