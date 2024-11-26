using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct LoseTargetSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((
            RefRO<LocalTransform> localTransform,
            RefRW<Target> target,
            RefRO<LoseTarget> loseTarget,
            RefRO<TargetOverride> targetOverride)
                in SystemAPI.Query<
                    RefRO<LocalTransform>,
                    RefRW<Target>,
                    RefRO<LoseTarget>,
                    RefRO<TargetOverride>>())
        {
            if(target.ValueRO.targetEntity == Entity.Null) { continue; }
            if(targetOverride.ValueRO.targetEntity != Entity.Null) { continue; }

            LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
            float targetDistance = math.distance(localTransform.ValueRO.Position, targetLocalTransform.Position);

            if(targetDistance > loseTarget.ValueRO.loseTargetDistance)
            {
                // TARGET IS TOO FAR, RESET IT
                target.ValueRW.targetEntity = Entity.Null;
            }
        }
    }
}
