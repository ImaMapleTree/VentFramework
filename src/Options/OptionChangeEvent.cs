namespace VentLib.Options;

public struct OptionChangeEvent
{
    public Option Source { get; }
    public object OldValue { get; }
    public object NewValue { get; }
    public OptionChangeType ChangeType { get; }

    internal OptionChangeEvent(Option source, object oldValue, object newValue, OptionChangeType changeType)
    {
        Source = source;
        OldValue = oldValue;
        NewValue = newValue;
        ChangeType = changeType;
    }
}

public enum OptionChangeType
{
    ManualSet,
    Increment,
    Decrement
}