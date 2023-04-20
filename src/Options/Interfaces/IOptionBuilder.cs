using System;
using System.Collections.Generic;
using System.Reflection;
using VentLib.Options.IO;

namespace VentLib.Options.Interfaces;

public interface IOptionBuilder
{
    public IOptionBuilder Name(string name);

    public IOptionBuilder Key(string key);

    public IOptionBuilder Description(string description);

    public IOptionBuilder LocaleName(string keyPath);

    public IOptionBuilder IOSettings(Func<IOSettings, IOSettings> settings);

    public IOptionBuilder IOSettings(Action<IOSettings> settings);


    /// <summary>
    /// Introduces a condition for when to show sub-options. The predicate takes in an object which is the current
    /// value of the option and should return a bool with true indicating the sub options should be shown, and false indicating otherwise.
    /// </summary>
    /// <param name="subOptionPredicate">Predicate to determine if sub options should be shown</param>
    /// <returns></returns>
    public IOptionBuilder ShowSubOptionPredicate(Func<object, bool> subOptionPredicate);

    public IOptionBuilder Values(IEnumerable<object> values);

    public IOptionBuilder Values(int defaultIndex, params object[] values);
    
    public IOptionBuilder Values(int defaultIndex, Type valueType, params object[] values);

    public IOptionBuilder Values(IEnumerable<OptionValue> values);

    public IOptionBuilder Value(object value);

    public IOptionBuilder AddFloatRange(float start, float stop, float step);

    public IOptionBuilder AddIntRange(int start, int stop, int step = 1);

    public IOptionBuilder Bind(Action<object> changeConsumer);

    public IOptionBuilder BindEvent(Action<IOptionEvent> changeConsumer);

    public IOptionBuilder BindEvent(Action<IOptionValueEvent> changeConsumer);

    public IOptionBuilder BindInt(Action<int> changeConsumer);

    public IOptionBuilder BindBool(Action<bool> changeConsumer);

    public IOptionBuilder BindFloat(Action<float> changeConsumer);

    public IOptionBuilder BindString(Action<string> changeConsumer);

    public IOptionBuilder SubOption(Func<IOptionBuilder, Option> subOption);
    
    public IOptionBuilder Attribute(string key, object value);

    public void ClearValues();

    public void ClearAttributes();

    public IOptionBuilder Clone();

    public TR Build<TR>() where TR: Option;

    public TR BuildAndRegister<TR>(Assembly? assembly = null) where TR: Option;
    
    public TR BuildAndRegister<TR>(OptionManager manager) where TR: Option;
}

public interface IOptionBuilder<out T>: IOptionBuilder where T: IOptionBuilder
{
    public new T Name(string name);
    
    IOptionBuilder IOptionBuilder.Name(string name)
    {
        return Name(name);
    }

    public new T Key(string key);

    IOptionBuilder IOptionBuilder.Key(string key)
    {
        return Key(key);
    }

    public new T Description(string description);
    
    IOptionBuilder IOptionBuilder.Description(string description)
    {
        return Description(description);
    }

    public new T LocaleName(string qualifier);

    IOptionBuilder IOptionBuilder.LocaleName(string qualifier)
    {
        return LocaleName(qualifier);
    }

    public new T IOSettings(Func<IOSettings, IOSettings> settings);

    IOptionBuilder IOptionBuilder.IOSettings(Func<IOSettings, IOSettings> settings)
    {
        return IOSettings(settings);
    }

    public new T IOSettings(Action<IOSettings> settings);

    IOptionBuilder IOptionBuilder.IOSettings(Action<IOSettings> settings)
    {
        return IOSettings(settings);
    }

    /// <summary>
    /// Introduces a condition for when to show sub-options. The predicate takes in an object which is the current
    /// value of the option and should return a bool with true indicating the sub options should be shown, and false indicating otherwise.
    /// </summary>
    /// <param name="subOptionPredicate">Predicate to determine if sub options should be shown</param>
    /// <returns></returns>
    public new T ShowSubOptionPredicate(Func<object, bool> subOptionPredicate);

    IOptionBuilder IOptionBuilder.ShowSubOptionPredicate(Func<object, bool> subOptionPredicate)
    {
        return ShowSubOptionPredicate(subOptionPredicate);
    }

    public new T Values(IEnumerable<object> values);

    IOptionBuilder IOptionBuilder.Values(IEnumerable<object> values)
    {
        return Values(values);
    }

    public new T Values(int defaultIndex, params object[] values);

    IOptionBuilder IOptionBuilder.Values(int defaultIndex, params object[] values)
    {
        return Values(defaultIndex, values);
    }

    public new T Values(int defaultIndex, Type valueType, params object[] values);
    
    IOptionBuilder IOptionBuilder.Values(int defaultIndex, Type valueType, params object[] values)
    {
        return Values(defaultIndex, valueType, values);
    }

    public new T Values(IEnumerable<OptionValue> values);

    IOptionBuilder IOptionBuilder.Values(IEnumerable<OptionValue> values)
    {
        return Values(values);
    }

    public new T Value(object value);

    IOptionBuilder IOptionBuilder.Value(object value)
    {
        return Value(value);
    }

    public new T AddFloatRange(float start, float stop, float step);

    IOptionBuilder IOptionBuilder.AddFloatRange(float start, float stop, float step)
    {
        return AddFloatRange(start, stop, step);
    }

    public new T AddIntRange(int start, int stop, int step);

    IOptionBuilder IOptionBuilder.AddIntRange(int start, int stop, int step)
    {
        return AddIntRange(start, stop, step);
    }

    public new T Bind(Action<object> changeConsumer);

    IOptionBuilder IOptionBuilder.Bind(Action<object> changeConsumer)
    {
        return Bind(changeConsumer);
    }

    public new T BindEvent(Action<IOptionEvent> changeConsumer);

    IOptionBuilder IOptionBuilder.BindEvent(Action<IOptionEvent> changeConsumer)
    {
        return BindEvent(changeConsumer);
    }

    public new T BindEvent(Action<IOptionValueEvent> changeConsumer);

    IOptionBuilder IOptionBuilder.BindEvent(Action<IOptionValueEvent> changeConsumer)
    {
        return BindEvent(changeConsumer);
    }

    public new T BindInt(Action<int> changeConsumer);

    IOptionBuilder IOptionBuilder.BindInt(Action<int> changeConsumer)
    {
        return BindInt(changeConsumer);
    }

    public new T BindBool(Action<bool> changeConsumer);

    IOptionBuilder IOptionBuilder.BindBool(Action<bool> changeConsumer)
    {
        return BindBool(changeConsumer);
    }

    public new T BindFloat(Action<float> changeConsumer);

    IOptionBuilder IOptionBuilder.BindFloat(Action<float> changeConsumer)
    {
        return BindFloat(changeConsumer);
    }

    public new T BindString(Action<string> changeConsumer);

    IOptionBuilder IOptionBuilder.BindString(Action<string> changeConsumer)
    {
        return BindString(changeConsumer);
    }

    public T SubOption(Func<T, Option> subOption);

    IOptionBuilder IOptionBuilder.SubOption(Func<IOptionBuilder, Option> subOption)
    {
        return SubOption(subOption);
    }

    public new T Attribute(string key, object value);

    IOptionBuilder IOptionBuilder.Attribute(string key, object value)
    {
        return Attribute(key, value);
    }

    public new T Clone();

    IOptionBuilder IOptionBuilder.Clone()
    {
        return Clone();
    }
}