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
            LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);

            float distanceBeforeSq = math.distancesq(LocalTransform.ValueRO.Position, targetLocalTransform.Position);

            float3 moveDirection = targetLocalTransform.Position - LocalTransform.ValueRO.Position;
            moveDirection = math.normalize(moveDirection);

            LocalTransform.ValueRW.Position += moveDirection * bullet.ValueRO.speed * SystemAPI.Time.DeltaTime;

            float distanceAfterSq = math.distancesq(LocalTransform.ValueRO.Position, targetLocalTransform.Position);
            
            if(distanceAfterSq > distanceBeforeSq) // OVERSHOT
            {
                LocalTransform.ValueRW.Position = targetLocalTransform.Position;
            }

            float destroyDistanceSq = .2f;
            if(math.distancesq(LocalTransform.ValueRO.Position, targetLocalTransform.Position) < destroyDistanceSq)
            {
                // CLOSE ENOUGH TO DAMAGE TARGET
                RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);
                targetHealth.ValueRW.healthAmount -= bullet.ValueRO.damageAmount;

                entityCommandBuffer.DestroyEntity(entity);
            }
        }
    }
}
