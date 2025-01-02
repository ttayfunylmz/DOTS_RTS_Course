using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct ShakeObjectSystem : ISystem
{

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((
            RefRW<ShakeObject> shakeObject,
            EnabledRefRW<ShakeObject> shakeObjectEnabled)
            in SystemAPI.Query<
                RefRW<ShakeObject>,
                EnabledRefRW<ShakeObject>>())
        {

            shakeObject.ValueRW.timer -= SystemAPI.Time.DeltaTime;

            RefRW<LocalTransform> shakeLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(shakeObject.ValueRO.shakeEntity);

            Unity.Mathematics.Random random = shakeObject.ValueRO.random;

            shakeLocalTransform.ValueRW.Position = random.NextFloat3Direction() * shakeObject.ValueRO.intensity;

            shakeObject.ValueRW.random = random;

            if (shakeObject.ValueRO.timer <= 0f)
            {
                shakeLocalTransform.ValueRW.Position = float3.zero;
                shakeObjectEnabled.ValueRW = false;
            }
        }
    }
}