using System.Collections.Generic;

namespace VentLib.Utilities.Debug.Profiling;

public class Profilers
{
    public static List<Profiler> All = new();
    public static Profiler Global = new("Global");
}