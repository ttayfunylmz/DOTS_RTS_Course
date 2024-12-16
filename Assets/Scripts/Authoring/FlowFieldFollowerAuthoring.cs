using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class FlowFieldFollowerAuthoring : MonoBehaviour
{
    public class Baker : Baker<FlowFieldFollowerAuthoring>
    {
        public override void Bake(FlowFieldFollowerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new FlowFieldFollower());
            SetComponentEnabled<FlowFieldFollower>(entity, false);
        }
    }
}

public struct FlowFieldFollower : IComponentData, IEnableableComponent
{
    public float3 targetPosition;
    public float3 lastMovedVector;
}