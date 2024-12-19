using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ironcow;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.AI;
using System;
using Unity.Multiplayer.Playmode;
using Ironcow.WebSocketPacket;
using TMPro;
using UnityEngine.Tilemaps;


public class Character : FSMController<CharacterState, CharacterFSM, CharacterDataSO>
{
    [SerializeField] public eCharacterType characterType;
    [SerializeField] private SpriteAnimation anim;
    [SerializeField] private Rigidbody2D rig;
    [SerializeField] private GameObject selectCircle;
    [SerializeField] private SpriteRenderer minimapIcon;
    [SerializeField] private GameObject range;
    [SerializeField] private GameObject targetMark;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private GameObject death;
    [SerializeField] private CircleCollider2D collider;
    [SerializeField] public GameObject stop;
    [SerializeField] public ChatBubble chatBubble;
    [SerializeField] public GameObject hpBarCanvas;
    [SerializeField] private Image hpBar;
    [SerializeField] private TMP_Text level;
    [SerializeField] private float speed = 3;

    [HideInInspector] public UserInfo userInfo;

    public bool isPlayable { get => characterType == eCharacterType.playable; }
    public Vector2 dir;
    public float Speed { get => speed; }
    public bool isInside;

    private void Awake()
    {
        if (UserInfo.myInfo.roleType == eRoleType.psychopass)
            speed = 3.8f;
        else if (UserInfo.myInfo.roleType == eRoleType.bodyguard)
            speed = 3.0f;
        else
            speed = 3.0f;

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        if (characterType == eCharacterType.npc) minimapIcon.gameObject.SetActive(false);
    }

    public void UpdateHpBar()
    {
        if (userInfo == null || DataManager.instance?.userDict == null || hpBar == null)
        {
            hpBarCanvas.SetActive(false);
            return;
        }
        userInfo = DataManager.instance.userDict.TryGetValue(userInfo.id, out var info) ? info : userInfo;
        hpBar.fillAmount = (float)userInfo.hp / userInfo.maxHp;
        level.text = userInfo.level.ToString();
    }

    public void ChattingMessage(string chatMessage)
    {
        if (chatBubble.gameObject.activeSelf == true)
        {
            chatBubble.ChatBubbleSetChatMessage(chatMessage);
        }
        else
        {
            chatBubble.gameObject.SetActive(true);

            chatBubble.ChatBubbleSetChatMessage(chatMessage);
        }
    }

    public override async void Init(BaseDataSO data)
    {
        this.data = (CharacterDataSO)data;
        fsm = new CharacterFSM(CreateState<CharacterIdleState>().SetElement(anim, rig, this));
        minimapIcon.sprite = await ResourceManager.instance.LoadAsset<Sprite>(data.rcode, eAddressableType.Thumbnail);
    }

    public void SetCharacterType(eCharacterType characterType)
    {
        this.characterType = characterType;
        rig.mass = characterType == eCharacterType.playable ? 10 : 10000;
        agent.enabled = characterType != eCharacterType.playable;
        var tags = CurrentPlayer.ReadOnlyTags();
        if (tags.Length == 0)
        {
            tags = new string[1] { "player1" };
        }
        if (isPlayable)
        {
            if (tags[0].Equals("player1") &&
                (Application.platform == RuntimePlatform.WindowsPlayer ||
                Application.platform == RuntimePlatform.OSXPlayer ||
                Application.platform == RuntimePlatform.WindowsEditor))
            {
                UIGame.instance.stick.gameObject.SetActive(false);
            }
            else
            {
                UIGame.instance.stick.OnHandleChanged += MoveCharacter;
            }
        }
    }

    public void SetMovePosition(Vector3 pos)
    {
        if (characterType == eCharacterType.playable) return;
        //rig.MovePosition(pos);
        agent.SetDestination(pos);
        var isLeft = agent.velocity.x < 0;
        isLeft = data.isLeft ? !isLeft : isLeft;
        if (agent.velocity.x != 0)
            anim.SetFlip(isLeft);
        if (agent.velocity == Vector3.zero)
            ChangeState<CharacterIdleState>().SetElement(anim, rig, this);
        else
            ChangeState<CharacterWalkState>().SetElement(anim, rig, this);
    }

    public void SetPosition(Vector3 pos)
    {
        agent.enabled = false;
        transform.position = pos;
        agent.enabled = true;
    }

    public void OnChangeState<T>() where T : CharacterState
    {
        ChangeState<T>().SetElement(anim, rig, this);
    }

    public bool IsState<T>()
    {
        return fsm.IsState<T>();
    }

