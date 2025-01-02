using Unity.Entities;
using UnityEngine;

public class BuildingHarvesterAuthoring : MonoBehaviour
{



    public float harvestTimerMax;
    public ResourceTypeSO.ResourceType resourceType;


    public class Baker : Baker<BuildingHarvesterAuthoring>
    {

        public override void Bake(BuildingHarvesterAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BuildingHarvester
            {
                harvestTimerMax = authoring.harvestTimerMax,
                resourceType = authoring.resourceType,
            });
        }
    }
}



public struct BuildingHarvester : IComponentData
{

    public float harvestTimer;
    public float harvestTimerMax;
    public ResourceTypeSO.ResourceType resourceType;

}