using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIGameScene : UIBaseTwo
{
    public UIChattingInput uiChattingInput = null;

    private Coroutine coolTimeStartCO;

    private float _SkillCoolTime;    
    private float _SkillCoolTimeSpeed;

    private Image shotCooltimeImage = null;
    private Image attackCooltimeImage = null;

    public float cooltimeProgress = 1.0f;

    public override void Init()
	{
        
    }

	public override void Binding()
	{
		
	}

    public override void ShowCloseUI(bool IsShowClose)
    {
        
    }

    public void cooltimeAttackStart()
    {
        if (shotCooltimeImage == null)
        {
            GameObject buttonShotGO = UtilTwo.FindChild(gameObject, "ButtonShotCooltime", true);
            if (buttonShotGO != null)
            {
                shotCooltimeImage = buttonShotGO.GetComponent<Image>();
            }
        }

        if (attackCooltimeImage == null)
        {
            GameObject buttonAttackGO = UtilTwo.FindChild(gameObject, "ButtonAttackCooltime", true);
            if (buttonAttackGO != null)
            {
                attackCooltimeImage = buttonAttackGO.GetComponent<Image>();
            }
        }                  

        _SkillCoolTime = 2000.0f / 1000.0f;
        _SkillCoolTimeSpeed = 1.0f / _SkillCoolTime;

        if (coolTimeStartCO != null)
        {
            StopCoroutine(coolTimeStartCO);
        }

        coolTimeStartCO = StartCoroutine(CooltimeStart());
    }    

    IEnumerator CooltimeStart()
    {
        float Rate = _SkillCoolTimeSpeed;       

        while(cooltimeProgress >= 0)
        {
            float FillAmount = Mathf.Lerp(0, 1, cooltimeProgress);            
            if(shotCooltimeImage != null && attackCooltimeImage != null)
            {
                shotCooltimeImage.fillAmount = FillAmount;
                attackCooltimeImage.fillAmount = FillAmount;
            }

            cooltimeProgress -= Rate * Time.deltaTime;                        
            yield return null;
        }

        cooltimeProgress = 1.0f;

        shotCooltimeImage.fillAmount = 0;
        attackCooltimeImage.fillAmount = 0;

        coolTimeStartCO = null;
    }
}
