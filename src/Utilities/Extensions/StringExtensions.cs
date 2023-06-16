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
        if (times < 0) return "";
        string append = str;
        for (int i = 0; i < times; i++) str += append;
        return str;
    }

    /// <summary>
    /// Replaces the format item in a specified string with the string representation of a corresponding object in a specified array.
    /// </summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="arguments">An object array that contains zero or more objects to format.</param>
    /// <returns>A copy of format in which the format items have been replaced by the string representation of the corresponding objects in args.</returns>
    public static string Formatted(this string format, params object?[] arguments)
    {
        return string.Format(format, arguments);
    }
}