using TMPro;
using UnityEngine;
using VentLib.Logging;
using VentLib.Options.Game.Interfaces;
using VentLib.Utilities.Extensions;

namespace VentLib.Options.Game.Impl;

public class GameOptionRenderer: IGameOptionRenderer
{
    private const float DefaultOffset = 2.75f;
    private static readonly Color[] Colors = { Color.green, Color.red, Color.blue };
    private float offset = DefaultOffset;

    public MenuInitialized Initialize(MenuInitializer initializer) => initializer.Initialize();

    public void RenderTabs(IGameOptionTab[] tabs)
    {
        tabs.ForEach((tab, i) =>
        {
            tab.GetPosition().Handle(position =>
            {
                tab.SetPosition(new Vector2(0.8f * (i - 1) - tabs.Length / 2f, position.y));
            }, () => VentLogger.Warn($"Could not render tab: {tab} ({i})"));
        });
    }

    public void PreRender(GameOptionProperties properties, RenderOptions renderOptions)
    {
        bool isTitle = properties.Source.IsTitle;

        
        properties.SpriteRenderer.transform.localScale = new Vector3(1.2f, 1f, 1f);
        properties.Behaviour.transform.FindChild("Title_TMP").GetComponent<RectTransform>().sizeDelta = new Vector2(3.5f, 0.37f);
        
        if (isTitle)
        {
            properties.PlusButton.gameObject.SetActive(false);
            properties.MinusButton.gameObject.SetActive(false);
            properties.Value.gameObject.SetActive(false);
            TextMeshPro text = properties.Behaviour.transform.GetComponentInChildren<TextMeshPro>();
            text.transform.localPosition += new Vector3(0.05f, -0.2f, 0f);
            return;
        }
        
        
        properties.PlusButton.localPosition += new Vector3(0.3f, 0f, 0f);
        properties.MinusButton.localPosition += new Vector3(0.3f, 0f, 0f);
        properties.Value.localPosition += new Vector3(0.3f, 0f, 0f);
        properties.Behaviour.transform.Find("Title_TMP").transform.localPosition += new Vector3(0.15f, 0f, 0f);
    }

    
    public void Render(GameOptionProperties properties, (int level, int index) info, RenderOptions renderOptions)
    {
        if (info.index == 0) offset = DefaultOffset;
        var option = properties.Source;
        int lvl = option.Level - 1;
        StringOption stringOption = option.Behaviour.Get();
        stringOption.gameObject.SetActive(true);
        
        Transform transform = stringOption.transform;
        SpriteRenderer render = properties.SpriteRenderer;//transform.Find("Background").GetComponent<SpriteRenderer>();
        if (lvl > 0)
        {
            render.color = Colors[Mathf.Clamp(((lvl - 1) % 3), 0, 2)];
            render.size = new Vector2((float)(4.8f - (lvl - 1) * 0.2), 0.45f); // was -0.95
            properties.Text.transform.localPosition = new Vector3(-0.885f + 0.23f * Mathf.Clamp(lvl - 1, 0, int.MaxValue), 0f);
            //transform.FindChild("Title_TMP").GetComponent<RectTransform>().sizeDelta = new Vector2(3.4f, 0.37f);
            render.transform.localPosition = new Vector3(0.1f + 0.11f * (lvl - 1), 0f);
        }
        
        if (option.IsTitle) render.color = Color.clear;

        Vector3 pos = transform.localPosition;
        offset -= option.IsHeader ? 0.75f : 0.5f;
        transform.localPosition = new Vector3(pos.x, offset, pos.z);
        
    }

    public void PostRender(GameOptionsMenu menu)
    {
        if (menu.name == "SliderInner") return;
        menu.transform.FindChild("../../GameGroup/Text").GetComponent<TextMeshPro>().SetText(menu.name);
        menu.GetComponentInParent<Scroller>().ContentYBounds.max = -offset - 1.5f;
    }

    public void Close()
    {
    }
}