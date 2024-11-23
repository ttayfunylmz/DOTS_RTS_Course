using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct BulletMoverSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer 
            = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

        foreach ((
            RefRW<LocalTransform> LocalTransform,
            RefRO<Bullet> bullet,
            RefRO<Target> target,
            Entity entity)
                in SystemAPI.Query<
                    RefRW<LocalTransform>,
                    RefRO<Bullet>,
                    RefRO<Target>>().WithEntityAccess())
        {
            if(target.ValueRO.targetEntity == Entity.Null)
            {
                entityCommandBuffer.DestroyEntity(entity);
                continue;
            }

            LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
            ShootVictim targetShootVictim = SystemAPI.GetComponent<ShootVictim>(target.ValueRO.targetEntity);
            float3 targetPosition = targetLocalTransform.TransformPoint(targetShootVictim.hitLocalPosition); 

            float distanceBeforeSq = math.distancesq(LocalTransform.ValueRO.Position, targetPosition);

            float3 moveDirection = targetPosition - LocalTransform.ValueRO.Position;
            moveDirection = math.normalize(moveDirection);

            LocalTransform.ValueRW.Position += moveDirection * bullet.ValueRO.speed * SystemAPI.Time.DeltaTime;

            float distanceAfterSq = math.distancesq(LocalTransform.ValueRO.Position, targetPosition);
            
            if(distanceAfterSq > distanceBeforeSq) // OVERSHOT
            {
                LocalTransform.ValueRW.Position = targetPosition;
            }

            float destroyDistanceSq = .2f;
            if(math.distancesq(LocalTransform.ValueRO.Position, targetPosition) < destroyDistanceSq)
            {
                // CLOSE ENOUGH TO DAMAGE TARGET
                RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);
                targetHealth.ValueRW.healthAmount -= bullet.ValueRO.damageAmount;
                targetHealth.ValueRW.onHealthChanged = true;

                entityCommandBuffer.DestroyEntity(entity);
            }
        }
    }
}
