using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ShakeObjectAuthoring : MonoBehaviour
{


    public GameObject shakeGameObject;
    public bool startEnabledState = false;
    public float startTimer = 0f;
    public float startIntensity = .5f;


    public class Baker : Baker<ShakeObjectAuthoring>
    {


        public override void Bake(ShakeObjectAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ShakeObject
            {
                intensity = authoring.startIntensity,
                timer = authoring.startTimer,
                shakeEntity = GetEntity(authoring.shakeGameObject, TransformUsageFlags.Dynamic),
                random = new Unity.Mathematics.Random((uint)entity.Index),
            });

            SetComponentEnabled<ShakeObject>(entity, authoring.startEnabledState);
        }


    }


}

public struct ShakeObject : IComponentData, IEnableableComponent
{

    public float intensity;
    public float timer;
    public Entity shakeEntity;
    public Unity.Mathematics.Random random;

}