using UnityEngine;

public class HierarchyContextMenu : MonoBehaviour
{
    [SerializeField] private HierarchyWindow _hierarchy;
    private readonly string _suffixForTempItemCopy = " #(Temp Copy Item)#";
    [SerializeField] private IUnityContextMenu _contextMenu;


    public HierarchyItem SelectedItem => _hierarchy.SelectedItem;

    private void Awake()
    {
        _hierarchy = FindObjectOfType<HierarchyWindow>(true);
    }

    public void OpenContextMenu(Vector2 point)
    {
        _contextMenu = new UnityContextMenu();

        //	_contextMenu.AddItem("Create/Empty Object", true, () => RteObjectCreator.CreateEmptyObject());

        _contextMenu.AddItem("Rename", _hierarchy.CanRenameItem(), EnableRenameField);


//
        _contextMenu.Show(point);
    }

    private Vector2 CalculateContextMenuPosition(Vector2 contextMenuSize)
    {
        float xPosition;
        float yPosition;
        if (Input.mousePosition.y < contextMenuSize.y)
            yPosition = Input.mousePosition.y + contextMenuSize.y;
        else
            yPosition = Input.mousePosition.y;
        if (Input.mousePosition.x > Screen.width - contextMenuSize.x)
            xPosition = Screen.width - _contextMenu.Size.x / 2;
        else
            xPosition = Input.mousePosition.x + _contextMenu.Size.x / 2;
        return new Vector2(xPosition, yPosition);
    }


    public void EnableRenameField()
    {
        if (SelectedItem == null) return;
        SelectedItem.OpenRenameField(SelectedItem.ObjectName);
    }
}