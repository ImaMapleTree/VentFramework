using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using VentLib.Utilities.Extensions;

namespace VentLib.Utilities;

public class TextTable
{
    private List<string> columnHeaders;
    private List<int> longestField = new();
    private List<List<string>> entries = new();

    public TextTable(params string[] columns)
    {
        columnHeaders = columns.ToList();
        longestField = columns.Select(c => c.Length).ToList();
    }
    
    public TextTable AddColumn(string name)
    {
        columnHeaders.Add(name);
        longestField.Add(name.Length);
        return this;
    }

    public TextTable AddEntry(params object?[] data)
    {
        if (data.Length > columnHeaders.Count)
            throw new IndexOutOfRangeException($"Data fields was greater than number of columns ({data.Length} > {columnHeaders.Count})");
        int row = entries.Count;
        return AddEntry(row, data);
    }

    public TextTable AddEntry(int row, params object?[] data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            AddEntry(row, i, data[i]);
        }
        return this;
    }

    public TextTable AddEntry(int row, int column, object? data)
    {
        if (column > columnHeaders.Count)
            throw new IndexOutOfRangeException($"Specified column number is greater than columns in table ({column} > {columnHeaders.Count})");

        int neededRows = row - entries.Count + 1;
        for (int i = 0; i < neededRows; i++) entries.Add(new List<string>());
        List<string> rowList = entries[row];
        
        int neededColumns = column - rowList.Count + 1;
        for (int j = 0; j < neededColumns; j++) rowList.Add("");

        string dataString = data?.ToString() ?? "";
        
        rowList[column] = dataString;
        longestField[column] = Math.Max(dataString.Length, longestField[column]);
        return this;
    }

    public override string ToString()
    {
        string pattern = "| " + longestField.Select((f, i) => "{" + i + ",-" + f + "}").Join(delimiter: " | ") + " |";

        string table = string.Format(pattern, args: columnHeaders.Cast<object>().ToArray()) + "\n";
        
        table += string.Format(pattern, args: longestField.Select(f => "-".Repeat(f - 1)).Cast<object>().ToArray()) + "\n";

        foreach (List<string> entry in entries)
        {
            int neededRows = columnHeaders.Count - entry.Count + 1;
            for (int i = 0; i < neededRows; i++) entry.Add("");
            table += string.Format(pattern, args: entry.Cast<object>().ToArray()) + "\n";
        }
        
        return table.TrimEnd('\n');
    }
}