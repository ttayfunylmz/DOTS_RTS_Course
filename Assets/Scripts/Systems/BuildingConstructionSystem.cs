using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct BuildingConstructionSystem : ISystem
{

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach ((
            RefRO<LocalTransform> localTransform,
            RefRW<BuildingConstruction> buildingConstruction,
            Entity entity)
            in SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRW<BuildingConstruction>>().WithEntityAccess())
        {

            RefRW<LocalTransform> visualLocalTransform =
                SystemAPI.GetComponentRW<LocalTransform>(buildingConstruction.ValueRO.visualEntity);

            visualLocalTransform.ValueRW.Position =
                math.lerp(buildingConstruction.ValueRO.startPosition,
                    buildingConstruction.ValueRO.endPosition,
                    buildingConstruction.ValueRO.constructionTimer / buildingConstruction.ValueRO.constructionTimerMax);

            buildingConstruction.ValueRW.constructionTimer += SystemAPI.Time.DeltaTime;
            if (buildingConstruction.ValueRO.constructionTimer >= buildingConstruction.ValueRO.constructionTimerMax)
            {
                // Construct final building
                Entity spawnedBuildingEntity = entityCommandBuffer.Instantiate(buildingConstruction.ValueRO.finalPrefabEntity);
                entityCommandBuffer.SetComponent(spawnedBuildingEntity, LocalTransform.FromPosition(localTransform.ValueRO.Position));

                entityCommandBuffer.DestroyEntity(buildingConstruction.ValueRO.visualEntity);
                entityCommandBuffer.DestroyEntity(entity);
            }
        }
    }


}