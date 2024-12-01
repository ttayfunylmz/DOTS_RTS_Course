using Unity.Entities;
using UnityEngine;

public class UnitAnimationsAuthoring : MonoBehaviour
{
    public AnimationDataSO.AnimationType idleAnimationType;
    public AnimationDataSO.AnimationType walkAnimationType;
    public AnimationDataSO.AnimationType shootAnimationType;
    public AnimationDataSO.AnimationType aimAnimationType;
    public AnimationDataSO.AnimationType meleeAttackAnimationType;

    public class Baker : Baker<UnitAnimationsAuthoring>
    {
        public override void Bake(UnitAnimationsAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new UnitAnimations
            {
                idleAnimationType = authoring.idleAnimationType,
                walkAnimationType = authoring.walkAnimationType,
                shootAnimationType = authoring.shootAnimationType,
                aimAnimationType = authoring.aimAnimationType,
                meleeAttackAnimationType = authoring.meleeAttackAnimationType
            });
        }
    }
}

public struct UnitAnimations : IComponentData
{
    public AnimationDataSO.AnimationType idleAnimationType;
    public AnimationDataSO.AnimationType walkAnimationType;
    public AnimationDataSO.AnimationType shootAnimationType;
    public AnimationDataSO.AnimationType aimAnimationType;
    public AnimationDataSO.AnimationType meleeAttackAnimationType;
}
