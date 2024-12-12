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
                break;
            case GlobalMessageType.GlobalMessageCooltime:
                GetTextMeshPro((int)en_GlobalMessageText.GlobalMessageText).color = new Color32(100, 255, 100, 255);
                break;
            case GlobalMessageType.GlobalDebugModeOn:
                GetTextMeshPro((int)en_GlobalMessageText.GlobalMessageText).color = new Color32(255, 50, 50, 255);
                break;
            case GlobalMessageType.GlobalDebugModeOff:
                GetTextMeshPro((int)en_GlobalMessageText.GlobalMessageText).color = new Color32(50, 50, 255, 255);
                break;
        }

        GetTextMeshPro((int)en_GlobalMessageText.GlobalMessageText).text = globalMessage;

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
