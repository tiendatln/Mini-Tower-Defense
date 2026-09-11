using Unity.Entities;
using UnityEngine;

public struct MimiEnemyComponent : IComponentData
{
    public float MoveSpeed;
    public int Health;
    public int PathIndex;
}
