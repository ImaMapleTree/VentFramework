using System.Collections;
using System.Collections.Generic;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using HarmonyLib;
using UnityEngine;

namespace VentLib.Utilities;

internal class AUCWrapper
{
    internal static AUCWrapper? Instance;
    private List<IEnumerator> coroutines = new();
    
    public AUCWrapper()
    {
        Instance = this;
    }

    public bool StartCoroutine(IEnumerator coroutine, out Coroutine? coroutineHandle)
    {
        coroutineHandle = null;
        if (AmongUsClient.Instance != null) 
            coroutineHandle = AmongUsClient.Instance.StartCoroutine(coroutine.WrapToIl2Cpp());
        else
            coroutines.Add(coroutine);
        return AmongUsClient.Instance;
    }

    public bool StartCoroutine(IEnumerator coroutine)
    {
        if (AmongUsClient.Instance != null) AmongUsClient.Instance.StartCoroutine(coroutine.WrapToIl2Cpp());
        else coroutines.Add(coroutine);
        return AmongUsClient.Instance;
    }

    internal void RunCached()
    {
        coroutines.Do(coroutine => AmongUsClient.Instance.StartCoroutine(coroutine.WrapToIl2Cpp()));
    }
}