using System.Collections.Generic;
using UnityEngine;
using VentLib.Options.Game.Interfaces;
using VentLib.Utilities.Extensions;

namespace VentLib.Options.Game.Impl;

public class MainMenuRenderer : IVanillaMenuRenderer
{
    private static float _originalYBounds = -1f;
    
    public void Render(MonoBehaviour menu, List<GameOption> customOptions, IEnumerable<OptionBehaviour> vanillaOptions, RenderOptions renderOptions)
    {
        int optionCount = customOptions.Count;
        if (customOptions.Count == 0) return;
        float offset = 2.35f;
        customOptions.ForEach(opt => CustomOptionRender(opt.Properties.OrElseGet(() => new GameOptionProperties(opt)), opt.Level, ref offset));
        
        Vector3 pos = customOptions[0].Behaviour.Get().transform.localPosition;
        
        vanillaOptions.ForEach(opt =>
        {
            pos = new Vector3(pos.x, pos.y - 0.5f, pos.z);
            opt.transform.localPosition = pos;
        });

        if (_originalYBounds < 0) _originalYBounds = menu.GetComponentInParent<Scroller>().ContentYBounds.max;
        menu.GetComponentInParent<Scroller>().ContentYBounds.max = _originalYBounds + 0.5f * optionCount;
    }
    
    private void CustomOptionRender(GameOptionProperties properties, int level, ref float offset)
    {
        properties.SetActive(true);
        if (level > 1)
        {
            properties.Behaviour.transform.Find("Title_TMP").localPosition = new Vector3(-0.95f + 0.23f * Mathf.Clamp(level - 1, 0, int.MaxValue), 0f);
            properties.SetPosition(new Vector3(0.1f + 0.11f * (level - 1), 0f));
        }

        Vector3 pos = properties.GetPosition();
        offset -= properties.Source.IsHeader ? 0.75f : 0.5f;
        properties.SetPosition(new Vector3(pos.x, offset, pos.z));
    }
    
    
    
}