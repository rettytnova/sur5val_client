public enum en_QuickSlot
{
    QUICK_SLOT_UI_NONE,

    QUICK_SLOT_UI_INVENTORY,

    QUICK_SLOT_SKILL_USE
}

public enum en_KeyCode
{
    KEY_CODE_NONE,

    KEY_CODE_I,

    KEY_CODE_SPACE
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