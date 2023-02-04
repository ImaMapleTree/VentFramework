using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using VentLib.Utilities.Extensions;
using Object = UnityEngine.Object;

namespace VentLib.Options;

public partial class Option
{
    public static GameObject SharedGameObject;
    internal static StringOption? StringOption;
    private static Func<Option, Transform>? _transformAssigner;

    static Option()
    {
        SharedGameObject = new GameObject();
        SharedGameObject.DontDestroy();
        SharedGameObject.DontUnload();
        SharedGameObject.DontDestroyOnLoad();
        SharedGameObject.SetActiveRecursively(false);
    }
    
    public StringOption? Behaviour;
    public Transform? ParentTransform;
    internal bool SkipRender;
    internal bool NoRender;
    

    public List<Option> ActiveOptions(bool isActive = true, bool isParentFalse = false)
    {
        List<Option> active = new List<Option>();
        isActive = isActive && !isParentFalse;
        if (isActive) active.Add(this);
        
        Behaviour!.gameObject.SetActive(false);
        
        active.AddRange(SubOptions.SelectMany(sub => sub.ActiveOptions(MatchesPredicate(), !isActive)));
        return active;
    }

    public static void SetTransformAssigner(Func<Option, Transform> assigner) => _transformAssigner = assigner;

    internal void RenderInit(Transform? transform = null)
    {
        transform ??= _transformAssigner != null ? _transformAssigner(this) : SharedGameObject.transform;
        SubOptions.Do(sub => sub.RenderInit(transform));
        if (SkipRender || NoRender) return;
        SkipRender = true;
        ParentTransform = transform;
        Behaviour = Object.Instantiate(StringOption, transform)!;
        Behaviour.name = ColorName;
        Behaviour.TitleText.text = ColorName;
        Behaviour.ValueText.text = GetValueAsString();
        Behaviour.Value = 0;
        Behaviour.transform.FindChild("Background").localScale = new Vector3(1.2f, 1f, 1f);
        Behaviour.transform.FindChild("Plus_TMP").localPosition += new Vector3(0.3f, 0f, 0f);
        Behaviour.transform.FindChild("Minus_TMP").localPosition += new Vector3(0.3f, 0f, 0f);
        Behaviour.transform.FindChild("Value_TMP").localPosition += new Vector3(0.3f, 0f, 0f);
        Behaviour.transform.FindChild("Title_TMP").localPosition += new Vector3(0.15f, 0f, 0f);
        Behaviour.transform.FindChild("Title_TMP").GetComponent<RectTransform>().sizeDelta = new Vector2(3.5f, 0.37f);
    }

    public override string ToString() => $"Option(Name={Name}, Key={Key}, Value={GetValue()}, SubOptions={SubOptions.Count})";
}