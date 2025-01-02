using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManagerUI_Single : MonoBehaviour
{


    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI textMesh;


    public void Setup(ResourceTypeSO resourceTypeSO)
    {
        image.sprite = resourceTypeSO.sprite;
        textMesh.text = "0";
    }

    public void UpdateAmount(int amount)
    {
        textMesh.text = amount.ToString();
    }

}