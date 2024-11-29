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
            if(!activeAnimation.ValueRO.animationDataBlobAssetReference.IsCreated)
            {
                activeAnimation.ValueRW.animationDataBlobAssetReference = animationDataHolder.soldierIdle;
            }

            if(Input.GetKeyDown(KeyCode.T))
            {
                activeAnimation.ValueRW.animationDataBlobAssetReference = animationDataHolder.soldierIdle;
            }

            if(Input.GetKeyDown(KeyCode.Y))
            {
                activeAnimation.ValueRW.animationDataBlobAssetReference = animationDataHolder.soldierWalk;
            }

            activeAnimation.ValueRW.frameTimer += SystemAPI.Time.DeltaTime;
            if(activeAnimation.ValueRW.frameTimer > activeAnimation.ValueRO.animationDataBlobAssetReference.Value.frameTimerMax)
            {
                activeAnimation.ValueRW.frameTimer -= activeAnimation.ValueRO.animationDataBlobAssetReference.Value.frameTimerMax;
                activeAnimation.ValueRW.frame = (activeAnimation.ValueRW.frame + 1) % activeAnimation.ValueRO.animationDataBlobAssetReference.Value.frameMax;

                materialMeshInfo.ValueRW.MeshID
                    = activeAnimation.ValueRO.animationDataBlobAssetReference.Value.batchMeshIdBlobArray[activeAnimation.ValueRW.frame];
            }
        }
    }
}
