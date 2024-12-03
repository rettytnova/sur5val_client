using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIEventHandler : MonoBehaviour, IPointerExitHandler,
    IPointerEnterHandler, IPointerClickHandler, IBeginDragHandler,
    IDragHandler, IEndDragHandler, IDropHandler, IScrollHandler
{
    public Action<PointerEventData> OnPointerEnterHandler = null;
    public Action<PointerEventData> OnPointerExitHandler = null;
    public Action<PointerEventData> OnClickHandler = null;
    public Action<PointerEventData> OnBeginDragHandler = null;
    public Action<PointerEventData> OnDragHandler = null;
    public Action<PointerEventData> OnEndDragHandler = null;
    public Action<PointerEventData> OnDropHandler = null;
    public Action<PointerEventData> OnScrollHandler = null;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (OnPointerEnterHandler != null)
        {
            OnPointerEnterHandler.Invoke(eventData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (OnPointerExitHandler != null)
        {
            OnPointerExitHandler.Invoke(eventData);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnClickHandler != null)
        {
            OnClickHandler.Invoke(eventData);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (OnBeginDragHandler != null)
        {
            OnBeginDragHandler.Invoke(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (OnDragHandler != null)
        {
            OnDragHandler.Invoke(eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (OnEndDragHandler != null)
        {
            OnEndDragHandler.Invoke(eventData);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (OnDropHandler != null)
        {
            OnDropHandler.Invoke(eventData);
        }
    }

    public void OnScroll(PointerEventData eventData)
    {
        if (OnScrollHandler != null)
        {
            OnScrollHandler.Invoke(eventData);
        }
    }
}
