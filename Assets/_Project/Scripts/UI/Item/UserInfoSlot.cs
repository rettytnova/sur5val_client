using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ironcow;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System.Threading.Tasks;
using System.Xml.Linq;

public class UserInfoSlot : UIListItem
{
    [SerializeField] private Image thumbnail;
    [SerializeField] private TMP_Text nickname;
    [SerializeField] private List<GameObject> hpSlots;
    [SerializeField] private List<GameObject> hpGauges;
    [SerializeField] private Image hpGauge;
    [SerializeField] private TMP_Text hpGaugeText;
    [SerializeField] private TMP_Text mpTxt;
    [SerializeField] private TMP_Text attackTxt;
    [SerializeField] private TMP_Text armorTxt;
    [SerializeField] private TMP_Text goldTxt;    
    [SerializeField] private TMP_Text levelTxt;
    [SerializeField] private Image expGauge;
    [SerializeField] private TMP_Text expGaugeText;
    [SerializeField] private GameObject targetMark;
    [SerializeField] private TMP_Text index;
    [SerializeField] private Image weapon;
    [SerializeField] private List<Image> equips;
    [SerializeField] private List<Image> debuffs;
    [SerializeField] private GameObject select;
    [SerializeField] private GameObject death;

    public int idx;
    public UnityAction<int> callback;
    public bool isDeath { get => death.activeInHierarchy; }

    public async Task Init(UserInfo userinfo, int index, UnityAction<int> callback)
    {
        this.idx = index;
        this.callback = callback;
        nickname.text = userinfo.nickname;
        var data = DataManager.instance.GetData<CharacterDataSO>(userinfo.selectedCharacterRcode);
        thumbnail.sprite = await ResourceManager.instance.LoadAsset<Sprite>(data.rcode, eAddressableType.Thumbnail);
        targetMark.GetComponent<Image>().sprite = await ResourceManager.instance.LoadAsset<Sprite>("role_" + userinfo.roleType.ToString(), eAddressableType.Thumbnail);
        // 체력 바        
        SetHpGauge(userinfo.hp, userinfo.maxHp);

        if (UserInfo.myInfo.id == userinfo.id)
        {
            // 내 캐릭터일 경우 공격력, 방어력, 골드, 레벨, 경험치 표시
            mpTxt.text = userinfo.mp.ToString();
            attackTxt.text = userinfo.attack.ToString();
            armorTxt.text = userinfo.armor.ToString();
            goldTxt.text = userinfo.gold.ToString();
            levelTxt.text = userinfo.level.ToString();
            SetExpGauge(userinfo.exp, userinfo.maxExp);
        }
        // for (int i = 0; i < 5; i++)
        // {
        //     hpSlots[i].SetActive(userinfo.hp > i);
        //     hpGauges[i].SetActive(userinfo.hp > i);
        // }
        targetMark.SetActive(userinfo.roleType == eRoleType.target);
        this.index.text = (index + 1).ToString();
        gameObject.SetActive(true);
        if (weapon != null)
        {
            weapon.gameObject.SetActive(false);
        }
        if (equips.Count > 0)
        {
            equips.ForEach(obj => obj.gameObject.SetActive(false));
        }
        if (debuffs.Count > 0)
        {
            debuffs.ForEach(obj => obj.gameObject.SetActive(false));
        }
    }

    public async void UpdateData(UserInfo userinfo)
    {
        // for (int i = 0; i < 5; i++)
        // {
        //     hpGauges[i].SetActive(userinfo.hp > i);
        // }
        SetHpGauge(userinfo.hp, userinfo.maxHp);

        if (UserInfo.myInfo.id == userinfo.id)
        {
            // 내 캐릭터일 경우 공격력, 방어력, 골드, 경험치 표시
            mpTxt.text = userinfo.mp.ToString();
            attackTxt.text = userinfo.attack.ToString();
            armorTxt.text = userinfo.armor.ToString();
            goldTxt.text = userinfo.gold.ToString();
            levelTxt.text = userinfo.level.ToString();            
            SetExpGauge(userinfo.exp, userinfo.maxExp);
            Debug.Log("levelTxt : " + userinfo.level + ", expText : " + userinfo.exp + ", maxExpText : " + userinfo.maxExp);
        }
        // if (weapon != null)
        // {
        //     weapon.gameObject.SetActive(userinfo.weapon != null);
        //     if (userinfo.weapon != null)
        //     {
        //         weapon.sprite = await ResourceManager.instance.LoadAsset<Sprite>("icon_" + userinfo.weapon.rcode, eAddressableType.Thumbnail);
        //     }
        // }
        // for (int i = 0; i < equips.Count; i++)
        // {
        //     if (userinfo.equips.Count > i)
        //     {
        //         equips[i].gameObject.SetActive(true);
        //         equips[i].sprite = await ResourceManager.instance.LoadAsset<Sprite>("icon_" + userinfo.equips[i].rcode, eAddressableType.Thumbnail);
        //     }
        //     else
        //     {
        //         equips[i].gameObject.SetActive(false);
        //     }
        // }
        // for (int i = 0; i < debuffs.Count; i++)
        // {
        //     if (userinfo.debuffs.Count > i)
        //     {
        //         debuffs[i].gameObject.SetActive(true);
        //         debuffs[i].sprite = await ResourceManager.instance.LoadAsset<Sprite>("icon_" + userinfo.debuffs[i].rcode, eAddressableType.Thumbnail);
        //     }
        //     else
        //     {
        //         debuffs[i].gameObject.SetActive(false);
        //     }
        // }
    }

    public void OnClickItem()
    {
        callback?.Invoke(idx);
    }

    public void SetSelectVisible(bool visible)
    {
        select.SetActive(visible);
    }

    public void SetVisibleRole(bool isVisible)
    {
        targetMark.SetActive(isVisible);
    }

    public void SetHpGauge(int hp, int maxHp)
    {
        string maxHpText = " / " + maxHp.ToString();
        hpGaugeText.text = hp.ToString() + maxHpText;
        hpGauge.fillAmount = (float)hp / maxHp;
    }

    public void SetExpGauge(int exp, int maxExp)
    {
        string expText = exp.ToString();
        string maxExpText = maxExp.ToString();
        expGaugeText.text = expText + " / " + maxExpText;
        expGauge.fillAmount = (float)exp / maxExp;
    }

    public void SetDeath()
    {
        for (int i = 0; i < 5; i++)
        {
            hpGauges[i].SetActive(false);
        }
        death.SetActive(true);
        SetVisibleRole(true);
    }
}