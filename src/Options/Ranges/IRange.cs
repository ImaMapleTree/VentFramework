using System.Collections.Generic;

namespace VentLib.Options.Ranges;

public interface IRange<out T>
{
    IEnumerable<T> AsEnumerable();
}