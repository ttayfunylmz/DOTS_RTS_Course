using UnityEngine;

[CreateAssetMenu(fileName = "BuildingResourceHarvesterTypeSO", menuName = "ScriptableObjects/BuildingResourceHarvesterTypeSO")]
public class BuildingResourceHarvesterTypeSO : BuildingTypeSO
{
    public ResourceTypeSO.ResourceType harvestableResourceType;
    public float harvestDistance;
}
