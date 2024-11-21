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
        if (eventData.pointerDrag != null)
        {
            // Card(Script)를 가져옴 이미지 숫자를 알기 위함
            Card cardScript = eventData.pointerDrag.GetComponent<Card>();
            DraggableUI draggableUI = eventData.pointerDrag.GetComponent<DraggableUI>();

            // 드랍 했을때 카테고리가 있는 경우
            if (cardScript != null)
            {
                // Card(Script)에서 Rcode를 가져옴
                string rcode = cardScript.cardData.rcode;

                // Rcode에서 "CAD" 뒤의 숫자를 추출
                string parentCategory = "";
                if (rcode.StartsWith("CAD"))
                {                    
                    int rcodeNumber = int.Parse(rcode.Substring(3));

                    if (rcodeNumber >= 1 && rcodeNumber <= 12)
                    {
                        parentCategory = "Skill";// 스킬
                    }
                    else if (rcodeNumber >= 13 && rcodeNumber <= 20)
                    {
                        parentCategory = "Slots";// 무기 및 장비
                    }
                    else if (rcodeNumber >= 21 && rcodeNumber <= 23)
                    {
                        parentCategory = "Expendables";// 소모품
                    }
                }

                // 카테고리가 맞는지 확인
                if (transform.parent.name == parentCategory || transform.parent.name == "HandCards")
                {
                    // 드랍할 슬롯에 이미 아이템이 있는지 확인
                    if (transform.childCount > 0)
                    {
                        // 슬롯에 이미 있는 아이템
                        Transform existingItem = transform.GetChild(0);                      

                        // 드랍 위치 아이템의 부모를 드래그 아이템의 부모로 설정 후 이동
                        existingItem.SetParent(draggableUI.previousParent);
                        existingItem.transform.localPosition = Vector3.zero;

                        // 드래그된 아이템의 부모를 드랍 위치 슬롯으로 설정 후 이동
                        eventData.pointerDrag.transform.SetParent(transform);
                        eventData.pointerDrag.transform.localPosition = Vector3.zero;
                    }
                    else
                    {
                        // 슬롯이 비어 있으면 부모 설정 후 이동
                        eventData.pointerDrag.transform.SetParent(transform);
                        eventData.pointerDrag.transform.localPosition = Vector3.zero;
                    }

                    // 슬롯에 드랍한 카드가 소모품일 경우
                    if(parentCategory == "Expendables")
                    {
                        UIPotion uIPotion = eventData.pointerDrag.transform.parent.parent.parent.parent.parent.Find("UI").Find("UIGame").GetComponent<UIPotion>();
                        uIPotion.potionObject = eventData.pointerDrag.transform; // nullRef 오류 뜰수도 있음
                    }                    
                }
                else // 카테고리가 맞지 않는 경우
                {
                    // 드래그된 아이템을 원래 위치로 복원
                    eventData.pointerDrag.transform.SetParent(draggableUI.previousParent);
                    eventData.pointerDrag.transform.localPosition = Vector3.zero;
                }
            }
            else // 드랍 했을때 카테고리가 없는 경우
            {
                if (draggableUI != null)
                {
                    // 드래그된 아이템을 원래 위치로 복원
                    eventData.pointerDrag.transform.SetParent(draggableUI.previousParent);
                    eventData.pointerDrag.transform.localPosition = Vector3.zero;
                }
            }
        }
    }

}
