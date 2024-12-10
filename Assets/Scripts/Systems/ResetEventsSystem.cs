using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(LateSimulationSystemGroup), OrderLast = true)]
partial struct ResetEventsSystem : ISystem
{
    private NativeArray<JobHandle> jobHandleNativeArray;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        jobHandleNativeArray = new NativeArray<JobHandle>(4, Allocator.Persistent);
    }

    public void OnUpdate(ref SystemState state)
    {
        if(SystemAPI.HasSingleton<BuildingHQ>())
        {
            Health hqHealth = SystemAPI.GetComponent<Health>(SystemAPI.GetSingletonEntity<BuildingHQ>());
            if(hqHealth.onDead)
            {
                DOTSEventsManager.Instance.TriggerOnHQDead();
            }
        }

        jobHandleNativeArray[0] = new ResetSelectedEventsJob().ScheduleParallel(state.Dependency);
        jobHandleNativeArray[1] = new ResetHealthEventsJob().ScheduleParallel(state.Dependency);
        jobHandleNativeArray[2] = new ResetShootAttackEventsJob().ScheduleParallel(state.Dependency);
        jobHandleNativeArray[3] = new ResetMeleeAttackEventsJob().ScheduleParallel(state.Dependency);

        NativeList<Entity> onBarracksUnitQueueChangedEntityList = new NativeList<Entity>(Allocator.TempJob);
        new ResetBuildingBarracksEventsJob()
        {
            onUnitQueueChangedEntityList = onBarracksUnitQueueChangedEntityList.AsParallelWriter(),
        }.ScheduleParallel(state.Dependency).Complete();

        DOTSEventsManager.Instance.TriggerOnBarracksUnitQueueChanged(onBarracksUnitQueueChangedEntityList);

        state.Dependency = JobHandle.CombineDependencies(jobHandleNativeArray);
    }
}

[BurstCompile]
public partial struct ResetShootAttackEventsJob : IJobEntity
{
    public void Execute(ref ShootAttack shootAttack)
    {
        shootAttack.onShoot.isTriggered = false;
    }
}

[BurstCompile]
public partial struct ResetHealthEventsJob : IJobEntity
{
    public void Execute(ref Health health)
    {
        health.onHealthChanged = false;
        health.onDead = false;
    }
}

[BurstCompile]
[WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
public partial struct ResetSelectedEventsJob : IJobEntity
{
    public void Execute(ref Selected selected)
    {
        selected.onSelected = false;
        selected.onDeselected = false;
    }
}

[BurstCompile]
public partial struct ResetMeleeAttackEventsJob : IJobEntity
{
    public void Execute(ref MeleeAttack meleeAttack)
    {
        meleeAttack.onAttacked = false;
    }
}

[BurstCompile]
public partial struct ResetBuildingBarracksEventsJob : IJobEntity
{
    public NativeList<Entity>.ParallelWriter onUnitQueueChangedEntityList;

    public void Execute(ref BuildingBarracks buildingBarracks, Entity entity)
    {
        if(buildingBarracks.onUnitQueueChanged)
        {
            onUnitQueueChangedEntityList.AddNoResize(entity);
        }

        buildingBarracks.onUnitQueueChanged = false;
    }
}