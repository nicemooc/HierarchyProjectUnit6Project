using System;
using System.Collections;
using System.Collections.Generic;
using ExtraLinq;
using NaughtyAttributes;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class HierarchyWindow : MonoBehaviour
{
    [SerializeField] private HierarchyWindowView _view;
    [SerializeField] private HierarchyContextMenu _hierarchyContextMenu;

    [SerializeField] [ReadOnly] private HierarchyItem _draggedItem;
    [SerializeField] [ReadOnly] private HierarchyItem _hoveredItem;
    [SerializeField] [ReadOnly] private HierarchyItem _selectedItem;
    public List<HierarchyItem> ListItemsSortByIndex = new();
    private readonly TimeSpan _searchUpdateTimeInSec = TimeSpan.FromSeconds(0.35);
    private readonly float _timeToOpen = 1f;
    private float _hoverTime;

    private IDisposable _searchDisposable;

    private Coroutine hoverCoroutine;
    public List<HierarchyItem> Items => _view.Items;
    public HierarchyWindowView View => _view;

    public bool IsSearching => !View.SearchText.IsNullOrEmpty();

    public HierarchyItem HoveredItem
    {
        get => _hoveredItem;
        private set => _hoveredItem = value;
    }

    public HierarchyItem SelectedItem
    {
        get => _selectedItem;
        private set => _selectedItem = value;
    }

    public HierarchyItem DraggedItem
    {
        get => _draggedItem;
        private set => _draggedItem = value;
    }


    private void Awake()
    {
        View.OnItemFoldoutClick += (item, isCollapsed) => item.OnClickedFoldout(isCollapsed);

        View.OnItemClicked += HandleOnClickDown;
        View.OnItemEnter += HandleOnItemEnter;
        View.OnItemBeginDrag += HandleItemBeginDrag;
        View.OnItemDrop += HandleItemDrop;
        View.OnClicked += OnWindowClick;
        View.OnItemDrag += HandleItemDrag;
        View.OnItemExit += HandleItemExit;
        View.OnSearchInputChanged += OnSearchInputChanged;
        View.OnItemEndDrag += HandleItemEndDrag;
    }

    public void UpdateListItemsSortByIndex()
    {
        if (ListItemsSortByIndex.Count > 0) ListItemsSortByIndex.Clear();
        foreach (var item in View.ItemContainer.GetComponentsInChildren<HierarchyItem>())
            if (!ListItemsSortByIndex.Contains(item) && item.View.gameObject.activeInHierarchy)
                ListItemsSortByIndex.Add(item);
    }

    private void OnSearchInputChanged(string searchChanged)
    {
        if (_searchDisposable != null)
        {
            _searchDisposable.Dispose();
            _searchDisposable = null;
        }

        _searchDisposable = Observable.Timer(_searchUpdateTimeInSec)
            .Subscribe(_ => { View.SearchForHierarchyItem(View.SearchText); });
    }


    private void HandleOnClickDown(PointerEventData eventData, HierarchyItem item)
    {
        //  if (HoveredItem != null) HoveredItem.View.SetActiveGuideline(false);
        //  HandleSelectItem(item);
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            HandleSelectItem(item);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            //  if (!RteSelection.Contains(item))
            //  {
            HandleSelectItem(item, false);
            // }

            _hierarchyContextMenu.OpenContextMenu(Input.mousePosition);
        }
    }

    public void HandleSelectItem(HierarchyItem item, bool isLeftClick = true)
    {
        if (item)
        {
            if (SelectedItem & (SelectedItem != item)) SelectedItem.Deselect();
            SelectedItem = item;
            SelectedItem.Select();
        }
        else
        {
            if (SelectedItem) SelectedItem.Deselect();
            SelectedItem = null;
        }
    }


    public void HandleOnItemEnter(PointerEventData eventData, HierarchyItem item)
    {
        if (HoveredItem != null)
            HoveredItem.View.SetActiveGuideline(false);

        HoveredItem = item;
        HoveredItem.DefineItemPos();

        if (IsSearching) return;
        if ((hoverCoroutine != null) & (DraggedItem == null))
        {
            StopCoroutine(hoverCoroutine);
            Debug.Log("+++++" + hoverCoroutine);
        }

        if (DraggedItem != null)
            hoverCoroutine = StartCoroutine(HoverToExpand(item));
    }

    private IEnumerator HoverToExpand(HierarchyItem item)
    {
        var timer = 0f;
        while (timer < _timeToOpen)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        item.Expand();
    }


    public void HandleItemBeginDrag(PointerEventData eventData, HierarchyItem item)
    {
        DraggedItem = item;
        HandleSelectItem(item);
    }

    public void HandleItemEndDrag(PointerEventData eventData, HierarchyItem item)
    {
        DraggedItem = null;
        HandleSelectItem(null);
    }

    public void HandleItemDrag(PointerEventData eventData, HierarchyItem item)
    {
        if (IsSearching) return;
        if (DraggedItem == null) return;
        //     if (eventData.button == PointerEventData.InputButton.Right) return;
        item.OnDetectGuideline(HoveredItem, eventData);
    }

    public void HandleItemDrop(PointerEventData eventData, HierarchyItem item)
    {
        if (IsSearching) return;
        item.HandleDropToItem(DraggedItem);
    }

    public bool CanRenameItem()
    {
        return SelectedItem != null;
    }

    public void HandleItemExit(PointerEventData eventData, HierarchyItem item)
    {
        if (HoveredItem == null) return;
        HoveredItem.View.SetActiveGuideline(false);
        _hoverTime = 0f;
        if (hoverCoroutine != null) StopCoroutine(hoverCoroutine);

        HoveredItem = null;
    }

    public void OnWindowClick(PointerEventData eventData)
    {
        HandleSelectItem(null);
        if (eventData.button == PointerEventData.InputButton.Right)
            _hierarchyContextMenu.OpenContextMenu(Input.mousePosition);
    }

    public event Action<PointerEventData> OnItemBeginDrag;
    public event Action<PointerEventData> OnItemDrag;
    public event Action<PointerEventData> OnItemEndDrag;
}