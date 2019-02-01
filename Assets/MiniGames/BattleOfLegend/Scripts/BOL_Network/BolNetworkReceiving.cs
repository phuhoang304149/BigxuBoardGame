
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BolNetworkReceiving : MyGamePlayData
{
    public static BolNetworkReceiving instance
    {
        get
        {
            if (ins == null)
            {
                ins = new BolNetworkReceiving();
            }
            return ins;
        }
    }
    static BolNetworkReceiving ins;
    public BolNetworkReceiving() { }
    public static void SelfDestruction()
    {
        ins = null;
    }
    //public const float rateFoot = 0.25f;
    public long betSetup;
    public const float ratioFootCol = -0.25f;
    public const float ratioFootRow = 0.25f;
    int tempHeroLeft = 12;
    int tempHeroRight = 12;
    int tempSpellLeft;
    int tempSpellRight;
    public const float timeTween = 0.04f;
    public short health_left_df;
    public short health_right_df;
    public short mana_left_df;
    public short mana_right_df;
    public short shield_left_df;
    public short shield_right_df;
    public int HeroIDLeft, HeroIDRight;
    GameObject[,] matrixTempLeft = new GameObject[Constant.ROW, Constant.COL];
    GameObject[,] matrixTempRight = new GameObject[Constant.ROW, Constant.COL];
    bool hadGetAchievementDetail;

    PopupMessageController popupSetbetOK;
    PopupMessageController popupSetbetFail;
    PopupMessageController popupSitdownFail;
    public void SetupReceive()
    {
#if TEST
        Debug.Log("**********Setup receive**********");
#endif
        //NetworkGlobal.instance.instanceRealTime.ResumeReceiveMessage();
        //Dùng cho mục chat
        // NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_SERVER_UPDATE_LIST_VIEWER, S_GAMEPLAY_SERVER_UPDATE_LIST_VIEWER);
        //Cấu hình trước khi vào trận

        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_SETBET, S_GAMEPLAY_SETBET);
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_PLAYER_SETBET, S_GAMEPLAY_PLAYER_SETBET);
        //NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_CHANGE_POSITION_CHAIR, S_GAMEPLAY_CHANGE_POSITION_CHAIR);
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_GET_CHAIR_STATUS, S_GAMEPLAY_GET_CHAIR_STATUS);
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_XHCD_CHANGE_CHARACTER, S_XHCD_CHANGE_CHARACTER);
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_XHCD_PLAYER_CHANGE_CHARACTER, S_XHCD_PLAYER_CHANGE_CHARACTER);
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_READY, S_GAMEPLAY_READY);
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_PLAYER_READY, S_GAMEPLAY_PLAYER_READY);
        //Move
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_XHCD_MOVE_LEFT, S_MOVE_ERROR);
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_XHCD_PLAYER_MOVE_LEFT, S_XHCD_PLAYER_MOVE);
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_XHCD_MOVE_RIGHT, S_MOVE_ERROR);
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_XHCD_PLAYER_MOVE_RIGHT, S_XHCD_PLAYER_MOVE);
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_XHCD_MOVE_DOWN, S_MOVE_ERROR);
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_XHCD_PLAYER_MOVE_DOWN, S_XHCD_PLAYER_MOVE_DOWN);
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_XHCD_CHANGE_PIECE_STATE, S_MOVE_ERROR);
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_XHCD_PLAYER_CHANGE_PIECE_STATE, S_XHCD_PLAYER_CHANGE_PIECE_STATE);

        ////Matrix
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_XHCD_CURRENT_PIECE, S_XHCD_CURRENT_PIECE);
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_XHCD_ADD_PIECE_TO_MATRIX_AND_NEXT_PIECE,
        (MessageReceiving) =>
        {
            S_XHCD_ADD_PIECE_TO_MATRIX_AND_NEXT_PIECE(MessageReceiving);
        });
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_XHCD_MAXTRIX_DATA,
        (MessageReceiving) =>
        {
            S_XHCD_MAXTRIX_DATA(MessageReceiving);
        });
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_XHCD_PIECE_FALLING,
        (MessageReceiving) =>
        {
            S_XHCD_PIECE_FALLING(MessageReceiving);
        });
        //Fighting
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_XHCD_PieceBreak_ATTACK_1,
        (MessageReceiving) =>
        {
            S_XHCD_PieceBreak_ATTACK_1(MessageReceiving);
        });
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_XHCD_PieceBreak_ATTACK_2,
        (MessageReceiving) =>
        {
            S_XHCD_PieceBreak_ATTACK_2(MessageReceiving);
        });
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_XHCD_PieceBreak_HP,
        (MessageReceiving) =>
        {
            S_XHCD_PieceBreak_HP(MessageReceiving);
        });
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_XHCD_PieceBreak_MANA,
        (MessageReceiving) =>
        {
            S_XHCD_PieceBreak_MANA(MessageReceiving);
        });
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_XHCD_PieceBreak_SHIELD,
        (MessageReceiving) =>
        {
            S_XHCD_PieceBreak_SHIELD(MessageReceiving);
        });
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_XHCD_PieceBreak_SPECIAL,
        (MessageReceiving) =>
        {
            S_XHCD_PieceBreak_SPECIAL(MessageReceiving);
        });

        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_XHCD_PLAYER_USE_SPELL, S_XHCD_PLAYER_USE_SPELL);
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_XHCD_USE_SPELL, S_XHCD_USE_SPELL_ERROR);
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_XHCD_PLAYER_CALLSKILL_1, (MessageReceiving) =>
        {
            S_XHCD_PLAYER_CALLSKILL(MessageReceiving, CMD_REALTIME.S_XHCD_PLAYER_CALLSKILL_1);
        });
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_XHCD_CALLSKILL_1, S_XHCD_CALLSKILL_ERROR);
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_XHCD_PLAYER_CALLSKILL_2, (MessageReceiving) =>
        {
            S_XHCD_PLAYER_CALLSKILL(MessageReceiving, CMD_REALTIME.S_XHCD_PLAYER_CALLSKILL_2);
        });
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_XHCD_CALLSKILL_2, S_XHCD_CALLSKILL_ERROR);
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_XHCD_PLAYER_CALLSKILL_ULTIMATE, (MessageReceiving) =>
        {
            S_XHCD_PLAYER_CALLSKILL(MessageReceiving, CMD_REALTIME.S_XHCD_PLAYER_CALLSKILL_ULTIMATE);
        });
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_XHCD_CALLSKILL_ULTIMATE, S_XHCD_CALLSKILL_ERROR);
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_PLAYER_JOIN_GAME, S_GAMEPLAY_PLAYER_JOIN_GAME);
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_PLAYER_LEFT_GAME, S_GAMEPLAY_PLAYER_LEFT_GAME);
        //start and finish Game
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_START_GAME, S_GAMEPLAY_START_GAME);
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_FINISH_GAME, S_GAMEPLAY_FINISH_GAME);
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_PLAYER_GET_LOSE, S_GAMEPLAY_PLAYER_GET_LOSE);
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_CHAT_IN_TABLE, S_GAMEPLAY_CHAT_IN_TABLE);
        // CMD mới
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_SITDOWN, S_GAMEPLAY_SITDOWN);
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_PLAYER_SITDOWN, S_GAMEPLAY_PLAYER_SITDOWN);
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_STANDUP, S_GAMEPLAY_STANDUP);
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_PLAYER_STANDUP, S_GAMEPLAY_PLAYER_STANDUP);
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_onPlayerAddGold, S_onPlayerAddGold);
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_SET_PARENT, S_PlayerSetParent);

        RegisterActionAlertUpdateServer();
    }
    #region start, finish, info
    void S_MOVE_ERROR(MessageReceiving message)
    {
        sbyte caseCheck = message.readByte();
        if (caseCheck > 0)
        {
            switch (caseCheck)
            {
                case -1:
#if TEST
                    Debug.Log("game o trạng thái đang không chơi hoặc đang view");
                    PopupManager.Instance.CreateToast("Game không chơi hoặc dang view");
#endif
                    break;
                case -2:
#if TEST
                    Debug.Log("đóng băng");
                    PopupManager.Instance.CreateToast("đóng băng");
#endif
                    break;
                case -3:
#if TEST
                    Debug.Log("stun");
                    PopupManager.Instance.CreateToast("stun");
#endif

                    break;
                case -4:
#if TEST
                    Debug.Log("cấm đổi piece");
                    PopupManager.Instance.CreateToast("cấm đổi piece");
#endif
                    break;
                case -5:
#if TEST
                    Debug.Log("cấm di chuyển piece");
                    PopupManager.Instance.CreateToast("cấm di chuyển piece");
#endif
                    break;
            }
        }
    }
    void S_GAMEPLAY_SETBET(MessageReceiving message)
    {
        sbyte isSetOK = message.readByte();
        if (popupSetbetFail != null)
        {
            popupSetbetFail.Close();
        }
        switch (isSetOK)
        {
            case 1:
#if TEST
                Debug.Log("đôi cược thành công " + isSetOK);
#endif

                break;
            case -1:
#if TEST
                Debug.Log("không được đổi bet < 0" + isSetOK);
#endif

                popupSetbetFail = PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kWarning)
                                         , string.Format(MyLocalize.GetString("BOL/Message_PlayGame_SetBetFail"), MyConstant.GetMoneyString(BOL_Table_Info.instance.bet, 9999))
                                         , "-1"
                                         , MyLocalize.GetString(MyLocalize.kOk)
                                         , () =>
                                         {
                                             BOL_ChoiceHero.instance.betInGame.text = betSetup.ToString();
                                         });
                break;
            case -2:
#if TEST
                Debug.Log("không đủ gold " + isSetOK);
#endif
                popupSetbetFail = PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kWarning)
                        , string.Format(MyLocalize.GetString("BOL/Message_PlayGame_NotEnoughMoney"))
                        , "-2"
                        , MyLocalize.GetString(MyLocalize.kOk)
                        , () =>
                        {
                            BOL_ChoiceHero.instance.betInGame.text = betSetup.ToString();
                        });
                break;
            case -3:
#if TEST
                Debug.Log("không phải trạng thái dang chờ");
#endif

                break;
            case -4:
#if TEST
                Debug.Log("phải ngồi mới được settup" + isSetOK);
