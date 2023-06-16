using System.Linq;
using UnityEngine;
using VentLib.Utilities.Optionals;

namespace VentLib.Utilities.Extensions;

///
/// Class taken from Reactor
/// https://github.com/NuclearPowered/Reactor/blob/master/Reactor/Utilities/Extensions/UnityObjectExtensions.cs
/// All credit goes to js6pak and AlexejheroYTB for this code
/// This exists to bridge the gap between Reactor and VentFramework so that mod authors don't need to make their own utils class
/// just for these methods
/// 


/// <summary>
/// Provides extension methods for <see cref="Object"/>.
/// </summary>
public static class UnityObjectExtensions
{
    /// <summary>
    /// Stops <paramref name="obj"/> from being destroyed.
    /// </summary>
    /// <param name="obj">The object to stop from being destroyed.</param>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <returns>Passed <paramref name="obj"/>.</returns>
    public static T DontDestroy<T>(this T obj) where T : Object
    {
        obj.hideFlags |= HideFlags.HideAndDontSave;

        return obj.DontDestroyOnLoad();
    }

    /// <summary>
    /// Stops <paramref name="obj"/> from being unloaded.
    /// </summary>
    /// <param name="obj">The object to stop from being unloaded.</param>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <returns>Passed <paramref name="obj"/>.</returns>
    public static T DontUnload<T>(this T obj) where T : Object
    {
        obj.hideFlags |= HideFlags.DontUnloadUnusedAsset;

        return obj;
    }

    /// <summary>
    /// Stops <paramref name="obj"/> from being destroyed on load.
    /// </summary>
    /// <param name="obj">The object to stop from being destroyed on load.</param>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <returns>Passed <paramref name="obj"/>.</returns>
    public static T DontDestroyOnLoad<T>(this T obj) where T : Object
    {
        Object.DontDestroyOnLoad(obj);

        return obj;
    }

    /// <summary>
    /// Destroys the <paramref name="obj"/>.
    /// </summary>
    /// <param name="obj">The object to destroy.</param>
    public static void Destroy(this Object obj)
    {
        Object.Destroy(obj);
    }

    /// <summary>
    /// Destroys the <paramref name="obj"/> immediately.
    /// </summary>
    /// <param name="obj">The object to destroy immediately.</param>
    public static void DestroyImmediate(this Object obj)
    {
        Object.DestroyImmediate(obj);
    }

    public static string TypeName(this Object obj) => obj.GetIl2CppType().FullName;

    public static T FindChild<T>(this MonoBehaviour obj, string name, bool includeInactive = false) where T: Object
    {
        return obj.GetComponentsInChildren<T>(includeInactive).First(c => c.name == name);
    }

    public static T FindChild<T>(this GameObject obj, string name, bool includeInactive = false) where T: Object
    {
        return obj.GetComponentsInChildren<T>(includeInactive).First(c => c.name == name);
    }
    
    public static UnityOptional<T> FindChildOrEmpty<T>(this MonoBehaviour obj, string name, bool includeInactive = false) where T: Object
    {
        return new UnityOptional<T>(obj.GetComponentsInChildren<T>(includeInactive).FirstOrDefault(c => c.name == name));
    }
    
    public static UnityOptional<T> FindChildOrEmpty<T>(this GameObject obj, string name, bool includeInactive = false) where T: Object
    {
        return new UnityOptional<T>(obj.GetComponentsInChildren<T>(includeInactive).FirstOrDefault(c => c.name == name));
    }
}