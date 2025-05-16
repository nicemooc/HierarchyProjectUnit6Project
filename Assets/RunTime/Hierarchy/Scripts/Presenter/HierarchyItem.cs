using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HierarchyItem : MonoBehaviour
{
    [SerializeField] private HierarchyItemView _view;

    [SerializeField] private HierarchyItem _parent;

    [SerializeField] private List<HierarchyItem> _children = new();
    public bool IsRenaming;
    public GameObjectReference ObjectRef;
    private float _gameObjectHeight, _topPos, _bottomPos;
    private ItemPosition _pos;


    public bool IsSelected { get; private set; }
    public bool IsCollapsed { get; private set; } = true;

    public HierarchyItemView View => _view;

    public HierarchyItem Parent
    {
        get => _parent;

        private set => _parent = value;
    }

    public List<HierarchyItem> Children
    {
        get => _children;
        set => _children = value;
    }

    public bool HasChildren => Children.Count > 0;

    public string ObjectName => View.ObjectName;


    private void Awake()
    {
        View.OnFoldoutClick += () => OnFoldoutClick?.Invoke(this, IsCollapsed);
        View.OnItemEnter += data => OnEnter(data, this);
        View.OnItemClicked += data => OnClicked(data, this);
        View.OnItemClickDown += data => OnClickDown(data, this);
        View.OnItemBeginDrag += data => OnBeginDrag(data, this);
        View.OnItemDrop += data => OnDrop(data, this);
        View.OnItemDrag += data => OnDrag(data, this);
        View.OnItemExit += data => OnExit(data, this);
        View.OnItemEndDrag += data => OnEndDrag(data, this);
        View.OnRenameFieldDeselect += CloseRenameFieldAndSaveNewName;
    }

    private void Start()
    {
        View.ShowFoldout(HasChildren);
    }


    public event Action<PointerEventData, HierarchyItem> OnEnter;
    public event Action<PointerEventData, HierarchyItem> OnExit;
    public event Action<PointerEventData, HierarchyItem> OnBeginDrag;
    public event Action<PointerEventData, HierarchyItem> OnDrag;
    public event Action<PointerEventData, HierarchyItem> OnEndDrag;
    public event Action<PointerEventData, HierarchyItem> OnDrop;
    public event Action<PointerEventData, HierarchyItem> OnClicked;
    public event Action<PointerEventData, HierarchyItem> OnClickDown;
    public event Action<PointerEventData, HierarchyItem> OnClickUp;
    public event Action<HierarchyItem, bool> OnActiveObjectChanged;
    public event Action<HierarchyItem> RemoveInactiveItemOnStart;
    public event Action OnDestroySelf;
    public event Action<string> OnRename;
    public event Action<HierarchyItem, bool> OnFoldoutClick;
    public event Action<HierarchyItem> OnParentChanged;

    public void DefineItemPos()
    {
        var itemRect = View.GetComponent<RectTransform>();
        var itemPos = itemRect.position;
        _gameObjectHeight = itemRect.rect.height;
        _topPos = Camera.allCameras[0].WorldToScreenPoint(itemPos).y + _gameObjectHeight / 6;
        _bottomPos = Camera.allCameras[0].WorldToScreenPoint(itemPos).y - _gameObjectHeight / 6;
        //     Debug.Log(_topPos + "+++++++" + _bottomPos);
    }

    public void SetParent(HierarchyItem parent)
    {
        if (Children.Contains(parent)) return;


        if (Parent != parent)
        {
            if (Parent) Parent.RemoveChild(this);
            Parent = parent;
            View.DepthLever = parent.View.DepthLever + 1;
            gameObject.transform.SetParent(Parent.transform);
            // parent.AddChild(this, !IsCollapsed);
        }

        RefreshView();
    }

    public void AddChild(HierarchyItem childItem, bool isExpand = true)
    {
        if (childItem == null) return;

        childItem.SetParent(this);
        if (!Children.Contains(childItem)) Children.Add(childItem);
        childItem.transform.SetParent(transform);

        if (isExpand) Expand();
    }

    public void RemoveChild(HierarchyItem child)
    {
        if (!Children.Contains(child)) return;
        Children.Remove(child);
        RefreshView();
    }

    public void Select()
    {
        if (IsSelected) Debug.Log("+++" + ObjectName);
        //   OpenRenameField(ObjectName);
        IsSelected = true;
        View.Select();
    }

    public void Deselect()
    {
        IsSelected = false;
        View.Deselect();
    }

    public void SetDisplay(bool isActive)
    {
        View.SetDisplay(isActive);
    }

    public void Expand(bool isExpandAll = false)
    {
        // Log.Info("Expand, isExpandAll: " + isExpandAll, this);

        if (!HasChildren) return;

        IsCollapsed = false;

        if (isExpandAll)
            this.TraverseAllChild(child =>
            {
                if (child != null)
                {
                    child.SetDisplay(true);
                    child.Expand();
                }
            });
        else
            this.TraverseAllChild(child =>
            {
                if (child != null)
                    if (child.Parent != null)
                        if (!child.IsAnyParentCollapsed())
                            child.SetDisplay(true);
            });

        RefreshView();
    }


    public void Collapse(bool isCollapsedAll = false)
    {
        if (!HasChildren) return;

        IsCollapsed = true;
        if (isCollapsedAll)
            this.TraverseAllChild(child =>
            {
                if (child != null)
                {
                    child.SetDisplay(false);
                    child.Collapse();
                }
            });
        else
            this.TraverseAllChild(child =>
            {
                if (child != null) child.SetDisplay(false);
            });

        RefreshView();
    }

    public void OnClickedFoldout(bool isCollapsed)
    {
        if (isCollapsed)
            Expand();
        else
            Collapse();
    }

    public void HandleDropToItem(HierarchyItem dragItem)
    {
        if (dragItem.Children.Exists(e => e.Equals(this))) return;
        if (dragItem == this) return;

        switch (_pos)
        {
            case ItemPosition.TOP_POS:
                OnDropTopArea(dragItem);
                break;
            case ItemPosition.MIDDLE_POS:
                OnDropMiddleArea(dragItem);
                break;
            case ItemPosition.BOTTOM_POS:
                OnDropBottomArea(dragItem);
                break;
        }

        dragItem.SetActiveView(dragItem.ObjectRef.GameObjectRef.activeInHierarchy);
        RefreshView();
    }

    private void OnDropTopArea(HierarchyItem dragItem)
    {
        if (dragItem == this) return;

        var isOtherParent = dragItem.Parent != Parent;
        if (isOtherParent) dragItem.ChangeToNewParent(Parent);

        var isDragItemAbove = dragItem.transform.GetSiblingIndex() < transform.GetSiblingIndex();
        if (isDragItemAbove) dragItem.SetSiblingIndex(transform.GetSiblingIndex() - 1);
        dragItem.SetSiblingIndex(transform.GetSiblingIndex());
    }

    private void OnDropMiddleArea(HierarchyItem dragItem)
    {
        if (dragItem == this) return;

        dragItem.ChangeToNewParent(this);
    }

    private void OnDropBottomArea(HierarchyItem dragItem)
    {
        if (dragItem == this) return;

        var isOtherParent = dragItem.Parent != Parent;
        if (isOtherParent) dragItem.ChangeToNewParent(Parent);

        var isDragItemAbove = dragItem.transform.GetSiblingIndex() < transform.GetSiblingIndex();

        if (isDragItemAbove)
            dragItem.SetSiblingIndex(transform.GetSiblingIndex());
        else
            dragItem.SetSiblingIndex(transform.GetSiblingIndex() + 1);
    }

    public void SetActiveView(bool isActive)
    {
        View.SetActiveViewItem(isActive);
    }

    public void SetSiblingIndex(int index)
    {
        var addSiblingIndex = index - transform.GetSiblingIndex();
        transform.SetSiblingIndex(index);
        ObjectRef.GameObjectRef.transform.SetSiblingIndex(ObjectRef.GameObjectRef.transform.GetSiblingIndex() +
                                                          addSiblingIndex);

        var siblingIndex = Parent == null ? index : ObjectRef.GameObjectRef.transform.GetSiblingIndex();
    }

    public void SetAsFirstSibling()
    {
        if (Parent == null)
        {
            transform.SetSiblingIndex(0);
            ObjectRef.GameObjectRef.transform.SetAsFirstSibling();
        }
        else
        {
            transform.SetSiblingIndex(1);
            ObjectRef.GameObjectRef.transform.SetAsFirstSibling();
        }
    }

    public void SetAsLastSibling()
    {
        transform.SetAsLastSibling();
        ObjectRef.GameObjectRef.transform.SetAsLastSibling();
    }

    public void ChangeToNewParent(HierarchyItem newParent)
    {
        if (newParent == Parent)
        {
            SetAsLastSibling();

            return;
        }

        Parent?.RemoveChild(this);

        if (newParent == null)
        {
            ObjectRef.GameObjectRef.transform.SetParent(null);
            gameObject.transform.SetParent(GetRootTransform());
            Parent = null;

            //      GameObject root = enabled;
            SetAsLastSibling();
            View.DepthLever = 0;
        }
        else
        {
            ObjectRef.GameObjectRef.transform.SetParent(newParent.ObjectRef.GameObjectRef.transform);
            newParent.AddChild(this);
            SetAsLastSibling();
            RefreshChildDepth(this);
        }


        RefreshView();

        OnParentChanged?.Invoke(newParent);
    }


    private void RefreshView()
    {
        View.SetFoldoutSprite(!IsCollapsed);
        View.ShowFoldout(HasChildren);
        if (Parent) View.DepthLever = Parent.View.DepthLever + 1;
        RefreshChildDepth(this);
    }

    private void RefreshChildDepth(HierarchyItem item)
    {
        if (!HasChildren) return;
        foreach (var child in item.Children)
        {
            var childView = child.View;
            childView.DepthLever = child.Parent.View.DepthLever + 1;
            RefreshChildDepth(child);
        }
    }

    public static bool IsAnyParentCollapsed(HierarchyItem item)
    {
        if (item.Parent == null) return false;
        if (item.Parent.IsCollapsed) return true;

        if (IsAnyParentCollapsed(item.Parent)) return true;

        return false;
    }

    public void OpenRenameField(string name)
    {
        IsRenaming = true;
        View.OpenRenameField(name);
    }

    public void CloseRenameField()
    {
        View.CloseRenameField();
        IsRenaming = false;
    }

    public void CloseRenameFieldAndSaveNewName(string name)
    {
        OnRename?.Invoke(name);
        // string newName;
        if (CheckVaildName(name)) SetTitle(name);

        // 
        CloseRenameField();
    }

    public bool CheckVaildName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        return true;
    }

    public void CloseRenameFieldAndSaveNewName()
    {
        CloseRenameFieldAndSaveNewName(View.RenameText);
    }

    public void SetTitle(string title)
    {
        gameObject.name = title;
        View.SetTitle(title);
        // if (Data.IsMissingRef) View.SetMissing(title);
    }

    public Transform GetRootTransform()
    {
        var currentTransform = this;

        while (currentTransform.Parent != null) currentTransform = currentTransform.Parent;
        //  Debug.Log("+++++" + currentTransform.gameObject.name);
        return currentTransform.transform.parent;
    }

    public void OnDetectGuideline(HierarchyItem hoveredItem, PointerEventData pointerEventData)
    {
        if (hoveredItem == null) return;
        if (hoveredItem.Equals(this)) return;

        var isHitTop = pointerEventData.position.y > hoveredItem._topPos;

        if (isHitTop)
        {
            hoveredItem._pos = ItemPosition.TOP_POS;
            //   Debug.Log("ItemPosition.TOP_POS;");
            hoveredItem.View.SetTopGuideline(true);
            hoveredItem.View.SetBottomGuideline(false);
        }
        else
        {
            var isHitBottom = pointerEventData.position.y < hoveredItem._bottomPos;
            if (isHitBottom)
            {
                hoveredItem._pos = ItemPosition.BOTTOM_POS;
                //   Debug.Log("ItemPosition.BOTTOM_POS;");
                hoveredItem.View.SetTopGuideline(false);
                hoveredItem.View.SetBottomGuideline(true);
            }
            else
            {
                var isHitMiddle = pointerEventData.position.y >= hoveredItem._bottomPos &&
                                  pointerEventData.position.y <= hoveredItem._topPos;
                if (isHitMiddle)
                {
                    hoveredItem._pos = ItemPosition.MIDDLE_POS;
                    //  Debug.Log("ItemPosition.MIDDLE_POS;");
                    hoveredItem.View.SetActiveGuideline(false);
                }
            }
        }
    }
}

public enum ItemPosition
{
    TOP_POS,
    MIDDLE_POS,
    BOTTOM_POS
}