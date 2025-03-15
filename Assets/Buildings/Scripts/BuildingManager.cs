using System.Collections.Generic;
using System;
using UnityEngine;
using Zenject;

public class BuildingManager : MonoBehaviour
{
    [SerializeField] private BuildingConfig[] buildingConfigs;
    private BuildingConfig selectedBuilding;
    private GameObject previewInstance;
    private readonly List<GameObject> placedBuildings = new();
    private IInputService inputService;
    private IGridService gridService;
    private ISaveLoadService saveLoadService;

    private enum BuildingMode { None, Place, Remove }
    private BuildingMode currentMode = BuildingMode.None;
    private Vector2Int lastPreviewGridPos;

    [Inject]
    public void Construct(IInputService input, IGridService grid, ISaveLoadService saveLoad)
    {
        inputService = input;
        gridService = grid;
        saveLoadService = saveLoad;

        var savedBuildings = saveLoadService.GetBuildings();
        foreach (var kvp in savedBuildings)
        {
            var config = Array.Find(buildingConfigs, c => c.Name == kvp.Value.Item1);
            if (config != null)
            {
                Vector3 pos = new(kvp.Key.x, 0, kvp.Key.y);
                Vector3 adjustedPos = pos + new Vector3((config.Size.x * gridService.CellSize - gridService.CellSize) * 0.5f, 0, (config.Size.y * gridService.CellSize - gridService.CellSize) * 0.5f);
                var building = Instantiate(config.Prefab, adjustedPos, Quaternion.identity);
                Vector3 scale = new(config.Size.x * gridService.CellSize, gridService.CellSize, config.Size.y * gridService.CellSize);
                building.transform.localScale = scale;
                gridService.OccupyArea(kvp.Key, config.Size);

                BuildingData buildingData = building.AddComponent<BuildingData>();
                buildingData.Initialize(config.Name, kvp.Key, config.Size);

                placedBuildings.Add(building);
                SetBuildingColor(building, Color.white);
            }
        }
    }

    public void Update()
    {
        if (inputService.IsLeftClickPressed)
        {
            if (currentMode == BuildingMode.Place)
            {
                PlaceBuilding();
            }
            else if (currentMode == BuildingMode.Remove)
            {
                RemoveBuilding();
            }
        }

        if (currentMode == BuildingMode.Place && selectedBuilding != null)
        {
            UpdatePreview();
        }
        else if (previewInstance != null)
        {
            Destroy(previewInstance);
            previewInstance = null;
        }
    }
    public void OnMouseOverUIChanged(bool isOverUI)
    {
        if (isOverUI && currentMode != BuildingMode.None)
        {
            currentMode = BuildingMode.None;
            if (previewInstance != null)
            {
                Destroy(previewInstance);
                previewInstance = null;
            }
        }
    }
    public void SetPlaceMode()
    {
        currentMode = BuildingMode.Place;
        if (previewInstance != null)
        {
            Destroy(previewInstance);
            previewInstance = null;
        }
    }

    public void SetRemoveMode()
    {
        currentMode = BuildingMode.Remove;
        if (previewInstance != null)
        {
            Destroy(previewInstance);
            previewInstance = null;
        }
    }

    public void SelectBuilding(int index)
    {
        selectedBuilding = buildingConfigs[index];
        if (currentMode == BuildingMode.Place && previewInstance != null)
        {
            Destroy(previewInstance);
            previewInstance = null;
        }
    }

