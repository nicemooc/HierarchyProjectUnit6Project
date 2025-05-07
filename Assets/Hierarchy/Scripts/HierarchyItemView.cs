using System;
using NaughtyAttributes;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HierarchyItemView : MonoBehaviour
{
    public HierarchyItem HierarchyItem;
    [SerializeField] private Button _button;
    [SerializeField] private Button _foldoutButton;
    [SerializeField] private LayoutGroup _layoutGroup;
    [SerializeField] private GameObjectItemColorTheme _colorTheme;

    [SerializeField] [ReadOnly] private int _depthLever;
    private readonly int _depthChildLevelWidth = 20;

    public int DepthLever
    {
        get => _depthLever;

        set
        {
            if (_depthLever != value)
            {
                _depthLever = value;
                SetDepth(_depthLever);
            }
        }
    }

    private void Awake()
    {
        _button.OnBeginDragAsObservable().Subscribe(data => { OnItemBeginDrag?.Invoke(data); });
        _button.OnDragAsObservable().Subscribe(data => { OnItemDrag?.Invoke(data); });
        _button.OnEndDragAsObservable().Subscribe(data => { OnItemEndDrag?.Invoke(data); });


        _foldoutButton.OnPointerClickAsObservable().Subscribe(data => { OnFoldoutClick?.Invoke(data); });


        _button.OnPointerClickAsObservable().Subscribe(data => { OnItemClicked?.Invoke(data); });
        _button.OnPointerDownAsObservable().Subscribe(data => { OnItemClickDown?.Invoke(data); });
        _button.OnPointerUpAsObservable().Subscribe(data => { OnItemClickUp?.Invoke(data); });


        _button.OnPointerEnterAsObservable().Subscribe(data => { OnItemEnter?.Invoke(data); });
        _button.OnDragAsObservable().Subscribe(data => { OnItemDrop?.Invoke(data); });
        _button.OnPointerExitAsObservable().Subscribe(data => { OnItemExit?.Invoke(data); });
    }

    public void SetDepth(int depth)
    {
        var depthOfLayout = _depthChildLevelWidth * depth;
        _layoutGroup.padding.left = depthOfLayout;
        LayoutRebuilder.MarkLayoutForRebuild(_layoutGroup.GetComponent<RectTransform>());
    }

    public event Action<PointerEventData> OnFoldoutClick;
    public event Action<PointerEventData> OnItemClicked;
    public event Action<PointerEventData> OnItemClickDown;
    public event Action<PointerEventData> OnItemClickUp;
    public event Action<PointerEventData> OnItemBeginDrag;
    public event Action<PointerEventData> OnItemDrag;
    public event Action<PointerEventData> OnItemEndDrag;
    public event Action<PointerEventData> OnItemDrop;
    public event Action<PointerEventData> OnItemEnter;
    public event Action<PointerEventData> OnItemExit;
}