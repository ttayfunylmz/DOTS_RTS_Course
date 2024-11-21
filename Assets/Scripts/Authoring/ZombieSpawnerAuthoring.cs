using Unity.Entities;
using UnityEngine;

public class ZombieSpawnerAuthoring : MonoBehaviour
{
    public float timerMax;
    public float randomWalkingDistanceMin;
    public float randomWalkingDistanceMax;

    public class Baker : Baker<ZombieSpawnerAuthoring>
    {
        public override void Bake(ZombieSpawnerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ZombieSpawner
            {
                timerMax = authoring.timerMax,
                randomWalkingDistanceMin = authoring.randomWalkingDistanceMin,
                randomWalkingDistanceMax = authoring.randomWalkingDistanceMax,
            });
        }
    }
}

public struct ZombieSpawner : IComponentData
{
    public float timer;
    public float timerMax;
    public float randomWalkingDistanceMin;
    public float randomWalkingDistanceMax;
}
