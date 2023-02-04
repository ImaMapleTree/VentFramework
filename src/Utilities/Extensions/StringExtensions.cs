using System;

namespace VentLib.Utilities.Extensions;

public static class StringExtensions
{
    public static string ReplaceFirst(this string str, string oldValue, string newValue)
    {
        int startIndex = str.IndexOf(oldValue, StringComparison.Ordinal);
        return string.Concat(newValue, str.AsSpan(startIndex + oldValue.Length));
    }
    
    /// <summary>
    /// Returns a new string repeated N times. If the number of times is 0 this returns the original string.
    /// </summary>
    /// <example><code>"hello".Repeat(1); // returns "hellohello"</code></example>
    /// <param name="str">string to repeat</param>
    /// <param name="times">number of times to repeat the string</param>
    /// <returns>If times is greater than 0, a new repeated string, otherwise the original string</returns>
    public static string Repeat(this string str, int times)
    {
        for (int i = 0; i < times; i++) str += str;
        return str;
    }
}