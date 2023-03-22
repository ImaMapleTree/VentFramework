using System;
using System.Collections.Generic;

namespace VentLib.Options;

public class SubOptions : List<Option>
{
    internal Func<object, bool> Predicate = _ => true;

    public void SetPredicate(Func<object, bool> predicate)
    {
        Predicate = predicate;
    }

    public bool Matches(object value) => Predicate(value);

    public List<Option> GetConditionally(object value) => Predicate(value) ? this : new List<Option>();
}