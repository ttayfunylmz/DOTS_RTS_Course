using Unity.Entities;
using UnityEngine;

public class EntitiesReferencesAuthoring : MonoBehaviour
{


    public GameObject bulletPrefabGameObject;
    public GameObject zombiePrefabGameObject;
    public GameObject shootLightPrefabGameObject;
    public GameObject scoutPrefabGameObject;
    public GameObject soldierPrefabGameObject;

    public GameObject buildingTowerPrefabGameObject;
    public GameObject buildingBarracksPrefabGameObject;
    public GameObject buildingIronHarvesterPrefabGameObject;
    public GameObject buildingGoldHarvesterPrefabGameObject;
    public GameObject buildingOilHarvesterPrefabGameObject;

    public GameObject buildingTowerVisualPrefabGameObject;
    public GameObject buildingBarracksVisualPrefabGameObject;
    public GameObject buildingIronHarvesterVisualPrefabGameObject;
    public GameObject buildingGoldHarvesterVisualPrefabGameObject;
    public GameObject buildingOilHarvesterVisualPrefabGameObject;

    public GameObject buildingConstructionPrefabGameObject;
    public GameObject droneHarvesterPrefabGameObject;


    public class Baker : Baker<EntitiesReferencesAuthoring>
    {


        public override void Bake(EntitiesReferencesAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EntitiesReferences
            {
                bulletPrefabEntity = GetEntity(authoring.bulletPrefabGameObject, TransformUsageFlags.Dynamic),
                zombiePrefabEntity = GetEntity(authoring.zombiePrefabGameObject, TransformUsageFlags.Dynamic),
                shootLightPrefabEntity = GetEntity(authoring.shootLightPrefabGameObject, TransformUsageFlags.Dynamic),
                scoutPrefabEntity = GetEntity(authoring.scoutPrefabGameObject, TransformUsageFlags.Dynamic),
                soldierPrefabEntity = GetEntity(authoring.soldierPrefabGameObject, TransformUsageFlags.Dynamic),

                buildingTowerPrefabEntity = GetEntity(authoring.buildingTowerPrefabGameObject, TransformUsageFlags.Dynamic),
                buildingBarracksPrefabEntity = GetEntity(authoring.buildingBarracksPrefabGameObject, TransformUsageFlags.Dynamic),
                buildingIronHarvesterPrefabEntity = GetEntity(authoring.buildingIronHarvesterPrefabGameObject, TransformUsageFlags.Dynamic),
                buildingGoldHarvesterPrefabEntity = GetEntity(authoring.buildingGoldHarvesterPrefabGameObject, TransformUsageFlags.Dynamic),
                buildingOilHarvesterPrefabEntity = GetEntity(authoring.buildingOilHarvesterPrefabGameObject, TransformUsageFlags.Dynamic),

                buildingTowerVisualPrefabEntity = GetEntity(authoring.buildingTowerVisualPrefabGameObject, TransformUsageFlags.Dynamic),
                buildingBarracksVisualPrefabEntity = GetEntity(authoring.buildingBarracksVisualPrefabGameObject, TransformUsageFlags.Dynamic),
                buildingIronHarvesterVisualPrefabEntity = GetEntity(authoring.buildingIronHarvesterVisualPrefabGameObject, TransformUsageFlags.Dynamic),
                buildingGoldHarvesterVisualPrefabEntity = GetEntity(authoring.buildingGoldHarvesterVisualPrefabGameObject, TransformUsageFlags.Dynamic),
                buildingOilHarvesterVisualPrefabEntity = GetEntity(authoring.buildingOilHarvesterVisualPrefabGameObject, TransformUsageFlags.Dynamic),

                buildingConstructionPrefabEntity = GetEntity(authoring.buildingConstructionPrefabGameObject, TransformUsageFlags.Dynamic),
                droneHarvesterPrefabEntity = GetEntity(authoring.droneHarvesterPrefabGameObject, TransformUsageFlags.Dynamic),
            });
        }

    }

}


public struct EntitiesReferences : IComponentData
{

    public Entity bulletPrefabEntity;
    public Entity zombiePrefabEntity;
    public Entity shootLightPrefabEntity;
    public Entity scoutPrefabEntity;
    public Entity soldierPrefabEntity;

    public Entity buildingTowerPrefabEntity;
    public Entity buildingBarracksPrefabEntity;
    public Entity buildingIronHarvesterPrefabEntity;
    public Entity buildingGoldHarvesterPrefabEntity;
    public Entity buildingOilHarvesterPrefabEntity;

    public Entity buildingTowerVisualPrefabEntity;
    public Entity buildingBarracksVisualPrefabEntity;
    public Entity buildingIronHarvesterVisualPrefabEntity;
    public Entity buildingGoldHarvesterVisualPrefabEntity;
    public Entity buildingOilHarvesterVisualPrefabEntity;

    public Entity buildingConstructionPrefabEntity;
    public Entity droneHarvesterPrefabEntity;

}