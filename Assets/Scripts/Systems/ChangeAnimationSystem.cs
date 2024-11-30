using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;

[UpdateBefore(typeof(ActiveAnimationSystem))]
partial struct ChangeAnimationSystem : ISystem
{
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
            if(activeAnimation.ValueRO.activeAnimationType != activeAnimation.ValueRO.nextAnimationType)
            {
                activeAnimation.ValueRW.frame = 0;
                activeAnimation.ValueRW.frameTimer = 0f;
                activeAnimation.ValueRW.activeAnimationType = activeAnimation.ValueRO.nextAnimationType;

                ref AnimationData animationData
                    = ref animationDataHolder.animationDataBlobArrayBlobAssetReference.Value[(int)activeAnimation.ValueRW.activeAnimationType];

                materialMeshInfo.ValueRW.MeshID = animationData.batchMeshIdBlobArray[0];
            }
        }
    }
}
