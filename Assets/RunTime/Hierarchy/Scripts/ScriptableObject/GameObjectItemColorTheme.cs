using UnityEngine;

[CreateAssetMenu(fileName = "GameObjectItemColorTheme", menuName = "Scriptable Objects/GameObjectItemColorTheme")]
public class GameObjectItemColorTheme : ScriptableObject
{
    public
        Color _normalColor = Color.white;

    public
        Color _focusColor = Color.yellow * 0.8f;

    public
        Color _selectedColor = Color.blue;

    public
        Color _missingColor = Color.red;

    public
        Color _missingSelectedColor = Color.yellow;

    public
        Color _normalIconColor = Color.black;

    public
        Color _selectedIconColor = Color.white;

    public
        Color _inactiveIconColor = Color.black * 0.5f;

    public
        Sprite _foldoutOn;

    public
        Sprite _foldoutOff;
}