using Unity.Entities;
using UnityEngine;

public class BuildingHQAuthoring : MonoBehaviour
{


    public class Baker : Baker<BuildingHQAuthoring>
    {

        public override void Bake(BuildingHQAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BuildingHQ());
        }
    }
}



public struct BuildingHQ : IComponentData
{

}