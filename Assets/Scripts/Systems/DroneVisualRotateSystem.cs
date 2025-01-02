using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct DroneVisualRotateSystem : ISystem
{


    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((
            RefRW<LocalTransform> localTransform,
            RefRW<DroneVisualRotate> droneVisualRotate)
            in SystemAPI.Query<
                RefRW<LocalTransform>,
                RefRW<DroneVisualRotate>>())
        {

            localTransform.ValueRW = localTransform.ValueRO.RotateY(droneVisualRotate.ValueRO.rotateSpeed * SystemAPI.Time.DeltaTime);
        }
    }


}