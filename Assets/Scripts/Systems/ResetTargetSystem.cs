using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
partial struct ResetTargetSystem : ISystem
{
    private ComponentLookup<LocalTransform> localTransformComponentLookup;
    private EntityStorageInfoLookup entityStorageInfoLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        localTransformComponentLookup = state.GetComponentLookup<LocalTransform>(true);
        entityStorageInfoLookup = state.GetEntityStorageInfoLookup();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        localTransformComponentLookup.Update(ref state);
        entityStorageInfoLookup.Update(ref state);

        ResetTargetJob resetTargetJob = new ResetTargetJob
        {
            localTransformComponentLookup = localTransformComponentLookup,
            entityStorageInfoLookup = entityStorageInfoLookup
        };

        resetTargetJob.ScheduleParallel();

        ResetTargetOverrideJob resetTargetOverrideJob = new ResetTargetOverrideJob
        {
            localTransformComponentLookup = localTransformComponentLookup,
            entityStorageInfoLookup = entityStorageInfoLookup
        };

        resetTargetOverrideJob.ScheduleParallel();

        // foreach(RefRW<Target> target in SystemAPI.Query<RefRW<Target>>())
        // {
        //     if(target.ValueRW.targetEntity != null)
        //     {
        //         if(!SystemAPI.Exists(target.ValueRO.targetEntity) ||
        //             !SystemAPI.HasComponent<LocalTransform>(target.ValueRO.targetEntity))
        //         {
        //             target.ValueRW.targetEntity = Entity.Null;
        //         }
        //     }
        // }

        // foreach(RefRW<TargetOverride> targetOverride in SystemAPI.Query<RefRW<TargetOverride>>())
        // {
        //     if(targetOverride.ValueRW.targetEntity != null)
        //     {
        //         if(!SystemAPI.Exists(targetOverride.ValueRO.targetEntity) ||
        //             !SystemAPI.HasComponent<LocalTransform>(targetOverride.ValueRO.targetEntity))
        //         {
        //             targetOverride.ValueRW.targetEntity = Entity.Null;
        //         }
        //     }
        // }
    }
}

[BurstCompile]
public partial struct ResetTargetJob : IJobEntity
{
    [ReadOnly] public ComponentLookup<LocalTransform> localTransformComponentLookup;
    [ReadOnly] public EntityStorageInfoLookup entityStorageInfoLookup;

    public void Execute(ref Target target)
    {
        if(target.targetEntity != null)
        {
            if(!entityStorageInfoLookup.Exists(target.targetEntity) ||
                !localTransformComponentLookup.HasComponent(target.targetEntity))
            {
                target.targetEntity = Entity.Null;
            }
        }
    }
}

[BurstCompile]
public partial struct ResetTargetOverrideJob : IJobEntity
{
    [ReadOnly] public ComponentLookup<LocalTransform> localTransformComponentLookup;
    [ReadOnly] public EntityStorageInfoLookup entityStorageInfoLookup;

    public void Execute(ref TargetOverride targetOverride)
    {
        if(targetOverride.targetEntity != null)
        {
            if(!entityStorageInfoLookup.Exists(targetOverride.targetEntity) ||
                !localTransformComponentLookup.HasComponent(targetOverride.targetEntity))
            {
                targetOverride.targetEntity = Entity.Null;
            }
        }
    }
}