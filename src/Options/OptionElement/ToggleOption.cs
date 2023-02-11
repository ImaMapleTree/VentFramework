/*using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VentLib.Logging;
using VentLib.Utilities.Extensions;
using Object = UnityEngine.Object;

namespace VentLib.Options.OptionElement;

public class ToggleOption: Option
{
    private static Color[] _colors = { new(0.63f, 0.74f, 0.69f), new(0.51f, 0.62f, 0.56f)};
    internal PassiveButton? PassiveButton;
    internal bool IsHovered;
    
    internal override void RenderInit(Transform? transform = null)
    {
        transform ??= _transformAssigner != null ? _transformAssigner(this) : SharedGameObject.transform;
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
        PassiveButton = Object.Instantiate(Behaviour!.GetComponentsInChildren<PassiveButton>()[0], Behaviour.transform);

        var localScale = PassiveButton.transform.localScale;
        PassiveButton!.transform.localScale = new Vector3(27, localScale.y + 0.5f, localScale.z);
        /*PassiveButton.GetComponentsInChildren<Component>().Do(c => VentLogger.Fatal($"Component: {c.GetIl2CppType().FullName}"));
        PassiveButton.GetComponentInChildren<MeshRenderer>().material.color = new Color(0, 0, 0, 0);
        PassiveButton.GetComponentInChildren<MeshRenderer>().enabled = false;#1#

        AddButtonListeners();
        List<PassiveUiElement> passiveElements = PassiveButtonManager.Instance.Buttons.ToArray().ToList();
        CreateDropdownOptions(transform, PassiveButton);
        passiveElements.AddRange(SubOptions.Cast<ToggleOption>().Select(o => o.PassiveButton!));
        PassiveButtonManager.Instance.Buttons.Clear();
        passiveElements.Do(e => PassiveButtonManager.Instance.Buttons.Add(e));
    }

    public override List<Option> ActiveOptions(bool isActive = true, bool isParentFalse = false) => !isActive ? new List<Option>() : new List<Option> { this };

    public override void Render(ref float offset)
    {
        float _offset = offset;
        Vector3 parentPosition = Behaviour!.transform.localPosition;
        foreach ((int index, ToggleOption option) in SubOptions.Cast<ToggleOption>().Indexed())
        {
            option.Behaviour!.gameObject.SetActive(IsHovered);
            option.Behaviour.TitleText.text = option.ColorName;
            if (!IsHovered) continue;
            Transform transform = option.Behaviour.transform;
            SpriteRenderer render = transform.Find("Background").GetComponent<SpriteRenderer>();
            render.color = option.IsHovered ?  new Color(0.22f, 0.74f, 0.22f) : _colors[index % 2];
            render.size = new Vector2((float)(4.8f - ((option.Level - 1) * 0.2)), 0.45f);
            transform.Find("Title_TMP").transform.localPosition = new Vector3(-0.95f + (0.23f * (Mathf.Clamp(option.Level - 1, 0, Int32.MaxValue))), 0f);
            render.transform.localPosition = new Vector3(0.1f + (0.11f * (option.Level - 1)), 0f);
            _offset -= 0.465f;
            transform.localPosition = new Vector3(parentPosition.x + 0.11f, _offset, parentPosition.z);
            render.sortingOrder = 100;
            option.Behaviour.TitleText.sortingOrder = 101;
        }
    }

    private void CreateDropdownOptions(Transform transform, PassiveButton passiveButton)
    {
        SubOptions.Clear();
        if (Values.Count == 0) return;
        SubOptions.AddRange(Values.Select((v, i) =>
            {
                var opt = new ToggleOption
                {
                    Name = v.ToString(),
                    SaveOnChange = false,
                    Level = Level + 1,
                    Behaviour = Object.Instantiate(StringOption, transform),
                };

                opt.PassiveButton = Object.Instantiate(passiveButton, opt.Behaviour!.transform);

                opt.Behaviour!.transform.FindChild("Background").localScale = new Vector3(1.2f, 1f, 1f);
                opt.Behaviour.transform.FindChild("Title_TMP").localPosition += new Vector3(0.15f, 0f, 0f);
                opt.Behaviour.transform.FindChild("Plus_TMP").gameObject.SetActive(false);
                opt.Behaviour.transform.FindChild("Minus_TMP").gameObject.SetActive(false);
                
                
                //PassiveButtonManager.Instance.RegisterOne(opt.PassiveButton);
                opt.PassiveButton!.OnClick = new Button.ButtonClickedEvent();
                /*opt.PassiveButton.OnClick.RemoveAllListeners();
                opt.PassiveButton.OnMouseOver.RemoveAllListeners();
                opt.PassiveButton.OnMouseOut.RemoveAllListeners();#1#
                
                void OnClick()
                {
                    VentLogger.Fatal($"Clicked: {i}");
                    SetValue(i);
                }

                opt.PassiveButton.OnClick.AddListener((UnityAction)OnClick);
                opt.AddButtonListeners(this);
                opt.SetRendererAlphas(10);
                
                var localScale = opt.PassiveButton.transform.localScale;
                /*opt.PassiveButton!.transform.localScale = new Vector3(27, localScale.y + 0.5f, localScale.z);
                opt.PassiveButton.GetComponentsInChildren<Component>().Do(c => VentLogger.Fatal($"Component: {c.GetIl2CppType().FullName}"));
                opt.PassiveButton.GetComponentInChildren<MeshRenderer>().material.color = new Color(0, 0, 0, 0);
                opt.PassiveButton.GetComponentInChildren<MeshRenderer>().enabled = false;#1#
                return opt;
            }
        ));
    }

    private void AddButtonListeners(ToggleOption? parent = null)
    {
        parent ??= this;
        PassiveButton.OnMouseOver.AddListener((UnityAction)(Action)(() =>
        {
            IsHovered = true;
            parent.IsHovered = true;
        }));
        PassiveButton.OnMouseOut.AddListener((UnityAction)(Action)(() =>
        {
            IsHovered = false;
            parent.IsHovered = false;
        }));
    }
    
    private void SetRendererAlphas(float alpha) {
        SpriteRenderer render = Behaviour!.transform.Find("Background").GetComponent<SpriteRenderer>();
        foreach (Material material in render.materials)
        {
            material.color = new Color(material.color.r, material.color.g, material.color.b, alpha);
        }
    }
}*/