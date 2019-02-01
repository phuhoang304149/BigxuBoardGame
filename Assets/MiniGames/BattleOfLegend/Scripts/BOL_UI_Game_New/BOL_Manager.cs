using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Lean.Pool;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;

public class BOL_Manager : IMySceneManager
{

    public override Type mySceneType
    {
        get
        {
            return Type.BolGamePlay;
        }
    }

    public static BOL_Manager instance;

    [SerializeField] MyArrowFocusController arrowFocusGetGold;
    public BOL_Table_Info bol_Table_Info;
    public ScreenChatController screenChat;
    public PopupChatManager popupChatManager;
    public System.Action onPressBack;
    public GameObject iconNotificationChat;
    MessageSending messageSendingChat;
    public float TimeBetweenShots = 0.25f;
    public Text textTableID, textServerID;
    float leftTimestamp, rightTimestamp, downTimestamp;

    public long betDefault;

    [Header("Audio Info")]
    public BOL_AudioInfo myAudioInfo;
    public PopupChatController currentPopupChat { get; set; }
    public PopupChatController currentPopupChatTop { get; set; }
    public PopupChatController currentPopupChatLeft { get; set; }
    public PopupChatController currentPopupChatRight { get; set; }
    [Header("Test")]
    bool isConnect;

    #region List Proccess
    List<IEnumerator> listProcess_left;
    List<IEnumerator> listProcess_right;
    List<IEnumerator> listCurrent_left;
    List<IEnumerator> listCurrent_right;
    IEnumerator _processCurrent_left;
    IEnumerator _processCurrent_right;
    IEnumerator _processAction_left;
    IEnumerator _processAction_right;
    IEnumerator _DoActionCheckFocusIconGetGold;
    #endregion

    void Awake()
    {
        instance = this;
        CoreGameManager.instance.currentSceneManager = instance;
        listProcess_left = new List<IEnumerator>();
        listProcess_right = new List<IEnumerator>();
        listCurrent_right = new List<IEnumerator>();
        listCurrent_left = new List<IEnumerator>();
    }

    void Start()
    {
#if TEST
        Debugs.LogRed(">>> vào scene");
#endif

        if (NetworkGlobal.instance != null && NetworkGlobal.instance.instanceRealTime != null)
        {
            NetworkGlobal.instance.instanceRealTime.onDisconnect = () =>
            {
                LoadingCanvasController.instance.Hide();
                PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
                    , MyLocalize.GetString(MyLocalize.kConnectionError)
                    , string.Empty
                    , MyLocalize.GetString(MyLocalize.kOk)
                    , () =>
                    {
                        Debug.LogError("xử lý chuyển scene khi mất kết nối");
                        CoreGameManager.instance.SetUpOutRoomAndBackToChooseTableScreen();
                    });

            };
            DataManager.instance.userData.sessionId = NetworkGlobal.instance.instanceRealTime.sessionId;
        }
        StartCoroutine(DoActionRun());
        //StartControlMatrix();
        //StartCurrentMatrix();
    }

    public override void RefreshAgainWhenCloseSubGamePlay()
    {
#if TEST
        Debug.Log(Debugs.ColorString("dong sub game", Color.red));
#endif

        if (CoreGameManager.instance.currentSubGamePlay != null)
        {
            CoreGameManager.instance.currentSubGamePlay = null;
        }

        MyAudioManager.instance.PlayMusic(myAudioInfo.bgm);

        BOL_Main_Controller.instance.panelUserInfo.RefreshGoldInfo(true);
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_onPlayerAddGold, BolNetworkReceiving.instance.S_onPlayerAddGold);
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_SET_PARENT, BolNetworkReceiving.instance.S_PlayerSetParent);
        BolNetworkReceiving.instance.RegisterActionAlertUpdateServer();
    }


