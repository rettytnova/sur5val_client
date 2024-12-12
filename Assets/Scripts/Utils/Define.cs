using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constant
{
    public const float GLOBAL_MESSAGE_GAP = 50.0f;
    public const float GLOBAL_MESSAGE_TIME = 20.0f;
}

public class Define
{
    public enum en_GlobalMessageType
    {
        GLOBAL_MESSAGE_NONE,

        GLOBAL_MESSAGE_ROUND
    }

    public enum en_UIEvent
    {
        PointerEnter,
        PointerExit,
        MouseClick,
        BeginDrag,
        Drag,
        EndDrag,
        Drop,
        Scroll
    }    

    public enum en_ResourceName
    {
        RESOURCE_NAME_NONE,

        RESOURCE_UI_GLOBAL_MESSAGE,
        RESOURCE_GENERIC_DEATH
    }
}
