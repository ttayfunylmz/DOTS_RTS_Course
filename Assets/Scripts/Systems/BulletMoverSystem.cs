using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct BulletMoverSystem : ISystem
{


    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach ((
            RefRW<LocalTransform> localTransform,
            RefRW<Bullet> bullet,
            RefRO<Target> target,
            Entity entity)
            in SystemAPI.Query<
                RefRW<LocalTransform>,
                RefRW<Bullet>,
                RefRO<Target>>().WithEntityAccess())
        {

            float3 targetPosition;

            if (target.ValueRO.targetEntity == Entity.Null)
            {
                // Has no target
                targetPosition = bullet.ValueRO.lastTargetPosition;
            }
            else
            {
                // Has target
                LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
                ShootVictim targetShootVictim = SystemAPI.GetComponent<ShootVictim>(target.ValueRO.targetEntity);
                targetPosition = targetLocalTransform.TransformPoint(targetShootVictim.hitLocalPosition);
            }

            bullet.ValueRW.lastTargetPosition = targetPosition;

            float distanceBeforeSq = math.distancesq(localTransform.ValueRO.Position, targetPosition);

            float3 moveDirection = targetPosition - localTransform.ValueRO.Position;
            moveDirection = math.normalize(moveDirection);

            localTransform.ValueRW.Position += moveDirection * bullet.ValueRO.speed * SystemAPI.Time.DeltaTime;

            float distanceAfterSq = math.distancesq(localTransform.ValueRO.Position, targetPosition);

            if (distanceAfterSq > distanceBeforeSq)
            {
                // Overshot
                localTransform.ValueRW.Position = targetPosition;
            }

            float destroyDistanceSq = .2f;
            if (math.distancesq(localTransform.ValueRO.Position, targetPosition) < destroyDistanceSq)
            {
                // Close enough to damage target
                if (target.ValueRO.targetEntity != Entity.Null)
                {
                    RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);
                    targetHealth.ValueRW.healthAmount -= bullet.ValueRO.damageAmount;
                    targetHealth.ValueRW.onHealthChanged = true;
                    targetHealth.ValueRW.onTookDamage = true;
                }

                entityCommandBuffer.DestroyEntity(entity);
            }
        }
    }


}
