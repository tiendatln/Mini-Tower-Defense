using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

class TowerSpawnAuthoring : MonoBehaviour
{
    public GameObject TowerPrefab;
    public int MaxTowerEnable = 7;
    public int Coin = 0;
    public int MaxCoin = 10;
    public int CoinDefault = 2;
    public float CoinCooldown = 1f;
    public int SpawnedTower = 0;
}

class TowerSpawnAuthoringBaker : Baker<TowerSpawnAuthoring>
{
    public override void Bake(TowerSpawnAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.None);
        Entity entityPrefab = GetEntity(authoring.TowerPrefab, TransformUsageFlags.Dynamic);

        AddComponent(entity, new TowerSpawnComponent{
            TowerPrefab = entityPrefab,
            MaxTowerEnable = authoring.MaxTowerEnable,
            Coin = authoring.Coin,
            MaxCoin = authoring.MaxCoin,
            CoinDefault = authoring.CoinDefault,
            CoinCooldown = authoring.CoinCooldown,
            SpawnedTower = authoring.SpawnedTower
        });
    }
}
