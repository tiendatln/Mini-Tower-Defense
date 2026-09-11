using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

partial struct EnemySpawnSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    // [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;


        if (!SystemAPI.TryGetSingleton<TargetMapComponent>(out var mapComponent))
        {
            Debug.Log("map component not found!");
        }

        GridManager gridManager = mapComponent.MapRef.Value;

        List<Vector3> vector3s = gridManager.GetSpawnPoint();

        EntityCommandBuffer ecb =
            new EntityCommandBuffer(state.WorldUpdateAllocator);

        foreach (var spawner
                 in SystemAPI.Query<RefRW<EnemySpawnerComponent>>())
        {
            spawner.ValueRW.Timer += deltaTime;

            spawner.ValueRW.TimeWaitTurn_1 -= deltaTime;

            if (spawner.ValueRO.SpawnedEnemy >=
                spawner.ValueRO.MaxEnemy)
            {
                continue;
            }

            if (spawner.ValueRO.Timer < spawner.ValueRO.SpawnInterval)
            {
                continue;
            }

            spawner.ValueRW.Timer = 0f;

            Entity enemy = ecb.Instantiate(
                spawner.ValueRO.EnemyPrefab
            );
            ecb.SetComponent(
                            enemy,
                            LocalTransform.FromPosition(
                                vector3s[0]
                            )
                        );


            spawner.ValueRW.SpawnedEnemy++;
        }

        ecb.Playback(state.EntityManager);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
