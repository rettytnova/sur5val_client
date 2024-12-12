using UnityEngine;
using Ironcow;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class ServerSession : Session
{
    public int id = 0;

    public void LoginResponse(GamePacket gamePacket)
    {
        var response = gamePacket.LoginResponse;
        if (response.Success)
        {
            if (response.MyInfo != null)
            {
                UserInfo.myInfo = new UserInfo(response.MyInfo);
            }
            UIManager.Get<PopupLogin>().OnLoginEnd(response.Success);
        }
    }

    public void RegisterResponse(GamePacket gamePacket)
    {
        var response = gamePacket.RegisterResponse;
        UIManager.Get<PopupLogin>().OnRegisterEnd(response.Success);
    }

    // 방 생성
    public void CreateRoomResponse(GamePacket gamePacket)
    {
        var response = gamePacket.CreateRoomResponse;
        Debug.Log("failcode : " + response.FailCode.ToString());
        UIManager.Get<PopupRoomCreate>().OnRoomCreateResult(response.Success, response.Room);
    }

    // 방 목록 조회
    public void GetRoomListResponse(GamePacket gamePacket)
    {
        var response = gamePacket.GetRoomListResponse;
        UIManager.Get<UIMain>().SetRoomList(response.Rooms.ToList());
    }

    // 방 참가
    public void JoinRoomResponse(GamePacket gamePacket)
    {
        var response = gamePacket.JoinRoomResponse;
        if (response.Success)
        {
            UIManager.Show<UIRoom>(response.Room);
        }
    }

    // 랜덤 방 참가
    public void JoinRandomRoomResponse(GamePacket gamePacket)
    {
        var response = gamePacket.JoinRandomRoomResponse;
        if (response.FailCode != 0)
        {
            Debug.Log("failcode : " + response.FailCode.ToString());
        }
        else if (response.Success)
        {
            UIManager.Show<UIRoom>(response.Room);
        }
    }

    // 방 참가
    public void JoinRoomNotification(GamePacket gamePacket)
    {
        var response = gamePacket.JoinRoomNotification;
        if (response.JoinUser.Id != UserInfo.myInfo.id)
        {
            UIManager.Get<UIRoom>().AddUserInfo(response.JoinUser.ToUserInfo());
        }
    }

    // 방 나가기
    public void LeaveRoomResponse(GamePacket gamePacket)
    {
        var response = gamePacket.LeaveRoomResponse;
        if (response.Success)
        {
            UIManager.Hide<UIRoom>();
        }
    }

    // 방 나가기
    public void LeaveRoomNotification(GamePacket gamePacket)
    {
        var response = gamePacket.LeaveRoomNotification;
        UIManager.Get<UIRoom>().RemoveUserInfo(response.UserId);
    }

    public void GamePrepareResponse(GamePacket gamePacket)
    {
        var response = gamePacket.GamePrepareResponse;
        if (response.FailCode != 0)
        {
            UIManager.ShowAlert(response.FailCode.ToString(), "오류");
            Debug.Log("GamePrepareResponse Failcode : " + response.FailCode.ToString());
        }
    }

    public void GamePrepareNotification(GamePacket gamePacket)
    {
        var response = gamePacket.GamePrepareNotification;
        if (response.Room != null)
        {
            UIManager.Get<UIRoom>().SetRoomInfo(response.Room);
        }
        if (response.Room.Users != null)
        {
            UIManager.Get<UIRoom>().OnPrepare(response.Room.Users);
        }
    }

    public void GameStartResponse(GamePacket gamePacket)
    {
        var response = gamePacket.GameStartResponse;
        if (response.FailCode != 0)
        {
            UIManager.ShowAlert(response.FailCode.ToString(), "오류");
            Debug.Log("GameStartResponse Failcode : " + response.FailCode.ToString());
        }
    }

    // 게임 시작
    public async void GameStartNotification(GamePacket gamePacket)
    {
        var response = gamePacket.GameStartNotification;
        await SceneManager.LoadSceneAsync("Game");
        while (!UIManager.IsOpened<UIGame>())
        {
            await Task.Yield();
        }
        DataManager.instance.users.Clear();
        for (int i = 0; i < response.Users.Count; i++)
        {
            if (response.Users[i] == null) continue;
            var user = response.Users[i];
            var userinfo = user.ToUserInfo();
            if (UserInfo.myInfo.id == user.Id)
            {
                //userinfo = UserInfo.myInfo;                
                UserInfo.myInfo.UpdateUserInfo(user);
                DataManager.instance.users.Add(UserInfo.myInfo);
                DataManager.instance.userDict[UserInfo.myInfo.id] = UserInfo.myInfo;
            }
            else
            {
                DataManager.instance.users.Add(userinfo);
                DataManager.instance.userDict[userinfo.id] = userinfo;
            }
        }
        for (int i = 0; i < response.Users.Count; i++)
        {
            await GameManager.instance.OnCreateCharacter(DataManager.instance.users[i], i);
            GameManager.instance.isInit = true;
            GameManager.instance.characters[DataManager.instance.users[i].id].SetPosition(response.CharacterPositions[i].ToVector3());
        }
        GameManager.instance.OnGameStart();
        GameManager.instance.VisualHiddenRoad(false);
        GameManager.instance.SetGameState(response.GameState);
    }

    // 위치 업데이트
    public void PositionUpdateNotification(GamePacket gamePacket)
    {
        var response = gamePacket.PositionUpdateNotification;
        for (int i = 0; i < response.CharacterPositions.Count; i++)
        {
            if (GameManager.instance.characters[response.CharacterPositions[i].Id].userInfo.hp == 0)
            {
                continue;
            }

            if (GameManager.instance.characters != null && GameManager.instance.characters.ContainsKey(response.CharacterPositions[i].Id))
                GameManager.instance.characters[response.CharacterPositions[i].Id].SetMovePosition(response.CharacterPositions[i].ToVector3());
        }
    }

    // 카드 사용 응답
    public void UseCardResponse(GamePacket gamePacket)
    {
        var response = gamePacket.UseCardResponse;
        if (response.Success)
        {
            // if (UIManager.IsOpened<PopupDeck>())
            //     UIManager.Hide<PopupDeck>();
            // if (UIManager.IsOpened<PopupBattle>())
            //     UIManager.Hide<PopupBattle>();
            // UIGame.instance.SetSelectCard(null);
            // GameManager.instance.targetCharacter.OnSelect();
            // GameManager.instance.targetCharacter = null;
            if (UIManager.IsOpened<PopupDeck>())
            {
                UIManager.Get<PopupDeck>().SetList();
            }
        }
    }

    public async void UseCardNotification(GamePacket gamePacket)
    {
        var response = gamePacket.UseCardNotification;
        var card = response.CardType.GetCardData();
        if (card.isTargetCardSelection && response.UserId == UserInfo.myInfo.id)
        {
            await UIManager.Show<PopupCardSelection>(response.TargetUserId, card.rcode);
        }
        var use = DataManager.instance.users.Find(obj => obj.id == response.UserId);
        var target = DataManager.instance.users.Find(obj => obj.id == response.TargetUserId);
        var text = string.Format(response.TargetUserId != 0 ? "{0}유저가 {1}카드를 사용했습니다." : "{0}유저가 {1}카드를 {2}유저에게 사용했습니다.",
            use.nickname, response.CardType.GetCardData().displayName, target.nickname);
        UIGame.instance.SetNotice(text);
        if (response.UserId == UserInfo.myInfo.id && card.cardType == CardType.Bbang)
        {
            //UserInfo.myInfo.shotCount++;
            UIGame.instance.SetSelectCard(null);
        }

    }

    public void EquipCardNotification(GamePacket gamePacket)
    {
        var response = gamePacket.UseCardNotification;
        var userinfo = DataManager.instance.users.Find(obj => obj.id == response.UserId);
        userinfo.OnUseCard(response.CardType.GetCardRcode());
    }

    public void CardEffectNotification(GamePacket gamePacket)
    {
        var response = gamePacket.UseCardNotification;
        var use = DataManager.instance.users.Find(obj => obj.id == response.UserId);
        var target = DataManager.instance.users.Find(obj => obj.id == response.TargetUserId);
        var text = string.Format(response.TargetUserId != 0 ? "{0}유저가 {1}카드를 사용했습니다." : "{0}유저가 {1}카드를 {2}유저에게 사용했습니다.",
            use.nickname, response.CardType.GetCardData().displayName, target.nickname);
        UIGame.instance.SetNotice(text);
    }

    public async void FleaMarketPickResponse(GamePacket gamePacket)
    {
        var response = gamePacket.FleaMarketPickResponse;

        PopupPleaMarket popupPleaMarket = UIManager.Get<PopupPleaMarket>();
        if (popupPleaMarket == null)
        {
            popupPleaMarket = await UIManager.Show<PopupPleaMarket>();
        }

        // isInit = 플리마켓 팝업창의 cards가 0 보다 크면 true   
        if (!popupPleaMarket.isInitCards)
        {
            popupPleaMarket.SetCards(response.FleaMarketCardTypes);
        }
    }

    public void FleMarketCardPickResponse(GamePacket gamePacket)
    {
        var response = gamePacket.FleMarketCardPickResponse;

        Debug.Log($"마켓에서 카드 선택함 {response.HandCards}");
        var users = DataManager.instance.users;
        for (int i = 0; i < users.Count; i++)
        {
            if (users[i].id == response.UserId)
            {
                users[i].UpdateHandCard(response.HandCards);
                break;
            }
        }
    }

    public async void FleaMarketNotification(GamePacket gamePacket)
    {
        var response = gamePacket.FleaMarketNotification;
        var ui = UIManager.Get<PopupPleaMarket>();
        if (ui == null)
        {
            ui = await UIManager.Show<PopupPleaMarket>();
        }
        if (!ui.isInitCards)
            ui.SetCards(response.CardTypes);
        if (response.CardTypes.Count > response.PickIndex.Count)
            ui.OnSelectedCard(response.PickIndex);
        else
        {
            UIManager.Hide<PopupPleaMarket>();
            for (int i = 0; i < DataManager.instance.users.Count; i++)
            {
                var targetCharacter = GameManager.instance.characters[DataManager.instance.users[i].id];
                targetCharacter.OnChangeState<CharacterIdleState>();
            }
        }
    }

    public void ReactionResponse(GamePacket gamePacket)
    {
        var response = gamePacket.ReactionResponse;
        if (response.Success)
        {
            if (UIManager.IsOpened<PopupBattle>())
                UIManager.Hide<PopupBattle>();

            GameManager.instance.VisualHiddenRoad(true, (int)response.FailCode);
        }
    }

    // 카드 사용 등으로 인한 유저 정보 업데이트
    public async void UserUpdateNotification(GamePacket gamePacket)
    {
        var response = gamePacket.UserUpdateNotification;
        var users = DataManager.instance.users.UpdateUserData(response.User);
        if (!GameManager.isInstance || GameManager.instance.characters == null || GameManager.instance.characters.Count == 0) return;
        var myIndex = users.FindIndex(obj => obj.id == UserInfo.myInfo.id);
        for (int i = 0; i < users.Count; i++)
        {
            var targetCharacter = GameManager.instance.characters[users[i].id];
            if (!users[i].aliveState && users[i].hp <= 0)
            {
                GameManager.instance.userCharacter?.MoveCharacter(Vector2.zero);

                targetCharacter.SetDeath();
                UIGame.instance.SetDeath(users[i].id);
            }

            //targetCharacter.OnVisibleMinimapIcon(Util.GetDistance(myIndex, i, DataManager.instance.users.Count) + users[i].slotFar <= UserInfo.myInfo.slotRange && myIndex != i); // ������ �Ÿ��� �ִ� ���� �����ܸ� ǥ��

            if (users[i].id == UserInfo.myInfo.id)
            {
                var user = users[i];
                var targetId = user.characterData.StateInfo.StateTargetUserId;
                var targetInfo = DataManager.instance.users.Find(obj => obj.id == targetId);

                switch ((eCharacterState)users[i].characterData.StateInfo.State)
                {
                    case eCharacterState.NONE:
                        {
                            if (!targetCharacter.IsState<CharacterDeathState>())
                            {
                                targetCharacter.OnChangeState<CharacterIdleState>();
                            }

                            targetCharacter.OnChangeState<CharacterIdleState>();

                            if (UIManager.IsOpened<PopupPleaMarket>())
                                UIManager.Hide<PopupPleaMarket>();
                            if (UIManager.IsOpened<PopupBattle>())
                                UIManager.Hide<PopupBattle>();
                        }
                        break;
                }
            }
        }

        if (UIGame.instance != null)
        {
            UIGame.instance.UpdateUserSlot(users);
        }
    }

    // 턴 종료시 (phaseType 3) 카드 버리기
    public void DestroyCardResponse(GamePacket gamePacket)
    {
        var response = gamePacket.DestroyCardResponse;
        UIManager.Hide<PopupRemoveCardSelection>();
        UserInfo.myInfo.UpdateHandCard(response.HandCards);
        UIGame.instance.SetSelectCard();
        UIGame.instance.SetDeckCount();
    }

    // 페이즈 업데이트
    public void PhaseUpdateNotification(GamePacket gamePacket)
    {
        var response = gamePacket.PhaseUpdateNotification;
        if (UIGame.instance != null)
            GameManager.instance.SetGameState(response.PhaseType, response.NextPhaseAt);
        for (int i = 0; i < response.CharacterPositions.Count; i++)
        {
            GameManager.instance.characters[DataManager.instance.users[i].id].SetPosition(response.CharacterPositions[i].ToVector3());
        }
    }

    // 게임 종료
    public void GameEndNotification(GamePacket gamePacket)
    {
        var response = gamePacket.GameEndNotification;
        GameManager.instance.OnGameEnd();

        UIManager.Show<PopupResult>(response.Winners, response.WinType);
    }

    public void CardSelectResponse(GamePacket gamePacket)
    {
        var response = gamePacket.CardSelectResponse;
        if (response.Success)
        {
            UIManager.Hide<PopupCardSelection>();
        }
        else
        {
            Debug.Log("CardSelectResponse is failed");
        }
    }

    // 폭탄 넘기기
    public void PassDebuffResponse(GamePacket gamePacket)
    {
        var response = gamePacket.PassDebuffResponse;
        if (response.Success)
        {
            GameManager.instance.targetCharacter.OnSelect();
            GameManager.instance.targetCharacter = null;
            UIGame.instance.SetBombButton(false);
        }
    }

    // 폭탄 위기 시
    public void WarningNotification(GamePacket gamePacket)
    {
        var response = gamePacket.WarningNotification;
        UIGame.instance.SetBombAlert(response.WarningType == WarningType.BombWaning);
    }

    // 애니메이션 표시
    public async void AnimationNotification(GamePacket gamePacket)
        {
            var response = gamePacket.AnimationNotification;
            var target = GameManager.instance.characters[response.UserId].transform;
            switch (response.AnimationType)
            {
            case AnimationType.Sur5verAttackAnimation:
                {
                    // GameManager.instance.virtualCamera.Target.TrackingTarget = target;
                    var sur5verAttack = Instantiate(await ResourceManager.instance.LoadAsset<Transform>("Sur5verAttack", eAddressableType.Prefabs));
                    sur5verAttack.transform.position = target.position;
                }
                break;
            case AnimationType.MonsterAttackAnimation:
                {
                    // GameManager.instance.virtualCamera.Target.TrackingTarget = target;
                    var monsterAttack = Instantiate(await ResourceManager.instance.LoadAsset<Transform>("MonsterAttack", eAddressableType.Prefabs));
                    monsterAttack.transform.position = target.position;
                }
                break;
            case AnimationType.TwinmagicSkillAnimation:
                {
                    var twinmagicSkill = Instantiate(await ResourceManager.instance.LoadAsset<Transform>("TwinmagicSkill", eAddressableType.Prefabs));
                    twinmagicSkill.transform.position = target.position;
                }
                break;
            case AnimationType.MagicianFinalSkillAnimation:
                {
                    var magicianFinalSkill = Instantiate(await ResourceManager.instance.LoadAsset<Transform>("MagicianFinalSkill", eAddressableType.Prefabs));
                    magicianFinalSkill.transform.position = target.position;
                }
                break;
            case AnimationType.ChargeshotSkillAnimation:
                {
                    var chargeshotSkill = Instantiate(await ResourceManager.instance.LoadAsset<Transform>("ChargeshotSkill", eAddressableType.Prefabs));
                    chargeshotSkill.transform.position = target.position;
                }
                break;
            case AnimationType.BuffSkillAnimation:
                {
                    var buffSkill = Instantiate(await ResourceManager.instance.LoadAsset<Transform>("BuffSkill", eAddressableType.Prefabs));
                    buffSkill.transform.position = target.position;
                }
                break;
            case AnimationType.SpiritAttackAnimation:
                {
                    var spiritAttack = Instantiate(await ResourceManager.instance.LoadAsset<Transform>("SpiritAttack", eAddressableType.Prefabs));
                    spiritAttack.transform.position = target.position;
                }
                break;
            case AnimationType.RogueBasicSkill:
                {
                    var rogueBasicSkill = Instantiate(await ResourceManager.instance.LoadAsset<Transform>("RogueBasicSkill", eAddressableType.Prefabs));
                    rogueBasicSkill.transform.position = target.position;
                }
                break;
            case AnimationType.MasterSkillAnimation:
                {
                    var masterSkill = Instantiate(await ResourceManager.instance.LoadAsset<Transform>("MasterSkill", eAddressableType.Prefabs));
                    masterSkill.transform.position = target.position;
                }
                break;
            case AnimationType.WarriorExtendedSkillAnimation:
                {
                    var warriorExtendedSkill = Instantiate(await ResourceManager.instance.LoadAsset<Transform>("WarriorExtendedSkill", eAddressableType.Prefabs));
                    warriorExtendedSkill.transform.position = target.position;
                }
                break;
            case AnimationType.BossExtendedSkillAnimation:
                {
                    var bossExtendedSkill = Instantiate(await ResourceManager.instance.LoadAsset<Transform>("BossExtendedSkill", eAddressableType.Prefabs));
                    bossExtendedSkill.transform.position = target.position;
                }
                break;
        }
    }

    public void GlobalMessageResponse(GamePacket gamePacket)
    {
        var response = gamePacket.GlobalMessageResponse;

        var GameSceneUI = GameScene.GetInstance?.gameSceneUI;
        if (GameSceneUI == null)
        {
            return;
        }

        GameSceneUI.globalMessageBox.NewGlobalMessage(response.GlobalMessageType, response.GlobalMessage);
    }

    public void ChattingServerLoginResponse(ChattingPacket chattingPacket)
    {

    }

    public void ChattingServerCreateRoomResponse(ChattingPacket chattingPacket)
    {
        var response = chattingPacket.ChattingServerCreateRoomResponse;

        //var roomId = response.ChattingRoomId;

        //Debug.Log($"채팅방 생성 응답 받음 {response.ChattingRoomId}");
    }

    public void ChattingServerChatSendResponse(ChattingPacket chattingPacket)
    {
        var response = chattingPacket.ChattingServerChatSendResponse;

        var users = DataManager.instance.users;
        if (users != null)
        {
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].nickname == response.NickName)
                {
                    var targetCharacter = GameManager.instance.characters[users[i].id];
                    if (targetCharacter != null)
                    {
                        targetCharacter.ChattingMessage(response.ChatMessage);
                    }
                }
            }
        }
    }
}