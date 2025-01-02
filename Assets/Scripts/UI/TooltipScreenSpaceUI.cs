using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TooltipScreenSpaceUI : MonoBehaviour
{

    public static TooltipScreenSpaceUI Instance { get; private set; }



    [SerializeField] private RectTransform canvasRectTransform;

    private RectTransform backgroundRectTransform;
    private TextMeshProUGUI textMeshPro;
    private RectTransform rectTransform;


    private System.Func<string> getTooltipTextFunc;
    private float? showTimer;


    private void Awake()
    {
        Instance = this;

        backgroundRectTransform = transform.Find("background").GetComponent<RectTransform>();
        textMeshPro = transform.Find("text").GetComponent<TextMeshProUGUI>();
        rectTransform = transform.GetComponent<RectTransform>();


        HideTooltip();
    }

    private void SetText(string tooltipText)
    {
        textMeshPro.SetText(tooltipText);
        textMeshPro.ForceMeshUpdate();

        Vector2 textSize = textMeshPro.GetRenderedValues(false);
        Vector2 paddingSize = new Vector2(8, 8);

        backgroundRectTransform.sizeDelta = textSize + paddingSize;
    }

    private void Update()
    {
        SetText(getTooltipTextFunc());

        PositionTooltip();

        if (showTimer != null)
        {
            showTimer -= Time.deltaTime;
            if (showTimer <= 0)
            {
                HideTooltip();
            }
        }
    }

    private void PositionTooltip()
    {
        Vector2 anchoredPosition = Input.mousePosition / canvasRectTransform.localScale.x;

        if (anchoredPosition.x + backgroundRectTransform.rect.width > canvasRectTransform.rect.width)
        {
            // Tooltip left screen on right side
            anchoredPosition.x = canvasRectTransform.rect.width - backgroundRectTransform.rect.width;
        }
        if (anchoredPosition.y + backgroundRectTransform.rect.height > canvasRectTransform.rect.height)
        {
            // Tooltip left screen on top side
            anchoredPosition.y = canvasRectTransform.rect.height - backgroundRectTransform.rect.height;
        }

        rectTransform.anchoredPosition = anchoredPosition;
    }

    private void ShowTooltip(string tooltipText, float? showTimer)
    {
        ShowTooltip(() => tooltipText, showTimer);
    }

    private void ShowTooltip(System.Func<string> getTooltipTextFunc, float? showTimer)
    {
        this.getTooltipTextFunc = getTooltipTextFunc;
        this.showTimer = showTimer;
        gameObject.SetActive(true);
        SetText(getTooltipTextFunc());
        PositionTooltip();
    }

    private void HideTooltip()
    {
        gameObject.SetActive(false);
    }

    public static void ShowTooltip_Static(string tooltipText, float? showTimer)
    {
        Instance.ShowTooltip(tooltipText, showTimer);
    }

    public static void ShowTooltip_Static(System.Func<string> getTooltipTextFunc, float? showTimer)
    {
        Instance.ShowTooltip(getTooltipTextFunc, showTimer);
    }

    public static void HideTooltip_Static()
    {
        Instance.HideTooltip();
    }


}
