using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
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

        EntityCommandBuffer entityCommandBuffer =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach ((
           RefRO<LocalTransform> localTransform,
           RefRW<Horde> horde,
           Entity entity)
           in SystemAPI.Query<
               RefRO<LocalTransform>,
               RefRW<Horde>>().WithEntityAccess())
        {

            if (!horde.ValueRO.isSetup)
            {
                horde.ValueRW.isSetup = true;
                entityCommandBuffer.AddComponent<DisableRendering>(horde.ValueRO.minimapIconEntity);
            }

            float beforeStartTimer = horde.ValueRO.startTimer;

            horde.ValueRW.startTimer -= SystemAPI.Time.DeltaTime;

            float startSpawningSoonTime = 15f;
            if (beforeStartTimer > startSpawningSoonTime && horde.ValueRO.startTimer <= startSpawningSoonTime)
            {
                horde.ValueRW.onStartSpawningSoon = true;
                entityCommandBuffer.RemoveComponent<DisableRendering>(horde.ValueRO.minimapIconEntity);
            }

            if (horde.ValueRO.startTimer > 0)
            {
                continue;
            }

            if (beforeStartTimer > 0)
            {
                horde.ValueRW.onStartSpawning = true;
            }

            // Start timer is elapsed

            if (horde.ValueRO.zombieAmountToSpawn <= 0)
            {
                // All zombies already spawned
                entityCommandBuffer.DestroyEntity(entity);
                continue;
            }

            // Still has zombies to spawn
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