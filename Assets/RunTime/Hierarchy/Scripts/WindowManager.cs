using UnityEngine;

public class WindowManager : MonoBehaviour
{
    [SerializeField] private HierarchyWindowView _hierarchyWindowView;
    [SerializeField] private KeyboardShortcut _keyboardShortcut;
    private HierarchyContext _hierarchyContext;


    public HierarchyWindowPresenter HierarchyWindowPresenter => _hierarchyContext.HierarchyWindowPresenter;

    public HierarchyContextMenuPresenter HierarchyContextMenuPresenter =>
        _hierarchyContext.HierarchyContextMenuPresenter;

    public HierarchyKeyboardShortcutPresenter HierarchyKeyboardShortcutPresenter =>
        _hierarchyContext.HierarchyKeyboardShortcutPresenter;

    public HierarchyNavigation HierarchyNavigation => _hierarchyContext.HierarchyNavigation;


    private void Awake()

    {
        _hierarchyContext = new HierarchyContext();
        _hierarchyContext.HierarchyWindowView = _hierarchyWindowView;
        _hierarchyContext.KeyboardShortcut = _keyboardShortcut;

        _hierarchyContext.HierarchyWindowPresenter = new HierarchyWindowPresenter();
        _hierarchyContext.HierarchyContextMenuPresenter = new HierarchyContextMenuPresenter();
        _hierarchyContext.HierarchyKeyboardShortcutPresenter = new HierarchyKeyboardShortcutPresenter();
        _hierarchyContext.HierarchyNavigation = new HierarchyNavigation();

        HierarchyWindowPresenter.Init(_hierarchyContext);
        HierarchyContextMenuPresenter.Init(_hierarchyContext);
        HierarchyKeyboardShortcutPresenter.Init(_hierarchyContext);
        HierarchyNavigation.Init(_hierarchyContext);
    }
}