using Unity.Entities;
using UnityEngine;

public class BuildingTypeSOHolderAuthoring : MonoBehaviour
{


    public BuildingTypeSO.BuildingType buildingType;


    public class Baker : Baker<BuildingTypeSOHolderAuthoring>
    {


        public override void Bake(BuildingTypeSOHolderAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BuildingTypeSOHolder
            {
                buildingType = authoring.buildingType,
            });
        }
    }

}



public struct BuildingTypeSOHolder : IComponentData
{


    public BuildingTypeSO.BuildingType buildingType;


}