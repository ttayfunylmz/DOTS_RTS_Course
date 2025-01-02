using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class RagdollManager : MonoBehaviour
{


    [SerializeField] private UnitTypeListSO unitTypeListSO;


    private void Start()
    {
        DOTSEventsManager.Instance.OnHealthDead += DOTSEventsManager_OnHealthDead;
    }

    private void DOTSEventsManager_OnHealthDead(object sender, System.EventArgs e)
    {
        Entity entity = (Entity)sender;
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        if (entityManager.HasComponent<UnitTypeHolder>(entity))
        {
            LocalTransform localTransform = entityManager.GetComponentData<LocalTransform>(entity);
            UnitTypeHolder unitTypeHolder = entityManager.GetComponentData<UnitTypeHolder>(entity);
            UnitTypeSO unitTypeSO = unitTypeListSO.GetUnitTypeSO(unitTypeHolder.unitType);

            Transform ragdollTransform = Instantiate(unitTypeSO.ragdollPrefab, localTransform.Position, localTransform.Rotation);

            Vector3 explosionPosition =
                ragdollTransform.position +
                ragdollTransform.forward +
                new Vector3(Random.Range(-1f, +1f), Random.Range(0f, +2f), Random.Range(-1f, +1f));

            foreach (Rigidbody rigidbody in ragdollTransform.GetComponentsInChildren<Rigidbody>())
            {
                rigidbody.AddExplosionForce(1000f, explosionPosition, 10f);
            }
        }
    }


}