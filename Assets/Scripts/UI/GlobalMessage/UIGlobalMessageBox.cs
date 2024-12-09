using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIGlobalMessageBox : UIBaseTwo
{
    public Dictionary<GlobalMessageType, UIGlobalMessage> globalMessages = new Dictionary<GlobalMessageType, UIGlobalMessage>();

    enum en_GlobalMessageBoxGameObject
    {
        GlobalMessageBoxScroll,
        GlobalMessagePannel
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(en_GlobalMessageBoxGameObject));


    }

    public override void Binding()
    {
        
    }    

    public override void ShowCloseUI(bool IsShowClose)
    {
        
    }

    public void Update()
    {
        int globalMessageCount = globalMessages.Count;
        GameObject globalMessageBoxGo = GetGameObject((int)en_GlobalMessageBoxGameObject.GlobalMessageBoxScroll);
        if (globalMessageBoxGo != null)
        {
            RectTransform globalMessageBoxRect = globalMessageBoxGo.GetComponent<RectTransform>();
            globalMessageBoxRect.sizeDelta = new Vector2(globalMessageBoxRect.rect.width, globalMessageCount * Constant.GLOBAL_MESSAGE_GAP);
        }
    }

    public void NewGlobalMessage(GlobalMessageType globalMessageType, string globalMessage)
    {
        if (globalMessages.Count > 0)
        {            
            UIGlobalMessage findGlobalMessage = globalMessages.Values
                .FirstOrDefault(globalMessageUI => globalMessageUI.globalMessageType == globalMessageType);
            if (findGlobalMessage != null)
            {
                Destroy(findGlobalMessage);

                globalMessages.Remove(globalMessageType);
            }
        }

        GameObject newGlobalMessageGo = Managers.resourceManager.Instantiate(Define.en_ResourceName.RESOURCE_UI_GLOBAL_MESSAGE,
            GetGameObject((int)en_GlobalMessageBoxGameObject.GlobalMessagePannel).transform);
        
        UIGlobalMessage globalMessageUI = newGlobalMessageGo.GetComponent<UIGlobalMessage>();
        globalMessageUI.SetGlobalMessage(globalMessageType, globalMessage);
        globalMessages.Add(globalMessageType, globalMessageUI);
    }

    public void GlobalMessageDestroy(UIGlobalMessage globalMessageUI)
    {
        GameObject destoryGlobalMessageUI = globalMessages[globalMessageUI.globalMessageType].gameObject;
        globalMessages.Remove(globalMessageUI.globalMessageType);
        Destroy(destoryGlobalMessageUI.gameObject);
    }
}