    private void UpdatePreview()
    {
        Ray ray = Camera.main.ScreenPointToRay(inputService.MousePosition);
        Plane groundPlane = new(Vector3.up, Vector3.zero);
        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 worldPos = ray.GetPoint(distance);
            Vector3 snappedPos = gridService.SnapToGrid(worldPos);
            Vector2Int gridPos = new(Mathf.FloorToInt(snappedPos.x / gridService.CellSize), Mathf.FloorToInt(snappedPos.z / gridService.CellSize));

            gridPos.x = Mathf.Clamp(gridPos.x, -gridService.GridSize, gridService.GridSize - selectedBuilding.Size.x + 1);
            gridPos.y = Mathf.Clamp(gridPos.y, -gridService.GridSize, gridService.GridSize - selectedBuilding.Size.y + 1);
            snappedPos = new Vector3(gridPos.x * gridService.CellSize, 0, gridPos.y * gridService.CellSize);

            Vector3 adjustedPos = snappedPos + new Vector3((selectedBuilding.Size.x * gridService.CellSize - gridService.CellSize) * 0.5f, 0, (selectedBuilding.Size.y * gridService.CellSize - gridService.CellSize) * 0.5f);

            if (previewInstance == null)
            {
                previewInstance = Instantiate(selectedBuilding.Prefab, adjustedPos, Quaternion.identity);
                Vector3 scale = new(selectedBuilding.Size.x * gridService.CellSize, gridService.CellSize, selectedBuilding.Size.y * gridService.CellSize);
                previewInstance.transform.localScale = scale;
                SetPreviewTransparency(previewInstance, true);
            }
            else
            {
                previewInstance.transform.position = adjustedPos;
                bool canPlace = gridService.IsAreaAvailable(gridPos, selectedBuilding.Size) && gridService.IsAreaWithinBounds(gridPos, selectedBuilding.Size);
                SetPreviewColor(previewInstance, canPlace);
            }

            lastPreviewGridPos = gridPos;
        }
    }

    private void PlaceBuilding()
    {
        if (previewInstance == null) return;

        Vector2Int gridPos = lastPreviewGridPos;

        if (gridService.IsAreaAvailable(gridPos, selectedBuilding.Size) && gridService.IsAreaWithinBounds(gridPos, selectedBuilding.Size))
        {
            SetPreviewTransparency(previewInstance, false);
            gridService.OccupyArea(gridPos, selectedBuilding.Size);
            saveLoadService.SaveBuilding(gridPos, new Tuple<string, Vector2Int>(selectedBuilding.Name, selectedBuilding.Size));

            BuildingData buildingData = previewInstance.AddComponent<BuildingData>();
            buildingData.Initialize(selectedBuilding.Name, gridPos, selectedBuilding.Size);

            placedBuildings.Add(previewInstance);
            previewInstance = null;
        }
    }

    public void RemoveBuilding()
    {
        Ray ray = Camera.main.ScreenPointToRay(inputService.MousePosition);
        Plane groundPlane = new(Vector3.up, Vector3.zero);
        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 worldPos = ray.GetPoint(distance);

            GameObject buildingToRemove = null;
            BuildingData buildingData = null;

            foreach (var building in placedBuildings)
            {
                if (!building.TryGetComponent<BuildingData>(out var data)) continue;

                Vector2Int gridPos = data.GridPosition;
                Vector2Int size = data.Size;

                float minX = gridPos.x * gridService.CellSize;
                float maxX = (gridPos.x + size.x) * gridService.CellSize;
                float minZ = gridPos.y * gridService.CellSize;
                float maxZ = (gridPos.y + size.y) * gridService.CellSize;

                if (worldPos.x >= minX && worldPos.x < maxX &&
                    worldPos.z >= minZ && worldPos.z < maxZ)
                {
                    buildingToRemove = building;
                    buildingData = data;
                    break;
                }
            }

            if (buildingToRemove != null && buildingData != null)
            {
                Vector2Int gridPos = buildingData.GridPosition;
                Vector2Int size = buildingData.Size;

                placedBuildings.Remove(buildingToRemove);
                Destroy(buildingToRemove);
                gridService.FreeArea(gridPos, size);
                saveLoadService.RemoveBuilding(gridPos);
            }
        }
    }

    private void SetPreviewColor(GameObject obj, bool canPlace)
    {
        if (obj.TryGetComponent<Renderer>(out var renderer))
        {
            Color color = canPlace ? Color.green : Color.red;
            color.a = 0.5f;
            renderer.material.color = color;
        }
    }

    private void SetPreviewTransparency(GameObject obj, bool isPreview)
    {
        if (obj.TryGetComponent<Renderer>(out var renderer))
        {
            Color color = renderer.material.color;
            color.a = isPreview ? 0.5f : 1f;
            if (!isPreview) color = Color.white;
            renderer.material.color = color;
        }
    }

    private void SetBuildingColor(GameObject obj, Color color)
    {
        if (obj.TryGetComponent<Renderer>(out var renderer))
        {
            color.a = 1f;
            renderer.material.color = color;
        }
    }
}