    public async void SetTargetMark()
    {
        // 임시로 내용 변경 - 보스일 경우 호출해서 타겟마크의 위치 조절
        //targetMark.SetActive(true);
        targetMark.GetComponent<SpriteRenderer>().sprite = await ResourceManager.instance.LoadAsset<Sprite>("Role_" + userInfo.roleType.ToString(), eAddressableType.Thumbnail);
        if (userInfo.roleType == eRoleType.psychopass)
            targetMark.transform.position += new Vector3(0, 0.2f, 0);
    }

    public void OnVisibleMinimapIcon(bool visible)
    {
        // if (characterType == eCharacterType.non_playable)
        //     minimapIcon.gameObject.SetActive(visible && !isInside);
        // else
        //     minimapIcon.gameObject.SetActive(false);
        minimapIcon.gameObject.SetActive(visible);
    }

    public void OnSelect()
    {
        selectCircle.SetActive(!selectCircle.activeInHierarchy);
    }

    public void OnVisibleRange()
    {
        range.SetActive(!range.activeInHierarchy);
    }

    float syncFrame = 0;
    public void MoveCharacter(Vector2 dir)
    {
        if (fsm.IsState<CharacterStopState>() || fsm.IsState<CharacterPrisonState>() || fsm.IsState<CharacterDeathState>()) return;

        this.dir = dir;
        var isLeft = dir.x < 0;
        isLeft = data.isLeft ? !isLeft : isLeft;
        if (dir.x != 0)
            anim.SetFlip(isLeft);
        if (dir == Vector2.zero) ChangeState<CharacterIdleState>().SetElement(anim, rig, this);
        else ChangeState<CharacterWalkState>().SetElement(anim, rig, this);
    }

    private async void OnTriggerEnter2D(Collider2D collision)
    {
        if (characterType == eCharacterType.playable)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("BuildingInsideTrigger"))
            {
                if (!isInside)
                {
                    isInside = true;
                    var buildingSprite = collision.gameObject.transform.Find("Building Sprite");
                    GameManager.instance.SetMapInside(buildingSprite, isInside);
                }
            }

            if (collision.gameObject.layer == LayerMask.NameToLayer("BuildingOutsideTrigger"))
            {
                if (!isInside)
                {
                    isInside = true;

                    var buildingSprite = collision.gameObject.transform.parent.Find("Building Sprite").GetComponent<Tilemap>();
                    var buildingInside = collision.gameObject.transform.parent.Find("Building Inside").GetComponent<TilemapRenderer>();
                    var buildingInsideDeco = collision.gameObject.transform.parent.Find("Building Inside Deco").GetComponent<TilemapRenderer>();

                    Color buildingSpriteColor = buildingSprite.color;
                    buildingSpriteColor.a = 0.5f;
                    buildingSprite.color = buildingSpriteColor;

                    buildingInside.sortingOrder = 4;
                    buildingInsideDeco.sortingOrder = 5;
                }
            }

            if (collision.gameObject.layer == LayerMask.NameToLayer("PlantsTrigger"))
            {
                if (!isInside)
                {
                    isInside = true;

                    var plantsTileMap = collision.gameObject.transform.parent.GetComponent<Tilemap>();
                    Color color = plantsTileMap.color;
                    color.a = 0.5f;
                    plantsTileMap.color = color;
                }
            }

            if (collision.gameObject.layer == LayerMask.NameToLayer("Store"))
            {
                if (UserInfo.myInfo.characterData.RoleType != RoleType.Psychopath)
                {
                    UIGame.instance.SetShopButton(true);
                }
            }

