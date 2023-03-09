using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VentLib.Options.Game.Impl;
using VentLib.Options.Game.Interfaces;
using VentLib.Utilities.Optionals;
using Object = UnityEngine.Object;

namespace VentLib.Options.Game.Tabs;

public sealed class VanillaMainTab : VanillaTab
{
    public static VanillaMainTab Instance = null!;
    private static IVanillaMenuRenderer _renderer = new MainMenuRenderer();
    
    private UnityOptional<SpriteRenderer> highlight = UnityOptional<SpriteRenderer>.Null();
    private UnityOptional<GameOptionsMenu> innerMenu = UnityOptional<GameOptionsMenu>.Null();

    public VanillaMainTab()
    {
        Instance = this;
    }

    public static void SetRenderer(IVanillaMenuRenderer renderer)
    {
        _renderer = renderer;
    }
    
    public override StringOption InitializeOption(StringOption sourceBehavior)
    {
        if (!innerMenu.Exists()) throw new ArgumentException("Cannot Initialize Behaviour because menu does not exist");
        return Object.Instantiate(sourceBehavior, innerMenu.Get().transform);
    }

    public override void Setup(MenuInitialized initialized)
    {
        TabButton = UnityOptional<GameObject>.NonNull(initialized.GameTab);
        RelatedMenu = UnityOptional<GameObject>.NonNull(initialized.GameSettingMenu.RegularGameSettings);
        highlight = UnityOptional<SpriteRenderer>.Of(initialized.GameSettingMenu.GameSettingsHightlight);
        innerMenu = RelatedMenu.UnityMap(menu => menu.transform.GetComponentsInChildren<Transform>().First(c => c.name.Equals("SliderInner")).GetComponent<GameOptionsMenu>());

        var button = initialized.GameTab.GetComponentInChildren<PassiveButton>();
        button.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
        button.OnClick.AddListener((Action)HandleClick);
    }

    public override List<GameOption> PreRender()
    {
        var returnList = new List<GameOption>();
        if (!innerMenu.Exists()) return returnList;

        List<GameOption> options = GetOptions().SelectMany(opt => opt.GetDisplayedMembers()).ToList();
        
        if (options.Count == 0) return returnList;
        var menu = innerMenu.Get();
        
        options.ForEach(opt => GameOptionController.ValidateOptionBehaviour(opt, false));
        _renderer.Render(options, menu.Children.Skip(1), GameOptionController.RenderOptions);
        
        return new List<GameOption>();
    }
    

    protected override UnityOptional<SpriteRenderer> Highlight() => highlight;
}