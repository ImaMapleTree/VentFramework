using UnityEngine;
using VentLib.Logging;
using VentLib.Options.GUI.Interfaces;
using VentLib.Options.GUI.Tabs;
using VentLib.Utilities.Extensions;

namespace VentLib.Options.GUI.Impl;

public class Renderer: IRenderer
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
        properties.SpriteRenderer.transform.localScale = new Vector3(1.2f, 1f, 1f);
        properties.PlusButton.localPosition += new Vector3(0.3f, 0f, 0f);
        properties.MinusButton.localPosition += new Vector3(0.3f, 0f, 0f);
        properties.Value.localPosition += new Vector3(0.3f, 0f, 0f);
        properties.Behaviour.transform.Find("Title_TMP").transform.localPosition += new Vector3(0.15f, 0f, 0f);
        properties.Behaviour.transform.FindChild("Title_TMP").GetComponent<RectTransform>().sizeDelta = new Vector2(3.5f, 0.37f);
    }

    
    public void Render(GameOptionProperties properties, (int level, int index) info, RenderOptions renderOptions)
    {
        if (info.index == 0) offset = DefaultOffset;
        var option = properties.Source;
        int lvl = option.Level - 1;
        option.Behaviour.Get().gameObject.SetActive(true);
        Transform transform = option.Behaviour.Get().transform;
        SpriteRenderer render = option.Behaviour.Get().transform.Find("Background").GetComponent<SpriteRenderer>();
        if (lvl > 0)
        {
            render.color = Colors[Mathf.Clamp(((lvl - 1) % 3), 0, 2)];
            render.size = new Vector2((float)(4.8f - ((lvl - 1) * 0.2)), 0.45f);
            option.Behaviour.Get().transform.Find("Title_TMP").transform.localPosition = new Vector3(-0.95f + (0.23f * (Mathf.Clamp(lvl - 1, 0, int.MaxValue))), 0f);
            option.Behaviour.Get().transform.FindChild("Title_TMP").GetComponent<RectTransform>().sizeDelta = new Vector2(3.4f, 0.37f);
            render.transform.localPosition = new Vector3(0.1f + (0.11f * (lvl - 1)), 0f);
        }

        Vector3 pos = transform.localPosition;
        offset -= option.IsHeader ? 0.75f : 0.5f;
        transform.localPosition = new Vector3(pos.x, offset, pos.z);
    }

    public void PostRender(GameOptionsMenu menu)
    {
        if (menu.name == "SliderInner") return;
        menu.transform.FindChild("../../GameGroup/Text").GetComponent<TMPro.TextMeshPro>().SetText(menu.name);
        menu.GetComponentInParent<Scroller>().ContentYBounds.max = -offset - 1.5f;
    }

    public void Close()
    {
    }
}