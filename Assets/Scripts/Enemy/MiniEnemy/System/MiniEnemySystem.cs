using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
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



        if (!SystemAPI.TryGetSingleton<TargetMapComponent>(out var mapComponent))
        {
            Debug.Log("map component not found!");
        }

        GridManager gridManager = mapComponent.MapRef.Value;


        List<Vector3> APathList = gridManager.FindPath(gridManager.GetSpawnPoint()[0], gridManager.GetGoalPoint()[0]);

        foreach (var (transform, EnemyComponent) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<MimiEnemyComponent>>())
        {

            int index = EnemyComponent.ValueRO.PathIndex;

            // Đã đi hết path
            if (index >= APathList.Count)
                continue;

            float3 targetPosition = APathList[index];

            float3 currentPosition =
                transform.ValueRO.Position;

            float3 direction =
                math.normalizesafe(targetPosition - currentPosition);

            transform.ValueRW.Position +=
                direction *
                EnemyComponent.ValueRO.MoveSpeed *
                deltaTime;

            // Đã tới waypoint
            if (math.distance(
                    currentPosition,
                    targetPosition) < 0.05f)
            {
                transform.ValueRW.Position = targetPosition;

                EnemyComponent.ValueRW.PathIndex++;
            }


        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
