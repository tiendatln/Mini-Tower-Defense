using Unity.Entities;

public struct EnemySpawnerComponent : IComponentData
{
    public Entity EnemyPrefab;
    public float SpawnInterval;
    public float Timer;
    public int MaxEnemy;
    public int SpawnedEnemy;
    public float TimeWaitTurn_1;
}
