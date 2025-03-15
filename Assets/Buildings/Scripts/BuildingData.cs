using UnityEngine;

public class BuildingData : MonoBehaviour
{
    public string BuildingName { get; set; }
    public Vector2Int GridPosition { get; set; }
    public Vector2Int Size { get; set; }

    public void Initialize(string name, Vector2Int gridPos, Vector2Int size)
    {
        BuildingName = name;
        GridPosition = gridPos;
        Size = size;
    }
}