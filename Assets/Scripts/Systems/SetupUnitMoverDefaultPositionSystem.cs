using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct SetupUnitMoverDefaultPositionSystem : ISystem
{


    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach ((
            RefRO<LocalTransform> localTransform,
            RefRW<UnitMover> unitMover,
            RefRO<SetupUnitMoverDefaultPosition> setupUnitMoverDefaultPosition,
            Entity entity)
            in SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRW<UnitMover>,
                RefRO<SetupUnitMoverDefaultPosition>>().WithEntityAccess())
        {

            unitMover.ValueRW.targetPosition = localTransform.ValueRO.Position;
            entityCommandBuffer.RemoveComponent<SetupUnitMoverDefaultPosition>(entity);
        }
    }


}