#if UNITY_EDITOR
    void Update()
    {
        if (Time.time >= leftTimestamp && Input.GetKeyDown(KeyCode.LeftArrow))
        {
            EventMove(CMD_REALTIME.C_XHCD_MOVE_LEFT);
            leftTimestamp = Time.time + TimeBetweenShots;
        }
        else if (Time.time >= rightTimestamp && Input.GetKeyDown(KeyCode.RightArrow))
        {
            EventMove(CMD_REALTIME.C_XHCD_MOVE_RIGHT);
            rightTimestamp = Time.time + TimeBetweenShots;
        }
        else if (Time.time >= downTimestamp && Input.GetKeyDown(KeyCode.DownArrow))
        {
            EventMove(CMD_REALTIME.C_XHCD_MOVE_DOWN);
            downTimestamp = Time.time + TimeBetweenShots;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            EventMove(CMD_REALTIME.C_XHCD_CHANGE_PIECE_STATE);
        }
        else if (Time.time >= rightTimestamp && Input.GetKeyDown(KeyCode.Joystick1Button3))
        {
            EventMove(CMD_REALTIME.C_XHCD_CHANGE_PIECE_STATE);
        }
        else if (Time.time >= rightTimestamp && Input.GetKeyDown(KeyCode.Joystick1Button1))
        {
            EventMove(CMD_REALTIME.C_XHCD_CHANGE_PIECE_STATE);
        }
        else if (Time.time >= rightTimestamp && Input.GetKeyDown(KeyCode.Joystick1Button2))
        {
            EventMove(CMD_REALTIME.C_XHCD_CHANGE_PIECE_STATE);
        }
        else if (Time.time >= rightTimestamp && Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            EventMove(CMD_REALTIME.C_XHCD_CHANGE_PIECE_STATE);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            //             if (isConnect)
            //             {
            //                 isConnect = false;
            // #if TEST
            //                 Debug.LogError("resume realtime");
            //                 NetworkGlobal.instance.instanceRealTime.ResumeReceiveMessage();
            // #endif

            //             }
            //             else
            //             {
            //                 isConnect = true;
            // #if TEST
            //                 Debug.LogError("pause realtime");
            //                 NetworkGlobal.instance.instanceRealTime.PauseReceiveMessage();
            // #endif
            //             }
        }
    }
