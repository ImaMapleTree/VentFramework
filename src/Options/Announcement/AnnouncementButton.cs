using System;
using UnityEngine;
using UnityEngine.UI;
using VentLib.Utilities.Extensions;
using VentLib.Utilities.Optionals;

namespace VentLib.Options.Announcement;

public class AnnouncementButton
{
    private UnityOptional<AnnouncementPanel> backing;
    private AnnouncementOption option;

    internal Vector3 Position => backing.Map(b => b.transform.localPosition).OrElseGet(() => new Vector3(0f, 0f, 0f));

    internal AnnouncementButton(AnnouncementPanel panel, AnnouncementOption option)
    {
        this.option = option;
        backing = UnityOptional<AnnouncementPanel>.Of(panel);
        var b = backing.Get();
        panel.DateText.text = option.Name();
        panel.TitleText.text = this.option.GetValueText();
        panel.NewIcon.enabled = false;

        b.transform.localScale -= new Vector3(0.3f, 0.25f);
        b.PassiveButton.OnClick = new Button.ButtonClickedEvent();
        b.PassiveButton.OnClick.AddListener((Action)Increment);
    }

    private void Increment()
    {
        option.Increment();
        backing.IfPresent(b => b.TitleText.text = option.GetValueText());
    }

    public void SetLocalPosition(Vector3 position)
    {
        backing.IfPresent(b => b.transform.localPosition = position);
    }

    public static AnnouncementButton operator +(AnnouncementButton button, Vector3 position)
    {
        button.backing.IfPresent(b => b.transform.localPosition += position);
        return button;
    }
    
    public static AnnouncementButton operator -(AnnouncementButton button, Vector3 position)
    {
        button.backing.IfPresent(b => b.transform.localPosition -= position);
        return button;
    }

    internal void Destroy()
    {
        backing.Get().gameObject.SetActive(false);
        backing.Get().DestroyImmediate();
    }
}