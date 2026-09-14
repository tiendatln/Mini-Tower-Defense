using System.Collections.Generic;
using System.Linq;
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

    [Header("Path Graph")]
    [SerializeField] public List<PathGraphAsset> pathGraphAsset;

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
    /// Tìm đường đi ngắn nhất bằng SPFA giữa hai vị trí node.
    /// </summary>
    public List<Vector3> FindPath(List<PathNode> pathNodeDefinitions, Vector3 startPosition, Vector3 targetPosition)
    {
        List<Vector3> path = new List<Vector3>();
        if (pathNodeDefinitions == null || pathNodeDefinitions.Count == 0)
        {
            return path;
        }

        Dictionary<int, PathNode> nodesById = new Dictionary<int, PathNode>();
        foreach (PathNode node in pathNodeDefinitions)
        {
            if (node == null || !nodesById.TryAdd(node.Id, node))
            {
                return path;
            }
        }

        foreach (PathNode node in pathNodeDefinitions)
        {
            node.Neighbors.Clear();
            if (node.NeighborIds == null)
            {
                continue;
            }

            foreach (int neighborId in node.NeighborIds)
            {
                if (nodesById.TryGetValue(neighborId, out PathNode neighbor))
                {
                    node.Neighbors.Add(neighbor);
                }
            }
        }

        PathNode startNode = pathNodeDefinitions.FirstOrDefault(
            node => node.CellPosition == startPosition);
        PathNode targetNode = pathNodeDefinitions.FirstOrDefault(
            node => node.CellPosition == targetPosition);
        if (startNode == null || targetNode == null)
        {
            return path;
        }

        Queue<PathNode> queue = new Queue<PathNode>();
        Dictionary<int, float> distances = new Dictionary<int, float>();
        Dictionary<int, int> trace = new Dictionary<int, int>();
        Dictionary<int, bool> inQueue = new Dictionary<int, bool>();
        Dictionary<int, int> enqueueCount = new Dictionary<int, int>();

        foreach (PathNode node in pathNodeDefinitions)
        {
            distances[node.Id] = float.PositiveInfinity;
            trace[node.Id] = -1;
            inQueue[node.Id] = false;
            enqueueCount[node.Id] = 0;
        }

        distances[startNode.Id] = 0f;
        queue.Enqueue(startNode);
        inQueue[startNode.Id] = true;
        enqueueCount[startNode.Id] = 1;

        while (queue.Count > 0)
        {
            PathNode currentNode = queue.Dequeue();
            inQueue[currentNode.Id] = false;

            if (currentNode.Neighbors == null)
            {
                continue;
            }

            foreach (PathNode neighbor in currentNode.Neighbors)
            {
                if (neighbor == null || !distances.ContainsKey(neighbor.Id))
                {
                    continue;
                }

                float newDistance = distances[currentNode.Id] + Vector3.Distance(
                    currentNode.CellPosition,
                    neighbor.CellPosition);

                if (newDistance >= distances[neighbor.Id])
                {
                    continue;
                }

                distances[neighbor.Id] = newDistance;
                trace[neighbor.Id] = currentNode.Id;

                if (!inQueue[neighbor.Id])
                {
                    queue.Enqueue(neighbor);
                    inQueue[neighbor.Id] = true;
                    enqueueCount[neighbor.Id]++;

                    if (enqueueCount[neighbor.Id] > pathNodeDefinitions.Count)
                    {
                        Debug.LogError("Phát hiện chu trình âm! Không thể tìm đường đi ngắn nhất.");
                        return new List<Vector3>();
                    }
                }
            }
        }

        if (float.IsPositiveInfinity(distances[targetNode.Id]))
        {
            return path;
        }

        int currentId = targetNode.Id;
        while (currentId != -1)
        {
            path.Add(nodesById[currentId].CellPosition);
            if (currentId == startNode.Id)
            {
                path.Reverse();
                Debug.Log($"SPFA path ({path.Count} nodes): {string.Join(" -> ", path)}");
                return path;
            }

            currentId = trace[currentId];
        }

        return path;
    }

    private void buildGraph(Dictionary<Vector3, PathNode> nodesByPosition, List<PathNode> pathNodes)
    {
        foreach (var node in pathNodes)
        {
            nodesByPosition.Add(node.CellPosition, node);
        }
    }

    private PathNode FindClosestNode(Vector3 position)
    {

        PathNode closestNode = null;


        return closestNode;
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
    /// Lấy vị trí node đích sau khi SPFA tìm thấy đường đi.
    /// </summary>
    private static Vector3 BuildPath(PathNode targetNode)
    {
        return targetNode.CellPosition;
    }


    /// <summary>
    /// Lấy tất cả vị trí world có tile spawn trên spawn tilemap.
    /// </summary>
    public List<Vector3> GetFirstGraphPoint()
    {
        List<Vector3> spawnPoints = new List<Vector3>();

        foreach (var point in pathGraphAsset)
        {
            spawnPoints.Add(point.Nodes.First().CellPosition);
        }

        return spawnPoints;
    }

    /// <summary>
    /// Lấy tất cả vị trí world có tile goal trên goal tilemap.
    /// </summary>
    public List<Vector3> GetLastGraphPoint()
    {
        List<Vector3> goalPoints = new List<Vector3>();

        foreach (var point in pathGraphAsset)
        {
            goalPoints.Add(point.Nodes.Last().CellPosition);
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
