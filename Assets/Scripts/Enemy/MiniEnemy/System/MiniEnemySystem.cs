using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

partial struct MiniEnemySystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

    // [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;

        

        if(!SystemAPI.TryGetSingleton<TargetMapComponent>(out var mapComponent))
        {
            Debug.Log("map component not found!");
        }

        GridManager gridManager = mapComponent.MapRef.Value;

        foreach(var (transform, EnemyComponent) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<MimiEnemyComponent>>())
        {
            if (TileType.Ground == gridManager.GetTileType(Vector3Int.RoundToInt(transform.ValueRO.Position)))
            {
                transform.ValueRW.Position += transform.ValueRO.Right() *
                EnemyComponent.ValueRO.MoveSpeed *
                deltaTime;
            }
                
        } 
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
