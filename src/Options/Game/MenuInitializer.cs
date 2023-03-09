using System;
using System.Linq;
using UnityEngine;
using VentLib.Utilities.Optionals;
using Object = UnityEngine.Object;

namespace VentLib.Options.Game;

public class MenuInitializer
{
    private GameObject gameTab = GameObject.Find("GameTab");
    private GameObject roleTab = GameObject.Find("RoleTab");
    private GameObject mainSettings = GameObject.Find("Game Settings");
    private GameSettingMenu gameSettingMenu = Object.FindObjectsOfType<GameSettingMenu>().First();

    internal MenuInitializer()
    {
    }

    public MenuInitializer GameTab(Func<GameObject, GameObject> gameTabSetter)
    {
        gameTab = gameTabSetter(gameTab);
        return this;
    }
    public MenuInitializer RoleTab(Func<GameObject, GameObject> roleTabSetter)
    {
        roleTab = roleTabSetter(roleTab);
        return this;
    }
    public MenuInitializer MainSettings(Func<GameObject, GameObject> settingsSetter)
    {
        mainSettings = settingsSetter(mainSettings);
        return this;
    }

    public MenuInitializer GameSettingMenu(Func<GameSettingMenu, GameSettingMenu> gameSettingMenuSetter)
    {
        gameSettingMenu = gameSettingMenuSetter(gameSettingMenu);
        return this;
    }

    public MenuInitialized Initialize()
    {
        return new MenuInitialized
        {
            GameTab = gameTab,
            RoleTab = roleTab,
            MainSettings = mainSettings,
            GameSettingMenu = gameSettingMenu,
            Status1 = UnityOptional<GameObject>.NonNull(gameTab),
            Status2 = UnityOptional<GameObject>.NonNull(roleTab),
        };
    }

}

public class MenuInitialized
{
    public GameObject GameTab { get; internal init; } = null!;
    public GameObject RoleTab { get; internal init; } = null!;
    public GameObject MainSettings { get; internal init; } = null!;
    public GameSettingMenu GameSettingMenu { get; internal init; } = null!;
    internal UnityOptional<GameObject> Status1 { private get; init; } = UnityOptional<GameObject>.Null();
    internal UnityOptional<GameObject> Status2 { private get; init; } = UnityOptional<GameObject>.Null();

    internal MenuInitialized()
    {
    }

    public bool IsAlive() => Status1.Exists() && Status2.Exists();
}