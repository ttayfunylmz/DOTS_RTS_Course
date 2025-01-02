using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

partial struct TestingSystem : ISystem
{

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        /*
        int unitCount = 0;

        foreach (
            RefRW<Zombie> zombie
            in SystemAPI.Query<
                RefRW<Zombie>>()) {

            unitCount++;
        }

        Debug.Log("unitCount: " + unitCount);
        */
    }

}