using System;
using System.Collections.Concurrent;
using VentLib.Utilities.Attributes;
using VentLib.Utilities.Debug.Profiling;

namespace VentLib.Utilities;

[LoadStatic]
public class MainThreadAnchor
{
    /// <summary>
    /// Gets/Sets the delay for checking the main thread action queue. Lower values will <b>significantly</b> impact performance
    /// </summary>
    public static float QueueCheckDelay = 0.1f;
    
    private static int _mainThreadId;
    private static ConcurrentQueue<Action> _actions = new();

    static MainThreadAnchor()
    {
        _mainThreadId = Environment.CurrentManagedThreadId;
        Async.Schedule(CheckActionQueue, 0.1f, true);
    }

    /// <summary>
    /// Ensures execution of an action on the main thread. This is primarily used to run Unity-based functions from an
    /// asynchronous environment as most Unity functions will require the main thread.
    /// <br/><br/>
    /// Do not use this method if you need exact timings. Functions in this method run on cycles defined by <see cref="QueueCheckDelay"/> thus,
    /// missing a cycle can result in large variability in functions that require high time precision.
    /// </summary>
    /// <param name="action">Action to be queued on the main thread</param>
    public static void ExecuteOnMainThread(Action action)
    {
        if (Environment.CurrentManagedThreadId == _mainThreadId) action();
        else _actions.Enqueue(action);
    }

    /// <summary>
    /// Utility wrapper for <see cref="ExecuteOnMainThread"/> which creates a consumer of T.
    /// </summary>
    /// <param name="consumer">Consumed-based action to be ran on the main thread</param>
    /// <typeparam name="T">The consumed object type</typeparam>
    /// <returns>A consumer representing the passed in consumed, but guaranteed to run on the main thread</returns>
    public static Action<T> ExecuteOnMainThread<T>(Action<T> consumer)
    {
        return item => ExecuteOnMainThread(() => consumer(item));
    }

    private static void CheckActionQueue()
    {
        while (!_actions.IsEmpty) if (_actions.TryDequeue(out Action? action)) action.Invoke();
    }
}