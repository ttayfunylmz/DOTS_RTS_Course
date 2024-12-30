using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;
using Unity.Transforms;

partial struct VisualUnderFogOfWarSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameSceneTag>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;

        EntityCommandBuffer entityCommandBuffer
            = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach((
            RefRW<VisualUnderFogOfWar> visualUnderFogOfWar,
            Entity entity)
                in SystemAPI.Query<
                    RefRW<VisualUnderFogOfWar>>().WithEntityAccess())
        {
            LocalTransform parentLocalTransform = SystemAPI.GetComponent<LocalTransform>(visualUnderFogOfWar.ValueRO.parentEntity);

            if(!collisionWorld.SphereCast(
                parentLocalTransform.Position,
                visualUnderFogOfWar.ValueRO.sphereCastSize,
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
                if(visualUnderFogOfWar.ValueRO.isVisible)
                {
                    visualUnderFogOfWar.ValueRW.isVisible = false;
                    entityCommandBuffer.AddComponent<DisableRendering>(entity);
                }
            }
            else
            {
                // UNDER VISIBLE FOG OF WAR, SHOW IT
                if(!visualUnderFogOfWar.ValueRO.isVisible)
                {
                    visualUnderFogOfWar.ValueRW.isVisible = true;
                    entityCommandBuffer.RemoveComponent<DisableRendering>(entity);
                }
            }
        }
    }
}
