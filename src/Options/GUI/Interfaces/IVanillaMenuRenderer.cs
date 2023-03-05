using System.Collections.Generic;

namespace VentLib.Options.GUI.Interfaces;

public interface IVanillaMenuRenderer
{
    internal void Render(List<GameOption> customOptions, IEnumerable<OptionBehaviour> vanillaOptions, RenderOptions renderOptions);
}