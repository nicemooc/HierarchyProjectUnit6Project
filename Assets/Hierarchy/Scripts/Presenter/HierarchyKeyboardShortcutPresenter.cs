using System;
using UnityEngine;

public class HierarchyKeyboardShortcutPresenter
{
    private HierarchyKeyboardShortcutConfig _config;
    private IDisposable _delayChangeItem;
    private HierarchyContextMenuPresenter _hierarchyContextMenu;
    private HierarchyNavigation _hierarchyNavigation;

    private HierarchyWindowPresenter _hierarchyWindow;

    private KeyboardShortcut _keyboardShortcut;
    private KeyCode _toggleActiveObjectKey = KeyCode.Space;

    private HierarchyItem SelectedItem => _hierarchyWindow.SelectedItem;

    public void Init(HierarchyContext context)

    {
        _hierarchyNavigation = context.HierarchyNavigation;
        _hierarchyWindow = context.HierarchyWindowPresenter;
        _hierarchyContextMenu = context.HierarchyContextMenuPresenter;
        //  _config = context.HierarchyKeyboardShortcutConfig;
        _keyboardShortcut = context.KeyboardShortcut;
        InitKeyboardRegister();
    }

    private void InitKeyboardRegister()
    {
        _keyboardShortcut.RegisterKeyCommand(KeyCode.Return, () => SelectedItem != null && SelectedItem.IsRenaming,
            () => { SelectedItem.CloseRenameFieldAndSaveNewName(); });
    }
}