using System;
using System.Linq;
using UnityEngine;
using VentLib.Logging;
using VentLib.Options.Game.Events;
using VentLib.Options.Game.Impl;
using VentLib.Options.Game.Interfaces;
using VentLib.Options.Game.Patches;
using VentLib.Options.Game.Tabs;
using VentLib.Utilities.Attributes;
using VentLib.Utilities.Collections;
using VentLib.Utilities.Extensions;
using VentLib.Utilities.Optionals;

namespace VentLib.Options.Game;

[LoadStatic]
public static class GameOptionController
{
    private static readonly IGameOptionTab[] BuiltinGameTabs = { new VanillaMainTab(), new VanillaRoleTab()};
    private static readonly OrderedSet<GameOptionTab> Tabs = new();
    public static RenderOptions RenderOptions { get; set; } = new();
    private static IGameOptionRenderer _renderer = new GameOptionRenderer();
    private static IGameOptionTab _currentTab;

    private static UnityOptional<GameObject> _settingsMenu = UnityOptional<GameObject>.Null();

    private static OrderedSet<Action<IControllerEvent>> _tabEvents = new();
    private static MenuInitialized _lastInitialized = new();

    internal static bool Enabled;

    static GameOptionController()
    {
        BuiltinGameTabs[0].AddEventListener(tb => CurrentTab = tb);
        BuiltinGameTabs[1].AddEventListener(tb => CurrentTab = tb);
        _currentTab = BuiltinGameTabs[0];
    }

    public static IGameOptionTab CurrentTab
    {
        get => _currentTab;
        set
        {
            SwitchTab(value);
            _currentTab = value;
        }
    }

    private static IGameOptionTab[] AllTabs() => BuiltinGameTabs.Concat(Tabs).ToArray();

    public static void Enable() => Enabled = true;
    
    public static void AddTab(GameOptionTab tab)
    {
        Tabs.Add(tab);
        tab.AddEventListener(tb => CurrentTab = tb);
        Refresh();
        tab.Show();
    }

    public static void RemoveTab(GameOptionTab tab)
    {
        Tabs.Remove(tab);
        Refresh();
        tab.Hide();
    }

    public static void ClearTabs()
    {
        Tabs.ToArray().ForEach(RemoveTab);
    }

    public static void SetRenderer(IGameOptionRenderer iRenderer)
    {
        _renderer = iRenderer;
    }

    public static void RegisterEventHandler(Action<IControllerEvent> eventHandler)
    {
        _tabEvents.Add(eventHandler);
    }
    
    internal static void HandleOpen()
    {
        if (!Enabled) return;
        _currentTab = BuiltinGameTabs[0];
        _lastInitialized = _renderer.Initialize(new MenuInitializer());
        
        _settingsMenu.OrElseSet(() => _lastInitialized.MainSettings);
        AllTabs().ForEach(tab => tab.Setup(_lastInitialized));

        _renderer.RenderTabs(AllTabs());
        
        OptionOpenEvent openEvent = new();
        _tabEvents.ForEach(handler => handler(openEvent));
    }

    internal static void Refresh()
    {
        if (!_lastInitialized.IsAlive())
        {
            VentLogger.Warn("Unable to Refresh Option Controller", "OptionController");
            return;
        }
        AllTabs().ForEach(tab => tab.Setup(_lastInitialized));
        _renderer.RenderTabs(AllTabs());
    }

    internal static void DoRender(GameOptionsMenu menu)
    {
        if (!Enabled) return;
        CurrentTab.GetOptions().ForEach(opt => opt.HideMembers());
        CurrentTab.PreRender().ForEach(RenderCheck);
        _renderer.PostRender(menu);
    }

    internal static void ValidateOptionBehaviour(GameOption option, bool preRender = true)
    {
        option.Behaviour.IfNotPresent(() =>
        {
            var template = OptionOpenPatch.Template.OrElse(null!);
            if (template == null) throw new NullReferenceException("Template not found during ValidateOptionBehaviour");
            
            string name = template.name;
            template.name = "Modded";

            StringOption stringOption = CurrentTab.InitializeOption(template);
            template.name = name;

            stringOption.OnValueChanged = new Action<OptionBehaviour>(_ => { });
            stringOption.TitleText.text = option.Name();
            stringOption.ValueText.text = option.GetValueText();
            stringOption.Value = stringOption.oldValue = -1;

            option.Behaviour = UnityOptional<StringOption>.NonNull(stringOption);

            GameOptionProperties properties = option.Properties.OrElseGet(() => new GameOptionProperties(option));
            if (preRender) _renderer.PreRender(properties, RenderOptions);
            option.BindPlusMinusButtons(properties);
        });
    }
    
    private static void RenderCheck(GameOption option, int index)
    {
        ValidateOptionBehaviour(option);
        _renderer.Render(option.Properties.OrElseGet(() => new GameOptionProperties(option)), (option.Level, index), RenderOptions);
    }
    
    private static void SwitchTab(IGameOptionTab newTab)
    {
        _currentTab.Deactivate();
        newTab.Activate();
        TabChangeEvent changeEvent = new(_currentTab, newTab);
        _tabEvents.ForEach(handler => handler(changeEvent));
    }
}