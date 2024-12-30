using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class RagdollManager : MonoBehaviour
{
    [SerializeField] private UnitTypeListSO unitTypeListSO;

    private void Start() 
    {
        DOTSEventsManager.Instance.OnHealthDead += DOTSEventsManager_OnHealthDead;    
    }

    private void DOTSEventsManager_OnHealthDead(object sender, EventArgs e)
    {
        Entity entity = (Entity)sender;
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        if(entityManager.HasComponent<UnitTypeHolder>(entity))
        {
            LocalTransform localTransform = entityManager.GetComponentData<LocalTransform>(entity);
            UnitTypeHolder unitTypeHolder = entityManager.GetComponentData<UnitTypeHolder>(entity);
            UnitTypeSO unitTypeSO = unitTypeListSO.GetUnitTypeSO(unitTypeHolder.unitType);
            Instantiate(unitTypeSO.ragdollPrefab, localTransform.Position, Quaternion.identity);
        }
    }
}
