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
        // �巡�� ��ġ�� ī��� ��� ��ġ�� ī�忡 ���� ���� �ʱ�ȭ
        GameObject dragPosCard = eventData.pointerDrag;
        Transform dropPosCard = null;

        // ī�� ���� �������� ----------------------------------------------------------------------                        
        // �巡�� ��ġ�� ī�� ���� ��������
        Card dragCardScript = dragPosCard.GetComponent<Card>();
        DragCard dragCard = dragPosCard.GetComponent<DragCard>();

        // �巡�� ��ġ�� ī��� ��� ��ġ�� ī�� ī�װ� ��������        
        string dragCardCategory = "";
        string dropCardCategory = transform.parent.name;
        string rcode = dragCardScript.cardData.rcode;
        if (rcode.StartsWith("CAD"))
        {
            int rcodeNumber = int.Parse(rcode.Substring(3));

            if (rcodeNumber >= 1 && rcodeNumber <= 12)
            {
                dragCardCategory = "Skill";// ��ų
            }
            else if (rcodeNumber >= 13 && rcodeNumber <= 20)
            {
                dragCardCategory = "Slots";// ���� �� ���
            }
            else if (rcodeNumber >= 21 && rcodeNumber <= 23)
            {
                dragCardCategory = "Potion";// �Ҹ�ǰ
            }
        }

        // ī���� ��ġ �̵� ----------------------------------------------------------------------                        
        // ��� ��ġ�� �巡�� ī��� ī�װ��� �´� ���
        if (dropCardCategory == dragCardCategory || dropCardCategory == "HandCards")
        {            
            // ����� ���Կ� �̹� �������� �ִ��� Ȯ��
            if (transform.childCount > 0)
            {
                dropPosCard = transform.GetChild(0);

                // ��� ��ġ�� ī�带 �巡�� ��ġ�� ī��� �̵�
                dropPosCard.SetParent(dragCard.previousParent);
                dropPosCard.transform.localPosition = Vector3.zero;
            }

            // �巡�� ��ġ�� ī�带 ��� ��ġ�� �̵�
            dragPosCard.transform.SetParent(transform);
            dragPosCard.transform.localPosition = Vector3.zero;

            // Ŭ���̾�Ʈ ������ �̵� �ʿ�

            // ī�� ������ �̵� �ʿ�
        }
        else // ��� ��ġ�� �巡�� ī��� ī�װ��� �ٸ� ���
        {
            // �巡�׵� �������� ���� ��ġ�� ����
            dragPosCard.transform.SetParent(dragCard.previousParent);
            dragPosCard.transform.localPosition = Vector3.zero;
        }
    }
}
