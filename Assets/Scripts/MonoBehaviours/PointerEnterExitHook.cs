using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerEnterExitHook : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{


    private Action onPointerEnterAction;
    private Action onPointerExitAction;


    public void Setup(Action onPointerEnterAction, Action onPointerExitAction)
    {
        this.onPointerEnterAction = onPointerEnterAction;
        this.onPointerExitAction = onPointerExitAction;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onPointerEnterAction?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onPointerExitAction?.Invoke();
    }

}