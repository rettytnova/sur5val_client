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

        // ���� DB�۾����� �ؾߵ�(�ʿ��ϸ� ����)
        int rcodeNumber = int.Parse(rcode.Substring(3));
        float coolTimeMilliSecond = 0.0f;
        switch(rcodeNumber)
        {
            case (int)CardType.Sur5VerBasicSkill: // �⺻ ����
                coolTimeMilliSecond = 3000.0f;
                break;
            case (int)CardType.MagicianBasicSkill: // �ֵ��� ���� (������ �⺻ ��ų)
                coolTimeMilliSecond = 5000.0f;
                break;
            case (int)CardType.ArcherBasicSkill: // ���� �� (�ü� �⺻ ��ų)
                coolTimeMilliSecond = 5000.0f;
                break;
            case (int)CardType.RogueBasicSkill: // �޽� (���� �⺻ ��ų)
                coolTimeMilliSecond = 5000.0f;
                break;
            case (int)CardType.WarriorBasicSkill: // ������ ��� (���� �⺻ ��ų)
                coolTimeMilliSecond = 5000.0f;
                break;
            case (int)CardType.MagicianExtendedSkill: // ���� Ÿ�� (������ ��ȭ ��ų)
                coolTimeMilliSecond = 7000.0f;
                break;
            case (int)CardType.ArcherExtendedSkill: // ��ǳ�� �� (�ü� ��ȭ ��ų)
                coolTimeMilliSecond = 7000.0f;
                break;
            case (int)CardType.RogueExtendedSkill: // �׸����� �� (���� ��ȭ ��ų)
                coolTimeMilliSecond = 7000.0f;
                break;
            case (int)CardType.WarriorExtendedSkill: // õ���� ��Ÿ (���� ��ȭ ��ų)
                coolTimeMilliSecond = 7000.0f;
                break;
            case (int)CardType.MagicianFinalSkill: // �Ҹ��� ��ǳ
                coolTimeMilliSecond = 10000.0f;
                break;
            case (int)CardType.ArcherFinalSkill: // ��ȣ�� ���
                coolTimeMilliSecond = 10000.0f;
                break;
            case (int)CardType.RogueFinalSkill: // ����ȭ �ܰ�
                coolTimeMilliSecond = 10000.0f;
                break;
            case (int)CardType.WarriorFinalSkill: // ������ ��
                coolTimeMilliSecond = 10000.0f;
                break;
            case (int)CardType.BossBasicSkill: // ���� �⺻ ����
                coolTimeMilliSecond = 5000.0f;
                break;
            case (int)CardType.BossExtendedSkill: // ������� ��Ÿ
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
