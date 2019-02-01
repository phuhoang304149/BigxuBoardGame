using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

[System.Serializable]
public class BOL_Table_Info
{
    public static BOL_Table_Info instance
    {
        get
        {
            if (ins == null)
            {
                ins = new BOL_Table_Info();
            }
            return ins;
        }
    }
    static BOL_Table_Info ins;
    public static void SelfDestruction()
    {
        ins = null;
    }
    #region Variable recieve
    public bool hasLoadTableInfo;
    public long bet;
    public short mapid;
    public long TIME_PLAY;
    public sbyte status;

    public short sessionid0;
    public bool isReady0;
    public sbyte characterid0;
    public sbyte spell0;
    public short sessionid1;
    public bool isReady1;
    public sbyte characterid1;
    public sbyte spell1;

    // player 1 chair2: left;
    public short sessionid;
    public sbyte leftdatabaseid;
    public long leftuserid;
    public sbyte leftavatarids;
    public long leftGOLD;
    public string leftnameShow;
    public int leftwin;
    public int lefttie;
    public int leftdraw;
    public sbyte leftcharackterID;
    public short lefthp;
    public short leftmana;
    public short leftdamgeAttack1;
    public short leftdamgeAttack2;
    public short leftshield;
    public sbyte leftspell;
    public short leftmanaOfSkill1;
    public long leftdelayOfSkill1;
    public short leftmanaOfSkill2;
    public long leftdelayOfSkill2;
    public short leftmanaOfUtimate;
    public long leftdelayOfUtimate;
    public short leftmax_hp;
    public short leftmax_mana;
    public short leftmax_shield;
    public long lefttimeDelaySpell;
    public long lefttimeDelaySkill1;
    public long lefttimeDelaySkill2;
    public long lefttimeDelayUltimate;
    public long lefteffect1_time_stun;
    public long lefteffect2_time_doNotRecoverHP;
    public long lefteffect3_time_upDamage;
    public short lefteffect3_value_upDamage;
    public long lefteffect4_time_slowMatrix;
    public long lefteffect5_time_doNotMovePiece;
    public long lefteffect6_time_doNotChangePiece;
    public long lefteffect7_time_disableSpell;
    public long lefteffect8_time_freeze;
    public long lefteffect9_time_speedupMatrix;
    public long lefteffect10_time_hypnosis_pieceMoveBackwards;
    public long lefteffect11_time_backDamage;
    public long lefteffect12_time_avoidDamage;
    public long lefteffect13_time_downDamage;
    public short lefteffect13_value_downDamage;
    public long lefteffect14_time_virtual_hp;
    public short lefteffect14_value_virtual_hp;
    public long lefteffect15_time_bloodsucking;
    public short lefteffect15_perthousand_bloodsucking;
    public long lefteffect16_time_avoidEffect;
    public sbyte[] leftcurrentPiece;
    public sbyte[] leftnextPiece;
    public sbyte[,] leftmatrix = new sbyte[Constant.ROW, Constant.COL];
    // player 2 chair4: right;
    public short sessionid1s;
    public sbyte rightdatabaseid;
    public long rightuserid;
    public sbyte rightavatarids;
    public long rightGOLD;
    public string rightnameShow;
    public int rightwin;
    public int righttie;
    public int rightdraw;
    public sbyte rightcharackterID;
    public short righthp;
    public short rightmana;
    public short rightdamgeAttack1;
    public short rightdamgeAttack2;
    public short rightshield;
    public sbyte rightspell;
    public short rightmanaOfSkill1;
    public long rightdelayOfSkill1;
    public short rightmanaOfSkill2;
    public long rightdelayOfSkill2;
    public short rightmanaOfUtimate;
    public long rightdelayOfUtimate;
    public short rightmax_hp;
    public short rightmax_mana;
    public short rightmax_shield;
    public long righttimeDelaySpell;
    public long righttimeDelaySkill1;
    public long righttimeDelaySkill2;
    public long righttimeDelayUltimate;
    public long righteffect1_time_stun;
    public long righteffect2_time_doNotRecoverHP;
    public long righteffect3_time_upDamage;
    public short righteffect3_value_upDamage;
    public long righteffect4_time_slowMatrix;
    public long righteffect5_time_doNotMovePiece;
    public long righteffect6_time_doNotChangePiece;
    public long righteffect7_time_disableSpell;
    public long righteffect8_time_freeze;
    public long righteffect9_time_speedupMatrix;
    public long righteffect10_time_hypnosis_pieceMoveBackwards;
    public long righteffect11_time_backDamage;
    public long righteffect12_time_avoidDamage;
    public long righteffect13_time_downDamage;
    public short righteffect13_value_downDamage;
    public long righteffect14_time_virtual_hp;
    public short righteffect14_value_virtual_hp;
    public long righteffect15_time_bloodsucking;
    public short righteffect15_perthousand_bloodsucking;
    public long righteffect16_time_avoidEffect;
    public sbyte[] rightcurrentPiece;
    public sbyte[] rightnextPiece;
    public sbyte[,] rightmatrix = new sbyte[Constant.ROW, Constant.COL];
    #endregion
    public List<UserDataInGame> listOtherPlayerData;
    public List<GameObject> listPlayerInGame;
    public BOL_Table_Info()
    {
        listOtherPlayerData = new List<UserDataInGame>();
        listPlayerInGame = new List<GameObject>();
        hasLoadTableInfo = false;
    }
    public void SetDataTableInfo(MessageReceiving message)
    {
        InitListOtherUserDataInGame(message);
        bet = message.readLong();
        BOL_Manager.instance.betDefault = bet;
        BolNetworkReceiving.instance.betSetup = bet;
        BOL_ChoiceHero.instance.betInGame.text = bet.ToString();
        long betProcess = message.readLong();
        mapid = message.readShort();
        TIME_PLAY = message.readLong();
        status = message.readByte();
        Debugs.LogRed("bet " + bet + "betProcess" + betProcess + " |mapid|" + mapid + "  |Time_play|" + TIME_PLAY + " |status|" + status);
        if (status == 0)
        {
            BOL_Main_Controller.instance.ChairPosition = Constant.CHAIR_PLAYER;
            ReceiveWaitingGame(message);
        }
        else
        {
            BOL_Main_Controller.instance.ChairPosition = Constant.CHAIR_VIEWER;
            ReceivePlayingGame(message);
        }
        hasLoadTableInfo = true;
    }
    public void ReceiveWaitingGame(MessageReceiving message)
    {

        sessionid0 = message.readShort();
        isReady0 = message.readBoolean();
        characterid0 = message.readByte();
        spell0 = message.readByte();
        sessionid1 = message.readShort();
        isReady1 = message.readBoolean();
        characterid1 = message.readByte();
        spell1 = message.readByte();
#if TEST
        Debug.Log("isReady0" + isReady0 + "isReady1" + isReady1);

#endif

        if (isReady0)
        {
#if TEST
            Debug.Log("bên trái sẵn sàng");
#endif
            BOL_ChoiceHero.instance.btnChoiceChairLeft.ShowImageReady();
        }
        else
        {
#if TEST
            Debug.Log("bên trái không sẵn sàng");
#endif
            BOL_ChoiceHero.instance.btnChoiceChairLeft.HideImageReady();
        }
        if (isReady1)
        {
#if TEST
            Debug.Log("bên phải sẵn sàng");
#endif
            BOL_ChoiceHero.instance.btnChoiceChairRight.ShowImageReady();
        }
        else
        {
#if TEST
            Debug.Log("bên phải không sẵn sàng");
#endif
            BOL_ChoiceHero.instance.btnChoiceChairRight.HideImageReady();
        }
#if TEST
        Debug.Log("sessionid " + sessionid0 + "  " + sessionid1);
#endif
        if (sessionid0 >= 0)
        {
            BOL_ChoiceHero.instance.btnChoiceChairLeft.HideArrow();
            BOL_Main_Controller.instance.SpawnHeroWhenChoice(Constant.CHAIR_LEFT, characterid0, sessionid0);
            BOL_ChoiceHero.instance.btnChoiceChairLeft.SetSitdownSuccess();
            BOL_ChoiceHero.instance.btnChoiceChairLeft.btnStandUp.gameObject.SetActive(false);
            BOL_ChoiceHero.instance.btnChoiceChairLeft.btnSitdown.enabled = false;
            for (int i = 0; i < BOL_ShowPlayer_Manager.instance.listUserIngame.Count; i++)
            {
                if (sessionid0 == BOL_ShowPlayer_Manager.instance.listUserIngame[i].sessionId)
                {
#if TEST
                    Debug.Log("session " + BOL_ShowPlayer_Manager.instance.listUserIngame[i].sessionId
                    + " nameshow " + BOL_ShowPlayer_Manager.instance.listUserIngame[i].nameShowInGame
                    + " list count " + BOL_ShowPlayer_Manager.instance.listUserIngame.Count);
#endif
                    BOL_ChoiceHero.instance.btnChoiceChairLeft.infoPlayer.InitData(BOL_ShowPlayer_Manager.instance.listUserIngame[i]);
                }
            }
            BOL_ChoiceHero.instance.btnChoiceChairLeft.ShowInfoPlayer();
#if TEST
            Debug.Log("show bên trái" + sessionid0);
#endif
        }
        else
        {
            BOL_ChoiceHero.instance.btnChoiceChairLeft.ShowArrow();
            BOL_ChoiceHero.instance.btnChoiceChairRight.isChoiceChair = false;
            BOL_Main_Controller.instance.SpawnHeroWhenChoice(Constant.CHAIR_LEFT, -1, sessionid0);
            BOL_ChoiceHero.instance.btnChoiceChairLeft.btnSitdown.enabled = true;
        }
        if (sessionid1 >= 0)
        {
            BOL_ChoiceHero.instance.btnChoiceChairRight.HideArrow();
            BOL_Main_Controller.instance.SpawnHeroWhenChoice(Constant.CHAIR_RIGHT, characterid1, sessionid1);
            BOL_ChoiceHero.instance.btnChoiceChairRight.SetSitdownSuccess();
            BOL_ChoiceHero.instance.btnChoiceChairRight.btnStandUp.gameObject.SetActive(false);
            BOL_ChoiceHero.instance.btnChoiceChairRight.btnSitdown.enabled = false;
            for (int i = 0; i < BOL_ShowPlayer_Manager.instance.listUserIngame.Count; i++)
            {
                if (sessionid1 == BOL_ShowPlayer_Manager.instance.listUserIngame[i].sessionId)
                {
                    BOL_ChoiceHero.instance.btnChoiceChairRight.infoPlayer.InitData(BOL_ShowPlayer_Manager.instance.listUserIngame[i]);
#if TEST
                    Debug.Log("session " + BOL_ShowPlayer_Manager.instance.listUserIngame[i].sessionId
                    + " nameshow " + BOL_ShowPlayer_Manager.instance.listUserIngame[i].nameShowInGame
                    + " list count " + BOL_ShowPlayer_Manager.instance.listUserIngame.Count);
#endif
                }
            }
            BOL_ChoiceHero.instance.btnChoiceChairRight.ShowInfoPlayer();
#if TEST
            Debug.Log("show bên phải" + sessionid1);
#endif
        }
        else
        {
            BOL_Main_Controller.instance.SpawnHeroWhenChoice(Constant.CHAIR_RIGHT, -1, sessionid1);
            BOL_ChoiceHero.instance.btnChoiceChairRight.isChoiceChair = false;
            BOL_ChoiceHero.instance.btnChoiceChairRight.ShowArrow();
            BOL_ChoiceHero.instance.btnChoiceChairRight.btnSitdown.enabled = true;
        }
        BOL_ShowPlayer_Manager.instance.AddPlayerPlayGame(sessionid0, Constant.CHAIR_LEFT);
        BOL_ShowPlayer_Manager.instance.AddPlayerPlayGame(sessionid1, Constant.CHAIR_RIGHT);

    }
    public void ReceivePlayingGame(MessageReceiving message)
    {
        sessionid = message.readShort();
        leftdatabaseid = message.readByte();
        leftuserid = message.readLong();
        leftavatarids = message.readByte();
        leftGOLD = message.readLong();
        leftnameShow = message.readString();
        leftwin = message.readInt();
        lefttie = message.readInt();
        leftdraw = message.readInt();

        leftcharackterID = message.readByte();
        lefthp = message.readShort();
        leftmana = message.readShort();
        leftdamgeAttack1 = message.readShort();
        leftdamgeAttack2 = message.readShort();
        leftshield = message.readShort();
        leftspell = message.readByte();
        leftmanaOfSkill1 = message.readShort();
        leftdelayOfSkill1 = message.readLong();
        leftmanaOfSkill2 = message.readShort();
        leftdelayOfSkill2 = message.readLong();
        leftmanaOfUtimate = message.readShort();
        leftdelayOfUtimate = message.readLong();
        leftmax_hp = message.readShort();
        leftmax_mana = message.readShort();
        leftmax_shield = message.readShort();
        lefttimeDelaySpell = message.readLong();
        lefttimeDelaySkill1 = message.readLong();
        lefttimeDelaySkill2 = message.readLong();
        lefttimeDelayUltimate = message.readLong();
        lefteffect1_time_stun = message.readLong();
        lefteffect2_time_doNotRecoverHP = message.readLong();
        lefteffect3_time_upDamage = message.readLong();
        lefteffect3_value_upDamage = message.readShort();
        lefteffect4_time_slowMatrix = message.readLong();
        lefteffect5_time_doNotMovePiece = message.readLong();
        lefteffect6_time_doNotChangePiece = message.readLong();
        lefteffect7_time_disableSpell = message.readLong();
        lefteffect8_time_freeze = message.readLong();
        lefteffect9_time_speedupMatrix = message.readLong();
        lefteffect10_time_hypnosis_pieceMoveBackwards = message.readLong();
        lefteffect11_time_backDamage = message.readLong();
        lefteffect12_time_avoidDamage = message.readLong();
        lefteffect13_time_downDamage = message.readLong();
        lefteffect13_value_downDamage = message.readShort();
        lefteffect14_time_virtual_hp = message.readLong();
        lefteffect14_value_virtual_hp = message.readShort();
        lefteffect15_time_bloodsucking = message.readLong();
        lefteffect15_perthousand_bloodsucking = message.readShort();
        lefteffect16_time_avoidEffect = message.readLong();
        leftcurrentPiece = message.readMiniByte();
        leftnextPiece = message.readMiniByte();
        for (int i = 0; i < Constant.ROW; i++)
        {
            for (int j = 0; j < Constant.COL; j++)
            {
                leftmatrix[i, j] = message.readByte();

            }
        }
        // player 2 chair4: right;
        sessionid1s = message.readShort();
        rightdatabaseid = message.readByte();
        rightuserid = message.readLong();
        rightavatarids = message.readByte();
        rightGOLD = message.readLong();
        rightnameShow = message.readString();
        rightwin = message.readInt();
        righttie = message.readInt();
        rightdraw = message.readInt();


        rightcharackterID = message.readByte();
        righthp = message.readShort();
        rightmana = message.readShort();
        rightdamgeAttack1 = message.readShort();
        rightdamgeAttack2 = message.readShort();
        rightshield = message.readShort();
        rightspell = message.readByte();
        rightmanaOfSkill1 = message.readShort();
        rightdelayOfSkill1 = message.readLong();
        rightmanaOfSkill2 = message.readShort();
        rightdelayOfSkill2 = message.readLong();
        rightmanaOfUtimate = message.readShort();
        rightdelayOfUtimate = message.readLong();
        rightmax_hp = message.readShort();
        rightmax_mana = message.readShort();
        rightmax_shield = message.readShort();
        righttimeDelaySpell = message.readLong();
        righttimeDelaySkill1 = message.readLong();
        righttimeDelaySkill2 = message.readLong();
        righttimeDelayUltimate = message.readLong();
        righteffect1_time_stun = message.readLong();
        righteffect2_time_doNotRecoverHP = message.readLong();
        righteffect3_time_upDamage = message.readLong();
        righteffect3_value_upDamage = message.readShort();
        righteffect4_time_slowMatrix = message.readLong();
        righteffect5_time_doNotMovePiece = message.readLong();
        righteffect6_time_doNotChangePiece = message.readLong();
        righteffect7_time_disableSpell = message.readLong();
        righteffect8_time_freeze = message.readLong();
        righteffect9_time_speedupMatrix = message.readLong();
        righteffect10_time_hypnosis_pieceMoveBackwards = message.readLong();
        righteffect11_time_backDamage = message.readLong();
        righteffect12_time_avoidDamage = message.readLong();
        righteffect13_time_downDamage = message.readLong();
        righteffect13_value_downDamage = message.readShort();
        righteffect14_time_virtual_hp = message.readLong();
        righteffect14_value_virtual_hp = message.readShort();
        righteffect15_time_bloodsucking = message.readLong();
        righteffect15_perthousand_bloodsucking = message.readShort();
        righteffect16_time_avoidEffect = message.readLong();
        rightcurrentPiece = message.readMiniByte();
        rightnextPiece = message.readMiniByte();
        for (int i = 0; i < Constant.ROW; i++)
        {
            for (int j = 0; j < Constant.COL; j++)
            {
                rightmatrix[i, j] = message.readByte();
            }
        }
        Debugs.Log(">>>>> ReceivePlayingGame >>>>> sessionid0 " + sessionid0 + " >>>>>>>>>> sessionid1 " + sessionid1);

        //BOL_Main_Controller.instance.InitDataBattle();
        //BOL_Main_Controller.instance.InitDataShowHeroAndButton();
        if (BOL_Main_Controller.instance.ChairPosition == Constant.CHAIR_VIEWER || BOL_Main_Controller.instance.ChairPosition == Constant.CHAIR_PLAYER)
        {
            BOL_Main_Controller.instance.SpawnHeroWhenChoice(Constant.CHAIR_LEFT, leftcharackterID, sessionid);
            BOL_Main_Controller.instance.SpawnHeroWhenChoice(Constant.CHAIR_RIGHT, rightcharackterID, sessionid1s);

            //BOL_ChoiceHero.instance.btnChoiceChairLeft.SpawnHerowhenchoice(leftcharackterID,sessionid);
            //BOL_ChoiceHero.instance.btnChoiceChairRight.SpawnHerowhenchoice(leftcharackterID,sessionid1s);
        }
        BolNetworkReceiving.instance.health_left_df = leftmax_hp;
        BolNetworkReceiving.instance.mana_left_df = leftmax_mana;
        BolNetworkReceiving.instance.shield_left_df = leftmax_shield;
        BolNetworkReceiving.instance.health_right_df = rightmax_hp;
        BolNetworkReceiving.instance.mana_right_df = rightmax_mana;
        BolNetworkReceiving.instance.shield_right_df = rightmax_shield;
        BolNetworkReceiving.instance.HeroIDLeft = leftcharackterID;
        BolNetworkReceiving.instance.HeroIDRight = rightcharackterID;
        BOL_Main_Controller.instance.StartInitData(BOL_Main_Controller.instance.InitDataPlaying, BOL_Main_Controller.instance.ActionPlaying);
        BOL_Main_Controller.instance._BOL_PlayBattle_left.SetValuePiece(leftnextPiece, BOL_Battle_PlayerInGame.NEXT_PIECE);
        BOL_Main_Controller.instance._BOL_PlayBattle_left.SetPostionPiece(leftcurrentPiece, 1, 0, 3);
        BOL_Main_Controller.instance._BOL_PlayBattle_left.SetValueHPorMP(BOL_Battle_PlayerInGame.HEALTH, lefthp, BolNetworkReceiving.instance.health_left_df, Constant.CHAIR_LEFT);
        BOL_Main_Controller.instance._BOL_PlayBattle_left.SetValueHPorMP(BOL_Battle_PlayerInGame.MANA, leftmana, BolNetworkReceiving.instance.mana_left_df, Constant.CHAIR_LEFT);
        BOL_Main_Controller.instance._BOL_PlayBattle_left.SetValueHPorMP(BOL_Battle_PlayerInGame.SHIELD, leftshield, BolNetworkReceiving.instance.shield_left_df, Constant.CHAIR_LEFT);
        BOL_Main_Controller.instance._BOL_PlayBattle_right.SetValuePiece(rightnextPiece, BOL_Battle_PlayerInGame.NEXT_PIECE);
        BOL_Main_Controller.instance._BOL_PlayBattle_right.SetPostionPiece(rightcurrentPiece, 1, 0, 3);
        BOL_Main_Controller.instance._BOL_PlayBattle_right.SetValueHPorMP(BOL_Battle_PlayerInGame.HEALTH, righthp, BolNetworkReceiving.instance.health_right_df, Constant.CHAIR_RIGHT);
        BOL_Main_Controller.instance._BOL_PlayBattle_right.SetValueHPorMP(BOL_Battle_PlayerInGame.MANA, rightmana, BolNetworkReceiving.instance.mana_right_df, Constant.CHAIR_RIGHT);
        BOL_Main_Controller.instance._BOL_PlayBattle_right.SetValueHPorMP(BOL_Battle_PlayerInGame.SHIELD, rightshield, BolNetworkReceiving.instance.shield_right_df, Constant.CHAIR_RIGHT);
        BOL_PlaySkill_Controller.instance.isStartGame = true;
        BOL_PlaySkill_Controller.instance.InitData();
        if (BOL_Main_Controller.instance.ChairPosition == Constant.CHAIR_PLAYER || BOL_Main_Controller.instance.ChairPosition == Constant.CHAIR_VIEWER)
        {
#if TEST
            Debug.Log(leftmatrix.Length + "  " + rightmatrix.Length);
            Debug.Log(leftcharackterID + "  " + rightcharackterID);
#endif

            for (int i = 0; i < Constant.ROW; i++)
            {
                for (int j = 0; j < Constant.COL; j++)
                {
                    BOL_Main_Controller.instance._BOL_PlayBattle_left.AddPieceInMatrix(leftmatrix[i, j], i, j);
                    BOL_Main_Controller.instance._BOL_PlayBattle_right.AddPieceInMatrix(rightmatrix[i, j], i, j);
                }
            }
        }

        BOL_ShowPlayer_Manager.instance.AddPlayerPlayGame(sessionid, Constant.CHAIR_LEFT);
        BOL_ShowPlayer_Manager.instance.AddPlayerPlayGame(sessionid1s, Constant.CHAIR_RIGHT);
        for (int i = 0; i < BOL_ShowPlayer_Manager.instance.listUserIngame.Count; i++)
        {
            if (sessionid == BOL_ShowPlayer_Manager.instance.listUserIngame[i].sessionId)
            {
#if TEST
                Debug.Log("session " + BOL_ShowPlayer_Manager.instance.listUserIngame[i].sessionId
                + " nameshow " + BOL_ShowPlayer_Manager.instance.listUserIngame[i].nameShowInGame
                + " list count " + BOL_ShowPlayer_Manager.instance.listUserIngame.Count);
#endif
                BOL_ChoiceHero.instance.btnChoiceChairLeft.infoPlayer.InitData(BOL_ShowPlayer_Manager.instance.listUserIngame[i]);
            }
        }
        BOL_ChoiceHero.instance.btnChoiceChairLeft.ShowInfoPlayer();
        for (int i = 0; i < BOL_ShowPlayer_Manager.instance.listUserIngame.Count; i++)
        {
            if (sessionid1s == BOL_ShowPlayer_Manager.instance.listUserIngame[i].sessionId)
            {
#if TEST
                Debug.Log("session " + BOL_ShowPlayer_Manager.instance.listUserIngame[i].sessionId
                + " nameshow " + BOL_ShowPlayer_Manager.instance.listUserIngame[i].nameShowInGame
                + " list count " + BOL_ShowPlayer_Manager.instance.listUserIngame.Count);
#endif
                BOL_ChoiceHero.instance.btnChoiceChairRight.infoPlayer.InitData(BOL_ShowPlayer_Manager.instance.listUserIngame[i]);
            }
        }
        BOL_ChoiceHero.instance.btnChoiceChairRight.ShowInfoPlayer();
    }
    public void InitListOtherUserDataInGame(MessageReceiving _mess)
    {
        listOtherPlayerData = new List<UserDataInGame>();
        UserDataInGame _usedata = null;
        sbyte _maxViewer = _mess.readByte();
        for (int i = 0; i < (int)_maxViewer; i++)
        {
            sbyte _caseCheck = _mess.readByte(); //(nếu giá trị -1 thì không đọc data dưới --> tiếp tục vòng for)
            if (_caseCheck >= 0)
            {
                short _sessionId = _mess.readShort();
                _usedata = new UserDataInGame(_mess, _sessionId, (sbyte)i);
                BOL_ShowPlayer_Manager.instance.InitUserInroom(_usedata);
                if (_usedata.sessionId != DataManager.instance.userData.sessionId)
                {
                    listOtherPlayerData.Add(_usedata);
                }
                else
                {
                    DataManager.instance.userData.CastToUserDataInGame().index = (sbyte)i;
                    AchievementDetail _achievementDetail = DataManager.instance.achievementData.GetAchievementDetail(IMiniGameInfo.Type.BattleOfLegend);
                    if (_achievementDetail != null)
                    {
                        _achievementDetail.countWin = _usedata.win;
                        _achievementDetail.countDraw = _usedata.tie;
                        _achievementDetail.countLose = _usedata.lose;
                        // Debug.Log(_usedata.win + " - " + _usedata.tie + " - " + _usedata.lose);
                    }
                    else
                    {
                        Debug.LogError(">>> _achievementDetail is null");
                    }
                }
            }
            else
            {
                _usedata = new UserDataInGame();
                listOtherPlayerData.Add(_usedata);
            }
            //Debug.LogError(_usedata.sessionId + " - " + DataManager.instance.userData.sessionId);
        }

        sbyte numberChairs = _mess.readByte();
        for (int i = 0; i < numberChairs; i++)
        {
            sbyte checkCase = _mess.readByte();
            if (checkCase >= 0)
            {
                short sessionIds = _mess.readShort();
            }
        }
    }
    public void SetUpUserJoinGame(MessageReceiving _mess)
    {
        short _sessionId = _mess.readShort();
        sbyte _viewerId = _mess.readByte();
        sbyte _myIndexChair = DataManager.instance.userData.CastToUserDataInGame().index;

        if (_viewerId == _myIndexChair)
        {
#if TEST
            Debug.LogError(">>> Chỗ này tao đang ngồi: " + _viewerId);
#endif
        }
        else if (_viewerId > _myIndexChair)
        {
            _viewerId--;
        }
        UserDataInGame _usedata = new UserDataInGame(_mess, _sessionId, _viewerId);
        if (listOtherPlayerData[_viewerId].sessionId >= 0)
        {
#if TEST
            Debug.LogError(">>> Chỗ này đã có người rồi: " + _viewerId);
#endif
            return;
        }
        if (_usedata.sessionId != DataManager.instance.userData.sessionId)
        {
            listOtherPlayerData[_viewerId] = _usedata;
#if TEST
            Debug.Log(">>> Có người chơi " + listOtherPlayerData[_viewerId].nameShowInGame + " vào bàn tại vị trí " + _viewerId);
#endif
            if (BOL_ShowPlayer_Manager.ins != null)
            {
                BOL_ShowPlayer_Manager.instance.InitUserInroom(_usedata);
            }
#if TEST
            PopupManager.Instance.CreateToast(_usedata.nameShowInGame + " is join game");
#endif

        }
        else
        {
#if TEST
            Debug.LogError(">>> Trả session ID tào lao: " + _sessionId);
#endif
        }
    }
    public void SetUpUserLeftGame(MessageReceiving _mess)
    {
        short _sessionId = _mess.readShort();
        for (int i = 0; i < listOtherPlayerData.Count; i++)
        {
            if (listOtherPlayerData[i].IsEqual(_sessionId))
            {
#if TEST
                Debug.Log(">>> Có người chơi " + listOtherPlayerData[i].nameShowInGame + " thoát bàn tại vị trí " + i);
#endif
                if (BOL_ShowPlayer_Manager.ins != null)
                {
                    for (int j = 0; j < BOL_ShowPlayer_Manager.instance.listUserIngame.Count; j++)
                    {
                        if (_sessionId == BOL_ShowPlayer_Manager.instance.listUserIngame[j].sessionId)
                        {
                            BOL_ShowPlayer_Manager.instance.listUserIngame[j] = new UserDataInGame();
                            break;
                        }
                    }
                }
                else
                {
#if TEST
                    Debug.Log("BOL_ShowPlayer_Manager.ins = null");
#endif
                }
                listOtherPlayerData[i] = new UserDataInGame();
                return;
            }
        }
#if TEST
        Debug.LogError(">>> Không tìm thấy session ID: " + _sessionId);
#endif
    }
}
