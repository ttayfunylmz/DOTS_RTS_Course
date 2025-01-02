using System.Collections.Generic;
using UnityEngine;

public class ResourceManagerUI : MonoBehaviour
{


    [SerializeField] private Transform container;
    [SerializeField] private Transform template;
    [SerializeField] private ResourceTypeListSO resourceTypeListSO;


    private Dictionary<ResourceTypeSO.ResourceType, ResourceManagerUI_Single> resourceTypeUISingleDictionary;


    private void Awake()
    {
        template.gameObject.SetActive(false);
    }

    private void Start()
    {
        ResourceManager.Instance.OnResourceAmountChanged += ResourceManager_OnResourceAmountChanged;

        Setup();
        UpdateAmounts();
    }

    private void ResourceManager_OnResourceAmountChanged(object sender, System.EventArgs e)
    {
        UpdateAmounts();
    }

    private void Setup()
    {
        foreach (Transform child in container)
        {
            if (child == template)
            {
                continue;
            }
            Destroy(child.gameObject);
        }

        resourceTypeUISingleDictionary = new Dictionary<ResourceTypeSO.ResourceType, ResourceManagerUI_Single>();
        foreach (ResourceTypeSO resourceTypeSO in resourceTypeListSO.resourceTypeSOList)
        {
            Transform resourceTransform = Instantiate(template, container);
            resourceTransform.gameObject.SetActive(true);
            ResourceManagerUI_Single resourceManagerUISingle = resourceTransform.GetComponent<ResourceManagerUI_Single>();
            resourceManagerUISingle.Setup(resourceTypeSO);

            resourceTypeUISingleDictionary[resourceTypeSO.resourceType] = resourceManagerUISingle;
        }
    }

    private void UpdateAmounts()
    {
        foreach (ResourceTypeSO resourceTypeSO in resourceTypeListSO.resourceTypeSOList)
        {
            resourceTypeUISingleDictionary[resourceTypeSO.resourceType].
                UpdateAmount(ResourceManager.Instance.GetResourceAmount(resourceTypeSO.resourceType));
        }
    }

}