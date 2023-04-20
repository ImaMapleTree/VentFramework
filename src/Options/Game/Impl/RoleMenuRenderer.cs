using System.Collections.Generic;
using UnityEngine;
using VentLib.Options.Game.Interfaces;

namespace VentLib.Options.Game.Impl;

public class RoleMenuRenderer : IVanillaMenuRenderer
{
    public void Render(MonoBehaviour menu, List<GameOption> customOptions, IEnumerable<OptionBehaviour> vanillaOptions, RenderOptions renderOptions)
    {
        throw new System.NotImplementedException();
    }
}