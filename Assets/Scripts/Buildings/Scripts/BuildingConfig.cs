using UnityEngine;

[CreateAssetMenu(fileName = "BuildingConfig", menuName = "Configs/BuildingConfig")]
public class BuildingConfig : ScriptableObject
{
    public string Name;
    public GameObject Prefab;
    public Vector2Int Size;
}