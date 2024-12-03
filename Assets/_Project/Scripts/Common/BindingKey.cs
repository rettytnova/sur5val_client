public enum en_QuickSlot
{
    QUICK_SLOT_UI_NONE,

    QUICK_SLOT_UI_INVENTORY,

    QUICK_SLOT_MOVE_UP,
    QUICK_SLOT_MOVE_DOWN,
    QUICK_SLOT_MOVE_LEFT,
    QUICK_SLOT_MOVE_RIGHT,    

    QUICK_SLOT_SKILL_USE,

    QUICK_SLOT_CHAT_INPUT
}

public enum en_KeyCode
{
    KEY_CODE_NONE,

    KEY_CODE_I,

    KEY_CODE_SPACE,

    KEY_CODE_W,
    KEY_CODE_S,
    KEY_CODE_A,
    KEY_CODE_D,

    KEY_CODE_ENTER
}

public class BindingKey
{
    public en_QuickSlot quickSlotType;
    public en_KeyCode keyCode;

    public BindingKey()
    {
        quickSlotType = en_QuickSlot.QUICK_SLOT_UI_NONE;
        keyCode = en_KeyCode.KEY_CODE_NONE;
    }
}