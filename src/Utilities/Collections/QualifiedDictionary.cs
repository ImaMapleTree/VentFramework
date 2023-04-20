using System.Collections.Generic;
using VentLib.Utilities.Extensions;

namespace VentLib.Utilities.Collections;

public class QualifiedDictionary: Dictionary<string, object>
{
    public QualifiedDictionary(IDictionary<string, object> dictionary) : base(dictionary)
    {
    }

    public QualifiedDictionary()
    {
    }
    
    public object Get(string[] qualifier)
    {
        Dictionary<string, object> dictionary = this;
        for (int i = 0; i < qualifier.Length - 1; i++)
        {
            string key = qualifier[i];
            object? value = dictionary[key];
            if (value is Dictionary<string, object> vanillaDict) dictionary = vanillaDict;
            if (value is Dictionary<object, object> objObjDict) dictionary = objObjDict.ToDict(k => (string)k.Key, k => k.Value);
        }

        return dictionary[qualifier[^1]];
    }

    public object Get(string qualifier) => Get(SplitQualifier(qualifier));

    public bool TryGet(string[] qualifier, out object? value)
    {
        Dictionary<string, object> dictionary = this;
        
        value = default;
        
        try
        {
            for (int i = 0; i < qualifier.Length - 1; i++)
            {
                string key = qualifier[i];
                object newValue = dictionary[key];
                if (newValue is Dictionary<string, object> vanillaDict) dictionary = vanillaDict;
                else if (newValue is Dictionary<object, object> objObjDict) dictionary = objObjDict.ToDict(k => (string)k.Key, k => k.Value);
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
        Dictionary<string, object> dictionary = this;
        for (int i = 0; i < qualifier.Length - 1; i++)
        {
            string key = qualifier[i];
            object? dynamicValue = dictionary.GetValueOrDefault(key);
            if (dynamicValue == null && createMissingKeys) dictionary = (Dictionary<string, object>)(dictionary[key] = new Dictionary<string, object>());
            else if (dynamicValue is Dictionary<string, object> vanillaDict) dictionary = vanillaDict;
            else if (dynamicValue is Dictionary<object, object> objObjDict) dictionary = objObjDict.ToDict(k => (string)k.Key, k => k.Value);
        }

        dictionary[qualifier[^1]] = value;
    }

    public void Set(string qualifier, object value, bool createMissingKeys = false) => Set(SplitQualifier(qualifier), value, createMissingKeys);

    private string[] SplitQualifier(string qualifier) => qualifier.Split(".");
}