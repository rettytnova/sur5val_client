using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constant
{
    public const float GLOBAL_MESSAGE_GAP = 60.0f;
    public const float GLOBAL_MESSAGE_TIME = 2.0f;
}

public class Define
{
    public enum en_DebugSpawnPosition
    {
        DEBUG_SPAWN_POSITION_NONE,
        DEBUG_SPAWN_POSITION_MONSTER,
        DEBUG_SPAWN_POSITION_PLAYER,
        DEBUG_SPAWN_POSITION_BOSS
    }

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
