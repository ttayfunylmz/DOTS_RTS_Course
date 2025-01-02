using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

partial struct FindTargetSystem : ISystem
{

    private ComponentLookup<LocalTransform> localTransformComponentLookup;
    private ComponentLookup<Faction> factionComponentLookup;
    private EntityStorageInfoLookup entityStorageInfoLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        localTransformComponentLookup = state.GetComponentLookup<LocalTransform>(true);
        factionComponentLookup = state.GetComponentLookup<Faction>(true);
        entityStorageInfoLookup = state.GetEntityStorageInfoLookup();
    }


    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
        //NativeList<DistanceHit> distanceHitList = new NativeList<DistanceHit>(Allocator.Temp);

        localTransformComponentLookup.Update(ref state);
        factionComponentLookup.Update(ref state);
        entityStorageInfoLookup.Update(ref state);

        FindTargetJob findTargetJob = new FindTargetJob
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            collisionWorld = collisionWorld,
            entityStorageInfoLookup = entityStorageInfoLookup,
            factionComponentLookup = factionComponentLookup,
            localTransformComponentLookup = localTransformComponentLookup,
        };
        findTargetJob.ScheduleParallel();

        /*
        foreach ((
            RefRO<LocalTransform> localTransform,
            RefRW<FindTarget> findTarget,
            RefRW<Target> target,
            RefRO<TargetOverride> targetOverride)
            in SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRW<FindTarget>,
                RefRW<Target>,
                RefRO<TargetOverride>>()) {


            findTarget.ValueRW.timer -= SystemAPI.Time.DeltaTime;
            if (findTarget.ValueRO.timer > 0f) {
                // Timer not elapsed
                continue;
            }
            findTarget.ValueRW.timer = findTarget.ValueRO.timerMax;

            if (targetOverride.ValueRO.targetEntity != Entity.Null) {
                target.ValueRW.targetEntity = targetOverride.ValueRO.targetEntity;
                continue;
            }

            distanceHitList.Clear();
            CollisionFilter collisionFilter = new CollisionFilter {
                BelongsTo = ~0u,
                CollidesWith = 1u << GameAssets.UNITS_LAYER | 1u << GameAssets.BUILDINGS_LAYER,
                GroupIndex = 0,
            };
            Entity closestTargetEntity = Entity.Null;
            float closestTargetDistance = float.MaxValue;
            float currentTargetDistanceOffset = 0f;
            if (target.ValueRO.targetEntity != Entity.Null) {
                closestTargetEntity = target.ValueRO.targetEntity;
                LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
                closestTargetDistance = math.distance(localTransform.ValueRO.Position, targetLocalTransform.Position);
                currentTargetDistanceOffset = 2f;
            }
            if (collisionWorld.OverlapSphere(localTransform.ValueRO.Position, findTarget.ValueRO.range, ref distanceHitList, collisionFilter)) {
                foreach (DistanceHit distanceHit in distanceHitList) {
                    if (!SystemAPI.Exists(distanceHit.Entity) || !SystemAPI.HasComponent<Faction>(distanceHit.Entity)) {
                        continue;
                    }
                    Faction targetFaction = SystemAPI.GetComponent<Faction>(distanceHit.Entity);
                    if (targetFaction.factionType == findTarget.ValueRO.targetFaction) {
                        // Valid target
                        if (closestTargetEntity == Entity.Null) {
                            closestTargetEntity = distanceHit.Entity;
                            closestTargetDistance = distanceHit.Distance;
                        } else {
                            if (distanceHit.Distance + currentTargetDistanceOffset < closestTargetDistance) {
                                closestTargetEntity = distanceHit.Entity;
                                closestTargetDistance = distanceHit.Distance;
                            }
                        }
                    }
                }
            }
            if (closestTargetEntity != Entity.Null) {
                target.ValueRW.targetEntity = closestTargetEntity;
            }
        }
        */
    }


    [BurstCompile]
    public partial struct FindTargetJob : IJobEntity
    {


        [ReadOnly] public ComponentLookup<LocalTransform> localTransformComponentLookup;
        [ReadOnly] public ComponentLookup<Faction> factionComponentLookup;
        [ReadOnly] public EntityStorageInfoLookup entityStorageInfoLookup;
        [ReadOnly] public CollisionWorld collisionWorld;

        public float deltaTime;


        public void Execute(
            in LocalTransform localTransform,
            ref FindTarget findTarget,
            ref Target target,
            in TargetOverride targetOverride)
        {


            findTarget.timer -= deltaTime;
            if (findTarget.timer > 0f)
            {
                // Timer not elapsed
                return;
            }
            findTarget.timer += findTarget.timerMax;

            if (targetOverride.targetEntity != Entity.Null)
            {
                target.targetEntity = targetOverride.targetEntity;
                return;
            }

            NativeList<DistanceHit> distanceHitList = new NativeList<DistanceHit>(Allocator.TempJob);

            CollisionFilter collisionFilter = new CollisionFilter
            {
                BelongsTo = ~0u,
                CollidesWith = 1u << GameAssets.UNITS_LAYER | 1u << GameAssets.BUILDINGS_LAYER,
                GroupIndex = 0,
            };
            Entity closestTargetEntity = Entity.Null;
            float closestTargetDistance = float.MaxValue;
            float currentTargetDistanceOffset = 0f;
            if (target.targetEntity != Entity.Null)
            {
                closestTargetEntity = target.targetEntity;
                LocalTransform targetLocalTransform = localTransformComponentLookup[target.targetEntity];
                closestTargetDistance = math.distance(localTransform.Position, targetLocalTransform.Position);
                currentTargetDistanceOffset = 2f;
            }
            if (collisionWorld.OverlapSphere(localTransform.Position, findTarget.range, ref distanceHitList, collisionFilter))
            {
                foreach (DistanceHit distanceHit in distanceHitList)
                {
                    if (!entityStorageInfoLookup.Exists(distanceHit.Entity) || !factionComponentLookup.HasComponent(distanceHit.Entity))
                    {
                        continue;
                    }
                    Faction targetFaction = factionComponentLookup[distanceHit.Entity];
                    if (targetFaction.factionType == findTarget.targetFaction)
                    {
                        // Valid target
                        if (closestTargetEntity == Entity.Null)
                        {
                            closestTargetEntity = distanceHit.Entity;
                            closestTargetDistance = distanceHit.Distance;
                        }
                        else
                        {
                            if (distanceHit.Distance + currentTargetDistanceOffset < closestTargetDistance)
                            {
                                closestTargetEntity = distanceHit.Entity;
                                closestTargetDistance = distanceHit.Distance;
                            }
                        }
                    }
                }
            }
            if (closestTargetEntity != Entity.Null)
            {
                target.targetEntity = closestTargetEntity;
            }

            distanceHitList.Dispose();
        }
    }

}
