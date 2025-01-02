using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
[UpdateBefore(typeof(ResetEventsSystem))]
partial struct OnTookDamageSystem : ISystem
{


    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((
            RefRO<Health> health,
            RefRW<ShakeObject> shakeObject,
            EnabledRefRW<ShakeObject> shakeObjectEnabled)
            in SystemAPI.Query<
                RefRO<Health>,
                RefRW<ShakeObject>,
                EnabledRefRW<ShakeObject>>().WithPresent<ShakeObject>())
        {

            if (health.ValueRO.onTookDamage)
            {
                shakeObject.ValueRW.timer = .03f;
                shakeObjectEnabled.ValueRW = true;
            }
        }
    }


}