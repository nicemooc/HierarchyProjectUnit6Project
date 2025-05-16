using System;
using UniRx;
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

    private HierarchyKeyboardShortcutConfig _keyboardShortcutConfig =>
        _keyboardShortcut.HierarchyKeyboardShortcutConfig;

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

        _keyboardShortcut.RegisterKeyCommand(_keyboardShortcutConfig._navigationDownKey,
            () => _hierarchyWindow.CanNavigationItem(), delegate
            {
                _hierarchyWindow.UpdateListItemsSortByIndex();
                if (_delayChangeItem != null) _delayChangeItem.Dispose();
                _hierarchyNavigation.MoveToNextItem();
                _delayChangeItem = Observable.Timer(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.025))
                    .Subscribe(time => { _hierarchyNavigation.MoveToNextItem(); });
            }, delegate
            {
                if (_delayChangeItem != null) _delayChangeItem.Dispose();
            });

        _keyboardShortcut.RegisterKeyCommand(_keyboardShortcutConfig._navigationUpKey,
            () => _hierarchyWindow.CanNavigationItem(), delegate
            {
                _hierarchyWindow.UpdateListItemsSortByIndex();
                if (_delayChangeItem != null) _delayChangeItem.Dispose();
                _hierarchyNavigation.MoveToPreviousItem();
                _delayChangeItem = Observable.Timer(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.025))
                    .Subscribe(time => { _hierarchyNavigation.MoveToPreviousItem(); });
            }, delegate
            {
                if (_delayChangeItem != null) _delayChangeItem.Dispose();
            });

        _keyboardShortcut.RegisterKeyCommand(_keyboardShortcutConfig._navigationLeftKey,
            () => _hierarchyWindow.CanNavigationItem() && !_hierarchyWindow.IsSearching, delegate
            {
                _hierarchyWindow.UpdateListItemsSortByIndex();
                if (_delayChangeItem != null) _delayChangeItem.Dispose();
                _hierarchyNavigation.MoveToPreviousItemWithFoldoutButton();
                _delayChangeItem = Observable.Timer(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.025))
                    .Subscribe(time => { _hierarchyNavigation.MoveToPreviousItemWithFoldoutButton(); });
            }, delegate
            {
                if (_delayChangeItem != null) _delayChangeItem.Dispose();
            });

        _keyboardShortcut.RegisterKeyCommand(_keyboardShortcutConfig._navigationRightKey,
            () => _hierarchyWindow.CanNavigationItem() && !_hierarchyWindow.IsSearching, delegate
            {
                _hierarchyWindow.UpdateListItemsSortByIndex();
                if (_delayChangeItem != null) _delayChangeItem.Dispose();
                _hierarchyNavigation.MoveToNextItemWithFoldoutButton();
                _delayChangeItem = Observable.Timer(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.025))
                    .Subscribe(time => { _hierarchyNavigation.MoveToNextItemWithFoldoutButton(); });
            }, delegate
            {
                if (_delayChangeItem != null) _delayChangeItem.Dispose();
            });
    }
}