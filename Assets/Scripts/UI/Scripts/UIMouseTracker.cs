using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIMouseTracker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Action<bool> OnMouseOverUIChanged;

    public void Initialize(Action<bool> mouseChanged)
    {
        OnMouseOverUIChanged = mouseChanged;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnMouseOverUIChanged?.Invoke(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnMouseOverUIChanged?.Invoke(false);
    }
}