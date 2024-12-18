using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class TargetPositionPathQueuedAuthoring : MonoBehaviour
{
    public class Baker : Baker<TargetPositionPathQueuedAuthoring>
    {
        public override void Bake(TargetPositionPathQueuedAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new TargetPositionPathQueued());
            SetComponentEnabled<TargetPositionPathQueued>(entity, false);
        }
    }
}

public struct TargetPositionPathQueued : IComponentData, IEnableableComponent
{
    public float3 targetPosition;
}