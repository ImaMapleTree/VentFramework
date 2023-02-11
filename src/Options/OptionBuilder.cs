using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using VentLib.Localization;
using VentLib.Options.Interfaces;
using VentLib.Options.OptionElement;
using FRange = VentLib.Ranges.FloatRange;
using IRange = VentLib.Ranges.IntRange;
using static VentLib.Options.OptionValue;

namespace VentLib.Options;

public class OptionBuilder
{
    /// <summary>
    /// The current option being built, this is made public for use with extensions so that developers can make
    /// their own builder methods utilizing option attributes
    /// </summary>
    public Option option = new Option();

    public OptionBuilder() { }

    public OptionBuilder(int level)
    {
        option.Level = level;
    }

    public static OptionBuilder Typed<T>() where T: Option
    {
        OptionBuilder optionBuilder = new OptionBuilder {
            option = (T)typeof(T).GetConstructor(Array.Empty<Type>())!.Invoke(null)
        };
        return optionBuilder;
    }
        
    public static OptionBuilder From(Option option, BuilderFlags flags = BuilderFlags.KeepNone) => option.ToBuilder(flags);

    public OptionBuilder Name(string name)
    {
        option.Name = name;
        return this;
    }

    public OptionBuilder LocaleName(string keyPath)
    {
        option.Name = Localizer.Get(keyPath, Assembly.GetCallingAssembly().GetName().Name);
        return this;
    }

    public OptionBuilder Entry(string entry)
    {
        option.Entry = entry;
        return this;
    }

    public OptionBuilder Color(Color color)
    {
        option.Color = color;
        return this;
    }

    public OptionBuilder Tab(AbstractOptionTab tab)
    {
        option.Tab = tab;
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
        option.ShowOptionsPredicate = subOptionPredicate;   
        return this;
    }

    public OptionBuilder Values(IEnumerable<object> values)
    {
        option.Values.AddRange(values.Select(v => new OptionValueBuilder().Value(v).Build()));
        return this;
    }

    public OptionBuilder Values(int defaultIndex, params object[] values)
    {
        option.DefaultIndex = defaultIndex;
        option.Values.AddRange(values.Select(v => new OptionValueBuilder().Value(v).Build()));
        return this;
    }

    public OptionBuilder Values(IEnumerable<OptionValue> values)
    {
        option.Values.AddRange(values);
        return this;
    }

    public OptionBuilder Value(Func<OptionValueBuilder, OptionValue> valueBuilder)
    {
        option.Values.Add(valueBuilder(new OptionValueBuilder()));
        return this;
    }

    public OptionBuilder Value(object value)
    {
        option.Values.Add(new OptionValueBuilder().Value(value).Build());
        return this;
    }

    public OptionBuilder AddFloatRange(float start, float stop, float step, int defaultIndex = 0, string suffix = "")
    {
        var values = new FRange(start, stop, step).AsEnumerable()
            .Select(v => new OptionValueBuilder().Value(v).Suffix(suffix).Build());
        option.DefaultIndex = defaultIndex;
        option.Values.AddRange(values);
        return this;
    }

    public OptionBuilder AddIntRange(int start, int stop, int step = 1, int defaultIndex = 0, string suffix = "")
    {
        var values = new IRange(start, stop, step).AsEnumerable()
            .Select(v => new OptionValueBuilder().Value(v).Suffix(suffix).Build());
        option.DefaultIndex = defaultIndex;
        option.Values.AddRange(values);
        return this;
    }

    public OptionBuilder Bind(Action<object> changeConsumer)
    {
        option.AddEventHandler(oce => changeConsumer(oce.NewValue));
        return this;
    }

    public OptionBuilder BindEvent(Action<OptionChangeEvent> changeConsumer)
    {
        option.AddEventHandler(changeConsumer);
        return this;
    }

    public OptionBuilder BindInt(Action<int> changeConsumer)
    {
        option.AddEventHandler(oce => changeConsumer((int)oce.NewValue));
        return this;
    }

    public OptionBuilder BindBool(Action<bool> changeConsumer)
    {
        option.AddEventHandler(oce => changeConsumer((bool)oce.NewValue));
        return this;
    }
    
    public OptionBuilder BindFloat(Action<float> changeConsumer)
    {
        option.AddEventHandler(oce => changeConsumer((float)oce.NewValue));
        return this;
    }
    
    public OptionBuilder BindString(Action<string> changeConsumer)
    {
        option.AddEventHandler(oce => changeConsumer((string)oce.NewValue));
        return this;
    }

    public OptionBuilder SubOption(Func<OptionBuilder, Option> subOption)
    {
        Option sub = subOption(new OptionBuilder(option.Level + 1));
        option.SubOptions.Add(sub);
        return this;
    }

    public OptionBuilder Attribute(string key, object value)
    {
        option.Attributes[key] = value;
        return this;
    }

    public void ClearValues()
    {
        option.Values.Clear();
    }

    public void ClearAttributes()
    {
        option.Attributes.Clear();
    }

    public OptionBuilder Clone()
    {
        return From(option, BuilderFlags.KeepValues | BuilderFlags.KeepSubOptions | BuilderFlags.KeepSubOptionPredicate | BuilderFlags.KeepAttributes);
    }
    
    public Option Build()
    {
        return option;
    }

    public Option BuildAndRegister(Assembly? assembly = null, bool isRendered = true)
    {
        assembly ??= Assembly.GetCallingAssembly();
        option.Register(assembly, isRendered);
        return option;
    }
    
    /// <summary>
    /// Lazy method to add ON and OFF values to this option
    /// </summary>
    /// <returns>Current builder</returns>
    public OptionBuilder AddOnOffValues(bool defaultOn = true)
    {
        return 
            Value(val =>
                val.Text(defaultOn ? "ON" : "OFF")
                    .Value(defaultOn)
                    .Color(defaultOn ? UnityEngine.Color.cyan : UnityEngine.Color.red)
                    .Build())
            .Value(val =>
                val.Text(defaultOn ? "OFF" : "ON")
                    .Value(!defaultOn)
                    .Color(defaultOn ? UnityEngine.Color.red : UnityEngine.Color.cyan)
                    .Build());
    }
}