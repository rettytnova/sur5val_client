using System.Collections;
using TMPro;
using UnityEngine;
using static Define;

public class UIGlobalMessage : UIBaseTwo
{
    public GlobalMessageType globalMessageType = GlobalMessageType.GlobalMessageNone;

    enum en_GlobalMessageText
    {
        GlobalMessageText
    }

    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(en_GlobalMessageText));
    }

    public override void Binding()
    {
    }    

    public override void ShowCloseUI(bool IsShowClose)
    {
    }

    public void SetGlobalMessage(GlobalMessageType globalMessageType, string globalMessage)
    {
        this.globalMessageType = globalMessageType;

        switch(globalMessageType)
        {
            case GlobalMessageType.GlobalMessageRound:
                GetTextMeshPro((int)en_GlobalMessageText.GlobalMessageText).color = new Color32(255, 100, 100, 255);
                GetTextMeshPro((int)en_GlobalMessageText.GlobalMessageText).text = globalMessage;
                break;
            case GlobalMessageType.GlobalMessageCooltime:
                GetTextMeshPro((int)en_GlobalMessageText.GlobalMessageText).color = new Color32(100, 255, 100, 255);
                GetTextMeshPro((int)en_GlobalMessageText.GlobalMessageText).text = globalMessage;
                break;
        }       

        StartCoroutine(GlobalMessageUIDestory());
    }

    IEnumerator GlobalMessageUIDestory()
    {
        yield return new WaitForSeconds(Constant.GLOBAL_MESSAGE_TIME);

        var gameSceneUI = GameScene.GetInstance.gameSceneUI;
        if (gameSceneUI != null)
        {
            UIGlobalMessageBox globalMessageBoxUI = gameSceneUI.globalMessageBox;
            if(globalMessageBoxUI != null)
            {
                globalMessageBoxUI.GlobalMessageDestroy(this);
            }
        }
    }
}
