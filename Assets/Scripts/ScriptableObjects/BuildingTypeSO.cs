using Unity.Entities;
using UnityEngine;

[CreateAssetMenu()]
public class BuildingTypeSO : ScriptableObject
{


    public enum BuildingType
    {
        None,
        ZombieSpawner,
        Tower,
        Barracks,
        HQ,
        GoldHarvester,
        IronHarvester,
        OilHarvester,
    }


    public string nameString;
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
        switch (buildingType)
        {
            default:
            case BuildingType.None:
            case BuildingType.Tower: return entitiesReferences.buildingTowerPrefabEntity;
            case BuildingType.Barracks: return entitiesReferences.buildingBarracksPrefabEntity;
            case BuildingType.IronHarvester: return entitiesReferences.buildingIronHarvesterPrefabEntity;
            case BuildingType.GoldHarvester: return entitiesReferences.buildingGoldHarvesterPrefabEntity;
            case BuildingType.OilHarvester: return entitiesReferences.buildingOilHarvesterPrefabEntity;
        }
    }

    public Entity GetVisualPrefabEntity(EntitiesReferences entitiesReferences)
    {
        switch (buildingType)
        {
            default:
            case BuildingType.None:
            case BuildingType.Tower: return entitiesReferences.buildingTowerVisualPrefabEntity;
            case BuildingType.Barracks: return entitiesReferences.buildingBarracksVisualPrefabEntity;
            case BuildingType.IronHarvester: return entitiesReferences.buildingIronHarvesterVisualPrefabEntity;
            case BuildingType.GoldHarvester: return entitiesReferences.buildingGoldHarvesterVisualPrefabEntity;
            case BuildingType.OilHarvester: return entitiesReferences.buildingOilHarvesterVisualPrefabEntity;
        }
    }

}