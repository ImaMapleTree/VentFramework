using System;
using System.Collections.Generic;

namespace VentLib.Options.Ranges;

public class IntRange: IRange<int>
{
    private int start;
    private int end;
    private int step;

    public IntRange(int start, int end, int step = 1)
    {
        this.start = start;
        this.end = end;
        this.step = step;
    }

    public IEnumerable<int> AsEnumerable()
    {
        List<int> values = new();
        for (int i = start; i <= end; i += step) values.Add(Convert.ToInt32(Math.Round(Convert.ToDecimal(i), 2)));
        return values;
    }
}