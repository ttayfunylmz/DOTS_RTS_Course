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
    public float buildingConstructionTimerMax;
    public float constructionYOffset;
    public Transform prefab;
    public float buildingDistanceMin;
    public bool showInBuildingPlacementManagerUI;
    public Sprite sprite;
    public Transform visualPrefab;
    public ResourceAmount[] buildCostResourceAmountArray;

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

    public Entity GetVisualPrefabEntity(EntitiesReferences entitiesReferences)
    {
        return buildingType switch
        {
            BuildingType.Tower => entitiesReferences.buildingTowerVisualPrefabEntity,
            BuildingType.Barracks => entitiesReferences.buildingBarracksVisualPrefabEntity,
            BuildingType.IronHarvester => entitiesReferences.buildingIronHarvesterVisualPrefabEntity,
            BuildingType.GoldHarvester => entitiesReferences.buildingGoldHarvesterVisualPrefabEntity,
            BuildingType.OilHarvester => entitiesReferences.buildingOilHarvesterVisualPrefabEntity,
            _ => entitiesReferences.buildingTowerVisualPrefabEntity,
        };
    }
}
