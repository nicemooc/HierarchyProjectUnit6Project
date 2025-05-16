using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;

public static class HierarchyItemExtension
{
    public static int GetTotalChildNumber(this HierarchyItem item, int childCount = 0)
    {
        childCount += item.Children.Count;
        foreach (var child in item.Children) child.GetTotalChildNumber(childCount);

        return childCount;
    }

//    public static bool IsAllParentActive(this HierarchyItem item)
//    {
//        if (item.Parent == null) return true;
//        if (!item.Parent.RteObjectRef.IsActiveInView)
//        {
//            Log.Info($"{item.name} Inactive");
//            return false;
//        }
//
//        if (!IsAllParentActive(item.Parent))
//        {
//            Log.Info($"{item.name} Inactive");
//            return false;
//        }
//
//        return true;
//    }

    public static bool IsAnyParentCollapsed(this HierarchyItem item)
    {
        if (item.Parent == null) return false;
        if (item.Parent.IsCollapsed) return true;

        if (IsAnyParentCollapsed(item.Parent)) return true;

        return false;
    }

    public static void TraverseAllChild(this HierarchyItem item, Action<HierarchyItem> action)
    {
        foreach (var child in item.Children)
        {
            var childItems = new List<HierarchyItem>();

            TraverseAllChildIncludeParentInternal(child, childItems);

            foreach (var rteHierarchyItem in childItems) action?.Invoke(rteHierarchyItem);
        }
    }

    public static void TraverseAllChildIncludeParent(this HierarchyItem item, Action<HierarchyItem> action)
    {
        action?.Invoke(item);

        if (item.Children.Count == 0) return;

        foreach (var child in item.Children)
        {
            var items = new List<HierarchyItem>();

            TraverseAllChildIncludeParentInternal(child, items);

            foreach (var rteHierarchyItem in items) action?.Invoke(rteHierarchyItem);
        }
    }

    private static void TraverseAllChildIncludeParentInternal(HierarchyItem item, List<HierarchyItem> items)
    {
        items.Add(item);

        if (item.Children.Count == 0) return;

        foreach (var child in item.Children) TraverseAllChildIncludeParentInternal(child, items);
    }

    public static bool IsNotFocusInputField()
    {
        if (EventSystem.current.currentSelectedGameObject == null) return true;

        var tmpInputField = EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>();
        if (tmpInputField == null) return true;
        if (tmpInputField.GetType() == typeof(TMP_InputField)) return false;

        return true;
    }
}