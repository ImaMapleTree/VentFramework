using System;
using System.Collections.Generic;
using UnityEngine;
using VentLib.Logging;
using VentLib.Options.Game.Interfaces;
using VentLib.Utilities.Collections;
using VentLib.Utilities.Optionals;

namespace VentLib.Options.Game.Tabs;

public abstract class VanillaTab : IGameOptionTab
{
    protected UnityOptional<GameObject> TabButton = UnityOptional<GameObject>.Null();
    protected UnityOptional<GameObject> RelatedMenu = UnityOptional<GameObject>.Null();

    private OrderedSet<GameOption> options = new();
    private readonly List<Action<IGameOptionTab>> callbacks = new();
    
    public void Activate()
    {
        VentLogger.Info($"Activated Vanilla Tab: \"{GetType().Name}\"", "TabSwitch");
        Highlight().IfPresent(highlight => highlight.enabled = true);
        RelatedMenu.Handle(menu => menu.SetActive(true), () => VentLogger.Warn($"Error Activating Menu for {GetType().Name}"));
    }

    public void Deactivate()
    {
        VentLogger.Debug($"Deactivated Vanilla Tab: \"{GetType().Name}\"", "TabSwitch");
        Highlight().IfPresent(highlight => highlight.enabled = false);
        RelatedMenu.IfPresent(menu => menu.SetActive(false));
    }

    public void AddEventListener(Action<IGameOptionTab> callback) => callbacks.Add(callback);

    public void AddOption(GameOption option)
    {
        if (options.Contains(option)) return;
        options.Add(option);
    }

    public void RemoveOption(GameOption option) => options.Remove(option);

    public void HandleClick()
    {
        callbacks.ForEach(cb => cb(this));
    }

    public abstract StringOption InitializeOption(StringOption sourceBehavior);

    public abstract void Setup(MenuInitialized initialized);

    public void SetPosition(Vector2 position)
    {
        TabButton.IfPresent(btn => btn.transform.localPosition = position);
    }
    
    public void Show()
    {
        TabButton.IfPresent(button => button.SetActive(true));
    }

    public void Hide()
    {
        TabButton.IfPresent(button => button.SetActive(false));
    }

    public abstract List<GameOption> PreRender();

    public Optional<Vector3> GetPosition() => TabButton.Map(btn => btn.transform.localPosition);

    public List<GameOption> GetOptions() => options.AsList();

    protected abstract UnityOptional<SpriteRenderer> Highlight();
}