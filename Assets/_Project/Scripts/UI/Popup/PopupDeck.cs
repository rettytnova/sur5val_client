using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ironcow;
using UnityEngine.UI;
using TMPro;

public class PopupDeck : UIListBase<Card>
{
    [SerializeField] private UIPagingViewController uiPagingViewController;
    [SerializeField] private GameObject select;
    [SerializeField] private Button use;
    [SerializeField] private Transform weaponSlot;
    [SerializeField] private List<Transform> equipSlots;
    public override void Opened(object[] param)
    {
        SetList();
        if (GameManager.instance.userCharacter.IsState<CharacterPrisonState>() ||
            GameManager.instance.userCharacter.IsState<CharacterStopState>())
        {
            use.gameObject.SetActive(false);
        }
        uiPagingViewController.OnChangeValue += OnChangeValue;
        uiPagingViewController.OnMoveStart += OnMoveStart;
        uiPagingViewController.OnMoveEnd += OnMoveEnd;
        UIGame.instance.OnSelectDirectTarget(false);
    }

    private void Update()
    {
        if (GameManager.instance.userCharacter.IsState<CharacterIdleState>() ||
            GameManager.instance.userCharacter.IsState<CharacterWalkState>())
        {
            use.gameObject.SetActive(true);
        }

    }

    public void OnChangeValue(int idx)
    {
        use.interactable = UserInfo.myInfo.handCards[idx].isUsable;
    }

    public override void HideDirect()
    {
        UIGame.instance.SetDeckCount();
        UIManager.Hide<PopupDeck>();
    }

    public void ClearWeapon()
    {
        if (weaponSlot.childCount > 0)
            Destroy(weaponSlot.GetChild(0).gameObject);
    }

    public void AddWeapon(CardDataSO data)
    {
        var item = Instantiate(itemPrefab, weaponSlot);
        item.Init(data);
        item.rectTransform.anchoredPosition = Vector2.zero;
        item.rectTransform.sizeDelta = new Vector2(225, 300);
    }

    public void ClearEquips()
    {
        for (int i = 0; i < equipSlots.Count; i++)
        {
            if (equipSlots[i].childCount > 0)
                Destroy(equipSlots[i].GetChild(0).gameObject);
        }
    }

    public void AddEquip(CardDataSO data, int idx)
    {
        var slot = equipSlots[idx];
        var item = Instantiate(itemPrefab, slot);
        item.Init(data);
        item.rectTransform.anchoredPosition = Vector2.zero;
        item.rectTransform.sizeDelta = new Vector2(225, 300);
    }

    public override void SetList()
    {
        ClearList();
        ClearEquips();
        ClearWeapon();

        var datas = UserInfo.myInfo.handCards;
        Dictionary<string, Card> cardStacks = new Dictionary<string, Card>();

        foreach (var data in datas)
        {
            string cardKey = data.cardType.ToString();

            if (cardStacks.ContainsKey(cardKey))
            {
                // 이미 같은 종류의 카드가 있다면 자식으로 추가
                var item = AddItem();
                item.transform.SetParent(cardStacks[cardKey].transform);
                item.Init(data, OnClickItem);

                // 부모의 자식 개수에 따라 위치 조정 (자기 자신 포함)
                float offset = GetCardComponentChildCount(item.transform.parent) * 15f;
                item.rectTransform.anchoredPosition = new Vector2(0, offset);
            }
            else
            {
                // 새로운 종류의 카드라면 listParent 아래에 생성
                var item = AddItem();
                item.Init(data, OnClickItem);
                cardStacks.Add(cardKey, item);
            }
        }

        if (UserInfo.myInfo.weapon != null)
        {
            AddWeapon(UserInfo.myInfo.weapon);
        }

        for (int i = 0; i < UserInfo.myInfo.equips.Count; i++)
        {
            AddEquip(UserInfo.myInfo.equips[i], i);
        }
    }

    public void OnClickItem(CardDataSO data)
    {

    }

    private void OnMoveStart()
    {
        select.SetActive(false);
    }

    private void OnMoveEnd()
    {
        select.SetActive(true);
    }

    public void OnClickUse()
    {
        var idx = GetHandCardIndex(uiPagingViewController.selectedIdx);
        // var card = UserInfo.myInfo.handCards[idx];
        // if (card.rcode == "CAD00005")
        // {
        //     UIManager.ShowAlert("누구에게 사용 하시겠습니까?", "119 호출", "나에게", "모두에게", () =>
        //     {
        //         UserInfo.myInfo.OnUseCard(idx);
        //         GameManager.instance.SendSocketUseCard(UserInfo.myInfo, UserInfo.myInfo, card.rcode);
        //     }, () =>
        //     {
        //         UserInfo.myInfo.OnUseCard(idx);
        //         GameManager.instance.SendSocketUseCard(null, UserInfo.myInfo, card.rcode);
        //     });
        // }
        // else
        // {
        //     OnUseCard();
        // }
        OnUseCard();
    }

    public void OnUseCard()
    {
        var idx = GetHandCardIndex(uiPagingViewController.selectedIdx);
        var card = UserInfo.myInfo.OnUseCard(idx);
        if (card.isTargetSelect || (UserInfo.myInfo.isSniper && card.cardType == CardType.Bbang))
        {
            UIGame.instance.OnSelectDirectTarget(true);
        }
        if (!card.isDirectUse)
        {
            HideDirect();
        }
        else
        {
            GameManager.instance.OnUseCard(card.rcode);
            SetList();
        }
    }

    // 페이징 인덱스를 실제 카드 인덱스로 변환하는 메서드
    private int GetHandCardIndex(int pageIndex)
    {
        int actualIndex = 0;
        int visibleIndex = 0;

        foreach (var item in items)
        {
            if (item.transform.parent == listParent)  // 최상위 카드만 확인
            {
                if (visibleIndex == pageIndex)
                {
                    // 선택된 카드가 자식을 가지고 있다면, 스택에서 몇 번째 카드인지 확인
                    int stackIndex = 0;
                    Transform currentTransform = item.transform;
                    while (currentTransform.childCount > 0)
                    {
                        // 자식 오브젝트 중 Card 컴포넌트가 있는 것만 확인
                        Transform childCard = null;
                        for (int i = 0; i < currentTransform.childCount; i++)
                        {
                            if (currentTransform.GetChild(i).GetComponent<Card>() != null)
                            {
                                childCard = currentTransform.GetChild(i);
                                break;
                            }
                        }

                        if (childCard == null) break;

                        stackIndex++;
                        currentTransform = childCard;
                    }
                    int handCardIndex = actualIndex + stackIndex;

                    // 인덱스가 범위를 벗어나지 않도록 확인
                    if (handCardIndex >= 0 && handCardIndex < UserInfo.myInfo.handCards.Count)
                    {
                        return handCardIndex;
                    }
                    else
                    {
                        Debug.LogWarning("Calculated handCardIndex is out of range.");
                        return 0; // 기본값 반환
                    }
                }
                visibleIndex++;
            }
            actualIndex++;
        }

        Debug.LogWarning("PageIndex is out of range.");
        return 0; // 기본값 반환
    }

    int GetCardComponentChildCount(Transform parent)
    {
        int count = 0;
        for (int i = 0; i < parent.childCount; i++)
        {
            if (parent.GetChild(i).GetComponent<Card>() != null)
            {
                count++;
            }
        }
        return count;
    }
}