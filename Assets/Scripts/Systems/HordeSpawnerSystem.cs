using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct HordeSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EntitiesReferences>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

        EntityCommandBuffer entityCommandBuffer
            = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach ((
            RefRO<LocalTransform> localTransform,
            RefRW<Horde> horde)
                in SystemAPI.Query<
                    RefRO<LocalTransform>,
                    RefRW<Horde>>())
        {
            horde.ValueRW.startTimer -= SystemAPI.Time.DeltaTime;

            if (horde.ValueRO.startTimer > 0) { continue; }

            // START TIMER IS ELAPSED

            if (horde.ValueRO.zombieAmountToSpawn <= 0)
            {
                // ALL ZOMBIES ALREADY SPAWNED
                continue;
            }

            // STILL HAS ZOMBIES TO SPAWN
            horde.ValueRW.spawnTimer -= SystemAPI.Time.DeltaTime;
            if (horde.ValueRO.spawnTimer <= 0)
            {
                horde.ValueRW.spawnTimer = horde.ValueRW.spawnTimerMax;

                Entity zombieEntity = entityCommandBuffer.Instantiate(entitiesReferences.zombiePrefabEntity);

                Random random = horde.ValueRO.random;
                float3 spawnPosition = localTransform.ValueRO.Position;
                spawnPosition.x += random.NextFloat(-horde.ValueRO.spawnAreaWidth, +horde.ValueRO.spawnAreaWidth);
                spawnPosition.z += random.NextFloat(-horde.ValueRO.spawnAreaHeight, +horde.ValueRO.spawnAreaHeight);
                horde.ValueRW.random = random;

                entityCommandBuffer.SetComponent(zombieEntity, LocalTransform.FromPosition(spawnPosition));
                entityCommandBuffer.AddComponent<EnemyAttackHQ>(zombieEntity);

                horde.ValueRW.zombieAmountToSpawn--;
            }
        }
    }
}
