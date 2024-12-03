using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using Ironcow;
using UnityEngine.EventSystems;
using System.Collections;

public class UIChattingInput : UIBaseTwo
{
    private Button inputBtn;
    private Button closeBtn;
    private TMP_InputField chattingInputField;    

    enum en_ChattingInputGameObject
    {
        UIChattingInput
    }

    enum en_ChattingInputfield
    {
        inputField
    }

    public override void Init()
    {
        chattingInputField = transform.Find("inputField").GetComponent<TMP_InputField>();

        Bind<GameObject>(typeof(en_ChattingInputGameObject));
        Bind<TMP_InputField>(typeof(en_ChattingInputfield));

        BindEvent(GetGameObject((int)en_ChattingInputGameObject.UIChattingInput).gameObject, OnChattingInputDrag, Define.en_UIEvent.Drag);
    }

    public override void Binding()
    {

    }

    public override void ShowCloseUI(bool IsShowClose)
    {
        gameObject.SetActive(IsShowClose);

        if (IsShowClose == true)
        {
            GetTMPInputField((int)en_ChattingInputfield.inputField).Select();
            GetTMPInputField((int)en_ChattingInputfield.inputField).ActivateInputField();
        }
        else
        {            
            string chatMessage = GetTMPInputField((int)en_ChattingInputfield.inputField).text;
            if (chatMessage.Length > 0)
            {
                ChattingPacket C2SChattingSendPacket = new ChattingPacket();
                C2SChattingSendPacket.ChattingServerChatSendRequest = new C2SChattingServerChatSendRequest() { ChatMessage = chatMessage };
                Managers.networkManager.ChattingServerSend(C2SChattingSendPacket);

                GetTMPInputField((int)en_ChattingInputfield.inputField).DeactivateInputField();
                GetTMPInputField((int)en_ChattingInputfield.inputField).text = "";
            }            
        }
    }   

    void OnChattingInputDrag(PointerEventData Event)
    {
        GetGameObject((int)en_ChattingInputGameObject.UIChattingInput).gameObject.GetComponent<RectTransform>().anchoredPosition += Event.delta;
    }
}
