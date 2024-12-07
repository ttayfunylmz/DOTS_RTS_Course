using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingTypeListSO", menuName = "ScriptableObjects/BuildingTypeListSO")]
public class BuildingTypeListSO : ScriptableObject
{
    public List<BuildingTypeSO> buildingTypeSOList;

    public BuildingTypeSO GetBuildingTypeSO(BuildingTypeSO.BuildingType buildingType)
    {
        foreach(BuildingTypeSO buildingTypeSO in buildingTypeSOList)
        {
            if(buildingTypeSO.buildingType == buildingType)
            {
                return buildingTypeSO;
            }
        }

        Debug.LogError("Could not find BuildingTypeSO for BuildingType: " + buildingType);
        return null;
    }
}
