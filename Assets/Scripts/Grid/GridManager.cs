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

    public int Width => width;
    public int Height => height;

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

        return TileType.None;
    }

    private void Start() {
        var em = World.DefaultGameObjectInjectionWorld.EntityManager;

        Entity bridgeEntity = em.CreateEntity();

        em.AddComponentData(bridgeEntity, new TargetMapComponent
        {
            MapRef = this
        });
        
    }
}
