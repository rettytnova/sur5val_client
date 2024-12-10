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
        if (idx >= 4)
        {
            if (CardType.ExplorerWeapon <= data.cardType && data.cardType <= CardType.LegendaryGlove)
            {
                // ����, ����, ����, �尩 ī�� Ÿ���� ������ ��ȯ ��
                // %10 %5 �� �Ͽ� �ش� Ÿ���� ��� ��ȯ(���� ����)
                CardType cardType = data.cardType;
                int cardSlot = (int)cardType % 10 % 5;

                if (cardSlot == 2) // ����
                    idx = 0;
                else if (cardSlot == 3) // ����
                    idx = 1;
                else if (cardSlot == 4) // ����
                    idx = 2;
                else if (cardSlot == 0) // �尩
                    idx = 3;
            }
        }
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
                // �̹� ���� ������ ī�尡 �ִٸ� �ڽ����� �߰�
                var item = AddItem();
                item.transform.SetParent(cardStacks[cardKey].transform);
                item.Init(data, OnClickItem);

                // �θ��� �ڽ� ������ ���� ��ġ ���� (�ڱ� �ڽ� ����)
                float offset = GetCardComponentChildCount(item.transform.parent) * 15f;
                item.rectTransform.anchoredPosition = new Vector2(0, offset);
            }
            else
            {
                // ���ο� ������ ī���� listParent �Ʒ��� ����
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

        // ī�� ��� ���� �� ���� ���õ� �ε����� ��ȿ���� Ȯ���ϰ� ����
        if (uiPagingViewController.selectedIdx >= items.Count)
        {
            // ���� �ε����� ������ �� �����ϵ�, �ִ� ī�� ������ ���� �ʵ��� ����
            uiPagingViewController.selectedIdx = Mathf.Min(uiPagingViewController.selectedIdx, items.Count - 1);
            // ����¡ �� ��Ʈ�ѷ��� ������ �̵� ���
            uiPagingViewController.OnCalcMovePos(Vector2.zero, uiPagingViewController.selectedIdx);
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
        //     UIManager.ShowAlert("�������� ��� �Ͻðڽ��ϱ�?", "119 ȣ��", "������", "��ο���", () =>
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
            //if (!((CardType.BasicHpPotion <= card.cardType && card.cardType <= CardType.MasterExpPotion) || (CardType.ExplorerWeapon <= card.cardType && card.cardType <= CardType.LegendaryGlove)))
            //GameManager.instance.OnUseCard(card.rcode);
            GameManager.instance.OnUseCard(card.rcode, idx);
            SetList();
        }
    }

    // ����¡ �ε����� ���� ī�� �ε����� ��ȯ�ϴ� �޼���
    private int GetHandCardIndex(int pageIndex)
    {
        int actualIndex = 0;
        int visibleIndex = 0;

        foreach (var item in items)
        {
            if (item.transform.parent == listParent)  // �ֻ��� ī�常 Ȯ��
            {
                if (visibleIndex == pageIndex)
                {
                    // ���õ� ī�尡 �ڽ��� ������ �ִٸ�, ���ÿ��� �� ��° ī������ Ȯ��
                    int stackIndex = 0;
                    Transform currentTransform = item.transform;
                    while (currentTransform.childCount > 0)
                    {
                        // �ڽ� ������Ʈ �� Card ������Ʈ�� �ִ� �͸� Ȯ��
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

                    // �ε����� ������ ����� �ʵ��� Ȯ��
                    if (handCardIndex >= 0 && handCardIndex < UserInfo.myInfo.handCards.Count)
                    {
                        return handCardIndex;
                    }
                    else
                    {
                        Debug.LogWarning("Calculated handCardIndex is out of range.");
                        return 0; // �⺻�� ��ȯ
                    }
                }
                visibleIndex++;
            }
            actualIndex++;
        }

        Debug.LogWarning("PageIndex is out of range.");
        return 0; // �⺻�� ��ȯ
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