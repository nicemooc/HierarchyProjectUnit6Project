using System;
using NaughtyAttributes;
using TMPro;
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
    [SerializeField] private Image _bgImage;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_InputField _nameInputField;
    [SerializeField] private LayoutGroup _layoutGroup;
    [SerializeField] private Image _foldoutImage;
    [SerializeField] private GameObject _topGuideline;
    [SerializeField] private GameObject _bottomGuideline;
    [SerializeField] private GameObjectItemColorTheme _colorTheme;

    [SerializeField] [ReadOnly] private int _depthLever;
    public bool DoesNeedTempDepth = true;
    private readonly int _depthChildLevelWidth = 20;
    private readonly float offSetGuideline = 5f;

    public TMP_Text NameText => _nameText;
    public string ObjectName => _nameText.text;
    public string RenameText => _nameInputField.text;

    public Image FoldoutImage => _foldoutImage;

    public int DepthLever
    {
        get => _depthLever;

        set
        {
            if (_depthLever != value)
            {
                _depthLever = value;
                SetDepth();
            }
        }
    }

    private void Awake()
    {
        SetDepth();
        _nameInputField.onDeselect.AddListener(input => { OnRenameFieldDeselect?.Invoke(input); });
        _button.OnBeginDragAsObservable().Subscribe(data => { OnItemBeginDrag?.Invoke(data); });
        _button.OnDragAsObservable().Subscribe(data => { OnItemDrag?.Invoke(data); });
        _button.OnEndDragAsObservable().Subscribe(data => { OnItemEndDrag?.Invoke(data); });
        _button.OnDropAsObservable().Subscribe(data => { OnItemDrop?.Invoke(data); });


        _foldoutButton.OnPointerClickAsObservable().Subscribe(data => { OnFoldoutClick?.Invoke(); });


        _button.OnPointerClickAsObservable().Subscribe(data => { OnItemClicked?.Invoke(data); });
        _button.OnPointerDownAsObservable().Subscribe(data => { OnItemClickDown?.Invoke(data); });
        _button.OnPointerUpAsObservable().Subscribe(data => { OnItemClickUp?.Invoke(data); });


        _button.OnPointerEnterAsObservable().Subscribe(data => { OnItemEnter?.Invoke(data); });
        _button.OnPointerExitAsObservable().Subscribe(data => { OnItemExit?.Invoke(data); });
    }

    public void Select()
    {
        //   _nameText.color = Color.white;
        _bgImage.color = _colorTheme._selectedColor;
    }

    public void Deselect()
    {
        _bgImage.color = _colorTheme._normalColor;
    }

    public void ShowFoldout(bool show)
    {
        _foldoutButton.gameObject.SetActive(show);
    }

    public void SetDisplay(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    public void SetDepth(bool trueDepth = true)
    {
        int tempDepth;
        if (trueDepth)
            tempDepth = DepthLever;
        else
            tempDepth = 0;
        var depthOfLayout = _depthChildLevelWidth * tempDepth;
        _layoutGroup.padding.left = depthOfLayout;
        LayoutRebuilder.MarkLayoutForRebuild(_layoutGroup.GetComponent<RectTransform>());
    }

    public void OpenRenameField(string name)
    {
        _nameText.gameObject.SetActive(false);
        _nameInputField.text = name;
        _nameInputField.gameObject.SetActive(true);
        _nameInputField.Select();
    }

    public void CloseRenameField()
    {
        // if (RenameText == "") _nameInputField.text = ObjectName;
        _nameText.gameObject.SetActive(true);
        _nameInputField.gameObject.SetActive(false);

        if (!EventSystem.current.alreadySelecting) EventSystem.current.SetSelectedGameObject(null);
    }


    public void SetTitle(string title)
    {
        _nameText.text = title;
        _nameInputField.text = title;
        gameObject.name = title + "_View";
    }


    public void ForceUpdateDepthLayout()
    {
        _layoutGroup.enabled = false;
        _layoutGroup.enabled = true;
    }

    public void SetFoldoutSprite(bool isOn)
    {
        FoldoutImage.sprite = isOn ? _colorTheme._foldoutOn : _colorTheme._foldoutOff;
    }

    public void SetTopGuideline(bool isActive)
    {
        if (isActive) SetSizeGuideline(_topGuideline);
        _topGuideline.SetActive(isActive);
    }

    public void SetBottomGuideline(bool isActive)
    {
        if (isActive) SetSizeGuideline(_bottomGuideline);
        _bottomGuideline.SetActive(isActive);
    }

    public void SetSizeGuideline(GameObject guideline)
    {
        var newWidth = GetComponent<RectTransform>().rect.width -
                       _foldoutImage.GetComponent<RectTransform>().rect.width - _depthChildLevelWidth * _depthLever -
                       offSetGuideline;
        ;

        guideline.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
    }

    public void SetActiveGuideline(bool isActive)
    {
        SetTopGuideline(isActive);
        SetBottomGuideline(isActive);
    }

    public event Action OnFoldoutClick;
    public event Action<PointerEventData> OnItemClicked;
    public event Action<PointerEventData> OnItemClickDown;
    public event Action<PointerEventData> OnItemClickUp;
    public event Action<PointerEventData> OnItemBeginDrag;
    public event Action<PointerEventData> OnItemDrag;
    public event Action<PointerEventData> OnItemEndDrag;
    public event Action<PointerEventData> OnItemDrop;
    public event Action<PointerEventData> OnItemEnter;
    public event Action<PointerEventData> OnItemExit;
    public event Action<string> OnRenameFieldDeselect;
}