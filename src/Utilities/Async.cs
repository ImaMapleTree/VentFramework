using System;
using System.Threading.Tasks;
using VentLib.Logging;

// ReSharper disable LoopVariableIsNeverChangedInsideLoop

namespace VentLib.Utilities;

public static class Async
{
    public static async void Execute(Action action)
    {
        await Task.Run(action);
    }

    public static async void Execute<T>(Func<T> producer, Action<T> consumer)
    {
        consumer(await Task.Run(producer));
    }
    
    /// <summary>
    /// Schedules a task to be executed upon in the background. Contrary to common believe this action DOES block. But
    /// it only blocks when running the action. Thus this method should only be used to schedule light tasks or one-offs.
    /// </summary>
    /// <param name="action">The action to be complete</param>
    /// <param name="delay">Delay before starting the action</param>
    /// <param name="repeat">If the action should repeat with the same delay after execution</param>
    public static void Schedule(Action action, float delay, bool repeat = false)
    {
        _Schedule(action, delay, repeat);
    }

    /// <summary>
    /// Schedules a task to be executed upon in the background. Contrary to common believe this action DOES block. But
    /// it only blocks when running the action. Thus this method should only be used to schedule light tasks or one-offs.
    /// Unlike the no-argument schedule. This version takes a supplier which allows for passing in a parameter to the action
    /// when the action is ran.
    /// </summary>
    /// <param name="action">The action to be complete</param>
    /// <param name="delay">Delay before starting the action</param>
    /// <param name="supplier">Supplier to pass argument into the action</param>
    /// <param name="repeat">If the action should repeat with the same delay after execution</param>
    public static void Schedule<T>(Action<T> action, float delay, Func<T> supplier, bool repeat = false)
    {
        _Schedule(action, delay, supplier, repeat);
    }

    private static async void _Schedule(Action action, float delay, bool repeat)
    {
        int intDelay = (int)(1000f * delay);
        await Task.Delay(new TimeSpan(0, 0, 0, 0, intDelay));
        try { action(); }
        catch (Exception e) { VentLogger.Exception(e); }
        while (repeat) {
            await Task.Delay(new TimeSpan(0, 0, 0, 0, intDelay));
            action();
        }
    }
    
    private static async void _Schedule<T>(Action<T> action, float delay, Func<T> supplier, bool repeat)
    {
        int intDelay = (int)(1000f * delay);
        await Task.Delay(new TimeSpan(0, 0, 0, 0, intDelay));
        try { action(supplier()); }
        catch (Exception e) { VentLogger.Exception(e); }
        while (repeat) {
            await Task.Delay(new TimeSpan(0, 0, 0, 0, intDelay));
            action(supplier());
        }
    }
}