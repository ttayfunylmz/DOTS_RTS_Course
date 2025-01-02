using UnityEngine;

[CreateAssetMenu()]
public class ResourceTypeSO : ScriptableObject
{


    public enum ResourceType
    {
        None,
        Iron,
        Gold,
        Oil,
    }


    public ResourceType resourceType;
    public Sprite sprite;

}