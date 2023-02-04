using System;
using System.Collections.Generic;
using System.Linq;

namespace VentLib.Utilities.Extensions;

public static class EnumerableExtensions
{
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
}