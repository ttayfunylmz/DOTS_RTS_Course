using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using UnityEngine;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial struct HealthDeadTestSystem : ISystem
{


    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach ((
            RefRW<Health> health,
            Entity entity)
            in SystemAPI.Query<
                RefRW<Health>>().WithEntityAccess())
        {

            if (health.ValueRO.healthAmount <= 0)
            {
                // This entity is dead
                health.ValueRW.onDead = true;

                if (SystemAPI.HasComponent<BuildingConstruction>(entity))
                {
                    BuildingConstruction buildingConstruction = SystemAPI.GetComponent<BuildingConstruction>(entity);
                    entityCommandBuffer.DestroyEntity(buildingConstruction.visualEntity);
                }

                entityCommandBuffer.DestroyEntity(entity);
            }
        }
    }


}