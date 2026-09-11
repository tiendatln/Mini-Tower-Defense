using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class TowerSpawnRequest : IComponentData
{
    public float3 WorldPosition;
}
