using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExtraLinq;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class HierarchyWindowPresenter
{
    private readonly TimeSpan _searchUpdateTimeInSec = TimeSpan.FromSeconds(0.35);
    private readonly float _timeToOpen = 1f;
    private HierarchyContext _hierarchyContext;
    private HierarchyContextMenuPresenter _hierarchyContextMenu;
    private float _hoverTime;
    private IDisposable _searchDisposable;


    private Coroutine hoverCoroutine;
    public List<HierarchyItem> ListItemsSortByIndex = new();


    public bool IsSearching => !View.SearchText.IsNullOrEmpty();
    private List<HierarchyItem> Items => View.Items;


    private HierarchyWindowView View { get; set; }


    public HierarchyItem HoveredItem { get; private set; }

    public HierarchyItem SelectedItem { get; private set; }

    public HierarchyItem DraggedItem { get; private set; }


    private void RegisterViewEvents()
    {
        View.OnItemFoldoutClick += (item, isCollapsed) => item.OnClickedFoldout(isCollapsed);

        View.OnItemClicked += HandleOnItemClicked;
        View.OnItemClickDown += HandleOnItemClickDown;
        View.OnItemEnter += HandleOnItemEnter;
        View.OnItemBeginDrag += HandleItemBeginDrag;
        View.OnItemDrop += HandleItemDrop;
        View.OnClicked += OnWindowClick;
        View.OnItemDrag += HandleItemDrag;
        View.OnItemExit += HandleItemExit;
        View.OnSearchInputChanged += OnSearchInputChanged;
        View.OnItemEndDrag += HandleItemEndDrag;
    }

    public void Init(HierarchyContext hierarchyContext)
    {
        _hierarchyContext = hierarchyContext;
        _hierarchyContextMenu = _hierarchyContext.HierarchyContextMenuPresenter;
        View = _hierarchyContext.HierarchyWindowView;
        RegisterViewEvents();
    }

    public void UpdateListItemsSortByIndex()
    {
        if (ListItemsSortByIndex.Count > 0) ListItemsSortByIndex.Clear();
        foreach (var item in View.ItemContainer.GetComponentsInChildren<HierarchyItem>())
            if (!ListItemsSortByIndex.Contains(item) && item.View.gameObject.activeInHierarchy)
                ListItemsSortByIndex.Add(item);
    }

    private void ClearListItemsSortByIndex()
    {
        ListItemsSortByIndex.Clear();
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


    private void HandleOnItemClicked(PointerEventData eventData, HierarchyItem item)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            HandleSelectItem(item);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (!ItemSelection.Contains(item)) HandleSelectItem(item);


            _hierarchyContextMenu.OpenContextMenu(Input.mousePosition);
        }
    }

    private void HandleOnItemClickDown(PointerEventData eventData, HierarchyItem item)
    {
    }

    public void HandleSelectItem(HierarchyItem item)
    {
        var isMultiSelected = KeyboardShortcut.IsControlPressed || KeyboardShortcut.IsShiftPressed;

        if (item != null)
        {
            if (isMultiSelected)
            {
                if (KeyboardShortcut.IsControlPressed)
                {
                    if (item.IsSelected)
                    {
                        if (item) item.Deselect();
                        ItemSelection.Remove(item);

                        HierarchyItem lastItem = null;
                        if (ItemSelection.Count > 0) lastItem = ItemSelection.Items.LastOrDefault();
                        SelectedItem = lastItem;
                    }
                    else
                    {
                        if (item) ItemSelection.AddItem(item);
                        SelectedItem = item;
                    }

                    if (SelectedItem) SelectedItem.Select();

                    else
                        DeselectAllItems();
                }
                else if (KeyboardShortcut.IsShiftPressed)
                {
                    //TODO: Refactor, use when expand/collapse instead
                    UpdateListItemsSortByIndex();
                    // if (item)
                    // {
                    //TODO: Handle Shift
                    if (!SelectedItem)
                    {
                        SelectedItem = item;
                        if (SelectedItem) SelectedItem.Select();
                    }
                    else
                    {
                        HierarchyItem prevItem = null;
                        prevItem = SelectedItem;
                        SelectedItem = item;
                        var listSelection = new List<HierarchyItem>();
                        var prevItemIndex = ListItemsSortByIndex.FindIndex(item => item.Equals(prevItem));
                        var curItemIndex = ListItemsSortByIndex.FindIndex(item => item.Equals(SelectedItem));
                        var itemRange = Mathf.Abs(prevItemIndex - curItemIndex) + 1;
                        if (prevItemIndex >= curItemIndex)
                            listSelection = ListItemsSortByIndex.GetRange(curItemIndex, itemRange);
                        else
                            listSelection = ListItemsSortByIndex.GetRange(prevItemIndex, itemRange);


                        if (SelectedItem && !SelectedItem.IsSelected)
                        {
                            foreach (var selectItem in listSelection)
                            {
                                if (ItemSelection.Items.Contains(selectItem)) ItemSelection.Remove(selectItem);
                                selectItem.Select();
                            }

                            ItemSelection.AddItems(listSelection.ToArray());
                        }
                        else if (SelectedItem && SelectedItem.IsSelected)
                        {
                            listSelection.Remove(SelectedItem);
                            foreach (var deselectItem in listSelection)
                            {
                                ItemSelection.Remove(deselectItem);
                                deselectItem.Deselect();
                            }
                        }
                    }
                }
            }
            else
            {
                if (SelectedItem) SelectedItem.Deselect();


                DeselectAllItems(true);

                if (item) ItemSelection.AddItem(item);

                // Set Select item
                SelectedItem = item;
                if (SelectedItem) SelectedItem.Select();
            }

            //	View.SetFocus(true);
        }
        else
        {
            if (SelectedItem) SelectedItem.Deselect();

            SelectedItem = null;
        }


        if (SelectedItem == null)
        {
            DeselectAllItems();

            return;
        }

        UpdateListItemsSortByIndex();


        //if (SelectedItem.RteObjectRef != null)
        //{
        //	try
        //	{
        //		_rteOutlineController.ClearAllOutline();

        //		// Add Outline
        //		var selectionItems = ItemSelection.Items;
        //		foreach (var hierarchyItem in selectionItems)
        //		{
        //			if (hierarchyItem.RteObjectRef != null) ToggleHighlight(hierarchyItem, true);
        //		}
        //	}
        //	catch (Exception e)
        {
            //	Log.Error(e);
            //		}
        }
    }

    public void DeselectAllItems(bool isWithoutCurrentItem = false)
    {
        var selectionItems = ItemSelection.Items;
        foreach (var selectedItem in selectionItems)
            if (isWithoutCurrentItem)
            {
                if (selectedItem != SelectedItem) selectedItem?.Deselect();
            }
            else
            {
                selectedItem?.Deselect();
            }

        ItemSelection.Clear();
    }


    public void HandleOnItemEnter(PointerEventData eventData, HierarchyItem item)
    {
        if (HoveredItem != null)
            HoveredItem.View.SetActiveGuideline(false);

        HoveredItem = item;
        HoveredItem.DefineItemPos();

        //   if (IsSearching) return;
        //   if ((hoverCoroutine != null) & (DraggedItem == null))
        //   {
        //       StopCoroutine(hoverCoroutine);
        //       Debug.Log("+++++" + hoverCoroutine);
        //   }
        //   if (DraggedItem != null)
        //       hoverCoroutine = StartCoroutine(HoverToExpand(item));
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

    private void OnClickDownNewSelectItem(HierarchyItem dragItem)
    {
        if (ItemSelection.Items.Contains(dragItem) ||
            KeyboardShortcut.IsShiftPressed ||
            KeyboardShortcut.IsControlPressed)
            return;

        foreach (var item in ItemSelection.Items) item.Deselect();


        ItemSelection.Clear();
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
        return SelectedItem != null && !ItemSelection.HasMultiItems;
    }

    public void HandleItemExit(PointerEventData eventData, HierarchyItem item)
    {
        if (HoveredItem == null) return;
        HoveredItem.View.SetActiveGuideline(false);
        _hoverTime = 0f;
        //    if (hoverCoroutine != null) StopCoroutine(hoverCoroutine);

        //    HoveredItem = null;
    }

    public void OnWindowClick(PointerEventData eventData)
    {
        HandleSelectItem(null);
        if (eventData.button == PointerEventData.InputButton.Right)
            _hierarchyContextMenu.OpenContextMenu(Input.mousePosition);
    }

    public bool CanNavigationItem()
    {
        return SelectedItem is { IsRenaming: false } && SceneUtils.IsNotFocusInputField();
    }

    public event Action<PointerEventData> OnItemBeginDrag;
    public event Action<PointerEventData> OnItemDrag;
    public event Action<PointerEventData> OnItemEndDrag;
}