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
    [SerializeField] private List<Transform> inventorySlots;
    [SerializeField] private Transform weaponSlot;
    [SerializeField] private List<Transform> equipSlots;
    private bool UIState = false;
    public override void Opened(object[] param)
    {
        if(!UIState)
        {
            SetList();
            //if (GameManager.instance.userCharacter.IsState<CharacterPrisonState>() ||
            //    GameManager.instance.userCharacter.IsState<CharacterStopState>())
            //{
            //    use.gameObject.SetActive(false);
            //}
            //uiPagingViewController.OnChangeValue += OnChangeValue;
            uiPagingViewController.OnMoveStart += OnMoveStart;
            uiPagingViewController.OnMoveEnd += OnMoveEnd;
            UIGame.instance.OnSelectDirectTarget(false);
            UIState = true;
        } else gameObject.SetActive(true);
    }

    private void Update()
    {
        //if(GameManager.instance.userCharacter.IsState<CharacterIdleState>() ||
        //    GameManager.instance.userCharacter.IsState<CharacterWalkState>())
        //{
        //    use.gameObject.SetActive(true); // nullRef 오류 뜰수도 있음
        //}

    }

    public void OnChangeValue(int idx)
    {
        //use.interactable = UserInfo.myInfo.handCards[idx].isUsable;
    }

    public override void HideDirect()
    {
        gameObject.SetActive(false);
        //UIGame.instance.SetDeckCount();
        //UIManager.Hide<PopupDeck>();
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
        for(int i = 0; i < equipSlots.Count; i++)
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

        // 빵 게임 원본 코드
        //var datas = UserInfo.myInfo.handCards;
        //foreach (var data in datas)
        //{            
        //    var item = AddItem();
        //    item.Init(data, OnClickItem);
        //}

        // 아이템 인벤토리 추가 부분
        var handCards = UserInfo.myInfo.handCards;
        for(int i = 0; i < UserInfo.myInfo.handCards.Count; i++)
        {
            var slot = inventorySlots[i];
            var item = Instantiate(itemPrefab, slot);
            item.Init(handCards[i], OnClickItem);
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
        var idx = uiPagingViewController.selectedIdx;
        var card = UserInfo.myInfo.handCards[idx];

        // 스킬일 경우 카드 사용 요청
        if (card.rcode == "CAD00001" || card.rcode == "CAD00002" || card.rcode == "CAD00003" || card.rcode == "CAD00004" || card.rcode == "CAD00005" || card.rcode == "CAD00006" ||
            card.rcode == "CAD00007" || card.rcode == "CAD00008" || card.rcode == "CAD00009" || card.rcode == "CAD00010" || card.rcode == "CAD00011" || card.rcode == "CAD00012")
        {
            //UserInfo.myInfo.OnUseCard(idx);
            GameManager.instance.SendSocketUseCard(UserInfo.myInfo, UserInfo.myInfo, card.rcode);
        }
        else // 장비 소모품일 경우 카드 사용 요청
        {
            GameManager.instance.SendSocketUseCard(null, UserInfo.myInfo, card.rcode);
        }
        //else
        //{
        //    OnUseCard();
        //}
    }

    public void OnUseCard()
    {
        var idx = uiPagingViewController.selectedIdx;
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
}