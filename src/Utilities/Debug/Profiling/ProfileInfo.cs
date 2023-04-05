using System;
using System.Reflection;

namespace VentLib.Utilities.Debug.Profiling;

public class ProfileInfo
{
    public string Name;
    public int Iterations { get; private set; }
    public double MinRuntime { get; private set; }
    public double MaxRuntime { get; private set; }
    public double TotalRuntime { get; private set; }
    internal MethodBase Method;

    public double AverageRuntime => Iterations == 0 ? 0 : TotalRuntime / Iterations;

    public ProfileInfo(string name, MethodBase method)
    {
        Name = name;
        Method = method;
    }

    public void RecordTiming(double duration)
    {
        TotalRuntime += duration;
        Iterations++;
        MinRuntime = Iterations == 1 ? duration : Math.Min(MinRuntime, duration);
        MaxRuntime = Math.Max(MaxRuntime, duration);
    }
}