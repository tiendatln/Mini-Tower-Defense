using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;

partial struct TowerSpawnSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    // [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingleton<TargetMapComponent>(out var mapComponent))
        {
            Debug.Log("map component not found!");
        }

        EntityCommandBuffer ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);

        GridManager gridManager = mapComponent.MapRef.Value;

        if (Mouse.current == null)
        {
            Debug.Log("mouse not found");
            return;
        }


        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

        Vector3 mouseWorldPosition =
            Camera.main.ScreenToWorldPoint(mouseScreenPosition);

        mouseWorldPosition.z = 0;

        Debug.Log($"Screen: {mouseScreenPosition}");
        Debug.Log($"World: {mouseWorldPosition}");



        if (gridManager.CanPlaceHightTower(Vector3Int.FloorToInt(mouseWorldPosition)) == true)
        {
            foreach (var towerSpawn in SystemAPI.Query<RefRW<TowerSpawnComponent>>())
            {
                if (towerSpawn.ValueRO.MaxTowerEnable > towerSpawn.ValueRO.SpawnedTower)
                {
                    Entity tower = ecb.Instantiate(
                        towerSpawn.ValueRO.TowerPrefab
                    );

                    ecb.SetComponent(tower, LocalTransform.FromPosition(
                        gridManager.GetPlaceTowerPosition(mouseWorldPosition)
                    ));

                    Debug.Log($"Tower position: {gridManager.GetPlaceTowerPosition(mouseWorldPosition)}");

                    towerSpawn.ValueRW.SpawnedTower++;
                }
            }
        }
        ecb.Playback(state.EntityManager);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
