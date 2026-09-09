using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    [Header("Tilemaps")]
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap highGroundTilemap;
    [SerializeField] private Tilemap blockedTilemap;

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

    public bool IsWalkable(Vector3Int cellPosition)
    {
        TileType tileType = GetTileType(cellPosition);

        return tileType == TileType.Ground;
    }

    public bool CanDeployMelee(Vector3Int cellPosition)
    {
        TileType tileType = GetTileType(cellPosition);

        return tileType == TileType.Ground;
    }

    public bool CanDeployRanged(Vector3Int cellPosition)
    {
        TileType tileType = GetTileType(cellPosition);

        return tileType == TileType.HighGround;
    }
}
