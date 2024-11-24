using Ironcow;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropCard : MonoBehaviour, IPointerEnterHandler, IDropHandler, IPointerExitHandler
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
        // 드래그 위치의 카드와 드랍 위치의 카드에 담을 변수 초기화
        GameObject dragPosCard = eventData.pointerDrag;
        Transform dropPosCard = null;

        // 카드 정보 가져오기 ----------------------------------------------------------------------                        
        // 드래그 위치의 카드 정보 가져오기
        Card dragCardScript = dragPosCard.GetComponent<Card>();
        DragCard dragCard = dragPosCard.GetComponent<DragCard>();

        // 드래그 위치의 카드와 드랍 위치의 카드 카테고리 가져오기        
        string dragCardCategory = "";
        string dropCardCategory = transform.parent.name;
        string rcode = dragCardScript.cardData.rcode;
        if (rcode.StartsWith("CAD"))
        {
            int rcodeNumber = int.Parse(rcode.Substring(3));

            if (rcodeNumber >= 1 && rcodeNumber <= 12)
            {
                dragCardCategory = "Skill";// 스킬
            }
            else if (rcodeNumber >= 13 && rcodeNumber <= 20)
            {
                dragCardCategory = "Slots";// 무기 및 장비
            }
            else if (rcodeNumber >= 21 && rcodeNumber <= 23)
            {
                dragCardCategory = "Potion";// 소모품
            }
        }

        // 카드의 위치 이동 ----------------------------------------------------------------------                        
        // 드랍 위치가 드래그 카드와 카테고리가 맞는 경우
        if (dropCardCategory == dragCardCategory || dropCardCategory == "HandCards")
        {            
            // 드랍할 슬롯에 이미 아이템이 있는지 확인
            if (transform.childCount > 0)
            {
                dropPosCard = transform.GetChild(0);

                // 드랍 위치의 카드를 드래그 위치의 카드로 이동
                dropPosCard.SetParent(dragCard.previousParent);
                dropPosCard.transform.localPosition = Vector3.zero;
            }

            // 드래그 위치의 카드를 드랍 위치로 이동
            dragPosCard.transform.SetParent(transform);
            dragPosCard.transform.localPosition = Vector3.zero;

            // 클라이언트 데이터 이동 필요

            // 카드 데이터 이동 필요
        }
        else // 드랍 위치가 드래그 카드와 카테고리가 다를 경우
        {
            // 드래그된 아이템을 원래 위치로 복원
            dragPosCard.transform.SetParent(dragCard.previousParent);
            dragPosCard.transform.localPosition = Vector3.zero;
        }
    }
}
