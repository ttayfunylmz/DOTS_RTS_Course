using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;

[WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
[UpdateInGroup(typeof(PostBakingSystemGroup))]
partial struct AnimationDataHolderBakingSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        AnimationDataListSO animationDataListSO = null;
        foreach (RefRO<AnimationDataHolderObjectData> animationDataHolderObjectData in SystemAPI.Query<RefRO<AnimationDataHolderObjectData>>())
        {
            animationDataListSO = animationDataHolderObjectData.ValueRO.animationDataListSO.Value;
        }

        Dictionary<AnimationDataSO.AnimationType, int[]> blobAssetDataDictionary = new Dictionary<AnimationDataSO.AnimationType, int[]>();

        foreach (AnimationDataSO.AnimationType animationType in System.Enum.GetValues(typeof(AnimationDataSO.AnimationType)))
        {
            AnimationDataSO animationDataSO = animationDataListSO.GetAnimationDataSO(animationType);
            blobAssetDataDictionary[animationType] = new int[animationDataSO.meshArray.Length];
        }

        foreach ((
            RefRO<AnimationDataHolderSubEntity> animationDataHolderSubEntity,
            RefRO<MaterialMeshInfo> materialMeshInfo)
                in SystemAPI.Query<
                    RefRO<AnimationDataHolderSubEntity>,
                    RefRO<MaterialMeshInfo>>())
        {
            blobAssetDataDictionary[animationDataHolderSubEntity.ValueRO.animationType]
                [animationDataHolderSubEntity.ValueRO.meshIndex]
                    = materialMeshInfo.ValueRO.Mesh;

            // Debug.Log(animationDataHolderSubEntity.ValueRO.animationType + " :: " +
            //     animationDataHolderSubEntity.ValueRO.meshIndex + " = " + materialMeshInfo.ValueRO.Mesh);
        }

        foreach (RefRW<AnimationDataHolder> animationDataHolder in SystemAPI.Query<RefRW<AnimationDataHolder>>())
        {
            BlobBuilder blobBuilder = new BlobBuilder(Allocator.Temp);
            ref BlobArray<AnimationData> animationDataBlobArray = ref blobBuilder.ConstructRoot<BlobArray<AnimationData>>();

            BlobBuilderArray<AnimationData> animationDataBlobBuilderArray
                = blobBuilder.Allocate<AnimationData>(ref animationDataBlobArray, System.Enum.GetValues(typeof(AnimationDataSO.AnimationType)).Length);

            int index = 0;
            foreach (AnimationDataSO.AnimationType animationType in System.Enum.GetValues(typeof(AnimationDataSO.AnimationType)))
            {
                AnimationDataSO animationDataSO = animationDataListSO.GetAnimationDataSO(animationType);

                BlobBuilderArray<int> blobBuilderArray
                    = blobBuilder.Allocate<int>(ref animationDataBlobBuilderArray[index].intMeshIdBlobArray, animationDataSO.meshArray.Length);

                animationDataBlobBuilderArray[index].frameTimerMax = animationDataSO.frameTimerMax;
                animationDataBlobBuilderArray[index].frameMax = animationDataSO.meshArray.Length;

                for (int i = 0; i < animationDataSO.meshArray.Length; ++i)
                {
                    blobBuilderArray[i] = blobAssetDataDictionary[animationType][i];
                }

                index++;
            }

            animationDataHolder.ValueRW.animationDataBlobArrayBlobAssetReference
                = blobBuilder.CreateBlobAssetReference<BlobArray<AnimationData>>(Allocator.Persistent);

            blobBuilder.Dispose();
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        foreach(RefRW<AnimationDataHolder> animationDataHolder in SystemAPI.Query<RefRW<AnimationDataHolder>>())
        {
            animationDataHolder.ValueRW.animationDataBlobArrayBlobAssetReference.Dispose();
        }
    }
}
