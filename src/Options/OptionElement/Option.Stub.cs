using VentLib.Utilities.Extensions;

namespace VentLib.Options.OptionElement;

public partial class Option
{
    internal class OptionStub
    {
        internal string Entry { get; }
        internal string Value { get; }
        internal OptionStub? Parent;
        internal int Level;
    
        public OptionStub(string stubLine, int level)
        {
            string[] split = stubLine.Split(": ");
            Entry = split[0].TrimStart('*');
            Value = split[1];
            Level = level;
        }

        internal OptionStub(Option option)
        {
            Entry = option.Key;
            Value = option.GetValue().ToString()!;
            Level = option.Level;
        }
        
        internal string Qualifier() => Parent == null ? Entry : Parent.Qualifier() + "." + Entry;

        public override string ToString()
        {
            string levelString = Level == 0 ? "" : "  ".Repeat(Level - 1) + "* ";
            return $"{levelString}{Entry}: {Value}";
        }
    }
}