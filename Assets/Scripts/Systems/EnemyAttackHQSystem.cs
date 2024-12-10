using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct EnemyAttackHQSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BuildingHQ>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Entity hqEntity = SystemAPI.GetSingletonEntity<BuildingHQ>();
        float3 hqPosition = SystemAPI.GetComponent<LocalTransform>(hqEntity).Position;

        foreach((
            RefRO<EnemyAttackHQ> enemyAttackHQ,
            RefRW<UnitMover> unitMover,
            RefRO<Target> target)
                in SystemAPI.Query<
                    RefRO<EnemyAttackHQ>,
                    RefRW<UnitMover>,
                    RefRO<Target>>().WithDisabled<MoveOverride>())
        {
            if(target.ValueRO.targetEntity != Entity.Null) { continue; }

            unitMover.ValueRW.targetPosition = hqPosition;
        }
    }
}
