using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIUseSkill : MonoBehaviour
{
    public Transform skillObject = null;
    [SerializeField] private Button myButton;

    private void Start()
    {
        // 버튼에 클릭 이벤트 연결
        myButton.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        // 스킬 슬롯에 스킬이 있으면 스킬 사용
        if (skillObject != null)
        {
            Transform skillName = skillObject.Find("cardName");
            TMP_Text skillNameTextComponent = skillName.GetComponent<TMP_Text>();
            UseSkill(skillNameTextComponent.text);
        }
        else // 스킬 슬롯에 스킬이 없음
        {
            Debug.Log("스킬이 없습니다.");
        }
    }

    // 스킬 사용 후 로직
    private void UseSkill(string skillName)
    {
        switch (skillName)
        {
            case "전사 공격 스킬":
                Debug.Log("전사 공격 스킬 사용");
                break;
            case "전사 방어 스킬":
                Debug.Log("전사 방어 스킬 사용");
                break;
            case "전사 특수 스킬":
                Debug.Log("전사 특수 스킬 사용");
                break;
            case "로그 공격 스킬":
                Debug.Log("로그 공격 스킬 사용");
                break;
            case "로그 이동 스킬":
                Debug.Log("로그 이동 스킬 사용");
                break;
            case "로그 특수 스킬":
                Debug.Log("로그 특수 스킬 사용");
                break;
            case "법사 공격 스킬1":
                Debug.Log("법사 공격 스킬1 사용");
                break;
            case "법사 공격 스킬2":
                Debug.Log("법사 공격 스킬2 사용");
                break;
            case "법사 특수 스킬":
                Debug.Log("법사 특수 스킬 사용");
                break;
            case "서폿 공격 스킬":
                Debug.Log("서폿 공격 스킬 사용");
                break;
            case "서폿 서포팅 스킬":
                Debug.Log("서포팅 서포팅 스킬 사용");
                break;
            case "특수 특수 스킬":
                Debug.Log("특수 특수 스킬 사용");
                break;
            default:
                Debug.Log("알 수 없는 스킬 사용");
                break;
        }
    }
}
