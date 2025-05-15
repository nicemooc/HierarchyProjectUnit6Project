using UnityEngine;

public class HierarchyNavigation : MonoBehaviour
{
    // KeyboardShortcut _rteKeyboardShortcut;
    private HierarchyWindowPresenter _hierarchyWindowierarchy;

    private HierarchyItem SelectedItem => _hierarchyWindowierarchy.SelectedItem;

    private void Awake()
    {
        //	_rteKeyboardShortcut = FindObjectOfType<RteKeyboardShortcut>(true);
        //	_hierarchyWindowierarchy = FindObjectOfType<RteHierarchyWindow>(true);
    }

    public void MoveToNextItem()
    {
        HierarchyItem nextItem = null;

        if (SelectedItem == null) return;

        //    var selectedItemIndex =
        //         _hierarchyWindowierarchy.ListItemsSortByIndex.FindIndex(item => item.Equals(SelectedItem));

        //    nextItem = _hierarchyWindowierarchy.ListItemsSortByIndex.SkipWhile((item, index) => index <= selectedItemIndex)
        //        .FirstOrDefault();

        if (nextItem != null && nextItem.gameObject.activeInHierarchy)
            _hierarchyWindowierarchy.HandleSelectItem(nextItem);
    }

    public void MoveToPreviousItem()
    {
        HierarchyItem prevItem = null;

        if (SelectedItem == null) return;

        //    var selectedItemIndex =
        //        _hierarchyWindowierarchy.ListItemsSortByIndex.FindIndex(item => item.Equals(SelectedItem));

        //     prevItem = _hierarchyWindowierarchy.ListItemsSortByIndex.TakeWhile((item, index) => index < selectedItemIndex)
        //        .LastOrDefault();

        if (prevItem != null && prevItem.gameObject.activeInHierarchy)
            _hierarchyWindowierarchy.HandleSelectItem(prevItem);
    }

    public void MoveToNextParentItem()
    {
        if (SelectedItem == null) return;

        //   var selectedItemIndex =
        //       _hierarchyWindowierarchy.ListItemsSortByIndex.FindIndex(item => item.Equals(SelectedItem));

        //   var nextItem = _hierarchyWindowierarchy.ListItemsSortByIndex
        //       .SkipWhile((item, index) => index <= selectedItemIndex)
        //       .Where(item => item.View.FoldoutImage.gameObject.activeInHierarchy).FirstOrDefault();

//if (nextItem != null) _hierarchyWindowierarchy.HandleSelectItem(nextItem);
    }

    public void MoveToPreviousParentItem()
    {
        if (SelectedItem == null) return;

        var prevItem = SelectedItem.Parent;
        if (prevItem != null) _hierarchyWindowierarchy.HandleSelectItem(prevItem);
    }

    public void MoveToNextItemWithFoldoutButton()
    {
        if (SelectedItem.IsCollapsed && SelectedItem.HasChildren)
        {
            SelectedItem.Expand();
            _hierarchyWindowierarchy.UpdateListItemsSortByIndex();
        }
        else
        {
            MoveToNextParentItem();
        }
    }

    public void MoveToPreviousItemWithFoldoutButton()
    {
        if (!SelectedItem.IsCollapsed && SelectedItem.HasChildren)
        {
            SelectedItem.Collapse();
            _hierarchyWindowierarchy.UpdateListItemsSortByIndex();
        }
        else
        {
            // MoveToPreviousItem();
            MoveToPreviousParentItem();
        }
    }
}