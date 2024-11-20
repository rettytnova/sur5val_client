using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropableUI : MonoBehaviour, IPointerEnterHandler, IDropHandler, IPointerExitHandler
{
    private Image image;
    private RectTransform rect;

    private void Awake()
    {
        image = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = Color.yellow;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = Color.white;
    }

    public void OnDrop(PointerEventData eventData)
    {
        // 현재 슬롯에 이미 아이템이 있는지 확인
        if (transform.childCount > 0)
        {
            // 슬롯에 이미 있는 아이템
            Transform existingItem = transform.GetChild(0);         

            // 기존 아이템의 부모를 드래그된 아이템의 원래 부모로 설정
            existingItem.SetParent(eventData.pointerDrag.transform.parent);            
            existingItem.transform.localPosition = Vector3.zero;

            // 드래그된 아이템의 부모를 현재 슬롯으로 설정
            eventData.pointerDrag.transform.SetParent(transform);
            eventData.pointerDrag.transform.localPosition = Vector3.zero;
        }
        else
        {
            // 슬롯이 비어 있으면 드래그된 아이템만 추가
            eventData.pointerDrag.transform.SetParent(transform);
            eventData.pointerDrag.transform.localPosition = Vector3.zero;
        }
    }
}
