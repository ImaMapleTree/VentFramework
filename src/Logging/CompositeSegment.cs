namespace VentLib.Logging;

// ReSharper disable once InconsistentNaming
public interface CompositeSegment
{
    public byte Alpha { get; }
    public byte Red { get; }
    public byte Green { get; }
    public byte Blue { get; }
    
    public string Text { get; set; }
    public System.Drawing.Color Color { set; }

    public bool GetColor(out System.Drawing.Color c);
    
    public System.Drawing.Color? GetColor();

    public string ToString(bool useColor);
}