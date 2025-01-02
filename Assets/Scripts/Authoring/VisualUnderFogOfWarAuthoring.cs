using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class VisualUnderFogOfWarAuthoring : MonoBehaviour
{


    public GameObject parentGameObject;
    public float sphereCastSize;


    public class Baker : Baker<VisualUnderFogOfWarAuthoring>
    {

        public override void Bake(VisualUnderFogOfWarAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new VisualUnderFogOfWar
            {
                isVisible = false,
                parentEntity = GetEntity(authoring.parentGameObject, TransformUsageFlags.Dynamic),
                sphereCastSize = authoring.sphereCastSize,
                timer = 0f,
                timerMax = .2f,
            });
            AddComponent(entity, new DisableRendering());
        }
    }
}



public struct VisualUnderFogOfWar : IComponentData
{

    public bool isVisible;
    public Entity parentEntity;
    public float sphereCastSize;
    public float timer;
    public float timerMax;

}