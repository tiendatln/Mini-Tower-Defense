using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct TowerSpawnComponent : IComponentData
{
    public Entity TowerPrefab;
    public int MaxTowerEnable;
    public int Coin;
    public int MaxCoin;
    public int CoinDefault;
    public float CoinCooldown;
    public int SpawnedTower;
}
