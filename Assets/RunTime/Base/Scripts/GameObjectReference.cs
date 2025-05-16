using System;
using UnityEngine;

[Serializable]
public class GameObjectReference
{
    public int Depth;
    public GameObject GameObjectRef;
    public bool IsActiveInView = true;

    public string Name = "New Object";
    public string ParentId = string.Empty;

    public GameObjectReference(GameObject gameObjectRef)
    {
        Name = gameObjectRef.name;
        GameObjectRef = gameObjectRef;
    }

    public string Id { get; private set; } = string.Empty;

    public bool IsRoot => ParentId == string.Empty;
}