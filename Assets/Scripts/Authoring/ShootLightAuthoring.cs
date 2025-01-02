using Unity.Entities;
using UnityEngine;

public class ShootLightAuthoring : MonoBehaviour
{


    public float timer;


    public class Baker : Baker<ShootLightAuthoring>
    {


        public override void Bake(ShootLightAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ShootLight
            {
                timer = authoring.timer,
            });
        }
    }


}



public struct ShootLight : IComponentData
{

    public float timer;

}