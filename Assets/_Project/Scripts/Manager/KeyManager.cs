using System.Collections.Generic;
using UnityEngine;

public class KeyManager
{
    private List<BindingKey> uiBindingKeys = new List<BindingKey>();

    public KeyManager()
    {
        BindingKey inventoryOpenCloseKey = new BindingKey();
        inventoryOpenCloseKey.quickSlotType = en_QuickSlot.QUICK_SLOT_UI_INVENTORY;
        inventoryOpenCloseKey.keyCode = en_KeyCode.KEY_CODE_I;

        BindingKey skillUseKey = new BindingKey();
        skillUseKey.quickSlotType = en_QuickSlot.QUICK_SLOT_SKILL_USE;
        skillUseKey.keyCode = en_KeyCode.KEY_CODE_SPACE;

        uiBindingKeys.Add(inventoryOpenCloseKey);
        uiBindingKeys.Add(skillUseKey);
    }

    public bool KeyboardGetKeyDownActions(en_KeyCode keyCode)
    {
        bool isKeyboardKeyAction = false;

        switch (keyCode)
        {
            case en_KeyCode.KEY_CODE_I:
                if (Input.GetKeyDown(KeyCode.I))
                {
                    isKeyboardKeyAction = true;
                }
                break;
            case en_KeyCode.KEY_CODE_SPACE:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    isKeyboardKeyAction = true;
                }
                break;
        }

        return isKeyboardKeyAction;
    }

    public void QuickSlotBarActions()
    {
        foreach (BindingKey key in uiBindingKeys)
        {
            if (KeyboardGetKeyDownActions(key.keyCode))
            {
                UIActions(key.quickSlotType);
            }
        }
    }

    public void UIActions(en_QuickSlot quickSlot)
    {
        switch (quickSlot)
        {
            case en_QuickSlot.QUICK_SLOT_UI_INVENTORY:                
                if(UIManager.IsOpened<PopupDeck>())
                {
                    UIManager.Hide<PopupDeck>();
                    return;
                }

                UIManager.Show<PopupDeck>();
                break;
            case en_QuickSlot.QUICK_SLOT_SKILL_USE:
                UIGame.instance.OnCardUse();
                break;
        }
    }
}