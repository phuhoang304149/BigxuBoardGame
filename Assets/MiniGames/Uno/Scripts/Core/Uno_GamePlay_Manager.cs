using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class Uno_GamePlay_Manager : IMySceneManager {

	public static Uno_GamePlay_Manager instance;

	public override Type mySceneType{
		get{
			return Type.Uno;
		}
	}
	
	[Header("Manager")]
	public Canvas myCanvas;
	public Uno_UIManager UIManager;
	public ScreenChatController screenChat{get;set;}
	public PopupChatManager popupChatManager{get;set;}
	public GameObject iconNotificationChat;

	[Header("Data")]
	public UnoGamePlayData unoGamePlayData;
	public List<CardUnoDetail> listCardDetail;
	public List<Uno_PlayerGroup> listPlayerGroup;
	public Uno_CallbackManager callbackManager;

	
	[Header("Audio Info")]
    public Uno_AudioInfo myAudioInfo;

	[Header("Prefabs")]
	[SerializeField] GameObject screenChatPrefab;
	[SerializeField] GameObject popupChatManagerPrefab;

	public Uno_PlayerGroup myPlayerGroup{get;set;}

	IEnumerator actionRunProcessPlaying, actionRunProcessNonPlaying, actionCheckFocusIconGetGold;
	IEnumerator actionCallBot;
	List<IEnumerator> listProcessPlaying;
	List<IEnumerator> listProcessNonPlaying;

	public System.Action onPressBack;
	
	private void Awake() {
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
						CoreGameManager.instance.SetUpOutRoomAndBackToChooseTableScreen();
					});
			};
			DataManager.instance.userData.sessionId = NetworkGlobal.instance.instanceRealTime.sessionId;
		}
		
        StartCoroutine(DoActionRun());
    }

	IEnumerator DoActionRun(){
		yield return Yielders.EndOfFrame;
		InitData();

		actionRunProcessPlaying = DoActionRunProcessPlaying();
		StartCoroutine(actionRunProcessPlaying);

		actionRunProcessNonPlaying = DoActionRunProcessNonPlaying();
		StartCoroutine(actionRunProcessNonPlaying);

		actionCheckFocusIconGetGold = DoActionCheckFocusIconGetGold();
		StartCoroutine(actionCheckFocusIconGetGold);

		actionCallBot = DoActionAutoCallBot();
		StartCoroutine(actionCallBot);
		
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_GET_TABLE_INFO, (_mess)=>{
			unoGamePlayData.InitDataWhenGetTableInfo(_mess);
			if(_mess.avaiable() > 0){
				#if TEST
				Debug.Log (">>> Chua doc het CMD : " + _mess.getCMDName ());
				#endif
			}
			UIManager.RefreshAllUINow();

			RegisterActionPlayerJoinGame();
            RegisterActionPlayerLeftGame();
			RegisterActionPlayerSitDown();
			RegisterActionPlayerStandUp();
			RegisterActionMeSitDown();

			RegisterActionSetParentInfo();

			RegisterActionPlayerAddGold();
			RegisterActionPlayerChat();
			RegisterActionAlertUpdateServer();

			RegisterActionGamePlayReady();
			RegisterActionGamePlayStartGame();

			RegisterActionPlayerPutCard();
			RegisterActionMePutCardFail();

			RegisterActionChangeTurn();

			RegisterActionOtherPlayerGetCard();
			RegisterActionMeGetCard();

			RegisterActionPlayerCallUno();
			RegisterActionMeCallUnoFail();

			RegisterActionPlayerAtkUno();
			RegisterActionMeAtkUnoFail();

			RegisterActionOtherPlayerAtkUnoMe();

			RegisterActionFinishGame();
		});
		NetworkGlobal.instance.instanceRealTime.ResumeReceiveMessage();
		yield return new WaitUntil(() => unoGamePlayData != null && unoGamePlayData.hasLoadTableInfo);
		yield return Yielders.Get(0.5f);

		canShowScene = true;

		MyAudioManager.instance.PlayMusic(myAudioInfo.bgm);

		if(unoGamePlayData.currentGameState == UnoGamePlayData.GameState.STATUS_WAIT_FOR_PLAYER){
			if(unoGamePlayData.nextTimeToStartGame > System.DateTime.Now){ // đang countdown start game
				System.TimeSpan _tmpDelta = unoGamePlayData.nextTimeToStartGame - System.DateTime.Now;
				UIManager.ShowCountDownStartGame(_tmpDelta.TotalSeconds);
			}
		}else if(unoGamePlayData.currentGameState == UnoGamePlayData.GameState.STATUS_FINISHGAME){
			#if TEST
			Debug.LogError(">>> Không setup cho trạng thái này");
			#endif
		}else{ // trường hợp đang chơi dở dang thì cho count down tại thằng đang đánh
			System.TimeSpan _tmpDelta = unoGamePlayData.nextTimeToChangeCircle - System.DateTime.Now;
			double _timeCountDown = _tmpDelta.TotalSeconds;
			UnoGamePlayData.Uno_PlayerPlayingData _currentPlayerPlaying = unoGamePlayData.listPlayerPlayingData[unoGamePlayData.currentCircle];
			Uno_PlayerGroup _currentPlayerGroup = listPlayerGroup[_currentPlayerPlaying.indexChair];
			if(_currentPlayerGroup != null && _currentPlayerGroup.isInitialized){
				_currentPlayerGroup.panelPlayerInfo.StartCountDown(_timeCountDown, _timeCountDown);
				callbackManager.onChangeNewTurn += _currentPlayerGroup.panelPlayerInfo.StopCountDown;
			}
			System.TimeSpan _tmpDeltaPlayGame = unoGamePlayData.nextTimeToStopGame - System.DateTime.Now;
			UIManager.ShowCountDownStopGame(_tmpDeltaPlayGame.TotalSeconds);
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

	void InitData(){
		unoGamePlayData = new UnoGamePlayData();
		listProcessPlaying = new List<IEnumerator>();
		listProcessNonPlaying = new List<IEnumerator>();
		callbackManager = new Uno_CallbackManager();

		screenChat = ((GameObject) Instantiate(screenChatPrefab, transform)).GetComponent<ScreenChatController>();
		popupChatManager = ((GameObject) Instantiate(popupChatManagerPrefab, transform)).GetComponent<PopupChatManager>();

		HideIconNotificationChat();

		UIManager.InitFirst();
		InitAllCallback();
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
	void InitAllCallback(){
		callbackManager.onDestructAllObject = ()=>{
			UIManager.goldObjectPoolManager.ClearAllObjectsNow();
			UIManager.cardsGlobalPoolManager.ClearAllObjectsNow();
			for(int i = 0; i < listPlayerGroup.Count; i++){
				if(listPlayerGroup[i].isInitialized){
					listPlayerGroup[i].ClearAllCards();
				}
			}
			UIManager.effectPoolManager.ClearAllObjectsNow();
			UIManager.panelScoreBoardFinishGame.scoreBoardOptionPoolManager.ClearAllObjectsNow();
			UIManager.panelHistory.historyOptionPoolManager.ClearAllObjectsNow();
		};

		screenChat.onSendMessage = (_mess) =>
        {
            Uno_RealTimeAPI.instance.SendMessageChat(_mess);
        };
        screenChat.onStartShow += HideIconNotificationChat;
		screenChat.onHasNewMessage += ShowIconNotificationChat;
	}
	void RegisterActionAlertUpdateServer()
    {
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_ALERT_UPDATE_SERVER, (_mess) =>
        {
            if(unoGamePlayData != null){
                MyGamePlayData.AlertUpdateServer_Data _data = new MyGamePlayData.AlertUpdateServer_Data(_mess);
                System.TimeSpan _timeSpanRemain = _data.timeToUpdateServer - System.DateTime.Now;                
                PopupManager.Instance.CreateToast(string.Format(MyLocalize.GetString("System/Message_ServerMaintenance"), _timeSpanRemain.Minutes, _timeSpanRemain.Seconds));
            }
        });
    }
	void RegisterActionPlayerJoinGame(){
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_PLAYER_JOIN_GAME, (_mess) =>
        {
			if(unoGamePlayData != null){
				unoGamePlayData.SetUpUserJoinGame(_mess);
				listProcessNonPlaying.Add(DoActionCheckPlayerJoinGame());
			}
        });
    }
    void RegisterActionPlayerLeftGame(){
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_PLAYER_LEFT_GAME, (_mess) =>
        {
			if(unoGamePlayData != null){
				unoGamePlayData.SetUpUserLeftGame(_mess);
				listProcessNonPlaying.Add(DoActionCheckPlayerLeftGame());
			}
        });
    }
	void RegisterActionPlayerSitDown(){
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_PLAYER_SITDOWN, (_mess) =>
        {
			if(unoGamePlayData != null){
				unoGamePlayData.SetUpPlayerSitDown(_mess);
				listProcessNonPlaying.Add(DoActionCheckPlayerSitDown());
			}
        });
	}
	void RegisterActionPlayerStandUp(){
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_PLAYER_STANDUP, (_mess) =>
        {
			if(unoGamePlayData != null){
				unoGamePlayData.SetUpPlayerStandUp(_mess);
				listProcessNonPlaying.Add(DoActionCheckPlayerStandUp());
			}
        });
	}
	void RegisterActionMeSitDown(){
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_SITDOWN, (_mess) =>
        {	
			if(unoGamePlayData != null){
				unoGamePlayData.SetUpMeSitDownFail(_mess);
				listProcessNonPlaying.Add(DoActionMeSitDownFail());
			}
		});
	}

	void RegisterActionPlayerAddGold()
    {
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_onPlayerAddGold, (_mess) =>
        {
			if(unoGamePlayData != null){
				unoGamePlayData.SetPlayerAddGoldData(_mess);
				listProcessNonPlaying.Add(DoActionPlayerAddGold());
			}
        });
    }
	void RegisterActionPlayerChat(){
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_CHAT_IN_TABLE, (_mess) =>
        {   
			if(unoGamePlayData != null){
				unoGamePlayData.SetPlayerChatData(_mess);
				listProcessNonPlaying.Add(DoActionPlayerChat());
			}
        });
    }
	void RegisterActionGamePlayReady(){
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_READY, (_mess) =>
        {   
			if(unoGamePlayData != null){
				unoGamePlayData.SetDataWhenReady(_mess);
				listProcessPlaying.Add(DoActionGameReady());
			}
		});
	}

	void RegisterActionGamePlayStartGame(){
		// - Khi nhận được cmt này tức là start game đã trừ tiền các người chơi
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_START_GAME, (_mess) => 
		{
			if(unoGamePlayData != null){
				unoGamePlayData.SetDataWhenStartGame(_mess);
				listProcessPlaying.Add(DoActionStartNewGame());
			}
		});
	}

	void RegisterActionPlayerPutCard(){
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_PLAYER_PUT_CARD, (_mess) => 
		{
			if(unoGamePlayData != null){
				unoGamePlayData.SetDataWhenPlayerPutCard(_mess);
				listProcessPlaying.Add(DoActionPlayerPutCard());
			}
		});
	}

	void RegisterActionMePutCardFail(){
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_PUT_CARD, (_mess) => 
		{
			if(unoGamePlayData != null){
				if(unoGamePlayData.currentGameState == UnoGamePlayData.GameState.STATUS_PLAYING){
					sbyte _caseCheck = _mess.readByte();
					#if TEST
					Debug.LogError("Lỗi MePutCard : " + _caseCheck);
					#endif
				}else{
					#if TEST 
					Debug.LogError(">>> Bug Logic RegisterActionMePutCardFail (0): " + unoGamePlayData.currentGameState.ToString());
					#endif
				}
			}
		});
	}

	void RegisterActionChangeTurn(){
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_SERVER_CHANGE_TURN, (_mess) => 
		{
			if(unoGamePlayData != null){
				unoGamePlayData.SetDataWhenPlayerChangeTurn(_mess);
				listProcessPlaying.Add(DoActionPlayerChangeTurn(false));
			}
		});
	}

	void RegisterActionOtherPlayerGetCard(){
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_PLAYER_GET_CARD, (_mess) => 
		{
			if(unoGamePlayData != null){
				unoGamePlayData.SetDataWhenOtherPlayerGetCard(_mess);
				listProcessPlaying.Add(DoActionOtherPlayerGetCard());
			}
		});
	}

	void RegisterActionMeGetCard(){
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_GET_CARD, (_mess) => 
		{
			if(unoGamePlayData != null){
				unoGamePlayData.SetDataWhenMeGetCard(_mess);
				listProcessPlaying.Add(DoActionMeGetCard());
			}
		});
	}

	void RegisterActionPlayerCallUno(){
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_PLAYER_CALL_WIN, (_mess) => 
		{	
			if(unoGamePlayData != null){
				unoGamePlayData.SetDataWhenPlayerCallUno(_mess);
				listProcessPlaying.Add(DoActionPlayerCallUno());
			}
		});
	}

	void RegisterActionMeCallUnoFail(){
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_CALL_WIN, (_mess) => 
		{
			sbyte _caseCheck = _mess.readByte();
			#if TEST
			Debug.Log(">>> Me Call Uno Fail: " + _caseCheck);
			#endif
		});
	}

	void RegisterActionPlayerAtkUno(){
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_PLAYER_ATTACK_WIN, (_mess) => 
		{
			if(unoGamePlayData != null){
				unoGamePlayData.SetDataWhenPlayerAtkUno(_mess);
				listProcessPlaying.Add(DoActionPlayerAtkUno());
			}
		});
	}

	void RegisterActionMeAtkUnoFail(){
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_ATTACK_WIN, (_mess) => 
		{
			bool _isSuccess = _mess.readBoolean();
			#if TEST
			Debug.Log(">>> Me Atk Uno Fail: " + _isSuccess);
			#endif
		});
	}

	void RegisterActionOtherPlayerAtkUnoMe(){
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_RECEIVE_ATTACK_WIN, (_mess) => 
		{
			if(unoGamePlayData != null){
				unoGamePlayData.SetDataWhenOtherPlayerAtkUnoMe(_mess);
				listProcessPlaying.Add(DoActionOtherPlayerAtkUnoMe());
			}
		});
	}

	void RegisterActionFinishGame(){
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_FINISH_GAME, (_mess) => 
		{
			if(unoGamePlayData != null){
				unoGamePlayData.SetDataWhenFinishGame(_mess);
				listProcessPlaying.Add(DoActionFinishGame());
			}
		});
	}

	void RegisterActionSetParentInfo(){
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_SET_PARENT, (_mess) => 
		{
			if(unoGamePlayData != null){
				unoGamePlayData.SetDataWhenSetParent(_mess);
				listProcessNonPlaying.Add(DoActionPlayerSetParent());
			}
		});
	}
	#endregion

	#region Logic
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
			yield return StartCoroutine(listProcessNonPlaying[0]);
			listProcessNonPlaying.RemoveAt(0);
		}
	}

	IEnumerator DoActionCheckFocusIconGetGold(){
		while(true){
			if(!canShowScene){
				yield return null;
				continue;
			}
			if(DataManager.instance.userData.gold < unoGamePlayData.betDefault){
				UIManager.ShowArrowFocusGetGold();
			}else{
				UIManager.HideArrowFocusGetGold();
			}
			yield return Yielders.Get(1f);
		}
	}

	IEnumerator DoActionAutoCallBot(){
		while(true){
			if(!canShowScene){
				yield return null;
				continue;
			}
			if(unoGamePlayData.currentGameState != UnoGamePlayData.GameState.STATUS_WAIT_FOR_PLAYER
				|| DataManager.instance.miniGameData.currentMiniGameDetail.tableData.currentTableDetail.isLockByPass){
				yield return Yielders.Get(1f);
				continue;
			}

			if(unoGamePlayData.GetTotalPlayerInGame() == 1){

				yield return Yielders.Get(3f);

				if(unoGamePlayData.currentGameState != UnoGamePlayData.GameState.STATUS_WAIT_FOR_PLAYER
					|| DataManager.instance.miniGameData.currentMiniGameDetail.tableData.currentTableDetail.isLockByPass){
					yield return Yielders.Get(1f);
					continue;
				}
				
				int _numBot = 0;
				int _numPlayerInChair = unoGamePlayData.GetTotalRealPlayerOnChair();

				if(_numPlayerInChair == 0){
					_numBot = 1;
				}else if(_numPlayerInChair == 1){
					_numBot = Random.Range(1, 4);
				}else{
					// #if TEST
					// Debug.LogError(">>> BUG Logic (0)");
					// #endif
				}
				if(_numBot > 0){
					while(unoGamePlayData.GetTotalPlayerInGame() == 1){
						#if TEST
						Debug.Log(">>> Call " +_numBot+" bot: ");
						#endif
						GlobalRealTimeSendingAPI.SendMessageCallBotUno((byte)_numBot);
						yield return Yielders.Get(Random.Range(4f, 6f));
						if(unoGamePlayData.currentGameState != UnoGamePlayData.GameState.STATUS_WAIT_FOR_PLAYER
							|| DataManager.instance.miniGameData.currentMiniGameDetail.tableData.currentTableDetail.isLockByPass){
							break;
						}
					}
				}
			}else{
				yield return Yielders.Get(1f);
			}
		}
	}

	IEnumerator DoActionCheckPlayerJoinGame(){
		UnoGamePlayData.PlayerJoinGame_Data _playerJoinGameData = unoGamePlayData.processPlayerJoinGame[0];
		System.Action _onFinished = ()=>{
			_playerJoinGameData = null;
			unoGamePlayData.processPlayerJoinGame.RemoveAt(0);
		};

		if(unoGamePlayData.listGlobalPlayerData[_playerJoinGameData.viewerId].sessionId >= 0){
            #if TEST
            Debug.LogError(">>> Chỗ này đã có người rồi: " + _playerJoinGameData.viewerId);
            #endif
			if(_onFinished != null){
				_onFinished();
			}
			yield break;
        }
        if(_playerJoinGameData.userData.sessionId != DataManager.instance.userData.sessionId){
            unoGamePlayData.listGlobalPlayerData[_playerJoinGameData.viewerId] = _playerJoinGameData.userData;
			unoGamePlayData.listSessionIdGlobalPlayer[_playerJoinGameData.viewerId] = _playerJoinGameData.sessionId;
            #if TEST
            Debug.Log(">>> Có người chơi " + unoGamePlayData.listGlobalPlayerData[_playerJoinGameData.viewerId].nameShowInGame + " vào game tại vị trí " + _playerJoinGameData.viewerId);
            #endif
        }else{
			#if TEST
            Debug.LogError(">>> Trả session ID tào lao: " + _playerJoinGameData.sessionId);
            #endif
		}

		if(_onFinished != null){
			_onFinished();
		}
	}

	IEnumerator DoActionCheckPlayerLeftGame(){
		UnoGamePlayData.PlayerLeftGame_Data _playerLeftGameData = unoGamePlayData.processPlayerLeftGame[0];
		System.Action _onFinished = ()=>{
			_playerLeftGameData = null;
			unoGamePlayData.processPlayerLeftGame.RemoveAt(0);
		};

		if(_playerLeftGameData.sessionId == DataManager.instance.userData.sessionId){
			#if TEST
            Debug.LogError(">>> Trả session ID tào lao: " + _playerLeftGameData.sessionId);
            #endif
			if(_onFinished != null){
				_onFinished();
			}
			yield break;
		}
		int _index = unoGamePlayData.listSessionIdGlobalPlayer.IndexOf(_playerLeftGameData.sessionId);
		if(_index < 0){
			#if TEST
			Debug.LogError(">>> Không tìm thấy session ID: " + _playerLeftGameData.sessionId);
			#endif
			if(_onFinished != null){
				_onFinished();
			}
			yield break;
		}

		#if TEST
		Debug.Log(">>> Có người chơi " + unoGamePlayData.listGlobalPlayerData[_index].nameShowInGame + " thoát game tại vị trí " + _index);
		#endif
		unoGamePlayData.listGlobalPlayerData[_index] = new UserDataInGame();
		unoGamePlayData.listSessionIdGlobalPlayer[_index] = -1;

		if(_onFinished != null){
			_onFinished();
		}
	}

	IEnumerator DoActionCheckPlayerSitDown(){
		UnoGamePlayData.PlayerSitDown_Data _playerSitDownData = unoGamePlayData.processPlayerSitDown[0];
		System.Action _onFinished = ()=>{
			_playerSitDownData = null;
			unoGamePlayData.processPlayerSitDown.RemoveAt(0);
		};

		// ------- Check Logic ------- //
		if(_playerSitDownData.sessionId < 0){
			#if TEST
			Debug.LogError(">>> sessionId nhảm : " + _playerSitDownData.sessionId);
			#endif
			if(_onFinished != null){
				_onFinished();
			}
			yield break;
		}

		if(unoGamePlayData.listSessionIdGlobalPlayer == null || unoGamePlayData.listSessionIdGlobalPlayer.Count == 0){
			#if TEST
            Debug.LogError(">>> listSessionIdGlobalPlayer is NULL");
            #endif
			if(_onFinished != null){
				_onFinished();
			}
			yield break;
		}

		if(unoGamePlayData.listSessionIdOnChair == null || unoGamePlayData.listSessionIdOnChair.Count == 0){
			#if TEST
			Debug.LogError(">>> listSessionIdOnChair is NULL");
			#endif
			if(_onFinished != null){
				_onFinished();
			}
			yield break;
		}
		if(_playerSitDownData.indexChair < 0 || _playerSitDownData.indexChair >= unoGamePlayData.listSessionIdOnChair.Count){
			#if TEST
			Debug.LogError(">>> _indexChair out off range: " + _playerSitDownData.indexChair);
			#endif
			if(_onFinished != null){
				_onFinished();
			}
			yield break;
		}

		if(!unoGamePlayData.listSessionIdGlobalPlayer.Contains(_playerSitDownData.sessionId)){
			#if TEST
            Debug.LogError(">>> Không tìm thấy (0): " + _playerSitDownData.sessionId);
            #endif
			if(_onFinished != null){
				_onFinished();
			}
			yield break;
		}

		if(unoGamePlayData.listSessionIdOnChair[_playerSitDownData.indexChair] >= 0){
			#if TEST
			Debug.LogError(">>> Có người đang ĐỢI ngồi ngay tại vị trí " + _playerSitDownData.indexChair);
			#endif
			if(_onFinished != null){
				_onFinished();
			}
			yield break;
		}

		unoGamePlayData.listSessionIdOnChair[_playerSitDownData.indexChair] = _playerSitDownData.sessionId;
		
		for(int i = 0; i < unoGamePlayData.listPlayerPlayingData.Count; i++){
			if(unoGamePlayData.listPlayerPlayingData[i].indexChair == _playerSitDownData.indexChair){
				if(unoGamePlayData.currentGameState == UnoGamePlayData.GameState.STATUS_FINISHGAME){
					listProcessPlaying.Add(DoActionPlayerSitDown(_playerSitDownData));
				}else{
					#if TEST
					Debug.LogError(">>> Có người đang CHƠI ngồi ngay tại vị trí " +_playerSitDownData.indexChair);
					#endif
				}
				if(_onFinished != null){
					_onFinished();
				}
				yield break;
			}
		}
		// ------------------------ //

		yield return StartCoroutine(DoActionPlayerSitDown(_playerSitDownData));
		
		if(_onFinished != null){
			_onFinished();
		}
	}

	IEnumerator DoActionPlayerSitDown(UnoGamePlayData.PlayerSitDown_Data _playerSitDownData){
		UserDataInGame _userData = unoGamePlayData.GetUserDataInGameFromListGlobal(_playerSitDownData.sessionId);
		if(_userData == null){
			#if TEST
			Debug.LogError(">>> Không tìm dc userdata in listGlobal: " + _playerSitDownData.sessionId + " - " + _playerSitDownData.indexChair);
			#endif
			yield break;
		}

		#if TEST
		Debug.Log(">>> " + _userData.nameShowInGame + " ngồi vào ghế " + _playerSitDownData.indexChair);
		#endif
		if(unoGamePlayData.currentGameState == UnoGamePlayData.GameState.STATUS_WAIT_FOR_PLAYER){
			listPlayerGroup[_playerSitDownData.indexChair].InitData(_userData);
			LeanTween.scale(listPlayerGroup[_playerSitDownData.indexChair].panelPlayerInfo.gameObject, Vector3.one * UIManager.listPlaceHolderPanelPlayerInfo_Wating[listPlayerGroup[_playerSitDownData.indexChair].realIndex].ratioScale, 0.2f)
				.setEase(LeanTweenType.easeOutBack);
			if(unoGamePlayData.CheckIfIsMe(_userData.sessionId)){
				myPlayerGroup = listPlayerGroup[_playerSitDownData.indexChair];
				if(UIManager.isChangingPosPlayingOrWaiting){
					yield return new WaitUntil(()=>!UIManager.isChangingPosPlayingOrWaiting);
				}
				UIManager.ChangeView(myPlayerGroup);

				if(!UIManager.IsCountingDown()){
					if(unoGamePlayData.nextTimeToStartGame > System.DateTime.Now){ // đang countdown start game
						System.TimeSpan _tmpDelta = unoGamePlayData.nextTimeToStartGame - System.DateTime.Now;
						UIManager.ShowCountDownStartGame(_tmpDelta.TotalSeconds);
					}
				}
			}
		}else{
			listPlayerGroup[_playerSitDownData.indexChair].InitAsIncognito(_userData);
			LeanTween.scale(listPlayerGroup[_playerSitDownData.indexChair].panelPlayerInfo.gameObject, Vector3.one * UIManager.listPlaceHolderPanelPlayerInfo_Wating[listPlayerGroup[_playerSitDownData.indexChair].realIndex].ratioScale, 0.2f)
					.setEase(LeanTweenType.easeOutBack);
			if(unoGamePlayData.CheckIfIsMe(_userData.sessionId)){
				myPlayerGroup = listPlayerGroup[_playerSitDownData.indexChair];
				myPlayerGroup.ShowButtonStandUp(false);
			}
		}
		UIManager.RefreshUIButtonSitDown();
	}

	IEnumerator DoActionMeSitDownFail(){
		UnoGamePlayData.MeSitDownFail_Data _meSitDownFailData = unoGamePlayData.processMeSitDownFail[0];
		System.Action _onFinished = ()=>{
			_meSitDownFailData = null;
			unoGamePlayData.processMeSitDownFail.RemoveAt(0);
		};

		if(!_meSitDownFailData.isSuccess){
			unoGamePlayData.UpdateGoldAgain(DataManager.instance.userData.sessionId, _meSitDownFailData.myGold);

			UIManager.myPanelUserInfo.RefreshGoldInfo(true);

			#if TEST
			Debug.Log(">>> Sit Down false: " + _meSitDownFailData.chairId + "|" + _meSitDownFailData.currentChairId + "|" + _meSitDownFailData.totalBet + "|" + _meSitDownFailData.myGold);
			#endif
		}

		if(_onFinished != null){
			_onFinished();
		}
		
		yield break;
	}

	IEnumerator DoActionCheckPlayerStandUp(){
		UnoGamePlayData.PlayerStandUp_Data _playerStandUpData = unoGamePlayData.processPlayerStandUp[0];
		System.Action _onFinished = ()=>{
			_playerStandUpData = null;
			unoGamePlayData.processPlayerStandUp.RemoveAt(0);
		};
		
		if(_playerStandUpData.sessionId < 0){
			#if TEST
            Debug.LogError(">>> sessionId nhảm : " + _playerStandUpData.sessionId);
            #endif
			if(_onFinished != null){
				_onFinished();
			}
			yield break;
		}

		if(unoGamePlayData.listSessionIdGlobalPlayer == null || unoGamePlayData.listSessionIdGlobalPlayer.Count == 0){
			#if TEST
            Debug.LogError(">>> listSessionIdGlobalPlayer is NULL");
            #endif
			if(_onFinished != null){
				_onFinished();
			}
			yield break;
		}
		
		if(unoGamePlayData.listSessionIdOnChair == null || unoGamePlayData.listSessionIdOnChair.Count == 0){
			#if TEST
            Debug.LogError(">>> listSessionIdOnChair is NULL");
            #endif
			if(_onFinished != null){
				_onFinished();
			}
			yield break;
		}

		if(_playerStandUpData.indexChair < 0 || _playerStandUpData.indexChair >= unoGamePlayData.listSessionIdOnChair.Count){
			#if TEST
            Debug.LogError(">>> Không tìm thấy (0): " + _playerStandUpData.indexChair);
            #endif
			if(_onFinished != null){
				_onFinished();
			}
			yield break;
		}

		if(!unoGamePlayData.listSessionIdGlobalPlayer.Contains(_playerStandUpData.sessionId)){
			#if TEST
            Debug.LogError(">>> Không tìm thấy (1): " + _playerStandUpData.sessionId);
            #endif
			if(_onFinished != null){
				_onFinished();
			}
			yield break;
		}

		if(!unoGamePlayData.listSessionIdOnChair.Contains(_playerStandUpData.sessionId)){
			#if TEST
            Debug.LogError(">>> Không tìm thấy (2): " + _playerStandUpData.sessionId);
            #endif
			if(_onFinished != null){
				_onFinished();
			}
			yield break;
		}

		if(unoGamePlayData.listSessionIdOnChair[_playerStandUpData.indexChair] < 0){
			#if TEST
            Debug.LogError(">>> Không tìm thấy (3): " + _playerStandUpData.indexChair);
            #endif
			if(_onFinished != null){
				_onFinished();
			}
			yield break;
		}

		_playerStandUpData.isPlaying = false;
		int _tmpIndex = unoGamePlayData.listSessionIdPlaying.IndexOf(_playerStandUpData.sessionId);
		if(_tmpIndex >= 0 && unoGamePlayData.listSessionIdOnChair[unoGamePlayData.listPlayerPlayingData[_tmpIndex].indexChair] >= 0){ 
			_playerStandUpData.isPlaying = true;
		}
		unoGamePlayData.listSessionIdOnChair[_playerStandUpData.indexChair] = -1;

		if(unoGamePlayData.currentGameState == UnoGamePlayData.GameState.STATUS_FINISHGAME){
			if(_playerStandUpData.isPlaying){
				listProcessPlaying.Add(DoActionPlayerStandUp(_playerStandUpData));
				if(_onFinished != null){
					_onFinished();
				}
				yield break;
			}
		}

		yield return StartCoroutine(DoActionPlayerStandUp(_playerStandUpData));
		
		if(_onFinished != null){
			_onFinished();
		}
	}

	IEnumerator DoActionPlayerStandUp(UnoGamePlayData.PlayerStandUp_Data _playerStandUpData){
		// ------- Check Logic ------- //
		if(_playerStandUpData.isPlaying){
			if(unoGamePlayData.currentGameState == UnoGamePlayData.GameState.STATUS_WAIT_FOR_PLAYER){
				_playerStandUpData.isPlaying = false;
			}
		}		

		if(_playerStandUpData.isPlaying){			
			int _tmpIndex = unoGamePlayData.listSessionIdPlaying.IndexOf(_playerStandUpData.sessionId);
			if(_tmpIndex >= 0){ 
				UserDataInGame _userDataPlaying = unoGamePlayData.listPlayerPlayingData[_tmpIndex].userData;
				sbyte _indexChair = unoGamePlayData.listPlayerPlayingData[_tmpIndex].indexChair;
				if(_indexChair != _playerStandUpData.indexChair){
					#if TEST
					Debug.LogError(">>> Dữ liệu không đồng bộ giữa client và server: " + _userDataPlaying.nameShowInGame + " | " + _indexChair + " | " + _playerStandUpData.indexChair);
					#endif
					yield break;
				}
				#if TEST
				Debug.Log(">>> " + _userDataPlaying.nameShowInGame + " đang chơi và đứng dậy tại ghế " + _indexChair);
				#endif
			}else{
				#if TEST
				Debug.LogError(">>> Bug Logic DoActionPlayerStandUp: " + _playerStandUpData.sessionId + " - " + _playerStandUpData.indexChair);
				#endif
				yield break;
			}
		}else{
			UserDataInGame _userData = unoGamePlayData.GetUserDataInGameFromListGlobal(_playerStandUpData.sessionId);
			if(_userData == null){
				#if TEST
				Debug.LogError(">>> Không tìm dc userdata in listGlobal: " + _playerStandUpData.sessionId + " - " + _playerStandUpData.indexChair);
				#endif
			}else{
				#if TEST
				Debug.Log(">>> " + _userData.nameShowInGame + " đứng dậy tại ghế " + _playerStandUpData.indexChair);
				#endif
			}
		}
		// --------------------------- //

		// --- Xử lý --- //
		if(unoGamePlayData.currentGameState == UnoGamePlayData.GameState.STATUS_WAIT_FOR_PLAYER){
			listPlayerGroup[_playerStandUpData.indexChair].HideAndClear();
			LeanTween.scale(listPlayerGroup[_playerStandUpData.indexChair].panelPlayerInfo.gameObject, Vector3.one * 0.9f, 0.2f)
				.setEase(LeanTweenType.easeOutBack);
			int _tmpCount = 0;
			for(int i = 0; i < listPlayerGroup.Count; i++){
				if(listPlayerGroup[i].isInitialized){
					_tmpCount ++;
				}
			}
			if(_tmpCount < 2){
				UIManager.StopShowCountDownStartGame();
			}

			if(unoGamePlayData.CheckIfIsMe(_playerStandUpData.sessionId)){
				listPlayerGroup[_playerStandUpData.indexChair].HideButtonStandUp();
				myPlayerGroup = null;
				if(UIManager.isChangingPosPlayingOrWaiting){
					yield return new WaitUntil(()=>!UIManager.isChangingPosPlayingOrWaiting);
				}
				UIManager.ChangeView(null);
				UIManager.ShowMyPanelUserInfo();
			}
		}else{
			if(!_playerStandUpData.isPlaying){
				listPlayerGroup[_playerStandUpData.indexChair].HideAndClear();
				LeanTween.scale(listPlayerGroup[_playerStandUpData.indexChair].panelPlayerInfo.gameObject, Vector3.one * 0.9f, 0.2f)
					.setEase(LeanTweenType.easeOutBack);
				if(unoGamePlayData.CheckIfIsMe(_playerStandUpData.sessionId)){
					listPlayerGroup[_playerStandUpData.indexChair].HideButtonStandUp();
					myPlayerGroup = null;
					UIManager.ShowMyPanelUserInfo();
				}
			}else{
				// Nếu mình đang chơi mà đứng dậy thì 
				//	- Úp bài , bỏ focus, set default
				//	- Ẩn nút đứng dậy
				//	- Hiện panel UserInfo
				if(unoGamePlayData.CheckIfIsMe(_playerStandUpData.sessionId)){
					listProcessPlaying.Add(DoActionImPlayingAndStandUp());
					yield break;
				}
			}
		}
		UIManager.RefreshUIButtonSitDown();
	}

	IEnumerator DoActionGameReady(){
		if(unoGamePlayData.currentGameState != UnoGamePlayData.GameState.STATUS_WAIT_FOR_PLAYER){
			#if TEST
			Debug.LogError("Not in State STATUS_WAIT_FOR_PLAYER: " + unoGamePlayData.currentGameState.ToString());
			#endif
			unoGamePlayData.processGameReadyData.RemoveAt(0);
			yield break;
		}

		// #if TEST
		// Debug.Log("<color=green> Start process game ready</color>");
		// #endif

		// --- Merge vào dữ liệu thật --- //
		UnoGamePlayData.Uno_GameReady_Data _readyGameData = unoGamePlayData.processGameReadyData[0];
		unoGamePlayData.nextTimeToStartGame = _readyGameData.nextTimeToStartGame;
		// ------------------------------ //

		System.TimeSpan _tmpDelta = unoGamePlayData.nextTimeToStartGame - System.DateTime.Now;
		UIManager.ShowCountDownStartGame(_tmpDelta.TotalSeconds);

		_readyGameData = null;
		unoGamePlayData.processGameReadyData.RemoveAt(0);
		
		// #if TEST
		// Debug.Log("<color=green> End process game ready: " + unoGamePlayData.processGameReadyData.Count +"</color>");
		// #endif
	}

	IEnumerator DoActionStartNewGame(){
		if(unoGamePlayData.currentGameState != UnoGamePlayData.GameState.STATUS_WAIT_FOR_PLAYER){
			#if TEST
			Debug.LogError("Not in State STATUS_WAIT_FOR_PLAYER: " + unoGamePlayData.currentGameState.ToString());
			#endif
			unoGamePlayData.processStartGameData.RemoveAt(0);
			yield break;
		}

		//TODO: Start game
		// - Merge Data
		// - stop count down start game
		// - Vào vị trí
		// - Chia bài cho tất cả người chơi
		// - Hiện hiệu ứng đặt cược
		// - Refresh lại tiền cho người chơi
		// - Chuyển qua tuyến trình chơi game
		
		// #if TEST
		// Debug.Log("<color=green> Start process Start New Game</color>");
		// #endif

		// --- Merge vào dữ liệu thật --- //
		unoGamePlayData.currentGameState = UnoGamePlayData.GameState.STATUS_PLAYING;

		UnoGamePlayData.Uno_StartGame_Data _startGameData = unoGamePlayData.processStartGameData[0];
		for(int i = 0; i < _startGameData.listPlayerPlaying.Count; i++){
			sbyte _indexGlobal = (sbyte) unoGamePlayData.listSessionIdGlobalPlayer.IndexOf(_startGameData.listPlayerPlaying[i].userData.sessionId);
			if(_indexGlobal < 0){
				#if TEST
				Debug.Log(_startGameData.listPlayerPlaying[i].userData.nameShowInGame + " đã out room trước đó");
				#endif
			}
			_startGameData.listPlayerPlaying[i].userData.index = _indexGlobal;
			unoGamePlayData.UpdateGoldAgain(_startGameData.listPlayerPlaying[i].userData.sessionId, _startGameData.listPlayerPlaying[i].userData.gold);
		}
		unoGamePlayData.listPlayerPlayingData = _startGameData.listPlayerPlaying;
		unoGamePlayData.listSessionIdPlaying = _startGameData.listSessionIdPlaying;
		unoGamePlayData.totalBet = _startGameData.totalBet;
		unoGamePlayData.currentBet = _startGameData.currentBet;
		unoGamePlayData.turnDirection = _startGameData.turnDirection;
		unoGamePlayData.nextTimeToChangeCircle = _startGameData.nextTimeToChangeCircle;
		unoGamePlayData.nextTimeToStopGame = _startGameData.nextTimeToStopGame;
		for(int i = 0; i < unoGamePlayData.listPlayerPlayingData.Count; i++){
			int _indexChair = unoGamePlayData.listPlayerPlayingData[i].indexChair;
			Uno_PlayerGroup _playerGroup = listPlayerGroup[_indexChair];
			if(!_playerGroup.isInitialized){
				#if TEST
				Debug.LogError("Lỗi chưa init player group hoặc đang start game mà thoát: " + unoGamePlayData.listPlayerPlayingData[i].userData.nameShowInGame + " - " + unoGamePlayData.listPlayerPlayingData[i].indexChair + " - " + unoGamePlayData.listPlayerPlayingData[i].userData.sessionId);
				#endif
			}
			_playerGroup.InitData(unoGamePlayData.listPlayerPlayingData[i].userData);
			LeanTween.scale(_playerGroup.panelPlayerInfo.gameObject
						, Vector3.one * UIManager.listPlaceHolderPanelPlayerInfo_Wating[_playerGroup.realIndex].ratioScale
						, 0.2f).setEase(LeanTweenType.easeOutBack);
		}
		// ------------------------------- //
		
		UIManager.RefreshUIButtonSitDown();
		UIManager.StopShowCountDownStartGame();
		if(UIManager.isChangingView){
			yield return new WaitUntil(()=>!UIManager.isChangingView);
		}
		yield return UIManager.MoveAllToPosPlaying(false);

		System.TimeSpan _tmpDelta = unoGamePlayData.nextTimeToStopGame - System.DateTime.Now;
		UIManager.ShowCountDownStopGame((long) _tmpDelta.TotalSeconds);
		
		List<IEnumerator> _listProcess = new List<IEnumerator>();
		_listProcess.Add(UIManager.DoActionPrepareToStartGame());
		_listProcess.Add(DoActionDealCardAtStartGame());
		_listProcess.Add(DoActionShowCircleTurnAtStartGame());
		_listProcess.Add(UIManager.unoBackground.DoActionSetColor(unoGamePlayData.currentColor, false));
		yield return CoroutineChain.Start.Parallel(_listProcess.ToArray());

		yield return StartCoroutine(DoActionPlayerChangeTurn());

		_startGameData = null;
		unoGamePlayData.processStartGameData.RemoveAt(0);

		// #if TEST
		// Debug.Log("<color=green> End process Start New Game: " + unoGamePlayData.processStartGameData.Count +"</color>");
		// #endif
	}

	IEnumerator DoActionDealCardAtStartGame(){
		for(int i = 0; i < 7; i ++){
			for(int j = 0; j < unoGamePlayData.listPlayerPlayingData.Count; j++){
				int _indexChair = unoGamePlayData.listPlayerPlayingData[j].indexChair;
				Uno_PlayerGroup _playerGroup = listPlayerGroup[_indexChair];
				if(_playerGroup.isInitialized){
					UIManager.DealPlayerCard(_playerGroup, unoGamePlayData.listPlayerPlayingData[j].ownCards[i], 0.16f);
					yield return Yielders.Get(0.03f);
				}else{
					Debug.LogError("BUG LOGIC!");
				}
			}
		}
	}

	IEnumerator DoActionShowCircleTurnAtStartGame(){
		if(UIManager.unoCircleTurn.myState == Uno_CircleTurn_Controller.State.Hide){
			UIManager.unoCircleTurn.Show(false);
			yield return StartCoroutine(UIManager.unoCircleTurn.DoActionSetTurnDirection(unoGamePlayData.turnDirection, false));
		}
	}

	// IEnumerator DoActionPlayGame(){
	// 	//TODO: Play game
	// 	// - Change turn
	// 	// - Play game
	// 	System.TimeSpan _tmpDelta = unoGamePlayData.nextTimeToStopGame - System.DateTime.Now;
	// 	UIManager.ShowCountDownStopGame((long) _tmpDelta.TotalMilliseconds);

	// 	yield return StartCoroutine(DoActionPlayerChangeTurn());

	// 	if(UIManager.unoCircleTurn.myState == Uno_CircleTurn_Controller.State.Hide){
	// 		UIManager.unoCircleTurn.Show(false);
	// 		yield return StartCoroutine(UIManager.unoCircleTurn.DoActionSetTurnDirection(unoGamePlayData.turnDirection, false));
	// 	}

	// 	if(listProcessPlaying.Count == 0){
	// 		yield return null;
	// 	}

	// 	while(true){
	// 		yield return new WaitUntil(()=> listProcessPlaying.Count > 0);
	// 		yield return StartCoroutine(listProcessPlaying[0]);
	// 		listProcessPlaying.RemoveAt(0);
	// 	}
	// }

	IEnumerator DoActionImPlayingAndStandUp(){
		if(myPlayerGroup == null){
			#if TEST
			Debug.LogError(">>> Không tìm thấy myPlayerGroup");
			#endif
			yield break;
		}
		myPlayerGroup.currentCardUnoFocusing = null;
		myPlayerGroup.HideButtonStandUp();
		for(int i = 0; i < listPlayerGroup.Count; i++){
			if(listPlayerGroup[i].isInitialized){
				listPlayerGroup[i].HideButtonAtkUno();
			}
		}
		float _tmpTime = 0.2f;

		List<IEnumerator> _tmpListProcess = new List<IEnumerator>();
		for(int i = 0; i < myPlayerGroup.ownCardPoolManager.listObjects.Count; i++){
			PanelCardUnoDetailController _panelCardDetail = (PanelCardUnoDetailController) myPlayerGroup.ownCardPoolManager.listObjects[i];
			_panelCardDetail.onPointerDown = null;
			_panelCardDetail.SetUpShadow(false);
			_panelCardDetail.MoveLocal(Vector2.zero, _tmpTime, LeanTweenType.easeOutBack);
			_panelCardDetail.HideHighlight();
			_tmpListProcess.Add(_panelCardDetail.Hide());
		}
		yield return CoroutineChain.Start.
			Parallel(_tmpListProcess.ToArray());

		_tmpListProcess.Clear();
		
		CardHolderController _cardHolder = null;
		bool _canCompactCard = false;
		if(myPlayerGroup.ownCardPoolManager.listObjects.Count > UIManager.numCardsCompact){
			_canCompactCard = true;
		}

		for(int i = 0; i < myPlayerGroup.ownCardPoolManager.listObjects.Count; i++){
			PanelCardUnoDetailController _panelCardDetail = (PanelCardUnoDetailController) myPlayerGroup.ownCardPoolManager.listObjects[i];
			_panelCardDetail.DestroyCardHolder();
			_cardHolder = UIManager.CreateCardHolder(myPlayerGroup);
			_panelCardDetail.SetCardHolder(_cardHolder);
			_tmpListProcess.Add(_panelCardDetail.ScaleTo(Vector2.one * _cardHolder.ratioScale, _tmpTime, LeanTweenType.notUsed));
			
			if(_canCompactCard){
				_tmpListProcess.Add(_panelCardDetail.Move(UIManager.listPanelContainPlayerCardsCompactHolder[myPlayerGroup.viewIndex].position, _tmpTime, LeanTweenType.easeInOutSine));
			}else{
				_tmpListProcess.Add(_panelCardDetail.MoveToCardHolder(_tmpTime, LeanTweenType.easeInOutSine));
			}
		}
		_tmpListProcess.Add(UIManager.DoActionMoveToPosPlaying(myPlayerGroup, false));

		yield return CoroutineChain.Start.
			Parallel(_tmpListProcess.ToArray());
		
		if(_canCompactCard){
			UIManager.listTxtPlayerNumberCards[myPlayerGroup.viewIndex].gameObject.SetActive(true);
			UIManager.listTxtPlayerNumberCards[myPlayerGroup.viewIndex].text = myPlayerGroup.ownCardPoolManager.listObjects.Count.ToString();
		}

		myPlayerGroup = null;
		UIManager.myBarController.HideAllButtons();
		UIManager.myBarController.HideBtnUno();
		UIManager.ShowMyPanelUserInfo();
		UIManager.RefreshUIButtonSitDown();
	}

	IEnumerator DoActionPlayerPutCard(){
		if(unoGamePlayData.currentGameState != UnoGamePlayData.GameState.STATUS_PLAYING){
			#if TEST
			Debug.LogError("Not in State STATUS_PLAYING: " + unoGamePlayData.currentGameState.ToString());
			#endif
			unoGamePlayData.processPlayerPutCardData.RemoveAt(0);
			yield break;
		}

		//TODO: Đánh bài
		// - Quăng lá bài cần đánh
		// - Xóa card holder
		// - Xếp lại bộ card
		// - Refresh lại button của người chơi
		// - SetUp hiệu ứng
		// - UnFocus lá bài

		// #if TEST
		// Debug.Log("<color=green> Start process Player Put Card</color>");
		// #endif

		UnoGamePlayData.Uno_Player_PutCard_Data _playerPutCardData = unoGamePlayData.processPlayerPutCardData[0];
		
		sbyte _indexCirclePlaying = _playerPutCardData.indexCircle;
		sbyte _indexChair = unoGamePlayData.listPlayerPlayingData[_indexCirclePlaying].indexChair;

		// --- Merge vào dữ liệu thật --- //
		bool _isReverseTurn = false;
		if(_playerPutCardData.turnDirection != unoGamePlayData.turnDirection){
			_isReverseTurn = true;
		}else{
			_isReverseTurn = false;
		}
		bool _isChangeColor = false;
		if(_playerPutCardData.bgColor != unoGamePlayData.currentColor){
			_isChangeColor = true;
		}else{
			_isChangeColor = false;
		}
		unoGamePlayData.indexCircleBeSkipped = -1;
		if(unoGamePlayData.IsSkipCard(_playerPutCardData.cardValue)){
			if(_playerPutCardData.turnDirection == UnoGamePlayData.TurnDirection.ClockWise){
				int _index = unoGamePlayData.currentCircle + 1;
				if(_index >= unoGamePlayData.listPlayerPlayingData.Count){
					_index = 0;
				}
				unoGamePlayData.indexCircleBeSkipped = _index;
			}else{
				int _index = unoGamePlayData.currentCircle - 1;
				if(_index < 0){
					_index = unoGamePlayData.listPlayerPlayingData.Count - 1;
				}
				unoGamePlayData.indexCircleBeSkipped = _index;
			}
		}

		unoGamePlayData.lastCardPut = _playerPutCardData.cardValue;
		unoGamePlayData.globalCards.Add(unoGamePlayData.lastCardPut);
		unoGamePlayData.sumCardGet = _playerPutCardData.sumCardGet;
		unoGamePlayData.turnDirection = _playerPutCardData.turnDirection;
		unoGamePlayData.currentColor = _playerPutCardData.bgColor;
		if(_playerPutCardData.clientIndexCard < 0
			|| _playerPutCardData.clientIndexCard >= unoGamePlayData.listPlayerPlayingData[_indexCirclePlaying].ownCards.Count){
			Debug.LogError(">>> Bug index out of range: " + _playerPutCardData.clientIndexCard);
		}
		unoGamePlayData.listPlayerPlayingData[_indexCirclePlaying].ownCards.RemoveAt(_playerPutCardData.clientIndexCard);
		
		// #if TEST
		// Debug.Log(">>> (PlayerPutCard) Player " + unoGamePlayData.listPlayerPlayingData[_indexCirclePlaying].userData.nameShowInGame + " tại vị trí playing " + _indexCirclePlaying + " còn " + _playerPutCardData.countCard + " lá bài");
		// #endif
		// ------------------------------ //

		Uno_PlayerGroup _unoPlayerGroup = listPlayerGroup[_indexChair];
		if(!_unoPlayerGroup.isInitialized){
			#if TEST
			Debug.LogError(">>> BUG LOGIC PlayerPutCard (0)");
			#endif
		}
		if(_playerPutCardData.clientIndexCard < 0 || _playerPutCardData.clientIndexCard >= _unoPlayerGroup.ownCardPoolManager.listObjects.Count){
			#if TEST
			Debug.LogError(">>> BUG LOGIC PlayerPutCard (1): " + _playerPutCardData.clientIndexCard);
			#endif
		}
		PanelCardUnoDetailController _panelCardUno = (PanelCardUnoDetailController) _unoPlayerGroup.ownCardPoolManager.listObjects[_playerPutCardData.clientIndexCard];
		_panelCardUno.HideHighlight();
		Vector3 _rot = UIManager.panelGlobalCardsHolderCatched.rotation.eulerAngles;
		_rot.z += Random.Range(-60f, 60f);
		bool _isMyTurn = false;
		bool _isThisWildCard = false;

		if(_panelCardUno.cardValue < 0){
			CardUnoInfo _cardInfo = null;
			if(unoGamePlayData.IsWildCardColor(_playerPutCardData.cardValue)){
				_cardInfo = this.GetCardInfo(CardUnoInfo.CardType._Special_WildColor);
				_isThisWildCard = true;
			}else if(unoGamePlayData.IsWildCardDraw(_playerPutCardData.cardValue)){
				_cardInfo = this.GetCardInfo(CardUnoInfo.CardType._Special_Draw4Cards);
				_isThisWildCard = true;
			}else{
				_cardInfo = this.GetCardInfo(_playerPutCardData.cardValue);
			}
			if(_cardInfo == null){
				#if TEST
				Debug.LogError(">>> Không tìm thấy cardInfo (2): " + _playerPutCardData.cardValue);
				#endif
			}
			_panelCardUno.ShowNow(_cardInfo, _playerPutCardData.cardValue);
		}else{
			if(unoGamePlayData.IsWildCardColor(_playerPutCardData.cardValue)){
				_isThisWildCard = true;
			}else if(unoGamePlayData.IsWildCardDraw(_playerPutCardData.cardValue)){
				_isThisWildCard = true;
			}
		}

		#if TEST
		if(unoGamePlayData.listPlayerPlayingData[_indexCirclePlaying].isMe){
			Debug.Log("<color=brown>" + ">>> (PlayerPutCard) Mình tại vị trí playing " + _indexCirclePlaying + " đánh lá " + _panelCardUno.cardInfo.cardType.ToString()  + " còn " + _playerPutCardData.countCard + " lá bài </color>");
		}else{
			Debug.Log("<color=brown>" + ">>> (PlayerPutCard) Player tại vị trí playing " + _indexCirclePlaying + " tên là " + unoGamePlayData.listPlayerPlayingData[_indexCirclePlaying].userData.nameShowInGame +" đánh lá " + _panelCardUno.cardInfo.cardType.ToString() + "</color>");
		}
		#endif

		Vector2 _ratioScale = _panelCardUno.GetRatioScale(UIManager.sizeCardDefault.x, UIManager.sizeCardDefault.y);

		if(_unoPlayerGroup.isMe
			&& unoGamePlayData.CheckIsPlaying(DataManager.instance.userData.sessionId)){
			_isMyTurn = true;
			UIManager.myBarController.HideAllButtons();
		}

		_panelCardUno.DestroyCardHolder();
		_panelCardUno.ResetLocalPos();
		_panelCardUno.onPointerDown = null;
		_panelCardUno.onSelfDestruction = null;
		_unoPlayerGroup.ownCardPoolManager.listObjects.RemoveAt(_playerPutCardData.clientIndexCard);
		
		yield return null;
		float _timeAnim = 0.2f;

		bool _canCompactCard = true;
		if(_unoPlayerGroup.isMe
			&& unoGamePlayData.CheckIsPlaying(DataManager.instance.userData.sessionId)){
			_canCompactCard = false;
		}

		if(_canCompactCard && _unoPlayerGroup.ownCardPoolManager.listObjects.Count > UIManager.numCardsCompact){
			UIManager.listTxtPlayerNumberCards[_unoPlayerGroup.viewIndex].gameObject.SetActive(true);
			UIManager.listTxtPlayerNumberCards[_unoPlayerGroup.viewIndex].text = _unoPlayerGroup.ownCardPoolManager.listObjects.Count.ToString();

			for(int i = 0; i < _unoPlayerGroup.ownCardPoolManager.listObjects.Count; i++){
				PanelCardUnoDetailController _panelCardDetail = (PanelCardUnoDetailController) _unoPlayerGroup.ownCardPoolManager.listObjects[i];
				StartCoroutine(_panelCardDetail.Move(UIManager.listPanelContainPlayerCardsCompactHolder[_unoPlayerGroup.viewIndex].position, _timeAnim / 2f, LeanTweenType.easeOutSine));
			}
			yield return Yielders.Get(_timeAnim / 2f);
			for(int i = 0; i < _unoPlayerGroup.ownCardPoolManager.listObjects.Count; i++){
				PanelCardUnoDetailController _panelCardDetail = (PanelCardUnoDetailController) _unoPlayerGroup.ownCardPoolManager.listObjects[i];
				if(i == 0){
					_panelCardDetail.SetVisible();
				}else{
					_panelCardDetail.SetInVisible();
				}
			}
		}else{
			UIManager.listTxtPlayerNumberCards[_unoPlayerGroup.viewIndex].gameObject.SetActive(false);
			for(int i = 0; i < _unoPlayerGroup.ownCardPoolManager.listObjects.Count; i++){
				PanelCardUnoDetailController _panelCardDetail = (PanelCardUnoDetailController) _unoPlayerGroup.ownCardPoolManager.listObjects[i];
				_panelCardDetail.SetVisible();
				StartCoroutine(_panelCardDetail.MoveToCardHolder(_timeAnim, LeanTweenType.easeInOutSine));
			}
		}

		yield return CoroutineChain.Start
			.Parallel(_panelCardUno.Move(UIManager.panelGlobalCardsHolderCatched.position, _timeAnim, LeanTweenType.easeOutSine, this.CanPlayMusicAndSfx() ? myAudioInfo.sfx_Card : null)
				, _panelCardUno.Rotate(_rot, _timeAnim)
				, _panelCardUno.ScaleTo(_ratioScale, _timeAnim, LeanTweenType.notUsed));
		_panelCardUno.transform.SetParent(UIManager.panelContainGlobalCards);
		UIManager.cardsGlobalPoolManager.AddObject(_panelCardUno);

		if(_isMyTurn){
			_unoPlayerGroup.currentCardUnoFocusing = null;
			for(int i = 0; i < _unoPlayerGroup.ownCardPoolManager.listObjects.Count; i++){
				PanelCardUnoDetailController _panelCardDetail = (PanelCardUnoDetailController) _unoPlayerGroup.ownCardPoolManager.listObjects[i];
				_panelCardDetail.SetCanPut(false, false);
				_panelCardDetail.MoveLocal(Vector2.down * 30f, _timeAnim, LeanTweenType.easeOutBack);
			}
			yield return Yielders.Get(_timeAnim);
		}

		// --- Setup hiệu ứng --- //
		List<IEnumerator> _listProgess = new List<IEnumerator>();
		if(_isReverseTurn){
			_listProgess.Add(UIManager.unoCircleTurn.DoActionSetTurnDirection(_playerPutCardData.turnDirection, false));
		}
		if(_isChangeColor){
			_listProgess.Add(UIManager.unoBackground.DoActionSetColor(_playerPutCardData.bgColor, false));
			_listProgess.Add(UIManager.unoCircleTurn.DoActionSetColor(_playerPutCardData.bgColor, false));
		}
		if(_isThisWildCard){
			_listProgess.Add(_panelCardUno.DoActionChangeBgColor(_playerPutCardData.bgColor, false));
		}
		if(_listProgess != null && _listProgess.Count > 0){
			if(_isReverseTurn){
				if(this.CanPlayMusicAndSfx()){
					MyAudioManager.instance.PlaySfx(myAudioInfo.sfx_CardReverseTurn);
				}
			}
			if(_isThisWildCard){
				if(this.CanPlayMusicAndSfx()){
					MyAudioManager.instance.PlaySfx(myAudioInfo.sfx_WildCard);
				}
			}
			yield return CoroutineChain.Start
				.Parallel(_listProgess.ToArray());
		}
		if(unoGamePlayData.CheckIsPlaying(DataManager.instance.userData.sessionId)){
			if(!unoGamePlayData.listPlayerPlayingData[_indexCirclePlaying].isMe
			&& unoGamePlayData.listPlayerPlayingData[_indexCirclePlaying].ownCards.Count < 2
			&& !unoGamePlayData.listPlayerPlayingData[_indexCirclePlaying].hasCalledUno){
				if(this.CanPlayMusicAndSfx()){
					MyAudioManager.instance.PlaySfx(myAudioInfo.sfx_BtnUnoOrAtkUnoAppear);
				}
				_unoPlayerGroup.ShowButtonAtkUno();
			}
		}
		// ---------------------- //
		
		_playerPutCardData = null;
		unoGamePlayData.processPlayerPutCardData.RemoveAt(0);

		// #if TEST
		// Debug.Log("<color=green> End process Player Put Card: " + unoGamePlayData.processPlayerPutCardData.Count +"</color>");
		// #endif
	}

	IEnumerator DoActionPlayerChangeTurn(bool _initAtStartGame = true){
		if(unoGamePlayData.currentGameState != UnoGamePlayData.GameState.STATUS_PLAYING){
			#if TEST
			Debug.LogError("Not in State STATUS_PLAYING: " + unoGamePlayData.currentGameState.ToString());
			#endif
			if(!_initAtStartGame){
				unoGamePlayData.processPlayerChangeTurnData.RemoveAt(0);
			}
			yield break;
		}

		//TODO: Change Turn
		// - Focus bài mình nếu tới lượt mình
		// - Cho countDown người cược tiếp theo

		// #if TEST
		// Debug.Log("<color=green> Start process Player Change Turn</color>");
		// #endif

		UnoGamePlayData.Uno_PlayerPlayingData _currentPlayerPlaying = null;
		Uno_PlayerGroup _currentPlayerGroup = null;
		UnoGamePlayData.Uno_Player_ChangeTurn_Data _playerChangeTurnData = null;

		// --- Merge vào dữ liệu thật --- //
		if(!_initAtStartGame){
			if(unoGamePlayData.processPlayerChangeTurnData.Count > 0){
				_playerChangeTurnData = unoGamePlayData.processPlayerChangeTurnData[0];

				if(unoGamePlayData.listPlayerPlayingData.Count == 2
					&& !unoGamePlayData.IsSkipCard(unoGamePlayData.lastCardPut)
					&& unoGamePlayData.currentCircle == _playerChangeTurnData.nextCircleIndex){
					#if TEST
					Debug.LogError(">>> BUG logic ChangeTurn (0): " + unoGamePlayData.currentCircle + " - " + _playerChangeTurnData.nextCircleIndex);
					#endif
					yield break;
				}

				_currentPlayerPlaying = unoGamePlayData.listPlayerPlayingData[unoGamePlayData.currentCircle];
				_currentPlayerGroup = listPlayerGroup[_currentPlayerPlaying.indexChair];
				if(_currentPlayerGroup == null
					|| !_currentPlayerGroup.isInitialized){
					#if TEST
					Debug.LogError(">>> BUG logic ChangeTurn (1)");
					#endif
					yield break;
				}
				if(callbackManager.onChangeNewTurn != null){
					callbackManager.onChangeNewTurn();
					callbackManager.onChangeNewTurn = null;
				}

				if(_currentPlayerPlaying.hasCalledUno
					&& _currentPlayerPlaying.ownCards.Count >= 2){
					_currentPlayerPlaying.hasCalledUno = false;
				}

				unoGamePlayData.currentCircle = _playerChangeTurnData.nextCircleIndex;
				unoGamePlayData.nextTimeToChangeCircle = _playerChangeTurnData.nextTimeToChangeCircle;

				if(unoGamePlayData.indexCircleBeSkipped >= 0
					&& unoGamePlayData.indexCircleBeSkipped < unoGamePlayData.listPlayerPlayingData.Count){
					UnoGamePlayData.Uno_PlayerPlayingData _playerPlayingDataBeSkipped = unoGamePlayData.listPlayerPlayingData[unoGamePlayData.indexCircleBeSkipped];
					Uno_PlayerGroup _playerGroupBeSkipped = listPlayerGroup[_playerPlayingDataBeSkipped.indexChair];
					// Debug.Log("indexChairBeSkipped: " + _playerPlayingDataBeSkipped.indexChair + "|" + unoGamePlayData.indexCircleBeSkipped);
					if(_playerGroupBeSkipped.isInitialized){
						if(this.CanPlayMusicAndSfx()){
							MyAudioManager.instance.PlaySfx(myAudioInfo.sfx_CardForbiden);
						}
						_playerGroupBeSkipped.effForbiden.Show(unoGamePlayData.currentColor);
						callbackManager.onChangeNewTurn += _playerGroupBeSkipped.effForbiden.Hide;
					}else{
						#if TEST
						Debug.LogError(">>> BUG logic ChangeTurn (3)");
						#endif
					}
					unoGamePlayData.indexCircleBeSkipped = -1;
				}
			}
		}
		// ------------------------------ //

		System.TimeSpan _tmpDelta = unoGamePlayData.nextTimeToChangeCircle - System.DateTime.Now;
		double _timeCountDown = _tmpDelta.TotalSeconds;
		_currentPlayerPlaying = unoGamePlayData.listPlayerPlayingData[unoGamePlayData.currentCircle];
		_currentPlayerGroup = listPlayerGroup[_currentPlayerPlaying.indexChair];
		if(_currentPlayerGroup != null && _currentPlayerGroup.isInitialized){
			_currentPlayerGroup.panelPlayerInfo.StartCountDown(_timeCountDown, _timeCountDown);
			callbackManager.onChangeNewTurn += _currentPlayerGroup.panelPlayerInfo.StopCountDown;

			if(_currentPlayerGroup.isMe
				&& unoGamePlayData.CheckIsPlaying(DataManager.instance.userData.sessionId)){
				_currentPlayerGroup.currentCardUnoFocusing = null;
				for(int i = 0; i < _currentPlayerGroup.ownCardPoolManager.listObjects.Count; i++){
					PanelCardUnoDetailController _panelCardDetail = (PanelCardUnoDetailController) _currentPlayerGroup.ownCardPoolManager.listObjects[i];
					if(unoGamePlayData.CheckCanPutCard((sbyte) _panelCardDetail.cardValue)){
						_panelCardDetail.SetCanPut(true);
						_panelCardDetail.MoveLocal(Vector2.zero, 0.2f, LeanTweenType.easeOutBack);
					}else{
						_panelCardDetail.SetCanPut(false);
						_panelCardDetail.MoveLocal(Vector2.down * 30f, 0.2f, LeanTweenType.easeOutBack);
					}
				}
			}else{
				if(myPlayerGroup != null && unoGamePlayData.CheckIsPlaying(myPlayerGroup.userData.sessionId)){
					for(int i = 0; i < myPlayerGroup.ownCardPoolManager.listObjects.Count; i++){
						PanelCardUnoDetailController _panelCardDetail = (PanelCardUnoDetailController) myPlayerGroup.ownCardPoolManager.listObjects[i];
						myPlayerGroup.currentCardUnoFocusing = null;
						_panelCardDetail.SetCanPut(false, false);
						_panelCardDetail.SetUpShadow(false);
						_panelCardDetail.MoveLocal(Vector2.zero, 0.2f, LeanTweenType.easeOutBack);
						_panelCardDetail.HideHighlight();
					}
				}
			}
			bool _canHideBtnCallUno = true;
			if(unoGamePlayData.CheckIsPlaying(DataManager.instance.userData.sessionId)
				&& !unoGamePlayData.CheckIsMyTurn()){
				if(myPlayerGroup != null
					&& myPlayerGroup.ownCardPoolManager != null
					&& myPlayerGroup.ownCardPoolManager.listObjects.Count < 2){
					int _index = unoGamePlayData.listSessionIdPlaying.IndexOf(DataManager.instance.userData.sessionId);
					if(!unoGamePlayData.listPlayerPlayingData[_index].hasCalledUno){
						UIManager.myBarController.ShowBtnUno();
						_canHideBtnCallUno = false;
					}
				}
			}
			if(_canHideBtnCallUno){
				UIManager.myBarController.HideBtnUno();
			}
			UIManager.myBarController.RefreshAllMyButton();
			yield return Yielders.Get(0.5f);
		}else{
			#if TEST
			Debug.LogError(">>> BUG logic circleCurrent: " + unoGamePlayData.currentCircle);
			#endif
			if(!_currentPlayerGroup.isInitialized){
				#if TEST
				Debug.LogError(">>> BUG logic ChangeTurn (2)");
				#endif
			}

			UIManager.myBarController.RefreshAllMyButton();
		}

		if(_playerChangeTurnData != null){
			_playerChangeTurnData = null;
			unoGamePlayData.processPlayerChangeTurnData.RemoveAt(0);
			// #if TEST
			// Debug.Log("<color=green> End process Change Turn: " + unoGamePlayData.processPlayerChangeTurnData.Count +"</color>");
			// #endif
		}else{
			// #if TEST
			// Debug.Log("<color=green> End process Change Turn </color>");
			// #endif
		}
	}

	IEnumerator DoActionMeGetCard(){
		if(unoGamePlayData.currentGameState != UnoGamePlayData.GameState.STATUS_PLAYING){
			#if TEST
			Debug.LogError("Not in State STATUS_PLAYING: " + unoGamePlayData.currentGameState.ToString());
			#endif
			unoGamePlayData.processMeGetCard.RemoveAt(0);
			yield break;
		}

		//TODO: Me get card
		//	- Merge vào dữ liệu thật
		// 	- Chia bài
		// 	- Set check can put card

		// #if TEST
		// Debug.Log("<color=green> Start process Me Get Card </color>");
		// #endif

		UnoGamePlayData.Uno_Me_GetCard_Data _meGetCardData = unoGamePlayData.processMeGetCard[0];
		if(_meGetCardData.numberCardGet < 1){
			#if TEST
			Debug.Log("<color=brown>" + ">>> (MeGetCard) Lỗi rút bài nhiều lần: " + _meGetCardData.numberCardGet + "</color>");
			#endif
			_meGetCardData = null;
			unoGamePlayData.processMeGetCard.RemoveAt(0);
			yield break;
		}

		int _tmpStart = 0;
		int _tmpEnd = 0;
		bool _canCheckCard = true;

		// --- Merge vào dữ liệu thật --- //
		UnoGamePlayData.Uno_PlayerPlayingData _myPlayingData = null;
		for(int i = 0; i < unoGamePlayData.listPlayerPlayingData.Count; i ++){
			if(unoGamePlayData.listPlayerPlayingData[i].isMe){
				_myPlayingData = unoGamePlayData.listPlayerPlayingData[i];
				break;
			}
		}
		if(_myPlayingData == null){
			#if TEST
			Debug.LogError(">>> BUG logic MeGetCard (0)");
			#endif
			yield break;
		}

		_tmpStart = _myPlayingData.ownCards.Count;
		_tmpEnd = _tmpStart + _meGetCardData.myCardsValue.Count;

		for(int i = 0; i < _meGetCardData.myCardsValue.Count; i++){
			_myPlayingData.ownCards.Add(_meGetCardData.myCardsValue[i]);
		}
		if(unoGamePlayData.sumCardGet > 0){
			_canCheckCard = false;
		}
		unoGamePlayData.sumCardGet = 0;
		_myPlayingData.hasCalledUno = false;

		#if TEST
		Debug.Log("<color=brown>" + ">>> (MeGetCard) Mình rút " + _meGetCardData.numberCardGet + " lá bài, còn lại "+ _meGetCardData.countCard + " lá bài </color>");
		#endif
		// ------------------------------ //

		UIManager.panelChooseColor.Hide(-1);

		Uno_PlayerGroup _currentPlayerGroup = listPlayerGroup[_myPlayingData.indexChair];
		if(_currentPlayerGroup != null){
			if(!_currentPlayerGroup.isInitialized){
				Debug.LogError(">>> BUG logic MeGetCard (1)");
				yield break;
			}
			int _tmpCardDealFinished = 0;

			if(_meGetCardData.myCardsValue.Count > 1){
				if(this.CanPlayMusicAndSfx()){
					MyAudioManager.instance.PlaySfx(myAudioInfo.sfx_PublishGetMoreCards);
				}
			}

			for(int i = 0; i < _meGetCardData.myCardsValue.Count; i ++){
				UIManager.DealPlayerCard(_currentPlayerGroup, _meGetCardData.myCardsValue[i], 0.2f, ()=>{
					_tmpCardDealFinished ++;
				});
				yield return Yielders.Get(0.1f);
			}
			_currentPlayerGroup.effCallUno.Hide();
			yield return new WaitUntil(()=>(_tmpCardDealFinished == _meGetCardData.myCardsValue.Count));
		}else{
			Debug.LogError(">>> BUG logic MeGetCard (2)");
		}
		
		if(unoGamePlayData.CheckIsPlaying(_myPlayingData.userData.sessionId)){
			if(_myPlayingData.ownCards.Count <= 2){
				UIManager.myBarController.ShowBtnUno();
			}else{
				UIManager.myBarController.HideBtnUno();
			}
			if(unoGamePlayData.CheckIsMyTurn()){
				if(_canCheckCard){
					for(int i = _tmpStart; i < _tmpEnd; i++){
						PanelCardUnoDetailController _panelCardDetail = (PanelCardUnoDetailController) _currentPlayerGroup.ownCardPoolManager.listObjects[i];
						if(unoGamePlayData.CheckCanPutCard((sbyte) _panelCardDetail.cardValue)){
							_panelCardDetail.SetCanPut(true);
							_panelCardDetail.MoveLocal(Vector2.zero, 0.2f, LeanTweenType.easeOutBack);
						}else{
							_panelCardDetail.SetCanPut(false);
							_panelCardDetail.MoveLocal(Vector2.down * 30f, 0.2f, LeanTweenType.easeOutBack);
						}
					}
				}
				
				if(UIManager.myBarController.stateDrawCard == 1){
					UIManager.myBarController.stateDrawCard = 2;
					UIManager.myBarController.HideBtnDraw(false);
					UIManager.myBarController.SetBtnSkip(UIManager.myBarController.alphaMyBtnWhenActive, true, false);
					yield return Yielders.Get(0.1f);
				}
			}
		}

		if(_currentPlayerGroup.effCallUno.currentState == Uno_EffectCallUno_Controller.State.Show){
			_currentPlayerGroup.effCallUno.Hide();
		}
		
		_meGetCardData = null;
		unoGamePlayData.processMeGetCard.RemoveAt(0);
		// #if TEST
		// Debug.Log("<color=green> End process Me Get Card: " + unoGamePlayData.processMeGetCard.Count +"</color>");
		// #endif
	}

	IEnumerator DoActionOtherPlayerGetCard(){
		if(unoGamePlayData.currentGameState != UnoGamePlayData.GameState.STATUS_PLAYING){
			#if TEST
			Debug.LogError("Not in State STATUS_PLAYING: " + unoGamePlayData.currentGameState.ToString());
			#endif
			unoGamePlayData.processOtherPlayerGetCard.RemoveAt(0);
			yield break;
		}

		//TODO: Other Player get card
		//	- Merge vào dữ liệu thật
		// 	- Chia bài
		
		// #if TEST
		// Debug.Log("<color=green> Start process Other Player Get Card </color>");
		// #endif

		UnoGamePlayData.Uno_OtherPlayer_GetCard_Data _otherPlayerGetCardData = unoGamePlayData.processOtherPlayerGetCard[0];

		if(_otherPlayerGetCardData.indexCircle != unoGamePlayData.currentCircle){
			#if TEST
			Debug.LogError(">>> BUG logic OtherPlayerGetCard (0) : " + _otherPlayerGetCardData.indexCircle + " - " + unoGamePlayData.currentCircle);
			#endif
			yield break;
		}

		// --- Merge vào dữ liệu thật --- //
		if(_otherPlayerGetCardData.indexCircle < 0
			|| _otherPlayerGetCardData.indexCircle >= unoGamePlayData.listPlayerPlayingData.Count){
			#if TEST
			Debug.LogError(">>> BUG logic OtherPlayerGetCard (1)");
			#endif
			yield break;
		}
		UnoGamePlayData.Uno_PlayerPlayingData _playingData = unoGamePlayData.listPlayerPlayingData[_otherPlayerGetCardData.indexCircle];
		if(_playingData == null){
			#if TEST
			Debug.LogError(">>> BUG logic OtherPlayerGetCard (2)");
			#endif
			yield break;
		}
		for(int i = 0; i < _otherPlayerGetCardData.cardsValue.Count; i++){
			_playingData.ownCards.Add(_otherPlayerGetCardData.cardsValue[i]);
		}
		unoGamePlayData.sumCardGet = 0;
		_playingData.hasCalledUno = false;

		#if TEST
		Debug.Log("<color=brown>" + ">>> (OtherPlayerGetCard) Player " + _playingData.userData.nameShowInGame + " tại vị trí " + _otherPlayerGetCardData.indexCircle + " rút " + _otherPlayerGetCardData.numberCardGet + " lá bài, còn lại " + _otherPlayerGetCardData.countCard + " lá bài </color>");
		#endif
		// ------------------------------ //

		Uno_PlayerGroup _currentPlayerGroup = listPlayerGroup[_playingData.indexChair];
		if(_currentPlayerGroup.isMe){
			#if TEST
			Debug.LogError(">>> BUG logic OtherPlayerGetCard (3)");
			#endif
		}
		if(_currentPlayerGroup != null){
			if(!_currentPlayerGroup.isInitialized){
				#if TEST
				Debug.LogError(">>> BUG logic OtherPlayerGetCard (4)");
				#endif
				yield break;
			}
			int _tmpCardDealFinished = 0;
			if(_otherPlayerGetCardData.cardsValue.Count > 1){
				if(this.CanPlayMusicAndSfx()){
					MyAudioManager.instance.PlaySfx(myAudioInfo.sfx_PublishGetMoreCards);
				}
			}
			for(int i = 0; i < _otherPlayerGetCardData.cardsValue.Count; i ++){
				UIManager.DealPlayerCard(_currentPlayerGroup, _otherPlayerGetCardData.cardsValue[i], 0.2f, ()=>{
					_tmpCardDealFinished ++;
				});
				yield return Yielders.Get(0.1f);
			}
			_currentPlayerGroup.effCallUno.Hide();
			yield return new WaitUntil(()=>(_tmpCardDealFinished == _otherPlayerGetCardData.cardsValue.Count));
		}else{
			#if TEST
			Debug.LogError(">>> BUG logic OtherPlayerGetCard (5)");
			#endif
		}
		
		_otherPlayerGetCardData = null;
		unoGamePlayData.processOtherPlayerGetCard.RemoveAt(0);
		// #if TEST
		// Debug.Log("<color=green> End process Other Player Get Card: " + unoGamePlayData.processOtherPlayerGetCard.Count +"</color>");
		// #endif
	}

	IEnumerator DoActionPlayerCallUno(){
		if(unoGamePlayData.currentGameState != UnoGamePlayData.GameState.STATUS_PLAYING){
			#if TEST
			Debug.LogError("Not in State STATUS_PLAYING: " + unoGamePlayData.currentGameState.ToString());
			#endif
			unoGamePlayData.processPlayerCallUnoData.RemoveAt(0);
			yield break;
		}

		//TODO: Player Call Uno
		//	- Merge vào dữ liệu thật
		//	- Ẩn nút uno (nếu là mình)
		// 	- Hiện hiệu ứng uno trên avatar người đó
		
		// #if TEST
		// Debug.Log("<color=green> Start process Player Call Uno </color>");
		// #endif

		UnoGamePlayData.Uno_Player_CallUno_Data _playerCallUnoData = unoGamePlayData.processPlayerCallUnoData[0];

		// --- Merge vào dữ liệu thật --- //
		if(_playerCallUnoData.indexCircle < 0
			|| _playerCallUnoData.indexCircle >= unoGamePlayData.listPlayerPlayingData.Count){
			#if TEST
			Debug.LogError(">>> BUG logic PlayerCallUno (0) : " + _playerCallUnoData.indexCircle);
			#endif
			yield break;
		}
		UnoGamePlayData.Uno_PlayerPlayingData _playingData = unoGamePlayData.listPlayerPlayingData[_playerCallUnoData.indexCircle];
		if(_playingData == null){
			#if TEST
			Debug.LogError(">>> BUG logic PlayerCallUno (1)");
			#endif
			yield break;
		}
		_playingData.hasCalledUno = true;
		// ------------------------------ //

		Uno_PlayerGroup _currentPlayerGroup = listPlayerGroup[_playingData.indexChair];
		if(!_currentPlayerGroup.isInitialized){
			#if TEST
			Debug.LogError(">>> BUG logic PlayerCallUno (2)");
			#endif
			yield break;
		}
		if(_currentPlayerGroup.isMe
			&& unoGamePlayData.CheckIsPlaying(_currentPlayerGroup.userData.sessionId)){
			UIManager.myBarController.HideBtnUno();
		}
		_currentPlayerGroup.effCallUno.Show();
		_currentPlayerGroup.HideButtonAtkUno();

		_playerCallUnoData = null;
		unoGamePlayData.processPlayerCallUnoData.RemoveAt(0);
		// #if TEST
		// Debug.Log("<color=green> End process Other Player Call Uno: " + unoGamePlayData.processPlayerCallUnoData.Count +"</color>");
		// #endif
	}

	IEnumerator DoActionPlayerAtkUno(){
		if(unoGamePlayData.currentGameState != UnoGamePlayData.GameState.STATUS_PLAYING){
			#if TEST
			Debug.LogError("Not in State STATUS_PLAYING: " + unoGamePlayData.currentGameState.ToString());
			#endif
			unoGamePlayData.processPlayerAtkUnoData.RemoveAt(0);
			yield break;
		}

		//TODO: Player Atk Uno
		//	- Show hiệu ứng
		//	- Rút bài

		// #if TEST
		// Debug.Log("<color=green> Start process Player Atk Uno </color>");
		// #endif

		UnoGamePlayData.Uno_Player_AtkUno_Data _unoPlayerAtkUnoData = unoGamePlayData.processPlayerAtkUnoData[0];

		// --- Check logic --- //
		if(_unoPlayerAtkUnoData.indexAttack < 0
			|| _unoPlayerAtkUnoData.indexAttack >= unoGamePlayData.listPlayerPlayingData.Count){
			#if TEST
			Debug.LogError(">>> BUG logic PlayerAtkUno (0) : " + _unoPlayerAtkUnoData.indexAttack);
			#endif
			yield break;
		}
		if(_unoPlayerAtkUnoData.indexBeAttacked < 0
			|| _unoPlayerAtkUnoData.indexBeAttacked >= unoGamePlayData.listPlayerPlayingData.Count){
			#if TEST
			Debug.LogError(">>> BUG logic PlayerAtkUno (1) : " + _unoPlayerAtkUnoData.indexBeAttacked);
			#endif
			yield break;
		}
		UnoGamePlayData.Uno_PlayerPlayingData _playerAtkData = unoGamePlayData.listPlayerPlayingData[_unoPlayerAtkUnoData.indexAttack];
		if(_playerAtkData == null){
			#if TEST
			Debug.LogError(">>> BUG logic PlayerAtkUno (2)");
			#endif
			yield break;
		}
		Uno_PlayerGroup _playerGroup_Atk = listPlayerGroup[_playerAtkData.indexChair];
		if(!_playerGroup_Atk.isInitialized){
			#if TEST
			Debug.LogError(">>> BUG logic PlayerAtkUno (3)");
			#endif
			yield break;
		}
		UnoGamePlayData.Uno_PlayerPlayingData _playerBeAttackedData = unoGamePlayData.listPlayerPlayingData[_unoPlayerAtkUnoData.indexBeAttacked];
		if(_playerBeAttackedData == null){
			#if TEST
			Debug.LogError(">>> BUG logic PlayerAtkUno (4)");
			#endif
			yield break;
		}
		if(_playerBeAttackedData.isMe){
			#if TEST
			Debug.LogError(">>> BUG logic PlayerAtkUno (5)");
			#endif
			yield break;
		}
		Uno_PlayerGroup _playerGroup_BeAttacked = listPlayerGroup[_playerBeAttackedData.indexChair];
		if(!_playerGroup_BeAttacked.isInitialized){
			#if TEST
			Debug.LogError(">>> BUG logic PlayerAtkUno (6)");
			#endif
			yield break;
		}
		for(int i = 0; i < 2; i++){
			_playerBeAttackedData.ownCards.Add(-1);
		}
		// ------------------------------ //

		yield return UIManager.ShowEffAtkUno(_playerGroup_Atk, _playerGroup_BeAttacked);
		_playerGroup_BeAttacked.HideButtonAtkUno();

		int _tmpCardDealFinished = 0;
		for(int i = 0; i < 2; i ++){
			UIManager.DealPlayerCard(_playerGroup_BeAttacked, -1, 0.2f, ()=>{
				_tmpCardDealFinished ++;
			});
			yield return Yielders.Get(0.1f);
		}
		yield return new WaitUntil(()=>(_tmpCardDealFinished == 2));

		_unoPlayerAtkUnoData = null;
		unoGamePlayData.processPlayerAtkUnoData.RemoveAt(0);
		// #if TEST
		// Debug.Log("<color=green> End process Player Atk Uno: " + unoGamePlayData.processPlayerAtkUnoData.Count +"</color>");
		// #endif
	}

	IEnumerator DoActionOtherPlayerAtkUnoMe(){
		if(unoGamePlayData.currentGameState != UnoGamePlayData.GameState.STATUS_PLAYING){
			#if TEST
			Debug.LogError("Not in State STATUS_PLAYING: " + unoGamePlayData.currentGameState.ToString());
			#endif
			unoGamePlayData.processOtherPlayerAtkUnoMeData.RemoveAt(0);
			yield break;
		}

		//TODO: Other player Atk Uno me
		//	- Show hiệu ứng
		//	- Rút bài
		
		// #if TEST
		// Debug.Log("<color=green> Start process Other Player Atk Uno Me </color>");
		// #endif

		UnoGamePlayData.Uno_OtherPlayer_AtkUno_Me_Data _unoOtherPlayerAtkUnoMeData = unoGamePlayData.processOtherPlayerAtkUnoMeData[0];

		// --- Check logic --- //
		if(_unoOtherPlayerAtkUnoMeData.indexPlayingAttackMe < 0
			|| _unoOtherPlayerAtkUnoMeData.indexPlayingAttackMe >= unoGamePlayData.listPlayerPlayingData.Count){
			#if TEST
			Debug.LogError(">>> BUG logic OtherPlayerAtkUnoMe (0) : " + _unoOtherPlayerAtkUnoMeData.indexPlayingAttackMe);
			#endif
			yield break;
		}
		UnoGamePlayData.Uno_PlayerPlayingData _playerAtkData = unoGamePlayData.listPlayerPlayingData[_unoOtherPlayerAtkUnoMeData.indexPlayingAttackMe];
		if(_playerAtkData == null){
			#if TEST
			Debug.LogError(">>> BUG logic OtherPlayerAtkUnoMe (1)");
			#endif
			yield break;
		}
		Uno_PlayerGroup _playerGroup_Atk = listPlayerGroup[_playerAtkData.indexChair];
		if(!_playerGroup_Atk.isInitialized){
			#if TEST
			Debug.LogError(">>> BUG logic OtherPlayerAtkUnoMe (2)");
			#endif
			yield break;
		}
		UnoGamePlayData.Uno_PlayerPlayingData _myPlayingData = null;
		for(int i = 0; i < unoGamePlayData.listPlayerPlayingData.Count; i ++){
			if(unoGamePlayData.listPlayerPlayingData[i].isMe){
				_myPlayingData = unoGamePlayData.listPlayerPlayingData[i];
				break;
			}
		}
		if(_myPlayingData == null){
			#if TEST
			Debug.LogError(">>> BUG logic OtherPlayerAtkUnoMe (3)");
			#endif
			yield break;
		}
		Uno_PlayerGroup _myGroup = listPlayerGroup[_myPlayingData.indexChair];
		if(_myGroup != null){
			if(!_myGroup.isInitialized){
				#if TEST
				Debug.LogError(">>> BUG logic OtherPlayerAtkUnoMe (4)");
				#endif
				yield break;
			}
			if(!_myGroup.isMe){
				#if TEST
				Debug.LogError(">>> BUG logic OtherPlayerAtkUnoMe (5)");
				#endif
				yield break;
			}
		}else{
			Debug.LogError(">>> BUG logic OtherPlayerAtkUnoMe (6)");
			yield break;
		}

		int _tmpStart = _myPlayingData.ownCards.Count;
		int _tmpEnd = _tmpStart + _unoOtherPlayerAtkUnoMeData.cardsDraw.Count;
		bool _isPlaying = unoGamePlayData.CheckIsPlaying(_myPlayingData.userData.sessionId);
		bool _isMyTurn = unoGamePlayData.CheckIsMyTurn();
		for(int i = 0; i < _unoOtherPlayerAtkUnoMeData.cardsDraw.Count; i++){
			_myPlayingData.ownCards.Add(_unoOtherPlayerAtkUnoMeData.cardsDraw[i]);
		}
		// ------------------------------ //

		yield return UIManager.ShowEffAtkUno(_playerGroup_Atk, _myGroup);
		if(_isPlaying){
			if(_myPlayingData.ownCards.Count <= 2){
				UIManager.myBarController.ShowBtnUno();
			}else{
				UIManager.myBarController.HideBtnUno();
			}
		}
		// _myGroup.HideButtonAtkUno();

		int _tmpCardDealFinished = 0;
		for(int i = 0; i < _unoOtherPlayerAtkUnoMeData.cardsDraw.Count; i ++){
			UIManager.DealPlayerCard(_myGroup, _unoOtherPlayerAtkUnoMeData.cardsDraw[i], 0.2f, ()=>{
				_tmpCardDealFinished ++;
			});
			yield return Yielders.Get(0.1f);
		}
		yield return new WaitUntil(()=>(_tmpCardDealFinished == _unoOtherPlayerAtkUnoMeData.cardsDraw.Count));

		if(_isPlaying && _isMyTurn){
			for(int i = _tmpStart; i < _tmpEnd; i++){
				PanelCardUnoDetailController _panelCardDetail = (PanelCardUnoDetailController) _myGroup.ownCardPoolManager.listObjects[i];
				if(unoGamePlayData.CheckCanPutCard((sbyte) _panelCardDetail.cardValue)){
					_panelCardDetail.SetCanPut(true);
					_panelCardDetail.MoveLocal(Vector2.zero, 0.2f, LeanTweenType.easeOutBack);
				}else{
					_panelCardDetail.SetCanPut(false);
					_panelCardDetail.MoveLocal(Vector2.down * 30f, 0.2f, LeanTweenType.easeOutBack);
				}
			}
			yield return Yielders.Get(0.2f);
		}

		_unoOtherPlayerAtkUnoMeData = null;
		unoGamePlayData.processOtherPlayerAtkUnoMeData.RemoveAt(0);
		// #if TEST
		// Debug.Log("<color=green> End process Other Player Atk Uno Me: " + unoGamePlayData.processOtherPlayerAtkUnoMeData.Count +"</color>");
		// #endif
	}

	IEnumerator DoActionFinishGame(){
		if(unoGamePlayData.currentGameState != UnoGamePlayData.GameState.STATUS_PLAYING){
			#if TEST
			Debug.LogError("Not in State STATUS_PLAYING: " + unoGamePlayData.currentGameState.ToString());
			#endif
			unoGamePlayData.processFinishGameData.RemoveAt(0);
			yield break;
		}

		//TODO: Finish Game
		// 	- Tắt hiệu ứng countdown, kêu uno, ...
		//	- Merge dữ liệu + tính điểm
		//	- Chỉ ra rõ nguyên nhân finish game là do người chơi tới hết bài hay hết thời gian hoặc rút hết bài, hoặc người chơi thoát hết
		//	- Show bài ra hết
		//	- Show bảng Scoreboard
		//	- Tắt bảng và destroy hết bài + hiệu ứng linh tinh + cập nhật lại chỗ ngồi

		// #if TEST
		// Debug.Log("<color=green> Start process Finish Game </color>");
		// #endif

		unoGamePlayData.currentGameState = UnoGamePlayData.GameState.STATUS_FINISHGAME;

		UnoGamePlayData.Uno_FinishGame_Data _unoFinishGameData = unoGamePlayData.processFinishGameData[0];

		// --- Tắt hiệu ứng countdown, kêu uno, ... --- //
		if(callbackManager.onChangeNewTurn != null){
			callbackManager.onChangeNewTurn();
			callbackManager.onChangeNewTurn = null;
		}
		UIManager.StopShowCountDownStopGame();
		UIManager.myBarController.HideAllButtons();
		UIManager.myBarController.HideBtnUno();
		UIManager.panelChooseColor.Hide(-1);
		
		for(int i = 0; i < listPlayerGroup.Count; i++){
			if(listPlayerGroup[i].isInitialized){
				listPlayerGroup[i].effCallUno.Hide();
				listPlayerGroup[i].effForbiden.Hide();
				listPlayerGroup[i].HideButtonAtkUno();
				listPlayerGroup[i].panelPlayerInfo.StopCountDown();
				for(int j = 0; j < listPlayerGroup[i].ownCardPoolManager.listObjects.Count; j++){
					PanelCardUnoDetailController _panelCardDetail = (PanelCardUnoDetailController) listPlayerGroup[i].ownCardPoolManager.listObjects[j];
					listPlayerGroup[i].currentCardUnoFocusing = null;
					_panelCardDetail.onPointerDown = null;
					_panelCardDetail.SetUpShadow(false);
					_panelCardDetail.MoveLocal(Vector2.zero, 0.2f, LeanTweenType.easeOutBack);
					_panelCardDetail.HideHighlight();
				}
			}
		}
		for(int i = 0; i < UIManager.listTxtPlayerNumberCards.Count; i++){
			UIManager.listTxtPlayerNumberCards[i].text = string.Empty;
			UIManager.listTxtPlayerNumberCards[i].gameObject.SetActive(false);
		}
		// -------------------------------------------- //

		// --- Merge dữ liệu --- //
		AchievementDetail _achievementDetail = DataManager.instance.achievementData.GetAchievementDetail(IMiniGameInfo.Type.Uno);
		if(_achievementDetail == null){
			#if TEST
			Debug.LogError(">>> _achievementDetail is null");
			#endif
		}
		for(int i = 0; i < _unoFinishGameData.listPlayersData.Count; i ++){
			UnoGamePlayData.Uno_FinishGame_Data.Player_Data _playerFinish = _unoFinishGameData.listPlayersData[i];
			UnoGamePlayData.Uno_PlayerPlayingData _playerPlayingData = unoGamePlayData.listPlayerPlayingData[_playerFinish.indexCircle];
			UserDataInGame _userDataInListGlobal = unoGamePlayData.GetUserDataInGameFromListGlobal(_playerPlayingData.userData.sessionId);
			// if(_playerFinish.ownCards.Count != _playerPlayingData.ownCards.Count){
			// 	#if TEST
			// 	Debug.LogError(">>> Bug Logic FinishGame (0): " + i + "|" + _playerFinish.ownCards.Count + "|" + _playerPlayingData.ownCards.Count);
			// 	#endif
			// 	yield break;
			// }
			_playerPlayingData.userData.gold = _playerFinish.goldLast;
			unoGamePlayData.UpdateGoldAgain(_playerPlayingData.userData.sessionId, _playerFinish.goldLast);
			_playerPlayingData.totalPoint = _playerFinish.totalPoint;
			if(_userDataInListGlobal != null){
				if(_playerFinish.achievementWinUpdate >= 0){
					_userDataInListGlobal.win = _playerFinish.achievementWinUpdate;
				}
				if(_playerFinish.achievementLoseUpdate >= 0){
					_userDataInListGlobal.lose = _playerFinish.achievementLoseUpdate;
				}
			}
			if(_playerPlayingData.isMe){
				if(_achievementDetail != null){
					if(_playerFinish.achievementWinUpdate >= 0){
						_achievementDetail.countWin = _playerFinish.achievementWinUpdate;
					}
					if(_playerFinish.achievementLoseUpdate >= 0){
						_achievementDetail.countLose = _playerFinish.achievementLoseUpdate;
					}
				}
			}
			if(_playerFinish.ownCards.Count != _playerPlayingData.ownCards.Count){
				#if TEST
				Debug.LogError(">>> Bug Logic FinishGame (0): " + i + "|" + _playerFinish.ownCards.Count + "|" + _playerPlayingData.ownCards.Count);
				#endif
				continue;
			}
			if(_playerPlayingData.isMe
				&& unoGamePlayData.CheckIsPlaying(_playerPlayingData.userData.sessionId)){
				#if TEST
				for(int m = 0; m < _playerPlayingData.ownCards.Count; m ++){
					bool _isExist = false;
					for(int n = 0; n < _playerFinish.ownCards.Count; n++){
						if(_playerPlayingData.ownCards[m] == _playerFinish.ownCards[n]){
							_isExist = true;
							break;
						}
					}
					if(!_isExist){
						#if TEST
						Debug.LogError(">>> Bug Logic FinishGame (1): " + i + "|" + _playerPlayingData.ownCards[m]);
						#endif
					} 
				}
				#endif
			}else{
				for(int m = 0; m < _playerPlayingData.ownCards.Count; m ++){
					if(_playerFinish.ownCards[m] < 0){
						#if TEST
						Debug.LogError(">>> Bug Logic FinishGame (4): " + m + "|" + _playerFinish.ownCards[m] + "|"+ _playerPlayingData.ownCards[m]);
						#endif
					}
					_playerPlayingData.ownCards[m] = _playerFinish.ownCards[m];
				}
			}
		}
		UIManager.panelScoreBoardFinishGame.InitData(_unoFinishGameData);
		UIManager.panelHistory.InitData(_unoFinishGameData, unoGamePlayData.listPlayerPlayingData);
		// --------------------- //

		if(this.CanPlayMusicAndSfx()){
			MyAudioManager.instance.PlaySfx(myAudioInfo.sfx_Fanfare);
		}

		yield return UIManager.panelReasonFinishGame.Show(_unoFinishGameData);
		yield return Yielders.Get(0.5f);
		yield return UIManager.panelReasonFinishGame.Hide();

		List<PanelCardUnoDetailController> _tmpListCards = new List<PanelCardUnoDetailController>();
		for(int i = 0; i < unoGamePlayData.listPlayerPlayingData.Count; i ++){
			int _indexChair = unoGamePlayData.listPlayerPlayingData[i].indexChair;
			Uno_PlayerGroup _playerGroup = listPlayerGroup[_indexChair];
			if(!_playerGroup.isInitialized){
				#if TEST
				Debug.LogError(">>> Bug Logic FinishGame (2)");
				#endif
			}else{
				if(_playerGroup.isMe
					&& unoGamePlayData.CheckIsPlaying(unoGamePlayData.listPlayerPlayingData[i].userData.sessionId)){
					for(int j = 0; j < unoGamePlayData.listPlayerPlayingData[i].ownCards.Count; j++){
						_tmpListCards.Add((PanelCardUnoDetailController) _playerGroup.ownCardPoolManager.listObjects[j]);
					}
				}else{
					CardUnoInfo _cardInfo = null;
					for(int j = 0; j < unoGamePlayData.listPlayerPlayingData[i].ownCards.Count; j++){
						_cardInfo = this.GetCardInfo(unoGamePlayData.listPlayerPlayingData[i].ownCards[j]);
						PanelCardUnoDetailController _card = ((PanelCardUnoDetailController) _playerGroup.ownCardPoolManager.listObjects[j]);
						if(_cardInfo == null){
							#if TEST
							Debug.LogError(">>> Bug Logic FinishGame (3): " + unoGamePlayData.listPlayerPlayingData[i].userData.nameShowInGame + " - " + unoGamePlayData.listPlayerPlayingData[i].ownCards[j]);
							#endif
						}else{
							CoroutineChain.Start
								.Parallel(_card.MoveToCardHolder(0.1f, LeanTweenType.easeOutSine)
									, _card.Show(_cardInfo, unoGamePlayData.listPlayerPlayingData[i].ownCards[j], 0.2f));
						}
						_tmpListCards.Add(_card);
					}
				}
			}
		}

		bool _showGoldWinFinished = false;
		StartCoroutine(UIManager.DoActionShowGoldWinToPlayerAtFinishGame(_unoFinishGameData, ()=>{
			_showGoldWinFinished = true;
		}));

		float _tmpCountTime = 0;
		bool _skipToShowScoreBoard = false;
		UIManager.panelTapToSkip.Show();
		UIManager.panelTapToSkip.onTap = ()=>{
			_skipToShowScoreBoard = true;
		};
		while(_tmpCountTime < 2f){
			yield return Yielders.FixedUpdate;
			_tmpCountTime += Time.fixedDeltaTime;
			if(_skipToShowScoreBoard){
				break;
			}
		}

		if(this.CanPlayMusicAndSfx()){
			MyAudioManager.instance.PlaySfx(myAudioInfo.sfx_OpenScoreBoard);
		}

		yield return UIManager.panelScoreBoardFinishGame.Show();
		bool _skipToHideScoreBoard = false;
		UIManager.panelTapToSkip.onTap = ()=>{
			_skipToHideScoreBoard = true;
		};
		_tmpCountTime = 0;
		while(_tmpCountTime < 3f){
			yield return Yielders.FixedUpdate;
			_tmpCountTime += Time.fixedDeltaTime;
			if(_skipToHideScoreBoard){
				break;
			}
		}
		yield return UIManager.panelScoreBoardFinishGame.Hide();
		UIManager.panelTapToSkip.Hide();

		for(int i = 0; i < UIManager.cardsGlobalPoolManager.listObjects.Count; i++){
			_tmpListCards.Add((PanelCardUnoDetailController) UIManager.cardsGlobalPoolManager.listObjects[i]);
		}

		yield return new WaitUntil(()=>_showGoldWinFinished);
		yield return UIManager.ClearAllCards(_tmpListCards);
		UIManager.cardsGlobalPoolManager.ClearAllObjectsNow();
		UIManager.unoCircleTurn.Hide(false);
		UIManager.unoBackground.Hide(false);
		yield return Yielders.Get(0.2f);
		
		unoGamePlayData.currentCircle = 0;
		unoGamePlayData.indexCircleBeSkipped = -1;
		unoGamePlayData.lastCardPut = -1;
		unoGamePlayData.totalBet = 0;
		unoGamePlayData.currentBet = unoGamePlayData.betDefault;
		unoGamePlayData.listPlayerPlayingData.Clear();
		unoGamePlayData.listSessionIdPlaying.Clear();
		unoGamePlayData.globalCards.Clear();

		UIManager.SetTableBet(unoGamePlayData.currentBet);
		if(UIManager.isChangingView){
			yield return new WaitUntil(()=>!UIManager.isChangingView);
		}
		yield return UIManager.MoveAllToPosWaiting(false);

		for(int i = 0; i < listPlayerGroup.Count; i++){
			if(!listPlayerGroup[i].isInitialized){
				continue;
			}
			listPlayerGroup[i].HideAndClear();
		}
		bool _waitToRefreshPlayergroup = false;
		for(int i = 0; i < listPlayerGroup.Count; i++){
			if(unoGamePlayData.listSessionIdOnChair[i] >= 0){
				UserDataInGame _userData = unoGamePlayData.GetUserDataInGameFromListGlobal(unoGamePlayData.listSessionIdOnChair[i]);
				if(_userData != null){
					listPlayerGroup[i].InitData(_userData);
					LeanTween.scale(listPlayerGroup[i].panelPlayerInfo.gameObject
						, Vector3.one * UIManager.listPlaceHolderPanelPlayerInfo_Wating[listPlayerGroup[i].realIndex].ratioScale
						, 0.2f).setEase(LeanTweenType.easeOutBack);
					_waitToRefreshPlayergroup = true;
				}
			}else{
				LeanTween.scale(listPlayerGroup[i].panelPlayerInfo.gameObject, Vector3.one, 0.2f)
						.setEase(LeanTweenType.easeOutBack);
				_waitToRefreshPlayergroup = true;
			}
		}

		if(_waitToRefreshPlayergroup) {
			yield return Yielders.Get(0.2f);
		}

		UIManager.RefreshUIButtonSitDown();

		if(UIManager.isChangingPosPlayingOrWaiting){
			yield return new WaitUntil(()=>!UIManager.isChangingPosPlayingOrWaiting);
		}
		yield return UIManager.ChangeView(myPlayerGroup);

		_unoFinishGameData = null;
		unoGamePlayData.processFinishGameData.RemoveAt(0);
		// #if TEST
		// Debug.Log("<color=green> End process Finish Game: " + unoGamePlayData.processFinishGameData.Count +"</color>");
		// #endif

		unoGamePlayData.currentGameState = UnoGamePlayData.GameState.STATUS_WAIT_FOR_PLAYER;
	}

	IEnumerator DoActionPlayerChat(){
		// #if TEST
		// Debug.Log("<color=green> Start process PlayerChat </color>");
		// #endif
		UnoGamePlayData.PlayerChat_Data _playerChatData = unoGamePlayData.processPlayerChatData[0];
		screenChat.AddMessage(_playerChatData.sessionId, _playerChatData.strMess, unoGamePlayData.listGlobalPlayerData);
        this.ShowPopupChat(_playerChatData.sessionId, _playerChatData.strMess);
		_playerChatData = null;
		unoGamePlayData.processPlayerChatData.RemoveAt(0);
		// #if TEST
		// Debug.Log("<color=green> End process PlayerChat: " + unoGamePlayData.processUnoPlayerChatData.Count +"</color>");
		// #endif
		yield break;
	}

	IEnumerator DoActionPlayerAddGold(){
		// #if TEST
		// Debug.Log("<color=green> Start process PlayerChat </color>");
		// #endif
		UnoGamePlayData.PlayerAddGold_Data _playerAddGoldData = unoGamePlayData.processPlayerAddGoldData[0];
		unoGamePlayData.UpdateGoldAgain(_playerAddGoldData.sessionId, _playerAddGoldData.goldLast);
		UIManager.SetUpPlayerAddGold(_playerAddGoldData.sessionId, _playerAddGoldData.reason, _playerAddGoldData.goldAdd, _playerAddGoldData.goldLast);
		_playerAddGoldData = null;
		unoGamePlayData.processPlayerAddGoldData.RemoveAt(0);
		// #if TEST
		// Debug.Log("<color=green> End process PlayerChat: " + unoGamePlayData.processUnoPlayerChatData.Count +"</color>");
		// #endif
		yield break;
	}

	IEnumerator DoActionPlayerSetParent(){
		UnoGamePlayData.PlayerSetParent_Data _playerSetParentData = unoGamePlayData.processPlayerSetParentData[0];
		unoGamePlayData.SetUpActionPlayerSetParent(_playerSetParentData);
		_playerSetParentData = null;
		unoGamePlayData.processPlayerSetParentData.RemoveAt(0);
		yield break;
	}
	#endregion

	#region On Button Click
	public void OnButtonSitDown(int _indexChair){
		if(unoGamePlayData.currentGameState == UnoGamePlayData.GameState.STATUS_WAIT_FOR_PLAYER){
			if(System.DateTime.Now.AddSeconds(1f) >= unoGamePlayData.nextTimeToStartGame
				&& System.DateTime.Now <= unoGamePlayData.nextTimeToStartGame){
				return;
			}
			if(System.DateTime.Now.AddSeconds(1f) >= unoGamePlayData.nextTimeToStartGame.AddSeconds(1f)
				&& System.DateTime.Now <= unoGamePlayData.nextTimeToStartGame.AddSeconds(1f)){
				return;
			}
		}

		if(UIManager.myBarController.nextTimeToTouch > System.DateTime.Now){
			return;
		}
		UIManager.myBarController.nextTimeToTouch = System.DateTime.Now.AddSeconds(0.5f);

		if(this.CanPlayMusicAndSfx()){
            MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
        }
		if(UIManager.isChangingView || UIManager.isChangingPosPlayingOrWaiting){
			return;
		}

		Uno_RealTimeAPI.instance.SendMessageSitDown((byte)_indexChair);
	}

	public void OnButtonStandUp(){
		if(unoGamePlayData.currentGameState == UnoGamePlayData.GameState.STATUS_WAIT_FOR_PLAYER){
			if(System.DateTime.Now.AddSeconds(1f) >= unoGamePlayData.nextTimeToStartGame
				&& System.DateTime.Now <= unoGamePlayData.nextTimeToStartGame){
				return;
			}
			if(System.DateTime.Now.AddSeconds(1f) >= unoGamePlayData.nextTimeToStartGame.AddSeconds(1f)
				&& System.DateTime.Now <= unoGamePlayData.nextTimeToStartGame.AddSeconds(1f)){
				return;
			}
		}

		if(UIManager.myBarController.nextTimeToTouch > System.DateTime.Now){
			return;
		}

		if(this.CanPlayMusicAndSfx()){
            MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
        }

		if(UIManager.isChangingView || UIManager.isChangingPosPlayingOrWaiting){
			return;
		}		

		if(unoGamePlayData.currentGameState == UnoGamePlayData.GameState.STATUS_WAIT_FOR_PLAYER
			|| unoGamePlayData.currentGameState == UnoGamePlayData.GameState.STATUS_FINISHGAME){
			Uno_RealTimeAPI.instance.SendMessageStandUp();
			UIManager.myBarController.nextTimeToTouch = System.DateTime.Now.AddSeconds(0.5f);		
		}else{
			if(unoGamePlayData.CheckIsPlaying(DataManager.instance.userData.sessionId)){
				PopupManager.Instance.CreatePopupDialog(MyLocalize.GetString(MyLocalize.kWarning)
					, MyLocalize.GetString("System/AskForStandUp")
					, string.Empty
					, MyLocalize.GetString(MyLocalize.kYes)
					, MyLocalize.GetString(MyLocalize.kNo)
					, () =>{
						Uno_RealTimeAPI.instance.SendMessageStandUp();
						UIManager.myBarController.nextTimeToTouch = System.DateTime.Now.AddSeconds(0.5f);	
					}, null);
			}else{
				Uno_RealTimeAPI.instance.SendMessageStandUp();
				UIManager.myBarController.nextTimeToTouch = System.DateTime.Now.AddSeconds(0.5f);
			}
		}
	}

	public void OnButtonAtkUno(int _indexChair){
		if(unoGamePlayData == null){
            #if TEST
            Debug.LogError(">>> BUG LOGIC AtkUno (0)");
            #endif
			return;
		}
		if(unoGamePlayData.currentGameState != UnoGamePlayData.GameState.STATUS_PLAYING){
			#if TEST
            Debug.LogError(">>> BUG LOGIC AtkUno (1)");
            #endif
            return;
		}
		if(unoGamePlayData.listPlayerPlayingData == null || unoGamePlayData.listPlayerPlayingData.Count == 0){
			#if TEST
			Debug.LogError(">>> BUG LOGIC AtkUno (2)");
			#endif
			return;
		}
		int _indexCircle = -1;
		for(int i = 0; i < unoGamePlayData.listPlayerPlayingData.Count; i++){
			if(unoGamePlayData.listPlayerPlayingData[i].indexChair == _indexChair){
				_indexCircle = i;
				break;
			}
		}
		if(_indexCircle == -1){
			#if TEST
			Debug.LogError(">>> BUG LOGIC AtkUno (3)");
			#endif
			return;
		}
		if(System.DateTime.Now < UIManager.myBarController.nextTimeToTouch){
            #if TEST
            Debug.LogError(">>> Chưa được bấm");
            #endif
            return;
        }
        UIManager.myBarController.nextTimeToTouch = System.DateTime.Now.AddSeconds(0.5f);

		if(this.CanPlayMusicAndSfx()){
            MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
        }
		
		Uno_RealTimeAPI.instance.SendMessageAtkUno(_indexCircle);
	}

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
		StopAllCoroutines();
		UIManager.StopAllCoroutines();
		if(callbackManager.onDestructAllObject != null){
			callbackManager.onDestructAllObject();
		}
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
		MyAudioManager.instance.PlayMusic(myAudioInfo.bgm);

		unoGamePlayData.UpdateGoldAgain(DataManager.instance.userData.sessionId, DataManager.instance.userData.gold);
		UIManager.myPanelUserInfo.RefreshGoldInfo(true);

		if(unoGamePlayData.currentGameState == UnoGamePlayData.GameState.STATUS_WAIT_FOR_PLAYER){
			int _tmpIndexChair = unoGamePlayData.listSessionIdOnChair.IndexOf(DataManager.instance.userData.sessionId);
			if(_tmpIndexChair >= 0){
				listPlayerGroup[_tmpIndexChair].userData.gold = DataManager.instance.userData.gold;
				listPlayerGroup[_tmpIndexChair].panelPlayerInfo.RefreshGoldInfo(true);
			}
		}else{
			if(unoGamePlayData.CheckIsPlaying(DataManager.instance.userData.sessionId)){
				int _index = unoGamePlayData.listSessionIdPlaying.IndexOf(DataManager.instance.userData.sessionId);
				if(_index >= 0){
					int _indexChair = unoGamePlayData.listPlayerPlayingData[_index].indexChair;
					if(unoGamePlayData.listSessionIdOnChair[_indexChair] == DataManager.instance.userData.sessionId){
						listPlayerGroup[_indexChair].userData.gold = DataManager.instance.userData.gold;
						listPlayerGroup[_indexChair].panelPlayerInfo.RefreshGoldInfo(true);
					}
				}
			}
		}
	}

	private void OnDestroy() {
		StopAllCoroutines();
		unoGamePlayData = null;
		Uno_RealTimeAPI.SelfDestruction();
		instance = null;
	}
}

public class Uno_CallbackManager
{
	public System.Action onDestructAllObject;
    public System.Action onChangeNewTurn;
}

[System.Serializable] public class Uno_SortingLayerManager
{
   public MySortingLayerInfo sortingLayerInfo_GoldObject;
}

[System.Serializable]public class Uno_AudioInfo
{
	[Header("Playback")]
    public AudioClip bgm;
	[Header("Sfx")]
	public AudioClip sfx_DealCard;
	public AudioClip sfx_Card;
	public AudioClip sfx_CardForbiden;
	public AudioClip sfx_CardReverseTurn;
	public AudioClip sfx_WildCard;
	public AudioClip sfx_PublishGetMoreCards;
	public AudioClip sfx_BtnUnoOrAtkUnoAppear;
	public AudioClip sfx_CallUno;
	public AudioClip sfx_AtkUno_Charge;
	public AudioClip sfx_AtkUno_Hit;
	public AudioClip sfx_PanelChooseColorAppear;
	public AudioClip sfx_Notification;
	public AudioClip sfx_Bet;
	public AudioClip sfx_Fanfare;
	public AudioClip sfx_OpenScoreBoard;
	public AudioClip sfx_HidePanel;
	public AudioClip sfx_PopupChat;
}