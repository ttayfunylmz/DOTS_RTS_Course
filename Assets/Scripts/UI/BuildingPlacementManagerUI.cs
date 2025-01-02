using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacementManagerUI : MonoBehaviour
{


    [SerializeField] private RectTransform buildingContainer;
    [SerializeField] private RectTransform buildingTemplate;
    [SerializeField] private BuildingTypeListSO buildingTypeListSO;


    private Dictionary<BuildingTypeSO, BuildingPlacementManagerUI_ButtonSingle> buildingButtonDictionary;


    private void Awake()
    {
        buildingTemplate.gameObject.SetActive(false);

        buildingButtonDictionary = new Dictionary<BuildingTypeSO, BuildingPlacementManagerUI_ButtonSingle>();

        foreach (BuildingTypeSO buildingTypeSO in buildingTypeListSO.buildingTypeSOList)
        {
            if (!buildingTypeSO.showInBuildingPlacementManagerUI)
            {
                continue;
            }

            RectTransform buildingRectTransfrom = Instantiate(buildingTemplate, buildingContainer);
            buildingRectTransfrom.gameObject.SetActive(true);

            BuildingPlacementManagerUI_ButtonSingle buttonSingle =
                buildingRectTransfrom.GetComponent<BuildingPlacementManagerUI_ButtonSingle>();

            buildingButtonDictionary[buildingTypeSO] = buttonSingle;

            buttonSingle.Setup(buildingTypeSO);
        }
    }

    private void Start()
    {
        BuildingPlacementManager.Instance.OnActiveBuildingTypeSOChanged += BuildingPlacementManager_OnActiveBuildingTypeSOChanged;

        UpdateSelectedVisual();
    }

    private void BuildingPlacementManager_OnActiveBuildingTypeSOChanged(object sender, System.EventArgs e)
    {
        UpdateSelectedVisual();
    }

    private void UpdateSelectedVisual()
    {
        foreach (BuildingTypeSO buildingTypeSO in buildingButtonDictionary.Keys)
        {
            buildingButtonDictionary[buildingTypeSO].HideSelected();
        }

        buildingButtonDictionary[BuildingPlacementManager.Instance.GetActiveBuildingTypeSO()].
            ShowSelected();
    }

}