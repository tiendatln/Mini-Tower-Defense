using Unity.Entities;
using UnityEngine;

class MiniEnemyAuthoring : MonoBehaviour
{
    public float MoveSpeed = 2f;
    public int Health;

}

class MiniEnemyAuthoringBaker : Baker<MiniEnemyAuthoring>
{
    public override void Bake(MiniEnemyAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(entity, new MimiEnemyComponent
        {
            MoveSpeed = authoring.MoveSpeed,
            Health = authoring.Health
        });

    }
}
