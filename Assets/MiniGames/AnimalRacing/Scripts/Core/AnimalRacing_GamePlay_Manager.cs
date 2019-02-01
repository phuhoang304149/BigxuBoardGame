using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimalRacing_GamePlay_Manager : IMySceneManager
{
    public override Type mySceneType{
		get{
			return Type.AnimalRacingGameplay;
		}
	}

    public static AnimalRacing_GamePlay_Manager instance;

    public enum State
    {
        Bet,
        ShowResult
    }
    public State currentState { get; set; }

    [Header("Manager")]
    public AnimalRacing_Bet_Manager betManager;
    public AnimalRacing_Result_Manager resultManager;
    public AnimalRacing_CallbackManager callbackManager;
    public PopupChatManager popupChatManager{get;set;}
    [SerializeField] ScreenChatController screenChat{get;set;}
    [SerializeField] GameObject iconNotificationChat;
    [SerializeField] MyArrowFocusController arrowFocusGetGold;
    [SerializeField] Image shadowChangeScreen;


    [Header("GamePlay Info")]
    public List<AnimalRacing_AnimalInfo> listAnimalInfo;
    public AnimalRacing_SortingLayerManager sortingLayerManager;
    public AnimalRacingData animalRacingData;

    [Header("Audio Info")]
    public AnimalRacing_AudioInfo myAudioInfo;

    [Header("Prefabs")]
	[SerializeField] GameObject screenChatPrefab;
	[SerializeField] GameObject popupChatManagerPrefab;

    IEnumerator actionRunProcessPlaying, actionRunProcessNonPlaying, actionCheckFocusIconGetGold;
	List<IEnumerator> listProcessPlaying;
    List<IEnumerator> listProcessNonPlaying;

    public System.Action onPressBack;


    [ContextMenu("Test Sort List Animal Info")]
    void TestSortListAnimalInfo()
    {
        if (listAnimalInfo == null)
        {
            Debug.LogError("listAnimalInfo is null");
            return;
        }

        listAnimalInfo.Sort(delegate (AnimalRacing_AnimalInfo x, AnimalRacing_AnimalInfo y)
        {
            Debug.Log(x.animalType + " - " + y.animalType + " - " + x.animalType.CompareTo(y.animalType));
            return x.animalType.CompareTo(y.animalType);
        });
    }

    private void Awake()
    {
        instance = this;
        CoreGameManager.instance.currentSceneManager = instance;
    }

    private void Start()
    {
        if(NetworkGlobal.instance.instanceRealTime != null){
			NetworkGlobal.instance.instanceRealTime.onDisconnect = () =>{
				LoadingCanvasController.instance.Hide();
				PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
					, MyLocalize.GetString(MyLocalize.kConnectionError)
					, string.Empty
					, MyLocalize.GetString(MyLocalize.kOk)
					, () =>
					{
						Debug.LogError("xử lý chuyển scene khi mất kết nối");
                        if(callbackManager != null
                            && callbackManager.onDestructAllObject != null){
                            callbackManager.onDestructAllObject();
                        }
                        DataManager.instance.userData.RemoveTotalBetInGameInfo(IMiniGameInfo.Type.AnimalRacing);
						CoreGameManager.instance.SetUpOutRoomAndBackToChooseTableScreen();
					});
			};
            DataManager.instance.userData.sessionId = NetworkGlobal.instance.instanceRealTime.sessionId;
		}
        StartCoroutine(DoActionRun());
    }

    IEnumerator DoActionRun()
    {
        yield return null;
        resultManager.Hide();
        InitData();

        actionRunProcessPlaying = DoActionRunProcessPlaying();
		StartCoroutine(actionRunProcessPlaying);

        actionRunProcessNonPlaying = DoActionRunProcessNonPlaying();
		StartCoroutine(actionRunProcessNonPlaying);

        actionCheckFocusIconGetGold = DoActionCheckFocusIconGetGold();
		StartCoroutine(actionCheckFocusIconGetGold);

        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_GET_TABLE_INFO, (_mess)=>{
            animalRacingData.InitDataWhenGetTableInfo(_mess);
			if(_mess.avaiable() > 0){
				#if TEST
				Debug.Log (">>> Chua doc het CMD : " + _mess.getCMDName ());
				#endif
			}
            animalRacingData.CheckListHistoryAgain();
         
            betManager.RefreshUIPanelListHistory();
            betManager.RefreshUIPanelTableBet(true);
            betManager.RefreshUIPanelCurrentScore(true);
            betManager.RefreshUIPanelListPlayerViewer();

            RegisterActionUpdateResultGame();
           
            RegisterActionPlayerJoinGame();
            RegisterActionPlayerLeftGame();
            RegisterActionCheckMeAddBet();
            RegisterActionCheckPlayerAddBet();
            RegisterActionUpdateTableBet();
            RegisterActionPlayerAddGold();
            RegisterActionSetParentInfo();
            RegisterActionPlayerChat();
            RegisterActionAlertUpdateServer();
        });
        NetworkGlobal.instance.instanceRealTime.ResumeReceiveMessage();

        yield return new WaitUntil(() => animalRacingData != null && animalRacingData.hasLoadTableInfo);
        yield return Yielders.Get(0.5f);
    
        canShowScene = true;

        MyAudioManager.instance.PlayMusic(myAudioInfo.bgm);

        betManager.Show(true);
        if (animalRacingData != null&& animalRacingData.currentResultData != null) {
            StartCoroutine(DoActionShowResultScreen());
        }else {
            StartCountDown();
        }

        onPressBack = ()=>{
			if(SettingScreenController.instance.currentState == UIHomeScreenController.State.Show){
				if(this.CanPlayMusicAndSfx()){
					MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
				}
				SettingScreenController.instance.Hide();
			}else{
				OnButtonSettingClicked();
			}
		};
		CoreGameManager.instance.RegisterNewCallbackPressBackKey (onPressBack);
    }

    void InitData()
    {
        currentState = State.Bet;
        callbackManager = new AnimalRacing_CallbackManager();
        animalRacingData = new AnimalRacingData();
        listProcessPlaying = new List<IEnumerator>();
        listProcessNonPlaying = new List<IEnumerator>();
        shadowChangeScreen.gameObject.SetActive(false);
        HideIconNotificationChat();
        betManager.InitData();

        screenChat = ((GameObject) Instantiate(screenChatPrefab, transform)).GetComponent<ScreenChatController>();
		popupChatManager = ((GameObject) Instantiate(popupChatManagerPrefab, transform)).GetComponent<PopupChatManager>();

        // -------- Init Callback -------- //
        InitAllCallback();
        // ------------------------------- //
    }

    void InitAllCallback(){
        
        callbackManager.onDestructAllObject = ()=>{
            betManager.effectPoolManager.ClearAllObjectsNow();
        };

        callbackManager.onStartShowResult += () =>
        {
            betManager.effectPoolManager.ClearAllObjectsNow();

            DataManager.instance.userData.SetTotalBetInGameInfo(IMiniGameInfo.Type.AnimalRacing, 0);
            for(int i = 0; i < animalRacingData.listOtherPlayerData.Count; i++){
                if(animalRacingData.listOtherPlayerData[i].sessionId >= 0){
                    animalRacingData.listOtherPlayerData[i].SetTotalBetInGameInfo(IMiniGameInfo.Type.AnimalRacing, 0);
                }
            }

            betManager.panelUserInGame.RefreshGoldInfo(true);
            for (int i = 0; i < betManager.listOtherPlayerInfo.Count; i++)
            {
                betManager.listOtherPlayerInfo[i].RefreshGoldInfo(true);
            }
            for (int i = 0; i < betManager.panelTableBet.listBetOption.Count; i++)
            {
                betManager.panelTableBet.listBetOption[i].glowParticle.gameObject.SetActive(false);
            }
            popupChatManager.ForcedRemoveAll();
            
            if(DataManager.instance.miniGameData.currentSubGameDetail == null){
                MyAudioManager.instance.PauseAll();
            }
        };
        callbackManager.onEndShowResult += () =>
        {
            for (int i = 0; i < betManager.panelTableBet.listBetOption.Count; i++)
            {
                betManager.panelTableBet.listBetOption[i].glowParticle.gameObject.SetActive(true);
            }
            if(DataManager.instance.miniGameData.currentSubGameDetail == null){
                MyAudioManager.instance.ResumeAll();
            }
        };

        screenChat.onSendMessage = (_mess) =>
        {
            AnimalRacing_RealTimeAPI.instance.SendMessageChat(_mess);
        };
        screenChat.onStartShow += HideIconNotificationChat;
		screenChat.onHasNewMessage += ShowIconNotificationChat;
    }

    void ShowIconNotificationChat(){
        if(!iconNotificationChat.activeSelf){
			if(this.CanPlayMusicAndSfx()){
				MyAudioManager.instance.PlaySfx(myAudioInfo.sfx_Notification);
			}
		}
		iconNotificationChat.SetActive(true);
	}
	void HideIconNotificationChat(){
		iconNotificationChat.SetActive(false);
	}

    #region Register Action RealTime
    void RegisterActionAlertUpdateServer()
    {
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_ALERT_UPDATE_SERVER, (_mess) =>
        {
            if(animalRacingData != null){
                MyGamePlayData.AlertUpdateServer_Data _data = new MyGamePlayData.AlertUpdateServer_Data(_mess);
                System.TimeSpan _timeSpanRemain = _data.timeToUpdateServer - System.DateTime.Now;                
                PopupManager.Instance.CreateToast(string.Format(MyLocalize.GetString("System/Message_ServerMaintenance"), _timeSpanRemain.Minutes, _timeSpanRemain.Seconds));
            }
        });
    }
    void RegisterActionUpdateResultGame()
    {
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_RESULT_GAME, (_mess) =>
        {
            if(animalRacingData != null){
                animalRacingData.SetDataWhenShowResult(_mess);
                listProcessPlaying.Add(DoActionSetResultData());
            }
        });
    }

    void RegisterActionPlayerJoinGame(){
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_PLAYER_JOIN_GAME, (_mess) =>
        {
            if(animalRacingData != null){
                animalRacingData.SetUpUserJoinGame(_mess);
                listProcessNonPlaying.Add(DoActionCheckPlayerJoinGame());
            }
        });
    }

    void RegisterActionPlayerLeftGame(){
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_PLAYER_LEFT_GAME, (_mess) =>
        {
            if(animalRacingData != null){
                animalRacingData.SetUpUserLeftGame(_mess);
                listProcessNonPlaying.Add(DoActionCheckPlayerLeftGame());
            }
        });
    }

    void RegisterActionUpdateTableBet(){
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_UPDATE_TABLE_BET, (_mess) =>
        {
            if(animalRacingData != null){
                animalRacingData.SetDataUpdateTableBet(_mess);
                listProcessNonPlaying.Add(DoActionCheckUpdateTableBet());
            }
        });
    }

    void RegisterActionCheckMeAddBet(){
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_ADDBET, (_mess) =>
        {
            if(animalRacingData != null){
                animalRacingData.SetDataMeAddBet(_mess);
                listProcessNonPlaying.Add(DoActionCheckMeAddBet());
            }
        });
    }

    void RegisterActionCheckPlayerAddBet()
    {
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_PLAYER_ADDBET, (_mess) =>
        {   
            if(animalRacingData != null){
                animalRacingData.SetDataPlayerAddBet(_mess);
                listProcessNonPlaying.Add(DoActionCheckPlayerAddBet());
            }
        });
    }

    void RegisterActionPlayerAddGold()
    {
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_onPlayerAddGold, (_mess) =>
        {
            if(animalRacingData != null){
                animalRacingData.SetPlayerAddGoldData(_mess);
                listProcessNonPlaying.Add(DoActionPlayerAddGold());
            }
        });
    }

    void RegisterActionSetParentInfo(){
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_SET_PARENT, (_mess) => 
		{
            if(animalRacingData != null){
                animalRacingData.SetDataWhenSetParent(_mess);
                listProcessNonPlaying.Add(DoActionPlayerSetParent());
            }
		});
	}

    void RegisterActionPlayerChat()
    {
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_CHAT_IN_TABLE, (_mess) =>
        {   
            if(animalRacingData != null){
                animalRacingData.SetPlayerChatData(_mess);
                listProcessNonPlaying.Add(DoActionPlayerChat());
            }
        });
    }
    #endregion

    #region Logic Game
    IEnumerator DoActionRunProcessPlaying(){
		while(true){
			if(!canShowScene){
				yield return null;
				continue;
			}
			if(listProcessPlaying.Count == 0){
				yield return new WaitUntil(()=> listProcessPlaying.Count > 0);
			}
			yield return StartCoroutine(listProcessPlaying[0]);
			listProcessPlaying.RemoveAt(0);
		}
	}
    
    IEnumerator DoActionRunProcessNonPlaying(){
		while(true){
			if(!canShowScene){
				yield return null;
				continue;
			}
			if(listProcessNonPlaying.Count == 0){
				yield return new WaitUntil(()=> listProcessNonPlaying.Count > 0);
			}
			StartCoroutine(listProcessNonPlaying[0]);
			listProcessNonPlaying.RemoveAt(0);
		}
	}

    IEnumerator DoActionCheckFocusIconGetGold(){
		while(true){
			if(!canShowScene){
				yield return null;
				continue;
			}
			if(DataManager.instance.userData.gold <= 0){
			    ShowArrowFocusGetGold();
            }else{
                HideArrowFocusGetGold();
            }
			yield return Yielders.Get(1f);
		}
	}

    IEnumerator DoActionPlayerChat(){
		// #if TEST
		// Debug.Log("<color=green> Start process PlayerChat </color>");
		// #endif
		AnimalRacingData.PlayerChat_Data _playerChatData = animalRacingData.processPlayerChatData[0];
		screenChat.AddMessage(_playerChatData.sessionId, _playerChatData.strMess, animalRacingData.listOtherPlayerData);
        this.ShowPopupChat(_playerChatData.sessionId, _playerChatData.strMess);
		_playerChatData = null;
		animalRacingData.processPlayerChatData.RemoveAt(0);
		// #if TEST
		// Debug.Log("<color=green> End process PlayerChat: " + unoGamePlayData.processUnoPlayerChatData.Count +"</color>");
		// #endif
		yield break;
	}

	IEnumerator DoActionPlayerAddGold(){
		// #if TEST
		// Debug.Log("<color=green> Start process PlayerAddGold </color>");
		// #endif
		AnimalRacingData.PlayerAddGold_Data _playerAddGoldData = animalRacingData.processPlayerAddGoldData[0];
        betManager.SetUpPlayerAddGold(_playerAddGoldData.sessionId, _playerAddGoldData.reason, _playerAddGoldData.goldAdd, _playerAddGoldData.goldLast);
        _playerAddGoldData = null;
		animalRacingData.processPlayerAddGoldData.RemoveAt(0);
		// #if TEST
		// Debug.Log("<color=green> End process PlayerAddGold: " + animalRacingData.processUnoPlayerChatData.Count +"</color>");
		// #endif
		yield break;
	}

    IEnumerator DoActionGetAlertUpdateServer(){
        yield return null;
    }

    IEnumerator DoActionPlayerSetParent(){
		AnimalRacingData.PlayerSetParent_Data _playerSetParentData = animalRacingData.processPlayerSetParentData[0];
		animalRacingData.SetUpActionPlayerSetParent(_playerSetParentData);
		_playerSetParentData = null;
		animalRacingData.processPlayerSetParentData.RemoveAt(0);
		yield break;
	}

    IEnumerator DoActionCheckPlayerJoinGame(){
        AnimalRacingData.PlayerJoinGame_Data _playerJoinGameData = animalRacingData.processPlayerJoinGame[0];
        System.Action _onFinished = ()=>{
			_playerJoinGameData = null;
			animalRacingData.processPlayerJoinGame.RemoveAt(0);
		};

        if(currentState == State.Bet){
            yield return StartCoroutine(DoActionPlayerJoinGame(_playerJoinGameData));
        }else{
            listProcessPlaying.Add(DoActionPlayerJoinGame(_playerJoinGameData));
        }
        
        if(_onFinished != null){
            _onFinished();
        }
        yield break;
    }

    IEnumerator DoActionPlayerJoinGame(AnimalRacingData.PlayerJoinGame_Data _playerJoinGameData){
        // ---- Merge dữ liệu ---- //
        if(animalRacingData.listOtherPlayerData[_playerJoinGameData.viewerId].sessionId >= 0){
            #if TEST
            Debug.LogError(">>> Chỗ này đã có người rồi: " + _playerJoinGameData.viewerId);
            #endif
            yield break;
        }
        if(_playerJoinGameData.userData.sessionId != DataManager.instance.userData.sessionId){
            animalRacingData.listOtherPlayerData[_playerJoinGameData.viewerId] = _playerJoinGameData.userData;
            animalRacingData.listOtherPlayerData[_playerJoinGameData.viewerId].AddNewTotalBetInGameInfo(IMiniGameInfo.Type.AnimalRacing);
            #if TEST
            Debug.Log(">>> Có người chơi " + animalRacingData.listOtherPlayerData[_playerJoinGameData.viewerId].nameShowInGame + " vào bàn tại vị trí " + _playerJoinGameData.viewerId);
            #endif
        }
        // ----------------------- //

        betManager.RefreshUIPanelListPlayerViewer();
    }

    IEnumerator DoActionCheckPlayerLeftGame(){
        AnimalRacingData.PlayerLeftGame_Data _playerLeftGameData = animalRacingData.processPlayerLeftGame[0];
        System.Action _onFinished = ()=>{
			_playerLeftGameData = null;
			animalRacingData.processPlayerLeftGame.RemoveAt(0);
		};

        if(currentState == State.Bet){
            yield return StartCoroutine(DoActionPlayerLeftGame(_playerLeftGameData));
        }else{
            listProcessPlaying.Add(DoActionPlayerLeftGame(_playerLeftGameData));
        }

        if(_onFinished != null){
            _onFinished();
        }
        yield break;
    }

    IEnumerator DoActionPlayerLeftGame(AnimalRacingData.PlayerLeftGame_Data _playerLeftGameData){
        // ---- Merge dữ liệu ---- //
        bool _isExist = false;
        for(int i = 0; i < animalRacingData.listOtherPlayerData.Count; i++){
            if(animalRacingData.listOtherPlayerData[i].IsEqual(_playerLeftGameData.sessionId)){
                #if TEST
                Debug.Log(">>> Có người chơi " + animalRacingData.listOtherPlayerData[i].nameShowInGame + " thoát bàn tại vị trí " + i);
                #endif
                animalRacingData.listOtherPlayerData[i] = new UserDataInGame();
                _isExist = true;
                break;
            }
        }
        if(!_isExist){
            #if TEST
            Debug.LogError(">>> Không tìm thấy session ID: " + _playerLeftGameData.sessionId);
            #endif
            yield break;
        }
        // ----------------------- //

        betManager.RefreshUIPanelListPlayerViewer();
    }

    IEnumerator DoActionCheckUpdateTableBet(){
        AnimalRacingData.AnimalRacing_UpdateTableBet_Data _updateTableBetData = animalRacingData.processUpdateTableBet[0];
        System.Action _onFinished = ()=>{
			_updateTableBetData = null;
			animalRacingData.processUpdateTableBet.RemoveAt(0);
		};

        if(currentState == State.Bet){
            yield return StartCoroutine(DoActionUpdateTableBet(_updateTableBetData));
        }else{
            listProcessPlaying.Add(DoActionUpdateTableBet(_updateTableBetData));
        }

        if(_onFinished != null){
            _onFinished();
        }
    }

    IEnumerator DoActionUpdateTableBet(AnimalRacingData.AnimalRacing_UpdateTableBet_Data _updateTableBetData){
        // ---- Merge dữ liệu ---- //
        animalRacingData.listGlobalBets = _updateTableBetData.listGlobalBet;
        // ----------------------- //
        betManager.RefreshUIPanelTableBet();
        yield break;
    }

    IEnumerator DoActionCheckMeAddBet(){
        AnimalRacingData.AnimalRacing_MeAddBet_Data _meAddBetData = animalRacingData.processMeAddBet[0];
        System.Action _onFinished = ()=>{
			_meAddBetData = null;
			animalRacingData.processMeAddBet.RemoveAt(0);
		};

        if(currentState == State.Bet){
            yield return StartCoroutine(DoActionMeAddBet(_meAddBetData));
        }else{
            listProcessPlaying.Add(DoActionMeAddBet(_meAddBetData));
        }

        if(_onFinished != null){
            _onFinished();
        }
        yield break;
    }

    IEnumerator DoActionMeAddBet(AnimalRacingData.AnimalRacing_MeAddBet_Data _meAddBetData){
        // ---- Merge dữ liệu ---- //
        if(!_meAddBetData.caseCheck){
            #if TEST
            Debug.LogError(">>> MeAddPet Trường hợp tiền người chơi nhỏ hơn tổng cược: " + _meAddBetData.myGOLD + " - " + _meAddBetData.totalBet);
            #endif
            animalRacingData.listMyBets = _meAddBetData.listGlobalBet;

            DataManager.instance.userData.gold = _meAddBetData.myGOLD;
		    DataManager.instance.userData.SetTotalBetInGameInfo(IMiniGameInfo.Type.AnimalRacing, _meAddBetData.totalBet);
            betManager.panelUserInGame.RefreshGoldInfo();
			betManager.panelListChip.SetFocusChipAgain();
        }
        // ----------------------- //
        yield break;
    }

    IEnumerator DoActionCheckPlayerAddBet(){
        AnimalRacingData.AnimalRacing_PlayerAddBet_Data _playerAddBetData = animalRacingData.processPlayerAddBet[0];
        System.Action _onFinished = ()=>{
			_playerAddBetData = null;
			animalRacingData.processPlayerAddBet.RemoveAt(0);
		};

        if(currentState == State.Bet){
            yield return StartCoroutine(DoActionPlayerAddBet(_playerAddBetData));
        }else{
            listProcessPlaying.Add(DoActionPlayerAddBet(_playerAddBetData));
        }

        if(_onFinished != null){
            _onFinished();
        }
        yield break;
    }

    IEnumerator DoActionPlayerAddBet(AnimalRacingData.AnimalRacing_PlayerAddBet_Data _playerAddBetData){
        // ---- Merge dữ liệu ---- //
        animalRacingData.listGlobalBets[_playerAddBetData.indexBet] = _playerAddBetData.globalBet;

        if(DataManager.instance.userData.sessionId == _playerAddBetData.sessionid){
            animalRacingData.listMyBets[_playerAddBetData.indexBet] = _playerAddBetData.playerBet;
            DataManager.instance.userData.gold = _playerAddBetData.myGOLD;
		    DataManager.instance.userData.SetTotalBetInGameInfo(IMiniGameInfo.Type.AnimalRacing, _playerAddBetData.totalBet);
        }else{
            int _tmpIndex = -1;
            for(int i = 0; i < animalRacingData.listOtherPlayerData.Count; i++){
                if(animalRacingData.listOtherPlayerData[i].sessionId == _playerAddBetData.sessionid){
                    _tmpIndex = i;
                    break;
                }
            }
            if(_tmpIndex != -1){
                animalRacingData.listOtherPlayerData[_tmpIndex].gold = _playerAddBetData.myGOLD;
                animalRacingData.listOtherPlayerData[_tmpIndex].SetTotalBetInGameInfo(IMiniGameInfo.Type.AnimalRacing, _playerAddBetData.totalBet);
            }
        }
        // ----------------------- //

        betManager.SetUpPlayerAddBet(_playerAddBetData.sessionid, _playerAddBetData.chipIndex, _playerAddBetData.indexBet, _playerAddBetData.myGOLD);
        betManager.RefreshUIPanelTableBet();
        yield break;
    }

    IEnumerator DoActionSetResultData(){
        animalRacingData.currentResultData = animalRacingData.processResultData[0];
        yield return new WaitUntil(()=> animalRacingData.currentResultData == null);
        animalRacingData.processResultData.RemoveAt(0);
    }

    ///<summary>
    /// DoActionShowResult: Show màn hình cho thú chạy
    ///</summary>
    IEnumerator DoActionShowResultScreen()
    {
        if (currentState == State.ShowResult)
        {
            Debug.LogError("Đang show result rồi");
            yield break;
        }

        currentState = State.ShowResult;

        bool _isFinished = false;
        shadowChangeScreen.gameObject.SetActive(true);
        Color _c = shadowChangeScreen.color;
        _c.a = 0f;
        shadowChangeScreen.color = _c;
        LeanTween.alpha(shadowChangeScreen.rectTransform, 1f, 0.1f).setOnComplete(()=>{
            _isFinished = true;
        });
        yield return new WaitUntil(()=>_isFinished);
        if (callbackManager != null && callbackManager.onStartShowResult != null){
            callbackManager.onStartShowResult();
        }
        yield return Yielders.EndOfFrame;
        betManager.Hide();
        resultManager.gameObject.SetActive(true);
        yield return Yielders.EndOfFrame;

        resultManager.Show();

        yield return Yielders.Get(0.3f);

        _isFinished = false;
        LeanTween.alpha(shadowChangeScreen.rectTransform, 0f, 0.2f).setOnComplete(()=>{
            shadowChangeScreen.gameObject.SetActive(false);
            _isFinished = true;
        });
        yield return new WaitUntil(()=>_isFinished);
        
        if (animalRacingData.currentResultData == null){
            yield return new WaitUntil(() => animalRacingData.currentResultData != null);
        }

        // ---- Merge dữ liệu ---- //
        #if TEST
        Debug.Log(">>> AnimalIndexWin: " + animalRacingData.currentResultData.animalWin);
        #endif
        if(animalRacingData.currentResultData.valueCheck == 1){
            for(int i = 0; i < animalRacingData.currentResultData.listGoldUpdate.Count; i++){
                if(animalRacingData.currentResultData.listGoldUpdate[i].caseCheck == 1){
                    // --- Cập nhật số tiền hiện có cho chính mình --- //
                    if(animalRacingData.currentResultData.listGoldUpdate[i].sessionId == DataManager.instance.userData.sessionId){
                        DataManager.instance.userData.gold = animalRacingData.currentResultData.listGoldUpdate[i].GOLD;
                        AchievementDetail _achievementDetail = DataManager.instance.achievementData.GetAchievementDetail(IMiniGameInfo.Type.AnimalRacing);
                        if(_achievementDetail != null){
                            if(animalRacingData.currentResultData.listGoldUpdate[i].goldProcess > 0){
                                _achievementDetail.countWin = animalRacingData.currentResultData.listGoldUpdate[i].achievement;
                            }else if(animalRacingData.currentResultData.listGoldUpdate[i].goldProcess < 0){
                                _achievementDetail.countLose = animalRacingData.currentResultData.listGoldUpdate[i].achievement;
                            }else{
                                _achievementDetail.countDraw = animalRacingData.currentResultData.listGoldUpdate[i].achievement;
                            }
                        }else{
                            Debug.LogError(">>> _achievementDetail is null");
                        }
                        continue;
                    }
                    for(int j = 0; j < animalRacingData.listOtherPlayerData.Count; j++){
                        if(animalRacingData.listOtherPlayerData[j].IsEqual(animalRacingData.currentResultData.listGoldUpdate[i].sessionId)){
                            animalRacingData.listOtherPlayerData[j].gold = animalRacingData.currentResultData.listGoldUpdate[i].GOLD;
                            if(animalRacingData.currentResultData.listGoldUpdate[i].goldProcess > 0){
                                animalRacingData.listOtherPlayerData[j].win = animalRacingData.currentResultData.listGoldUpdate[i].achievement;
                            }else if(animalRacingData.currentResultData.listGoldUpdate[i].goldProcess < 0){
                                animalRacingData.listOtherPlayerData[j].lose = animalRacingData.currentResultData.listGoldUpdate[i].achievement;
                            }else{
                                animalRacingData.listOtherPlayerData[j].tie = animalRacingData.currentResultData.listGoldUpdate[i].achievement;
                            }
                            break;
                        }
                    }
                }else if(animalRacingData.currentResultData.listGoldUpdate[i].caseCheck == -88){
                    if(animalRacingData.currentResultData.listGoldUpdate[i].sessionId == DataManager.instance.userData.sessionId){
                        DataManager.instance.userData.gold = animalRacingData.currentResultData.listGoldUpdate[i].GOLD;
                        continue;
                    }
                    for(int j = 0; j < animalRacingData.listOtherPlayerData.Count; j++){
                        if(animalRacingData.listOtherPlayerData[j].IsEqual(animalRacingData.currentResultData.listGoldUpdate[i].sessionId)){
                            animalRacingData.listOtherPlayerData[j].gold = animalRacingData.currentResultData.listGoldUpdate[i].GOLD;
                            break;
                        }
                    }
                }
            }
        }else{
            if(animalRacingData.currentResultData.valueCheck == -99){
                PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
                    , MyLocalize.GetString(MyLocalize.kConnectionError)
                    , string.Empty
                    , MyLocalize.GetString(MyLocalize.kOk));
            }
        }

        // --- Cập nhật lại history và tỉ lệ cược mới --- //
        AnimalRacingData.HistoryData _tmpHistoryData = new AnimalRacingData.HistoryData();
        _tmpHistoryData.animalType = animalRacingData.currentResultData.animalWin;
        _tmpHistoryData.score = animalRacingData.listCurrentScore[(int) _tmpHistoryData.animalType];
        animalRacingData.listHistoryData.Insert(0, _tmpHistoryData);
        animalRacingData.CheckListHistoryAgain();
        
        animalRacingData.listCurrentScore = animalRacingData.currentResultData.newListCurrentScore;
        animalRacingData.listGlobalBets = animalRacingData.currentResultData.newListGlobalBet;
        animalRacingData.listMyBets = animalRacingData.currentResultData.newListMyBet;
        // ----------------------------------------------- //

        callbackManager.onStartSetUpAfterShowResult = betManager.SetUpAfterShowResult;
        callbackManager.onEndSetUpAfterShowResult = () => {
            animalRacingData.currentResultData = null;
            currentState = State.Bet;
        };
        // ----------------------- //

        yield return null;

