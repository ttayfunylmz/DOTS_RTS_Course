using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManagerUI_Single : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text text;

    public void Setup(ResourceTypeSO resourceTypeSO)
    {
        image.sprite = resourceTypeSO.sprite;
        text.text = "0";
    }

    public void UpdateAmount(int amount)
    {
        text.text = amount.ToString();
    }
}
