using Pastel;

namespace VentLib.Logging;

public struct CompositeSegmentValue: CompositeSegment
{
    private const uint AByte = 0xFF000000;
    private const uint RByte = 0x00FF0000;
    private const uint GByte = 0x0000FF00;
    private const uint BByte = 0x000000FF;

    private const byte AShift = 24;
    private const byte RShift = 16;
    private const byte GShift = 8;

    public string Text { get; set; }
    public System.Drawing.Color Color
    {
        set => color = (value.A << 24) | (value.R << 16) | (value.G << 8) | value.B;
    }
    private int color;

    public CompositeSegmentValue(string text, System.Drawing.Color? color)
    {
        Text = text;
        if (color == null) this.color = -1;
        else
        {
            System.Drawing.Color nColor = color.Value;
            this.color = (nColor.A << AShift) | (nColor.R << RShift) | (nColor.G << GShift) | nColor.B;
        }
    }
    
    private byte A => (byte)((color & AByte) >> AShift);
    private byte R => (byte)((color & RByte) >> RShift);
    private byte G => (byte)((color & GByte) >> GShift);
    private byte B => (byte)(color & BByte);

    public byte Alpha => color == -1 ? (byte)0 : A;
    public byte Red => color == -1 ? (byte)0 : R;
    public byte Green => color == -1 ? (byte)0 : G;
    public byte Blue => color == -1 ? (byte)0 : B;
    
    public bool GetColor(out System.Drawing.Color c)
    {
        c = System.Drawing.Color.White;
        if (this.color == -1) return false;
        c = System.Drawing.Color.FromArgb(A, R, G, B);
        return true;
    }

    public System.Drawing.Color? GetColor()
    {
        if (this.color == -1) return null;
        return System.Drawing.Color.FromArgb(A, R, G, B);
    }

    public override string ToString() => Text;

    public string ToString(bool useColor)
    {
        if (!useColor) return Text;
        return GetColor(out System.Drawing.Color c) ? Text.Pastel(c) : Text;
    }
}