using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using Hazel;
using VentLib.Networking.Interfaces;
using VentLib.Options.Events;
using VentLib.Options.Interfaces;
using VentLib.Options.IO;
using VentLib.Utilities.Optionals;

namespace VentLib.Options;

public class Option: IRpcSendable<Option>
{
    internal string name = null!;
    internal string? Key;
    public Optional<string> Description = Optional<string>.Null();
    public IOSettings IOSettings { get; set; } = new();
    public Dictionary<string, object> Attributes = new();

    internal Optional<int> Index = Optional<int>.Null();
    internal int DefaultIndex;

    internal Type ValueType
    {
        get => valueType ??= Values.Count > 0 ? Values[0].Value.GetType() : typeof(string);
        set => valueType = value;
    }
    private Type? valueType;
    
    internal List<OptionValue> Values = new();

    protected Optional<OptionValue> Value = Optional<OptionValue>.Null();

    protected Optional<Option> Parent = Optional<Option>.Null();
    public readonly SubOptions Children = new();
    internal OptionManager? Manager;

    internal readonly List<Action<IOptionEvent>> EventHandlers = new();

    public string Name() => name;

    public string Qualifier() => Parent.Map(p => p.Qualifier() + ".").OrElse("") + (Key ?? Name());

    internal void AddChild(Option child)
    {
        Children.Add(child);
        child.Parent = Optional<Option>.Of(this);
    }

    public OptionValue GetDefault()
    {
        return Values[EnforceIndexConstraint(DefaultIndex, true)];
    }
    
    public OptionValue GetRawValue()
    {
        if (!Index.Exists()) return GetDefault();
        return Value.OrElseSet(() => Values[EnforceIndexConstraint(Index.Get(), true)]);
    }

    public object GetValue() => GetRawValue().Value;

    public T GetValue<T>() => (T)GetRawValue().Value;

    public string GetValueText() => GetRawValue().GetText();

    internal void SetValue(OptionValue value)
    {
        Index = Optional<int>.NonNull(Values.IndexOf(value));
        if (Index.Get() == -1) Index = Optional<int>.NonNull(DefaultIndex);
        Optional<object> oldValue = this.Value.Map(v => v.Value);
        
        Value = Optional<OptionValue>.Of(value);
        
        OptionValueEvent optionValueEvent = new(this, oldValue, value.Value);
        EventHandlers.ForEach(eh => eh(optionValueEvent));
    }

    internal void SetValue(int index, bool triggerEvent = true)
    {
        Index = Optional<int>.NonNull(EnforceIndexConstraint(index));
        Optional<object> oldValue = Value.Map(v => v.Value);
        
        Value = Optional<OptionValue>.NonNull(Values[index]);

        OptionValueEvent optionValueEvent = new(this, oldValue, Value.Get().Value);
        if (triggerEvent) EventHandlers.ForEach(eh => eh(optionValueEvent));
    }

    public void SetDefaultIndex(int index)
    {
        DefaultIndex = EnforceIndexConstraint(index);
    }
    
    public void RegisterEventHandler(Action<IOptionEvent> eventHandler)
    {
        EventHandlers.Add(eventHandler);
    }
    
    public void Register(OptionManager manager, OptionLoadMode loadMode = OptionLoadMode.None)
    {
        manager.Register(this);
        if (loadMode is OptionLoadMode.Load or OptionLoadMode.LoadOrCreate)
            Load(loadMode is OptionLoadMode.LoadOrCreate);
        OptionRegisterEvent registerEvent = new(this, manager);
        EventHandlers.ForEach(eh => eh(registerEvent));
    }

    public void Register(Assembly? assembly = null, OptionLoadMode loadMode = OptionLoadMode.None)
    {
        assembly ??= Assembly.GetCallingAssembly();
        OptionManager manager = OptionManager.GetManager(assembly);
        Register(manager, loadMode);
    }
    
    public void Load(bool saveOnCreation = true)
    {
        if (Manager == null)
            throw new FileLoadException($"Cannot load Option \"{Qualifier()}\". Option not registered.");
        Manager.Load(this, saveOnCreation);
        if (!Value.Exists()) return;
        OptionLoadEvent loadEvent = new(this, Value.Get().Value);
        EventHandlers.ForEach(eh => eh(loadEvent));
    }

    protected int EnforceIndexConstraint(int index, bool allowOUFlow = false)
    {
        if (Values.Count == 0)
            throw new ConstraintException($"Index fails constraint because no values exist! (Option={Qualifier()})");
        if (index >= Values.Count)
        {
            if (!allowOUFlow)
                throw new ConstraintException($"Index is greater than value count! ({index} >= {Values.Count})");
            index = 0;
        } 
        else if (index < 0)
        {
            if (!allowOUFlow)
                throw new ConstraintException($"Index is less than zero! ({index} < 0)");
            index = Values.Count - 1;
        }

        return index;
    }

    public void NotifySubscribers(IOptionEvent @event)
    {
        EventHandlers.ForEach(eh => eh(@event));
    }

    public Option Read(MessageReader reader)
    {
        string qualifier = reader.ReadString();

        Option? sourceOption = OptionManager.AllOptions.GetValueOrDefault(qualifier);
        if (sourceOption == null)
            throw new ArgumentException($"Could not find registered option with qualifier: \"{qualifier}\"");
        
        string textValue = reader.ReadString();
        
        MemoryStream stream = new(Encoding.UTF8.GetBytes(textValue));
        StreamReader streamReader = new(stream);
        OptionReader optionReader = new(streamReader);
        optionReader.ReadToEnd();
        streamReader.Close();
        stream.Close();
        
        optionReader.Update(sourceOption);

        return sourceOption;
    }

    public void Write(MessageWriter writer)
    {
        writer.Write(Qualifier());

        MemoryStream ms = new();
        StreamWriter streamWriter = new(ms);
        OptionWriter optionWriter = new(streamWriter);
        
        optionWriter.Write(this);
        optionWriter.Close();

        StreamReader streamReader = new(ms);
        
        string text = streamReader.ReadLine()!;
        streamReader.Close();
        ms.Close();
        
        writer.Write(text);
    }

    internal bool HasParent() => Parent.Exists();
}