using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct EnemySpawnSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        EntityCommandBuffer ecb =
            new EntityCommandBuffer(state.WorldUpdateAllocator);

        foreach (var spawner
                 in SystemAPI.Query<RefRW<EnemySpawnerComponent>>())
        {
            spawner.ValueRW.Timer += deltaTime;

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
                    new float3(-8, 0.5f, 0)
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
