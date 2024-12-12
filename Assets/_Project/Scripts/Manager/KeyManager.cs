using Ironcow;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeyManager
{
    private List<BindingKey> uiBindingKeys = new List<BindingKey>();

    private BindingKey moveUpKey = new BindingKey();
    private BindingKey moveDownKey = new BindingKey();
    private BindingKey moveLeftKey = new BindingKey();
    private BindingKey moveRightKey = new BindingKey();

    private BindingKey defaultAttackKey = new BindingKey();

    private BindingKey debugModeKey = new BindingKey();

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

        debugModeKey.quickSlotType = en_QuickSlot.QUICK_SLOT_DEBUG_MODE;
        debugModeKey.keyCode = en_KeyCode.KEY_CODE_Y;

        defaultAttackKey.quickSlotType = en_QuickSlot.QUICK_SLOT_DEFAULT_ATTACK;
        defaultAttackKey.keyCode = en_KeyCode.KEY_CODE_MOUSE_RIGHT_CLICK;

        uiBindingKeys.Add(inventoryOpenCloseKey);
        uiBindingKeys.Add(skillUseKey);
        uiBindingKeys.Add(chattingInputKey);
        uiBindingKeys.Add(debugModeKey);
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
            case en_KeyCode.KEY_CODE_Y:
                if (Input.GetKeyDown(KeyCode.Y))
                {
                    isKeyboardKeyAction = true;
                }
                break;
        }

        return isKeyboardKeyAction;
    } 

    public bool MouseGetKeyDownActions(en_KeyCode keyCode)
    {
        bool isMouseKeyAction = false;

        switch (keyCode)
        {
            case en_KeyCode.KEY_CODE_MOUSE_LEFT_CLICK:
                if(Input.GetMouseButtonDown(0))
                {
                    isMouseKeyAction = true;
                }
                break;
            case en_KeyCode.KEY_CODE_MOUSE_RIGHT_CLICK:
                if(Input.GetMouseButtonDown(1))
                {
                    isMouseKeyAction = true;
                }
                break;
        }

        return isMouseKeyAction;
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

        if (MouseGetKeyDownActions(defaultAttackKey.keyCode))
        {
            switch (defaultAttackKey.keyCode)
            {
                case en_KeyCode.KEY_CODE_MOUSE_RIGHT_CLICK:
                    GameManager.instance.OnBasicAttack();
                    break;
            }
        }

        if(GameManager.instance.isDebug == true)
        {
            if(!EventSystem.current.IsPointerOverGameObject())
            {
                if (Input.GetMouseButtonDown(0))
                {
                    string roleType = null;

                    if (GameManager.instance.isDebug == true)
                    {
                        switch (GameManager.instance.debugSpawnPosition)
                        {
                            case Define.en_DebugSpawnPosition.DEBUG_SPAWN_POSITION_MONSTER:
                                roleType = "monster";
                                break;
                            case Define.en_DebugSpawnPosition.DEBUG_SPAWN_POSITION_PLAYER:
                                roleType = "player";
                                break;
                            case Define.en_DebugSpawnPosition.DEBUG_SPAWN_POSITION_BOSS:
                                roleType = "boss";
                                break;
                        }

                        Vector3 ScreenToMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                        GamePacket spawnPositionSendPacket = new GamePacket();
                        spawnPositionSendPacket.SpawnPositionSendRequest = new C2SSpawnPositionSendRequest() { SpawnPositionX = ScreenToMousePosition.x, SpawnPositionY = ScreenToMousePosition.y, RoleType = roleType };
                        Managers.networkManager.GameServerSend(spawnPositionSendPacket);
                    }
                }
            }            
        }        
    }

    public async void UIActions(en_QuickSlot quickSlot)
    {
        var GameSceneUI = GameScene.GetInstance.gameSceneUI;

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
            case en_QuickSlot.QUICK_SLOT_DEBUG_MODE:
                bool isDebugMode = GameManager.instance.isDebug;
                if(isDebugMode == true)
                {
                    GameManager.instance.debugSpawnPosition = Define.en_DebugSpawnPosition.DEBUG_SPAWN_POSITION_NONE;
                    GameSceneUI.spawnPositionDebug.gameObject.SetActive(false);
                    GameSceneUI.globalMessageBox.NewGlobalMessage(GlobalMessageType.GlobalDebugModeOff, "디버그 모드가 꺼졌습니다.");
                    GameManager.instance.isDebug = false;
                }
                else
                {
                    GameSceneUI.spawnPositionDebug.gameObject.SetActive(true);
                    GameSceneUI.globalMessageBox.NewGlobalMessage(GlobalMessageType.GlobalDebugModeOn, "디버그 모드가 켜졌습니다.");
                    GameManager.instance.isDebug = true;                                      
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