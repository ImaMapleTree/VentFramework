using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using VentLib.Logging;

// ReSharper disable LoopVariableIsNeverChangedInsideLoop

namespace VentLib.Utilities;

public class Async
{
    internal static AUCWrapper AUCWrapper { get; } = new();

    /// <summary>
    /// Invokes a Unity coroutine
    /// </summary>
    /// <param name="coroutine">Unity coroutine to invoke</param>
    public static void Execute(IEnumerator coroutine)
    {
        AUCWrapper.StartCoroutine(coroutine);
    }

    /// <summary>
    /// Runs an anonymous function as a Unity-coroutine. Contrary to popular belief, this does not pass the function onto a separate thread.
    /// </summary>
    /// <param name="action">Anonymous function to be ran in step with the Unity Engine</param>
    public static void Execute(Action action)
    {
        AUCWrapper.StartCoroutine(CoroutineWrapper(action));
    }
    
    /// <summary>
    /// Runs an anonymous function as a Unity-coroutine. Contrary to popular belief, this does not pass the function onto a separate thread.
    /// This function takes an object producer and feeds the result to a consumer after invoking.
    /// </summary>
    /// <param name="producer">Anonymous function to be ran in step with the Unity Engine, returning some object</param>
    /// <param name="consumer">Consumer function to be called once the producer function completes</param>
    public static void Execute<T>(Func<T> producer, Action<T> consumer)
    {
        AUCWrapper.StartCoroutine(CoroutineWrapper(producer, consumer));
    }
    
    /// <summary>
    /// Runs an anonymous function on another thread, discarding the return result. Threads often times do not
    /// play nicely with Unity-Runtime objects so if you're running into 0xC0000005 issues use Execute instead.
    /// </summary>
    /// <param name="action">Anonymous function to be run on separate thread</param>
    public static async void ExecuteThreaded(Action action)
    {
        await Task.Run(action);
    }
    
    /// <summary>
    /// Runs an anonymous function on another thread, passing the return result to a consumer function. Threads often times do not
    /// play nicely with Unity-Runtime objects so if you're running into 0xC0000005 issues use Execute instead.
    /// </summary>
    /// <param name="producer">Anonymous function to be run on separate thread which returns an object</param>
    /// <param name="consumer">Consumer function to be called once the producer function completes</param>
    public static async void ExecuteThreaded<T>(Func<T> producer, Action<T> consumer)
    {
        consumer(await Task.Run(producer));
    }

    /// <summary>
    /// Schedules a Unity coroutine to run after a certain delay
    /// </summary>
    /// <param name="coroutine">Coroutine to run</param>
    /// <param name="delay">Delay to wait until running</param>
    /// <param name="repeat">If the coroutine should repeat continuously</param>
    public static async void Schedule(IEnumerator coroutine, float delay, bool repeat = false)
    {
        await Task.Delay((int)(delay * 1000f));
        AUCWrapper.StartCoroutine(coroutine);

        while (repeat)
        {
            await Task.Delay((int)(delay * 1000f));
            AUCWrapper.StartCoroutine(coroutine);
        }
    }
    
    /// <summary>
    /// Schedules an action to run after a certain delay. Internally this wraps the action into a Coroutine
    /// </summary>
    /// <param name="action">Action to run</param>
    /// <param name="delay">Delay to wait until running</param>
    /// <param name="repeat">If the action should be ran repeatedly</param>
    public static void Schedule(Action action, float delay, bool repeat = false)
    {
        AUCWrapper.StartCoroutine(CoroutineWrapper(action, delay, repeat));
    }

    /// <summary>
    /// Schedules a function to be ran after a certain delay, passing the result into the provided consumer
    /// </summary>
    /// <param name="producer">Producer function to run after a delay</param>
    /// <param name="consumer">Consumer function to be called once the producer function completes</param>
    /// <param name="delay">Delay to wait until running</param>
    /// <param name="repeat">If the function should be ran repeatedly</param>
    /// <typeparam name="T">Producer return type</typeparam>
    public static void Schedule<T>(Func<T> producer, Action<T> consumer, float delay, bool repeat = false)
    {
        AUCWrapper.StartCoroutine(CoroutineWrapper(producer, consumer, delay, repeat));
    }
    
    /// <summary>
    /// Schedules a task to be executed upon in the background. Contrary to common believe this action DOES block. But
    /// it only blocks when running the action. Thus this method should only be used to schedule light tasks or one-offs.
    /// </summary>
    /// <param name="action">The action to be complete</param>
    /// <param name="delay">Delay before starting the action</param>
    /// <param name="repeat">If the action should repeat with the same delay after execution</param>
    public static void ScheduleThreaded(Action action, float delay, bool repeat = false)
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
    public static void ScheduleThreaded<T>(Action<T> action, float delay, Func<T> supplier, bool repeat = false)
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
    
    private static IEnumerator CoroutineWrapper(Action action)
    {
        action();
        yield return null;
    }
    
    private static IEnumerator CoroutineWrapper(Action action, float delay, bool repeat)
    {
        yield return new WaitForSeconds(delay);
        action();

        while (repeat)
        {
            yield return new WaitForSeconds(delay);
            action();
        }
        
        yield return null;
    }
    
    private static IEnumerator CoroutineWrapper<T>(Func<T> producer, Action<T> consumer)
    {
        consumer(producer());
        yield return null;
    }
    
    private static IEnumerator CoroutineWrapper<T>(Func<T> producer, Action<T> consumer, float delay, bool repeat)
    {
        yield return new WaitForSeconds(delay);
        consumer(producer());

        while (repeat) {
            yield return new WaitForSeconds(delay);
            consumer(producer());
        }
        
        yield return null;
    }
}