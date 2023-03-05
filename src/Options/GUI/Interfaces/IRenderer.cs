using VentLib.Options.GUI.Tabs;

namespace VentLib.Options.GUI.Interfaces;

public interface IRenderer
{
    MenuInitialized Initialize(MenuInitializer initializer);

    void RenderTabs(IGameOptionTab[] tabs);

    void PreRender(GameOptionProperties properties, RenderOptions renderOptions);

    void Render(GameOptionProperties properties, (int level, int index) info, RenderOptions renderOptions);

    void PostRender(GameOptionsMenu menu);

    void Close();
}