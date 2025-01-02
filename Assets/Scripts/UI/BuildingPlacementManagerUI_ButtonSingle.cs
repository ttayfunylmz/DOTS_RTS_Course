using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingPlacementManagerUI_ButtonSingle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{


    public static event EventHandler OnAnyButtonSingleMouseOver;
    public static event EventHandler OnAnyButtonSingleMouseOut;


    [SerializeField] private Image iconImage;
    [SerializeField] private Image selectedImage;


    private BuildingTypeSO buildingTypeSO;


    public void Setup(BuildingTypeSO buildingTypeSO)
    {
        this.buildingTypeSO = buildingTypeSO;

        GetComponent<Button>().onClick.AddListener(() =>
        {
            BuildingPlacementManager.Instance.SetActiveBuildingTypeSO(buildingTypeSO);
        });

        iconImage.sprite = buildingTypeSO.sprite;
    }

    public void ShowSelected()
    {
        selectedImage.enabled = true;
    }

    public void HideSelected()
    {
        selectedImage.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipScreenSpaceUI.ShowTooltip_Static(
            buildingTypeSO.nameString + "\n" +
            ResourceAmount.GetString(buildingTypeSO.buildCostResourceAmountArray), 99f);

        OnAnyButtonSingleMouseOver?.Invoke(this, EventArgs.Empty);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipScreenSpaceUI.HideTooltip_Static();

        OnAnyButtonSingleMouseOut?.Invoke(this, EventArgs.Empty);
    }

}