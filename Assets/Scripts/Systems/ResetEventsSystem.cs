using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(LateSimulationSystemGroup), OrderLast = true)]
partial struct ResetEventsSystem : ISystem
{


    private NativeArray<JobHandle> jobHandleNativeArray;
    private NativeList<Entity> onBarracksUnitQueueChangedEntityList;
    private NativeList<Entity> onHealthDeadEntityList;
    private NativeList<Entity> onHordeStartedSpawningEntityList;
    private NativeList<Entity> onHordeStartSpawningSoonEntityList;


    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        jobHandleNativeArray = new NativeArray<JobHandle>(3, Allocator.Persistent);
        onBarracksUnitQueueChangedEntityList = new NativeList<Entity>(Allocator.Persistent);
        onHealthDeadEntityList = new NativeList<Entity>(64, Allocator.Persistent);
        onHordeStartedSpawningEntityList = new NativeList<Entity>(Allocator.Persistent);
        onHordeStartSpawningSoonEntityList = new NativeList<Entity>(Allocator.Persistent);
    }

    public void OnUpdate(ref SystemState state)
    {
        if (SystemAPI.HasSingleton<BuildingHQ>())
        {
            Health hqHealth = SystemAPI.GetComponent<Health>(SystemAPI.GetSingletonEntity<BuildingHQ>());
            if (hqHealth.onDead)
            {
                DOTSEventsManager.Instance.TriggerOnHQDead();
            }
        }


        jobHandleNativeArray[0] = new ResetSelectedEventsJob().ScheduleParallel(state.Dependency);
        jobHandleNativeArray[1] = new ResetShootAttackEventsJob().ScheduleParallel(state.Dependency);
        jobHandleNativeArray[2] = new ResetMeleeAttackEventsJob().ScheduleParallel(state.Dependency);

        onHordeStartedSpawningEntityList.Clear();
        onHordeStartSpawningSoonEntityList.Clear();
        new ResetHordeEventsJob()
        {
            onHordeStartedSpawningEntityList = onHordeStartedSpawningEntityList.AsParallelWriter(),
            onHordeStartSpawningSoonEntityList = onHordeStartSpawningSoonEntityList.AsParallelWriter(),
        }.ScheduleParallel(state.Dependency).Complete();

        DOTSEventsManager.Instance?.TriggerOnHordeStartedSpawning(onHordeStartedSpawningEntityList);
        DOTSEventsManager.Instance?.TriggerOnHordeStartSpawningSoon(onHordeStartSpawningSoonEntityList);



        onHealthDeadEntityList.Clear();
        new ResetHealthEventsJob()
        {
            onHealthDeadEntityList = onHealthDeadEntityList.AsParallelWriter(),
        }.ScheduleParallel(state.Dependency).Complete();

        DOTSEventsManager.Instance?.TriggerOnHealthDead(onHealthDeadEntityList);



        onBarracksUnitQueueChangedEntityList.Clear();
        new ResetBuildingBarracksEventsJob()
        {
            onUnitQueueChangedEntityList = onBarracksUnitQueueChangedEntityList.AsParallelWriter(),
        }.ScheduleParallel(state.Dependency).Complete();

        DOTSEventsManager.Instance?.TriggerOnBarracksUnitQueueChanged(onBarracksUnitQueueChangedEntityList);

        state.Dependency = JobHandle.CombineDependencies(jobHandleNativeArray);
    }

    public void OnDestroy(ref SystemState state)
    {
        jobHandleNativeArray.Dispose();
        onBarracksUnitQueueChangedEntityList.Dispose();
        onHealthDeadEntityList.Dispose();
        onHordeStartedSpawningEntityList.Dispose();
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


    public NativeList<Entity>.ParallelWriter onHealthDeadEntityList;


    public void Execute(ref Health health, Entity entity)
    {
        if (health.onDead)
        {
            onHealthDeadEntityList.AddNoResize(entity);
        }

        health.onHealthChanged = false;
        health.onDead = false;
        health.onTookDamage = false;
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
        if (buildingBarracks.onUnitQueueChanged)
        {
            onUnitQueueChangedEntityList.AddNoResize(entity);
        }

        buildingBarracks.onUnitQueueChanged = false;

    }

}


[BurstCompile]
public partial struct ResetHordeEventsJob : IJobEntity
{

    public NativeList<Entity>.ParallelWriter onHordeStartedSpawningEntityList;
    public NativeList<Entity>.ParallelWriter onHordeStartSpawningSoonEntityList;


    public void Execute(ref Horde horde, Entity entity)
    {
        if (horde.onStartSpawningSoon)
        {
            onHordeStartSpawningSoonEntityList.AddNoResize(entity);
        }
        if (horde.onStartSpawning)
        {
            onHordeStartedSpawningEntityList.AddNoResize(entity);
        }
        horde.onStartSpawning = false;
        horde.onStartSpawningSoon = false;
    }

}