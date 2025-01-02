using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BuildingConstructionAuthoring : MonoBehaviour
{


    public class Baker : Baker<BuildingConstructionAuthoring>
    {
        public override void Bake(BuildingConstructionAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BuildingConstruction());
        }
    }
}



public struct BuildingConstruction : IComponentData
{


    public float constructionTimer;
    public float constructionTimerMax;
    public float3 startPosition;
    public float3 endPosition;
    public BuildingTypeSO.BuildingType buildingType;
    public Entity finalPrefabEntity;
    public Entity visualEntity;


}

