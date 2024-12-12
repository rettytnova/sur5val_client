using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MyButton : UIBaseTwo
{
    public Define.en_DebugSpawnPosition debugSpawnPosition;

    enum en_MyButtonGameObject
    {
        MyButton
    }

    enum en_MyButtonText
    {
        ButtonText
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(en_MyButtonGameObject));
        Bind<TextMeshProUGUI>(typeof(en_MyButtonText));

        BindEvent(GetGameObject((int)en_MyButtonGameObject.MyButton).gameObject, OnMyButtonClick, Define.en_UIEvent.MouseClick);        
    }

    private void OnMyButtonClick(PointerEventData data)
    {
        Debug.Log("MyButton" + debugSpawnPosition);
        GameManager.instance.debugSpawnPosition = debugSpawnPosition;
    }

    public override void Binding()
    {
        
    }    

    public override void ShowCloseUI(bool IsShowClose)
    {
        gameObject.SetActive(IsShowClose);
    }    
}
