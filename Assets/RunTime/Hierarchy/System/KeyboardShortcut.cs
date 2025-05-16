using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// TODO: Register command key (command key, condition)
// TODO: Warning when conflict command key
// TODO: Add Enable/Disable
public class KeyboardShortcut : MonoBehaviour
{
    public static bool IsControlPressed;
    public static bool IsShiftPressed;
    public static bool IsAltPressed;
    public static bool IsActive = true;
    [SerializeField] private HierarchyKeyboardShortcutConfig _hierarchyKeyboardShortcutConfig;
    private readonly List<KeyCommand> _listKeyCommand = new();

    private readonly List<string> _listOfCombineKeyCode = new()
    {
        KeyCode.LeftControl.ToString(),
        KeyCode.LeftAlt.ToString(),
        KeyCode.LeftShift.ToString(),
        KeyCode.RightControl.ToString(),
        KeyCode.RightAlt.ToString(),
        KeyCode.RightShift.ToString()
    };

    private string _keyHold = "";
    private string _keyPressed = "";

    public HierarchyKeyboardShortcutConfig HierarchyKeyboardShortcutConfig
    {
        get => _hierarchyKeyboardShortcutConfig;
        set => _hierarchyKeyboardShortcutConfig = value;
    }

    private void Update()
    {
        if (!IsActive) return;
        // if (!RuntimeEditor.IsRuntimeEditor || RuntimeEditor.IsBusy) return;

        if (Input.anyKey && string.IsNullOrEmpty(_keyHold)) _keyHold = GetKeyHold();

        if (Input.anyKeyDown)
        {
            _keyPressed = GetKeyPressed();
            foreach (var keyCommand in _listKeyCommand)
            {
                var validateMainKey = keyCommand.ListKeys.Select(key => key.ToString()).Contains(_keyPressed) &&
                                      keyCommand.Condition();
                if (!_listOfCombineKeyCode.Contains(_keyHold))
                {
                    var doesCommandContainCombineKey = keyCommand.ListKeys.Select(key => key.ToString())
                        .Any(key => _listOfCombineKeyCode.Any(combineKey => key == combineKey));
                    if (validateMainKey && !doesCommandContainCombineKey)
                    {
                        keyCommand.RegisteredAction?.Invoke();
                        _keyPressed = "";
                    }
                }
                else
                {
                    var doesCommandContainKeyHold =
                        keyCommand.ListKeys.Select(key => key.ToString()).Contains(_keyHold);
                    var isKeyPressedDifferentWithKeyHold = _keyPressed != _keyHold;
                    if (validateMainKey && doesCommandContainKeyHold && isKeyPressedDifferentWithKeyHold)
                    {
                        keyCommand.RegisteredAction?.Invoke();
                        _keyPressed = "";
                    }
                }
            }
        }

        if (!Input.anyKey && (!string.IsNullOrEmpty(_keyHold) || !string.IsNullOrEmpty(_keyPressed)))
        {
            _keyPressed = "";
            _keyHold = "";
            foreach (var keyCommand in _listKeyCommand) keyCommand.CancelledAction?.Invoke();
        }

        GetModifierKeys();
    }

    private string GetKeyHold()
    {
        foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
            if (Input.GetKey(keyCode))
                return keyCode.ToString();

        return "";
    }

    private string GetKeyPressed()
    {
        foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
            if (Input.GetKeyDown(keyCode))
                return keyCode.ToString();

        return "";
    }

    private void GetModifierKeys()
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            if (!IsControlPressed)
                IsControlPressed = true;

        if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl))
            if (IsControlPressed)
                IsControlPressed = false;

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            if (!IsShiftPressed)
                IsShiftPressed = true;

        if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
            if (IsShiftPressed)
                IsShiftPressed = false;

        if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
            if (!IsAltPressed)
                IsAltPressed = true;

        if (!Input.GetKey(KeyCode.LeftAlt) && !Input.GetKey(KeyCode.RightAlt))
            if (IsAltPressed)
                IsAltPressed = false;
    }

    public void RegisterKeyCommand(KeyCode keyCode, Func<bool> condition, Action onActionAdded = null,
        Action onActionCancelled = null)
    {
        RegisterKeyCommand(new List<KeyCode> { keyCode }, condition, onActionAdded, onActionCancelled);
    }

    public void RegisterKeyCommand(List<KeyCode> keyList, Func<bool> condition, Action onActionAdded = null,
        Action onActionCancelled = null)
    {
        var newKeyAction = new KeyCommand
        {
            ListKeys = keyList,
            Condition = condition,
            RegisteredAction = onActionAdded,
            CancelledAction = onActionCancelled
        };

        _listKeyCommand.Add(newKeyAction);
    }

    public void UnregisterKeyCommand(KeyCommand keyCommand)
    {
        foreach (var command in _listKeyCommand)
            if (command.ListKeys.All(keyCommand.ListKeys.Contains) && command.Condition == keyCommand.Condition)
                _listKeyCommand.Remove(command);
    }
}

public enum ModifierKeys
{
    None,
    Control,
    Shift,
    Alt
}

public class KeyCommand
{
    public Action CancelledAction;
    public Func<bool> Condition;
    public List<KeyCode> ListKeys;
    public Action RegisteredAction;
}