using Unity.Entities;
using UnityEngine;

public class ResourceTypeSOHolderAuthoring : MonoBehaviour
{


    public ResourceTypeSO.ResourceType resourceType;


    public class Baker : Baker<ResourceTypeSOHolderAuthoring>
    {

        public override void Bake(ResourceTypeSOHolderAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ResourceTypeSOHolder
            {
                resourceType = authoring.resourceType,
            });
        }

    }
}


public struct ResourceTypeSOHolder : IComponentData
{

    public ResourceTypeSO.ResourceType resourceType;

}