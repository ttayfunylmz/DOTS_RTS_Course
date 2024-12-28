using Unity.Entities;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingTypeSO", menuName = "ScriptableObjects/BuildingTypeSO")]
public class BuildingTypeSO : ScriptableObject
{
    public enum BuildingType : byte
    {
        None,
        ZombieSpawner,
        Tower,
        Barracks,
        HQ,
        GoldHarvester,
        IronHarvester,
        OilHarvester
    }

    public BuildingType buildingType;
    public Transform prefab;
    public float buildingDistanceMin;
    public bool showInBuildingPlacementManagerUI;
    public Sprite sprite;
    public Transform visualPrefab;

    public bool IsNone()
    {
        return buildingType == BuildingType.None;
    }

    public Entity GetPrefabEntity(EntitiesReferences entitiesReferences)
    {
        return buildingType switch
        {
            BuildingType.Tower => entitiesReferences.buildingTowerPrefabEntity,
            BuildingType.Barracks => entitiesReferences.buildingBarracksPrefabEntity,
            BuildingType.IronHarvester => entitiesReferences.buildingIronHarvesterPrefabEntity,
            BuildingType.GoldHarvester => entitiesReferences.buildingGoldHarvesterPrefabEntity,
            BuildingType.OilHarvester => entitiesReferences.buildingOilHarvesterPrefabEntity,
            _ => entitiesReferences.buildingTowerPrefabEntity,
        };
    }
}
