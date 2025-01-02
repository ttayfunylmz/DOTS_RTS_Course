using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct DroneHarvesterControllerSystem : ISystem
{

    //[BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EntitiesReferences>();

        state.EntityManager.AddComponent<RandomHolder>(state.SystemHandle);
        state.EntityManager.SetComponentData(state.SystemHandle, new RandomHolder
        {
            random = new Random((uint)UnityEngine.Random.Range(0, 100000))
        });
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;

        EntityCommandBuffer entityCommandBuffer =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        RefRW<RandomHolder> randomHolder = SystemAPI.GetComponentRW<RandomHolder>(state.SystemHandle);
        Random random = randomHolder.ValueRO.random;

        foreach ((
            RefRO<LocalTransform> localTransform,
            RefRW<DroneHarvesterController> droneHarvesterController,
            Entity entity)
            in SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRW<DroneHarvesterController>>().WithEntityAccess())
        {

            droneHarvesterController.ValueRW.spawnTimer -= SystemAPI.Time.DeltaTime;
            if (droneHarvesterController.ValueRO.spawnTimer > 0)
            {
                continue;
            }
            droneHarvesterController.ValueRW.spawnTimer = 1.5f;

            // Can spawn more drones?
            if (droneHarvesterController.ValueRO.drone1Entity != Entity.Null &&
                droneHarvesterController.ValueRO.drone2Entity != Entity.Null &&
                droneHarvesterController.ValueRO.drone3Entity != Entity.Null &&
                droneHarvesterController.ValueRO.drone4Entity != Entity.Null)
            {
                // All drones already spawned
                continue;
            }

            NativeList<DistanceHit> distanceHitList = new NativeList<DistanceHit>(Allocator.Temp);
            float nearbyResourceNodeDistance = 10f;
            if (collisionWorld.OverlapSphere(
                localTransform.ValueRO.Position,
                nearbyResourceNodeDistance,
                ref distanceHitList,
                new CollisionFilter
                {
                    BelongsTo = ~0u,
                    CollidesWith = 1u << GameAssets.BUILDINGS_LAYER | 1u << GameAssets.DEFAULT_LAYER,
                    GroupIndex = 0,
                }))
            {

                foreach (DistanceHit distanceHit in distanceHitList)
                {
                    if (SystemAPI.HasComponent<ResourceTypeSOHolder>(distanceHit.Entity) &&
                        SystemAPI.GetComponent<ResourceTypeSOHolder>(distanceHit.Entity).resourceType == droneHarvesterController.ValueRO.resourceType)
                    {
                        // Found nearby resource node of same type
                        LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(distanceHit.Entity);
                        float randomDistance = random.NextFloat(0f, 5f);
                        float3 randomDir = random.NextFloat3(new float3(-randomDistance, 0, -randomDistance), new float3(+randomDistance, 0, +randomDistance));
                        float3 targetPosition = targetLocalTransform.Position + randomDir;
                        Entity droneHarvesterEntity = entityCommandBuffer.Instantiate(entitiesReferences.droneHarvesterPrefabEntity);
                        entityCommandBuffer.SetComponent(droneHarvesterEntity, LocalTransform.FromPosition(localTransform.ValueRO.Position));
                        entityCommandBuffer.SetComponent(droneHarvesterEntity, new DroneHarvester
                        {
                            parentEntity = entity,
                            state = DroneHarvester.State.GoingToNode,
                            harvestTimer = random.NextFloat(2f, 6f),
                            harvestTimerMax = random.NextFloat(2f, 6f),
                            spawnPosition = localTransform.ValueRO.Position,
                            targetPosition = targetPosition,
                        });
                        if (droneHarvesterController.ValueRO.drone1Entity == Entity.Null)
                        {
                            droneHarvesterController.ValueRW.drone1Entity = droneHarvesterEntity;
                            break;
                        }
                        if (droneHarvesterController.ValueRO.drone2Entity == Entity.Null)
                        {
                            droneHarvesterController.ValueRW.drone2Entity = droneHarvesterEntity;
                            break;
                        }
                        if (droneHarvesterController.ValueRO.drone3Entity == Entity.Null)
                        {
                            droneHarvesterController.ValueRW.drone3Entity = droneHarvesterEntity;
                            break;
                        }
                        if (droneHarvesterController.ValueRO.drone4Entity == Entity.Null)
                        {
                            droneHarvesterController.ValueRW.drone4Entity = droneHarvesterEntity;
                            break;
                        }
                        break;
                    }
                }
            }
        }

        randomHolder.ValueRW.random = random;
    }


}