using Unity.Entities;
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
                isVisible = true,
                parentEntity = GetEntity(authoring.parentGameObject, TransformUsageFlags.Dynamic),
                sphereCastSize = authoring.sphereCastSize
            });
        }
    }
}

public struct VisualUnderFogOfWar : IComponentData
{
    public bool isVisible;
    public Entity parentEntity;
    public float sphereCastSize;
}