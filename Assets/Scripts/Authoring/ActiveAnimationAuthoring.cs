using Unity.Entities;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

public class ActiveAnimationAuthoring : MonoBehaviour
{
    public AnimationDataSO.AnimationType nextAnimationType;

    public class Baker : Baker<ActiveAnimationAuthoring>
    {
        public override void Bake(ActiveAnimationAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ActiveAnimation
            {
                nextAnimationType = authoring.nextAnimationType,
            });
        }
    }
}

public struct ActiveAnimation : IComponentData
{
    public int frame;
    public float frameTimer;
    public AnimationDataSO.AnimationType activeAnimationType;
    public AnimationDataSO.AnimationType nextAnimationType;
}