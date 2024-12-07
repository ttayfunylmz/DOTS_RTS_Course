using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BuildingBarracksAuthoring : MonoBehaviour
{
    public float progressMax;

    public class Baker : Baker<BuildingBarracksAuthoring>
    {
        public override void Bake(BuildingBarracksAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BuildingBarracks
            {
                progressMax = authoring.progressMax,
                rallyPositionOffset = new float3(10, 0, 0)
            });

            DynamicBuffer<SpawnUnitTypeBuffer> spawnUnitTypeDynamicBuffer 
                = AddBuffer<SpawnUnitTypeBuffer>(entity);
            
            spawnUnitTypeDynamicBuffer.Add(new SpawnUnitTypeBuffer
            {
                unitType = UnitTypeSO.UnitType.Soldier
            });
            spawnUnitTypeDynamicBuffer.Add(new SpawnUnitTypeBuffer
            {
                unitType = UnitTypeSO.UnitType.Soldier
            });
            spawnUnitTypeDynamicBuffer.Add(new SpawnUnitTypeBuffer
            {
                unitType = UnitTypeSO.UnitType.Scout
            });
        }
    }
}

public struct BuildingBarracks : IComponentData
{
    public float progress;
    public float progressMax;
    public UnitTypeSO.UnitType activeUnitType;
    public float3 rallyPositionOffset;
}

[InternalBufferCapacity(10)]
public struct SpawnUnitTypeBuffer : IBufferElementData
{
    public UnitTypeSO.UnitType unitType;
}
