using TMPro;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class ZombieTopBarUI : MonoBehaviour
{


    [SerializeField] private TextMeshProUGUI zombieAmountTextMesh;
    [SerializeField] private TextMeshProUGUI zombieBuildingAmountTextMesh;


    private void Update()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Unit, Zombie>().Build(entityManager);

        zombieAmountTextMesh.text = entityQuery.CalculateEntityCount().ToString();

        entityQuery.Dispose();



        entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<ZombieSpawner>().Build(entityManager);

        zombieBuildingAmountTextMesh.text = entityQuery.CalculateEntityCount().ToString();

        entityQuery.Dispose();
    }

}