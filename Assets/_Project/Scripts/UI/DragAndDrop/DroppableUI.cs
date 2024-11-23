using Unity.VisualScripting;
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
        Transform dropPosItem = null;
        GameObject dragPosItem = eventData.pointerDrag;

        // 드래그 카드가 null이 아닐경우
        if (dragPosItem != null)
        {
            // 카드 정보 가져오기 ----------------------------------------------------------------------                        
            // Card(Script)를 가져옴 카드의 정보를 알기 위함
            Card cardScript = dragPosItem.GetComponent<Card>();
            DraggableUI draggableUI = dragPosItem.GetComponent<DraggableUI>();

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

                // 카드의 위치 이동 ----------------------------------------------------------------------                        
                // 카테고리가 맞는지 확인
                if (transform.parent.name == parentCategory || transform.parent.name == "HandCards")
                {
                    // 드랍할 슬롯에 이미 아이템이 있는지 확인
                    if (transform.childCount > 0)
                    {
                        dropPosItem = transform.GetChild(0);

                        // 클라이언트 카드 위치 이동 ----------------------------------------------------------------------                        
                        // 드랍 위치의 카드를 드래그 위치의 카드로 이동
                        dropPosItem.SetParent(draggableUI.previousParent);
                        dropPosItem.transform.localPosition = Vector3.zero;

                        // 드래그 위치의 카드를 드랍 위치의 카드로 이동
                        dragPosItem.transform.SetParent(transform);
                        dragPosItem.transform.localPosition = Vector3.zero;
                    }
                    else
                    {
                        // 클라이언트 카드 위치 이동 ----------------------------------------------------------------------
                        // 슬롯이 비어 있으면 부모 설정 후 이동
                        dragPosItem.transform.SetParent(transform);
                        dragPosItem.transform.localPosition = Vector3.zero;
                    }

                    // 카드 데이터 이동 ----------------------------------------------------------------------
                    if (parentCategory == "Skill")
                    {
                        // 드랍 위치의 카드를 드래그 위치의 카드로 수정
                        Card pointerDragCardScript = dragPosItem.GetComponent<Card>();
                        UserInfo.myInfo.weapon = pointerDragCardScript.cardData;
                    }
                    else if (parentCategory == "Slots")
                    {
                        //// 드랍 위치의 카드를 드래그 위치의 카드로 수정
                        //Card pointerDragCardScript = dragPosItem.GetComponent<Card>();
                        //UserInfo.myInfo.weapon = pointerDragCardScript.cardData;
                    }
                    else if (parentCategory == "Expendables")
                    {
                        // 드랍 위치의 카드를 드래그 위치의 카드로 수정
                        Card pointerDragCardScript = dragPosItem.GetComponent<Card>();
                        UserInfo.myInfo.potion = pointerDragCardScript.cardData;
                    }
                    else if (parentCategory == "HandCards")
                    {
                        CardDataSO dragCard = null;
                        CardDataSO dropCard = null;

                        // 드랍 위치의 카드를 드래그 위치의 카드로 수정
                        for (int i = 0; i < UserInfo.myInfo.handCards.Count; i++)
                        {
                            if (UserInfo.myInfo.handCards[i] != null && UserInfo.myInfo.handCards[i].name == dragPosItem.name)
                                dragCard = UserInfo.myInfo.handCards[i];
                            else if (UserInfo.myInfo.handCards[i] != null && UserInfo.myInfo.handCards[i].name == dropPosItem.name)
                                dropCard = UserInfo.myInfo.handCards[i];
                        }

                        CardDataSO tempCard = dragCard;
                        dragCard = dropCard;
                        dropCard = tempCard;
                    }

                    // 드래그 위치의 카드를 드랍 위치의 카드로 수정
                    for (int i = 0; i < UserInfo.myInfo.handCards.Count; i++)
                    {
                        if (UserInfo.myInfo.handCards[i] != null && UserInfo.myInfo.handCards[i].name == dragPosItem.name)
                        {
                            // 드랍 위치의 카드 정보 가져오기
                            Card droPosCardScript = dropPosItem.GetComponent<Card>();

                            // 드랍 위치의 카드 정보를 드래그 위치의 카드 정보로 업데이트
                            UserInfo.myInfo.handCards[i] = droPosCardScript.cardData;
                        }
                    }

                    // 카드 효과 적용 ----------------------------------------------------------------------
                    // 슬롯에 드랍한 카드가 스킬일 경우
                    if (parentCategory == "Skill")
                    {
                        UIUseSkill uISkill = dragPosItem.transform.parent.parent.parent.parent.parent.Find("UI").Find("UIGame").GetComponent<UIUseSkill>();
                        uISkill.skillObject = dragPosItem.transform; // nullRef 오류 뜰수도 있음

                    }
                    // 슬롯에 드랍한 카드가 장비일 경우
                    else if (parentCategory == "Slots")
                    {
                        UIEquip uIEquip = dragPosItem.transform.parent.parent.parent.parent.parent.parent.Find("UI").Find("UIGame").GetComponent<UIEquip>();
                        uIEquip.equipObject = dragPosItem.transform; // nullRef 오류 뜰수도 있음
                        uIEquip.OnEquip(dragPosItem.transform);
                        PopupDeck popupDeck = dragPosItem.transform.parent.parent.parent.parent.parent.Find("PopupDeck").GetComponent<PopupDeck>();
                        popupDeck.OnClickUse();
                    }
                    // 슬롯에 드랍한 카드가 소모품일 경우
                    else if (parentCategory == "Expendables")
                    {
                        UIPotion uIPotion = dragPosItem.transform.parent.parent.parent.parent.parent.Find("UI").Find("UIGame").GetComponent<UIPotion>();
                        uIPotion.potionObject = dragPosItem.transform; // nullRef 오류 뜰수도 있음
                    }
                }
                else // 카테고리가 맞지 않는 경우
                {
                    // 드래그된 아이템을 원래 위치로 복원
                    dragPosItem.transform.SetParent(draggableUI.previousParent);
                    dragPosItem.transform.localPosition = Vector3.zero;
                }
            }
            else 
            {
                // 드랍 했을때 카테고리가 없는 경우 == 슬롯에 드랍을 하지않은 경우
                if (draggableUI != null)
                {
                    // 드래그된 아이템을 원래 위치로 복원
                    dragPosItem.transform.SetParent(draggableUI.previousParent);
                    dragPosItem.transform.localPosition = Vector3.zero;
                }
            }

            // 서버에 데이터 업데이트 보내기
        }
    }
}
