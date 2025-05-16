using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemSelection : MonoBehaviour
{
    private static readonly List<HierarchyItem> _items = new();

    public static HierarchyItem[] Items
    {
        get => _items.ToArray();
        private set => Items = value;
    }

    public static int Count => Items.Length;
    public static bool HasMultiItems => Items.Length > 1;
    public static bool HasSelection => Items.Length > 0;
    public static HierarchyItem LastSelectedItem => Count > 0 ? Items.Last() : null;

    public static void AddItem(HierarchyItem item)
    {
        if (item == null) return;
        if (_items.Contains(item)) return;
        _items.Add(item);
    }

    public static void AddItems(HierarchyItem[] newItems, bool ignoreSelectedItems = true)
    {
        var resolvedItems = newItems;
        if (ignoreSelectedItems)
            resolvedItems = newItems.Where(newItem => !_items.Any(item => item == newItem)).ToArray();

        _items.AddRange(resolvedItems);
    }

    public static void Remove(HierarchyItem item)
    {
        _items.Remove(item);
    }

    public static void Clear()
    {
        _items.Clear();
    }

    public static bool Contains(HierarchyItem item)
    {
        return _items.Contains(item);
    }
}