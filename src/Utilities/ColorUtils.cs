using UnityEngine;

namespace VentLib.Utilities;

public static class ColorUtils
{
    public static string Colorize(this Color color, string str) => ColorString(color, str);
    
    public static string Colorize(this Color color, char ch) => ColorString(color, ch.ToString());
    
    public static string ToText(this Color color)
    {
        return $"<color={color.ToHex()}>";
    }
    
    public static string ColorString(Color c, string s)
    {
        return $"{c.ToText()}{s}</color>";
    }
    
    public static string ToHex(this Color c)
    {
        return $"#{ToByte(c.r):X2}{ToByte(c.g):X2}{ToByte(c.b):X2}";
    }
    
    private static byte ToByte(float f)
    {
        f = Mathf.Clamp01(f);
        return (byte)(f * 255);
    }
}