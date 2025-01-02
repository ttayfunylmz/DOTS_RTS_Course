using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct DroneHarvesterSystem : ISystem
{



    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach ((
            RefRW<LocalTransform> localTransform,
            RefRW<DroneHarvester> droneHarvester,
            RefRO<DroneHarvesterVisualEntities> droneHarvesterVisualEntities,
            Entity entity)
            in SystemAPI.Query<
                RefRW<LocalTransform>,
                RefRW<DroneHarvester>,
                RefRO<DroneHarvesterVisualEntities>>().WithEntityAccess())
        {

            switch (droneHarvester.ValueRO.state)
            {
                default:
                case DroneHarvester.State.GoingToNode:
                    float3 moveDir = droneHarvester.ValueRO.targetPosition - localTransform.ValueRO.Position;
                    moveDir = math.normalize(moveDir);

                    float moveSpeed = 8f;
                    localTransform.ValueRW.Position += moveDir * moveSpeed * SystemAPI.Time.DeltaTime;

                    float reachedPositionDistance = 1f;
                    if (math.distance(localTransform.ValueRO.Position, droneHarvester.ValueRO.targetPosition) < reachedPositionDistance)
                    {
                        droneHarvester.ValueRW.state = DroneHarvester.State.Harvesting;
                    }
                    break;
                case DroneHarvester.State.Harvesting:
                    MoveVisualSphere(SystemAPI.GetComponentRW<LocalTransform>(droneHarvesterVisualEntities.ValueRO.sphereVisual1Entity), SystemAPI.Time.DeltaTime);
                    MoveVisualSphere(SystemAPI.GetComponentRW<LocalTransform>(droneHarvesterVisualEntities.ValueRO.sphereVisual2Entity), SystemAPI.Time.DeltaTime);
                    MoveVisualSphere(SystemAPI.GetComponentRW<LocalTransform>(droneHarvesterVisualEntities.ValueRO.sphereVisual3Entity), SystemAPI.Time.DeltaTime);
                    MoveVisualSphere(SystemAPI.GetComponentRW<LocalTransform>(droneHarvesterVisualEntities.ValueRO.sphereVisual4Entity), SystemAPI.Time.DeltaTime);
                    MoveVisualSphere(SystemAPI.GetComponentRW<LocalTransform>(droneHarvesterVisualEntities.ValueRO.sphereVisual5Entity), SystemAPI.Time.DeltaTime);
                    MoveVisualSphere(SystemAPI.GetComponentRW<LocalTransform>(droneHarvesterVisualEntities.ValueRO.sphereVisual6Entity), SystemAPI.Time.DeltaTime);

                    droneHarvester.ValueRW.harvestTimer -= SystemAPI.Time.DeltaTime;

                    if (droneHarvester.ValueRO.harvestTimer <= 0d)
                    {
                        droneHarvester.ValueRW.harvestTimer = droneHarvester.ValueRW.harvestTimerMax;
                        droneHarvester.ValueRW.state = DroneHarvester.State.GoingBack;
                    }
                    break;
                case DroneHarvester.State.GoingBack:
                    HideVisualSphere(SystemAPI.GetComponentRW<LocalTransform>(droneHarvesterVisualEntities.ValueRO.sphereVisual1Entity), SystemAPI.Time.DeltaTime);
                    HideVisualSphere(SystemAPI.GetComponentRW<LocalTransform>(droneHarvesterVisualEntities.ValueRO.sphereVisual2Entity), SystemAPI.Time.DeltaTime);
                    HideVisualSphere(SystemAPI.GetComponentRW<LocalTransform>(droneHarvesterVisualEntities.ValueRO.sphereVisual3Entity), SystemAPI.Time.DeltaTime);
                    HideVisualSphere(SystemAPI.GetComponentRW<LocalTransform>(droneHarvesterVisualEntities.ValueRO.sphereVisual4Entity), SystemAPI.Time.DeltaTime);
                    HideVisualSphere(SystemAPI.GetComponentRW<LocalTransform>(droneHarvesterVisualEntities.ValueRO.sphereVisual5Entity), SystemAPI.Time.DeltaTime);
                    HideVisualSphere(SystemAPI.GetComponentRW<LocalTransform>(droneHarvesterVisualEntities.ValueRO.sphereVisual6Entity), SystemAPI.Time.DeltaTime);

                    moveDir = droneHarvester.ValueRO.spawnPosition - localTransform.ValueRO.Position;
                    moveDir = math.normalize(moveDir);

                    moveSpeed = 8f;
                    localTransform.ValueRW.Position += moveDir * moveSpeed * SystemAPI.Time.DeltaTime;

                    reachedPositionDistance = 1f;
                    if (math.distance(localTransform.ValueRO.Position, droneHarvester.ValueRO.spawnPosition) < reachedPositionDistance)
                    {
                        droneHarvester.ValueRW.state = DroneHarvester.State.GoingToNode;
                        if (!SystemAPI.Exists(droneHarvester.ValueRO.parentEntity))
                        {
                            // Parent was destroyed
                            entityCommandBuffer.DestroyEntity(entity);
                            continue;
                        }
                    }
                    break;
            }
        }

    }


    private static void MoveVisualSphere(RefRW<LocalTransform> localTransform, float deltaTime)
    {
        localTransform.ValueRW.Scale = .2f;
        float sphereMoveSpeed = 10f;
        localTransform.ValueRW.Position += new float3(0, 1, 0) * sphereMoveSpeed * deltaTime;
        float yTop = 5;
        float yBottom = -1;
        if (localTransform.ValueRO.Position.y > yTop)
        {
            localTransform.ValueRW.Position = new float3(0, yBottom, 0);
        }
    }

    private static void HideVisualSphere(RefRW<LocalTransform> localTransform, float deltaTime)
    {
        localTransform.ValueRW.Scale = 0f;
    }

}