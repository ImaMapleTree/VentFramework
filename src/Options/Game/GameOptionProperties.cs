using UnityEngine;
using VentLib.Utilities.Optionals;

namespace VentLib.Options.Game;

public class GameOptionProperties
{
    public UnityOptional<StringOption> UnityOptional;
    public StringOption Behaviour;
    public GameOption Source;
    public Transform Transform;
    public SpriteRenderer SpriteRenderer;
    public Transform Text;
    public Transform Value;
    public Transform PlusButton;
    public Transform MinusButton;

    internal GameOptionProperties(GameOption option)
    {
        Source = option;
        UnityOptional = Source.Behaviour;
        Behaviour = Source.Behaviour.Get();
        Transform = Behaviour.transform;
        SpriteRenderer = Transform.Find("Background").GetComponent<SpriteRenderer>();
        Text = Transform.FindChild("Title_TMP");
        Value = Transform.FindChild("Value_TMP");
        PlusButton = Transform.FindChild("Plus_TMP");
        MinusButton = Transform.FindChild("Minus_TMP");
        option.Properties = CustomOptional<GameOptionProperties>.Of(this, item => item.Exists());
    }

    public void SetActive(bool active)
    {
        Behaviour.gameObject.SetActive(active);
    }

    public void SetPosition(Vector3 position)
    {
        Transform.localPosition = position;
    }

    public Vector3 GetPosition()
    {
        return Transform.localPosition;
    }

    public void SetColor(Color color)
    {
        SpriteRenderer.color = color;
    }

    public bool Exists() => UnityOptional.Exists();
}