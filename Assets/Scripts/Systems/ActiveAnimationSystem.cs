using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

partial struct ActiveAnimationSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<AnimationDataHolder>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        AnimationDataHolder animationDataHolder = SystemAPI.GetSingleton<AnimationDataHolder>();

        foreach ((
            RefRW<ActiveAnimation> activeAnimation,
            RefRW<MaterialMeshInfo> materialMeshInfo)
                in SystemAPI.Query<
                    RefRW<ActiveAnimation>,
                    RefRW<MaterialMeshInfo>>())
        {
            ref AnimationData animationData
                = ref animationDataHolder.animationDataBlobArrayBlobAssetReference.Value[(int)activeAnimation.ValueRW.activeAnimationType];

            activeAnimation.ValueRW.frameTimer += SystemAPI.Time.DeltaTime;
            if(activeAnimation.ValueRW.frameTimer > animationData.frameTimerMax)
            {
                activeAnimation.ValueRW.frameTimer -= animationData.frameTimerMax;
                activeAnimation.ValueRW.frame = (activeAnimation.ValueRW.frame + 1) % animationData.frameMax;

                materialMeshInfo.ValueRW.MeshID
                    = animationData.batchMeshIdBlobArray[activeAnimation.ValueRW.frame];

                if(activeAnimation.ValueRO.frame == 0 &&
                    activeAnimation.ValueRO.activeAnimationType == AnimationDataSO.AnimationType.SoldierShoot)
                {
                    activeAnimation.ValueRW.activeAnimationType = AnimationDataSO.AnimationType.None;
                }

                if(activeAnimation.ValueRO.frame == 0 &&
                    activeAnimation.ValueRO.activeAnimationType == AnimationDataSO.AnimationType.ZombieAttack)
                {
                    activeAnimation.ValueRW.activeAnimationType = AnimationDataSO.AnimationType.None;
                }
            }
        }
    }
}
