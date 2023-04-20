using System.Collections.Generic;
using UnityEngine;

namespace VentLib.Options.Game.Interfaces;

public interface IVanillaMenuRenderer
{
    internal void Render(MonoBehaviour menu, List<GameOption> customOptions, IEnumerable<OptionBehaviour> vanillaOptions, RenderOptions renderOptions);
}