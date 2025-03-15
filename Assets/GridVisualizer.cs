using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private int gridSize = 10;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        for (int x = -gridSize; x <= gridSize; x++)
            for (int z = -gridSize; z <= gridSize; z++)
            {
                Vector3 center = new(x * cellSize, 0, z * cellSize);
                Gizmos.DrawWireCube(center, new Vector3(cellSize, 0.01f, cellSize));
            }
    }
}