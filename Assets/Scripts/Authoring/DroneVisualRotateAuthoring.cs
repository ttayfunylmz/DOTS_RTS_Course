using Unity.Entities;
using UnityEngine;

public class DroneVisualRotateAuthoring : MonoBehaviour
{


    public float rotateSpeed;


    public class Baker : Baker<DroneVisualRotateAuthoring>
    {


        public override void Bake(DroneVisualRotateAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new DroneVisualRotate
            {
                rotateSpeed = authoring.rotateSpeed,
            });
        }
    }
}


public struct DroneVisualRotate : IComponentData
{

    public float rotateSpeed;

}