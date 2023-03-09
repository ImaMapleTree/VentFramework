using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using VentLib.Localization;
using VentLib.Options.Announcement.Interfaces;
using VentLib.Options.Game;
using VentLib.Options.Game.Interfaces;
using VentLib.Options.Interfaces;
using VentLib.Options.IO;
using VentLib.Ranges;
using VentLib.Utilities.Optionals;

namespace VentLib.Options.Announcement;

public class AnnouncementOptionBuilder : IOptionBuilder<AnnouncementOptionBuilder>
{
    protected AnnouncementOption Option = new();
    
    public static AnnouncementOptionBuilder From(Option option, BuilderFlags flags = BuilderFlags.KeepNone) => (AnnouncementOptionBuilder)OptionHelpers.OptionToBuilder(option, typeof(AnnouncementOptionBuilder), flags);
    
    public AnnouncementOptionBuilder Name(string name)
    {
        Option.name = name;
        return this;
    }

    public AnnouncementOptionBuilder Key(string key)
    {
        Option.Key = key;
        return this;
    }

    public AnnouncementOptionBuilder Description(string description)
    {
        Option.Description = Optional<string>.NonNull(description);
        return this;
    }

    public AnnouncementOptionBuilder LocaleName(string keyPath)
    {
        Option.name = Localizer.Get(keyPath, Assembly.GetCallingAssembly().GetName().Name);
        Option.Key ??= keyPath;
        return this;
    }

    public AnnouncementOptionBuilder IOSettings(Func<IOSettings, IOSettings> settings)
    {
        Option.IOSettings = settings(Option.IOSettings);
        return this;
    }
    
    public AnnouncementOptionBuilder IOSettings(Action<IOSettings> settings)
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
    public AnnouncementOptionBuilder ShowSubOptionPredicate(Func<object, bool> subOptionPredicate)
    {
        Option.Children.SetPredicate(subOptionPredicate);
        return this;
    }

    public AnnouncementOptionBuilder Values(IEnumerable<object> values)
    {
        Option.Values.AddRange(values.Select(v => new OptionValue(v)));
        return this;
    }

    public AnnouncementOptionBuilder Values(int defaultIndex, params object[] values)
    {
        Option.DefaultIndex = defaultIndex;
        Option.Values.AddRange(values.Select(v => new OptionValue(v)));
        return this;
    }
    
    public AnnouncementOptionBuilder Values(int defaultIndex, Type valueType, params object[] values)
    {
        Option.DefaultIndex = defaultIndex;
        Option.Values.AddRange(values.Select(v => new OptionValue(v)));
        Option.ValueType = valueType;
        return this;
    }

    public AnnouncementOptionBuilder Values(IEnumerable<OptionValue> values)
    {
        Option.Values.AddRange(values);
        return this;
    }

    public AnnouncementOptionBuilder Value(Func<GameOptionValueBuilder, OptionValue> valueBuilder)
    {
        Option.Values.Add(valueBuilder(new GameOptionValueBuilder()));
        return this;
    }

    public AnnouncementOptionBuilder Value(object value)
    {
        Option.Values.Add(new OptionValue(value));
        return this;
    }

    public AnnouncementOptionBuilder AddFloatRange(float start, float stop, float step)
    {
        return AddFloatRange(start, stop, step, 0);
    }

    public AnnouncementOptionBuilder AddIntRange(int start, int stop, int step)
    {
        return AddIntRange(start, stop, step, 0);
    }

    public AnnouncementOptionBuilder Bind(Action<object> changeConsumer)
    {
        BindEvent(ve => changeConsumer(ve.NewValue()));
        return this;
    }

    public AnnouncementOptionBuilder BindEvent(Action<IOptionEvent> changeConsumer)
    {
        Option.RegisterEventHandler(changeConsumer);
        return this;
    }
    
    public AnnouncementOptionBuilder BindEvent(Action<IOptionValueEvent> changeConsumer)
    {
        Option.RegisterEventHandler(ce =>
        {
            if (ce is IOptionValueEvent ove) changeConsumer(ove);
        });
        return this;
    }

    public AnnouncementOptionBuilder BindInt(Action<int> changeConsumer)
    {
        BindEvent(ve => changeConsumer((int)ve.NewValue()));
        return this;
    }

    public AnnouncementOptionBuilder BindBool(Action<bool> changeConsumer)
    {
        BindEvent(ve => changeConsumer((bool)ve.NewValue()));
        return this;
    }
    
    public AnnouncementOptionBuilder BindFloat(Action<float> changeConsumer)
    {
        BindEvent(ve => changeConsumer((float)ve.NewValue()));
        return this;
    }
    
    public AnnouncementOptionBuilder BindString(Action<string> changeConsumer)
    {
        BindEvent(ve => changeConsumer((string)ve.NewValue()));
        return this;
    }

    public AnnouncementOptionBuilder SubOption(Func<AnnouncementOptionBuilder, Option> subOption)
    {
        Option sub = subOption(new AnnouncementOptionBuilder());
        Option.AddChild(sub);
        return this;
    }

    public AnnouncementOptionBuilder Attribute(string key, object value)
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

    public AnnouncementOptionBuilder Clone()
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

    public AnnouncementOption Build()
    {
        return Option;
    }

    public AnnouncementOption BuildAndRegister(Assembly? assembly = null, OptionLoadMode loadMode = OptionLoadMode.LoadOrCreate)
    {
        assembly ??= Assembly.GetCallingAssembly();
        Option.Register(assembly, loadMode);
        return Option;
    }
    
    public AnnouncementOption BuildAndRegister(OptionManager manager, OptionLoadMode loadMode = OptionLoadMode.LoadOrCreate)
    {
        Option.Register(manager, loadMode);
        return Option;
    }
    

    public AnnouncementOptionBuilder Tab(IAnnouncementTab tab)
    {
        Option.Tab = tab;
        return this;
    }
    
    public AnnouncementOptionBuilder Color(Color color)
    {
        Option.Color = color;
        return this;
    }

    public AnnouncementOptionBuilder AddFloatRange(float start, float stop, float step, int defaultIndex = 0, string suffix = "")
    {
        var values = new FloatRangeGen(start, stop, step).AsEnumerable().Select(v => new GameOptionValueBuilder().Value(v).Suffix(suffix).Build());
        Option.DefaultIndex = defaultIndex;
        Option.Values.AddRange(values);
        return this;
    }

    public AnnouncementOptionBuilder AddIntRange(int start, int stop, int step = 1, int defaultIndex = 0, string suffix = "")
    {
        var values = new IntRangeGen(start, stop, step).AsEnumerable()
            .Select(v => new GameOptionValueBuilder().Value(v).Suffix(suffix).Build());
        Option.DefaultIndex = defaultIndex;
        Option.Values.AddRange(values);
        return this;
    }
}