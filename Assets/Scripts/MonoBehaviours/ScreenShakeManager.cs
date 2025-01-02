using Unity.Cinemachine;
using Unity.Entities;
using UnityEngine;

public class ScreenShakeManager : MonoBehaviour
{



    [SerializeField] private CinemachineImpulseSource buildingPlacedCinemachineImpulseSource;
    [SerializeField] private CinemachineImpulseSource buildingExplodeCinemachineImpulseSource;


    private void Start()
    {
        BuildingPlacementManager.Instance.OnBuildingPlaced += BuildingPlacementManager_OnBuildingPlaced;
        DOTSEventsManager.Instance.OnHealthDead += DOTSEventsManager_OnHealthDead;
        SpecialSkillsManager.Instance.OnAirStrikeExplosion += SpecialSkillsManager_OnAirStrikeExplosion;
    }

    private void SpecialSkillsManager_OnAirStrikeExplosion(object sender, System.EventArgs e)
    {
        buildingExplodeCinemachineImpulseSource.GenerateImpulse();
    }

    private void DOTSEventsManager_OnHealthDead(object sender, System.EventArgs e)
    {
        Entity entity = (Entity)sender;

        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        if (entityManager.HasComponent<BuildingTypeSOHolder>(entity))
        {
            // Building Destroyed
            BuildingTypeSOHolder buildingTypeSOHolder = entityManager.GetComponentData<BuildingTypeSOHolder>(entity);
            buildingExplodeCinemachineImpulseSource.GenerateImpulse();
        }
    }

    private void BuildingPlacementManager_OnBuildingPlaced(object sender, System.EventArgs e)
    {
        buildingPlacedCinemachineImpulseSource.GenerateImpulse();
    }

}