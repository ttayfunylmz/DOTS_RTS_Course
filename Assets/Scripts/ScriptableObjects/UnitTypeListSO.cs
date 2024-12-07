using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitTypeListSO", menuName = "ScriptableObjects/UnitTypeListSO")]
public class UnitTypeListSO : ScriptableObject
{
    public List<UnitTypeSO> unitTypeSOList;

    public UnitTypeSO GetUnitTypeSO(UnitTypeSO.UnitType unitType)
    {
        foreach(UnitTypeSO unitTypeSO in unitTypeSOList)
        {
            if(unitTypeSO.unitType == unitType)
            {
                return unitTypeSO;
            }
        }

        Debug.LogError("Could not find GetUnitTypeSO for UnitType: " + unitType);
        return null;
    }
}
