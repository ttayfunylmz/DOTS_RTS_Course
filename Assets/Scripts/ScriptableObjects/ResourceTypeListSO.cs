using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceTypeListSO", menuName = "ScriptableObjects/ResourceTypeListSO")]
public class ResourceTypeListSO : ScriptableObject
{
    public List<ResourceTypeSO> resourceTypeSOList;
}
