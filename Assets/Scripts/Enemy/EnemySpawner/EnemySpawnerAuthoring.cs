using Unity.Entities;
using UnityEngine;

class EnemySpawnerAuthoring : MonoBehaviour
{
    public GameObject EnemyPrefab;
    public float SpawnInterval = 1f;
    public int MaxEnemy = 20;
    public float TimeWaitTurn_1 = 2f;
}

class EnemySpawnerAuthoringBaker : Baker<EnemySpawnerAuthoring>
{
    public override void Bake(EnemySpawnerAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.None);

            Entity enemyPrefab =
                GetEntity(authoring.EnemyPrefab, TransformUsageFlags.Dynamic);

            AddComponent(entity, new EnemySpawnerComponent
            {
                EnemyPrefab = enemyPrefab,
                SpawnInterval = authoring.SpawnInterval,
                Timer = 0f,
                MaxEnemy = authoring.MaxEnemy,
                SpawnedEnemy = 0,
                TimeWaitTurn_1 = authoring.TimeWaitTurn_1
            });
    }
}
