using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct BuildingBarracksSystem : ISystem
{


    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EntitiesReferences>();
    }

    public void OnUpdate(ref SystemState state)
    {
        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

        foreach ((
            RefRW<BuildingBarracks> buildingBarracks,
            DynamicBuffer<SpawnUnitTypeBuffer> spawnUnitTypeDynamicBuffer,
            RefRO<BuildingBarracksUnitEnqueue> buildingBarracksUnitEnqueue,
            EnabledRefRW<BuildingBarracksUnitEnqueue> buildingBarracksUnitEnqueueEnabled)
            in SystemAPI.Query<
                RefRW<BuildingBarracks>,
                DynamicBuffer<SpawnUnitTypeBuffer>,
                RefRO<BuildingBarracksUnitEnqueue>,
                EnabledRefRW<BuildingBarracksUnitEnqueue>>())
        {

            spawnUnitTypeDynamicBuffer.Add(new SpawnUnitTypeBuffer
            {
                unitType = buildingBarracksUnitEnqueue.ValueRO.unitType
            });
            buildingBarracksUnitEnqueueEnabled.ValueRW = false;

            buildingBarracks.ValueRW.onUnitQueueChanged = true;
        }


        foreach ((
            RefRO<LocalTransform> localTransform,
            RefRW<BuildingBarracks> buildingBarracks,
            DynamicBuffer<SpawnUnitTypeBuffer> spawnUnitTypeDynamicBuffer)
            in SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRW<BuildingBarracks>,
                DynamicBuffer<SpawnUnitTypeBuffer>>())
        {

            if (spawnUnitTypeDynamicBuffer.IsEmpty)
            {
                continue;
            }

            if (buildingBarracks.ValueRO.activeUnitType != spawnUnitTypeDynamicBuffer[0].unitType)
            {
                buildingBarracks.ValueRW.activeUnitType = spawnUnitTypeDynamicBuffer[0].unitType;

                UnitTypeSO activeUnitTypeSO =
                    GameAssets.Instance.unitTypeListSO.GetUnitTypeSO(buildingBarracks.ValueRO.activeUnitType);

                buildingBarracks.ValueRW.progressMax = activeUnitTypeSO.progressMax;
            }

            buildingBarracks.ValueRW.progress += SystemAPI.Time.DeltaTime;

            if (buildingBarracks.ValueRO.progress < buildingBarracks.ValueRO.progressMax)
            {
                continue;
            }

            buildingBarracks.ValueRW.progress = 0f;

            UnitTypeSO.UnitType unitType = spawnUnitTypeDynamicBuffer[0].unitType;
            UnitTypeSO unitTypeSO = GameAssets.Instance.unitTypeListSO.GetUnitTypeSO(unitType);

            spawnUnitTypeDynamicBuffer.RemoveAt(0);
            buildingBarracks.ValueRW.onUnitQueueChanged = true;

            Entity spawnedUnitEntity = state.EntityManager.Instantiate(unitTypeSO.GetPrefabEntity(entitiesReferences));
            SystemAPI.SetComponent(spawnedUnitEntity, LocalTransform.FromPosition(localTransform.ValueRO.Position));

            SystemAPI.SetComponent(spawnedUnitEntity, new MoveOverride
            {
                targetPosition = localTransform.ValueRO.Position + buildingBarracks.ValueRO.rallyPositionOffset
            });
            SystemAPI.SetComponentEnabled<MoveOverride>(spawnedUnitEntity, true);
        }
    }

}