using System.Collections.Generic;

namespace VentLib.Ranges;

public interface IRange<out T>
{
    IEnumerable<T> AsEnumerable();
}