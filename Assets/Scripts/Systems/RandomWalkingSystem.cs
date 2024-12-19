using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct RandomWalkingSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((
            RefRW<RandomWalking> randomWalking,
            RefRW<TargetPositionPathQueued> targetPositionPathQueued,
            EnabledRefRW<TargetPositionPathQueued> targetPositionPathQueuedEnabled,
            RefRO<LocalTransform> localTransform)
                in SystemAPI.Query<
                    RefRW<RandomWalking>,
                    RefRW<TargetPositionPathQueued>,
                    EnabledRefRW<TargetPositionPathQueued>,
                    RefRO<LocalTransform>>().WithPresent<TargetPositionPathQueued>())
        {
            if(math.distancesq(localTransform.ValueRO.Position, randomWalking.ValueRO.targetPosition) 
                < UnitMoverSystem.REACHED_TARGET_POSITION_DISTANCE_SQ)
            {
                // REACHED THE TARGET DISTANCE
                Random random = randomWalking.ValueRO.random;

                float3 randomDirection = new float3(random.NextFloat(-1f, +1f), 0, random.NextFloat(-1f, +1f));
                randomDirection = math.normalize(randomDirection);

                randomWalking.ValueRW.targetPosition =
                    randomWalking.ValueRO.originPosition +
                    randomDirection * random.NextFloat(randomWalking.ValueRO.distanceMin, randomWalking.ValueRO.distanceMax);

                randomWalking.ValueRW.random = random;
            }
            else
            {
                // TOO FAR, MOVE CLOSER
                targetPositionPathQueued.ValueRW.targetPosition = randomWalking.ValueRO.targetPosition;
                targetPositionPathQueuedEnabled.ValueRW = true;
            }
        }
    }
}
