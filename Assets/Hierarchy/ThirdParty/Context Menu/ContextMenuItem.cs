using System;

[Serializable]
public class ContextMenuItem
{
    public string Title;
    public bool IsEnabled = true;

    public ContextMenuItem()
    {
    }

    public ContextMenuItem(string title, Action onClick)
    {
        Title = title;
        OnClick = onClick;
    }

    public ContextMenuItem(string title, bool isEnabled, Action onClick)
    {
        Title = title;
        IsEnabled = isEnabled;
        OnClick = onClick;
    }

    public event Action OnClick;
}