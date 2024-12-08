using Unity.Entities;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitTypeSO", menuName = "ScriptableObjects/UnitTypeSO")]
public class UnitTypeSO : ScriptableObject
{
    public enum UnitType : byte
    {
        None,
        Soldier,
        Scout,
        Zombie
    }

    public UnitType unitType;
    public float progressMax;
    public Sprite sprite;

    public Entity GetPrefabEntity(EntitiesReferences entitiesReferences)
    {
        return unitType switch
        {
            UnitType.Soldier => entitiesReferences.soldierPrefabEntity,
            UnitType.Scout => entitiesReferences.scoutPrefabEntity,
            UnitType.Zombie => entitiesReferences.zombiePrefabEntity,
            _ => entitiesReferences.soldierPrefabEntity,
        };
    }
}
