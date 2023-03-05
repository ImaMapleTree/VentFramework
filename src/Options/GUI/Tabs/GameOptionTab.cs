using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VentLib.Logging;
using VentLib.Utilities.Collections;
using VentLib.Utilities.Extensions;
using VentLib.Utilities.Optionals;
using Object = UnityEngine.Object;

namespace VentLib.Options.GUI.Tabs;

public class GameOptionTab : IGameOptionTab
{
    private List<GameOption> Options { get; } = new();
    private string name;
    private Func<Sprite> spriteSupplier;

    private UnityOptional<Sprite> sprite = UnityOptional<Sprite>.Null();
    private UnityOptional<GameObject> tabButton = UnityOptional<GameObject>.Null();
    
    private UnityOptional<GameObject> relatedMenu = UnityOptional<GameObject>.Null();
    private UnityOptional<GameOptionsMenu> innerMenu = UnityOptional<GameOptionsMenu>.Null();

    private OrderedSet<Action<IGameOptionTab>> callbacks = new();

    public GameOptionTab(string name, Func<Sprite> spriteSupplier)
    {
        this.name = name;
        this.spriteSupplier = spriteSupplier;
    }

    public void AddEventListener(Action<IGameOptionTab> callback) => callbacks.Add(callback);

    public void AddOption(GameOption option)
    {
        if (Options.Contains(option)) return;
        Options.Add(option);
    }

    public void RemoveOption(GameOption option) => Options.Remove(option);

    public void HandleClick()
    {
        callbacks.ForEach(cb => cb(this));
    }

    public StringOption InitializeOption(StringOption sourceBehavior)
    {
        if (!innerMenu.Exists()) throw new ArgumentException("Cannot Initialize Behaviour because menu does not exist");
        return Object.Instantiate(sourceBehavior, innerMenu.Get().transform);
    }

    public void Setup(MenuInitialized initialized)
    {
        var main = tabButton.OrElseSet(() => Object.Instantiate(initialized.RoleTab, initialized.RoleTab.transform.parent));
        
        var menu = relatedMenu.OrElseSet(() =>
        {
            GameObject copy = Object.Instantiate(initialized.MainSettings, initialized.MainSettings.transform.parent);
            copy.name = name;
            copy.transform.FindChild("BackPanel").transform.localScale = copy.transform.FindChild("Bottom Gradient").transform.localScale = new Vector3(1.2f, 1f, 1f);
            copy.transform.FindChild("Background").transform.localScale = new Vector3(1.3f, 1f, 1f);
            copy.transform.FindChild("UI_Scrollbar").transform.localPosition += new Vector3(0.35f, 0f, 0f);
            copy.transform.FindChild("UI_ScrollbarTrack").transform.localPosition += new Vector3(0.35f, 0f, 0f);
            copy.transform.FindChild("GameGroup/SliderInner").transform.localPosition += new Vector3(-0.15f, 0f, 0f);
            return copy;
        });
        
        innerMenu.OrElseSet(() =>
        {
            GameOptionsMenu copy = menu.transform.FindChild("GameGroup/SliderInner").GetComponent<GameOptionsMenu>();
            copy.name = name;
            copy.GetComponentsInChildren<OptionBehaviour>().ForEach(x => Object.Destroy(x.gameObject));
            menu.SetActive(false);
            return copy;
        });

        GetTabRenderer().IfPresent(render => render.sprite = sprite.OrElseSet(spriteSupplier));
        
        var button = main.GetComponentInChildren<PassiveButton>();
        button.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
        button.OnClick.AddListener((Action)HandleClick);
    }

    public void SetPosition(Vector2 position)
    {
        tabButton.IfPresent(btn => btn.transform.localPosition = position);
    }

    public List<GameOption> PreRender() => Options.SelectMany(opt => opt.GetDisplayedMembers()).ToList();

    public Optional<Vector3> GetPosition() => tabButton.Map(btn => btn.transform.localPosition);
    
    public List<GameOption> GetOptions() => Options;

    public void Activate()
    {
        VentLogger.Info($"Activated Tab \"{name}\"", "TabSwitch");
        GetTabHighlight().IfPresent(highlight => highlight.enabled = true);
        relatedMenu.IfPresent(menu => menu.SetActive(true));
    }

    public void Deactivate()
    {
        VentLogger.Debug($"Deactivated Tab \"{name}\"", "TabSwitch");
        GetTabHighlight().IfPresent(highlight => highlight.enabled = false);
        relatedMenu.IfPresent(menu => menu.SetActive(false));
    }

    public void Show()
    {
        tabButton.IfPresent(button => button.SetActive(true));
    }

    public void Hide()
    {
        tabButton.IfPresent(button => button.SetActive(false));
    }


    private UnityOptional<SpriteRenderer> GetTabRenderer() => tabButton.UnityMap(button => button.transform.FindChild("Hat Button").FindChild("Icon").GetComponent<SpriteRenderer>());
    
    private UnityOptional<SpriteRenderer> GetTabHighlight() => tabButton.UnityMap(button => button.transform.FindChild("Hat Button").FindChild("Tab Background").GetComponent<SpriteRenderer>());
}