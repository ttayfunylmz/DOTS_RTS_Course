using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class FlowFieldPathRequestAuthoring : MonoBehaviour
{



    public class Baker : Baker<FlowFieldPathRequestAuthoring>
    {


        public override void Bake(FlowFieldPathRequestAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new FlowFieldPathRequest());
            SetComponentEnabled<FlowFieldPathRequest>(entity, false);
        }
    }


}


public struct FlowFieldPathRequest : IComponentData, IEnableableComponent
{

    public float3 targetPosition;

}