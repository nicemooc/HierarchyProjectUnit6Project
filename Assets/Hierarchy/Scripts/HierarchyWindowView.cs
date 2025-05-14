using System;
using System.Collections.Generic;
using ExtraLinq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HierarchyWindowView : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Transform _itemContainer;
    [SerializeField] private HierarchyItem _itemPrefab;
    [SerializeField] private TMP_InputField _searchInputField;
    [SerializeField] private Button _clearSeachButton;

    public List<HierarchyItem> Items = new();

    public Transform ItemContainer => _itemContainer;
    public string SearchText => _searchInputField.text;


    private void Awake()
    {
        foreach (var item in Items) RegisterItemEvents(item);
        _searchInputField.onValueChanged.AsObservable().Subscribe(OnSearchInputChanged);
        _clearSeachButton.onClick.AddListener(ClearSearchBar);
        _clearSeachButton.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClicked?.Invoke(eventData);
    }


    public event Action<PointerEventData, HierarchyItem> OnItemClicked;
    public event Action<PointerEventData, HierarchyItem> OnItemClickDown;
    public event Action<PointerEventData, HierarchyItem> OnItemClickUp;
    public event Action<PointerEventData, HierarchyItem> OnItemEnter;
    public event Action<PointerEventData, HierarchyItem> OnItemExit;
    public event Action<PointerEventData, HierarchyItem> OnItemDrag;
    public event Action<PointerEventData, HierarchyItem> OnItemBeginDrag;
    public event Action<PointerEventData, HierarchyItem> OnItemDrop;
    public event Action<PointerEventData, HierarchyItem> OnItemEndDrag;
    public event Action<HierarchyItem, bool> OnItemFoldoutClick;
    public event Action<PointerEventData> OnClickDown;
    public event Action<PointerEventData> OnClicked;
    public event Action<PointerEventData> OnDrop;
    public event Action<string> OnSearchInputChanged;

    public void RegisterItemEvents(HierarchyItem item)
    {
        item.OnClicked += (data, hierarchyItem) => OnItemClicked?.Invoke(data, hierarchyItem);

        item.OnFoldoutClick += (item, isCollapsed) => OnItemFoldoutClick?.Invoke(item, isCollapsed);

        item.OnEnter += (data, hierarchyItem) => OnItemEnter?.Invoke(data, hierarchyItem);
        item.OnExit += (data, hierarchyItem) => OnItemExit?.Invoke(data, hierarchyItem);

        item.OnDrag += (data, hierarchyItem) => OnItemDrag?.Invoke(data, hierarchyItem);
        item.OnBeginDrag += (data, hierarchyItem) => OnItemBeginDrag?.Invoke(data, hierarchyItem);
        item.OnEndDrag += (data, hierarchyItem) => OnItemEndDrag?.Invoke(data, hierarchyItem);

        item.OnDrop += (data, hierarchyItem) => OnItemDrop?.Invoke(data, hierarchyItem);
    }

    public void ClearSearchBar()
    {
        _searchInputField.text = "";
    }

    public void SetClearSeachButtonDisplay(bool enabled)
    {
        _clearSeachButton.gameObject.SetActive(enabled);
    }

    public void SearchForHierarchyItem(string itemName)
    {
        // var isOnlySpaces = itemName.All(c => c == ' ');
        // if (isOnlySpaces)
        // {
        //     foreach (var item in Items) item.SetDisplay(false);
        //     Debug.Log("aaaaaaaaaa" + isOnlySpaces);
        //     return;
        // }

        //  if (itemName == null)
        //  {
        //      item.View.SetDepth();
        //      item.View.ShowFoldout(item.HasChildren);
        //      SetActiveBaseOnParent(item);
        //  }

        foreach (var item in Items)
            if (!itemName.IsNullOrEmpty())
            {
                item.View.SetDepth(false);
                item.View.ShowFoldout(false);

                var string1 = item.name.Replace(" ", "").ToLower();
                var string2 = itemName.Replace(" ", "").ToLower();
                item.SetDisplay(string1.Contains(string2));
                SetClearSeachButtonDisplay(true);
            }
            else
            {
                item.View.SetDepth();
                item.View.ShowFoldout(item.HasChildren);
                SetActiveBaseOnParent(item);
                SetClearSeachButtonDisplay(false);
            }
    }


    public void SetActiveBaseOnParent(HierarchyItem item)
    {
        if (!item.Parent)
        {
            item.SetDisplay(true);
        }
        else
        {
            if (!item.Parent.View.gameObject.activeInHierarchy || item.Parent.IsCollapsed)
                item.SetDisplay(false);
            else
                item.SetDisplay(true);
        }
    }
}