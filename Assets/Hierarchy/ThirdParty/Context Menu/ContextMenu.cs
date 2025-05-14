using System;
using System.Collections.Generic;

public class ContextMenu
{
    private readonly List<ContextMenuItem> items = new();

    public ContextMenu(IContextMenuView view)
    {
        View = view;
    }

    public IContextMenuView View { get; set; }

    public ContextMenuItem AddItem(ContextMenuItem item)
    {
        items.Add(item);
        return item;
    }

    public ContextMenuItem AddItem(string title, Action onClick = null)
    {
        return AddItem(new ContextMenuItem(title, onClick));
    }

    public ContextMenuItem AddItem(string title, bool isEnabled = true, Action onClick = null)
    {
        return AddItem(new ContextMenuItem(title, isEnabled, onClick));
    }

    public void Show()
    {
        View.Show();
    }

    public void Hide()
    {
        View.Hide();
    }

    public void Clear()
    {
        items.Clear();
    }
}