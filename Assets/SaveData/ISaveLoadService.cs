using System;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveLoadService
{
    void SaveBuilding(Vector2Int position, Tuple<string, Vector2Int> buildingData);
    void RemoveBuilding(Vector2Int position);
    Dictionary<Vector2Int, Tuple<string, Vector2Int>> GetBuildings();
    void ClearSaves();
}