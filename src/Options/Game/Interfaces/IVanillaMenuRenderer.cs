using System.Collections.Generic;

namespace VentLib.Options.Game.Interfaces;

public interface IVanillaMenuRenderer
{
    internal void Render(List<GameOption> customOptions, IEnumerable<OptionBehaviour> vanillaOptions, RenderOptions renderOptions);
}