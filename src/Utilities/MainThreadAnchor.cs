using System;
using System.Collections.Concurrent;
using System.Threading;

namespace VentLib.Utilities;

public static class MainThreadAnchor
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
        Async.Schedule(CheckActionQueue, QueueCheckDelay, true);
    }

    /// <summary>
    /// Checks if the current invocation of this method is on the main thread.
    /// </summary>
    /// <returns>true if this method is ran on the main thread, otherwise false</returns>
    public static bool IsMainThread() => Environment.CurrentManagedThreadId == _mainThreadId;
    
    /// <summary>
    /// Ensures execution of a <b>scheduled</b> action on the main thread. This is primarily used to run Unity-based functions from an
    /// asynchronous environment as most Unity functions will require the main thread.
    /// <br/><br/>
    /// Do not use this method if you need exact timings. Functions in this method run on cycles defined by <see cref="QueueCheckDelay"/> thus,
    /// missing a cycle can result in large variability in functions that require high time precision.
    /// </summary>
    /// <param name="action">Action to be queued on the main thread</param>
    public static void ScheduleOnMainThread(Action action)
    {
        if (IsMainThread()) action();
        else _actions.Enqueue(action);
    }

    /// <summary>
    /// Utility wrapper for <see cref="ScheduleOnMainThread"/> which creates a consumer of T.
    /// </summary>
    /// <param name="consumer">Consumed-based action to be ran on the main thread</param>
    /// <typeparam name="T">The consumed object type</typeparam>
    /// <returns>A consumer representing the passed in consumed, but guaranteed to run on the main thread</returns>
    public static Action<T> ScheduleOnMainThread<T>(Action<T> consumer)
    {
        return item => ScheduleOnMainThread(() => consumer(item));
    }

    /// <summary>
    /// Schedules a task to be ran on the main thread, then waits for its execution before returning the result
    /// </summary>
    /// <param name="provider">function returning an object</param>
    /// <typeparam name="T">the type of object</typeparam>
    /// <returns>the object after being ran on the main thread</returns>
    public static T ExecuteOnMainThread<T>(Func<T> provider)
    {
        T item = default!;
        ExecuteOnMainThread(new Action(() => item = provider()));
        return item;
    }

    /// <summary>
    /// Schedules a task to to be rain on the main thread, then waits for its execution to finish
    /// </summary>
    /// <param name="action">function to run on main thread</param>
    public static void ExecuteOnMainThread(Action action)
    {
        Barrier barrier = new(2);
        ScheduleOnMainThread(() =>
        {
            action();
            barrier.SignalAndWait();
        });
        barrier.SignalAndWait();
    }

    private static void CheckActionQueue()
    {
        while (!_actions.IsEmpty) if (_actions.TryDequeue(out Action? action)) action.Invoke();
    }
}