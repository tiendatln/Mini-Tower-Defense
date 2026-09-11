using Unity.Entities;
using UnityEngine;

class ArrowTowerAuthoring : MonoBehaviour
{
    public int Damage = 10;
    public int Health = 50;
    public float TimeShoot = 1.5f;
}

class ArrowTowerAuthoringBaker : Baker<ArrowTowerAuthoring>
{
    public override void Bake(ArrowTowerAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(entity, new ArrowTowerComponent
        {
            Damage = authoring.Damage,
            Health = authoring.Health,
            TimeShoot = authoring.TimeShoot
        });
    }
}
