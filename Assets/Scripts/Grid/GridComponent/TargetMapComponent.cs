using Unity.Entities;

public struct TargetMapComponent : IComponentData
{
    public UnityObjectRef<GridManager> MapRef;
}
