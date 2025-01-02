using CodeMonkey.Utils;
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

public class SpecialSkillsManager : MonoBehaviour
{


    public const int AIRSTRIKE_SKILL_ZOMBIES_KILLED = 30;
    public const string PLAY_AIRSTRIKE = "PlayAirstrike";

    public static SpecialSkillsManager Instance { get; private set; }


    public event EventHandler OnZombiesKilledChanged;
    public event EventHandler OnIsAirstrikeButtonActiveChanged;
    public event EventHandler OnAirStrikeExplosion;


    [SerializeField] private Transform airstrikeVisualTransform;
    [SerializeField] private Transform explosionVfxPrefab;
    [SerializeField] private Transform shootLightPrefab;
    [SerializeField] private GameObject airstrikeHelicoptersGameObject;
    [SerializeField] private Animator airstrikeHelicoptersAnimator;


    private int zombiesKilled = 0;
    private bool ignoreZombiesKilledEvent;
    private bool isAirstrikeButtonActive;


    private void Awake()
    {
        Instance = this;

        airstrikeVisualTransform.gameObject.SetActive(false);
        airstrikeHelicoptersGameObject.gameObject.SetActive(false);
    }

    private void Start()
    {
        DOTSEventsManager.Instance.OnHealthDead += DOTSEventsManager_OnHealthDead;
    }

    private void DOTSEventsManager_OnHealthDead(object sender, System.EventArgs e)
    {
        if (ignoreZombiesKilledEvent)
        {
            return;
        }

        Entity entity = (Entity)sender;

        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        if (entityManager.HasComponent<Zombie>(entity))
        {
            zombiesKilled++;
            OnZombiesKilledChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool CanUseAirstrikeSkill()
    {
        return zombiesKilled > AIRSTRIKE_SKILL_ZOMBIES_KILLED;
    }

    public float GetAirstrikeSkillProgress()
    {
        return (float)zombiesKilled / AIRSTRIKE_SKILL_ZOMBIES_KILLED;
    }

    public void UseAirstrikeSkill(Vector3 worldPosition)
    {
        ignoreZombiesKilledEvent = true;

        airstrikeVisualTransform.position = worldPosition;
        airstrikeVisualTransform.gameObject.SetActive(true);

        airstrikeHelicoptersGameObject.SetActive(true);
        airstrikeHelicoptersGameObject.transform.position = worldPosition;

        airstrikeHelicoptersAnimator.SetTrigger(PLAY_AIRSTRIKE);

        float airStrikeRadius = 16f;
        int airStrikeCount = 10;

        float firstExplosionTime = 2.2f;
        FunctionTimer.Create(() =>
        {
            TriggerAirStrike(worldPosition);
        }, firstExplosionTime);

        for (int i = 0; i < airStrikeCount; i++)
        {
            FunctionTimer.Create(() =>
            {
                Vector3 randomDir = new Vector3(UnityEngine.Random.Range(-1f, +1f), 0f, UnityEngine.Random.Range(-1f, +1f));
                TriggerAirStrike(worldPosition + randomDir * UnityEngine.Random.Range(1f, airStrikeRadius));
            }, firstExplosionTime + .1f + i * .1f + UnityEngine.Random.Range(0f, .4f));
        }

        FunctionTimer.Create(() =>
        {
            ignoreZombiesKilledEvent = false;
            airstrikeVisualTransform.gameObject.SetActive(false);
        }, firstExplosionTime + 2f);

        SetIsAirstrikeButtonActive(false);

        zombiesKilled = 0;
        OnZombiesKilledChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetIsAirstrikeButtonActive(bool isAirstrikeButtonActive)
    {
        this.isAirstrikeButtonActive = isAirstrikeButtonActive;
        OnIsAirstrikeButtonActiveChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool IsAirstrikeButtonActive()
    {
        return isAirstrikeButtonActive;
    }

    private void TriggerAirStrike(float3 worldPosition)
    {
        Instantiate(shootLightPrefab, worldPosition + new float3(0, 2, 0), quaternion.identity);

        Transform explosionVfxTransform = Instantiate(explosionVfxPrefab, worldPosition, quaternion.identity);
        Destroy(explosionVfxTransform.gameObject, 15f);

        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
        PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;

        float explosionRadius = 7f;
        NativeList<DistanceHit> distanceHitList = new NativeList<DistanceHit>(Allocator.Temp);
        if (collisionWorld.OverlapSphere(
            worldPosition,
            explosionRadius,
            ref distanceHitList,
            new CollisionFilter
            {
                BelongsTo = ~0u,
                CollidesWith = 1u << GameAssets.UNITS_LAYER,
                GroupIndex = 0,
            }))
        {
            foreach (DistanceHit distanceHit in distanceHitList)
            {
                if (entityManager.HasComponent<Zombie>(distanceHit.Entity))
                {
                    // Zombie in range
                    // Deal damage
                    Health health = entityManager.GetComponentData<Health>(distanceHit.Entity);
                    health.healthAmount -= 40;
                    health.onHealthChanged = true;
                    health.onTookDamage = true;
                    entityManager.SetComponentData<Health>(distanceHit.Entity, health);
                }
            }
        }

        distanceHitList.Dispose();

        OnAirStrikeExplosion?.Invoke(this, EventArgs.Empty);
    }

}