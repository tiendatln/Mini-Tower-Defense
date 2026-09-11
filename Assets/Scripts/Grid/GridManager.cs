using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] private int width = 12;
    [SerializeField] private int height = 8;

    [Header("Tilemaps")]
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap highGroundTilemap;
    [SerializeField] private Tilemap blockedTilemap;

    [SerializeField] private Tilemap spawnTilemap;

    [SerializeField] private Tilemap goalTilemap;

    public int Width => width;
    public int Height => height;

    /// <summary>
    /// Xác định loại tile tại vị trí cell được cung cấp.
    /// </summary>
    public TileType GetTileType(Vector3Int cellPosition)
    {
        if (blockedTilemap.HasTile(cellPosition))
        {
            return TileType.Blocked;
        }

        if (highGroundTilemap.HasTile(cellPosition))
        {
            return TileType.HighGround;
        }

        if (groundTilemap.HasTile(cellPosition))
        {
            return TileType.Ground;
        }

        if (spawnTilemap.HasTile(cellPosition))
        {
            return TileType.Spawn;
        }

        if (goalTilemap.HasTile(cellPosition))
        {
            return TileType.Goal;
        }

        return TileType.None;
    }

    /// <summary>
    /// Tìm đường đi bằng SPFA từ vị trí world bắt đầu đến vị trí world đích.
    /// </summary>
    public List<Vector3> FindPath(Vector3 startWorldPosition, Vector3 targetWorldPosition)
    {
        Vector3Int startCell = groundTilemap.WorldToCell(startWorldPosition);
        Vector3Int targetCell = groundTilemap.WorldToCell(targetWorldPosition);
        List<Vector3Int> cellPath = FindPath(startCell, targetCell);
        List<Vector3> worldPath = new List<Vector3>(cellPath.Count);

        foreach (Vector3Int cell in cellPath)
        {
            worldPath.Add(groundTilemap.GetCellCenterWorld(cell));
        }

        return worldPath;
    }

    /// <summary>
    /// Tìm đường đi ngắn nhất bằng SPFA giữa hai cell, chỉ di chuyển theo bốn hướng.
    /// </summary>
    public List<Vector3Int> FindPath(Vector3Int startCell, Vector3Int targetCell)
    {
        if (!IsWalkable(startCell) || !IsWalkable(targetCell))
        {
            return new List<Vector3Int>();
        }

        Dictionary<Vector3Int, int> distances = new Dictionary<Vector3Int, int>
        {
            [startCell] = 0
        };
        Dictionary<Vector3Int, Vector3Int> previousPositions = new Dictionary<Vector3Int, Vector3Int>();
        HashSet<Vector3Int> positionsInQueue = new HashSet<Vector3Int>();
        Queue<Vector3Int> positionsToVisit = new Queue<Vector3Int>();
        positionsToVisit.Enqueue(startCell);
        positionsInQueue.Add(startCell);

        while (positionsToVisit.Count > 0)
        {
            Vector3Int currentPosition = positionsToVisit.Dequeue();
            positionsInQueue.Remove(currentPosition);

            if (currentPosition == targetCell)
            {
                return BuildPath(startCell, targetCell, previousPositions);
            }

            foreach (Vector3Int neighbourPosition in GetNeighbours(currentPosition))
            {
                if (!IsWalkable(neighbourPosition))
                {
                    continue;
                }

                int newDistance = distances[currentPosition] + 1;
                if (!distances.TryGetValue(neighbourPosition, out int currentDistance) ||
                    newDistance < currentDistance)
                {
                    distances[neighbourPosition] = newDistance;
                    previousPositions[neighbourPosition] = currentPosition;

                    if (positionsInQueue.Add(neighbourPosition))
                    {
                        positionsToVisit.Enqueue(neighbourPosition);
                    }
                }
            }
        }

        return new List<Vector3Int>();
    }

    /// <summary>
    /// Kiểm tra cell có thể đi qua hay không.
    /// </summary>
    private bool IsWalkable(Vector3Int cellPosition)
    {
        TileType tileType = GetTileType(cellPosition);
        return tileType == TileType.Ground ||
               tileType == TileType.Spawn ||
               tileType == TileType.Goal;
    }

    /// <summary>
    /// Kiểm tra cell có thể đặt hight tower hay không.
    /// </summary>
    public bool CanPlaceHightTower(Vector3Int cellPosition)
    {
        TileType tileType = GetTileType(cellPosition);
        return tileType == TileType.HighGround;
    }

    /// <summary>
    /// Kiểm tra cell có thể đặt ground tower hay không.
    /// </summary>
    public bool CanPlaceGroundTower(Vector3Int cellPosition)
    {
        TileType tileType = GetTileType(cellPosition);
        return tileType == TileType.Ground;
    }

    public Vector3 GetPlaceTowerPosition(Vector3 mousePosition)
    {
        if (highGroundTilemap.HasTile(Vector3Int.FloorToInt(mousePosition)))
        {
            // Chuyển Cell Position -> World Position
            Vector3 worldPos = highGroundTilemap.GetCellCenterWorld(Vector3Int.FloorToInt(mousePosition));

            return worldPos;

        }

        return mousePosition;
    }

    /// <summary>
    /// Dựng lại đường đi SPFA từ đích về nguồn bằng bảng vị trí trước đó.
    /// </summary>
    private static List<Vector3Int> BuildPath(
        Vector3Int startCell,
        Vector3Int targetCell,
        Dictionary<Vector3Int, Vector3Int> previousPositions)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        Vector3Int currentPosition = targetCell;

        while (currentPosition != startCell)
        {
            path.Add(currentPosition);
            currentPosition = previousPositions[currentPosition];
        }

        path.Add(startCell);
        path.Reverse();
        return path;
    }

    /// <summary>
    /// Trả về bốn cell láng giềng theo hướng lên, phải, xuống và trái.
    /// </summary>
    private static IEnumerable<Vector3Int> GetNeighbours(Vector3Int cellPosition)
    {
        yield return cellPosition + Vector3Int.up;
        yield return cellPosition + Vector3Int.right;
        yield return cellPosition + Vector3Int.down;
        yield return cellPosition + Vector3Int.left;
    }

    /// <summary>
    /// Lấy tất cả vị trí world có tile spawn trên spawn tilemap.
    /// </summary>
    public List<Vector3> GetSpawnPoint()
    {
        List<Vector3> spawnPoints = new List<Vector3>();

        foreach (var cellPos in spawnTilemap.cellBounds.allPositionsWithin)
        {
            if (spawnTilemap.HasTile(cellPos))
            {
                // Chuyển Cell Position -> World Position
                Vector3 worldPos = spawnTilemap.GetCellCenterWorld(cellPos);

                spawnPoints.Add(worldPos);

            }
        }

        return spawnPoints;
    }

    /// <summary>
    /// Lấy tất cả vị trí world có tile goal trên goal tilemap.
    /// </summary>
    public List<Vector3> GetGoalPoint()
    {
        List<Vector3> goalPoints = new List<Vector3>();

        foreach (var cellPos in goalTilemap.cellBounds.allPositionsWithin)
        {
            if (goalTilemap.HasTile(cellPos))
            {
                // Chuyển Cell Position -> World Position
                Vector3 worldPos = goalTilemap.GetCellCenterWorld(cellPos);

                goalPoints.Add(worldPos);
            }
        }

        return goalPoints;
    }

    /// <summary>
    /// Đăng ký GridManager vào ECS để các system khác có thể truy cập bản đồ.
    /// </summary>
    private void Start()
    {
        var em = World.DefaultGameObjectInjectionWorld.EntityManager;

        Entity bridgeEntity = em.CreateEntity();

        em.AddComponentData(bridgeEntity, new TargetMapComponent
        {
            MapRef = this
        });

    }
}
