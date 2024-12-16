using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIGameScene : UIBaseTwo
{
    public UIGlobalMessageBox globalMessageBox = null;
    public UIChattingInput uiChattingInput = null;
    public GameObject spawnPositionDebug = null;

    private Coroutine basicAttackTimeStartCO;
    private Coroutine skillTimeStartCO;

    private float _SkillCoolTime;    
    private float _SkillCoolTimeSpeed;

    private Image shotCooltimeImage = null;
    private Image attackCooltimeImage = null;

    public float basicAttackCooltimeProgress = 1.0f;
    public float skillCooltimeProgress = 1.0f;

    public override void Init()
	{
        
    }

	public override void Binding()
	{
		
	}

    public override void ShowCloseUI(bool IsShowClose)
    {
        
    }

    public void cooltimeAttackStart(string rcode)
    {
        if ((rcode != "CAD00100" || rcode != "CAD00113") && shotCooltimeImage == null)
        {
            GameObject buttonShotGO = UtilTwo.FindChild(gameObject, "ButtonShotCooltime", true);
            if (buttonShotGO != null)
            {
                shotCooltimeImage = buttonShotGO.GetComponent<Image>();
            }
        }
        else if ((rcode == "CAD00100" || rcode == "CAD00113") && attackCooltimeImage == null)
        {
            GameObject buttonAttackGO = UtilTwo.FindChild(gameObject, "ButtonAttackCooltime", true);
            if (buttonAttackGO != null)
            {
                attackCooltimeImage = buttonAttackGO.GetComponent<Image>();
            }
        }

        // 원래 DB작업으로 해야됨(필요하면 구현)
        int rcodeNumber = int.Parse(rcode.Substring(3));
        float coolTimeMilliSecond = 0.0f;
        switch(rcodeNumber)
        {
            case (int)CardType.Sur5VerBasicSkill: // 기본 공격
                coolTimeMilliSecond = 3000.0f;
                break;
            case (int)CardType.MagicianBasicSkill: // 쌍둥이 폭팔 (마법사 기본 스킬)
                coolTimeMilliSecond = 5000.0f;
                break;
            case (int)CardType.ArcherBasicSkill: // 차지 샷 (궁수 기본 스킬)
                coolTimeMilliSecond = 5000.0f;
                break;
            case (int)CardType.RogueBasicSkill: // 급습 (도적 기본 스킬)
                coolTimeMilliSecond = 5000.0f;
                break;
            case (int)CardType.WarriorBasicSkill: // 투사의 결단 (전사 기본 스킬)
                coolTimeMilliSecond = 5000.0f;
                break;
            case (int)CardType.MagicianExtendedSkill: // 삼중 타격 (마법사 강화 스킬)
                coolTimeMilliSecond = 7000.0f;
                break;
            case (int)CardType.ArcherExtendedSkill: // 폭풍의 눈 (궁수 강화 스킬)
                coolTimeMilliSecond = 7000.0f;
                break;
            case (int)CardType.RogueExtendedSkill: // 그림자의 춤 (도적 강화 스킬)
                coolTimeMilliSecond = 7000.0f;
                break;
            case (int)CardType.WarriorExtendedSkill: // 천둥의 강타 (전사 강화 스킬)
                coolTimeMilliSecond = 7000.0f;
                break;
            case (int)CardType.MagicianFinalSkill: // 불멸의 폭풍
                coolTimeMilliSecond = 10000.0f;
                break;
            case (int)CardType.ArcherFinalSkill: // 수호의 결단
                coolTimeMilliSecond = 10000.0f;
                break;
            case (int)CardType.RogueFinalSkill: // 무력화 단검
                coolTimeMilliSecond = 10000.0f;
                break;
            case (int)CardType.WarriorFinalSkill: // 흡혈의 검
                coolTimeMilliSecond = 10000.0f;
                break;
            case (int)CardType.BossBasicSkill: // 보스 기본 공격
                coolTimeMilliSecond = 5000.0f;
                break;
            case (int)CardType.BossExtendedSkill: // 대재앙의 강타
                coolTimeMilliSecond = 7000.0f;
                break;
        }

        _SkillCoolTime = (coolTimeMilliSecond + 10.0f) / 1000.0f;
        _SkillCoolTimeSpeed = 1.0f / _SkillCoolTime;

        if(rcodeNumber == (int)CardType.Sur5VerBasicSkill || rcodeNumber == (int)CardType.BossBasicSkill)
        {
            if (basicAttackTimeStartCO != null) StopCoroutine(basicAttackTimeStartCO);
            UIGame.instance.buttonAttackCooltimeText.gameObject.SetActive(true);
            basicAttackTimeStartCO = StartCoroutine(BasicAttackCooltimeStart(rcode));
        }
        else
        {
            if (skillTimeStartCO != null) StopCoroutine(skillTimeStartCO);
            UIGame.instance.buttonShotCooltimeText.gameObject.SetActive(true);
            skillTimeStartCO = StartCoroutine(SkillCooltimeStart(rcode));
        }
    }

    IEnumerator BasicAttackCooltimeStart(string rcode)
    {
        float Rate = _SkillCoolTimeSpeed;

        while (basicAttackCooltimeProgress >= 0)
        {
            float FillAmount = Mathf.Lerp(0, 1, basicAttackCooltimeProgress);
            attackCooltimeImage.fillAmount = FillAmount;
            basicAttackCooltimeProgress -= Rate * Time.deltaTime;
            
            float cooltime = basicAttackCooltimeProgress * _SkillCoolTime;
            UIGame.instance.buttonAttackCooltimeText.text = cooltime.ToString("N2");
            if(cooltime <= 0) UIGame.instance.buttonAttackCooltimeText.gameObject.SetActive(false);
            yield return null;
        }
        
        basicAttackCooltimeProgress = 1.0f;
        attackCooltimeImage.fillAmount = 0;
        basicAttackTimeStartCO = null;
    }

    IEnumerator SkillCooltimeStart(string rcode)
    {
        float Rate = _SkillCoolTimeSpeed;       

        while(skillCooltimeProgress >= 0)
        {
            float FillAmount = Mathf.Lerp(0, 1, skillCooltimeProgress);            
            shotCooltimeImage.fillAmount = FillAmount;
            skillCooltimeProgress -= Rate * Time.deltaTime;

            float cooltime = skillCooltimeProgress * _SkillCoolTime;
            UIGame.instance.buttonShotCooltimeText.text = cooltime.ToString("N2");
            if (cooltime <= 0) UIGame.instance.buttonShotCooltimeText.gameObject.SetActive(false);
            yield return null;
        }
        
        skillCooltimeProgress = 1.0f;
        shotCooltimeImage.fillAmount = 0;
        skillTimeStartCO = null;
    }
}
