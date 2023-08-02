using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using VentLib.Logging;
using VentLib.Utilities.Optionals;
using Object = UnityEngine.Object;

namespace VentLib.Utilities.Extensions;

public static class EnumerableExtensions
{
    private static StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(EnumerableExtensions));
    /// <summary>
    /// Maps a sequence into a new type, keeping all non-null values.
    /// </summary>
    /// <param name="source">A sequence of values to invoke a transform function on and filter.</param>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
    /// <returns>An <see cref="IEnumerable{T}"/> whose elements are the result of invoking the transform function on each element of source, then filtering out null elements.</returns>
    /// <exception cref="ArgumentNullException">source or selector is null</exception>
    public static IEnumerable<TResult> SelectWhere<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult?> selector)
    {
        return source.Select(selector).Where(item => item != null)!;
    }
    
    /// <summary>
    /// Maps a sequence into a new type, filtering all transformed values against a predicate.
    /// </summary>
    /// <param name="source">A sequence of values to invoke a transform function on and filter.</param>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <param name="predicate">A function to test each transformed element for a condition.</param>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
    /// <returns>An <see cref="IEnumerable{T}"/> whose elements are the result of invoking the transform function on each element of source, then filtering elements with the predicate.</returns>
    /// <exception cref="ArgumentNullException">source or selector is null</exception>
    public static IEnumerable<TResult?> SelectWhere<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult?> selector, Func<TResult?, bool> predicate)
    {
        return source.Select(selector).Where(predicate);
    }
    
    /// <summary>
    /// Filters a sequence, then maps all remaining elements into a new type.
    /// </summary>
    /// <param name="source">A sequence of values to invoke a transform function on and filter.</param>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <param name="predicate">A function to test each transformed element for a condition.</param>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
    /// <returns>An <see cref="IEnumerable{T}"/> whose elements are the result of invoking the transform function on each element of source, then filtering elements with the predicate.</returns>
    /// <exception cref="ArgumentNullException">source or selector is null</exception>
    public static IEnumerable<TResult?> SelectWhere<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult?> selector, Func<TSource, bool> predicate)
    {
        return source.Where(predicate).Select(selector);
    }

    /// <summary>
    /// Wraps element of this sequence in a new struct which contains the item and its index.
    /// This is a shortcut to LINQ Select.
    /// </summary>
    /// <param name="source">An ordered sequence of values.</param>
    /// <typeparam name="TSource">The type of elements of the source.</typeparam>
    /// <returns>A new sequence with a new struct representing each elements index and reference</returns>
    /// <exception cref="ArgumentNullException">source is null</exception>
    public static IEnumerable<(int index, TSource item)> Indexed<TSource>(this IEnumerable<TSource> source)
    {
        int i = 0;
        return source.Select(val => (i++, val));
    }

    /// <summary>
    /// Transforms a sequence of <see cref="Optional{T}"/> elements into all existing elements
    /// </summary>
    /// <param name="source">A sequence of optional elements</param>
    /// <typeparam name="TSource">The type of elements in the optional.</typeparam>
    /// <returns>A new sequence containing only the existing elements from the original sequence.</returns>
    /// <exception cref="ArgumentNullException">source is null</exception>
    public static IEnumerable<TSource> Filter<TSource>(this IEnumerable<Optional<TSource>> source)
    {
        return source.Where(opt => opt.Exists()).Select(opt => opt.Get());
    }

    /// <summary>
    /// Transforms a sequence of <see cref="Optional{T}"/> elements into all existing elements
    /// </summary>
    /// <param name="source">A sequence of optional elements</param>
    /// <param name="transformer">A transform function to apply to each element.</param>
    /// <typeparam name="TSource">The type of elements in the optional.</typeparam>
    /// <typeparam name="TResult">The type of the value returned by tranformer.</typeparam>
    /// <returns>A new sequence containing only the existing elements from the original sequence.</returns>
    /// <exception cref="ArgumentNullException">source is null</exception>
    public static IEnumerable<TResult> Filter<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, Optional<TResult>> transformer)
    {
        return source.Select(transformer).Where(opt => opt.Exists()).Select(opt => opt.Get());
    }

    /// <summary>
    /// Maps all elements of this sequence into a new dictionary via the provided key and value mapper functions.
    /// </summary>
    /// <param name="source">A sequence of values to put into a Dictionary.</param>
    /// <param name="keyMapper">Function to map element into its respective dictionary key.</param>
    /// <param name="valueMapper">Function to map element into its respective dictionary value.</param>
    /// <typeparam name="TSource">The type of elements of the source.</typeparam>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    /// <returns>A new <see cref="Dictionary{T,T}"/> containing mapped elements from the sequence.</returns>
    /// <exception cref="ArgumentNullException">source or mapper functions are null.</exception>
    public static Dictionary<TKey, TValue> ToDict<TSource, TKey, TValue>(this IEnumerable<TSource> source, Func<TSource, TKey> keyMapper, Func<TSource, TValue> valueMapper) where TKey: notnull
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (keyMapper == null) throw new ArgumentNullException(nameof(keyMapper));
        if (valueMapper == null) throw new ArgumentNullException(nameof(valueMapper));
        Dictionary<TKey, TValue> dictionary = new();
        source.Do(item => dictionary[keyMapper(item)] = valueMapper(item));
        return dictionary;
    }
    
    /// <summary>
    /// Maps all elements of this sequence into a new dictionary via the provided key and value mapper functions.
    /// </summary>
    /// <param name="source">A sequence of values to put into a Dictionary.</param>
    /// <param name="keyMapper">Function to map element into its respective dictionary key.</param>
    /// <param name="valueMapper">Function to map element into its respective dictionary value.</param>
    /// <typeparam name="TSource">The type of elements of the source.</typeparam>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    /// <returns>A new <see cref="Dictionary{T,T}"/> containing mapped elements from the sequence.</returns>
    /// <exception cref="ArgumentNullException">source or mapper functions are null.</exception>
    public static Dictionary<TKey, TValue> ToDict<TSource, TKey, TValue>(this IEnumerable<TSource> source, Func<TSource, int, TKey> keyMapper, Func<TSource, int, TValue> valueMapper) where TKey: notnull
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (keyMapper == null) throw new ArgumentNullException(nameof(keyMapper));
        if (valueMapper == null) throw new ArgumentNullException(nameof(valueMapper));
        Dictionary<TKey, TValue> dictionary = new();
        source.ForEach((item, i) => dictionary[keyMapper(item, i)] = valueMapper(item, i));
        return dictionary;
    }
    
    /// <summary>
    /// Executes an action for each element in this sequence.
    /// </summary>
    /// <param name="source">A sequence of values.</param>
    /// <param name="action">The action to execute on each element.</param>
    /// <typeparam name="TSource">The type of elements of the source.</typeparam>
    /// <exception cref="ArgumentNullException">source or mapper functions are null.</exception>
    public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (action == null) throw new ArgumentNullException(nameof(action));
        IEnumerator<TSource> enumerator = source.GetEnumerator();
        while (enumerator.MoveNext())
            action(enumerator.Current);
        enumerator.Dispose();
    }

    /// <summary>
    /// Executes an action for each element in this sequence.
    /// </summary>
    /// <param name="source">A sequence of values.</param>
    /// <param name="action">The action to execute on each element.</param>
    /// <typeparam name="TSource">The type of elements of the source.</typeparam>
    /// <exception cref="ArgumentNullException">source or mapper functions are null.</exception>
    public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource, int> action)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (action == null) throw new ArgumentNullException(nameof(action));
        int i = 0;
        IEnumerator<TSource> enumerator = source.GetEnumerator();
        while (enumerator.MoveNext())
            action(enumerator.Current, i++);
        enumerator.Dispose();
    }

    /// <summary>
    /// Returns an <see cref="Optional{T}"/> containing the first matching element of this sequence, or an empty optional of no such element exists.
    /// </summary>
    /// <param name="source">A sequence of values.</param>
    /// <typeparam name="TSource">The type of elements of the source.</typeparam>
    /// <returns>An <see cref="Optional{T}"/> containing the matching element, or an empty Optional otherwise.</returns>
    /// <exception cref="ArgumentNullException">source or predicate functions are null.</exception>
    public static Optional<TSource> FirstOrOptional<TSource>(this IEnumerable<TSource> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        try
        {
            return Optional<TSource>.NonNull(source.First());
        }
        catch (InvalidOperationException)
        {
            return Optional<TSource>.Null();
        }
    }
    
    /// <summary>
    /// Returns an <see cref="Optional{T}"/> containing the first matching element of this sequence, or an empty optional of no such element exists.
    /// </summary>
    /// <param name="source">A sequence of values.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <typeparam name="TSource">The type of elements of the source.</typeparam>
    /// <returns>An <see cref="Optional{T}"/> containing the matching element, or an empty Optional otherwise.</returns>
    /// <exception cref="ArgumentNullException">source or predicate functions are null.</exception>
    public static Optional<TSource> FirstOrOptional<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        try
        {
            return Optional<TSource>.NonNull(source.First(predicate));
        }
        catch (InvalidOperationException)
        {
            return Optional<TSource>.Null();
        }
    }

    /// <summary>
    /// Returns the index of the first element in a sequence that matches a specific predicate, or -1 if no such item exists
    /// </summary>
    /// <param name="source">A sequence of values</param>
    /// <param name="predicate">A function to test each element for a condition</param>
    /// <typeparam name="TSource">The type of elemetns</typeparam>
    /// <returns>The index of the first matching element, or -1 if no matching element exists</returns>
    /// <exception cref="ArgumentNullException">source or predicate functions are null</exception>
    public static int IndexOf<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        int index = 0;
        foreach (var item in source) {
            if (predicate.Invoke(item)) {
                return index;
            }
            index++;
        }
        return -1;
    }

    public static bool Majority<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, bool trueWhenFiftyFifty = true)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        if (source is not ICollection collection) collection = source.ToArray();

        int threshold = Mathf.RoundToInt(collection.Count / 2f);
        int matching = 0;
        foreach (TSource item in collection)
        {
            if (predicate(item)) matching++;
            if (threshold % 2 == 1)
            {
                if (matching >= threshold) return true;
            }
            else
            {
                if (matching > threshold) return true;
                if (matching == threshold && trueWhenFiftyFifty) return true;
            }
        }

        return false;
    }

    public static string Fuse<TSource>(this IEnumerable<TSource> source, string delimiter = ", ")
    {
        return string.Join(delimiter, source);
    }

    public static IEnumerable<TSource> Sorted<TSource, TComparable>(this IEnumerable<TSource> source, Func<TSource, TComparable> sortMapper) where TComparable : IComparable
    {
        (TSource, TComparable)[] arr = source.Select(element => (element, sortMapper(element))).ToArray();
        Sort(arr, 0, arr.Length - 1);
        return arr.Select(tuple => tuple.Item1);
    }

    internal static void Debug<TSource>(this IEnumerable<TSource> source) where TSource : Object
    {
        log.Fatal($"Debugging: {source.Select(s => (s.name, s.TypeName())).StrJoin()}");
    }
    
    
    
    
    
    
    
    // A utility function to swap two elements
    private static void Swap<T, TComparable>(IList<(T ,TComparable)> list, int i, int j) where TComparable: IComparable
    {
        (list[i], list[j]) = (list[j], list[i]);
    }
 
    /* This function takes last element as pivot, places
         the pivot element at its correct position in sorted
         array, and places all smaller (smaller than pivot)
         to left of pivot and all greater elements to right
         of pivot */
    static int Partition<T, TComparable>(IList<(T, TComparable)> arr, int low, int high) where TComparable: IComparable
    {
 
        // pivot
        TComparable pivot = arr[high].Item2;

        // Index of smaller element and
        // indicates the right position
        // of pivot found so far
        int i = (low - 1);
 
        for (int j = low; j <= high - 1; j++)
        {

            // If current element is smaller
            // than the pivot
            if (arr[j].Item2.CompareTo(pivot) >= 0) continue;
            // Increment index of
            // smaller element
            i++;
            Swap(arr, i, j);
        }
        Swap(arr, i + 1, high);
        return i + 1;
    }
 
    /* The main function that implements QuickSort
                arr[] --> Array to be sorted,
                low --> Starting index,
                high --> Ending index
       */
    static void Sort<T, TComparable>(IList<(T, TComparable)> arr, int low, int high) where TComparable: IComparable
    {
        if (low >= high) return;
        // pi is partitioning index, arr[p]
        // is now at right place
        int pi = Partition(arr, low, high);
 
        // Separately sort elements before
        // partition and after partition
        Sort(arr, low, pi - 1);
        Sort(arr, pi + 1, high);
    }
}