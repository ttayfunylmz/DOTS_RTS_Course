using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

partial struct WinGameOverTestSystem : ISystem
{


    private EntityQuery zombieUnitsEntityQuery;
    private EntityQuery zombieSpawnerBuildingsEntityQuery;
    private float timer;


    //[BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameSceneTag>();

        zombieUnitsEntityQuery = state.GetEntityQuery(typeof(Unit), typeof(Zombie));
        zombieSpawnerBuildingsEntityQuery = state.GetEntityQuery(typeof(ZombieSpawner));

        timer = 10f;
    }

    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        timer -= SystemAPI.Time.DeltaTime;
        if (timer > 0)
        {
            return;
        }
        float timerMax = .5f;
        timer = timerMax;

        if (zombieUnitsEntityQuery.CalculateEntityCount() == 0 &&
            zombieSpawnerBuildingsEntityQuery.CalculateEntityCount() == 0)
        {

            // Win!
            DOTSEventsManager.Instance.TriggerOnGameWin();
        }
    }

}