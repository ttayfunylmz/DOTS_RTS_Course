using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct MoveOverrideSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((
            RefRO<LocalTransform> localTransform,
            RefRO<MoveOverride> moveOverride,
            EnabledRefRW<MoveOverride> moveOverrideEnabled,
            RefRW<UnitMover> unitMover)
                in SystemAPI.Query<
                    RefRO<LocalTransform>,
                    RefRO<MoveOverride>,
                    EnabledRefRW<MoveOverride>,
                    RefRW<UnitMover>>())
        {
            if(math.distancesq(localTransform.ValueRO.Position, moveOverride.ValueRO.targetPosition) 
                > UnitMoverSystem.REACHED_TARGET_POSITION_DISTANCE_SQ)
            {
                //MOVE CLOSER
                unitMover.ValueRW.targetPosition = moveOverride.ValueRO.targetPosition;
            }
            else
            {
                //REACHED THE MOVE OVERRIDE POSITION
                moveOverrideEnabled.ValueRW = false;
            }
        }
    }
}
