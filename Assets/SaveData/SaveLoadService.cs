using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public class SaveLoadService : ISaveLoadService
{
    private readonly string filePath = Path.Combine(Application.persistentDataPath, "buildings.json");
    private Dictionary<Vector2Int, Tuple<string, Vector2Int>> buildings = new();

    public SaveLoadService()
    {
        LoadBuildings();
    }

    public void SaveBuilding(Vector2Int position, Tuple<string, Vector2Int> buildingData)
    {
        buildings[position] = buildingData;
        SaveToFile();
    }

    public void RemoveBuilding(Vector2Int position)
    {
        if (buildings.ContainsKey(position))
        {
            buildings.Remove(position);
            SaveToFile();
        }
    }

    public Dictionary<Vector2Int, Tuple<string, Vector2Int>> GetBuildings()
    {
        return new Dictionary<Vector2Int, Tuple<string, Vector2Int>>(buildings);
    }

    public void ClearSaves()
    {
        buildings.Clear();
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    private void SaveToFile()
    {
        string json = JsonUtility.ToJson(new SerializableBuildings(buildings));
        File.WriteAllText(filePath, json);
    }

    private void LoadBuildings()
    {
        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                SerializableBuildings data = JsonUtility.FromJson<SerializableBuildings>(json);
                buildings = data.ToDictionary();
            }
            catch (Exception)
            {
                ClearSaves();
            }
        }
    }

    [Serializable]
    private class SerializableBuildings
    {
        public List<Vector2Int> positions;
        public List<SerializableBuildingData> buildingData;

        public SerializableBuildings(Dictionary<Vector2Int, Tuple<string, Vector2Int>> dict)
        {
            positions = new List<Vector2Int>(dict.Keys);
            buildingData = new List<SerializableBuildingData>();
            foreach (var data in dict.Values)
            {
                buildingData.Add(new SerializableBuildingData(data.Item1, data.Item2));
            }
        }

        public Dictionary<Vector2Int, Tuple<string, Vector2Int>> ToDictionary()
        {
            var dict = new Dictionary<Vector2Int, Tuple<string, Vector2Int>>();
            if (positions == null || buildingData == null)
            {
                return dict;
            }

            int minLength = Mathf.Min(positions.Count, buildingData.Count);
            for (int i = 0; i < minLength; i++)
            {
                if (i >= positions.Count || i >= buildingData.Count)
                {
                    break;
                }
                dict[positions[i]] = new Tuple<string, Vector2Int>(buildingData[i].Name, buildingData[i].Size);
            }
            return dict;
        }
    }

    [Serializable]
    private class SerializableBuildingData
    {
        public string Name;
        public Vector2Int Size;

        public SerializableBuildingData(string name, Vector2Int size)
        {
            Name = name;
            Size = size;
        }
    }
}