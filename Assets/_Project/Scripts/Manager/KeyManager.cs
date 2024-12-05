using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeyManager
{
    private List<BindingKey> uiBindingKeys = new List<BindingKey>();

    private BindingKey moveUpKey = new BindingKey();
    private BindingKey moveDownKey = new BindingKey();
    private BindingKey moveLeftKey = new BindingKey();
    private BindingKey moveRightKey = new BindingKey();

    private UIChattingInput chattingInputUI = null;

    public KeyManager()
    {
        BindingKey inventoryOpenCloseKey = new BindingKey();
        inventoryOpenCloseKey.quickSlotType = en_QuickSlot.QUICK_SLOT_UI_INVENTORY;
        inventoryOpenCloseKey.keyCode = en_KeyCode.KEY_CODE_I;

        BindingKey skillUseKey = new BindingKey();
        skillUseKey.quickSlotType = en_QuickSlot.QUICK_SLOT_SKILL_USE;
        skillUseKey.keyCode = en_KeyCode.KEY_CODE_SPACE;

        BindingKey chattingInputKey = new BindingKey();
        chattingInputKey.quickSlotType = en_QuickSlot.QUICK_SLOT_CHAT_INPUT;
        chattingInputKey.keyCode = en_KeyCode.KEY_CODE_ENTER;      

        moveUpKey.quickSlotType = en_QuickSlot.QUICK_SLOT_MOVE_UP;
        moveUpKey.keyCode = en_KeyCode.KEY_CODE_W;

        moveDownKey.quickSlotType = en_QuickSlot.QUICK_SLOT_MOVE_DOWN;
        moveDownKey.keyCode = en_KeyCode.KEY_CODE_S;

        moveLeftKey.quickSlotType = en_QuickSlot.QUICK_SLOT_MOVE_LEFT;
        moveLeftKey.keyCode = en_KeyCode.KEY_CODE_A;

        moveRightKey.quickSlotType = en_QuickSlot.QUICK_SLOT_MOVE_RIGHT;
        moveRightKey.keyCode = en_KeyCode.KEY_CODE_D;

        uiBindingKeys.Add(inventoryOpenCloseKey);
        uiBindingKeys.Add(skillUseKey);
        uiBindingKeys.Add(chattingInputKey);
    }

    public bool KeyboardGetKeyActions(en_KeyCode keyCode)
    {
        bool isKeyboardKeyAction = false;

        switch (keyCode)
        {
            case en_KeyCode.KEY_CODE_W:
                if (Input.GetKey(KeyCode.W))
                {
                    isKeyboardKeyAction = true;
                }
                break;
            case en_KeyCode.KEY_CODE_S:
                if (Input.GetKey(KeyCode.S))
                {
                    isKeyboardKeyAction = true;
                }
                break;
            case en_KeyCode.KEY_CODE_A:
                if (Input.GetKey(KeyCode.A))
                {
                    isKeyboardKeyAction = true;
                }
                break;
            case en_KeyCode.KEY_CODE_D:                
                if (Input.GetKey(KeyCode.D))
                {
                    isKeyboardKeyAction = true;
                }
                break;
        }

        return isKeyboardKeyAction;
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
            case en_KeyCode.KEY_CODE_ENTER:
                if (Input.GetKeyDown(KeyCode.Return))
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

    public async void UIActions(en_QuickSlot quickSlot)
    {
        switch (quickSlot)
        {
            case en_QuickSlot.QUICK_SLOT_UI_INVENTORY:
                if (UIManager.IsOpened<PopupDeck>())
                {
                    UIManager.Hide<PopupDeck>();
                    return;
                }

                await UIManager.Show<PopupDeck>();
                break;
            case en_QuickSlot.QUICK_SLOT_SKILL_USE:
                UIGame.instance.OnCardUse();
                break;
            case en_QuickSlot.QUICK_SLOT_CHAT_INPUT:
                var GameSceneUI = GameScene.GetInstance.gameSceneUI;
                if (GameSceneUI != null)
                {
                    if (GameSceneUI.uiChattingInput.gameObject.activeSelf == false)
                    {
                        GameManager.instance.userCharacter?.MoveCharacter(Vector2.zero);
                        GameSceneUI.uiChattingInput.ShowCloseUI(true);
                    }
                    else
                    {
                        GameSceneUI.uiChattingInput.ShowCloseUI(false);
                    }

                }
                break;
        }
    }

    public void MoveKeyUpdate()
    {
        Vector2 moveDirection = new Vector2();

        moveDirection.x = 0;
        moveDirection.y = 0;

        if (KeyboardGetKeyActions(moveUpKey.keyCode))
        {
            moveDirection.y += 1.0f;
            if (moveDirection.y > 0)
            {
                moveDirection.y = 1.0f;
            }
        }

        if (KeyboardGetKeyActions(moveDownKey.keyCode))
        {
            moveDirection.y += -1.0f;
            if (moveDirection.y < 0)
            {
                moveDirection.y = -1.0f;
            }
        }

        if (KeyboardGetKeyActions(moveLeftKey.keyCode))
        {
            moveDirection.x += -1.0f;
            if (moveDirection.x < 0)
            {
                moveDirection.x = -1.0f;
            }
        }

        if (KeyboardGetKeyActions(moveRightKey.keyCode))
        {
            moveDirection.x += 1.0f;
            if (moveDirection.x > 0)
            {
                moveDirection.x = 1.0f;
            }
        }

        if (!GameScene.GetInstance.gameSceneUI.uiChattingInput.gameObject.activeSelf)
        {
            GameManager.instance.userCharacter?.MoveCharacter(moveDirection);
        }
    }
}