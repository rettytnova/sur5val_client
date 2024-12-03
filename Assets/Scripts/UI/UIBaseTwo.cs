using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UIBaseTwo : MonoBehaviour
{
    // UI에 할당된 배열 인덱스 번호    
    public int _SceneUIListIndex;

    Dictionary<Type, UnityEngine.Object[]> _Objects = new Dictionary<Type, UnityEngine.Object[]>();

    public abstract void Init();
    public abstract void Binding();
    public abstract void ShowCloseUI(bool IsShowClose);

    private void Awake()
    {
        Init();
    }

    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        string[] EnumNames = Enum.GetNames(type);

        UnityEngine.Object[] Objects = new UnityEngine.Object[EnumNames.Length];
        _Objects.Add(typeof(T), Objects);

        for (int i = 0; i < EnumNames.Length; i++)
        {
            if (typeof(T) == typeof(GameObject))
            {
                Objects[i] = UtilTwo.FindChild(gameObject, EnumNames[i], true);
            }
            else
            {
                Objects[i] = UtilTwo.FindChild<T>(gameObject, EnumNames[i], true);
            }

            if (Objects[i] == null)
            {
                Debug.Log($"{EnumNames[i]} 오브젝트를 찾을 수 없습니다.");
            }
        }
    }

    protected T Get<T>(int Index) where T : UnityEngine.Object
    {
        UnityEngine.Object[] Objects = null;
        if (_Objects.TryGetValue(typeof(T), out Objects) == false)
        {
            return null;
        }

        return Objects[Index] as T;
    }

    protected GameObject GetGameObject(int Index)
    {
        return Get<GameObject>(Index);
    }

    protected Text GetText(int Index)
    {
        return Get<Text>(Index);
    }

    protected Button GetButton(int Index)
    {
        return Get<Button>(Index);
    }

    protected Image GetImage(int Index)
    {
        return Get<Image>(Index);
    }

    protected Canvas GetCanvas(int Index)
    {
        return Get<Canvas>(Index);
    }

    protected Slider GetSlider(int Index)
    {
        return Get<Slider>(Index);
    }

    protected InputField GetInputField(int Index)
    {
        return Get<InputField>(Index);
    }

    protected TMP_InputField GetTMPInputField(int Index)
    {
        return Get<TMP_InputField>(Index);
    }

    protected ScrollRect GetScrollRect(int Index)
    {
        return Get<ScrollRect>(Index);
    }

    protected TextMeshProUGUI GetTextMeshPro(int Index)
    {
        return Get<TextMeshProUGUI>(Index);
    }

    public static void BindEvent(GameObject Go, Action<PointerEventData> Action, Define.en_UIEvent EventType)
    {
        //UI_EventHandler 핸들러 스크립트가 없으면 붙여줌
        UIEventHandler Evt = UtilTwo.GetOrAddComponent<UIEventHandler>(Go);

        switch (EventType)
        {
            case Define.en_UIEvent.PointerEnter:
                Evt.OnPointerEnterHandler += Action;
                break;
            case Define.en_UIEvent.PointerExit:
                Evt.OnPointerExitHandler += Action;
                break;
            case Define.en_UIEvent.MouseClick:
                Evt.OnClickHandler += Action;
                break;
            case Define.en_UIEvent.BeginDrag:
                Evt.OnBeginDragHandler += Action;
                break;
            case Define.en_UIEvent.Drag:
                Evt.OnDragHandler += Action;
                break;
            case Define.en_UIEvent.EndDrag:
                Evt.OnEndDragHandler += Action;
                break;
            case Define.en_UIEvent.Drop:
                Evt.OnDropHandler += Action;
                break;
            case Define.en_UIEvent.Scroll:
                Evt.OnScrollHandler += Action;
                break;                
        }
    }
}