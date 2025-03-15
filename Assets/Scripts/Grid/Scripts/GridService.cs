using System.Collections.Generic;
using UnityEngine;

namespace Grid
{
    public class GridService : IGridService
    {
        private readonly float cellSize = 1f;
        private readonly int gridSize = 10;
        private readonly Dictionary<Vector2Int, bool> occupiedCells = new();
        private readonly ISaveLoadService saveLoadService;

        public GridService(ISaveLoadService saveLoad)
        {
            saveLoadService = saveLoad;
            LoadOccupiedCells();
        }

        public float CellSize => cellSize;
        public int GridSize => gridSize;

        public Vector3 SnapToGrid(Vector3 position)
        {
            float x = Mathf.Round(position.x / cellSize) * cellSize;
            float z = Mathf.Round(position.z / cellSize) * cellSize;
            return new Vector3(x, 0, z);
        }

        public bool IsCellOccupied(Vector2Int gridPosition) => occupiedCells.ContainsKey(gridPosition);
        public void OccupyCell(Vector2Int gridPosition) => occupiedCells[gridPosition] = true;
        public void FreeCell(Vector2Int gridPosition) => occupiedCells.Remove(gridPosition);
        public void ClearAllCells() => occupiedCells.Clear();

        public bool IsPositionWithinBounds(Vector2Int gridPosition)
        {
            return gridPosition.x >= -gridSize && gridPosition.x <= gridSize &&
                   gridPosition.y >= -gridSize && gridPosition.y <= gridSize;
        }

        public bool IsAreaWithinBounds(Vector2Int startPosition, Vector2Int size)
        {
            if (!IsPositionWithinBounds(startPosition)) return false;

            int maxX = startPosition.x + size.x - 1;
            int maxZ = startPosition.y + size.y - 1;

            return maxX <= gridSize && maxZ <= gridSize && maxX >= -gridSize && maxZ >= -gridSize;
        }

        public bool IsAreaAvailable(Vector2Int startPosition, Vector2Int size)
        {
            if (!IsAreaWithinBounds(startPosition, size))
            {
                return false;
            }

            int maxX = startPosition.x + size.x - 1;
            int maxZ = startPosition.y + size.y - 1;

            for (int x = startPosition.x; x <= maxX; x++)
            {
                for (int z = startPosition.y; z <= maxZ; z++)
                {
                    Vector2Int cell = new Vector2Int(x, z);
                    if (occupiedCells.ContainsKey(cell) && occupiedCells[cell])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void OccupyArea(Vector2Int startPosition, Vector2Int size)
        {
            for (int x = startPosition.x; x < startPosition.x + size.x; x++)
            {
                for (int z = startPosition.y; z < startPosition.y + size.y; z++)
                {
                    Vector2Int cell = new Vector2Int(x, z);
                    occupiedCells[cell] = true;
                }
            }
        }

        public void FreeArea(Vector2Int startPosition, Vector2Int size)
        {
            var freeCell = new List<Vector2Int>();
            for (int x = startPosition.x; x < startPosition.x + size.x; x++)
            {
                for (int z = startPosition.y; z < startPosition.y + size.y; z++)
                {
                    Vector2Int cell = new(x, z);
                    if (occupiedCells.ContainsKey(cell))
                    {
                        freeCell.Add(cell);
                    }
                }
            }
            foreach (var cell in freeCell)
            {
                occupiedCells.Remove(cell);
            }
        }

        private void LoadOccupiedCells()
        {
            occupiedCells.Clear();
            var savedBuildings = saveLoadService.GetBuildings();
            foreach (var kvp in savedBuildings)
            {
                Vector2Int size = kvp.Value.Item2;
                OccupyArea(kvp.Key, size);
            }
        }
    }
}