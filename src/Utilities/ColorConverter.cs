using System;
using UEColor = UnityEngine.Color;
using UEColor32 = UnityEngine.Color32;
using SDColor = System.Drawing.Color;

namespace VentLib.Utilities;

public static class ColorConverter
{
    public static SDColor ToSystemDrawing(this UEColor color)
    {
        return SDColor.FromArgb((int)color.a * 255, (int)color.r * 255, (int)color.g * 255, (int)color.b * 255);
    }
    
    public static SDColor ToSystemDrawing(this UEColor32 color)
    {
        return SDColor.FromArgb(color.a, color.r, color.g, color.b);
    }

    public static SDColor ToSystemDrawing(this ConsoleColor consoleColor)
    {
        return consoleColor switch
        {
            ConsoleColor.Black => SDColor.Black,
            ConsoleColor.DarkBlue => SDColor.DarkBlue,
            ConsoleColor.DarkGreen => SDColor.DarkGreen,
            ConsoleColor.DarkCyan => SDColor.DarkCyan,
            ConsoleColor.DarkRed => SDColor.DarkRed,
            ConsoleColor.DarkMagenta => SDColor.DarkMagenta,
            ConsoleColor.DarkYellow => SDColor.DarkKhaki,
            ConsoleColor.Gray => SDColor.Gray,
            ConsoleColor.DarkGray => SDColor.DarkGray,
            ConsoleColor.Blue => SDColor.Blue,
            ConsoleColor.Green => SDColor.Green,
            ConsoleColor.Cyan => SDColor.Cyan,
            ConsoleColor.Red => SDColor.Red,
            ConsoleColor.Magenta => SDColor.Magenta,
            ConsoleColor.Yellow => SDColor.Yellow,
            ConsoleColor.White => SDColor.White,
            _ => throw new ArgumentOutOfRangeException(nameof(consoleColor), consoleColor, null)
        };
    }

    public static UEColor ToUnity(this SDColor color)
    {
        return new UEColor(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
    }

    public static UEColor ToUnity(this ConsoleColor consoleColor)
    {
        return consoleColor.ToSystemDrawing().ToUnity();
    }

    public static UEColor32 ToUnity32(this SDColor color)
    {
        return new UEColor32(color.R, color.G, color.B, color.A);
    }
    
    public static UEColor32 ToUnity32(this ConsoleColor consoleColor)
    {
        return consoleColor.ToSystemDrawing().ToUnity32();
    }

    public static ConsoleColor ToConsoleColor(this SDColor color)
    {
        if (color == SDColor.Black) return ConsoleColor.Black;
        if (color == SDColor.DarkBlue) return ConsoleColor.DarkBlue;
        if (color == SDColor.DarkGreen) return ConsoleColor.DarkGreen;
        if (color == SDColor.DarkCyan) return ConsoleColor.DarkCyan;
        if (color == SDColor.DarkRed) return ConsoleColor.DarkRed;
        if (color == SDColor.DarkMagenta) return ConsoleColor.DarkMagenta;
        if (color == SDColor.DarkKhaki) return ConsoleColor.DarkYellow;
        if (color == SDColor.Gray) return ConsoleColor.Gray;
        if (color == SDColor.DarkGray) return ConsoleColor.DarkGray;
        if (color == SDColor.Blue) return ConsoleColor.Blue;
        if (color == SDColor.Green) return ConsoleColor.Green;
        if (color == SDColor.Cyan) return ConsoleColor.Cyan;
        if (color == SDColor.Red) return ConsoleColor.Red;
        if (color == SDColor.Magenta) return ConsoleColor.Magenta;
        if (color == SDColor.Yellow) return ConsoleColor.Yellow;
        return color == SDColor.White ? ConsoleColor.White : ConsoleColor.Gray;
    }
}