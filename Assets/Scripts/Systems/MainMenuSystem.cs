using Unity.Burst;
using Unity.Entities;

partial struct MainMenuSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MainMenuSceneTag>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        
    }
}
