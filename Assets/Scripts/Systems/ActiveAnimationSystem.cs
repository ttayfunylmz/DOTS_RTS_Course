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
        
        ActiveAnimationJob activeAnimationJob = new ActiveAnimationJob
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            animationDataBlobArrayBlobAssetReference = animationDataHolder.animationDataBlobArrayBlobAssetReference
        };

        activeAnimationJob.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct ActiveAnimationJob : IJobEntity
{
    public float deltaTime;
    public BlobAssetReference<BlobArray<AnimationData>> animationDataBlobArrayBlobAssetReference;

    public void Execute(ref ActiveAnimation activeAnimation, ref MaterialMeshInfo materialMeshInfo)
    {
        ref AnimationData animationData
            = ref animationDataBlobArrayBlobAssetReference.Value[(int)activeAnimation.activeAnimationType];

        activeAnimation.frameTimer += deltaTime;
        if (activeAnimation.frameTimer > animationData.frameTimerMax)
        {
            activeAnimation.frameTimer -= animationData.frameTimerMax;
            activeAnimation.frame = (activeAnimation.frame + 1) % animationData.frameMax;

            materialMeshInfo.Mesh
                = animationData.intMeshIdBlobArray[activeAnimation.frame];

            if (activeAnimation.frame == 0 &&
                activeAnimation.activeAnimationType == AnimationDataSO.AnimationType.SoldierShoot)
            {
                activeAnimation.activeAnimationType = AnimationDataSO.AnimationType.None;
            }

            if (activeAnimation.frame == 0 &&
                activeAnimation.activeAnimationType == AnimationDataSO.AnimationType.ZombieAttack)
            {
                activeAnimation.activeAnimationType = AnimationDataSO.AnimationType.None;
            }
        }
    }
}