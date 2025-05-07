using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HierarchyWindowView : MonoBehaviour
{
    [SerializeField] private HierarchyItemView _view;
    [SerializeField] private Button _button;
    private HierarchyItem _draggedItem;
    private HierarchyItem _hoveredItem;


    private HierarchyItem _selectedItem;

    private void Awake()
    {
        _button.OnBeginDragAsObservable().Subscribe(
            data => { OnItemBeginDrag?.Invoke(data); }
        );
        _button.OnDragAsObservable().Subscribe(data => { OnItemDrag?.Invoke(data); });
        _button.OnEndDragAsObservable().Subscribe(data => { OnItemEndDrag?.Invoke(data); });
    }

    public event Action<PointerEventData> OnItemBeginDrag;
    public event Action<PointerEventData> OnItemDrag;
    public event Action<PointerEventData> OnItemEndDrag;
}