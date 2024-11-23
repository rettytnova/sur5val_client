using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPotion : MonoBehaviour
{    
    public Transform potionObject = null;
    [SerializeField] private Button myButton;

    private void Start()
    {
        // 버튼에 클릭 이벤트 연결
        myButton.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        // 포션 슬롯에 포션이 있으면 포션 사용
        if (potionObject != null)
        {
            Transform potionName = potionObject.Find("cardName");
            TMP_Text potionNameTextComponent = potionName.GetComponent<TMP_Text>();
            UsePotion(potionNameTextComponent.text);

            // 포션 오브젝트 삭제
            Destroy(potionObject.gameObject);
            potionObject = null;
        }
        else // 포션 슬롯에 포션이 없음
        {
            Debug.Log("포션이 없습니다.");
        }
    }

    // 포션 사용 후 로직
    private void UsePotion(string potionName)
    {
        switch (potionName)
        {
            case "체력 회복 포션":
                Debug.Log("캐릭터 체력 회복");
                break;
            case "마나 회복 포션":
                Debug.Log("캐릭터 마나 회복");
                break;
            case "상급 회복 포션":
                Debug.Log("캐릭터 상급 체력 회복");
                break;
            default:
                Debug.Log("알 수 없는 포션");
                break;
        }
    }
}