// #if TEST
//         Debug.Log(">>> ShowResultScreen");
// #endif
        
        yield return resultManager.StartRun();

        _isFinished = false;
        shadowChangeScreen.gameObject.SetActive(true);
        _c = shadowChangeScreen.color;
        _c.a = 0f;
        shadowChangeScreen.color = _c;
        LeanTween.alpha(shadowChangeScreen.rectTransform, 1f, 0.1f).setOnComplete(()=>{
            resultManager.Hide();
            animalRacingData.nextTimeToShowResult = animalRacingData.currentResultData.nextTimeToShowResult;
            if (callbackManager != null
                && callbackManager.onEndShowResult != null){
                callbackManager.onEndShowResult();
            }
            ShowBetScreenAgain();

            LeanTween.alpha(shadowChangeScreen.rectTransform, 0f, 0.1f).setOnComplete(()=>{
                shadowChangeScreen.gameObject.SetActive(false);
                _isFinished = true;
            });
        });
        yield return new WaitUntil(()=>_isFinished);
    }

    void ShowBetScreenAgain()
    {
        if (betManager.myCurrentState == AnimalRacing_Bet_Manager.State.Show)
        {
            Debug.LogError("Đang show bet screen rồi");
            return;
        }
// #if TEST
//         Debug.Log(">>> ShowBetScreen");
// #endif
        
        betManager.Show();
        StartCountDown();
    }

    void StartCountDown()
    {
        System.TimeSpan _tmpDeltaTime = animalRacingData.nextTimeToShowResult - System.DateTime.Now;
        double _timeCoundown = _tmpDeltaTime.TotalSeconds - 1.5f; 
        betManager.clock.StartCountDown(_timeCoundown, () =>
        {
            StartCoroutine(DoActionShowResultScreen());
        });
    }

    public void ShowArrowFocusGetGold(){
        if(arrowFocusGetGold.myState == MyArrowFocusController.State.Hide){
			if(this.CanPlayMusicAndSfx()){
				MyAudioManager.instance.PlaySfx(myAudioInfo.sfx_Notification);
			}
		}
		arrowFocusGetGold.Show();
	}

	public void HideArrowFocusGetGold(){
		arrowFocusGetGold.Hide();
	}
    #endregion

    #region On Button Clicked
    public void OnButtonSettingClicked(){
        if(this.CanPlayMusicAndSfx()){
            MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
        }
        SettingScreenController.instance.InitData();
        SettingScreenController.instance.Show();
        SettingScreenController.instance.LateInitData();
        SettingScreenController.instance.btnOutRoom.onClick.AddListener(OnButtonOutRoomClicked);
    }
    
    public void OnButtonOutRoomClicked()
    {
        if(this.CanPlayMusicAndSfx()){
            MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
        }
        if(callbackManager.onDestructAllObject != null){
            callbackManager.onDestructAllObject();
        }
        DataManager.instance.userData.RemoveTotalBetInGameInfo(IMiniGameInfo.Type.AnimalRacing);
        CoreGameManager.instance.SetUpOutRoomAndBackToChooseTableScreen();
    }
    public void OnButtonChatClicked()
    {
        if(this.CanPlayMusicAndSfx()){
            MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
        }

        screenChat.Show();
    }
    public void OnButtonShopClicked()
    {
        if(this.CanPlayMusicAndSfx()){
            MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
        }

        GetGoldScreenController.instance.InitData();
        GetGoldScreenController.instance.Show();
        GetGoldScreenController.instance.LateInitData();
    }
    public void OnButtonMiniGamesClicked()
    {
        if(this.CanPlayMusicAndSfx()){
            MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
        }

        ChooseSubGameScreenController.instance.InitData();
        ChooseSubGameScreenController.instance.Show();
        ChooseSubGameScreenController.instance.LateInitData();
    }
    #endregion

    public override void RefreshAgainWhenCloseSubGamePlay(){
        if(CoreGameManager.instance.currentSubGamePlay != null){
			CoreGameManager.instance.currentSubGamePlay = null;
		}
        
        RegisterActionPlayerAddGold();
        RegisterActionSetParentInfo();
        RegisterActionAlertUpdateServer();

        if(currentState == State.Bet){
            MyAudioManager.instance.PlayMusic(myAudioInfo.bgm);
        }else{
            MyAudioManager.instance.SetMusic(myAudioInfo.bgm);
        }

		betManager.panelUserInGame.RefreshGoldInfo(true);
        betManager.panelListChip.SetFocusChipAgain();
	}

    private void OnDestroy()
    {
        StopAllCoroutines();
        DataManager.instance.userData.RemoveTotalBetInGameInfo(IMiniGameInfo.Type.AnimalRacing);
        animalRacingData = null;
        AnimalRacing_RealTimeAPI.SelfDestruction();
        instance = null;
    }
}

