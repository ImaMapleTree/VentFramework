using System;
using System.Collections.Generic;

namespace VentLib.Ranges;

public class IntRangeGen: IRange<int>
{
    private int start;
    private int endInclusive;
    private int step;

    public IntRangeGen(int start, int endInclusive, int step = 1)
    {
        this.start = start;
        this.endInclusive = endInclusive;
        this.step = step;
    }

    public IEnumerable<int> AsEnumerable()
    {
        List<int> values = new();
        for (int i = start; i <= endInclusive; i += step) values.Add(Convert.ToInt32(Math.Round(Convert.ToDecimal(i), 2)));
        return values;
    }
}