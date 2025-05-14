using UnityEngine;

public interface IUnityUIContextMenu : IContextMenu
{
    Vector2 Size { get; }
    void ShowAt(RectTransform rectTransform, ContextMenuAnchor rectAnchor = ContextMenuAnchor.UpperLeft);
    void Show(Vector2 pos, ContextMenuAnchor anchor = ContextMenuAnchor.UpperLeft, bool isWorldPoint = true);
}