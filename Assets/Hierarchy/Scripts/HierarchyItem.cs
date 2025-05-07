using System.Collections.Generic;
using UnityEngine;

public class HierarchyItem : MonoBehaviour
{
    [SerializeField] private HierarchyItemView _view;

    [SerializeField] private HierarchyItem _parent;


    public List<HierarchyItem> Children { get; set; } = new();
}