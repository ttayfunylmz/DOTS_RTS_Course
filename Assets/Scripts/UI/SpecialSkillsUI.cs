using UnityEngine;
using UnityEngine.UI;

public class SpecialSkillsUI : MonoBehaviour
{


    [SerializeField] private Button airstrikeButton;
    [SerializeField] private GameObject airstrikeButtonSelectedGameObject;
    [SerializeField] private Image airstrikeProgressImage;


    private void Awake()
    {
        airstrikeButton.onClick.AddListener(() =>
        {
            if (SpecialSkillsManager.Instance.CanUseAirstrikeSkill())
            {
                // Can use skill
                BuildingPlacementManager.Instance.ClearActiveBuildingType();
                SpecialSkillsManager.Instance.SetIsAirstrikeButtonActive(true);
            }
        });

        airstrikeButton.GetComponent<PointerEnterExitHook>().Setup(
            () =>
            {
                TooltipScreenSpaceUI.ShowTooltip_Static(GetAirstrikeTooltipString, 99f);
            },
            () =>
            {
                TooltipScreenSpaceUI.HideTooltip_Static();
            }
        );
    }

    private void Start()
    {
        SpecialSkillsManager.Instance.OnIsAirstrikeButtonActiveChanged += SpecialSkillsManager_OnIsAirstrikeButtonActiveChanged;
        SpecialSkillsManager.Instance.OnZombiesKilledChanged += SpecialSkillsManager_OnZombiesKilledChanged;

        UpdateVisual();
    }

    private string GetAirstrikeTooltipString()
    {
        string returnString = "Airstrike";

        if (!SpecialSkillsManager.Instance.CanUseAirstrikeSkill())
        {
            returnString += "\n<color=#ff0000>Cannot use yet " + Mathf.RoundToInt(SpecialSkillsManager.Instance.GetAirstrikeSkillProgress() * 100f) + "%</color>";
        }

        return returnString;
    }

    private void SpecialSkillsManager_OnZombiesKilledChanged(object sender, System.EventArgs e)
    {
        UpdateVisual();
    }

    private void SpecialSkillsManager_OnIsAirstrikeButtonActiveChanged(object sender, System.EventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        airstrikeButtonSelectedGameObject.SetActive(SpecialSkillsManager.Instance.IsAirstrikeButtonActive());

        airstrikeProgressImage.fillAmount = SpecialSkillsManager.Instance.GetAirstrikeSkillProgress();
    }

}