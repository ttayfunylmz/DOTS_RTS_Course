using UnityEngine;
using UnityEngine.UI;

public class BuildingPlacementManagerUI_ButtonSingle : MonoBehaviour
{
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
}
