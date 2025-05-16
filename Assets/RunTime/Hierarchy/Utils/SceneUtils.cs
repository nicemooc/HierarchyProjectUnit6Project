using TMPro;
using UnityEngine.EventSystems;

internal static class SceneUtils
{
    public static bool IsNotFocusInputField()
    {
        if (EventSystem.current.currentSelectedGameObject == null) return true;

        var tmpInputField = EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>();
        if (tmpInputField == null) return true;
        if (tmpInputField.GetType() == typeof(TMP_InputField)) return false;

        return true;
    }
}