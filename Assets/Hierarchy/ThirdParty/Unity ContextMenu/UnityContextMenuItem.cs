using System;
using ExtraLinq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UnityContextMenuItem : MonoBehaviour
{
    [SerializeField] private Button button;
    [Space] [SerializeField] private Image bg;
    [SerializeField] private Image icon;
    [Space] [SerializeField] private Image expandIcon;
    [Space] [SerializeField] private TMP_Text leftText;
    [SerializeField] private TMP_Text rightText;

    private void Awake()
    {
        button.OnClickAsObservable().Subscribe(_ => { OnClick?.Invoke(); });
    }

    public event Action OnClick;

    public void SetIcon(Sprite sprite)
    {
        icon.sprite = sprite;
    }

    public void SetText(string title)
    {
        leftText.text = title;
        //	LocalizationHelper.BindTableAndEntry(leftText);
    }

    public void SetText(string title, string subTitle)
    {
        leftText.text = title;
        if (rightText) rightText.text = subTitle;
    }

    public void SetForegroundColor(Color color)
    {
        leftText.color = color;
        if (rightText) rightText.color = color;
    }

    public void SetBackgroundColor(Color color)
    {
        bg.color = color;
    }

    public void SetSubText(string subTitle)
    {
        if (rightText)
        {
            rightText.text = subTitle;
            rightText.gameObject.SetActive(!subTitle.IsNullOrEmpty());
        }
    }
}