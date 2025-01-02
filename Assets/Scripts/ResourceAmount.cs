using System;
using UnityEngine;

[Serializable]
public struct ResourceAmount
{

    public ResourceTypeSO.ResourceType resourceType;
    public int amount;


    public static string GetString(ResourceAmount[] resourceAmountArray)
    {
        string resourceAmountString = "";
        foreach (ResourceAmount resourceAmount in resourceAmountArray)
        {
            if (resourceAmountString != null)
            {
                resourceAmountString += "\n";
            }
            resourceAmountString += resourceAmount.resourceType + " x" + resourceAmount.amount;
        }
        return resourceAmountString;
    }

}