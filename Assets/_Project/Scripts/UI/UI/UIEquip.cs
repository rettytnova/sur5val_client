using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIEquip : MonoBehaviour
{
    public Transform equipObject = null;
    public void OnEquip(Transform equipObject)
    {
        // 장비 슬롯에 장비가 있으면 장비 장착
        if (equipObject != null)
        {
            Transform equipName = equipObject.Find("cardName");
            TMP_Text equipNameTextComponent = equipName.GetComponent<TMP_Text>();
            UseEquip(equipNameTextComponent.text);
        }
        else // 장비 슬롯에 장비가 없음
        {
            Debug.Log("장비가 없습니다.");
        }
    }

    // 장비 장착 후 로직
    private void UseEquip(string equipName)
    {
        switch (equipName)
        {
            case "검":
                Debug.Log("검 장착");
                break;
            case "방패":
                Debug.Log("방패 장착");
                break;
            case "활":
                Debug.Log("활 장착");
                break;
            case "스태프":
                Debug.Log("스태프 장착");
                break;
            case "판금 갑옷":
                Debug.Log("판금 갑옷 장착");
                break;
            case "가죽 갑옷":
                Debug.Log("가죽 갑옷 장착");
                break;
            case "로브":
                Debug.Log("로브 장착");
                break;
            case "전설 갑옷":
                Debug.Log("전설 갑옷 장착");
                break;
            default:
                Debug.Log("알 수 없는 장비 장착");
                break;
        }
    }
}
