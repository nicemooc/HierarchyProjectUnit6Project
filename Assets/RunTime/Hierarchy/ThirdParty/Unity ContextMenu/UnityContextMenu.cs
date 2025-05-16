using System;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

public class UnityContextMenu : IUnityContextMenu, IDisposable
{
    private IDisposable _everyUpdateDisposable;
    private readonly Vector2 _position;
    public UnityContextMenuView View;

    public UnityContextMenu()
    {
        //TODO: refactor - optimize
        var view = Object.FindObjectOfType<UnityContextMenuView>(false);

        if (view != null) UpdateView(view);
        View?.Clear();
    }

    public UnityContextMenu(UnityContextMenuView view)
    {
        UpdateView(view);
        View?.Clear();
    }

    public UnityContextMenu(Vector2 position)
    {
        //TODO: refactor - optimize
        var view = Object.FindObjectOfType<UnityContextMenuView>(false);

        _position = position;
        if (view != null) UpdateView(view);
        View?.Clear();
    }

    public UnityContextMenu(UnityContextMenuView view, Vector2 position)
    {
        _position = position;
        if (view != null) UpdateView(view);
        View?.Clear();
    }

    public void Dispose()
    {
        _everyUpdateDisposable?.Dispose();
        _everyUpdateDisposable = null;
    }

    public void Show(Vector2 position)
    {
        View.Show(position, ContextMenuAnchor.UpperLeft);
    }

    public void AddItem(string title, Action onClick = null)
    {
        View.AddItem(title, null, true, onClick);
    }

    public void AddItem(string title, bool isEnable, Action onClick = null)
    {
        View.AddItem(title, null, isEnable, onClick);
    }

    public void AddItem(string title, string subTitle, bool isEnable, Action onClick = null)
    {
        View.AddItem(title, subTitle, isEnable, onClick);
    }

    public void AddSeparate()
    {
        View.AddSeparate();
    }

    public void Clear()
    {
        View.Clear();
    }

    public void Close()
    {
        if (!View.IsOpened) return;
        // Log.Info("Close");
        View.Close();
    }

    public Vector2 Size => View.ContextMenuSize();

    public void ShowAt(RectTransform rectTransform, ContextMenuAnchor rectAnchor = ContextMenuAnchor.UpperLeft)
    {
        View.ShowAt(rectTransform, rectAnchor);
    }

    private void UpdateView(UnityContextMenuView view)
    {
        View = view;
        _everyUpdateDisposable?.Dispose();
        _everyUpdateDisposable = Observable.EveryUpdate().Subscribe(l => OnUpdate()).AddTo(View);
    }

    public void Show()
    {
        Show(_position);
    }

    private void OnUpdate()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            if (!View.IsHover || !View.IsOpened)
                Close();
    }
}

public interface IUnityContextMenu : IContextMenu
{
    public Vector2 Size { get; }
    void ShowAt(RectTransform rectTransform, ContextMenuAnchor rectAnchor = ContextMenuAnchor.UpperLeft);
}