public class AnimalRacing_CallbackManager
{
    public System.Action onStartSetUpAfterShowResult;
    public System.Action onEndSetUpAfterShowResult;
    public System.Action onStartShowResult;
    public System.Action onEndShowResult;
    public System.Action onDestructAllObject;
}

[System.Serializable]
public class AnimalRacing_AnimalInfo
{
    public AnimalRacing_AnimalController.AnimalType animalType;
    public Sprite mySprite;
    public AnimalRacing_AnimalController myController;
}

[System.Serializable]
public class AnimalRacing_SortingLayerManager
{
   public MySortingLayerInfo sortingLayerInfo_ChipObject;
   public MySortingLayerInfo sortingLayerInfo_GoldObject;
}

[System.Serializable] public class AnimalRacing_AudioInfo
{
    [Header("Playback")]
    public AudioClip bgm;
    [Header("Sfx")]
    public AudioClip sfx_Run;
    public AudioClip sfx_RunPlayBack;
    public AudioClip sfx_BeepCountDown_00;
    public AudioClip sfx_BeepCountDown_01;
    public AudioClip sfx_Bet;
    public AudioClip sfx_HighlightAnimalWin;
    public AudioClip sfx_HighlightPanelWin;
    public AudioClip sfx_Notification;
    public AudioClip sfx_PopupChat;
}