#endif

    public void EventMove(int eventMove)
    {
        short cmd = Convert.ToInt16(eventMove);
        MessageSending mgs = new MessageSending(cmd);
        NetworkGlobal.instance.SendMessageRealTime(mgs);
    }
    public void SendChatInTable(string _message)
    {
        if (messageSendingChat == null)
        {
            messageSendingChat = new MessageSending(CMD_REALTIME.C_GAMEPLAY_CHAT_IN_TABLE);
        }
        else
        {
            messageSendingChat.ClearData();
        }
        messageSendingChat.writeString(_message);
        string _tmp = string.Empty;
        _tmp += _message;

#if TEST
        Debug.Log(">>>CMD Chat : " + messageSendingChat.getCMD() + "|" + _tmp);
#endif
        NetworkGlobal.instance.SendMessageRealTime(messageSendingChat);
    }

    IEnumerator DoActionRun()
    {
        yield return Yielders.EndOfFrame;
        InitFirstData();
        RegisterAction();
        NetworkGlobal.instance.instanceRealTime.ResumeReceiveMessage();
        yield return new WaitUntil(() => bol_Table_Info != null && bol_Table_Info.hasLoadTableInfo);
#if TEST
        Debug.Log("action init first data");
#endif
        yield return Yielders.Get(0.5f);
        canShowScene = true;
        MyAudioManager.instance.PlayMusic(myAudioInfo.bgm);

        onPressBack = () =>
        {
            if (SettingScreenController.instance.currentState == UIHomeScreenController.State.Show)
            {
                if (this.CanPlayMusicAndSfx())
                {
                    MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
                }
                SettingScreenController.instance.Hide();
            }
            else
            {
                OnButtonSettingClicked();
            }
        };
        CoreGameManager.instance.RegisterNewCallbackPressBackKey(onPressBack);
    }

    void InitFirstData()
    {
        //BOL_Main_Controller.instance.InitDataChoice();
        //BOL_Main_Controller.instance.InitDataShowHeroAndButton();
        textTableID.text = string.Format("Table {0:00}", DataManager.instance.miniGameData.currentMiniGameDetail.tableData.currentTableDetail.tableId);
        textServerID.text = DataManager.instance.miniGameData.currentMiniGameDetail.currentServerDetail.subServerName;

        StartCheckGold();

        InitAllCallback();
    }

    void InitAllCallback()
    {
        if (screenChat != null)
        {
            screenChat.onSendMessage = (_mess) =>
            {
                SendChatInTable(_mess);
            };
            screenChat.onStartShow += HideIconNotificationChat;
            screenChat.onHasNewMessage += ShowIconNotificationChat;
        }
    }

    void RegisterAction()
    {
#if TEST
        Debug.Log("register action update viewer and get table info");
#endif
        bol_Table_Info = new BOL_Table_Info();
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_GET_TABLE_INFO, (_mess) =>
        {
            bol_Table_Info.SetDataTableInfo(_mess);
            BolNetworkReceiving.instance.SetupReceive();
        });
    }

    public void OnPlayOffline()
    {
        SceneManager.LoadScene("BOL_battle_offline");
    }
    public void OnButtonSettingClicked()
    {
        if (this.CanPlayMusicAndSfx())
        {
            MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
        }
        SettingScreenController.instance.InitData();
        SettingScreenController.instance.Show();
        SettingScreenController.instance.LateInitData();
        SettingScreenController.instance.btnOutRoom.onClick.AddListener(OnButtonOutRoom);
    }
    public void OnButtonOutRoom()
    {
        if (BOL_Manager.instance.CanPlayMusicAndSfx())
        {
            MyAudioManager.instance.PlaySfx(BOL_Manager.instance.myAudioInfo.sfx_Press);
        }

        CoreGameManager.instance.SetUpOutRoomAndBackToChooseTableScreen();
        bol_Table_Info = null;
        BolNetworkReceiving.SelfDestruction();
        instance = null;
        //if (objectBattle == null) {
        //PopupManager.Instance.CreatePopupDialog(MyLocalize.GetString(MyLocalize.kWarning)
        //, MyLocalize.GetString("System/AskForOutRoom")
        //, string.Empty
        //, MyLocalize.GetString(MyLocalize.kYes)
        //, MyLocalize.GetString(MyLocalize.kNo)
        //, () => {
        //	CoreGameManager.instance.SetUpOutRoomAndBackToChooseTableScreen();
        //	bol_Table_Info = null;
        //	BolNetworkReceiving.SelfDestruction();
        //	instance = null;
        //}, null);

    }
    void ShowIconNotificationChat()
    {
        if (!iconNotificationChat.activeSelf)
        {
            if (CanPlayMusicAndSfx())
            {
                MyAudioManager.instance.PlaySfx(myAudioInfo.sfx_Press);
            }
        }
        iconNotificationChat.SetActive(true);
    }
    void HideIconNotificationChat()
    {
        iconNotificationChat.SetActive(false);
    }

    public void OnButtonChatClicked()
    {
        if (CanPlayMusicAndSfx())
        {
            MyAudioManager.instance.PlaySfx(myAudioInfo.sfx_Press);
        }

        screenChat.Show();
#if TEST
        Debug.Log(">>> Show window Chat");
#endif
    }
    public void ShowPopupChat(short _sessionId, string _strMess)
    {
        if (_sessionId == BOL_ShowPlayer_Manager.instance.listUserPlayGame[0].sessionId)
        {
            PopupChatController _popupChat = popupChatManager.CreatePopupChat(PopupChatManager.PopupChatPosType.Right, _strMess, new Vector3(-3.3f, 1, 0));
            AddPopUpChat(3, _popupChat);
        }
        else if (_sessionId == BOL_ShowPlayer_Manager.instance.listUserPlayGame[1].sessionId)
        {
            PopupChatController _popupChat = popupChatManager.CreatePopupChat(PopupChatManager.PopupChatPosType.Left, _strMess, new Vector3(3.3f, 1, 0));
            AddPopUpChat(2, _popupChat);
        }
        else
        {
            PopupChatController _popupChat = popupChatManager.CreatePopupChat(PopupChatManager.PopupChatPosType.Top, _strMess, new Vector3(0, 1.5f, 0));
            AddPopUpChat(1, _popupChat);
        }
    }

    public void AddPopUpChat(int pospopup, PopupChatController _popUpChat)
    {
        switch (pospopup)
        {
            case 1:
                if (currentPopupChatTop != null)
                {
                    DestroyPopUpChat(currentPopupChatTop);
                }
                currentPopupChatTop = _popUpChat;
                currentPopupChatTop.onSelfDestruction += DestroyPopUpChat;
                break;
            case 2:
                if (currentPopupChatLeft != null)
                {
                    DestroyPopUpChat(currentPopupChatLeft);
                }
                currentPopupChatLeft = _popUpChat;
                currentPopupChatLeft.onSelfDestruction += DestroyPopUpChat;
                break;
            case 3:
                if (currentPopupChatRight != null)
                {
                    DestroyPopUpChat(currentPopupChatRight);
                }
                currentPopupChatRight = _popUpChat;
                currentPopupChatRight.onSelfDestruction += DestroyPopUpChat;
                break;

        }

    }

    public void DestroyPopUpChat(MySimplePoolObjectController _popUpChat)
    {
        if (currentPopupChatTop == _popUpChat)
        {
#if TEST
            Debug.LogError("popup Top");
#endif
            if (currentPopupChatTop != null)
            {
                currentPopupChatTop.onSelfDestruction -= DestroyPopUpChat;
                currentPopupChatTop.SelfDestruction();
            }
            currentPopupChatTop = null;
        }
        else if (currentPopupChatLeft == _popUpChat)
        {
#if TEST
            Debug.LogError("popup Left");
#endif
            if (currentPopupChatLeft != null)
            {
                currentPopupChatLeft.onSelfDestruction -= DestroyPopUpChat;
                currentPopupChatLeft.SelfDestruction();
            }
            currentPopupChatLeft = null;
        }
        else if (currentPopupChatRight == _popUpChat)
        {
#if TEST
            Debug.LogError("popup Right");
#endif
            if (currentPopupChatRight != null)
            {
                currentPopupChatRight.onSelfDestruction -= DestroyPopUpChat;
                currentPopupChatRight.SelfDestruction();
            }
            currentPopupChatRight = null;
        }
    }
    public void OnButtonShopClicked()
    {
        if (CanPlayMusicAndSfx())
        {
            MyAudioManager.instance.PlaySfx(myAudioInfo.sfx_Press);
        }
        GetGoldScreenController.instance.InitData();
        GetGoldScreenController.instance.Show();
        GetGoldScreenController.instance.LateInitData();
    }
    public void OnButtonMiniGamesClicked()
    {
        if (CanPlayMusicAndSfx())
        {
            MyAudioManager.instance.PlaySfx(myAudioInfo.sfx_Press);
        }
        ChooseSubGameScreenController.instance.InitData();
        ChooseSubGameScreenController.instance.Show();
        ChooseSubGameScreenController.instance.LateInitData();
    }
    private void OnDestroy()
    {
        bol_Table_Info = null;
        BolNetworkReceiving.SelfDestruction();
        //StopControlMatrix();
        //StopCurrentMatrix();
        instance = null;
        screenChat = null;
    }
    //	public void CheckPing(string ip) {
    //#if TEST
    //		StartCoroutine(StartPing(ip));
    //#endif
    //}
    //IEnumerator StartPing(string ip) {
    //	while (true) {
    //		yield return Yielders.Get(0.5f);
    //		WaitForSeconds f = new WaitForSeconds(0.05f);
    //		Ping p = new Ping(ip);
    //		while (p.isDone == false) {
    //			yield return f;
    //		}
    //		PingFinished(p);
    //	}
    //}
    //public void PingFinished(Ping p) {
    //	texPing.text = p.time.ToString();
    //	if (p.time < 80) {
    //		ImagePing.color = Color.green;
    //	} else if (p.time < 120) {
    //		ImagePing.color = Color.yellow;
    //	} else {
    //		ImagePing.color = Color.red;
    //	}
    //}


    IEnumerator ProcessAction_left()
    {
        while (true)
        {
            if (!canShowScene)
            {
                yield return null;
                continue;
            }
#if TEST
            Debug.Log(Debugs.ColorString("Left   đang có ", Color.red) + listProcess_left.Count + Debugs.ColorString("   process chuẩn bị chạy", Color.red));
#endif
            yield return new WaitUntil(() => listProcess_left.Count > 0);
            yield return StartCoroutine(listProcess_left[0]);

            listProcess_left.RemoveAt(0);
        }
    }
    IEnumerator ProcessAction_right()
    {

        while (true)
        {
            if (!canShowScene)
            {
                yield return null;
                continue;
            }
#if TEST
            Debug.Log(Debugs.ColorString("Right   đang có ", Color.red) + listProcess_right.Count + Debugs.ColorString("   process chuẩn bị chạy", Color.red));
#endif
            yield return new WaitUntil(() => listProcess_right.Count > 0);

            yield return StartCoroutine(listProcess_right[0]);

            listProcess_right.RemoveAt(0);
        }
    }

    public void StartControlMatrix()
    {
        if (_processAction_left != null)
        {
            StopCoroutine(_processAction_left);
            _processAction_left = null;
        }
        if (_processAction_right != null)
        {
            StopCoroutine(_processAction_right);
            _processAction_right = null;
        }
        _processAction_left = ProcessAction_left();
        _processAction_right = ProcessAction_right();
        StartCoroutine(_processAction_left);
        StartCoroutine(_processAction_right);
        //CoroutineChain.Start.Parallel(_processAction_left, _processAction_right);
    }
    //void StopControlMatrix() {
    //	CoroutineChain.StopAll();
    //}
    public void AddAction2Process(int position, Action action, string nameAction = "")
    {
        switch (position)
        {
            case (int)Constant.CHAIR_LEFT:
                listProcess_left.Add(AddProcesAction(action, nameAction));
                break;
            case (int)Constant.CHAIR_RIGHT:
                listProcess_right.Add(AddProcesAction(action, nameAction));
                break;
        }
    }

    IEnumerator ProcessCurrentLeft()
    {
        while (true)
        {
            if (!canShowScene)
            {
                yield return null;
                continue;
            }
#if TEST
            Debug.Log(Debugs.ColorString("currentLeft ", Color.red) + listCurrent_left.Count + Debugs.ColorString("   process chuẩn bị chạy", Color.red));
#endif
            yield return new WaitUntil(() => listCurrent_left.Count > 0);
            yield return StartCoroutine(listCurrent_left[0]);
            listCurrent_left.RemoveAt(0);
#if TEST
            Debug.Log(Debugs.ColorString("Left   còn lại ", Color.red) + listCurrent_left.Count + Debugs.ColorString("  process đang chờ", Color.red));
#endif
        }
    }

    IEnumerator ProcessCurrentRight()
    {
        while (true)
        {
            if (!canShowScene)
            {
                yield return null;
                continue;
            }
#if TEST
            Debug.Log(Debugs.ColorString("currentRight ", Color.red) + listCurrent_right.Count + Debugs.ColorString("   process chuẩn bị chạy", Color.red));
#endif
            yield return new WaitUntil(() => listCurrent_right.Count > 0);
            yield return StartCoroutine(listCurrent_right[0]);
            listCurrent_right.RemoveAt(0);
#if TEST
            Debug.Log(Debugs.ColorString("Left   còn lại ", Color.red) + listCurrent_right.Count + Debugs.ColorString("  process đang chờ", Color.red));
#endif
        }
    }

    public void StartCurrentMatrix()
    {
        if (_processCurrent_left != null)
        {
            StopCoroutine(_processCurrent_left);
            _processCurrent_left = null;
        }
        if (_processCurrent_right != null)
        {
            StopCoroutine(_processCurrent_right);
            _processCurrent_right = null;
        }
        _processCurrent_left = ProcessCurrentLeft();
        _processCurrent_right = ProcessCurrentRight();
        StartCoroutine(_processCurrent_left);
        StartCoroutine(_processCurrent_right);
        //CoroutineChain.Start.Parallel(_processCurrent_left, _processCurrent_right);
    }
    //void StopCurrentMatrix() {
    //	CoroutineChain.StopAll();
    //}
    public void AddCurrent2Process(int position, Action action, string nameAction = "")
    {
        switch (position)
        {
            case (int)Constant.CHAIR_LEFT:
                listCurrent_left.Add(AddProcesAction(action, nameAction));
                break;
            case (int)Constant.CHAIR_RIGHT:
                listCurrent_right.Add(AddProcesAction(action, nameAction));
                break;
        }

    }
    IEnumerator AddProcesAction(Action action, string nameAction = "")
    {
#if TEST
        Debug.Log(nameAction + Debugs.ColorString("   is running", Color.red));
#endif
        action();
        yield break;
    }

    IEnumerator DoActionCheckFocusIconGetGold()
    {
        while (true)
        {
            if (!canShowScene)
            {
                yield return null;
                continue;
            }
            if (DataManager.instance.userData.gold < betDefault)
            {
                arrowFocusGetGold.Show();
            }
            else
            {
                arrowFocusGetGold.Hide();
            }
            yield return Yielders.Get(1f);
        }
    }
    void StartCheckGold()
    {
        if (_DoActionCheckFocusIconGetGold != null)
        {
            StopCoroutine(_DoActionCheckFocusIconGetGold);
            _DoActionCheckFocusIconGetGold = null;
        }
        _DoActionCheckFocusIconGetGold = DoActionCheckFocusIconGetGold();
        StartCoroutine(_DoActionCheckFocusIconGetGold);
    }

    public bool CanPlayMusicAndSfx()
    {
        if (DataManager.instance.miniGameData.currentSubGameDetail != null)
        {
            return false;
        }
        return true;
    }

}
[System.Serializable]
public class BOL_AudioInfo
{
    [Header("Playback")]
    public AudioClip bgm;
    [Header("Sfx")]
    public AudioClip sfx_Start;
    public AudioClip sfx_Piece;
    public AudioClip sfx_Press;
    public AudioClip sfx_Skill;
    public AudioClip sfx_Move;
    public AudioClip sfx_Win;
    public AudioClip sfx_Lose;
    public AudioClip sfx_Gold;
    public AudioClip sfx_break;


    public AudioClip sfx_Attack1;
    public AudioClip sfx_Attack2;
    public AudioClip sfx_HP;
    public AudioClip sfx_MN;
    public AudioClip sfx_Shied;
    public AudioClip sfx_Special;

    public AudioClip sfx_Spell1;
    public AudioClip sfx_Spell2;
    public AudioClip sfx_Spell3;
    public AudioClip sfx_Spell4;
    public AudioClip sfx_Spell5;
    public AudioClip sfx_Spell6;

}
