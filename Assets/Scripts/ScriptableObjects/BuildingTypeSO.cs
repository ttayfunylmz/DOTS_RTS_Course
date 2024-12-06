using UnityEngine;

[CreateAssetMenu(fileName = "BuildingTypeSO", menuName = "ScriptableObjects/BuildingTypeSO")]
public class BuildingTypeSO : ScriptableObject
{
    public enum BuildingType : byte
    {
        None,
        ZombieSpawner,
        Tower
    }

    public BuildingType buildingType;
}
