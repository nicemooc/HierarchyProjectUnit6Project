using System.Collections.Generic;
using UnityEngine;

public class HierarchyWindow : MonoBehaviour
{
    [SerializeField] private Transform _itemContainer;
    [SerializeField] private HierarchyItem _itemPrefab;

    public List<HierarchyItem> Items = new();
    private HierarchyItem _draggedItem;
    private HierarchyItem _hoveredItem;


    private HierarchyItem _selectedItem;
}