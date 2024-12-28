using UnityEngine;

[CreateAssetMenu(fileName = "ResourceTypeSO", menuName = "ScriptableObjects/ResourceTypeSO")]
public class ResourceTypeSO : ScriptableObject
{
    public enum ResourceType : byte
    {
        None,
        Iron,
        Gold,
        Oil
    }

    public ResourceType resourceType;
    public Sprite sprite;
}
