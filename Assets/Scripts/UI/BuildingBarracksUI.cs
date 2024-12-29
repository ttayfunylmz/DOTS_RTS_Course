using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class BuildingBarracksUI : MonoBehaviour
{
    [SerializeField] private Button soldierButton;
    [SerializeField] private Button scoutButton;
    [SerializeField] private Image progressBarImage;
    [SerializeField] private RectTransform unitQueueContainer;
    [SerializeField] private RectTransform unitQueueTemplate;

    private Entity buildingBarracksEntity;
    private EntityManager entityManager;

    private void Awake()
    {
        soldierButton.onClick.AddListener(() =>
        {
            UnitTypeSO unitTypeSO = GameAssets.Instance.unitTypeListSO.GetUnitTypeSO(UnitTypeSO.UnitType.Soldier);
            if(!ResourceManager.Instance.CanSpendResourceAmount(unitTypeSO.spawnCostResourceAmountArray))
            {
                return;
            }

            ResourceManager.Instance.SpendResourceAmount(unitTypeSO.spawnCostResourceAmountArray);

            entityManager.SetComponentData(buildingBarracksEntity, new BuildingBarracksUnitEnqueue
            {
                unitType = UnitTypeSO.UnitType.Soldier
            });
            entityManager.SetComponentEnabled<BuildingBarracksUnitEnqueue>(buildingBarracksEntity, true);
        });

        scoutButton.onClick.AddListener(() =>
        {
            UnitTypeSO unitTypeSO = GameAssets.Instance.unitTypeListSO.GetUnitTypeSO(UnitTypeSO.UnitType.Scout);
            if(!ResourceManager.Instance.CanSpendResourceAmount(unitTypeSO.spawnCostResourceAmountArray))
            {
                return;
            }

            ResourceManager.Instance.SpendResourceAmount(unitTypeSO.spawnCostResourceAmountArray);

            entityManager.SetComponentData(buildingBarracksEntity, new BuildingBarracksUnitEnqueue
            {
                unitType = UnitTypeSO.UnitType.Scout
            });
            entityManager.SetComponentEnabled<BuildingBarracksUnitEnqueue>(buildingBarracksEntity, true);
        });

        unitQueueTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        UnitSelectionManager.Instance.OnSelectedEntitiesChanged += UnitSelectionManager_OnSelectedEntitiesChanged;
        DOTSEventsManager.Instance.OnBarracksUnitQueueChanged += DOTSEventsManager_OnBarracksUnitQueueChanged;

        Hide();
    }

    private void DOTSEventsManager_OnBarracksUnitQueueChanged(object sender, EventArgs e)
    {
        Entity entity = (Entity)sender;

        if(entity == buildingBarracksEntity)
        {
            UpdateUnitQueueVisual();
        }
    }

    private void Update()
    {
        UpdateProgressBarVisual();
    }

    private void UnitSelectionManager_OnSelectedEntitiesChanged(object sender, EventArgs e)
    {
        EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected, BuildingBarracks>().Build(entityManager);

        NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);

        if (entityArray.Length > 0)
        {
            // SELECTED A BARRACKS
            buildingBarracksEntity = entityArray[0];
            Show();
            UpdateProgressBarVisual();
            UpdateUnitQueueVisual();
        }
        else
        {
            buildingBarracksEntity = Entity.Null;
            Hide();
        }
    }

    private void UpdateProgressBarVisual()
    {
        if (buildingBarracksEntity == Entity.Null)
        {
            progressBarImage.fillAmount = 0f;
            return;
        }

        BuildingBarracks buildingBarracks = entityManager.GetComponentData<BuildingBarracks>(buildingBarracksEntity);

        if (buildingBarracks.activeUnitType == UnitTypeSO.UnitType.None)
        {
            progressBarImage.fillAmount = 0f;
        }
        else
        {
            progressBarImage.fillAmount = buildingBarracks.progress / buildingBarracks.progressMax;
        }
    }

    private void UpdateUnitQueueVisual()
    {
        foreach(Transform child in unitQueueContainer)
        {
            if(child == unitQueueTemplate)
            {
                continue;
            }
            Destroy(child.gameObject);
        }

        DynamicBuffer<SpawnUnitTypeBuffer> spawnUnitTypeDynamicBuffer
            = entityManager.GetBuffer<SpawnUnitTypeBuffer>(buildingBarracksEntity, true);
        
        foreach(SpawnUnitTypeBuffer spawnUnitTypeBuffer in spawnUnitTypeDynamicBuffer)
        {
            RectTransform unitQueueRectTransform = Instantiate(unitQueueTemplate, unitQueueContainer);
            unitQueueRectTransform.gameObject.SetActive(true);

            UnitTypeSO unitTypeSO = GameAssets.Instance.unitTypeListSO.GetUnitTypeSO(spawnUnitTypeBuffer.unitType);
            unitQueueRectTransform.GetComponent<Image>().sprite = unitTypeSO.sprite;
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
