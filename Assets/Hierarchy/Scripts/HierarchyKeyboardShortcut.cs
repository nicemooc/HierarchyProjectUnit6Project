using System;
using UnityEngine;
using UnityEngine.Serialization;

public class HierarchyKeyboardShortcut : MonoBehaviour
{
    [SerializeField] private HierarchyContextMenu _hierarchyContextMenu;
    [SerializeField] private HierarchyNavigation _hierarchyNavigation;

    [SerializeField] private KeyboardShortcut _keyboardShortcut;

//	SceneViewWindow _sceneViewWindow;
    [SerializeField] private HierarchyWindow _hierarchy;

    [FormerlySerializedAs("navigationUpKey")] [SerializeField]
    private KeyCode _navigationUpKey = KeyCode.UpArrow;

    [FormerlySerializedAs("navigationDownKey")] [SerializeField]
    private KeyCode _navigationDownKey = KeyCode.DownArrow;

    [FormerlySerializedAs("navigationLeftKey")] [SerializeField]
    private KeyCode _navigationLeftKey = KeyCode.LeftArrow;

    [FormerlySerializedAs("navigationRightKey")] [SerializeField]
    private KeyCode _navigationRightKey = KeyCode.RightArrow;

    [FormerlySerializedAs("focusObjectKey")] [Space] [SerializeField]
    private KeyCode _focusObjectKey = KeyCode.F;

    [FormerlySerializedAs("deleteObjectKey")] [SerializeField]
    private KeyCode _deleteObjectKey = KeyCode.Delete;

    private IDisposable _delayChangeItem;
    private KeyCode _toggleActiveObjectKey = KeyCode.Space;

    private HierarchyItem SelectedItem => _hierarchy.SelectedItem;

    private void Awake()
    {
        InitKeyboardRegister();
    }

    private void InitKeyboardRegister()
    {
        _keyboardShortcut.RegisterKeyCommand(KeyCode.Return, () => SelectedItem != null && SelectedItem.IsRenaming,
            () => { SelectedItem.CloseRenameFieldAndSaveNewName(); });
    }
}