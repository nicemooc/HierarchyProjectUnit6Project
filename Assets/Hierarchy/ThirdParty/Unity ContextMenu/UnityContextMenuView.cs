using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//TODO: refactor: Working only button clicked event
//TODO: add: Icon + expand width item

public class UnityContextMenuView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Camera uiCamera;
    [SerializeField] private RectTransform contextMenu;
    [SerializeField] private UnityContextMenuItem itemPrefab;
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private Transform container;
    [SerializeField] private LayoutGroup layoutGroup;
    [Space] [SerializeField] [ReadOnly] private List<UnityContextMenuItem> items = new();
    [SerializeField] [ReadOnly] private List<GameObject> separates = new();
    public Vector2 ItemSize;

    public bool IsHover;
    private RectOffset _layoutSize;
    public bool IsOpened => contextMenu.gameObject.activeSelf;
    public float ItemCount => items.Count;

    private void Awake()
    {
        if (uiCamera == null) uiCamera = Camera.main;

        ItemSize = itemPrefab.GetComponent<RectTransform>().rect.size;
        _layoutSize = layoutGroup.padding;

        layoutGroup.gameObject.SetActive(false);
        container.gameObject.SetActive(false);
        itemPrefab.gameObject.SetActive(false);
        linePrefab.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        IsHover = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsHover = false;
    }

    public event Action OnOpened;
    public event Action OnClosed;

    private ContextMenuAnchor InverseXContextMenuAnchor(ContextMenuAnchor anchor)
    {
        switch (anchor)
        {
            case ContextMenuAnchor.UpperLeft:
                return ContextMenuAnchor.LowerLeft;
            case ContextMenuAnchor.UpperCenter:
                return ContextMenuAnchor.LowerCenter;
            case ContextMenuAnchor.UpperRight:
                return ContextMenuAnchor.LowerLeft;
            case ContextMenuAnchor.MiddleLeft:
                return ContextMenuAnchor.MiddleRight;
            case ContextMenuAnchor.MiddleCenter:
                return ContextMenuAnchor.MiddleCenter;
            case ContextMenuAnchor.MiddleRight:
                return ContextMenuAnchor.MiddleLeft;
            case ContextMenuAnchor.LowerLeft:
                return ContextMenuAnchor.UpperLeft;
            case ContextMenuAnchor.LowerCenter:
                return ContextMenuAnchor.UpperCenter;
            case ContextMenuAnchor.LowerRight:
                return ContextMenuAnchor.UpperRight;
            default:
                return anchor;
        }
    }

    private void InverseXAnchor(RectTransform rt, ContextMenuAnchor anchor)
    {
        contextMenu.position = WorldPoint(rt, InverseXContextMenuAnchor(anchor));
    }

    private void InverseYAnchor(RectTransform rt, ContextMenuAnchor anchor)
    {
        contextMenu.position = WorldPoint(rt, InverseXContextMenuAnchor(anchor));
    }

    private void InverseXPivot()
    {
        var pivot = contextMenu.pivot;
        contextMenu.pivot = new Vector2(1 - pivot.x, pivot.y);
    }

    private void InverseYPivot()
    {
        var pivot = contextMenu.pivot;
        contextMenu.pivot = new Vector2(pivot.x, 1 - pivot.y);
    }

    public void Show(Vector2 pos, ContextMenuAnchor anchor = ContextMenuAnchor.UpperLeft, bool isWorldPoint = true)
    {
        SetPivot(anchor);

        var newPoint = pos;

        if (isWorldPoint) newPoint = uiCamera.ScreenToWorldPoint(pos);

        Show(newPoint);

        if (!CheckCornersVisibility(out var cornersVisible))
        {
            if ((!cornersVisible[0] && !cornersVisible[1]) || (!cornersVisible[2] && !cornersVisible[3]))
                InverseXPivot();

            if ((!cornersVisible[1] && !cornersVisible[2]) || (!cornersVisible[0] && !cornersVisible[3]))
                InverseYPivot();
        }
    }

    public void Show(Vector2 pos)
    {
        if (IsOpened)
        {
            Close();
            return;
        }

        Open();

        contextMenu.position = pos;

        OnOpened?.Invoke();
    }

    private bool CheckCornersVisibility(out bool[] cornersVisible)
    {
        var corners = new Vector3[4];
        cornersVisible = new bool[4];
        contextMenu.GetWorldCorners(corners);

        var isVisible = true;

        for (var index = 0; index < corners.Length; index++)
        {
            var corner = corners[index];
            var screenPoint = uiCamera.WorldToScreenPoint(corner);

            if (!uiCamera.pixelRect.Contains(screenPoint))
            {
                // Log.Error($"Corner {index} is outside the camera view: " + corner);
                isVisible = false;
                cornersVisible[index] = false;
            }
            else
            {
                cornersVisible[index] = true;
            }
        }

        // if (isVisible)
        // {
        // 	// Debug.Log("UI is fully visible on the screen!");
        // }
        // else
        // {
        // 	// Debug.LogError("UI is partially or fully outside the camera view!");
        // }

        return isVisible;
    }

    private void SetPivot(ContextMenuAnchor anchor)
    {
        switch (anchor)
        {
            case ContextMenuAnchor.UpperLeft:
                contextMenu.pivot = new Vector2(0, 1);
                break;
            case ContextMenuAnchor.UpperCenter:
                contextMenu.pivot = new Vector2(0.5f, 1);
                break;
            case ContextMenuAnchor.UpperRight:
                contextMenu.pivot = new Vector2(1, 1);
                break;
            case ContextMenuAnchor.MiddleLeft:
                contextMenu.pivot = new Vector2(0, 0.5f);
                break;
            case ContextMenuAnchor.MiddleCenter:
                contextMenu.pivot = new Vector2(0.5f, 0.5f);
                break;
            case ContextMenuAnchor.MiddleRight:
                contextMenu.pivot = new Vector2(1, 0.5f);
                break;
            case ContextMenuAnchor.LowerLeft:
                contextMenu.pivot = new Vector2(0, 0);
                break;
            case ContextMenuAnchor.LowerCenter:
                contextMenu.pivot = new Vector2(0.5f, 0);
                break;
            case ContextMenuAnchor.LowerRight:
                contextMenu.pivot = new Vector2(1, 0);
                break;
        }
    }

    public void ShowAt(RectTransform rt, ContextMenuAnchor rectAnchor = ContextMenuAnchor.UpperLeft)
    {
        // World Corners
        // 1---2
        // |   |
        // 0---3
        var worldPoint = WorldPoint(rt, rectAnchor);

        Show(worldPoint);

        Canvas.ForceUpdateCanvases();

        if (!CheckCornersVisibility(out var cornersVisible))
        {
            // Invisible Left Or Right Side
            if ((!cornersVisible[0] && !cornersVisible[1]) || (!cornersVisible[2] && !cornersVisible[3]))
            {
                InverseXPivot();
                InverseXAnchor(rt, rectAnchor);
            }

            // Invisible Top Or Bottom Side
            if ((!cornersVisible[1] && !cornersVisible[2]) || (!cornersVisible[0] && !cornersVisible[3]))
            {
                InverseYPivot();
                InverseYAnchor(rt, rectAnchor);
            }
        }
    }

    // TODO: refactor
    private Vector3 WorldPoint(RectTransform rt, ContextMenuAnchor anchor)
    {
        var worldCorners = new Vector3[4];
        rt.GetWorldCorners(worldCorners);

        var worldPoint = worldCorners[0];

        switch (anchor)
        {
            case ContextMenuAnchor.UpperLeft:
                SetPivot(ContextMenuAnchor.LowerLeft);
                // contextMenu.pivot = new Vector2(0, 0);
                worldPoint = worldCorners[1];
                break;
            case ContextMenuAnchor.UpperCenter:
                SetPivot(ContextMenuAnchor.LowerCenter);
                // contextMenu.pivot = new Vector2(0.5f, 0);
                worldPoint = (worldCorners[1] + worldCorners[2]) / 2;
                break;
            case ContextMenuAnchor.UpperRight:
                SetPivot(ContextMenuAnchor.LowerRight);
                // contextMenu.pivot = new Vector2(1, 0);
                worldPoint = worldCorners[2];
                break;
            case ContextMenuAnchor.MiddleLeft:
                SetPivot(ContextMenuAnchor.MiddleLeft);
                contextMenu.pivot = new Vector2(0, 0.5f);
                worldPoint = (worldCorners[0] + worldCorners[1]) / 2;
                break;
            case ContextMenuAnchor.MiddleCenter:
                SetPivot(ContextMenuAnchor.MiddleCenter);
                // contextMenu.pivot = new Vector2(0.5f, 0.5f);
                worldPoint = (worldCorners[0] + worldCorners[2]) / 2;
                break;
            case ContextMenuAnchor.MiddleRight:
                SetPivot(ContextMenuAnchor.MiddleCenter);
                // contextMenu.pivot = new Vector2(1, 0.5f);
                worldPoint = (worldCorners[2] + worldCorners[3]) / 2;
                break;
            case ContextMenuAnchor.LowerLeft:
                SetPivot(ContextMenuAnchor.UpperLeft);
                // contextMenu.pivot = new Vector2(0, 1);
                worldPoint = worldCorners[0];
                break;
            case ContextMenuAnchor.LowerCenter:
                SetPivot(ContextMenuAnchor.UpperCenter);
                // contextMenu.pivot = new Vector2(0.5f, 1);
                worldPoint = (worldCorners[0] + worldCorners[3]) / 2;
                break;
            case ContextMenuAnchor.LowerRight:
                SetPivot(ContextMenuAnchor.UpperRight);
                // contextMenu.pivot = new Vector2(1, 1);
                worldPoint = worldCorners[3];
                break;
        }

        return worldPoint;
    }

    public void Open()
    {
        contextMenu.gameObject.SetActive(true);
    }

    public void Close()
    {
        contextMenu.gameObject.SetActive(false);

        // ContextMenuMouseHelper.Instance.OnMouseClick -= Close;

        OnClosed?.Invoke();
    }

    public void AddSeparate()
    {
        var separate = Instantiate(linePrefab, container);
        separate.gameObject.SetActive(true);

        separates.Add(separate);
    }

    public void AddItem(string title, Action action = null)
    {
        AddItem(title, true, action);
    }

    public void AddItem(string title, bool isEnable = true, Action onClick = null)
    {
        AddItem(title, null, isEnable, onClick);
    }

    public void AddItem(string title, string subTitle, bool isEnable, Action onClick)
    {
        var item = Instantiate(itemPrefab, container);
#if UNITY_EDITOR
        item.gameObject.name = title;
#endif
        // item.interactable = enable;

        item.SetText(title);
        item.SetSubText(subTitle);
        item.SetForegroundColor(isEnable ? Color.black : Color.gray);
        item.OnClick += () =>
        {
            if (isEnable)
            {
                onClick?.Invoke();
                Close();
            }
        };
        item.gameObject.SetActive(true);

        items.Add(item);
    }

    public void Clear()
    {
        for (var i = 0; i < items.Count; i++) Destroy(items[i].gameObject);

        for (var i = 0; i < separates.Count; i++) Destroy(separates[i].gameObject);

        items.Clear();
        separates.Clear();
    }

    public Vector2 ContextMenuSize()
    {
        return new Vector2(ItemSize.x + _layoutSize.left + _layoutSize.right,
            ItemSize.y * ItemCount + _layoutSize.top + _layoutSize.bottom);
    }
}