            if (collision.gameObject.layer == LayerMask.NameToLayer("Extrance"))
            {
                if (userInfo.roleType == eRoleType.bodyguard)
                {
                    GamePacket packet = new GamePacket();
                    packet.ResultRequest = new C2SResultRequest() { ResultType = ResultType.SurvivorWin };
                    Managers.networkManager.GameServerSend(packet);
                }
            }
        }
    }

    private async void OnTriggerStay2D(Collider2D collision)
    {
        if (characterType == eCharacterType.playable)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("BuildingInsideTrigger"))
            {
                if (!isInside)
                {
                    isInside = true;
                    var buildingSprite = collision.gameObject.transform.Find("Building Sprite");
                    GameManager.instance.SetMapInside(buildingSprite, isInside);
                }
            }

            if (collision.gameObject.layer == LayerMask.NameToLayer("BuildingOutsideTrigger"))
            {
                if (!isInside)
                {
                    isInside = true;

                    var buildingSprite = collision.gameObject.transform.parent.Find("Building Sprite").GetComponent<Tilemap>();
                    var buildingInside = collision.gameObject.transform.parent.Find("Building Inside").GetComponent<TilemapRenderer>();
                    var buildingInsideDeco = collision.gameObject.transform.parent.Find("Building Inside Deco").GetComponent<TilemapRenderer>();

                    Color buildingSpriteColor = buildingSprite.color;
                    buildingSpriteColor.a = 0.5f;
                    buildingSprite.color = buildingSpriteColor;

                    buildingInside.sortingOrder = 4;
                    buildingInsideDeco.sortingOrder = 5;
                }
            }

            if (collision.gameObject.layer == LayerMask.NameToLayer("PlantsTrigger"))
            {
                if (!isInside)
                {
                    isInside = true;

                    var plantsTileMap = collision.gameObject.transform.parent.GetComponent<Tilemap>();
                    Color color = plantsTileMap.color;
                    color.a = 0.5f;
                    plantsTileMap.color = color;
                }
            }

            if (collision.gameObject.layer == LayerMask.NameToLayer("Store"))
            {
                if (UserInfo.myInfo.characterData.RoleType != RoleType.Psychopath)
                {
                    UIGame.instance.SetShopButton(true);
                }
            }

            if (collision.gameObject.layer == LayerMask.NameToLayer("Extrance"))
            {
                if (userInfo.roleType == eRoleType.bodyguard)
                {
                    GamePacket packet = new GamePacket();
                    packet.ResultRequest = new C2SResultRequest() { ResultType = ResultType.SurvivorWin };
                    Managers.networkManager.GameServerSend(packet);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (characterType == eCharacterType.playable)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("BuildingInsideTrigger"))
            {
                if (isInside)
                {
                    isInside = false;
                    var buildingSprite = collision.gameObject.transform.Find("Building Sprite");
                    GameManager.instance.SetMapInside(buildingSprite, isInside);
                }
            }

            if (collision.gameObject.layer == LayerMask.NameToLayer("BuildingOutsideTrigger"))
            {
                if (isInside)
                {
                    isInside = false;

                    var buildingSprite = collision.gameObject.transform.parent.Find("Building Sprite").GetComponent<Tilemap>();
                    var buildingInside = collision.gameObject.transform.parent.Find("Building Inside").GetComponent<TilemapRenderer>();
                    var buildingInsideDeco = collision.gameObject.transform.parent.Find("Building Inside Deco").GetComponent<TilemapRenderer>();

                    Color buildingSpriteColor = buildingSprite.color;
                    buildingSpriteColor.a = 1.0f;
                    buildingSprite.color = buildingSpriteColor;

                    buildingInside.sortingOrder = 1;
                    buildingInsideDeco.sortingOrder = 2;
                }
            }

            if (collision.gameObject.layer == LayerMask.NameToLayer("PlantsTrigger"))
            {
                if (isInside)
                {
                    isInside = false;

                    var plantsTileMap = collision.gameObject.transform.parent.GetComponent<Tilemap>();
                    Color color = plantsTileMap.color;
                    color.a = 1.0f;
                    plantsTileMap.color = color;
                }
            }

            if (collision.gameObject.layer == LayerMask.NameToLayer("Store"))
            {
                if (UserInfo.myInfo.characterData.RoleType != RoleType.Psychopath)
                {
                    UIGame.instance.SetShopButton(false);
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Character>(out var character))
        {
            if (!Managers.networkManager.GameServerIsConnected() && character == GameManager.instance.userCharacter &&
                userInfo.handCards.Find(obj => obj.rcode == "CAD00001"))
            {
                GameManager.instance.SendSocketUseCard(character.userInfo, userInfo, "CAD00001");
            }
        }
    }

    private void Update()
    {
        if (fsm != null)
            fsm.UpdateState();
        if (hpBarCanvas.activeInHierarchy)
            UpdateHpBar();
    }

    public async void SetDeath()
    {
        death.SetActive(true);
        collider.enabled = false;
        targetMark.SetActive(true);
        targetMark.GetComponent<SpriteRenderer>().sprite = await ResourceManager.instance.LoadAsset<Sprite>("Role_" + userInfo.roleType.ToString(), eAddressableType.Thumbnail);
        minimapIcon.gameObject.SetActive(false);
        ChangeState<CharacterDeathState>();
        GameManager.instance.CheckBossRound(userInfo);
    }

    protected override T ChangeState<T>()
    {
        if (currentState is T)
        {
            if (!IsState<CharacterDeathState>())
                return base.ChangeState<T>();
            else
                return currentState == null ? null : (T)currentState;
        }

        return base.ChangeState<T>();
    }
}