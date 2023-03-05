using System;
using System.Collections.Generic;

namespace VentLib.Ranges;

public class FloatRangeGen: IRange<float>
{
    private float start;
    private float end;
    private float step;

    public FloatRangeGen(float start, float end, float step = 1)
    {
        this.start = start;
        this.end = end;
        this.step = step;
    }

    public IEnumerable<float> AsEnumerable()
    {
        List<float> values = new();
        for (float i = start; i < end || Math.Abs(end - i) < 0.005f ; i += step) values.Add(Convert.ToSingle(Math.Round(Convert.ToDecimal(i), 2)));
        return values;
    }
}