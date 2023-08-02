using System.Collections.Generic;

namespace VentLib.Utilities.Collections;

internal class QualifiedDictionary: Dictionary<object, object>
{
    public QualifiedDictionary(IDictionary<object, object> dictionary) : base(dictionary)
    {
    }

    public QualifiedDictionary()
    {
    }
    
    public object Get(string[] qualifier)
    {
        Dictionary<object, object> dictionary = this;
        for (int i = 0; i < qualifier.Length - 1; i++)
        {
            string key = qualifier[i];
            object value = dictionary[key];
            if (value is Dictionary<object, object> vanillaDict) dictionary = vanillaDict;
        }

        return dictionary[qualifier[^1]];
    }

    public object Get(string qualifier) => Get(SplitQualifier(qualifier));

    public bool TryGet(string[] qualifier, out object? value)
    {
        Dictionary<object, object> dictionary = this;
        
        value = default;
        
        try
        {
            for (int i = 0; i < qualifier.Length - 1; i++)
            {
                string key = qualifier[i];
                object newValue = dictionary[key];
                if (newValue is Dictionary<object, object> vanillaDict) dictionary = vanillaDict;
            }

            value = dictionary[qualifier[^1]];
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool TryGet(string qualifier, out object? value) => TryGet(SplitQualifier(qualifier), out value);

    public void Set(string[] qualifier, object value, bool createMissingKeys = false)
    {
        Dictionary<object, object> dictionary = this;
        for (int i = 0; i < qualifier.Length - 1; i++)
        {
            string key = qualifier[i];
            object? dynamicValue = dictionary.GetValueOrDefault(key);
            if (dynamicValue == null && createMissingKeys)
            {
                dictionary[key] = new Dictionary<object, object>();
                dictionary = (Dictionary<object, object>)dictionary[key];
            }
            else if (dynamicValue is Dictionary<object, object> vanillaDict) dictionary = vanillaDict;
        }

        dictionary[qualifier[^1]] = value;
    }

    public void Set(string qualifier, object value, bool createMissingKeys = false) => Set(SplitQualifier(qualifier), value, createMissingKeys);

    private string[] SplitQualifier(string qualifier) => qualifier.Split(".");
}