using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using VentLib.Logging;
using VentLib.Utilities.Collections;
using VentLib.Utilities.Extensions;

namespace VentLib.Utilities.Debug.Profiling;

public class Profiler
{
    private static StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(Profiler));
    public ProfilingSampler Sampler;
    public bool IsActive { get; private set;  }= true;
    protected readonly string Name;
    private readonly Dictionary<(MethodBase, string), ProfileInfo> profiledMethods = new();

    public Profiler(string name)
    {
        Name = name;
        Sampler = new ProfilingSampler(this);
        Profilers.All.Add(this);
    }

    public class ProfilingSampler
    {
        private readonly Profiler profiler;
        private readonly UuidList<(DateTime, ProfileInfo)> methodTimings = new(); 

        public ProfilingSampler(Profiler profiler)
        {
            this.profiler = profiler;
        }

        public uint Start(string? name = null)
        {
            if (!profiler.IsActive) return uint.MaxValue;
            MethodBase? callingMethod = new StackFrame(1).GetMethod();
            if (callingMethod == null) return uint.MaxValue;
            name ??= callingMethod.Name;
            ProfileInfo profileInfo = profiler.profiledMethods.GetOrCompute((callingMethod, name), () => new ProfileInfo(name, callingMethod));
            return methodTimings.Add((DateTime.Now, profileInfo));
        }

        public Sample Sampled(string? name = null)
        {
            if (!profiler.IsActive) return new Sample(profiler, uint.MaxValue);
            MethodBase? callingMethod = new StackFrame(1).GetMethod();
            if (callingMethod == null) return new Sample(profiler, uint.MaxValue);
            name ??= callingMethod.Name;
            ProfileInfo profileInfo = profiler.profiledMethods.GetOrCompute((callingMethod, name), () => new ProfileInfo(name, callingMethod));
            return new Sample(profiler, methodTimings.Add((DateTime.Now, profileInfo)));
        }

        // ReSharper disable once MemberHidesStaticFromOuterClass
        public void Sample(Action action, string name)
        {
            uint id = profiler.Sampler.Start(name);
            action();
            profiler.Sampler.Stop(id);
        }

        public void Stop(uint id, string? name = null)
        {
            if (!profiler.IsActive || id == uint.MaxValue) return;
            DateTime stopTime = DateTime.Now; 
            (DateTime startTime, ProfileInfo profile) = methodTimings.RemoveItem(id);
            if (name != null)
            {
                MethodBase method = profile.Method;
                profile = profiler.profiledMethods.GetOrCompute((method, name), () => new ProfileInfo(name, method));
            }

            profile.RecordTiming(stopTime.Subtract(startTime).TotalMilliseconds);
        }

        public void Discard(uint id)
        {
            methodTimings.Remove(id);
        }
        
    }

    public class Sample
    {
        public Profiler Profiler { get; }
        public uint Id { get; }

        internal Sample(Profiler profiler, uint id)
        {
            Profiler = profiler;
            Id = id;
        }

        public void Stop(string? name = null) => Profiler.Sampler.Stop(Id, name);

        public void Discard() => Profiler.Sampler.Discard(Id);
    }

    public void SetActive(bool active)
    {
        IsActive = active;
    }

    public void Clear()
    {
        profiledMethods.Clear();
        Sampler = new ProfilingSampler(this);
    }

    public TextTable GetCurrentData(TimeUnit unit = TimeUnit.Milliseconds, Sort sort = Sort.AvgTime)
    {
        TextTable textTable = new("Name", "Iterations", "Avg Runtime", "Total Runtime", "Min Runtime", "Max Runtime", "Full Name");

        IEnumerable<(MethodBase, ProfileInfo)> profiling = profiledMethods.Select(kvp => (kvp.Key.Item1, kvp.Value));
        if (sort is not Sort.None)
            profiling = profiling.Sorted(tuple =>
            {
                ProfileInfo info = tuple.Item2;
                return sort switch
                {
                    Sort.Iterations => (IComparable)info.Iterations,
                    Sort.TotalTime => info.TotalRuntime,
                    Sort.AvgTime => info.AverageRuntime,
                    Sort.Alphabetical => info.Name,
                    Sort.Min => info.MinRuntime,
                    Sort.Max => info.MaxRuntime,
                    Sort.None => info.Iterations,
                    _ => info.Iterations
                };
            }).Reverse();
        
        profiling.ForEach(tuple =>
        {
            MethodBase method = tuple.Item1;
            ProfileInfo info = tuple.Item2;
            object?[] entries =
            {
                info.Name, info.Iterations, CutoffDuration(info.AverageRuntime, unit), CutoffDuration(info.TotalRuntime, unit), CutoffDuration(info.MinRuntime, unit), CutoffDuration(info.MaxRuntime, unit),
                (method.ReflectedType?.FullName ?? "") + "." + method.Name
            };
            textTable.AddEntry(entries);
        });
        return textTable;
    }

    public void Display(TimeUnit unit = TimeUnit.Milliseconds, Sort sort = Sort.AvgTime)
    {
        log.Debug($"Profiler {Name} Results:\n{GetCurrentData(unit, sort)}");
    }

    private static string CutoffDuration(double duration, TimeUnit unit)
    {
        if (unit is TimeUnit.Seconds || duration >= 10000) return Math.Round(duration / 1000, 3).ToString(CultureInfo.InvariantCulture) + "s";
        return Math.Round(duration, 3) + "ms";
    }
}


public enum TimeUnit
{
    Milliseconds,
    Seconds
}

public enum Sort
{
    Iterations,
    TotalTime,
    AvgTime,
    Alphabetical,
    Min,
    Max,
    None
}