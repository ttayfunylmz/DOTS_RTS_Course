using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingPlacementManager : MonoBehaviour
{


    public static BuildingPlacementManager Instance { get; private set; }


    public event EventHandler OnActiveBuildingTypeSOChanged;
    public event EventHandler OnBuildingPlaced;


    [SerializeField] private BuildingTypeSO buildingTypeSO;
    [SerializeField] private UnityEngine.Material ghostMaterial;
    [SerializeField] private UnityEngine.Material ghostRedMaterial;
    [SerializeField] private Transform buildingDirtParticleSystem;


    private Transform ghostTransform;
    private UnityEngine.Material activeGhostMaterial;


    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (ghostTransform != null)
        {
            ghostTransform.position = MouseWorldPosition.Instance.GetPosition();
        }

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (buildingTypeSO.IsNone())
        {
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            ClearActiveBuildingType();
            return;
        }

        TooltipScreenSpaceUI.ShowTooltip_Static(
            buildingTypeSO.nameString + "\n" +
            ResourceAmount.GetString(buildingTypeSO.buildCostResourceAmountArray), .05f);

        if (!ResourceManager.Instance.CanSpendResourceAmount(buildingTypeSO.buildCostResourceAmountArray))
        {
            // Cannot afford this building
            SetGhostMaterial(ghostRedMaterial);
            TooltipScreenSpaceUI.ShowTooltip_Static(
                buildingTypeSO.nameString + "\n" +
                ResourceAmount.GetString(buildingTypeSO.buildCostResourceAmountArray) + "\n" +
                "<color=#ff0000>Cannot afford resource cost!</color>", .05f);
            return;
        }
        else
        {
            SetGhostMaterial(ghostMaterial);
        }

        {
            if (!CanPlaceBuilding(out string errorMessage))
            {
                // Cannot place building here
                SetGhostMaterial(ghostRedMaterial);
                TooltipScreenSpaceUI.ShowTooltip_Static(
                    buildingTypeSO.nameString + "\n" +
                    ResourceAmount.GetString(buildingTypeSO.buildCostResourceAmountArray) + "\n" +
                    "<color=#ff0000>" + errorMessage + "</color>", .05f);
                return;
            }
            else
            {
                SetGhostMaterial(ghostMaterial);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (ResourceManager.Instance.CanSpendResourceAmount(buildingTypeSO.buildCostResourceAmountArray))
            {
                if (CanPlaceBuilding(out string errorMessage))
                {
                    ResourceManager.Instance.SpendResourceAmount(buildingTypeSO.buildCostResourceAmountArray);

                    Vector3 mouseWorldPosition = MouseWorldPosition.Instance.GetPosition();

                    EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

                    EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(EntitiesReferences));
                    EntitiesReferences entitiesReferences = entityQuery.GetSingleton<EntitiesReferences>();

                    //Entity spawnedEntity = entityManager.Instantiate(buildingTypeSO.GetPrefabEntity(entitiesReferences));
                    //entityManager.SetComponentData(spawnedEntity, LocalTransform.FromPosition(mouseWorldPosition));

                    Entity buildingConstructionVisualEntity = entityManager.Instantiate(buildingTypeSO.GetVisualPrefabEntity(entitiesReferences));

                    Entity visualParentEntity = entityManager.CreateEntity();
                    entityManager.AddComponent<LocalTransform>(visualParentEntity);
                    entityManager.AddComponent<LocalToWorld>(visualParentEntity);
                    entityManager.SetComponentData(visualParentEntity, LocalTransform.FromPosition(mouseWorldPosition));
                    entityManager.AddBuffer<Child>(visualParentEntity);
                    entityManager.GetBuffer<Child>(visualParentEntity).Add(new Child { Value = buildingConstructionVisualEntity });
                    entityManager.SetName(visualParentEntity, "construction");

                    entityManager.AddComponent<Parent>(buildingConstructionVisualEntity);
                    entityManager.SetComponentData(buildingConstructionVisualEntity, new Parent { Value = visualParentEntity });


                    entityManager.SetComponentData(buildingConstructionVisualEntity, LocalTransform.FromPosition(new Vector3(0, buildingTypeSO.constructionYOffset, 0)));
                    entityManager.AddComponent<ShakeObject>(buildingConstructionVisualEntity);
                    entityManager.SetComponentData(buildingConstructionVisualEntity, new ShakeObject
                    {
                        intensity = .05f,
                        timer = 999f,
                        random = new Unity.Mathematics.Random((uint)buildingConstructionVisualEntity.Index),
                        shakeEntity = visualParentEntity,
                    });

                    Entity buildingConstructionEntity = entityManager.Instantiate(entitiesReferences.buildingConstructionPrefabEntity);
                    entityManager.SetComponentData(buildingConstructionEntity, LocalTransform.FromPosition(mouseWorldPosition));
                    entityManager.SetComponentData(buildingConstructionEntity, new BuildingConstruction
                    {
                        buildingType = buildingTypeSO.buildingType,
                        constructionTimer = 0f,
                        constructionTimerMax = buildingTypeSO.buildingConstructionTimerMax,
                        finalPrefabEntity = buildingTypeSO.GetPrefabEntity(entitiesReferences),
                        visualEntity = buildingConstructionVisualEntity,
                        startPosition = mouseWorldPosition + new Vector3(0, buildingTypeSO.constructionYOffset, 0),
                        endPosition = mouseWorldPosition,
                    });

                    // Spawn Particles
                    Vector3 colliderSize = buildingTypeSO.prefab.GetComponent<UnityEngine.BoxCollider>().size * .4f;
                    Vector3[] dirtParticleSystemOffsetArray = new Vector3[] {
                        mouseWorldPosition,
                        mouseWorldPosition + new Vector3(+colliderSize.x, 0, +colliderSize.z),
                        mouseWorldPosition + new Vector3(-colliderSize.x, 0, +colliderSize.z),
                        mouseWorldPosition + new Vector3(-colliderSize.x, 0, -colliderSize.z),
                        mouseWorldPosition + new Vector3(+colliderSize.x, 0, -colliderSize.z),
                    };

                    foreach (Vector3 dirtParticleSystemOffset in dirtParticleSystemOffsetArray)
                    {
                        Transform buildingDirtParticleSystemTransform = Instantiate(buildingDirtParticleSystem);
                        buildingDirtParticleSystemTransform.position = dirtParticleSystemOffset;
                        Destroy(buildingDirtParticleSystemTransform.gameObject, buildingTypeSO.buildingConstructionTimerMax);
                    }

                    OnBuildingPlaced?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    // Cannot build here for reason: errorMessage
                }
            }
            else
            {
                // Cannot spend resources
            }
        }
    }

    private bool CanPlaceBuilding(out string errorMessage)
    {
        Vector3 mouseWorldPosition = MouseWorldPosition.Instance.GetPosition();
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
        PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
        CollisionFilter collisionFilter = new CollisionFilter
        {
            BelongsTo = ~0u,
            CollidesWith = 1u << GameAssets.BUILDINGS_LAYER | 1u << GameAssets.DEFAULT_LAYER,
            GroupIndex = 0,
        };

        UnityEngine.BoxCollider boxCollider = buildingTypeSO.prefab.GetComponent<UnityEngine.BoxCollider>();
        float bonusExtents = 1.1f;
        NativeList<DistanceHit> distanceHitList = new NativeList<DistanceHit>(Allocator.Temp);
        if (collisionWorld.OverlapBox(
            mouseWorldPosition,
            Quaternion.identity,
            boxCollider.size * .5f * bonusExtents,
            ref distanceHitList,
            collisionFilter))
        {
            // Hit something
            errorMessage = "Build area must be clear!";
            return false;
        }

        distanceHitList.Clear();
        if (collisionWorld.OverlapSphere(
            mouseWorldPosition,
            buildingTypeSO.buildingDistanceMin,
            ref distanceHitList,
            collisionFilter))
        {
            // Hit something within building radius
            foreach (DistanceHit distanceHit in distanceHitList)
            {
                if (entityManager.HasComponent<BuildingTypeSOHolder>(distanceHit.Entity))
                {
                    BuildingTypeSOHolder buildingTypeSOHolder = entityManager.GetComponentData<BuildingTypeSOHolder>(distanceHit.Entity);
                    if (buildingTypeSOHolder.buildingType == buildingTypeSO.buildingType)
                    {
                        // Same type too close
                        errorMessage = "Same Building Type too close!";
                        return false;
                    }
                }
                if (entityManager.HasComponent<BuildingConstruction>(distanceHit.Entity))
                {
                    BuildingConstruction buildingConstruction = entityManager.GetComponentData<BuildingConstruction>(distanceHit.Entity);
                    if (buildingConstruction.buildingType == buildingTypeSO.buildingType)
                    {
                        // Same type too close
                        errorMessage = "Same Building Type too close!";
                        return false;
                    }
                }
            }
        }

        if (buildingTypeSO is BuildingResourceHarversterTypeSO buildingResourceHarversterTypeSO)
        {
            bool hasValidNearbyResourceNodes = false;
            if (collisionWorld.OverlapSphere(
                mouseWorldPosition,
                buildingResourceHarversterTypeSO.harvestDistance,
                ref distanceHitList,
                collisionFilter))
            {
                // Hit something within harvest distance
                foreach (DistanceHit distanceHit in distanceHitList)
                {
                    if (entityManager.HasComponent<ResourceTypeSOHolder>(distanceHit.Entity))
                    {
                        ResourceTypeSOHolder resourceTypeSOHolder = entityManager.GetComponentData<ResourceTypeSOHolder>(distanceHit.Entity);
                        if (resourceTypeSOHolder.resourceType == buildingResourceHarversterTypeSO.harvestableResourceType)
                        {
                            // Nearby valid resource node
                            hasValidNearbyResourceNodes = true;
                            break;
                        }
                    }
                }
            }
            if (!hasValidNearbyResourceNodes)
            {
                errorMessage = "No valid Resource Nodes nearby!";
                return false;
            }
        }

        errorMessage = null;
        return true;
    }


    public BuildingTypeSO GetActiveBuildingTypeSO()
    {
        return buildingTypeSO;
    }

    public void ClearActiveBuildingType()
    {
        SetActiveBuildingTypeSO(GameAssets.Instance.buildingTypeListSO.none);
    }

    public void SetActiveBuildingTypeSO(BuildingTypeSO buildingTypeSO)
    {
        this.buildingTypeSO = buildingTypeSO;

        if (ghostTransform != null)
        {
            Destroy(ghostTransform.gameObject);
        }

        if (!buildingTypeSO.IsNone())
        {
            ghostTransform = Instantiate(buildingTypeSO.visualPrefab);
            SetGhostMaterial(ghostMaterial);
        }

        OnActiveBuildingTypeSOChanged?.Invoke(this, EventArgs.Empty);
    }

    private void SetGhostMaterial(UnityEngine.Material ghostMaterial)
    {
        if (activeGhostMaterial == ghostMaterial)
        {
            // Already set this material
            return;
        }

        activeGhostMaterial = ghostMaterial;

        foreach (MeshRenderer meshRenderer in ghostTransform.GetComponentsInChildren<MeshRenderer>())
        {
            meshRenderer.material = ghostMaterial;
        }
    }


}