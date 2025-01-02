using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class BuildingDestructionManager : MonoBehaviour
{


    [SerializeField] private Transform buildingHQVisualBrokenPartsPrefab;
    [SerializeField] private Transform buildingBarracksVisualBrokenPartsPrefab;
    [SerializeField] private Transform buildingTowerBrokenPartsPrefab;
    [SerializeField] private Transform buildingGoldHarvesterBrokenPartsPrefab;
    [SerializeField] private Transform buildingIronHarvesterBrokenPartsPrefab;
    [SerializeField] private Transform buildingOilHarvesterBrokenPartsPrefab;
    [SerializeField] private Transform buildingZombieSpawnerVisualBrokenPartsPrefab;


    private void Start()
    {
        DOTSEventsManager.Instance.OnHealthDead += DOTSEventsManager_OnHealthDead;
    }

    private void DOTSEventsManager_OnHealthDead(object sender, System.EventArgs e)
    {
        Entity entity = (Entity)sender;

        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        if (entityManager.HasComponent<BuildingTypeSOHolder>(entity))
        {
            // Building Destroyed
            BuildingTypeSOHolder buildingTypeSOHolder = entityManager.GetComponentData<BuildingTypeSOHolder>(entity);
            LocalTransform localTransform = entityManager.GetComponentData<LocalTransform>(entity);

            Transform brokenPartsTransform = null;

            switch (buildingTypeSOHolder.buildingType)
            {
                default:
                case BuildingTypeSO.BuildingType.ZombieSpawner:
                    brokenPartsTransform = Instantiate(buildingZombieSpawnerVisualBrokenPartsPrefab, localTransform.Position, Quaternion.identity);
                    break;
                case BuildingTypeSO.BuildingType.HQ:
                    brokenPartsTransform = Instantiate(buildingHQVisualBrokenPartsPrefab, localTransform.Position, Quaternion.identity);
                    break;
                case BuildingTypeSO.BuildingType.Barracks:
                    brokenPartsTransform = Instantiate(buildingBarracksVisualBrokenPartsPrefab, localTransform.Position, Quaternion.identity);
                    break;
                case BuildingTypeSO.BuildingType.Tower:
                    brokenPartsTransform = Instantiate(buildingTowerBrokenPartsPrefab, localTransform.Position, Quaternion.identity);
                    break;
                case BuildingTypeSO.BuildingType.GoldHarvester:
                    brokenPartsTransform = Instantiate(buildingGoldHarvesterBrokenPartsPrefab, localTransform.Position, Quaternion.identity);
                    break;
                case BuildingTypeSO.BuildingType.IronHarvester:
                    brokenPartsTransform = Instantiate(buildingIronHarvesterBrokenPartsPrefab, localTransform.Position, Quaternion.identity);
                    break;
                case BuildingTypeSO.BuildingType.OilHarvester:
                    brokenPartsTransform = Instantiate(buildingOilHarvesterBrokenPartsPrefab, localTransform.Position, Quaternion.identity);
                    break;
            }

            foreach (Rigidbody rigidbody in brokenPartsTransform.GetComponentsInChildren<Rigidbody>())
            {
                rigidbody.AddExplosionForce(300f, brokenPartsTransform.position, 10f);
            }

            Destroy(brokenPartsTransform.gameObject, 10f);
        }
    }

}


