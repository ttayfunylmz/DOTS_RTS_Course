using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class DroneHarvesterAuthoring : MonoBehaviour
{


    public GameObject sphereVisual1GameObject;
    public GameObject sphereVisual2GameObject;
    public GameObject sphereVisual3GameObject;
    public GameObject sphereVisual4GameObject;
    public GameObject sphereVisual5GameObject;
    public GameObject sphereVisual6GameObject;


    public class Baker : Baker<DroneHarvesterAuthoring>
    {


        public override void Bake(DroneHarvesterAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new DroneHarvester());
            AddComponent(entity, new DroneHarvesterVisualEntities
            {
                sphereVisual1Entity = GetEntity(authoring.sphereVisual1GameObject, TransformUsageFlags.Dynamic),
                sphereVisual2Entity = GetEntity(authoring.sphereVisual2GameObject, TransformUsageFlags.Dynamic),
                sphereVisual3Entity = GetEntity(authoring.sphereVisual3GameObject, TransformUsageFlags.Dynamic),
                sphereVisual4Entity = GetEntity(authoring.sphereVisual4GameObject, TransformUsageFlags.Dynamic),
                sphereVisual5Entity = GetEntity(authoring.sphereVisual5GameObject, TransformUsageFlags.Dynamic),
                sphereVisual6Entity = GetEntity(authoring.sphereVisual6GameObject, TransformUsageFlags.Dynamic),
            });
        }
    }
}



public struct DroneHarvester : IComponentData
{


    public enum State
    {
        GoingToNode,
        Harvesting,
        GoingBack,
    }


    public Entity parentEntity;
    public State state;
    public float3 spawnPosition;
    public float3 targetPosition;
    public float harvestTimer;
    public float harvestTimerMax;


}


public struct DroneHarvesterVisualEntities : IComponentData
{

    public Entity sphereVisual1Entity;
    public Entity sphereVisual2Entity;
    public Entity sphereVisual3Entity;
    public Entity sphereVisual4Entity;
    public Entity sphereVisual5Entity;
    public Entity sphereVisual6Entity;

}