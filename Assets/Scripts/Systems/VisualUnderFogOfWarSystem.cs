using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;
using Unity.Transforms;

partial struct VisualUnderFogOfWarSystem : ISystem
{
    private ComponentLookup<LocalTransform> localTransformComponentLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameSceneTag>();

        localTransformComponentLookup = state.GetComponentLookup<LocalTransform>(true);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;

        EntityCommandBuffer entityCommandBuffer
            = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        localTransformComponentLookup.Update(ref state);

        VisualUnderFogOfWarJob visualUnderFogOfWarJob = new VisualUnderFogOfWarJob
        {
            collisionWorld = collisionWorld,
            entityCommandBuffer = entityCommandBuffer.AsParallelWriter(),
            localTransformComponentLookup = localTransformComponentLookup,
            deltaTime = SystemAPI.Time.DeltaTime
        };

        visualUnderFogOfWarJob.ScheduleParallel();
    }

    [BurstCompile]
    public partial struct VisualUnderFogOfWarJob : IJobEntity
    {
        [ReadOnly] public ComponentLookup<LocalTransform> localTransformComponentLookup;
        [ReadOnly] public CollisionWorld collisionWorld;

        public EntityCommandBuffer.ParallelWriter entityCommandBuffer;
        public float deltaTime;

        public void Execute(ref VisualUnderFogOfWar visualUnderFogOfWar, [ChunkIndexInQuery] int chunkIndexInQuery, Entity entity)
        {
            visualUnderFogOfWar.timer -= deltaTime;
            if(visualUnderFogOfWar.timer > 0f) { return; }
            visualUnderFogOfWar.timer += visualUnderFogOfWar.timerMax;

            LocalTransform parentLocalTransform = localTransformComponentLookup[visualUnderFogOfWar.parentEntity];

            if (!collisionWorld.SphereCast(
                parentLocalTransform.Position,
                visualUnderFogOfWar.sphereCastSize,
                new float3(0, 1, 0),
                100,
                new CollisionFilter
                {
                    BelongsTo = ~0u,
                    CollidesWith = 1u << GameAssets.FOG_OF_WAR_LAYER,
                    GroupIndex = 0
                }))
            {
                // NOT UNDER VISIBLE FOG OF WAR, HIDE IT
                if (visualUnderFogOfWar.isVisible)
                {
                    visualUnderFogOfWar.isVisible = false;
                    entityCommandBuffer.AddComponent<DisableRendering>(chunkIndexInQuery, entity);
                }
            }
            else
            {
                // UNDER VISIBLE FOG OF WAR, SHOW IT
                if (!visualUnderFogOfWar.isVisible)
                {
                    visualUnderFogOfWar.isVisible = true;
                    entityCommandBuffer.RemoveComponent<DisableRendering>(chunkIndexInQuery, entity);
                }
            }
        }
    }
}
