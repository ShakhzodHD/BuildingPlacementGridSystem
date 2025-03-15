using UnityEngine;

public interface IGridService
{
    Vector3 SnapToGrid(Vector3 position);
    bool IsCellOccupied(Vector2Int gridPosition);
    void OccupyCell(Vector2Int gridPosition);
    void FreeCell(Vector2Int gridPosition);
    void ClearAllCells();
    float CellSize { get; }
    int GridSize { get; }
    bool IsPositionWithinBounds(Vector2Int gridPosition);
    bool IsAreaWithinBounds(Vector2Int startPosition, Vector2Int size);
    bool IsAreaAvailable(Vector2Int startPosition, Vector2Int size);
    void OccupyArea(Vector2Int startPosition, Vector2Int size);
    void FreeArea(Vector2Int startPosition, Vector2Int size);
}