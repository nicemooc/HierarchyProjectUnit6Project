using UnityEngine;

[CreateAssetMenu(fileName = "HierarchyKeyboardShortcutConfig",
    menuName = "Scriptable Objects/HierarchyKeyboardShortcutConfig")]
public class HierarchyKeyboardShortcutConfig : ScriptableObject
{
    public KeyCode _navigationUpKey = KeyCode.UpArrow;

    public KeyCode _navigationLeftKey = KeyCode.LeftArrow;

    public KeyCode _navigationRightKey = KeyCode.RightArrow;

    public KeyCode _focusObjectKey = KeyCode.F;

    public KeyCode _deleteObjectKey = KeyCode.Delete;

    private KeyCode _navigationDownKey = KeyCode.DownArrow;
}