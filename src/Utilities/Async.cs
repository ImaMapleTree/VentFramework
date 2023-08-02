using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using VentLib.Logging;

// ReSharper disable InconsistentNaming

// ReSharper disable LoopVariableIsNeverChangedInsideLoop

namespace VentLib.Utilities;

public class Async
{
    private static StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(Async));
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
    public static void Schedule(IEnumerator coroutine, float delay, bool repeat = false)
    {
        AUCWrapper.StartCoroutine(CoroutineWrapper(coroutine, delay, repeat));
    }
    
    /// <summary>
    /// Schedules an action to run after a certain delay. Internally this wraps the action into a Coroutine
    /// </summary>
    /// <param name="action">Action to run</param>
    /// <param name="delay">Delay to wait until running</param>
    /// <param name="repeat">If the action should be ran repeatedly</param>
    /// <param name="guaranteeMainThread">Guarantees task will be ran on the main thread, defaults to false as this option creates some overhead and delay</param>
    public static void Schedule(Action action, float delay, bool repeat = false, bool guaranteeMainThread = false)
    {
        if (guaranteeMainThread) MainThreadAnchor.ScheduleOnMainThread(() => AUCWrapper.StartCoroutine(CoroutineWrapper(action, delay, repeat)));
        else AUCWrapper.StartCoroutine(CoroutineWrapper(action, delay, repeat));
    }

    /// <summary>
    /// Schedules a function to be ran after a certain delay, passing the result into the provided consumer
    /// </summary>
    /// <param name="producer">Producer function to run after a delay</param>
    /// <param name="consumer">Consumer function to be called once the producer function completes</param>
    /// <param name="delay">Delay to wait until running</param>
    /// <param name="repeat">If the function should be ran repeatedly</param>
    /// <param name="guaranteeMainThread">Guarantees task will be ran on the main thread, defaults to false as this option creates some overhead and delay</param>
    /// <typeparam name="T">Producer return type</typeparam>
    public static void Schedule<T>(Func<T> producer, Action<T> consumer, float delay, bool repeat = false, bool guaranteeMainThread = false)
    {
        if (guaranteeMainThread) MainThreadAnchor.ScheduleOnMainThread(() => AUCWrapper.StartCoroutine(CoroutineWrapper(producer, consumer, delay, repeat)));
        else AUCWrapper.StartCoroutine(CoroutineWrapper(producer, consumer, delay, repeat));
    }

    /// <summary>
    /// Loops a producer infinitely with a given delay until it produces a non-null item, then passes its result to the callback
    /// </summary>
    /// <param name="producer">producer of an item, or null</param>
    /// <param name="callback">the consumer to be ran once the item is non-null</param>
    /// <param name="delay">the delay for re-checking the producer function</param>
    /// <param name="maxRetries">the maximum number of retries before ending the loop</param>
    /// <param name="guaranteeMainThread">guarantees the task will be ran on the main thread</param>
    /// <typeparam name="T">producer return type</typeparam>
    public static void WaitUntil<T>(Func<T?> producer, Action<T> callback, float delay = 0.1f, int maxRetries = int.MaxValue, bool guaranteeMainThread = false) where T: class
    {
        WaitUntil(producer, item => item != null, callback!, delay, maxRetries, guaranteeMainThread);
    }

    /// <summary>
    /// Loops a producer infinitely with a given delay until it matches a predicate, then passes its result to the callback
    /// </summary>
    /// <param name="producer">producer of an item</param>
    /// <param name="predicate">the function to check the item against</param>
    /// <param name="callback">the consumer to be ran once the item passes the predicate</param>
    /// <param name="delay">the delay for re-checking the producer function</param>
    /// <param name="maxRetries">the maximum number of retries before ending the loop</param>
    /// <param name="guaranteeMainThread">guarantees the task will be ran on the main thread</param>
    /// <typeparam name="T">producer return type</typeparam>
    public static void WaitUntil<T>(Func<T> producer, Func<T, bool> predicate, Action<T> callback, float delay = 0.1f, int maxRetries = int.MaxValue, bool guaranteeMainThread = false)
    {
        IEnumerator coroutine = LoopingCoroutineWrapper(producer, predicate, callback, delay, maxRetries);
        if (guaranteeMainThread) MainThreadAnchor.ScheduleOnMainThread(() => AUCWrapper.StartCoroutine(coroutine));
        else AUCWrapper.StartCoroutine(coroutine);
    }

    /// <summary>
    /// Loops an action until it succeeds, then calls the callback
    /// </summary>
    /// <param name="action">the action to be ran until success</param>
    /// <param name="callback">the action to be ran after success</param>
    /// <param name="delay">the delay for re-running the action</param>
    /// <param name="maxRetries">the maximum number of retries before ending the loop</param>
    /// <param name="guaranteeMainThread">guarantees the action will be ran on the main thread</param>
    public static void WaitUntil(Action action, Action callback, float delay = 0.1f, int maxRetries = int.MaxValue, bool guaranteeMainThread = false)
    {
        bool gate = false;
        bool Predicate() => gate;

        void ModifiedAction()
        {
            try
            {
                action();
                gate = true;
            }
            catch
            {
                // ignored
            }
        }

        WaitUntil(ModifiedAction, Predicate, callback, delay, maxRetries, guaranteeMainThread);
    }

    /// <summary>
    /// Loops an action until an indirect predicate function passes, then calls the callback function
    /// </summary>
    /// <param name="action">the action to be ran until the predicate passes</param>
    /// <param name="predicate">the function to check for success</param>
    /// <param name="callback">the action to be ran after passing predicate</param>
    /// <param name="delay">the delay for re-running the action</param>
    /// <param name="maxRetries">the maximum number of retries before ending the loop</param>
    /// <param name="guaranteeMainThread">guarantees the action will be ran on the main thread</param>
    public static void WaitUntil(Action action, Func<bool> predicate, Action callback, float delay = 0.1f, int maxRetries = int.MaxValue, bool guaranteeMainThread = false)
    {
        IEnumerator coroutine = LoopingCoroutineWrapper(action, predicate, callback, delay, maxRetries);
        if (guaranteeMainThread) MainThreadAnchor.ScheduleOnMainThread(() => AUCWrapper.StartCoroutine(coroutine));
        else AUCWrapper.StartCoroutine(coroutine);
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
        catch (Exception e) { log.Exception(e); }
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
        catch (Exception e) { log.Exception(e); }
        while (repeat) {
            await Task.Delay(new TimeSpan(0, 0, 0, 0, intDelay));
            action(supplier());
        }
    }

    private static IEnumerator CoroutineWrapper(IEnumerator coroutine, float delay, bool repeat)
    {
        yield return new WaitForSeconds(delay);
        AUCWrapper.StartCoroutine(coroutine);

        while (repeat)
        {
            yield return new WaitForSeconds(delay);
            AUCWrapper.StartCoroutine(coroutine);
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
    
    private static IEnumerator LoopingCoroutineWrapper(Action action, Func<bool> predicate, Action callback, float delay, int maxRetries = int.MaxValue)
    {
        action();
        int retries = 0;
        
        while (!predicate())
        {
            yield return new WaitForSeconds(delay);
            action();
            if (retries++ > maxRetries) yield return null;
        }

        callback();

        yield return null;
    }
    
    
    private static IEnumerator LoopingCoroutineWrapper<T>(Func<T> producer, Func<T, bool> predicate, Action<T> consumer, float delay, int maxRetries = int.MaxValue)
    {
        T item = producer();
        int retries = 0;
        
        while (!predicate(item))
        {
            yield return new WaitForSeconds(delay);
            item = producer();
            if (retries++ > maxRetries) yield return null;
        }

        consumer(item);

        yield return null;
    }
}