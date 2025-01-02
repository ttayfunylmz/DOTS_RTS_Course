using Unity.Entities;
using UnityEngine;

public class DroneHarvesterControllerAuthoring : MonoBehaviour
{


    public ResourceTypeSO.ResourceType resourceType;


    public class Baker : Baker<DroneHarvesterControllerAuthoring>
    {

        public override void Bake(DroneHarvesterControllerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new DroneHarvesterController
            {
                resourceType = authoring.resourceType,
            });
        }
    }

}



public struct DroneHarvesterController : IComponentData
{

    public float spawnTimer;
    public ResourceTypeSO.ResourceType resourceType;
    public Entity drone1Entity;
    public Entity drone2Entity;
    public Entity drone3Entity;
    public Entity drone4Entity;


}