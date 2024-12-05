using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

partial struct FindTargetSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
        NativeList<DistanceHit> distanceHitList = new NativeList<DistanceHit>(Allocator.Temp);

        foreach ((
            RefRO<LocalTransform> localTransform,
            RefRW<FindTarget> findTarget,
            RefRW<Target> target,
            RefRO<TargetOverride> targetOverride)
                in SystemAPI.Query<
                    RefRO<LocalTransform>,
                    RefRW<FindTarget>,
                    RefRW<Target>,
                    RefRO<TargetOverride>>())
        {
            findTarget.ValueRW.timer -= SystemAPI.Time.DeltaTime;
            if(findTarget.ValueRO.timer > 0f) { continue; }
            findTarget.ValueRW.timer = findTarget.ValueRO.timerMax;

            if(targetOverride.ValueRO.targetEntity != Entity.Null)
            {
                target.ValueRW.targetEntity = targetOverride.ValueRO.targetEntity;
                continue;
            }

            distanceHitList.Clear();

            CollisionFilter collisionFilter = new CollisionFilter
            {
                BelongsTo = ~0u,
                CollidesWith = 1u << GameAssets.UNITS_LAYER | 1u << GameAssets.BUILDINGS_LAYER,
                GroupIndex = 0,
            };

            Entity closestTargetEntity = Entity.Null;
            float closestTargetDistance = float.MaxValue;
            float closestTargetDistanceOffset = 0f;

            if(target.ValueRO.targetEntity != Entity.Null)
            {
                closestTargetEntity = target.ValueRO.targetEntity;
                LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
                closestTargetDistance = math.distance(localTransform.ValueRO.Position, targetLocalTransform.Position);
                closestTargetDistanceOffset = 2f;
            }

            if(collisionWorld.OverlapSphere(localTransform.ValueRO.Position, findTarget.ValueRO.range,
                    ref distanceHitList, collisionFilter))
            {
                foreach(DistanceHit distanceHit in distanceHitList)
                {
                    if(!SystemAPI.Exists(distanceHit.Entity) || 
                        !SystemAPI.HasComponent<Faction>(distanceHit.Entity))
                    {
                        continue;
                    }

                    Faction targetFaction = SystemAPI.GetComponent<Faction>(distanceHit.Entity);
                    if(targetFaction.factionType == findTarget.ValueRO.targetFaction) // VALID TARGET
                    {
                        if(closestTargetEntity == Entity.Null)
                        {
                            closestTargetEntity = distanceHit.Entity;
                            closestTargetDistance = distanceHit.Distance;
                        }
                        else
                        {
                            if(distanceHit.Distance + closestTargetDistanceOffset < closestTargetDistance)
                            {
                                closestTargetEntity = distanceHit.Entity;
                                closestTargetDistance = distanceHit.Distance;
                            }
                        }
                    }
                }
            }
            if(closestTargetEntity != Entity.Null)
            {
                target.ValueRW.targetEntity = closestTargetEntity;
            }
        }
    }
}