#endif
                popupSetbetFail = PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kWarning)
                                 , string.Format(MyLocalize.GetString("BOL/Message_PlayGame_SetBetFail_notSitdown"))
                                 , "-4"
                                 , MyLocalize.GetString(MyLocalize.kOk)
                                 , () =>
                                 {
                                     BOL_ChoiceHero.instance.betInGame.text = betSetup.ToString();
                                 });
                break;
        }
    }
    void S_GAMEPLAY_PLAYER_SETBET(MessageReceiving message)
    {
        short sessionID = message.readShort();
        betSetup = message.readLong();
        BOL_ChoiceHero.instance.betInGame.text = betSetup.ToString();
        if (popupSetbetOK != null)
        {
            popupSetbetOK.Close();
        }
        if (sessionID == DataManager.instance.userData.sessionId)
        {
            for (int i = 0; i < BOL_ShowPlayer_Manager.instance.listUserPlayGame.Length; i++)
            {
                if (sessionID == BOL_ShowPlayer_Manager.instance.listUserPlayGame[i].sessionId)
                {
                    string nameshowChange = BOL_ShowPlayer_Manager.instance.listUserPlayGame[i].nameShowInGame;
                    popupSetbetOK = PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kWarning)
                                              , string.Format(MyLocalize.GetString("BOL/Message_Player_SetBetOK"), nameshowChange, MyConstant.GetMoneyString(betSetup, 9999))
                                              , string.Empty
                                              , MyLocalize.GetString(MyLocalize.kOk)
                                              , null);

                    break;
                }
            }

        }
        else
        {
            for (int i = 0; i < BOL_ShowPlayer_Manager.instance.listUserPlayGame.Length; i++)
            {
                if (sessionID == BOL_ShowPlayer_Manager.instance.listUserPlayGame[i].sessionId)
                {
                    string nameshowChange = BOL_ShowPlayer_Manager.instance.listUserPlayGame[i].nameShowInGame;
                    PopupManager.Instance.CreateToast(string.Format(MyLocalize.GetString("BOL/Message_Player_SetBetOK"), nameshowChange, MyConstant.GetMoneyString(betSetup, 9999)));
                    break;
                }
            }

        }
    }
    void S_XHCD_CHANGE_CHARACTER(MessageReceiving message)
    {
        sbyte errorCase = message.readByte();
        switch (errorCase)
        {
            case 1:
                {
#if TEST
                    Debug.LogError("chọn tướng ko có trong danh sách-->errorCase  " + errorCase);
#endif
                    break;
                }
            case 2:
                {
#if TEST
                    Debug.LogError("chưa chon chỗ hoac game dang choi-->errorCase  " + errorCase);
#endif
                    break;
                }
        }
    }
    void S_XHCD_PLAYER_CHANGE_CHARACTER(MessageReceiving message)
    {
        short sessionId = message.readShort();
        sbyte chairId = message.readByte();
        sbyte characterid = message.readByte();
        sbyte spell = message.readByte();
#if TEST
        Debug.Log("chọn tướng thành công-->chairid " + chairId + " characterid " + characterid + "  spell  " + spell);
        Debug.Log("tempheroLeft " + tempHeroLeft + " temheroright " + tempHeroRight);
#endif
        BOL_Main_Controller.instance.SpawnHeroWhenChoice(chairId, characterid, sessionId);
    }
    void S_GAMEPLAY_READY(MessageReceiving message)
    {
        sbyte indexofchair = message.readByte();
        sbyte status = message.readByte();
        if (BOL_ChoiceHero.ins != null)
        {
            BOL_ChoiceHero.instance.ReceiveReadyGame(false);
        }
        else
        {

        }
#if TEST
        Debug.Log("ready failed " + indexofchair + " " + status);
#endif
    }
    void S_GAMEPLAY_PLAYER_READY(MessageReceiving message)
    {
        sbyte indexOfChair = message.readByte();
        bool isReady = message.readBoolean();
#if TEST
        Debug.Log(Debugs.ColorString(">>>>>player tai vị trí " + indexOfChair + " ready " + isReady, Color.red));
        Debug.Log(">>>>" + BOL_Main_Controller.instance.ChairPosition);
#endif
        if (indexOfChair == BOL_Main_Controller.instance.ChairPosition)
        {
            BOL_ChoiceHero.instance.ReceiveReadyGame(isReady);
        }
        switch (indexOfChair)
        {
            case (sbyte)Constant.CHAIR_LEFT:
                if (isReady)
                {
#if TEST
                    Debug.Log(" bên trái sẵn sàng");
#endif
                    BOL_ChoiceHero.instance.btnChoiceChairLeft.ShowImageReady();
                }
                else
                {
#if TEST
                    Debug.Log(" bên trái  chưa sẵn sàng");
#endif
                    BOL_ChoiceHero.instance.btnChoiceChairLeft.HideImageReady();
                }
                break;
            case (sbyte)Constant.CHAIR_RIGHT:
                if (isReady)
                {
#if TEST
                    Debug.Log(" bên phải  sẵn sàng");
#endif
                    BOL_ChoiceHero.instance.btnChoiceChairRight.ShowImageReady();
                }
                else
                {
#if TEST
                    Debug.Log(" bên phải  chưa sẵn sàng");
#endif
                    BOL_ChoiceHero.instance.btnChoiceChairRight.HideImageReady();
                }
                break;
        }

    }
    void S_GAMEPLAY_START_GAME(MessageReceiving message)
    {
        // if (BOL_Manager.instance.CanPlayMusicAndSfx())
        // {
        //     MyAudioManager.instance.StopMusic();
        //     MyAudioManager.instance.PlaySfx(BOL_Manager.instance.myAudioInfo.sfx_Start);
        // }
        long bets = message.readLong();
        short mapID = message.readShort();
        long timePlay = message.readLong();
        // player 1;
        short sessionid = message.readShort();
        sbyte leftdatabaseid = message.readByte();
        long leftuserid = message.readLong();
        sbyte leftavatarid = message.readByte();
        long leftGOLD = message.readLong();
        string leftnameShow = message.readString();
        int leftwin = message.readInt();
        int lefttie = message.readInt();
        int leftdraw = message.readInt();
        if (leftdatabaseid == 2)
        {
            long leftfacebookid = message.readLong();
        }
        var leftcharackterID = message.readByte();
        var lefthp = message.readShort();
        var leftmana = message.readShort();
        var leftdamgeAttack1 = message.readShort();
        var leftdamgeAttack2 = message.readShort();
        var leftshield = message.readShort();
        var leftspell = message.readByte();
        var leftmanaOfSkill1 = message.readShort();
        var leftdelayOfSkill1 = message.readLong();
        var leftmanaOfSkill2 = message.readShort();
        var leftdelayOfSkill2 = message.readLong();
        var leftmanaOfUtimate = message.readShort();
        var leftdelayOfUtimate = message.readLong();
        var leftmax_hp = message.readShort();
        var leftmax_mana = message.readShort();
        var leftmax_shield = message.readShort();
        var lefttimeDelaySpell = message.readLong();
        var lefttimeDelaySkill1 = message.readLong();
        var lefttimeDelaySkill2 = message.readLong();
        var lefttimeDelayUltimate = message.readLong();
        var lefteffect1_time_stun = message.readLong();
        var lefteffect2_time_doNotRecoverHP = message.readLong();
        var lefteffect3_time_upDamage = message.readLong();
        var lefteffect3_value_upDamage = message.readShort();
        var lefteffect4_time_slowMatrix = message.readLong();
        var lefteffect5_time_doNotMovePiece = message.readLong();
        var lefteffect6_time_doNotChangePiece = message.readLong();
        var lefteffect7_time_disableSpell = message.readLong();
        var lefteffect8_time_freeze = message.readLong();
        var lefteffect9_time_speedupMatrix = message.readLong();
        var lefteffect10_time_hypnosis_pieceMoveBackwards = message.readLong();
        var lefteffect11_time_backDamage = message.readLong();
        var lefteffect12_time_avoidDamage = message.readLong();
        var lefteffect13_time_downDamage = message.readLong();
        var lefteffect13_value_downDamage = message.readShort();
        var lefteffect14_time_virtual_hp = message.readLong();
        var lefteffect14_value_virtual_hp = message.readShort();
        var lefteffect15_time_bloodsucking = message.readLong();
        var lefteffect15_perthousand_bloodsucking = message.readShort();
        var lefteffect16_time_avoidEffect = message.readLong();
        sbyte[] leftcurrentPieces = message.readMiniByte();// show lúc bắt đầu game
        sbyte[] leftnextPieces = message.readMiniByte();

        // player 2;
        short sessionid11 = message.readShort();
        sbyte rightdatabaseid = message.readByte();
        long rightuserid = message.readLong();
        sbyte rightavatarid = message.readByte();
        long rightGOLD = message.readLong();
        string rightnameShow = message.readString();
        int rightwin = message.readInt();
        int righttie = message.readInt();
        int rightdraw = message.readInt();
        if (rightdatabaseid == 2)
        {
            long rightfacebookid = message.readLong();
        }
        var rightcharackterID = message.readByte();
        var righthp = message.readShort();
        var rightmana = message.readShort();
        var rightdamgeAttack1 = message.readShort();
        var rightdamgeAttack2 = message.readShort();
        var rightshield = message.readShort();
        var rightspell = message.readByte();
        var rightmanaOfSkill1 = message.readShort();
        var rightdelayOfSkill1 = message.readLong();
        var rightmanaOfSkill2 = message.readShort();
        var rightdelayOfSkill2 = message.readLong();
        var rightmanaOfUtimate = message.readShort();
        var rightdelayOfUtimate = message.readLong();
        var rightmax_hp = message.readShort();
        var rightmax_mana = message.readShort();
        var rightmax_shield = message.readShort();
        var righttimeDelaySpell = message.readLong();
        var righttimeDelaySkill1 = message.readLong();
        var righttimeDelaySkill2 = message.readLong();
        var righttimeDelayUltimate = message.readLong();
        var righteffect1_time_stun = message.readLong();
        var righteffect2_time_doNotRecoverHP = message.readLong();
        var righteffect3_time_upDamage = message.readLong();
        var righteffect3_value_upDamage = message.readShort();
        var righteffect4_time_slowMatrix = message.readLong();
        var righteffect5_time_doNotMovePiece = message.readLong();
        var righteffect6_time_doNotChangePiece = message.readLong();
        var righteffect7_time_disableSpell = message.readLong();
        var righteffect8_time_freeze = message.readLong();
        var righteffect9_time_speedupMatrix = message.readLong();
        var righteffect10_time_hypnosis_pieceMoveBackwards = message.readLong();
        var righteffect11_time_backDamage = message.readLong();
        var righteffect12_time_avoidDamage = message.readLong();
        var righteffect13_time_downDamage = message.readLong();
        var righteffect13_value_downDamage = message.readShort();
        var righteffect14_time_virtual_hp = message.readLong();
        var righteffect14_value_virtual_hp = message.readShort();
        var righteffect15_time_bloodsucking = message.readLong();
        var righteffect15_perthousand_bloodsucking = message.readShort();
        var righteffect16_time_avoidEffect = message.readLong();
        sbyte[] rightcurrentPieces = message.readMiniByte();// show lúc bắt đầu game
        sbyte[] rightnextPieces = message.readMiniByte();
#if TEST
        Debug.Log("1>>>>" + leftdelayOfSkill1 + ">>>>" + lefttimeDelaySkill1);
        Debug.Log("2>>>>" + leftdelayOfSkill2 + ">>>>" + lefttimeDelaySkill2);
        Debug.Log("U>>>>" + leftdelayOfUtimate + ">>>>" + lefttimeDelayUltimate);
        Debug.Log("S>>>>" + ">>>>" + lefttimeDelaySpell);
#endif
        switch (BOL_Main_Controller.instance.ChairPosition)
        {
            case Constant.CHAIR_LEFT:
                BOL_PlaySkill_Controller.instance.btnSkillQ.timeDelay = leftdelayOfSkill1;
                BOL_PlaySkill_Controller.instance.btnSkillQ.manaValue = leftmanaOfSkill1;
                BOL_PlaySkill_Controller.instance.btnSkillW.timeDelay = leftdelayOfSkill2;
                BOL_PlaySkill_Controller.instance.btnSkillW.manaValue = leftdelayOfSkill2;
                BOL_PlaySkill_Controller.instance.btnSkillE.timeDelay = leftdelayOfUtimate;
                BOL_PlaySkill_Controller.instance.btnSkillE.manaValue = leftmanaOfUtimate;
                BOL_PlaySkill_Controller.instance.btnSkillSpell.timeDelay = leftdelayOfUtimate;
                BOL_PlaySkill_Controller.instance.skillpostion = leftcharackterID;
                BOL_PlaySkill_Controller.instance.spellposition = leftspell;
                break;
            case Constant.CHAIR_RIGHT:
                BOL_PlaySkill_Controller.instance.btnSkillQ.timeDelay = rightdelayOfSkill1;
                BOL_PlaySkill_Controller.instance.btnSkillQ.manaValue = rightmanaOfSkill1;
                BOL_PlaySkill_Controller.instance.btnSkillW.timeDelay = rightdelayOfSkill2;
                BOL_PlaySkill_Controller.instance.btnSkillW.manaValue = rightmanaOfSkill2;
                BOL_PlaySkill_Controller.instance.btnSkillE.timeDelay = rightdelayOfUtimate;
                BOL_PlaySkill_Controller.instance.btnSkillE.manaValue = rightmanaOfUtimate;
                BOL_PlaySkill_Controller.instance.btnSkillSpell.timeDelay = rightdelayOfUtimate;
                BOL_PlaySkill_Controller.instance.skillpostion = rightcharackterID;
                BOL_PlaySkill_Controller.instance.spellposition = rightspell;
                break;
            default:
                BOL_PlaySkill_Controller.instance._buttonQ.gameObject.SetActive(false);
                BOL_PlaySkill_Controller.instance._buttonE.gameObject.SetActive(false);
                BOL_PlaySkill_Controller.instance._buttonW.gameObject.SetActive(false);
                BOL_PlaySkill_Controller.instance._buttonSpell.gameObject.SetActive(false);
                break;
        }
        Debug.Log(BOL_PlaySkill_Controller.instance.skillpostion + "  " + BOL_PlaySkill_Controller.instance.spellposition);
        if (BOL_Main_Controller.instance.ChairPosition != Constant.CHAIR_PLAYER || BOL_Main_Controller.instance.ChairPosition != Constant.CHAIR_VIEWER)
        {
            BOL_PlaySkill_Controller.instance.SetActivePanel(true,
            BOL_PlaySkill_Controller.instance.skillpostion,
            BOL_PlaySkill_Controller.instance.spellposition);
        }
        else
        {
            BOL_PlaySkill_Controller.instance.SetActivePanel(false);
        }
        health_left_df = leftmax_hp;
        mana_left_df = leftmax_mana;
        shield_left_df = leftmax_shield;
        health_right_df = rightmax_hp;
        mana_right_df = rightmax_mana;
        shield_right_df = rightmax_shield;
        HeroIDLeft = leftcharackterID;
        HeroIDRight = rightcharackterID;
        BOL_Main_Controller.instance.StartInitData(BOL_Main_Controller.instance.InitDataPlaying, BOL_Main_Controller.instance.ActionPlaying);
        Debug.Log(leftnextPieces.Length);
        Debug.Log(rightnextPieces.Length);
        BOL_Main_Controller.instance._BOL_PlayBattle_left._currentPieceCtrl.heroID = HeroIDLeft;
        BOL_Main_Controller.instance._BOL_PlayBattle_left._nextPieceCtrl.heroID = HeroIDLeft;
        BOL_Main_Controller.instance._BOL_PlayBattle_left.SetValuePiece(leftnextPieces, BOL_Battle_PlayerInGame.NEXT_PIECE);
        BOL_Main_Controller.instance._BOL_PlayBattle_left.SetPostionPiece(leftcurrentPieces, 1, 0, 3);
        BOL_Main_Controller.instance._BOL_PlayBattle_left.SetValueHPorMP(BOL_Battle_PlayerInGame.HEALTH, lefthp, health_left_df, Constant.CHAIR_LEFT);
        BOL_Main_Controller.instance._BOL_PlayBattle_left.SetValueHPorMP(BOL_Battle_PlayerInGame.MANA, leftmana, mana_left_df, Constant.CHAIR_LEFT);
        BOL_Main_Controller.instance._BOL_PlayBattle_left.SetValueHPorMP(BOL_Battle_PlayerInGame.SHIELD, leftshield, shield_left_df, Constant.CHAIR_LEFT);
        // BOL_ChoiceHero.instance.btnChoiceChairLeft.txtName.text = leftnameShow;
        BOL_ChoiceHero.instance.btnChoiceChairLeft.txtGold.text = MyConstant.GetMoneyString(leftGOLD, 99999);
        BOL_Main_Controller.instance._BOL_PlayBattle_right._currentPieceCtrl.heroID = HeroIDRight;
        BOL_Main_Controller.instance._BOL_PlayBattle_right._nextPieceCtrl.heroID = HeroIDRight;
        BOL_Main_Controller.instance._BOL_PlayBattle_right.SetValuePiece(rightnextPieces, BOL_Battle_PlayerInGame.NEXT_PIECE);
        BOL_Main_Controller.instance._BOL_PlayBattle_right.SetPostionPiece(rightcurrentPieces, 1, 0, 3);
        BOL_Main_Controller.instance._BOL_PlayBattle_right.SetValueHPorMP(BOL_Battle_PlayerInGame.HEALTH, righthp, health_right_df, Constant.CHAIR_RIGHT);
        BOL_Main_Controller.instance._BOL_PlayBattle_right.SetValueHPorMP(BOL_Battle_PlayerInGame.MANA, rightmana, mana_right_df, Constant.CHAIR_RIGHT);
        BOL_Main_Controller.instance._BOL_PlayBattle_right.SetValueHPorMP(BOL_Battle_PlayerInGame.SHIELD, rightshield, shield_right_df, Constant.CHAIR_RIGHT);
        // BOL_ChoiceHero.instance.btnChoiceChairRight.txtName.text = rightnameShow;
        BOL_ChoiceHero.instance.btnChoiceChairRight.txtGold.text = MyConstant.GetMoneyString(rightGOLD, 99999);
        BOL_PlaySkill_Controller.instance.isStartGame = true;
        BOL_PlaySkill_Controller.instance.InitData();

        Debugs.LogRed("ChairPosition" + BOL_Main_Controller.instance.ChairPosition);
        if (BOL_Main_Controller.instance.ChairPosition == Constant.CHAIR_VIEWER || BOL_Main_Controller.instance.ChairPosition == Constant.CHAIR_PLAYER)
        {
            BOL_Main_Controller.instance.SpawnHeroWhenChoice(Constant.CHAIR_LEFT, leftcharackterID, sessionid);
            BOL_Main_Controller.instance.SpawnHeroWhenChoice(Constant.CHAIR_RIGHT, rightcharackterID, sessionid11);
        }
        BOL_ChoiceHero.instance.btnChoiceChairLeft.SetStartgame();
        BOL_ChoiceHero.instance.btnChoiceChairRight.SetStartgame();
        if (BOL_ChoiceHero.instance.IsMe)
        {
            BOL_ChoiceHero.instance.btnChoiceChairLeft.infoPlayer.Hide();
            BOL_ChoiceHero.instance.btnChoiceChairRight.infoPlayer.Hide();

        }

    }
    void S_GAMEPLAY_FINISH_GAME(MessageReceiving message)
    {
        if (BOL_Manager.instance.CanPlayMusicAndSfx())
        {
            MyAudioManager.instance.PlayMusic(BOL_Manager.instance.myAudioInfo.bgm);
        }

        sbyte result = message.readByte();//-1: chairleft win; 0: hòa; 1: chairright win;
        long goldwin = message.readLong();


        sbyte dataIdWin = message.readByte();
        long userIDWin = message.readLong();
        long goldLastWin = message.readLong();
        int achievemenWIn = message.readInt();

        sbyte dataIdLose = message.readByte();
        long userIDLose = message.readLong();
        long goldLastLose = message.readLong();
        int achievemenLose = message.readInt();


        // chair status when finish game
        short sessionid2 = message.readShort();
        sbyte charackterid2 = message.readByte();
        sbyte spell2 = message.readByte();
        short sessionid4 = message.readShort();
        sbyte charackterid4 = message.readByte();
        sbyte spell4 = message.readByte();

        tempHeroLeft = 12;
        tempHeroRight = 12;
#if TEST
        Debug.Log("gold win" + goldLastWin + " gold lose " + goldLastLose);
        Debug.Log("result " + result);
#endif

        if (sessionid2 >= 0)
        {
            for (int i = 0; i < BOL_ShowPlayer_Manager.instance.listUserIngame.Count; i++)
            {
                if (sessionid2 == BOL_ShowPlayer_Manager.instance.listUserIngame[i].sessionId)
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


        }
        else
        {
            BOL_ChoiceHero.instance.btnChoiceChairLeft.infoPlayer.Hide();
        }
        if (sessionid4 >= 0)
        {
            for (int i = 0; i < BOL_ShowPlayer_Manager.instance.listUserIngame.Count; i++)
            {
                if (sessionid4 == BOL_ShowPlayer_Manager.instance.listUserIngame[i].sessionId)
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
        else
        {
            BOL_ChoiceHero.instance.btnChoiceChairRight.infoPlayer.Hide();
        }
        BOL_Main_Controller.instance.AddAction(() =>
        {
            if (result == -1)
            {
                BOL_Main_Controller.instance.TweenValue(result, goldLastWin, goldLastLose);
                if (sessionid2 == DataManager.instance.userData.sessionId)
                {
                    DataManager.instance.userData.gold = goldLastWin;
                }
                else if (sessionid4 == DataManager.instance.userData.sessionId)
                {
                    DataManager.instance.userData.gold = goldLastLose;
                }
            }
            else if (result == 1)
            {
                BOL_Main_Controller.instance.TweenValue(result, goldLastLose, goldLastWin);
                if (sessionid2 == DataManager.instance.userData.sessionId)
                {
                    DataManager.instance.userData.gold = goldLastLose;
                }
                else if (sessionid4 == DataManager.instance.userData.sessionId)
                {
                    DataManager.instance.userData.gold = goldLastWin;
                }
            }
            BOL_Main_Controller.instance.panelUserInfo.RefreshGoldInfo();
            BOL_ShowPlayer_Manager.instance.RefreshDataUserIngame(userIDWin, goldLastWin);
            BOL_ShowPlayer_Manager.instance.RefreshDataUserIngame(userIDLose, goldLastLose);

            BOL_ChoiceHero.instance.btnChoiceChairLeft.txtGold.text = MyConstant.GetMoneyString(BOL_ShowPlayer_Manager.instance.listUserPlayGame[0].gold, 9999);
            BOL_ChoiceHero.instance.btnChoiceChairRight.txtGold.text = MyConstant.GetMoneyString(BOL_ShowPlayer_Manager.instance.listUserPlayGame[1].gold, 9999);
            if (BOL_Main_Controller.instance.ChairPosition == Constant.CHAIR_LEFT)
            {
                if (sessionid2 >= 0)
                {
                    BOL_ChoiceHero.instance.isChoiceChair = true;
                    BOL_ChoiceHero.instance.btnChoiceChairLeft.HideImageReady();
                    if (BOL_Main_Controller.instance.ChairPosition != Constant.CHAIR_LEFT)
                    {
                        BOL_ChoiceHero.instance.btnChoiceChairLeft.SetFinishforViewer(true);
                    }
                    else
                    {
                        BOL_ChoiceHero.instance.btnChoiceChairLeft.SetFinishGame(true);
                    }
                }
                else
                {
                    if (BOL_Main_Controller.instance.ChairPosition != Constant.CHAIR_LEFT)
                    {
                        BOL_ChoiceHero.instance.btnChoiceChairLeft.SetFinishforViewer(false);
                    }
                    else
                    {
                        BOL_ChoiceHero.instance.btnChoiceChairLeft.SetFinishGame(false);
                    }
                }
                switch (result)
                {
                    case -1:
                        if (BOL_Manager.instance.CanPlayMusicAndSfx())
                        {
                            MyAudioManager.instance.PlaySfx(BOL_Manager.instance.myAudioInfo.sfx_Win);
                        }
                        PopupManager.Instance.CreateToast("YOU WIN");
                        BOL_Main_Controller.instance.StartInitData(BOL_Main_Controller.instance.InitDataFinish,
                        () =>
                        {
                            BOL_Main_Controller.instance.ActionFinish(BOL_Main_Controller.StateScene.win);
                        });
                        break;
                    case 1:
                        //BOL_Main_Controller.instance.InitDataFInish(BOL_Main_Controller.LOSE);
                        if (BOL_Manager.instance.CanPlayMusicAndSfx())
                        {
                            MyAudioManager.instance.PlaySfx(BOL_Manager.instance.myAudioInfo.sfx_Lose);
                        }
                        PopupManager.Instance.CreateToast("YOU LOSE");
                        BOL_Main_Controller.instance.StartInitData(BOL_Main_Controller.instance.InitDataFinish,
                        () =>
                        {
                            BOL_Main_Controller.instance.ActionFinish(BOL_Main_Controller.StateScene.lose);
                        });
                        break;
                    default:
                        PopupManager.Instance.CreateToast("TIE");
                        break;
                }

            }
            else if (BOL_Main_Controller.instance.ChairPosition == Constant.CHAIR_RIGHT)
            {
                if (sessionid4 >= 0)
                {
                    BOL_ChoiceHero.instance.isChoiceChair = true;
                    BOL_ChoiceHero.instance.btnChoiceChairRight.HideImageReady();
                    if (BOL_Main_Controller.instance.ChairPosition != Constant.CHAIR_RIGHT)
                    {
                        BOL_ChoiceHero.instance.btnChoiceChairRight.SetFinishforViewer(true);
                    }
                    else
                    {
                        BOL_ChoiceHero.instance.btnChoiceChairRight.SetFinishGame(true);
                    }
                }
                else
                {
                    if (BOL_Main_Controller.instance.ChairPosition != Constant.CHAIR_RIGHT)
                    {
                        BOL_ChoiceHero.instance.btnChoiceChairRight.SetFinishforViewer(false);
                    }
                    else
                    {
                        BOL_ChoiceHero.instance.btnChoiceChairRight.SetFinishGame(false);
                    }
                }
                switch (result)
                {
                    case -1:
                        PopupManager.Instance.CreateToast("YOU LOSE");
                        if (BOL_Manager.instance.CanPlayMusicAndSfx())
                        {
                            MyAudioManager.instance.PlaySfx(BOL_Manager.instance.myAudioInfo.sfx_Lose);
                        }
                        BOL_Main_Controller.instance.StartInitData(BOL_Main_Controller.instance.InitDataFinish,
                       () =>
                       {
                           BOL_Main_Controller.instance.ActionFinish(BOL_Main_Controller.StateScene.lose);
                       });
                        break;
                    case 1:
                        if (BOL_Manager.instance.CanPlayMusicAndSfx())
                        {
                            MyAudioManager.instance.PlaySfx(BOL_Manager.instance.myAudioInfo.sfx_Win);
                        }
                        PopupManager.Instance.CreateToast("YOU WIN");
                        //BOL_Main_Controller.instance.InitDataFInish(BOL_Main_Controller.WIN);
                        BOL_Main_Controller.instance.StartInitData(BOL_Main_Controller.instance.InitDataFinish,
                       () =>
                       {
                           BOL_Main_Controller.instance.ActionFinish(BOL_Main_Controller.StateScene.win);
                       });
                        break;
                    default:
                        PopupManager.Instance.CreateToast("TIE");
                        break;
                }
            }
            else
            {
                BOL_Main_Controller.instance.StartInitData(BOL_Main_Controller.instance.InitDataFinish,
                        () =>
                        {
                            BOL_Main_Controller.instance.ActionFinish(BOL_Main_Controller.StateScene.viewer);
                            BOL_Main_Controller.instance.StartInitData(BOL_Main_Controller.instance.InitDataWaiting, () =>
                            {
                                BOL_Main_Controller.instance.ActionWaiting(BOL_Main_Controller.StateScene.choicehero);
                                BOL_ChoiceHero.instance.SetActiveChoiceHero(false);
                            });
                        });
#if TEST
                PopupManager.Instance.CreateToast("view finish");
                Debug.Log(Debugs.ColorString("finishgame", Color.red));
#endif
            }

            if (BOL_Main_Controller.instance._BOL_PlayBattleleft != null)
            {

                BOL_Main_Controller.instance._BOL_PlayBattleleft.ResetData();
                BOL_Main_Controller.instance._BOL_PlayBattleleft.SelfDestruction();
                BOL_Main_Controller.instance._BOL_PlayBattle_left = null;
                BOL_Main_Controller.instance._BOL_PlayBattleleft = null;
                BOL_ChoiceHero.instance.btnChoiceChairLeft.HideImageReady();
            }
            else
            {
                BOL_ChoiceHero.instance.btnChoiceChairLeft.txtGold.text = string.Empty;
                BOL_ChoiceHero.instance.btnChoiceChairLeft.txtName.text = string.Empty;
            }
            if (BOL_Main_Controller.instance._BOL_PlayBattleright != null)
            {
                BOL_Main_Controller.instance._BOL_PlayBattleright.ResetData();
                BOL_Main_Controller.instance._BOL_PlayBattleright.SelfDestruction();
                BOL_Main_Controller.instance._BOL_PlayBattle_right = null;
                BOL_Main_Controller.instance._BOL_PlayBattleright = null;
                BOL_ChoiceHero.instance.btnChoiceChairRight.HideImageReady();
            }
            else
            {
                BOL_ChoiceHero.instance.btnChoiceChairRight.txtGold.text = string.Empty;
                BOL_ChoiceHero.instance.btnChoiceChairRight.txtName.text = string.Empty;
            }
            BOL_PlaySkill_Controller.instance.ClearAllActionWhenFinish();
            CoroutineChain.StopAll();
        });


    }
    void S_GAMEPLAY_PLAYER_JOIN_GAME(MessageReceiving message)
    {
        BOL_Manager.instance.bol_Table_Info.SetUpUserJoinGame(message);
    }
    void S_GAMEPLAY_PLAYER_LEFT_GAME(MessageReceiving message)
    {
        BOL_Manager.instance.bol_Table_Info.SetUpUserLeftGame(message);
    }
    void S_GAMEPLAY_CHAT_IN_TABLE(MessageReceiving message)
    {
        short _sessionId = message.readShort();
        string _strMess = message.readString();
#if TEST
        Debug.Log(_sessionId + _strMess);
        Debug.Log(BOL_Manager.instance.bol_Table_Info.listOtherPlayerData.Count);
#endif
        BOL_Manager.instance.screenChat.AddMessage(_sessionId, _strMess, BOL_Manager.instance.bol_Table_Info.listOtherPlayerData);
        BOL_Manager.instance.ShowPopupChat(_sessionId, _strMess);

    }
    // CMD mới
    void S_GAMEPLAY_GET_CHAIR_STATUS(MessageReceiving message)
    {
        sbyte status = message.readByte();
        short mapID = message.readShort();
        short sessionidLeft = message.readShort();
        sbyte heroLeft = message.readByte();
        sbyte spellLeft = message.readByte();
        short sessionidRight = message.readShort();
        sbyte heroRight = message.readByte();
        sbyte spellRight = message.readByte();
#if TEST
        Debug.Log("left characterid " + heroLeft);
        Debug.Log("right characterid " + heroRight);
#endif

        if (sessionidLeft >= 0)
        {
            if (tempHeroLeft != heroLeft)
            {
                tempHeroLeft = heroLeft;
                BOL_Main_Controller.instance.SpawnHeroWhenChoice(Constant.CHAIR_LEFT, heroLeft, sessionidLeft);
                //BOL_ChoiceHero.instance.btnChoiceChairLeft.SpawnHerowhenchoice(heroLeft,sessionidLeft);
                if (BOL_Main_Controller.instance.ChairPosition == Constant.CHAIR_LEFT)
                {
                    BOL_ChoiceHero.instance.btnChoiceChairLeft.SetSitdownSuccess();
                }
                else
                {
                    BOL_ChoiceHero.instance.btnChoiceChairLeft.SetStartgame();
                }

            }
            for (int i = 0; i < BOL_ShowPlayer_Manager.instance.listUserIngame.Count; i++)
            {
                if (sessionidLeft == BOL_ShowPlayer_Manager.instance.listUserIngame[i].sessionId)
                {
#if TEST
                    Debug.Log("session " + BOL_ShowPlayer_Manager.instance.listUserIngame[i].sessionId
                    + " nameshow " + BOL_ShowPlayer_Manager.instance.listUserIngame[i].nameShowInGame
                    + " list count " + BOL_ShowPlayer_Manager.instance.listUserIngame.Count);
#endif
                    // BOL_PlaySkill_Controller.instance.info1.InitDataInGame(BOL_ShowPlayer_Manager.instance.listUserIngame[i]);
                    BOL_ChoiceHero.instance.btnChoiceChairLeft.infoPlayer.InitData(BOL_ShowPlayer_Manager.instance.listUserIngame[i]);
                }
            }
            BOL_ChoiceHero.instance._btnChoiceChairLeft.enabled = false;
            BOL_ChoiceHero.instance.btnChoiceChairLeft.ShowInfoPlayer();
        }
        else
        {
            tempHeroLeft = 12;
            BOL_Main_Controller.instance.SpawnHeroWhenChoice(Constant.CHAIR_LEFT, -1, sessionidLeft);
            //BOL_ChoiceHero.instance.btnChoiceChairLeft.SpawnHerowhenchoice(-1,sessionidLeft);
            BOL_ChoiceHero.instance.btnChoiceChairLeft.SetSitdownFail();
            BOL_ChoiceHero.instance._btnChoiceChairLeft.enabled = true;
            BOL_ChoiceHero.instance.btnChoiceChairLeft.infoPlayer.Hide();
        }
        if (sessionidRight >= 0)
        {
            if (tempHeroRight != heroRight)
            {
                tempHeroRight = heroRight;
                BOL_Main_Controller.instance.SpawnHeroWhenChoice(Constant.CHAIR_RIGHT, heroRight, sessionidRight);
                //BOL_ChoiceHero.instance.btnChoiceChairRight.SpawnHerowhenchoice(heroRight,sessionidRight);
                if (BOL_Main_Controller.instance.ChairPosition == Constant.CHAIR_RIGHT)
                {
                    BOL_ChoiceHero.instance.btnChoiceChairRight.SetSitdownSuccess();
                }
                else
                {
                    BOL_ChoiceHero.instance.btnChoiceChairRight.SetStartgame();
                }
            }
            for (int i = 0; i < BOL_ShowPlayer_Manager.instance.listUserIngame.Count; i++)
            {
                if (sessionidRight == BOL_ShowPlayer_Manager.instance.listUserIngame[i].sessionId)
                {
#if TEST
                    Debug.Log("session " + BOL_ShowPlayer_Manager.instance.listUserIngame[i].sessionId
                    + " nameshow " + BOL_ShowPlayer_Manager.instance.listUserIngame[i].nameShowInGame
                    + " list count " + BOL_ShowPlayer_Manager.instance.listUserIngame.Count);
#endif
                    // BOL_PlaySkill_Controller.instance.info1.InitDataInGame(BOL_ShowPlayer_Manager.instance.listUserIngame[i]);
                    BOL_ChoiceHero.instance.btnChoiceChairRight.infoPlayer.InitData(BOL_ShowPlayer_Manager.instance.listUserIngame[i]);
                }
            }
            BOL_ChoiceHero.instance._btnChoiceChairRight.enabled = false;
            BOL_ChoiceHero.instance.btnChoiceChairRight.ShowInfoPlayer();
        }
        else
        {
            tempHeroRight = 12;
            BOL_Main_Controller.instance.SpawnHeroWhenChoice(Constant.CHAIR_RIGHT, -1, sessionidRight);
            //BOL_ChoiceHero.instance.btnChoiceChairRight.SpawnHerowhenchoice(-1,sessionidRight);
            BOL_ChoiceHero.instance.btnChoiceChairRight.SetSitdownFail();
            BOL_ChoiceHero.instance._btnChoiceChairRight.enabled = true;
            BOL_ChoiceHero.instance.btnChoiceChairRight.infoPlayer.Hide();
        }
        if (sessionidLeft == DataManager.instance.userData.sessionId)
        {

            //BOL_ChoiceHero.instance.ChoiceSpell(spellLeft);
            if (!BOL_ChoiceHero.instance.listLock[heroLeft].activeSelf)
            {
                BOL_ChoiceHero.instance.listHeroChoose[BOL_ChoiceHero.instance.tmpHero]._isChoiceHero = false;
                BOL_ChoiceHero.instance.listHeroChoose[heroLeft]._isChoiceHero = true;
                BOL_ChoiceHero.instance.tmpHero = heroLeft;
                BOL_ChoiceHero.instance.IsMe = true;
            }
            else
            {
                PopupManager.Instance.CreateToast(string.Format(MyLocalize.GetString("Global/CommingSoon")));
            }

            BOL_ChoiceHero.instance.listSpellChoose[BOL_ChoiceHero.instance.tmpSpell]._ischoiceSpell = false;
            BOL_ChoiceHero.instance.listSpellChoose[spellLeft]._ischoiceSpell = true;
            BOL_ChoiceHero.instance.tmpSpell = spellLeft;
        }
        if (sessionidRight == DataManager.instance.userData.sessionId)
        {
            //BOL_ChoiceHero.instance.ChoiceSpell(spellRight);
            if (!BOL_ChoiceHero.instance.listLock[heroRight].activeSelf)
            {
                BOL_ChoiceHero.instance.listHeroChoose[BOL_ChoiceHero.instance.tmpHero]._isChoiceHero = false;
                BOL_ChoiceHero.instance.listHeroChoose[heroRight]._isChoiceHero = true;
                BOL_ChoiceHero.instance.tmpHero = heroRight;
                BOL_ChoiceHero.instance.IsMe = true;
            }
            else
            {
                PopupManager.Instance.CreateToast(string.Format(MyLocalize.GetString("Global/CommingSoon")));
            }
            BOL_ChoiceHero.instance.listSpellChoose[BOL_ChoiceHero.instance.tmpSpell]._ischoiceSpell = false;
            BOL_ChoiceHero.instance.listSpellChoose[spellRight]._ischoiceSpell = true;
            BOL_ChoiceHero.instance.tmpSpell = spellRight;
        }
        if (BOL_ChoiceHero.instance.IsMe)
        {
#if TEST
            Debug.LogError("check is me at chairStatus " + BOL_ChoiceHero.instance.IsMe);
#endif
            BOL_ChoiceHero.instance.btnChoiceChairLeft.ArrowChoice.SetActive(false);
            BOL_ChoiceHero.instance.btnChoiceChairLeft.PlusChoice.SetActive(false);
            BOL_ChoiceHero.instance.btnChoiceChairRight.ArrowChoice.SetActive(false);
            BOL_ChoiceHero.instance.btnChoiceChairRight.PlusChoice.SetActive(false);
            BOL_Main_Controller.instance.panelUserInfo.Hide();
        }
        else
        {
#if TEST
            Debug.LogError("check is me at chairStatus " + BOL_ChoiceHero.instance.IsMe);
#endif
        }
    }
    void S_GAMEPLAY_SITDOWN(MessageReceiving message)
    {
        var isSitdown = message.readBoolean();
        var chairID = message.readByte();
        var currentChairID = message.readByte();
        var tableBet = message.readLong();
        var gold = message.readLong();
        BOL_Main_Controller.instance.ChairPosition = currentChairID;
        if (popupSitdownFail != null)
        {
            popupSetbetFail = null;
        }
        // popupSetbetFail = PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kWarning)
        //                                        , string.Format(MyLocalize.GetString("Error/GamePlay_CanNotSitDown"))
        //                                        , string.Empty
        //                                        , MyLocalize.GetString(MyLocalize.kOk)
        //                                        , () =>
        //                                        {

        //                                        });
        #if TEST
        Debug.Log("cannot sitdown");
        #endif
        switch (chairID)
        {
            case (sbyte)Constant.CHAIR_LEFT:
                if (!BOL_ChoiceHero.instance.IsMe)
                {
                    BOL_ChoiceHero.instance.btnChoiceChairLeft.btnStandUp.gameObject.SetActive(false);
                }
                break;
            case (sbyte)Constant.CHAIR_RIGHT:
                if (!BOL_ChoiceHero.instance.IsMe)
                {
                    BOL_ChoiceHero.instance.btnChoiceChairRight.btnStandUp.gameObject.SetActive(false);
                }
                break;
        }

#if TEST
        Debug.Log("|isSitdown" + isSitdown + "chairID" + chairID + " | currentChairID" + currentChairID + " | ChairPosition" + BOL_Main_Controller.instance.ChairPosition
        + "|tableBet" + tableBet + "|gold" + gold
        );
#endif
    }
    void S_GAMEPLAY_PLAYER_SITDOWN(MessageReceiving message)
    {
        var sessionID = message.readShort();
        var chairID = message.readByte();
        BOL_ChoiceHero.instance.isChoiceChair = true;
        switch (chairID)
        {
            case (sbyte)Constant.CHAIR_LEFT:
                BOL_Table_Info.instance.isReady0 = true;
                BOL_ChoiceHero.instance.btnChoiceChairLeft.SetSitdownSuccess();
                BOL_ChoiceHero.instance.btnChoiceChairLeft.isChoiceChair = true;
                for (int i = 0; i < BOL_ShowPlayer_Manager.instance.listUserIngame.Count; i++)
                {
                    if (sessionID == BOL_ShowPlayer_Manager.instance.listUserIngame[i].sessionId)
                    {
#if TEST
                        Debug.Log("session " + BOL_ShowPlayer_Manager.instance.listUserIngame[i].sessionId
                        + " nameshow " + BOL_ShowPlayer_Manager.instance.listUserIngame[i].nameShowInGame
                        + " list count " + BOL_ShowPlayer_Manager.instance.listUserIngame.Count);
#endif
                        // BOL_PlaySkill_Controller.instance.info1.InitDataInGame(BOL_ShowPlayer_Manager.instance.listUserIngame[i]);
                        BOL_ChoiceHero.instance.btnChoiceChairLeft.infoPlayer.InitData(BOL_ShowPlayer_Manager.instance.listUserIngame[i]);
                    }
                }
                break;
            case (sbyte)Constant.CHAIR_RIGHT:
                BOL_Table_Info.instance.isReady1 = true;
                BOL_ChoiceHero.instance.btnChoiceChairRight.SetSitdownSuccess();
                BOL_ChoiceHero.instance.btnChoiceChairRight.isChoiceChair = true;
                for (int i = 0; i < BOL_ShowPlayer_Manager.instance.listUserIngame.Count; i++)
                {
                    if (sessionID == BOL_ShowPlayer_Manager.instance.listUserIngame[i].sessionId)
                    {
#if TEST
                        Debug.Log("session " + BOL_ShowPlayer_Manager.instance.listUserIngame[i].sessionId
                        + " nameshow " + BOL_ShowPlayer_Manager.instance.listUserIngame[i].nameShowInGame
                        + " list count " + BOL_ShowPlayer_Manager.instance.listUserIngame.Count);
#endif
                        // BOL_PlaySkill_Controller.instance.info2.InitDataInGame(BOL_ShowPlayer_Manager.instance.listUserIngame[i]);
                        BOL_ChoiceHero.instance.btnChoiceChairRight.infoPlayer.InitData(BOL_ShowPlayer_Manager.instance.listUserIngame[i]);
                    }
                }
                break;
        }
        if (BOL_Main_Controller.instance.ChairPosition == chairID)
        {
            BOL_Main_Controller.instance.ActionWaiting(BOL_Main_Controller.StateScene.choicehero);
            BOL_ChoiceHero.instance.SetActiveChoiceHero(true);
            // switch (chairID)
            // {
            //     case (sbyte)Constant.CHAIR_LEFT:
            //         BOL_ChoiceHero.instance.btnChoiceChairLeft.SetSitdownSuccess();
            //         BOL_ChoiceHero.instance.btnChoiceChairLeft.isChoiceChair = true;
            //         break;
            //     case (sbyte)Constant.CHAIR_RIGHT:
            //         BOL_ChoiceHero.instance.btnChoiceChairRight.SetSitdownSuccess();
            //         BOL_ChoiceHero.instance.btnChoiceChairRight.isChoiceChair = true;
            //         break;
            // }
        }
        BOL_ShowPlayer_Manager.instance.AddPlayerPlayGame(sessionID, chairID);
    }
    void S_GAMEPLAY_STANDUP(MessageReceiving message)
    {
        var boolean = message.readBoolean();
        BOL_ChoiceHero.instance.ReceiveReadyGame(false);
        BOL_Main_Controller.instance.ChairPosition = Constant.CHAIR_VIEWER;
    }
    void S_GAMEPLAY_PLAYER_STANDUP(MessageReceiving message)
    {
        var sessionID = message.readShort();
        var indexChair = message.readByte();
#if TEST
        Debug.Log("chair pos" + BOL_Main_Controller.instance.ChairPosition + "  index chair" + indexChair);
#endif
        if (BOL_Main_Controller.instance.ChairPosition == indexChair)
        {
            BOL_Main_Controller.instance.ActionWaiting(BOL_Main_Controller.StateScene.showplayer);
            BOL_ChoiceHero.instance.SetActiveChoiceHero(false);
            BOL_Main_Controller.instance.ChairPosition = Constant.CHAIR_VIEWER;
            switch (indexChair)
            {
                case (sbyte)Constant.CHAIR_LEFT:
                    BOL_ChoiceHero.instance.btnChoiceChairLeft.SetStandupSuccess();
                    BOL_ChoiceHero.instance.btnChoiceChairLeft.isChoiceChair = false;
                    BOL_ChoiceHero.instance.btnChoiceChairLeft.HideImageReady();
                    break;
                case (sbyte)Constant.CHAIR_RIGHT:
                    BOL_ChoiceHero.instance.btnChoiceChairRight.SetStandupSuccess();
                    BOL_ChoiceHero.instance.btnChoiceChairRight.isChoiceChair = false;
                    BOL_ChoiceHero.instance.btnChoiceChairRight.HideImageReady();
                    break;
            }
        }
        if (sessionID == BOL_ShowPlayer_Manager.instance.listUserPlayGame[indexChair].sessionId)
        {
#if TEST
            Debug.Log("active false object" + sessionID + "  index chair" + indexChair);
#endif
            switch (indexChair)
            {
                case (sbyte)Constant.CHAIR_LEFT:
                    BOL_ChoiceHero.instance.btnChoiceChairLeft.isChoiceChair = false;
                    break;
                case (sbyte)Constant.CHAIR_RIGHT:
                    BOL_ChoiceHero.instance.btnChoiceChairRight.isChoiceChair = false;
                    break;
            }
            BOL_Main_Controller.instance.panelUserInfo.RefreshGoldInfo();
            BOL_ShowPlayer_Manager.instance.RemovePLayerPlayGame(sessionID, indexChair);
        }
        if (sessionID == DataManager.instance.userData.sessionId)
        {
            BOL_ChoiceHero.instance.IsMe = false;
        }
    }
    void S_GAMEPLAY_PLAYER_GET_LOSE(MessageReceiving message)
    {
        var sessionID = message.readShort();
    }
    public void S_onPlayerAddGold(MessageReceiving message)
    {
        PlayerAddGold_Data _data = new PlayerAddGold_Data(message);
        if (_data.sessionId == DataManager.instance.userData.sessionId)
        {
            Debug.Log("addgold");
            DataManager.instance.userData.gold = _data.goldLast;
            BOL_Main_Controller.instance.panelUserInfo.RefreshGoldInfo();
        }
        else
        {
            Debug.Log("khác sessionid" + _data.sessionId + "    usersession " + DataManager.instance.userData.sessionId);
        }
        if (_data.sessionId == BOL_ShowPlayer_Manager.instance.listUserPlayGame[0].sessionId)
        {
            BOL_ShowPlayer_Manager.instance.listUserPlayGame[0].gold = _data.goldLast;
        }
        else if (_data.sessionId == BOL_ShowPlayer_Manager.instance.listUserPlayGame[1].sessionId)
        {
            BOL_ShowPlayer_Manager.instance.listUserPlayGame[1].gold = _data.goldLast;
        }
    }
    public void S_PlayerSetParent(MessageReceiving _mess)
    {
        PlayerSetParent_Data _data = new PlayerSetParent_Data(_mess);
        SetUpActionPlayerSetParent(_data);
        if (_data.caseCheck == 1)
        {
            BOL_Main_Controller.instance.panelUserInfo.RefreshGoldInfo();
        }
    }

    public void RegisterActionAlertUpdateServer()
    {
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_ALERT_UPDATE_SERVER, (_mess) =>
        {
            if (ins != null)
            {
                MyGamePlayData.AlertUpdateServer_Data _data = new MyGamePlayData.AlertUpdateServer_Data(_mess);
                System.TimeSpan _timeSpanRemain = _data.timeToUpdateServer - System.DateTime.Now;
                PopupManager.Instance.CreateToast(string.Format(MyLocalize.GetString("System/Message_ServerMaintenance"), _timeSpanRemain.Minutes, _timeSpanRemain.Seconds));
            }
        });
    }
    #endregion
    #region moving in game
    void S_XHCD_CURRENT_PIECE(MessageReceiving message)
    {
        sbyte[] leftcurrentpiece = message.readMiniByte();
        sbyte r1 = message.readByte();
        sbyte c1 = message.readByte();
        sbyte[] rightcurrentpiece = message.readMiniByte();
        sbyte r2 = message.readByte();
        sbyte c2 = message.readByte();
        BOL_Main_Controller.instance._BOL_PlayBattle_left.SetPostionPiece(leftcurrentpiece, 1, r1, c1);
        BOL_Main_Controller.instance._BOL_PlayBattle_right.SetPostionPiece(rightcurrentpiece, 1, r2, c2);
    }
    void S_XHCD_MAXTRIX_DATA(MessageReceiving message)
    {
        //BOL_Main_Controller.instance.AddAction(SXHCD_MAXTRIX_DATA(message));
        sbyte chairID = message.readByte();
        List<sbyte> listvalue = new List<sbyte>();
        for (int i = 0; i < Constant.ROW; i++)
        {
            for (int j = 0; j < Constant.COL; j++)
            {
                sbyte pieceTemp = message.readByte();
                listvalue.Add(pieceTemp);
            }
        }
        switch (chairID)
        {
            case (sbyte)Constant.CHAIR_LEFT:
                {
                    BOL_Manager.instance.AddAction2Process(Constant.CHAIR_LEFT, () =>
                    {
                        for (int i = 0; i < Constant.ROW; i++)
                        {
                            for (int j = 0; j < Constant.COL; j++)
                            {
                                sbyte pieceTemp = message.readByte();
                                BOL_Main_Controller.instance._BOL_PlayBattle_left.AddPieceInMatrix(listvalue[i * 8 + j], i, j);
                            }
                        }
                    }, "matrix left data");
                    break;
                }
            case (sbyte)Constant.CHAIR_RIGHT:
                {
                    BOL_Manager.instance.AddAction2Process(Constant.CHAIR_RIGHT, () =>
                    {
                        for (int i = 0; i < Constant.ROW; i++)
                        {
                            for (int j = 0; j < Constant.COL; j++)
                            {
                                BOL_Main_Controller.instance._BOL_PlayBattle_right.AddPieceInMatrix(listvalue[i * 8 + j], i, j);
                            }
                        }
                    }, "matrix right data");
                    break;
                }
        }
    }
    void S_XHCD_ADD_PIECE_TO_MATRIX_AND_NEXT_PIECE(MessageReceiving message)
    {
        if (BOL_Manager.instance.CanPlayMusicAndSfx())
        {
            MyAudioManager.instance.PlaySfx(BOL_Manager.instance.myAudioInfo.sfx_Press);
        }
        sbyte chairPosition = message.readByte();
        sbyte lengtthPieceAdd = message.readByte();
        switch (chairPosition)
        {
            case (sbyte)Constant.CHAIR_LEFT:
                {
                    for (int i = 0; i < 3; i++)
                    {
                        BOL_Main_Controller.instance._BOL_PlayBattle_left.curentPiece.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = null;
                    }
                    List<sbyte> listvalue = new List<sbyte>();
                    List<sbyte> listrow = new List<sbyte>();
                    List<sbyte> listcol = new List<sbyte>();

                    for (int i = 0; i < lengtthPieceAdd; i++)
                    {
                        sbyte value = message.readByte();
                        sbyte row = message.readByte();
                        sbyte col = message.readByte();
                        listvalue.Add(value);
                        listrow.Add(row);
                        listcol.Add(col);
                    }
                    BOL_Manager.instance.AddAction2Process(Constant.CHAIR_LEFT, () =>
                    {
                        for (int i = 0; i < listvalue.Count; i++)
                        {
                            if (BOL_Main_Controller.instance != null)
                            {
                                if (BOL_Main_Controller.instance._BOL_PlayBattle_left != null)
                                {
                                    BOL_Main_Controller.instance._BOL_PlayBattle_left.AddPieceInMatrix(listvalue[i], listrow[i], listcol[i]);
                                }

                            }
                        }
                    }, "left ADD_PIECE_TO_MATRIX");
                    sbyte rowNewPiece = message.readByte();
                    sbyte colNewPiece = message.readByte();
                    sbyte[] currentPiece = message.readMiniByte();
                    sbyte[] nextPiece = message.readMiniByte();
                    //BOL_Main_Controller.instance._BOL_PlayBattle_left.SetPostionPiece(currentPiece, 1, rowNewPiece, colNewPiece);
                    BOL_Main_Controller.instance._BOL_PlayBattle_left.SetValuePiece(nextPiece, 2);
                    BOL_Main_Controller.instance._BOL_PlayBattle_left.CheckWarning();
                    break;
                }
            case (sbyte)Constant.CHAIR_RIGHT:
                {
                    for (int i = 0; i < 3; i++)
                    {
                        BOL_Main_Controller.instance._BOL_PlayBattle_right.curentPiece.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = null;
                    }
                    List<sbyte> listvalue = new List<sbyte>();
                    List<sbyte> listrow = new List<sbyte>();
                    List<sbyte> listcol = new List<sbyte>();
                    for (int i = 0; i < lengtthPieceAdd; i++)
                    {
                        sbyte value = message.readByte();
                        sbyte row = message.readByte();
                        sbyte col = message.readByte();
                        listvalue.Add(value);
                        listrow.Add(row);
                        listcol.Add(col);
                        //BOL_Main_Controller.instance._BOL_PlayBattle_right.AddPieceInMatrix(value, row, col);
                    }
                    BOL_Manager.instance.AddAction2Process(Constant.CHAIR_RIGHT, () =>
                    {
                        for (int i = 0; i < listvalue.Count; i++)
                        {
                            if (BOL_Main_Controller.instance != null)
                            {
                                if (BOL_Main_Controller.instance._BOL_PlayBattle_right != null)
                                {
                                    BOL_Main_Controller.instance._BOL_PlayBattle_right.AddPieceInMatrix(listvalue[i], listrow[i], listcol[i]);
                                }

                            }
                        }
                    }, "right ADD_PIECE_TO_MATRIX");
                    sbyte rowNewPiece = message.readByte();
                    sbyte colNewPiece = message.readByte();
                    sbyte[] currentPiece = message.readMiniByte();
                    sbyte[] nextPiece = message.readMiniByte();
                    //BOL_Main_Controller.instance._BOL_PlayBattle_right.SetPostionPiece(currentPiece, 1, rowNewPiece, colNewPiece);
                    BOL_Main_Controller.instance._BOL_PlayBattle_right.SetValuePiece(nextPiece, 2);
                    BOL_Main_Controller.instance._BOL_PlayBattle_right.CheckWarning();
                    break;
                }
        }
    }
    void S_XHCD_PIECE_FALLING(MessageReceiving message)
    {
        //BOL_Main_Controller.instance.AddAction(SXHCD_PIECE_FALLING(message));
        sbyte chairID = message.readByte();
        List<sbyte> listValue = new List<sbyte>();
        List<sbyte> listCol = new List<sbyte>();
        List<sbyte> listrowbegin = new List<sbyte>();
        List<sbyte> listrowfinish = new List<sbyte>();
        while (message.readBoolean())
        {
            sbyte matrixvalue = message.readByte();
            listValue.Add(matrixvalue);
            sbyte col = message.readByte();
            listCol.Add(col);
            sbyte rowbegin = message.readByte();
            listrowbegin.Add(rowbegin);
            sbyte rowfinish = message.readByte();
            listrowfinish.Add(rowfinish);
        }

        switch (chairID)
        {
            case (sbyte)Constant.CHAIR_LEFT:
                BOL_Manager.instance.AddAction2Process(chairID, () =>
                {
                    for (int i = 0; i < listValue.Count; i++)
                    {
                        BOL_Main_Controller.instance._BOL_PlayBattle_left.TweenPressMoveVerticalFalling(listValue[i], listCol[i], listrowbegin[i], listrowfinish[i]);
                    }
                }, "left falling");
                break;

            case (sbyte)Constant.CHAIR_RIGHT:
                BOL_Manager.instance.AddAction2Process(chairID, () =>
                {
                    for (int i = 0; i < listValue.Count; i++)
                    {
                        BOL_Main_Controller.instance._BOL_PlayBattle_right.TweenPressMoveVerticalFalling(listValue[i], listCol[i], listrowbegin[i], listrowfinish[i]);
                    }
                }, "rigt falling");
                break;

        }


    }
    void S_XHCD_PLAYER_MOVE(MessageReceiving message)
    {
        if (BOL_Manager.instance.CanPlayMusicAndSfx())
        {
            MyAudioManager.instance.PlaySfx(BOL_Manager.instance.myAudioInfo.sfx_Move);
        }
        sbyte chairid = message.readByte();
        bool isHypnosis = message.readBoolean();
        sbyte[] currentPiece = message.readMiniByte();
        sbyte rowBegin = message.readByte();
        sbyte colBegin = message.readByte();
        sbyte rowFinish = message.readByte();
        sbyte colFinish = message.readByte();
        switch (chairid)
        {
            case (sbyte)Constant.CHAIR_LEFT:
                BOL_Main_Controller.instance._BOL_PlayBattle_left.TweenPressMoveHorizontal(colFinish);
                break;
            case (sbyte)Constant.CHAIR_RIGHT:
                BOL_Main_Controller.instance._BOL_PlayBattle_right.TweenPressMoveHorizontal(colFinish);
                break;
        }
    }
    void S_XHCD_PLAYER_MOVE_DOWN(MessageReceiving message)
    {
        if (BOL_Manager.instance.CanPlayMusicAndSfx())
        {
            MyAudioManager.instance.PlaySfx(BOL_Manager.instance.myAudioInfo.sfx_Move);
        }
        sbyte chairid = message.readByte();
        sbyte[] currentPiece = message.readMiniByte();
        sbyte rowBegin = message.readByte();
        sbyte colBegin = message.readByte();
        sbyte rowFinish = message.readByte();
        sbyte colFinish = message.readByte();
        switch (chairid)
        {
            case (sbyte)Constant.CHAIR_LEFT:
                //LeanTween.moveLocal(BOL_Manager_Matrix.instance.currentPieceLeft, new Vector3(colFinish * ratioFootRow + ratioFootRow / 2, rowFinish * ratioFootCol + ratioFootCol / 2), timeTween);
                BOL_Main_Controller.instance._BOL_PlayBattle_left.TweenPressMoveVertical(rowFinish);
                break;
            case (sbyte)Constant.CHAIR_RIGHT:
                //LeanTween.moveLocal(BOL_Manager_Matrix.instance.currentPieceRight, new Vector3(colFinish * ratioFootRow + ratioFootRow / 2, rowFinish * ratioFootCol + ratioFootCol / 2), timeTween);
                BOL_Main_Controller.instance._BOL_PlayBattle_right.TweenPressMoveVertical(rowFinish);
                break;
        }
    }
    void S_XHCD_PLAYER_CHANGE_PIECE_STATE(MessageReceiving message)
    {
        if (BOL_Manager.instance.CanPlayMusicAndSfx())
        {
            MyAudioManager.instance.PlaySfx(BOL_Manager.instance.myAudioInfo.sfx_Move);
        }
        sbyte chairid = message.readByte();
        sbyte[] currentPiece = message.readMiniByte();
        switch (chairid)
        {
            case (sbyte)Constant.CHAIR_LEFT:
                {
                    BOL_Main_Controller.instance._BOL_PlayBattle_left.SetValuePiece(currentPiece, BOL_Battle_PlayerInGame.CURRENT_PIECE);
                    break;
                }
            case (sbyte)Constant.CHAIR_RIGHT:
                {
                    BOL_Main_Controller.instance._BOL_PlayBattle_right.SetValuePiece(currentPiece, BOL_Battle_PlayerInGame.CURRENT_PIECE);
                    break;
                }
        }
    }
    #endregion
    #region attack
    void S_XHCD_PieceBreak_ATTACK_1(MessageReceiving message)
    {
        if (BOL_Manager.instance.CanPlayMusicAndSfx())
        {
            MyAudioManager.instance.PlaySfx(BOL_Manager.instance.myAudioInfo.sfx_Attack1);
        }
        sbyte chairId = message.readByte();
        sbyte characterid = message.readByte();
        sbyte numberPiece = message.readByte();
#if TEST
        Debugs.LogCyan(chairId + " attack 1" + "characterid  " + characterid + " numberpiece" + numberPiece);
#endif
        switch (chairId)
        {
            case (sbyte)Constant.CHAIR_LEFT:
                {
                    for (int i = 0; i < numberPiece; i++)
                    {
                        sbyte rowbreak = message.readByte();
                        sbyte columnbreak = message.readByte();
                        //BOL_Manager_Matrix.instance.AddBreakInMatrix(Constant.CHAIR_LEFT, rowbreak, columnbreak);

                        BOL_Main_Controller.instance._BOL_PlayBattle_left.AddBreakInMatrix(rowbreak, columnbreak);
                    }
                    BOL_PlaySkill_Controller.instance._listAction.Add(BOL_PlaySkill_Controller.instance.Left_Attack1);
                    break;
                }
            case (sbyte)Constant.CHAIR_RIGHT:
                {
                    for (int i = 0; i < numberPiece; i++)
                    {
                        sbyte rowbreak = message.readByte();
                        sbyte columnbreak = message.readByte();
                        //BOL_Manager_Matrix.instance.AddBreakInMatrix(Constant.CHAIR_RIGHT, rowbreak, columnbreak);
                        BOL_Main_Controller.instance._BOL_PlayBattle_right.AddBreakInMatrix(rowbreak, columnbreak);
                    }
                    BOL_PlaySkill_Controller.instance._listAction.Add(BOL_PlaySkill_Controller.instance.Right_Attack1);
                    break;
                }
        }
        short damageCalculator = message.readShort();
        long timeDelayUpDamage = message.readLong();
        short valueUpdamage = message.readShort();
        long timeDelayDownDamage = message.readLong();
        short valueDownDamage = message.readShort();
        long timeDelaySucking = message.readLong();
        short valueSucking = message.readShort();
        sbyte caseCheck = message.readByte();
        switch (caseCheck)
        {
            case 1:
                {
                    long enemy_time_avoidDamage = message.readLong();
                    //BOL_SetupGame.instance.TweenText("phản Damge", Color.red, BOL_SetupGame.instance.infoLeft.transform);
                    //BOL_SetupGame.instance.TweenText("phản Damge", Color.red, BOL_SetupGame.instance.infoRight.transform);
                    break;
                }
            case 2:
                {
                    long me_time_backDamage = message.readLong();
                    short me_hpLeft = message.readShort();
                    short me_hpResult = message.readShort();
                    short me_manaResult = message.readShort();
                    short me_shieldResult = message.readShort();
                    long me_time_virtual_hp = message.readLong();
                    short me_value_virtual_hp = message.readShort();
                    switch (chairId)
                    {
                        case (sbyte)Constant.CHAIR_LEFT:
                            //SetScaleObject(BOL_Main_Battle.instance.health_left, me_hpResult, health_left_df);
                            //BOL_SetupGame.instance.TweenText("-" + me_hpLeft + " left hp", Color.red, BOL_SetupGame.instance.infoLeft.transform);
                            break;
                        case (sbyte)Constant.CHAIR_RIGHT:
                            //SetScaleObject(BOL_Main_Battle.instance.health_right, me_hpResult, health_right_df);
                            //BOL_SetupGame.instance.TweenText("-" + me_hpLeft + " right hp", Color.red, BOL_SetupGame.instance.infoRight.transform);
                            break;
                    }
                    break;
                }
            case 3:
                {
                    short enemy_hpLeft = message.readShort();
                    short enemy_hpResult = message.readShort();
                    short enemy_manaResult = message.readShort();
                    short enemy_shieldResult = message.readShort();
                    long enemy_time_virtual_hp = message.readLong();
                    short enemy_value_virtual_hp = message.readShort();
                    switch (chairId)
                    {
                        case (sbyte)Constant.CHAIR_LEFT:
                            //SetScaleObject(BOL_Main_Battle.instance.health_right, enemy_hpResult, health_right_df);
                            //BOL_SetupGame.instance.TweenText("-" + enemy_hpLeft + " right hp", Color.red, BOL_SetupGame.instance.infoRight.transform);

                            BOL_Main_Controller.instance._BOL_PlayBattle_right.SetValueHPorMP(BOL_Battle_PlayerInGame.HEALTH,
                             enemy_hpResult, health_right_df, Constant.CHAIR_RIGHT);
                            break;
                        case (sbyte)Constant.CHAIR_RIGHT:
                            //SetScaleObject(BOL_Main_Battle.instance.health_left, enemy_hpResult, health_left_df);
                            // Debug.LogWarning(enemy_hpLeft);
                            //BOL_SetupGame.instance.TweenText("-" + enemy_hpLeft + " left hp", Color.red, BOL_SetupGame.instance.infoLeft.transform);
                            BOL_Main_Controller.instance._BOL_PlayBattle_left.SetValueHPorMP(BOL_Battle_PlayerInGame.HEALTH,
                             enemy_hpResult, health_right_df, Constant.CHAIR_LEFT);
                            break;
                    }
                    break;
                }
        }
    }
    void S_XHCD_PieceBreak_ATTACK_2(MessageReceiving message)
    {
        if (BOL_Manager.instance.CanPlayMusicAndSfx())
        {
            MyAudioManager.instance.PlaySfx(BOL_Manager.instance.myAudioInfo.sfx_Attack2);
        }
        sbyte chairId = message.readByte();
        sbyte characterid = message.readByte();
        sbyte numberPiece = message.readByte();
        Debugs.LogCyan(chairId + " attack 2" + "characterid  " + characterid + " numberpiece" + numberPiece);

        switch (chairId)
        {
            case (sbyte)Constant.CHAIR_LEFT:
                {
                    for (int i = 0; i < numberPiece; i++)
                    {
                        sbyte rowbreak = message.readByte();
                        sbyte columnbreak = message.readByte();
                        //BOL_Manager_Matrix.instance.AddBreakInMatrix(Constant.CHAIR_LEFT, rowbreak, columnbreak);
                        BOL_Main_Controller.instance._BOL_PlayBattle_left.AddBreakInMatrix(rowbreak, columnbreak);
                    }
                    BOL_PlaySkill_Controller.instance._listAction.Add(BOL_PlaySkill_Controller.instance.Left_Attack2);

                    break;
                }
            case (sbyte)Constant.CHAIR_RIGHT:
                {
                    for (int i = 0; i < numberPiece; i++)
                    {
                        sbyte rowbreak = message.readByte();
                        sbyte columnbreak = message.readByte();
                        //BOL_Manager_Matrix.instance.AddBreakInMatrix(Constant.CHAIR_RIGHT, rowbreak, columnbreak);
                        BOL_Main_Controller.instance._BOL_PlayBattle_right.AddBreakInMatrix(rowbreak, columnbreak);
                    }
                    BOL_PlaySkill_Controller.instance._listAction.Add(BOL_PlaySkill_Controller.instance.Right_Attack2);
                    break;
                }
        }
        short damageCalculator = message.readShort();
        long timeDelayUpDamage = message.readLong();
        short valueUpdamage = message.readShort();
        long timeDelayDownDamage = message.readLong();
        short valueDownDamage = message.readShort();
        long timeDelaySucking = message.readLong();
        short valueSucking = message.readShort();
        short hp = message.readShort();
        short virtualHp = message.readShort();
        long time_virtual_hp = message.readLong();
        short mana = message.readShort();
        short shield = message.readShort();
        short competitor_hp = message.readShort();
        short competitor_virtualHp = message.readShort();
        long competitor_time_virtual_hp = message.readLong();
        short competitor_mana = message.readShort();
        short competitor_shield = message.readShort();
        sbyte caseCheck = message.readByte();
        switch (caseCheck)
        {
            case 1:
                {
                    Debug.Log("không đọc effect");
                    break;
                }
            case 2:
                {
                    Debug.Log("phản damge");
                    break;
                }
            case 3:
                {
                    sbyte effectID = message.readByte();
                    switch (effectID)
                    {
                        case 1:
                            {
                                Debug.Log("stun");
                                long timeAvoidEffect = message.readLong();
                                long timeStun = message.readLong();
                                break;
                            }
                        case 2:
                            {
                                Debug.Log(" don't recover HP");
                                long timeAvoidEffect = message.readLong();
                                break;
                            }
                        case 3:
                            {
                                Debug.Log(" up damge");
                                short damageValue = message.readShort();
                                short damageResult = message.readShort();
                                short timeAddDamage = message.readShort();
                                break;
                            }
                        case 4:
                            {
                                Debug.Log("slow matrix");
                                long timeSlowMatrix = message.readLong();
                                break;
                            }
                        case 5:
                            {
                                Debug.Log("EFFECT_DoNotMovePiece (5)");
                                long timeAvoidEffect = message.readLong();
                                long timeDonotMovePiece = message.readLong();
                                break;
                            }
                        case 6:
                            {
                                Debug.Log("//EFFECT_DoNotChangePiece (6)");
                                long timeAvoidEffect = message.readLong();
                                long timeDonotChangePiece = message.readLong();
                                break;
                            }
                        case 7:
                            {
                                Debug.Log("//EFFECT_DisableSpell (7)");

                                long timeAvoidEffect = message.readLong();
                                long timeDisableEffect = message.readLong();
                                break;
                            }
                        case 8:
                            {
                                Debug.Log("//EFFECT_Freeze (8)");
                                long timeAvoidEffect = message.readLong();
                                long timeFreeze = message.readLong();
                                break;
                            }
                        case 9:
                            {
                                Debug.Log("//EFFECT_SpeedUpMatrix (9)");
                                long timeAvoidEffect = message.readLong();
                                long timeSpeedUpMatrix = message.readLong();
                                break;
                            }
                        case 10:
                            {
                                Debug.Log("//EFFECT_HypnotizingPieceMoBackWards (10)");
                                long timeAvoidEffect = message.readLong();
                                long timeHypnotizingPieceMoBackWards = message.readLong();
                                break;
                            }
                        case 11:
                            {
                                Debug.Log("//EFFECT_backDamage (11)");
                                long timeBackDamage = message.readLong();
                                break;
                            }
                        case 12:
                            {
                                Debug.Log("//EFFECT_AvoidDamage (12)");
                                long timeAvoid = message.readLong();
                                break;
                            }
                        case 13:
                            {
                                Debug.Log("//EFFECT_DownDamage (13)");
                                long timeAvoidEffect = message.readLong();
                                long timeDownDamage = message.readLong();
                                short damageDown = message.readShort();
                                short damageDownSum = message.readShort();
                                break;
                            }
                        case 14:
                            {
                                Debug.Log("//EFFECT_VirtualHp (14)");
                                long timeAvaiableVirtualHp = message.readLong();
                                short valueAdd = message.readShort();
                                short sumValueAdd = message.readShort();
                                break;
                            }
                        case 15:
                            {
                                Debug.Log(" //EFFECT_Bloodsucking (15)");
                                long timeAvaiableBloodsucking = message.readLong();
                                short perthousand = message.readShort();
                                break;
                            }
                        case 16:
                            {
                                Debug.Log("//EFFECT_AvoidEffect (16)");
                                long timeAvoid = message.readLong();
                                break;
                            }
                        case 17:
                            {
                                Debug.Log("//EFFECT_BlindsMatrix (17)");
                                long timeBlind = message.readLong();
                                break;
                            }
                        case 18:
                            {
                                Debug.Log("//EFFECT_RecoverHP (18)");
                                short hpAdd = message.readShort();
                                short hpResult = message.readShort();
                                break;
                            }
                        case 19:
                            {
                                Debug.Log("//EFFECT_SuperShield (19)");
                                short shieldAdd = message.readShort();
                                short shieldResult = message.readShort();
                                break;
                            }
                        case 20:
                            {
                                Debug.Log("//EFFECT_HypnotizingVandalism (20)");
                                long timeAvoidEffect = message.readLong();
                                break;
                            }
                        case 21:
                            {
                                Debug.Log("//EFFECT_VandalismPiece (21)");
                                long timeAvoidEffect = message.readLong();
                                break;
                            }
                        case 22:
                            {
                                Debug.Log("//EFFECT_AutoMovePiece (22)");
                                long timeAvoidEffect = message.readLong();
                                break;
                            }
                        case 23:
                            {
                                Debug.Log("//EFFECT_AutoChangePiece (23)");
                                long timeAvoidEffect = message.readLong();
                                break;
                            }

                    }
                    break;
                }

        }
        short last_hp = message.readShort();
        short last_virtualHp = message.readShort();
        long last_time_virtual_hp = message.readLong();
        short last_mana = message.readShort();
        short last_shield = message.readShort();
        short last_competitor_hp = message.readShort();
        short last_competitor_virtualHp = message.readShort();
        long last_competitor_time_virtual_hp = message.readLong();
        short last_competitor_mana = message.readShort();
        short last_competitor_shield = message.readShort();
        switch (chairId)
        {
            case (sbyte)Constant.CHAIR_LEFT:
                {
                    BOL_Main_Controller.instance._BOL_PlayBattle_left.SetValueHPorMP(BOL_Battle_PlayerInGame.HEALTH, last_hp, health_left_df, Constant.CHAIR_LEFT);
                    BOL_Main_Controller.instance._BOL_PlayBattle_left.SetValueHPorMP(BOL_Battle_PlayerInGame.MANA, last_mana, mana_left_df, Constant.CHAIR_LEFT);
                    BOL_Main_Controller.instance._BOL_PlayBattle_right.SetValueHPorMP(BOL_Battle_PlayerInGame.HEALTH, last_competitor_hp, health_right_df, Constant.CHAIR_RIGHT);
                    BOL_Main_Controller.instance._BOL_PlayBattle_right.SetValueHPorMP(BOL_Battle_PlayerInGame.MANA, last_competitor_mana, mana_right_df, Constant.CHAIR_RIGHT);
                    break;
                }
            case (sbyte)Constant.CHAIR_RIGHT:
                {
                    BOL_Main_Controller.instance._BOL_PlayBattle_left.SetValueHPorMP(BOL_Battle_PlayerInGame.HEALTH, last_competitor_hp, health_left_df, Constant.CHAIR_LEFT);
                    BOL_Main_Controller.instance._BOL_PlayBattle_left.SetValueHPorMP(BOL_Battle_PlayerInGame.MANA, last_competitor_mana, mana_left_df, Constant.CHAIR_LEFT);
                    BOL_Main_Controller.instance._BOL_PlayBattle_right.SetValueHPorMP(BOL_Battle_PlayerInGame.HEALTH, last_hp, health_right_df, Constant.CHAIR_RIGHT);
                    BOL_Main_Controller.instance._BOL_PlayBattle_right.SetValueHPorMP(BOL_Battle_PlayerInGame.MANA, last_mana, mana_right_df, Constant.CHAIR_RIGHT);
                    break;
                }
        }

    }
    void S_XHCD_PieceBreak_HP(MessageReceiving message)
    {
        if (BOL_Manager.instance.CanPlayMusicAndSfx())
        {
            MyAudioManager.instance.PlaySfx(BOL_Manager.instance.myAudioInfo.sfx_HP);
        }
        sbyte chairId = message.readByte();
        sbyte numberPiece = message.readByte();
        Debugs.LogCyan(chairId + " HP" + " numberpiece" + numberPiece);
        switch (chairId)
        {
            case (sbyte)Constant.CHAIR_LEFT:
                {
                    for (int i = 0; i < numberPiece; i++)
                    {
                        sbyte rowbreak = message.readByte();
                        sbyte columnbreak = message.readByte();
                        BOL_Main_Controller.instance._BOL_PlayBattle_left.AddBreakInMatrix(rowbreak, columnbreak);
                    }
                    // BOL_PlaySkill_Controller.instance._listAction.Add(BOL_PlaySkill_Controller.instance.Left_Attack1);
                    break;
                }
            case (sbyte)Constant.CHAIR_RIGHT:
                {
                    for (int i = 0; i < numberPiece; i++)
                    {
                        sbyte rowbreak = message.readByte();
                        sbyte columnbreak = message.readByte();
                        BOL_Main_Controller.instance._BOL_PlayBattle_right.AddBreakInMatrix(rowbreak, columnbreak);
                    }
                    // BOL_PlaySkill_Controller.instance._listAction.Add(BOL_PlaySkill_Controller.instance.Right_Attack1);
                    break;
                }
        }
        sbyte isCanRecover = message.readByte();// (1: đang bị cấm hồi hp _ 2: hồi bình thường)
        short value = message.readShort();// value add
        short hp = message.readShort();// result
        switch (chairId)
        {
            case (sbyte)Constant.CHAIR_LEFT:
                {
                    BOL_Main_Controller.instance._BOL_PlayBattle_left.SetValueHPorMP(BOL_Battle_PlayerInGame.HEALTH,
                     hp, health_left_df, Constant.CHAIR_LEFT);
                    BOL_PlaySkill_Controller.instance.CallEffectBreakPiece((int)Constant.PieceIngame.health,
                    Constant.CHAIR_LEFT);
                    break;
                }
            case (sbyte)Constant.CHAIR_RIGHT:
                {
                    BOL_PlaySkill_Controller.instance.CallEffectBreakPiece((int)Constant.PieceIngame.health,
                     Constant.CHAIR_RIGHT);
                    BOL_Main_Controller.instance._BOL_PlayBattle_right.SetValueHPorMP(BOL_Battle_PlayerInGame.HEALTH,
                     hp, health_right_df, Constant.CHAIR_RIGHT);
                    break;
                }
        }
    }
    void S_XHCD_PieceBreak_MANA(MessageReceiving message)
    {
        if (BOL_Manager.instance.CanPlayMusicAndSfx())
        {
            MyAudioManager.instance.PlaySfx(BOL_Manager.instance.myAudioInfo.sfx_MN);
        }
        sbyte chairId = message.readByte();//: người tấn công
        sbyte numberPiece = message.readByte();
        Debugs.LogCyan(chairId + " mana" + " numberpiece" + numberPiece);
        switch (chairId)
        {
            case (sbyte)Constant.CHAIR_LEFT:
                {
                    for (int i = 0; i < numberPiece; i++)
                    {
                        sbyte rowbreak = message.readByte();
                        sbyte columnbreak = message.readByte();
                        //BOL_Manager_Matrix.instance.AddBreakInMatrix(Constant.CHAIR_LEFT, rowbreak, columnbreak);
                        BOL_Main_Controller.instance._BOL_PlayBattle_left.AddBreakInMatrix(rowbreak, columnbreak);
                    }
                    // BOL_PlaySkill_Controller.instance._listAction.Add(BOL_PlaySkill_Controller.instance.Left_Attack1);
                    break;
                }
            case (sbyte)Constant.CHAIR_RIGHT:
                {
                    for (int i = 0; i < numberPiece; i++)
                    {
                        sbyte rowbreak = message.readByte();
                        sbyte columnbreak = message.readByte();
                        //BOL_Manager_Matrix.instance.AddBreakInMatrix(Constant.CHAIR_RIGHT, rowbreak, columnbreak);
                        BOL_Main_Controller.instance._BOL_PlayBattle_right.AddBreakInMatrix(rowbreak, columnbreak);
                    }
                    // BOL_PlaySkill_Controller.instance._listAction.Add(BOL_PlaySkill_Controller.instance.Right_Attack1);
                    break;
                }
        }
        short value = message.readShort();
        short mana = message.readShort();
        switch (chairId)
        {
            case (sbyte)Constant.CHAIR_LEFT:
                {
                    //SetScaleObject(BOL_Main_Battle.instance.mana_left, mana, mana_left_df);
                    //BOL_SetupGame.instance.TweenText("+" + value, Color.blue, BOL_SetupGame.instance.infoLeft.transform);
                    BOL_Main_Controller.instance._BOL_PlayBattle_left.SetValueHPorMP(BOL_Battle_PlayerInGame.MANA,
                     mana, mana_left_df, Constant.CHAIR_LEFT);
                    BOL_PlaySkill_Controller.instance.CallEffectBreakPiece((int)Constant.PieceIngame.mana,
                    Constant.CHAIR_LEFT);
                    break;
                }
            case (sbyte)Constant.CHAIR_RIGHT:
                {
                    //SetScaleObject(BOL_Main_Battle.instance.mana_right, mana, mana_right_df);
                    //BOL_SetupGame.instance.TweenText("+" + value, Color.blue, BOL_SetupGame.instance.infoRight.transform);
                    BOL_Main_Controller.instance._BOL_PlayBattle_right.SetValueHPorMP(BOL_Battle_PlayerInGame.MANA,
                     mana, mana_right_df, Constant.CHAIR_RIGHT);
                    BOL_PlaySkill_Controller.instance.CallEffectBreakPiece((int)Constant.PieceIngame.mana,
                    Constant.CHAIR_RIGHT);
                    break;
                }
        }
    }
    void S_XHCD_PieceBreak_SHIELD(MessageReceiving message)
    {
        if (BOL_Manager.instance.CanPlayMusicAndSfx())
        {
            MyAudioManager.instance.PlaySfx(BOL_Manager.instance.myAudioInfo.sfx_Shied);
        }
        sbyte chairId = message.readByte();//: người tấn công;
        sbyte numberPiece = message.readByte();
        Debugs.LogCyan(chairId + " shield" + " numberpiece" + numberPiece);
        switch (chairId)
        {
            case (sbyte)Constant.CHAIR_LEFT:
                {
                    for (int i = 0; i < numberPiece; i++)
                    {
                        sbyte rowbreak = message.readByte();
                        sbyte columnbreak = message.readByte();
                        //BOL_Manager_Matrix.instance.AddBreakInMatrix(Constant.CHAIR_LEFT, rowbreak, columnbreak);
                        BOL_Main_Controller.instance._BOL_PlayBattle_left.AddBreakInMatrix(rowbreak, columnbreak);
                    }
                    // BOL_PlaySkill_Controller.instance._listAction.Add(BOL_PlaySkill_Controller.instance.Left_Attack1);
                    break;
                }
            case (sbyte)Constant.CHAIR_RIGHT:
                {
                    for (int i = 0; i < numberPiece; i++)
                    {
                        sbyte rowbreak = message.readByte();
                        sbyte columnbreak = message.readByte();
                        //BOL_Manager_Matrix.instance.AddBreakInMatrix(Constant.CHAIR_RIGHT, rowbreak, columnbreak);
                        BOL_Main_Controller.instance._BOL_PlayBattle_right.AddBreakInMatrix(rowbreak, columnbreak);
                    }
                    // BOL_PlaySkill_Controller.instance._listAction.Add(BOL_PlaySkill_Controller.instance.Right_Attack1);
                    break;
                }
        }
        short value = message.readShort();
        short shield = message.readShort();
        switch (chairId)
        {
            case (sbyte)Constant.CHAIR_LEFT:
                {
                    //BOL_SetupGame.instance.TweenText("+" + value, Color.yellow, BOL_SetupGame.instance.infoLeft.transform);
                    BOL_Main_Controller.instance._BOL_PlayBattle_left.SetValueHPorMP(BOL_Battle_PlayerInGame.SHIELD,
                     shield, shield_left_df, Constant.CHAIR_LEFT);
                    BOL_PlaySkill_Controller.instance.CallEffectBreakPiece((int)Constant.PieceIngame.shield,
                    Constant.CHAIR_LEFT);
                    break;
                }
            case (sbyte)Constant.CHAIR_RIGHT:
                {
                    //BOL_SetupGame.instance.TweenText("+" + value, Color.yellow, BOL_SetupGame.instance.infoRight.transform);
                    BOL_Main_Controller.instance._BOL_PlayBattle_right.SetValueHPorMP(BOL_Battle_PlayerInGame.SHIELD,
                     shield, shield_right_df, Constant.CHAIR_RIGHT);
                    BOL_PlaySkill_Controller.instance.CallEffectBreakPiece((int)Constant.PieceIngame.shield,
                    Constant.CHAIR_RIGHT);
                    break;
                }
        }
    }
    void S_XHCD_PieceBreak_SPECIAL(MessageReceiving message)
    {
        if (BOL_Manager.instance.CanPlayMusicAndSfx())
        {
            MyAudioManager.instance.PlaySfx(BOL_Manager.instance.myAudioInfo.sfx_Special);
        }
        sbyte chairId = message.readByte();//: người tấn công
        sbyte characterid = message.readByte();
        sbyte numberPiece = message.readByte();
        Debugs.LogCyan(chairId + " special " + "characterid  " + characterid + " numberpiece" + numberPiece);
        switch (chairId)
        {
            case (sbyte)Constant.CHAIR_LEFT:
                {
                    for (int i = 0; i < numberPiece; i++)
                    {
                        sbyte rowbreak = message.readByte();
                        sbyte columnbreak = message.readByte();
                        //BOL_Manager_Matrix.instance.AddBreakInMatrix(Constant.CHAIR_LEFT, rowbreak, columnbreak);
                        BOL_Main_Controller.instance._BOL_PlayBattle_left.AddBreakInMatrix(rowbreak, columnbreak);
                    }
                    // BOL_PlaySkill_Controller.instance._listAction.Add(BOL_PlaySkill_Controller.instance.Left_Attack1);
                    break;
                }
            case (sbyte)Constant.CHAIR_RIGHT:
                {
                    for (int i = 0; i < numberPiece; i++)
                    {
                        sbyte rowbreak = message.readByte();
                        sbyte columnbreak = message.readByte();
                        //BOL_Manager_Matrix.instance.AddBreakInMatrix(Constant.CHAIR_RIGHT, rowbreak, columnbreak);
                        BOL_Main_Controller.instance._BOL_PlayBattle_right.AddBreakInMatrix(rowbreak, columnbreak);
                    }
                    // BOL_PlaySkill_Controller.instance._listAction.Add(BOL_PlaySkill_Controller.instance.Right_Attack1);
                    break;
                }
        }
    }
    void S_XHCD_CALLSKILL_ERROR(MessageReceiving message)
    {
        sbyte errorCase = message.readByte();
        switch (errorCase)
        {
            case 1:
#if TEST
                Debug.Log("không đang trạng thái chơi game");
                // PopupManager.Instance.CreateToast("không đang trạng thái chơi game");
#endif
                break;
            case 2:
#if TEST
                Debug.Log("đang bị cấm dùng phép");
                // PopupManager.Instance.CreateToast("đang bị cấm dùng phép");
#endif
                break;
            case 3:
                long timedelayskill = message.readLong();
#if TEST
                Debug.Log("chưa hồi xong chiêu");
                // PopupManager.Instance.CreateToast("chưa hồi xong chiêu");
#endif
                break;
            case 4:
                short currentmana = message.readShort();
                short skillmana = message.readShort();
                PopupManager.Instance.CreateToast(MyLocalize.GetString("BOL/Message_NotEnoughMana"));

#if TEST
                // PopupManager.Instance.CreateToast("khong du mana");
                Debug.Log("khong du mana");
                // Debug.Log(Debugs.ColorString("current mana " + currentmana.ToString(), Color.red));
                // Debug.Log(Debugs.ColorString("skillmana " + skillmana.ToString(), Color.red));
#endif
                break;

        }
    }
    void S_XHCD_PLAYER_CALLSKILL(MessageReceiving message, int skill)
    {
        sbyte chairId = message.readByte();// người tấn công
        sbyte characterid = message.readByte();
#if TEST
        Debug.Log("vị trí tấn công chair " + chairId + " hero tấn công " + characterid);
#endif
        short damageCalculator = message.readShort();
        long timeDelayUpDamage = message.readLong();
        short valueUpdamage = message.readShort();
        long timeDelayDownDamage = message.readLong();
        short valueDownDamage = message.readShort();
        long timeDelaySucking = message.readLong();
        short valueSucking = message.readShort();

        // thơng tin 2 người chơi truoc khi tần công

        short hp = message.readShort();
        short virtualHp = message.readShort();
        long time_virtual_hp = message.readLong();
        short mana = message.readShort();
        short shield = message.readShort();

        short competitor_hp = message.readShort();
        short competitor_virtualHp = message.readShort();
        long competitor_time_virtual_hp = message.readLong();
        short competitor_mana = message.readShort();
        short competitor_shield = message.readShort();

        sbyte numberEffect = message.readByte();
        for (int i = 0; i < numberEffect; i++)
        {
            sbyte effectID = message.readByte();
            switch (effectID)
            {
                case 1:
                    {
                        Debug.Log("stun");
                        var timeAvoidEffect = message.readLong();
                        var timeStun = message.readLong();
                        break;
                    }
                case 2:
                    {
                        Debug.Log(" don't recover HP");
                        var timeAvoidEffect = message.readLong();
                        var timeDonotRecoverHP = message.readLong();
                        break;
                    }
                case 3:
                    {
                        Debug.Log(" up damge");
                        var damageValue = message.readShort();
                        var damageResult = message.readShort();
                        var timeAddDamage = message.readLong();
                        break;
                    }
                case 4:
                    {
                        Debug.Log("slow matrix");
                        var timeAvoidEffect = message.readLong();
                        var timeSlowMatrix = message.readLong();
                        break;
                    }
                case 5:
                    {
                        Debug.Log("EFFECT_DoNotMovePiece (5)");
                        var timeAvoidEffect = message.readLong();
                        var timeDonotMovePiece = message.readLong();
                        break;
                    }
                case 6:
                    {
                        Debug.Log("//EFFECT_DoNotChangePiece (6)");

                        var timeAvoidEffect = message.readLong();
                        var timeDonotChangePiece = message.readLong();

                        break;
                    }
                case 7:
                    {
                        Debug.Log("//EFFECT_DisableSpell (7)");

                        var timeAvoidEffect = message.readLong();
                        var timeDisableEffect = message.readLong();
                        break;
                    }
                case 8:
                    {
                        Debug.Log("//EFFECT_Freeze (8)");
                        var timeAvoidEffect = message.readLong();
                        var timeFreeze = message.readLong();
                        break;
                    }
                case 9:
                    {
                        Debug.Log("//EFFECT_SpeedUpMatrix (9)");
                        var timeAvoidEffect = message.readLong();
                        var timeSpeedUpMatrix = message.readLong();
                        break;
                    }
                case 10:
                    {
                        Debug.Log("//EFFECT_HypnotizingPieceMoBackWards (10)");
                        var timeAvoidEffect = message.readLong();
                        var timeHypnotizingPieceMoBackWards = message.readLong();
                        break;
                    }
                case 11:
                    {
                        Debug.Log("//EFFECT_backDamage (11)");
                        var timeBackDamage = message.readLong();
                        break;
                    }
                case 12:
                    {
                        Debug.Log("//EFFECT_AvoidDamage (12)");
                        var timeAvoid = message.readLong();
                        break;
                    }
                case 13:
                    {
                        Debug.Log("//EFFECT_DownDamage (13)");
                        var timeAvoidEffect = message.readLong();
                        var timeDownDamage = message.readLong();
                        var damageDown = message.readShort();
                        var damageDownSum = message.readShort();
                        break;
                    }
                case 14:
                    {
                        Debug.Log("//EFFECT_VirtualHp (14)");
                        var timeAvaiableVirtualHp = message.readLong();
                        var valueAdd = message.readShort();
                        var sumValueAdd = message.readShort();
                        break;
                    }
                case 15:
                    {
                        Debug.Log(" //EFFECT_Bloodsucking (15)");
                        var timeAvaiableBloodsucking = message.readLong();
                        var perthousand = message.readShort();
                        break;
                    }
                case 16:
                    {
                        Debug.Log("//EFFECT_AvoidEffect (16)");
                        var timeAvoid = message.readLong();
                        break;
                    }
                case 17:
                    {
                        Debug.Log("//EFFECT_BlindsMatrix (17)");
                        var timeBlind = message.readLong();
                        break;
                    }
                case 18:
                    {
                        Debug.Log("//EFFECT_RecoverHP (18)");
                        var hpAdd = message.readShort();
                        var hpResults = message.readShort();
                        break;
                    }
                case 19:
                    {
                        Debug.Log("//EFFECT_SuperShield (19)");
                        var shieldAdd = message.readShort();
                        var shieldResults = message.readShort();
                        break;
                    }
                case 20:
                    {
                        Debug.Log("//EFFECT_HypnotizingVandalism (20)");
                        var timeAvoidEffect = message.readLong();
                        break;
                    }
                case 21:
                    {
                        Debug.Log("//EFFECT_VandalismPiece (21)");
                        var timeAvoidEffect = message.readLong();
                        break;
                    }
                case 22:
                    {
                        Debug.Log("//EFFECT_AutoMovePiece (22)");
                        var timeAvoidEffect = message.readLong();
                        break;
                    }
                case 23:
                    {
                        Debug.Log("//EFFECT_AutoChangePiece (23)");
                        var timeAvoidEffect = message.readLong();
                        break;
                    }

            }
            break;
        }
        long timeDelaySkill = message.readLong();

        // thông tin sau khi tân công
        short hpResult = message.readShort();
        short virtualHpResult = message.readShort();
        long time_virtual_hpResult = message.readLong();
        short manaResult = message.readShort();
        short shieldResult = message.readShort();

        short competitor_hpResult = message.readShort();
        short competitor_virtualHpResult = message.readShort();
        long competitor_time_virtual_hpResult = message.readLong();
        short competitor_manaResult = message.readShort();
        short competitor_shieldResult = message.readShort();

#if TEST
        Debug.Log(Debugs.ColorString("vi trí thuc hien tấn công  chairId " + chairId, Color.red));
        Debug.Log(Debugs.ColorString("vi trí thuc hien tấn công  characterid " + characterid, Color.red));
#endif
        switch (chairId)
        {
            case (sbyte)Constant.CHAIR_LEFT:
                {
                    switch (skill)
                    {
                        case CMD_REALTIME.S_XHCD_PLAYER_CALLSKILL_1:
                            BOL_PlaySkill_Controller.instance.btnSkillQ.timeDelay = timeDelaySkill;
                            BOL_PlaySkill_Controller.instance._listAction.Add(BOL_PlaySkill_Controller.instance.Left_AttackQ);
#if TEST
                            Debug.Log("left play skill 1");
#endif
                            break;
                        case CMD_REALTIME.S_XHCD_PLAYER_CALLSKILL_2:
                            BOL_PlaySkill_Controller.instance.btnSkillW.timeDelay = timeDelaySkill;
                            BOL_PlaySkill_Controller.instance._listAction.Add(BOL_PlaySkill_Controller.instance.Left_AttackW);
#if TEST
                            Debug.Log("left play skill 2");
#endif
                            break;
                        case CMD_REALTIME.S_XHCD_PLAYER_CALLSKILL_ULTIMATE:
                            BOL_PlaySkill_Controller.instance.btnSkillE.timeDelay = timeDelaySkill;
                            BOL_PlaySkill_Controller.instance._listAction.Add(BOL_PlaySkill_Controller.instance.Left_AttackE);
#if TEST
                            Debug.Log("left play skill 3");
#endif
                            break;
                    }
                    break;
                }
            case (sbyte)Constant.CHAIR_RIGHT:
                {
                    switch (skill)
                    {
                        case CMD_REALTIME.S_XHCD_PLAYER_CALLSKILL_1:
                            BOL_PlaySkill_Controller.instance.btnSkillQ.timeDelay = timeDelaySkill;
                            BOL_PlaySkill_Controller.instance._listAction.Add(BOL_PlaySkill_Controller.instance.Right_AttackQ);
#if TEST
                            Debug.Log("right play skill 1");
#endif
                            break;
                        case CMD_REALTIME.S_XHCD_PLAYER_CALLSKILL_2:
                            BOL_PlaySkill_Controller.instance.btnSkillW.timeDelay = timeDelaySkill;
                            BOL_PlaySkill_Controller.instance._listAction.Add(BOL_PlaySkill_Controller.instance.Right_AttackW);
#if TEST
                            Debug.Log("right play skill 2");
#endif
                            break;
                        case CMD_REALTIME.S_XHCD_PLAYER_CALLSKILL_ULTIMATE:
                            BOL_PlaySkill_Controller.instance.btnSkillE.timeDelay = timeDelaySkill;
                            BOL_PlaySkill_Controller.instance._listAction.Add(BOL_PlaySkill_Controller.instance.Right_AttackE);
#if TEST
                            Debug.Log("right play skill 3");
#endif
                            break;
                    }
                    break;

                }
        }
    }
    void S_XHCD_USE_SPELL_ERROR(MessageReceiving message)
    {
        sbyte errorCase = message.readByte();
        long countdownspell = message.readLong();
#if TEST
        Debug.Log("countdownspell" + countdownspell);
#endif
        switch (errorCase)
        {
            case -1: Debug.Log(">>>>trạng thái chờ"); break;
            case -2: Debug.Log(">>>>đóng băng"); break;
            case -3: Debug.Log(">>>>stun"); break;
            case -4: Debug.Log(">>>>chua hết countdown"); break;
        }
    }
    void S_XHCD_PLAYER_USE_SPELL(MessageReceiving message)
    {
        var chairId = message.readByte();
        var characterID = message.readByte();
        sbyte spell = message.readByte();
        var countdowntime = message.readLong();
#if TEST
        Debug.Log("vị trí tấn công chair " + chairId + " hero tấn công " + characterID);
        Debug.Log("countdowntime" + countdowntime);
#endif
        switch (chairId)
        {
            case (sbyte)Constant.CHAIR_LEFT:
                {
                    BOL_PlaySkill_Controller.instance.btnSkillSpell.timeDelay = countdowntime;
                    BOL_PlaySkill_Controller.instance._listAction.Add(() => BOL_PlaySkill_Controller.instance.Left_AttackSpell(spell));
                    break;
                }
            case (sbyte)Constant.CHAIR_RIGHT:
                {
                    BOL_PlaySkill_Controller.instance.btnSkillSpell.timeDelay = countdowntime;
                    BOL_PlaySkill_Controller.instance._listAction.Add(() => BOL_PlaySkill_Controller.instance.Right_AttackSpell(spell));
                    break;
                }
        }

    }
    #endregion
}
