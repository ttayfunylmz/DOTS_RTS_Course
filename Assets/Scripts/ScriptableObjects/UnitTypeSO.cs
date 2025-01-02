using Unity.Entities;
using UnityEngine;

[CreateAssetMenu()]
public class UnitTypeSO : ScriptableObject
{


    public enum UnitType
    {
        None,
        Soldier,
        Scout,
        Zombie,
    }


    public UnitType unitType;
    public Transform ragdollPrefab;
    public float progressMax;
    public Sprite sprite;
    public ResourceAmount[] spawnCostResourceAmountArray;


    public Entity GetPrefabEntity(EntitiesReferences entitiesReferences)
    {
        return unitType switch
        {
            UnitType.Scout => entitiesReferences.scoutPrefabEntity,
            UnitType.Zombie => entitiesReferences.zombiePrefabEntity,
            _ => entitiesReferences.soldierPrefabEntity,
        };
    }


}