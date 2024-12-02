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

    [SerializeField] private float speed = 3;

    [HideInInspector] public UserInfo userInfo;

    public bool isPlayable { get => characterType == eCharacterType.playable; }
    public Vector2 dir;
    public float Speed { get => speed; }
    public bool isInside;

    private void Awake()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        if (characterType == eCharacterType.npc) minimapIcon.gameObject.SetActive(false);
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

    public void SetTargetMark()
    {
        targetMark.SetActive(true);
    }

    public void OnVisibleMinimapIcon(bool visible)
    {
        if (characterType == eCharacterType.non_playable)
            minimapIcon.gameObject.SetActive(visible && !isInside);
        else
            minimapIcon.gameObject.SetActive(false);
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Map"))
        {
            if (characterType == eCharacterType.playable)
            {
                if (!isInside)
                {
                    Debug.Log("OnTriggerEnter2D: Building In");
                    isInside = true;                    
                    var BuildingCover = collision.gameObject.transform.GetChild(0);
                    GameManager.instance.SetMapInside(BuildingCover, isInside);
                }
                if (userInfo != null)
                    OnVisibleMinimapIcon(Util.GetDistance(UserInfo.myInfo.index, userInfo.index, DataManager.instance.users.Count)
                        + userInfo.slotFar <= UserInfo.myInfo.slotRange && userInfo.id != UserInfo.myInfo.id);
            }
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("BuildingFront"))
        {
            if (characterType == eCharacterType.playable)
            {
                if (isInside)
                {
                    Debug.Log("OnTriggerEnter2D: Building Out");
                    isInside = false;
                    var BuildingCover = collision.gameObject.transform.parent.GetChild(collision.transform.GetSiblingIndex() - 1).GetChild(0);
                    GameManager.instance.SetMapInside(BuildingCover, isInside);
                }
                if (userInfo != null)
                    OnVisibleMinimapIcon(Util.GetDistance(UserInfo.myInfo.index, userInfo.index, DataManager.instance.users.Count)
                        + userInfo.slotFar <= UserInfo.myInfo.slotRange && userInfo.id != UserInfo.myInfo.id);
            }
        }
        else if(collision.gameObject.layer == LayerMask.NameToLayer("Store"))
        {
            if (characterType == eCharacterType.playable)
                Debug.Log("OnTriggerEnter2D: Store");
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Extrance"))
        {
            //if (characterType == eCharacterType.playable && userInfo.roleType == eRoleType.bodyguard)
            if (characterType == eCharacterType.playable)
            {
                GamePacket packet = new GamePacket();
                packet.ReactionRequest = new C2SReactionRequest() { ReactionType = ReactionType.NoneReaction };
                SocketManager.instance.Send(packet);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Store"))
        {
            if (characterType == eCharacterType.playable)
                Debug.Log("OnTriggerExit2D: Store");
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Character>(out var character))
        {
            if (!SocketManager.instance.isConnected && character == GameManager.instance.userCharacter &&
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
    }

    public async void SetDeath()
    {
        death.SetActive(true);
        collider.enabled = false;
        targetMark.SetActive(true);
        targetMark.GetComponent<SpriteRenderer>().sprite = await ResourceManager.instance.LoadAsset<Sprite>("Role_" + userInfo.roleType.ToString(), eAddressableType.Thumbnail);
        minimapIcon.gameObject.SetActive(false);
        ChangeState<CharacterDeathState>();
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