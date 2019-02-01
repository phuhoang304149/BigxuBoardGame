using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Poker_GamePlay_Manager : IMySceneManager {
	public static Poker_GamePlay_Manager instance;

	public override Type mySceneType{
		get{
			return Type.PokerGamePlay;
		}
	}

	[Header("Manager")]
	public Canvas myCanvas;
	public Poker_UIManager UiManager;
	public ScreenChatController screenChat{get;set;}
	public PopupChatManager popupChatManager{get;set;}
	public GameObject iconNotificationChat;
	
	[Header("Data")]
	public PokerGamePlayData pokerGamePlayData;
	public List<Poker_PlayerGroup> listPlayerGroup;
	public List<CardDetail> listCardDetail;
	public Poker_SortingLayerManager sortingLayerManager;
	public Poker_CallbackManager callbackManager;

	[Header("Panel Container")]
	public Transform panelCardContainer;

	[Header("Setting")]
	public Vector2 sizeCard;

	[Header("Prefabs")]
	[SerializeField] GameObject screenChatPrefab;
	[SerializeField] GameObject popupChatManagerPrefab;
	[SerializeField] GameObject goldPrefab;
	[SerializeField] GameObject panelBonusGoldPrefab;

	[Header("Audio Info")]
    public Poker_AudioInfo myAudioInfo;

	public MySimplePoolManager globalCardsPoolManager;
	public MySimplePoolManager effectPoolManager;

	IEnumerator actionRunProcessPlaying, actionRunProcessNonPlaying, actionCheckFocusIconGetGold;
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

        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_GET_TABLE_INFO, (_mess)=>{
            pokerGamePlayData.InitDataWhenGetTableInfo(_mess);
			if(_mess.avaiable() > 0){
				#if TEST
				Debug.Log (">>> Chua doc het CMD : " + _mess.getCMDName ());
				#endif
			}
			UiManager.RefreshAllUINow();

			RegisterActionPlayerJoinGame();
            RegisterActionPlayerLeftGame();
			RegisterActionPlayerSitDown();
			RegisterActionPlayerStandUp();

			RegisterActionMeSitDown();

			RegisterActionGamePlayReady();
			RegisterActionGamePlayStartGame();

			RegisterActionSetBetFailed();
			RegisterActionChangeTurn();

			RegisterActionFinishGame();
           
            RegisterActionPlayerChat();
			RegisterActionPlayerAddGold();
			RegisterActionSetParentInfo();
			RegisterActionAlertUpdateServer();
        });
        NetworkGlobal.instance.instanceRealTime.ResumeReceiveMessage();
		
		yield return new WaitUntil(() => pokerGamePlayData != null && pokerGamePlayData.hasLoadTableInfo);
		yield return Yielders.Get(0.5f);

        canShowScene = true;

		MyAudioManager.instance.PlayMusic(myAudioInfo.bgm);

		if(pokerGamePlayData.currentGameState == PokerGamePlayData.GameState.STATUS_WAIT_FOR_PLAYER){
			if(pokerGamePlayData.nextTimeToStartGame > System.DateTime.Now){ // đang countdown start game
				System.TimeSpan _tmpDelta = pokerGamePlayData.nextTimeToStartGame - System.DateTime.Now;
				UiManager.ShowCountDownStartGame(_tmpDelta.TotalSeconds);
			}
		}else if(pokerGamePlayData.currentGameState == PokerGamePlayData.GameState.STATUS_FINISHGAME){
			#if TEST
			Debug.LogError(">>> Không setup cho trạng thái này");
			#endif
		}else{ // trường hợp đang chơi dở dang
			System.TimeSpan _tmpDelta = pokerGamePlayData.nextTimeToChangeCircle - System.DateTime.Now;
			double _timeCountDown = _tmpDelta.TotalSeconds;
			PokerGamePlayData.Poker_PlayerPlayingData _currentPlayerPlaying = pokerGamePlayData.listPlayerPlayingData[pokerGamePlayData.currentCircle];
			Poker_PlayerGroup _currentPlayerGroup = listPlayerGroup[_currentPlayerPlaying.indexChair];
			if(_currentPlayerGroup != null && _currentPlayerGroup.isInitialized){
				_currentPlayerGroup.panelPlayerInfo.StartCountDown(_timeCountDown, _timeCountDown);
			}
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
		callbackManager = new Poker_CallbackManager();
		pokerGamePlayData = new PokerGamePlayData();
		listProcessPlaying = new List<IEnumerator>();
		listProcessNonPlaying = new List<IEnumerator>();

		effectPoolManager = new MySimplePoolManager();
		globalCardsPoolManager = new MySimplePoolManager();

		screenChat = ((GameObject) Instantiate(screenChatPrefab, transform)).GetComponent<ScreenChatController>();
		popupChatManager = ((GameObject) Instantiate(popupChatManagerPrefab, transform)).GetComponent<PopupChatManager>();

		HideIconNotificationChat();

		UiManager.InitFirst();
		InitAllCallback();
	}

	void InitAllCallback(){
		callbackManager.onDestructAllObject = ()=>{
			effectPoolManager.ClearAllObjectsNow();
			globalCardsPoolManager.ClearAllObjectsNow();
			for(int i = 0; i < listPlayerGroup.Count; i++){
				if(listPlayerGroup[i].isInitialized){
					listPlayerGroup[i].ClearAllCards();
				}
			}
		};

		screenChat.onSendMessage = (_mess) =>
        {
            Poker_RealTimeAPI.instance.SendMessageChat(_mess);
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
            if(pokerGamePlayData != null){
                MyGamePlayData.AlertUpdateServer_Data _data = new MyGamePlayData.AlertUpdateServer_Data(_mess);
                System.TimeSpan _timeSpanRemain = _data.timeToUpdateServer - System.DateTime.Now;                
                PopupManager.Instance.CreateToast(string.Format(MyLocalize.GetString("System/Message_ServerMaintenance"), _timeSpanRemain.Minutes, _timeSpanRemain.Seconds));
            }
        });
    }
	void RegisterActionFinishGame(){
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_FINISH_GAME, (_mess) =>
        {
			if(pokerGamePlayData != null){
				pokerGamePlayData.SetDataWhenFinishGame(_mess);
				listProcessPlaying.Add(DoActionFinishGame());
			}
        });
	}
	void RegisterActionPlayerJoinGame(){
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_PLAYER_JOIN_GAME, (_mess) =>
        {
			if(pokerGamePlayData != null){
				pokerGamePlayData.SetUpUserJoinGame(_mess);
				listProcessNonPlaying.Add(DoActionPlayerJoinGame());
			}
        });
    }
    void RegisterActionPlayerLeftGame(){
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_PLAYER_LEFT_GAME, (_mess) =>
        {
			if(pokerGamePlayData != null){
				pokerGamePlayData.SetUpUserLeftGame(_mess);
				listProcessNonPlaying.Add(DoActionPlayerLeftGame());
			}
        });
    }
	void RegisterActionPlayerSitDown(){
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_PLAYER_SITDOWN, (_mess) =>
        {
			if(pokerGamePlayData != null){
				pokerGamePlayData.SetUpPlayerSitDown(_mess);
				listProcessNonPlaying.Add(DoActionCheckPlayerSitDown());
			}
        });
	}
	void RegisterActionPlayerStandUp(){
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_PLAYER_STANDUP, (_mess) =>
        {
			if(pokerGamePlayData != null){
				pokerGamePlayData.SetUpPlayerStandUp(_mess);
				listProcessNonPlaying.Add(DoActionCheckPlayerStandUp());
			}
        });
	}
	void RegisterActionMeSitDown(){
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_SITDOWN, (_mess) =>
        {
			if(pokerGamePlayData != null){
				pokerGamePlayData.SetUpMeSitDownFail(_mess);
				listProcessNonPlaying.Add(DoActionMeSitDownFail());
			}
		});
	}
	void RegisterActionPlayerChat(){
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_CHAT_IN_TABLE, (_mess) =>
        {   
			if(pokerGamePlayData != null){
				pokerGamePlayData.SetPlayerChatData(_mess);
				listProcessNonPlaying.Add(DoActionPlayerChat());
			}
        });
    }
	void RegisterActionGamePlayReady(){
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_READY, (_mess) =>
        {   
			if(pokerGamePlayData != null){
				pokerGamePlayData.SetDataWhenReady(_mess);
				listProcessPlaying.Add(DoActionGameReady());
			}
		});
	}
	void RegisterActionGamePlayStartGame(){
		// - Khi nhận được cmt này tức là start game đã trừ tiền 2 người chơi SB và BB
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_START_GAME, (_mess) => 
		{
			if(pokerGamePlayData != null){
				pokerGamePlayData.SetDataWhenStartGame(_mess);
				listProcessPlaying.Add(DoActionStartNewGame());
			}
		});
	}
	void RegisterActionChangeTurn(){
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_SERVER_CHANGE_TURN, (_mess) => 
		{
			if(pokerGamePlayData != null){
				pokerGamePlayData.SetDataWhenChangeTurn(_mess);
				listProcessPlaying.Add(DoActionChangeTurn());
			}
		});
	}
	void RegisterActionSetBetFailed(){
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_SETBET, (_mess) => 
		{
			PokerGamePlayData.GameState _gameState = (PokerGamePlayData.GameState) _mess.readByte();
			sbyte _circleCurrent = _mess.readByte();
   			long _maxBet = _mess.readLong(); 
			#if TEST
			Debug.LogError("Người đang chơi thứ " + _circleCurrent + " cược sai - maxbet = " + _maxBet);
			#endif
		});
	}

	void RegisterActionPlayerAddGold()
    {
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_onPlayerAddGold, (_mess) =>
        {
			if(pokerGamePlayData != null){
				pokerGamePlayData.SetPlayerAddGoldData(_mess);
				listProcessNonPlaying.Add(DoActionPlayerAddGold());
			}
        });
    }

	void RegisterActionSetParentInfo(){
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_SET_PARENT, (_mess) => 
		{
			if(pokerGamePlayData != null){
				pokerGamePlayData.SetDataWhenSetParent(_mess);
				listProcessNonPlaying.Add(DoActionPlayerSetParent());
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
			if(DataManager.instance.userData.gold < pokerGamePlayData.betDefault){
				UiManager.ShowArrowFocusGetGold();
			}else{
				UiManager.HideArrowFocusGetGold();
			}
			yield return Yielders.Get(1f);
		}
	}

	IEnumerator DoActionPlayerJoinGame(){
		PokerGamePlayData.PlayerJoinGame_Data _playerJoinGameData = pokerGamePlayData.processPlayerJoinGame[0];
		System.Action _onFinished = ()=>{
			_playerJoinGameData = null;
			pokerGamePlayData.processPlayerJoinGame.RemoveAt(0);
		};

		// ------- Check Logic ------- //
        if(pokerGamePlayData.listGlobalPlayerData[_playerJoinGameData.viewerId].sessionId >= 0){
            #if TEST
            Debug.LogError(">>> Chỗ này đã có người rồi: " + _playerJoinGameData.viewerId);
            #endif
			if(_onFinished != null){
				_onFinished();
			}
			yield break;
        }
        if(_playerJoinGameData.userData.sessionId != DataManager.instance.userData.sessionId){
            pokerGamePlayData.listGlobalPlayerData[_playerJoinGameData.viewerId] = _playerJoinGameData.userData;
			pokerGamePlayData.listSessionIdGlobalPlayer[_playerJoinGameData.viewerId] = _playerJoinGameData.sessionId;
            #if TEST
            Debug.Log(">>> Có người chơi " + pokerGamePlayData.listGlobalPlayerData[_playerJoinGameData.viewerId].nameShowInGame + " vào game tại vị trí " + _playerJoinGameData.viewerId);
            #endif
        }else{
			#if TEST
            Debug.LogError(">>> Trả session ID tào lao: " + _playerJoinGameData.sessionId);
            #endif
		}
		// ------------------------ //

		if(_onFinished != null){
			_onFinished();
		}
	}
	
	IEnumerator DoActionPlayerLeftGame(){
		PokerGamePlayData.PlayerLeftGame_Data _playerLeftGameData = pokerGamePlayData.processPlayerLeftGame[0];
		System.Action _onFinished = ()=>{
			_playerLeftGameData = null;
			pokerGamePlayData.processPlayerLeftGame.RemoveAt(0);
		};

		// ------- Check Logic ------- //
		if(_playerLeftGameData.sessionId == DataManager.instance.userData.sessionId){
			#if TEST
            Debug.LogError(">>> Trả session ID tào lao: " + _playerLeftGameData.sessionId);
            #endif
			if(_onFinished != null){
				_onFinished();
			}
			yield break;
		}
		int _index = pokerGamePlayData.listSessionIdGlobalPlayer.IndexOf(_playerLeftGameData.sessionId);
		if(_index < 0){
			#if TEST
			Debug.LogError(">>> Không tìm thấy session ID: " + _playerLeftGameData.sessionId);
			#endif
			if(_onFinished != null){
				_onFinished();
			}
			yield break;
		}
		// ------------------------ //

		#if TEST
		Debug.Log(">>> Có người chơi " + pokerGamePlayData.listGlobalPlayerData[_index].nameShowInGame + " thoát game tại vị trí " + _index);
		#endif
		pokerGamePlayData.listGlobalPlayerData[_index] = new UserDataInGame();
		pokerGamePlayData.listSessionIdGlobalPlayer[_index] = -1;
		if(_onFinished != null){
			_onFinished();
		}
	}

	IEnumerator DoActionCheckPlayerSitDown(){
		PokerGamePlayData.PlayerSitDown_Data _playerSitDownData = pokerGamePlayData.processPlayerSitDown[0];
		System.Action _onFinished = ()=>{
			_playerSitDownData = null;
			pokerGamePlayData.processPlayerSitDown.RemoveAt(0);
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

		if(pokerGamePlayData.listSessionIdGlobalPlayer == null || pokerGamePlayData.listSessionIdGlobalPlayer.Count == 0){
			#if TEST
            Debug.LogError(">>> listSessionIdGlobalPlayer is NULL");
            #endif
			if(_onFinished != null){
				_onFinished();
			}
			yield break;
		}

		if(pokerGamePlayData.listSessionIdOnChair == null || pokerGamePlayData.listSessionIdOnChair.Count == 0){
			#if TEST
			Debug.LogError(">>> listSessionIdOnChair is NULL");
			#endif
			if(_onFinished != null){
				_onFinished();
			}
			yield break;
		}
		if(_playerSitDownData.indexChair < 0 || _playerSitDownData.indexChair >= pokerGamePlayData.listSessionIdOnChair.Count){
			#if TEST
			Debug.LogError(">>> _indexChair out off range: " + _playerSitDownData.indexChair);
			#endif
			if(_onFinished != null){
				_onFinished();
			}
			yield break;
		}

		if(!pokerGamePlayData.listSessionIdGlobalPlayer.Contains(_playerSitDownData.sessionId)){
			#if TEST
            Debug.LogError(">>> Không tìm thấy (0): " + _playerSitDownData.sessionId);
            #endif
			if(_onFinished != null){
				_onFinished();
			}
			yield break;
		}

		if(pokerGamePlayData.listSessionIdOnChair[_playerSitDownData.indexChair] >= 0){
			#if TEST
			Debug.LogError(">>> Có người đang ĐỢI ngồi ngay tại vị trí " + _playerSitDownData.indexChair);
			#endif
			if(_onFinished != null){
				_onFinished();
			}
			yield break;
		}

		pokerGamePlayData.listSessionIdOnChair[_playerSitDownData.indexChair] = _playerSitDownData.sessionId;
		
		for(int i = 0; i < pokerGamePlayData.listPlayerPlayingData.Count; i++){
			if(pokerGamePlayData.listPlayerPlayingData[i].indexChair == _playerSitDownData.indexChair){
				if(pokerGamePlayData.currentGameState == PokerGamePlayData.GameState.STATUS_FINISHGAME){
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

	IEnumerator DoActionPlayerSitDown(PokerGamePlayData.PlayerSitDown_Data _playerSitDownData){
		UserDataInGame _userData = pokerGamePlayData.GetUserDataInGameFromListGlobal(_playerSitDownData.sessionId);
		if(_userData == null){
			#if TEST
			Debug.LogError(">>> Không tìm dc userdata in listGlobal: " + _playerSitDownData.sessionId + " - " + _playerSitDownData.indexChair);
			#endif
			yield break;
		}

		#if TEST
		Debug.Log(">>> " + _userData.nameShowInGame + " ngồi vào ghế " + _playerSitDownData.indexChair);
		#endif

		if(pokerGamePlayData.currentGameState == PokerGamePlayData.GameState.STATUS_WAIT_FOR_PLAYER){
			listPlayerGroup[_playerSitDownData.indexChair].InitData(_userData);

			if(!UiManager.IsCountingDown()){
				if(pokerGamePlayData.nextTimeToStartGame > System.DateTime.Now){ // đang countdown start game
					System.TimeSpan _tmpDelta = pokerGamePlayData.nextTimeToStartGame - System.DateTime.Now;
					UiManager.ShowCountDownStartGame(_tmpDelta.TotalSeconds);
				}
			}
		}else{
			listPlayerGroup[_playerSitDownData.indexChair].InitAsIncognito(_userData);
		}

		UiManager.RefreshUIButtonSitDown();
		if(pokerGamePlayData.CheckIfIsMe(_userData.sessionId)){
			UiManager.myBarController.RefreshUI();
		}
	}

	IEnumerator DoActionMeSitDownFail(){
		PokerGamePlayData.MeSitDownFail_Data _meSitDownFailData = pokerGamePlayData.processMeSitDownFail[0];
		System.Action _onFinished = ()=>{
			_meSitDownFailData = null;
			pokerGamePlayData.processMeSitDownFail.RemoveAt(0);
		};

		if(!_meSitDownFailData.isSuccess){
			pokerGamePlayData.UpdateGoldAgain(DataManager.instance.userData.sessionId, _meSitDownFailData.myGold);

			UiManager.myPanelUserInfo.RefreshGoldInfo(true);

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
		PokerGamePlayData.PlayerStandUp_Data _playerStandUpData = pokerGamePlayData.processPlayerStandUp[0];
		System.Action _onFinished = ()=>{
			_playerStandUpData = null;
			pokerGamePlayData.processPlayerStandUp.RemoveAt(0);
		};
		
		// ------- Check Logic ------- //
		if(_playerStandUpData.sessionId < 0){
			#if TEST
            Debug.LogError(">>> sessionId nhảm : " + _playerStandUpData.sessionId);
            #endif
			if(_onFinished != null){
				_onFinished();
			}
			yield break;
		}
		if(pokerGamePlayData.listSessionIdGlobalPlayer == null || pokerGamePlayData.listSessionIdGlobalPlayer.Count == 0){
			#if TEST
            Debug.LogError(">>> listSessionIdGlobalPlayer is NULL");
            #endif
			if(_onFinished != null){
				_onFinished();
			}
			yield break;
		}
		if(pokerGamePlayData.listSessionIdOnChair == null || pokerGamePlayData.listSessionIdOnChair.Count == 0){
			#if TEST
            Debug.LogError(">>> listSessionIdOnChair is NULL");
            #endif
			if(_onFinished != null){
				_onFinished();
			}
			yield break;
		}
		if(_playerStandUpData.indexChair < 0 || _playerStandUpData.indexChair >= pokerGamePlayData.listSessionIdOnChair.Count){
			#if TEST
            Debug.LogError(">>> Không tìm thấy (0): " + _playerStandUpData.indexChair);
            #endif
			if(_onFinished != null){
				_onFinished();
			}
			yield break;
		}
		if(!pokerGamePlayData.listSessionIdGlobalPlayer.Contains(_playerStandUpData.sessionId)){
			#if TEST
            Debug.LogError(">>> Không tìm thấy (1): " + _playerStandUpData.sessionId);
            #endif
			if(_onFinished != null){
				_onFinished();
			}
			yield break;
		}
		if(!pokerGamePlayData.listSessionIdOnChair.Contains(_playerStandUpData.sessionId)){
			#if TEST
            Debug.LogError(">>> Không tìm thấy (2): " + _playerStandUpData.sessionId);
            #endif
			if(_onFinished != null){
				_onFinished();
			}
			yield break;
		}
		if(pokerGamePlayData.listSessionIdOnChair[_playerStandUpData.indexChair] < 0){
			#if TEST
            Debug.LogError(">>> Không tìm thấy (3): " + _playerStandUpData.indexChair);
            #endif
			if(_onFinished != null){
				_onFinished();
			}
			yield break;
		}

		_playerStandUpData.isPlaying = false;
		int _tmpIndex = pokerGamePlayData.listSessionIdPlaying.IndexOf(_playerStandUpData.sessionId);
		if(_tmpIndex >= 0 && pokerGamePlayData.listSessionIdOnChair[pokerGamePlayData.listPlayerPlayingData[_tmpIndex].indexChair] >= 0){ 
			_playerStandUpData.isPlaying = true;
		}
		pokerGamePlayData.listSessionIdOnChair[_playerStandUpData.indexChair] = -1;

		if(pokerGamePlayData.currentGameState == PokerGamePlayData.GameState.STATUS_FINISHGAME){
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

	IEnumerator DoActionPlayerStandUp(PokerGamePlayData.PlayerStandUp_Data _playerStandUpData){
		if(_playerStandUpData.isPlaying){
			if(pokerGamePlayData.currentGameState == PokerGamePlayData.GameState.STATUS_WAIT_FOR_PLAYER){
				_playerStandUpData.isPlaying = false;
			}
		}
		
		// ------- Check Logic ------- //
		if(_playerStandUpData.isPlaying){			
			int _tmpIndex = pokerGamePlayData.listSessionIdPlaying.IndexOf(_playerStandUpData.sessionId);
			if(_tmpIndex >= 0){ 
				UserDataInGame _userDataPlaying = pokerGamePlayData.listPlayerPlayingData[_tmpIndex].userData;
				sbyte _indexChair = pokerGamePlayData.listPlayerPlayingData[_tmpIndex].indexChair;
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
			UserDataInGame _userData = pokerGamePlayData.GetUserDataInGameFromListGlobal(_playerStandUpData.sessionId);
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

		// ------ Xử lý ------ //
		if(pokerGamePlayData.currentGameState == PokerGamePlayData.GameState.STATUS_WAIT_FOR_PLAYER){
			listPlayerGroup[_playerStandUpData.indexChair].HideAndClear();
			int _tmpCount = 0;
			for(int i = 0; i < listPlayerGroup.Count; i++){
				if(listPlayerGroup[i].isInitialized){
					_tmpCount ++;
				}
			}
			if(_tmpCount < 2){
				UiManager.StopShowCountDownStartGame();
			}
		}else{
			if(!_playerStandUpData.isPlaying){
				listPlayerGroup[_playerStandUpData.indexChair].HideAndClear();
			}
		}
		UiManager.RefreshUIButtonSitDown();
		if(pokerGamePlayData.CheckIfIsMe(_playerStandUpData.sessionId)){
			UiManager.myBarController.RefreshUI();
			UiManager.panelSupport.ResetData();
		}
		// ------------------- //
	}

	IEnumerator DoActionGameReady(){
		if(pokerGamePlayData.currentGameState != PokerGamePlayData.GameState.STATUS_WAIT_FOR_PLAYER){
			#if TEST
			Debug.LogError("Not in State STATUS_WAIT_FOR_PLAYER: " + pokerGamePlayData.currentGameState.ToString());
			#endif
			pokerGamePlayData.processGameReadyData.RemoveAt(0);
			yield break;
		}

		// #if TEST
		// Debug.Log("<color=green> Start process game ready</color>");
		// #endif

		// --- Merge vào dữ liệu thật --- //
		PokerGamePlayData.Poker_GameReady_Data _readyGameData = pokerGamePlayData.processGameReadyData[0];
		pokerGamePlayData.nextTimeToStartGame = _readyGameData.nextTimeToStartGame;
		// ------------------------------ //

		System.TimeSpan _tmpDelta = pokerGamePlayData.nextTimeToStartGame - System.DateTime.Now;
		// Debug.Log(">>> " + _tmpDelta.TotalMilliseconds + " - " + pokerGamePlayData.nextTimeToStartGame.ToString());
		UiManager.ShowCountDownStartGame(_tmpDelta.TotalSeconds);

		_readyGameData = null;
		pokerGamePlayData.processGameReadyData.RemoveAt(0);
		// #if TEST
		// Debug.Log("<color=green> End process game ready: " + pokerGamePlayData.processGameReadyData.Count +"</color>");
		// #endif
	}

	IEnumerator DoActionStartNewGame(){
		//TODO: Start game
		// - stop count down start game
		// - Hiện vai trò bigBlind, smallBlind
		// - Trả cược cho lượt đầu tiên: smallBlind trả phân nửa cược, bigBlind trả full cược
		// - Refresh lại tiền cho người chơi
		// - Chia bài cho tất cả người chơi
		// - Cho countDown người cược tiếp theo

		if(pokerGamePlayData.currentGameState != PokerGamePlayData.GameState.STATUS_WAIT_FOR_PLAYER){
			#if TEST
			Debug.LogError("Not in State STATUS_WAIT_FOR_PLAYER: " + pokerGamePlayData.currentGameState.ToString());
			#endif
			pokerGamePlayData.processStartGameData.RemoveAt(0);
			yield break;
		}

		// #if TEST
		// Debug.Log("<color=green> Start process Start New Game</color>");
		// #endif

		pokerGamePlayData.currentGameState = PokerGamePlayData.GameState.STATUS_TURN_1_CHIA_BAI_NGUOI_CHOI;
		PokerGamePlayData.Poker_StartGame_Data _startGameData = pokerGamePlayData.processStartGameData[0];

		// --- Merge vào dữ liệu thật --- //
		for(int i = 0; i < _startGameData.listPlayerPlaying.Count; i++){
			sbyte _indexGlobal = (sbyte) pokerGamePlayData.listSessionIdGlobalPlayer.IndexOf(_startGameData.listPlayerPlaying[i].userData.sessionId);
			if(_indexGlobal < 0){
				#if TEST
				Debug.Log(_startGameData.listPlayerPlaying[i].userData.nameShowInGame + " đã out room trước đó");
				#endif
			}
			_startGameData.listPlayerPlaying[i].userData.index = _indexGlobal;
			pokerGamePlayData.UpdateGoldAgain(_startGameData.listPlayerPlaying[i].userData.sessionId, _startGameData.listPlayerPlaying[i].userData.gold);
		}
		pokerGamePlayData.totalBet = _startGameData.totalBet;
		pokerGamePlayData.listPlayerPlayingData = _startGameData.listPlayerPlaying;
		pokerGamePlayData.listSessionIdPlaying = _startGameData.listSessionIdPlaying;
		pokerGamePlayData.maxBet = _startGameData.betDefault;
		pokerGamePlayData.totalRaiseInTurn = 0;
		pokerGamePlayData.currentCircle = _startGameData.circleCurrent;
		pokerGamePlayData.nextTimeToChangeCircle = _startGameData.nextTimeToChangeCircle;
		if(pokerGamePlayData.globalCards == null){
			pokerGamePlayData.globalCards = new List<sbyte>();
		}else if(pokerGamePlayData.globalCards != null && pokerGamePlayData.globalCards.Count > 0){
			pokerGamePlayData.globalCards.Clear();
		}
		// ------------------------------ //
		
		// ---- Xử lý logic ---- //
		for(int i = 0; i < pokerGamePlayData.listPlayerPlayingData.Count; i++){
			int _indexChair = pokerGamePlayData.listPlayerPlayingData[i].indexChair;
			Poker_PlayerGroup _playerGroup = listPlayerGroup[_indexChair];
			if(!_playerGroup.isInitialized){
				#if TEST
				Debug.LogError("Lỗi chưa init player group hoặc đang start game mà thoát: " + pokerGamePlayData.listPlayerPlayingData[i].userData.nameShowInGame + " - " + pokerGamePlayData.listPlayerPlayingData[i].indexChair + " - " + pokerGamePlayData.listPlayerPlayingData[i].userData.sessionId);
				#endif
			}
			_playerGroup.InitData(pokerGamePlayData.listPlayerPlayingData[i].userData);

			if(pokerGamePlayData.listPlayerPlayingData[i].isMe
				&& pokerGamePlayData.listSessionIdOnChair[pokerGamePlayData.listPlayerPlayingData[i].indexChair] == pokerGamePlayData.listPlayerPlayingData[i].userData.sessionId){
				UiManager.myBarController.InitData(pokerGamePlayData.listPlayerPlayingData[i], (sbyte) i);
				UiManager.panelSupport.InitData(pokerGamePlayData.listPlayerPlayingData[i]);
				break;
			}
		}

		UiManager.StopShowCountDownStartGame();
		UiManager.RefreshUIIndexCircle();
		UiManager.RefreshUIPlayerRole();
		UiManager.RefreshUIPanelPlayerPlayingBet(true);
		UiManager.RefreshUIPanelPlayerPlayingInfo();

		for(int i = 0; i < pokerGamePlayData.listPlayerPlayingData.Count; i++){
			int _indexChair = pokerGamePlayData.listPlayerPlayingData[i].indexChair;
			Poker_PlayerGroup _playerGroup = Poker_GamePlay_Manager.instance.listPlayerGroup[_indexChair];
			UiManager.DealPlayerCard(_playerGroup);
			yield return Yielders.Get(0.1f);
		}
		UiManager.panelSupport.RefreshData(false);
		
		System.TimeSpan _tmpDelta = pokerGamePlayData.nextTimeToChangeCircle - System.DateTime.Now;
		double _timeCountDown = _tmpDelta.TotalSeconds;
		Poker_PlayerGroup _currentPlayerGroup = listPlayerGroup[pokerGamePlayData.listPlayerPlayingData[pokerGamePlayData.currentCircle].indexChair];
		if(_currentPlayerGroup != null){
			_currentPlayerGroup.panelPlayerInfo.StartCountDown(_timeCountDown, _timeCountDown);
		}else{
			Debug.LogError(">>> BUG logic circleCurrent: " + pokerGamePlayData.currentCircle);
		}
		UiManager.myBarController.RefreshUI();
		// --------------------- //

		_startGameData = null;
		pokerGamePlayData.processStartGameData.RemoveAt(0);
		// #if TEST
		// Debug.Log("<color=green> End process Start New Game: " + pokerGamePlayData.processStartGameData.Count +"</color>");
		// #endif
	}

	IEnumerator DoActionChangeTurn(){
		//TODO: Change Turn
		//	- Stop count down của người vừa cược
		// 	- Cập nhật lại trạng thái của tất cả người đang chơi
		//	- Cập nhật lại tiền cược của họ
		//	- Nếu có người raise thì setup circle mới
		//	- Nếu người vừa cược là người cuối cùng của vòng thì: 
		//		+ Cập nhật lại tổng cược
		//		+ Reset tất cả tiền cược của người chơi trong turn
		//		+ Chia thêm lá mới
		//	- Cho countdown người mới
		//	- Refresh lại mybar

		// #if TEST
		// Debug.Log("<color=green> Start process Change Turn</color>");
		// #endif

		PokerGamePlayData.Poker_PlayerChangeTurn _playerChangeTurnData = pokerGamePlayData.processPlayerChangeTurnData[0];

		// --- Merge vào dữ liệu thật --- //
		PokerGamePlayData.Poker_PlayerPlayingData _lastPlayerData = pokerGamePlayData.listPlayerPlayingData[_playerChangeTurnData.lastPlayer_IndexCircle];

		bool _hasNewTurn = false;
		bool _hasNewCircle = false;
		if((sbyte) _playerChangeTurnData.currentGameState > (sbyte) pokerGamePlayData.currentGameState){
			_hasNewTurn = true;
		}else if((sbyte) _playerChangeTurnData.currentGameState < (sbyte) pokerGamePlayData.currentGameState){
			#if TEST
			Debug.LogError("BUG logic _newGameState: " + _playerChangeTurnData.currentGameState + " (" + (int) _playerChangeTurnData.currentGameState+")" + " | " + pokerGamePlayData.currentGameState + " (" + (int) pokerGamePlayData.currentGameState+")");
			#endif
		}else{
			if(_playerChangeTurnData.lastPlayer_CurrentState == PokerGamePlayData.Poker_PlayerPlayingData.State.STATEPOKER_RAISE
				|| _playerChangeTurnData.lastPlayer_CurrentState == PokerGamePlayData.Poker_PlayerPlayingData.State.STATEPOKER_ALLIN){
				if(_playerChangeTurnData.lastPlayer_TotalBet > pokerGamePlayData.maxBet){
					pokerGamePlayData.totalRaiseInTurn ++;
					_hasNewCircle = true;
				}
			}
		}

		List<sbyte> _newGlobalCards = new List<sbyte>();
		if(_hasNewTurn){
			for(int i = pokerGamePlayData.globalCards.Count; i < _playerChangeTurnData.globalCards.Count; i++){
				_newGlobalCards.Add(_playerChangeTurnData.globalCards[i]);
			}
		}

		_lastPlayerData.currentState = _playerChangeTurnData.lastPlayer_CurrentState;
		pokerGamePlayData.globalCards = _playerChangeTurnData.globalCards;

		_lastPlayerData.userData.gold = _playerChangeTurnData.lastPlayer_GoldRemain;
		_lastPlayerData.turnBet += _playerChangeTurnData.lastPlayer_CircleBet;
		_lastPlayerData.totalBet = _playerChangeTurnData.lastPlayer_TotalBet;
		pokerGamePlayData.totalBet += _playerChangeTurnData.lastPlayer_CircleBet;
		
		// if(listPlayerPlayingData[playerChangeTurnData.lastPlayer_IndexCircle].isMe){
		// 	Debugs.LogGreen(">>> Change turn" + listPlayerPlayingData[playerChangeTurnData.lastPlayer_IndexCircle].userData.gold + " | " + listPlayerPlayingData[playerChangeTurnData.lastPlayer_IndexCircle].turnBet + " | " + listPlayerPlayingData[playerChangeTurnData.lastPlayer_IndexCircle].totalBet + " | " + totalBet);
		// }
		
		if(_hasNewCircle){
			for(int i = 0; i < pokerGamePlayData.listPlayerPlayingData.Count; i ++){
				if(i != _playerChangeTurnData.lastPlayer_IndexCircle){
					if(pokerGamePlayData.listPlayerPlayingData[i].currentState != PokerGamePlayData.Poker_PlayerPlayingData.State.STATEPOKER_FOLD
						&& pokerGamePlayData.listPlayerPlayingData[i].currentState != PokerGamePlayData.Poker_PlayerPlayingData.State.STATEPOKER_ALLIN){
						pokerGamePlayData.listPlayerPlayingData[i].currentState = PokerGamePlayData.Poker_PlayerPlayingData.State.None;
					}
				}
			}
		}

		pokerGamePlayData.UpdateGoldAgain(_lastPlayerData.userData.sessionId, _playerChangeTurnData.lastPlayer_GoldRemain);
		pokerGamePlayData.maxBet = _playerChangeTurnData.currentMaxBet;
		pokerGamePlayData.currentCircle = _playerChangeTurnData.currentPlayer_IndexCircle;
		pokerGamePlayData.currentGameState = _playerChangeTurnData.currentGameState;
		pokerGamePlayData.nextTimeToChangeCircle = _playerChangeTurnData.nextTimeToChangeCircle;
		// ------------------------------ //

		// --- Xử lý Logic --- //
		int _tmpLastIndex = _lastPlayerData.indexChair;
		listPlayerGroup[_tmpLastIndex].panelPlayerInfo.StopCountDown();
		listPlayerGroup[_tmpLastIndex].ShowPlayerState(_lastPlayerData.currentState);
		if(_lastPlayerData.currentState != PokerGamePlayData.Poker_PlayerPlayingData.State.STATEPOKER_FOLD){
			if(_lastPlayerData.turnBet > 0){
				if(this.CanPlayMusicAndSfx()){
					MyAudioManager.instance.PlaySfx(myAudioInfo.sfx_Bet);
				}
				listPlayerGroup[_tmpLastIndex].panelPlayerInfo.RefreshGoldInfo();
				listPlayerGroup[_tmpLastIndex].myPanelBet.SetBet(_lastPlayerData.turnBet);
				listPlayerGroup[_tmpLastIndex].myPanelBet.Show();
			}else{
				if(this.CanPlayMusicAndSfx()){
					MyAudioManager.instance.PlaySfx(myAudioInfo.sfx_Check);
				}
				listPlayerGroup[_tmpLastIndex].myPanelBet.Hide();
			}
		}else{
			if(this.CanPlayMusicAndSfx()){
				MyAudioManager.instance.PlaySfx(myAudioInfo.sfx_Fold);
			}
			if(listPlayerGroup[_tmpLastIndex].isMe){
				for(int i = 0; i < listPlayerGroup[_tmpLastIndex].ownCardPoolManager.listObjects.Count; i++){
					((PanelCardDetailController) listPlayerGroup[_tmpLastIndex].ownCardPoolManager.listObjects[i]).SetUpShadow(true);
				}
			}else{
				listPlayerGroup[_tmpLastIndex].ClearAllCards();
			}
		}

		if(_hasNewTurn){
			yield return Yielders.Get(0.5f);
			pokerGamePlayData.ResetNewTurn();
			for(int i = 0; i < pokerGamePlayData.listPlayerPlayingData.Count; i++){
				int _indexChair = pokerGamePlayData.listPlayerPlayingData[i].indexChair;
				listPlayerGroup[_indexChair].myPanelBet.Hide();
				if(pokerGamePlayData.listPlayerPlayingData[i].currentState == PokerGamePlayData.Poker_PlayerPlayingData.State.STATEPOKER_FOLD
					|| pokerGamePlayData.listPlayerPlayingData[i].currentState == PokerGamePlayData.Poker_PlayerPlayingData.State.STATEPOKER_ALLIN){
					continue;
				}
				listPlayerGroup[_indexChair].ShowPlayerState(pokerGamePlayData.listPlayerPlayingData[i].currentState);
			}
			UiManager.SetGlobalBet(pokerGamePlayData.totalBet);
			
			yield return UiManager.DealGlobalCard(_newGlobalCards);
		}else if(_hasNewCircle){
			for(int i = 0; i < pokerGamePlayData.listPlayerPlayingData.Count; i++){
				if(i != _playerChangeTurnData.lastPlayer_IndexCircle){
					if(pokerGamePlayData.listPlayerPlayingData[i].currentState == PokerGamePlayData.Poker_PlayerPlayingData.State.STATEPOKER_FOLD
						|| pokerGamePlayData.listPlayerPlayingData[i].currentState == PokerGamePlayData.Poker_PlayerPlayingData.State.STATEPOKER_ALLIN){
						continue;
					}
					int _indexChair = pokerGamePlayData.listPlayerPlayingData[i].indexChair;
					listPlayerGroup[_indexChair].ShowPlayerState(pokerGamePlayData.listPlayerPlayingData[i].currentState);
				}
			}
		}
		
		PokerGamePlayData.Poker_PlayerPlayingData _currentPlayer = pokerGamePlayData.listPlayerPlayingData[pokerGamePlayData.currentCircle];
		int _tmpCurrentIndex = _currentPlayer.indexChair;
		System.TimeSpan _tmpDelta = pokerGamePlayData.nextTimeToChangeCircle - System.DateTime.Now;
		double _timeCountDown = _tmpDelta.TotalSeconds;
		listPlayerGroup[_tmpCurrentIndex].panelPlayerInfo.StartCountDown(_timeCountDown, _timeCountDown);
	
		UiManager.myBarController.RefreshUI();
		UiManager.panelSupport.RefreshData(_hasNewTurn);
		// ------------------------------ //

		_playerChangeTurnData = null;
		pokerGamePlayData.processPlayerChangeTurnData.RemoveAt(0);
		// #if TEST
		// Debug.Log("<color=green> End process Change Turn: " + pokerGamePlayData.processPlayerChangeTurnData.Count +"</color>");
		// #endif
	}

	IEnumerator DoActionFinishGame(){
		//TODO: Finish Game
		//	- Stop count down của người vừa cược
		// 	- Cập nhật lại trạng thái của người vừa cược
		//	- Cập nhật lại tiền cược của người cuối cùng + tiền đang có
		// 	- Delay 1 thời gian
		//	- Hide hết trạng thái người chơi trừ ai FOLD
		//  - Show bài + Hiện Shadow tất cả lá bài trên bàn
		// 	- Show kết quả lần lượt trong list người thắng bài (trường hợp đã chia đủ lá bài)
		//		+ Highlight bài mạnh nhất của người thắng
		//	- Chia tiền
		//	- Cập nhật Data
		//	- Cập nhật lại tiền đang có
		//	- Cập nhật lại danh sách người chơi trên bàn
		// 	- Reset List đang chơi
		//	- Cho countdown chờ game mới
		//	- Refresh lại mybard

		// #if TEST
		// Debug.Log("<color=green> Start process Finish Game</color>");
		// #endif

		pokerGamePlayData.currentGameState = PokerGamePlayData.GameState.STATUS_FINISHGAME;
		PokerGamePlayData.Poker_FinishGame _finishGameData = pokerGamePlayData.processFinishGameData[0];

		if(pokerGamePlayData.listPlayerPlayingData.Count != _finishGameData.listTemporaryPlayer.Count){
			#if TEST
			Debug.LogError("Lỗi đồng bộ dữ liệu từ sv: " + pokerGamePlayData.listPlayerPlayingData.Count + "|" +  _finishGameData.listTemporaryPlayer.Count);
			#endif
			yield break;
		}

		// --- Cập nhật 1 vài thông số vào dữ liệu thật --- //
		for(int i = 0; i < pokerGamePlayData.listPlayerPlayingData.Count; i++){
			pokerGamePlayData.listPlayerPlayingData[i].turnBet = _finishGameData.listTemporaryPlayer[i].turnBet;
			pokerGamePlayData.listPlayerPlayingData[i].totalBet = _finishGameData.listTemporaryPlayer[i].totalBet;
			pokerGamePlayData.listPlayerPlayingData[i].goldWinOrReturn = _finishGameData.listTemporaryPlayer[i].goldWinOrReturn;
			if(pokerGamePlayData.currentCircle == i){
				// Debugs.LogGreen(">>> Finish: " + pokerGamePlayData.listPlayerPlayingData[i].turnBet + " | " + pokerGamePlayData.listPlayerPlayingData[i].totalBet);
				pokerGamePlayData.listPlayerPlayingData[i].currentState = (PokerGamePlayData.Poker_PlayerPlayingData.State) _finishGameData.listTemporaryPlayer[i].currentState;
				pokerGamePlayData.listPlayerPlayingData[i].userData.gold -= _finishGameData.listTemporaryPlayer[i].circleBet;
				pokerGamePlayData.UpdateGoldAgain(pokerGamePlayData.listPlayerPlayingData[i].userData.sessionId, pokerGamePlayData.listPlayerPlayingData[i].userData.gold);
			}

			if(pokerGamePlayData.listPlayerPlayingData[i].currentState != PokerGamePlayData.Poker_PlayerPlayingData.State.STATEPOKER_FOLD){
				pokerGamePlayData.listPlayerPlayingData[i].ownCards[0] = _finishGameData.listTemporaryPlayer[i].card1;
				pokerGamePlayData.listPlayerPlayingData[i].ownCards[1] = _finishGameData.listTemporaryPlayer[i].card2;
			}
		}
		// -- Check trường hợp chia xong 5 lá và tất cả đều fold, chỉ có 1 người trên bàn
		int _tmpCountFold = 0;
		for(int i = 0; i < pokerGamePlayData.listPlayerPlayingData.Count; i++){
			if(pokerGamePlayData.listPlayerPlayingData[i].currentState == PokerGamePlayData.Poker_PlayerPlayingData.State.STATEPOKER_FOLD){
				_tmpCountFold ++;
				continue;
			}
		}
		bool _canShowCards = false;
		List<sbyte> _newGlobalCards = new List<sbyte>(); 
		if(_tmpCountFold != pokerGamePlayData.listPlayerPlayingData.Count - 1){
			_canShowCards = true;
			if(pokerGamePlayData.globalCards.Count != _finishGameData.listFullGlobalCards.Count){
				for(int i = pokerGamePlayData.globalCards.Count; i < _finishGameData.listFullGlobalCards.Count; i++){
					_newGlobalCards.Add(_finishGameData.listFullGlobalCards[i]);
				}
			}
			pokerGamePlayData.globalCards = _finishGameData.listFullGlobalCards;

			for(int i = 0; i < pokerGamePlayData.listPlayerPlayingData.Count; i++){
				if(pokerGamePlayData.listPlayerPlayingData[i].currentState != PokerGamePlayData.Poker_PlayerPlayingData.State.STATEPOKER_FOLD){
					PokerGamePlayData.CheckResultCard(pokerGamePlayData.listPlayerPlayingData[i].ownCards, pokerGamePlayData.globalCards, (_typeCardResult, _cardHighLight)=>{
						pokerGamePlayData.listPlayerPlayingData[i].typeCardResult = _typeCardResult;
						pokerGamePlayData.listPlayerPlayingData[i].highLightCardsResult = _cardHighLight;
					});
				}
			}

			pokerGamePlayData.historyData = new PokerGamePlayData.Poker_HistoryData();
			for(int i = 0; i < _finishGameData.listPlayerWin.Count; i ++){
				pokerGamePlayData.historyData.circleIndexWin.Add(_finishGameData.listPlayerWin[i].circleIndexWin);
			}
			for(int i = 0; i < pokerGamePlayData.globalCards.Count; i++){
				pokerGamePlayData.historyData.globalCards.Add(pokerGamePlayData.globalCards[i]);
			}
			for(int i = 0; i < pokerGamePlayData.listPlayerPlayingData.Count; i++){
				pokerGamePlayData.historyData.listPlayerPlayingData.Add(pokerGamePlayData.listPlayerPlayingData[i]);
			}
		}
		// ------------------------------ //


		UiManager.myBarController.RefreshUI();
		PokerGamePlayData.Poker_PlayerPlayingData _lastPlayer = pokerGamePlayData.listPlayerPlayingData[_finishGameData.lastPlayer_IndexCircle];
		int _tmpLastIndex = _lastPlayer.indexChair;
		listPlayerGroup[_tmpLastIndex].panelPlayerInfo.StopCountDown();
		listPlayerGroup[_tmpLastIndex].ShowPlayerState(_lastPlayer.currentState);
		if(_lastPlayer.currentState != PokerGamePlayData.Poker_PlayerPlayingData.State.STATEPOKER_FOLD){
			if(_lastPlayer.turnBet > 0){
				if(this.CanPlayMusicAndSfx()){
					MyAudioManager.instance.PlaySfx(myAudioInfo.sfx_Bet);
				}
				listPlayerGroup[_tmpLastIndex].panelPlayerInfo.RefreshGoldInfo();
				listPlayerGroup[_tmpLastIndex].myPanelBet.SetBet(_lastPlayer.turnBet);
				listPlayerGroup[_tmpLastIndex].myPanelBet.Show();
			}else{
				if(this.CanPlayMusicAndSfx()){
					MyAudioManager.instance.PlaySfx(myAudioInfo.sfx_Check);
				}
				listPlayerGroup[_tmpLastIndex].myPanelBet.Hide();
			}
		}else{
			if(this.CanPlayMusicAndSfx()){
				MyAudioManager.instance.PlaySfx(myAudioInfo.sfx_Fold);
			}
			if(listPlayerGroup[_tmpLastIndex].isMe){
				for(int i = 0; i < listPlayerGroup[_tmpLastIndex].ownCardPoolManager.listObjects.Count; i++){
					((PanelCardDetailController) listPlayerGroup[_tmpLastIndex].ownCardPoolManager.listObjects[i]).SetUpShadow(true);
				}
			}else{
				listPlayerGroup[_tmpLastIndex].ClearAllCards();
			}
		}
		
		yield return Yielders.Get(1f);

		// --- Merge data tạm còn lại vào data thật --- //
		AchievementDetail _achievementDetail = DataManager.instance.achievementData.GetAchievementDetail(IMiniGameInfo.Type.Poker);
		if(_achievementDetail == null){
			#if TEST
			Debug.LogError(">>> _achievementDetail is null");
			#endif
		}
		for(int i = 0; i < _finishGameData.listTemporaryPlayer.Count; i++){
			pokerGamePlayData.listPlayerPlayingData[i].userData.gold = _finishGameData.listTemporaryPlayer[i].goldRemain;
			UserDataInGame _userData = pokerGamePlayData.GetUserDataInGameFromListGlobal(pokerGamePlayData.listPlayerPlayingData[i].userData.sessionId);
			if(_userData != null){
				_userData.gold = pokerGamePlayData.listPlayerPlayingData[i].userData.gold;
				_userData.lose = _finishGameData.listTemporaryPlayer[i].achievementLose;
				if(pokerGamePlayData.listPlayerPlayingData[i].isMe){
					DataManager.instance.userData.gold = pokerGamePlayData.listPlayerPlayingData[i].userData.gold;
					if(_achievementDetail != null){
						_achievementDetail.countLose = _finishGameData.listTemporaryPlayer[i].achievementLose;
					}
				}
			}else{
				#if TEST
				Debug.Log(">>> Người này đã out room");
				#endif
			}
		}
		for(int i = 0; i < _finishGameData.listPlayerWin.Count; i++){
			PokerGamePlayData.Poker_PlayerPlayingData _playerData = pokerGamePlayData.listPlayerPlayingData[_finishGameData.listPlayerWin[i].circleIndexWin];
			UserDataInGame _userData = pokerGamePlayData.GetUserDataInGameFromListGlobal(_playerData.userData.sessionId);
			if(_userData != null){
				_userData.win = _finishGameData.listPlayerWin[i].achievementWin;
				if(_playerData.isMe){
					if(_achievementDetail != null){
						_achievementDetail.countWin = _finishGameData.listPlayerWin[i].achievementWin;
					}
				}
			}else{
				#if TEST
				Debug.Log(">>> Người này đã out room");
				#endif
			}
		}
		// ------------------------------------ //
		for(int i = 0; i < pokerGamePlayData.listPlayerPlayingData.Count; i++){
			if(pokerGamePlayData.listPlayerPlayingData[i].currentState == PokerGamePlayData.Poker_PlayerPlayingData.State.STATEPOKER_FOLD){
				continue;
			}
			int _indexChair = pokerGamePlayData.listPlayerPlayingData[i].indexChair;
			listPlayerGroup[_indexChair].myPanelStatus.Hide();
		}
		if(_canShowCards){
			// -- Trường hợp chia không đủ lá trên bàn  -- //
			UiManager.SetGlobalBet(pokerGamePlayData.totalBet);
			if(_newGlobalCards.Count > 0){
				yield return UiManager.DealGlobalCard(_newGlobalCards);
				UiManager.panelSupport.RefreshData(true);
				yield return Yielders.Get(0.5f);
			}else{
				UiManager.panelSupport.RefreshData(false);
			}
			// ------------------------------------------- //
			
			if(Poker_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
				MyAudioManager.instance.PlaySfx(myAudioInfo.sfx_Card);
			}
			for(int i = 0; i < pokerGamePlayData.listPlayerPlayingData.Count; i++){
				if(pokerGamePlayData.listPlayerPlayingData[i].currentState == PokerGamePlayData.Poker_PlayerPlayingData.State.STATEPOKER_FOLD){
					continue;
				}
				sbyte _indexChair = pokerGamePlayData.listPlayerPlayingData[i].indexChair;
				UiManager.ShowCard(pokerGamePlayData.listPlayerPlayingData[i].ownCards, listPlayerGroup[_indexChair]);
			}
			
			List<Poker_PlayerGroup> _listPlayerGroupNeedToShadow = new List<Poker_PlayerGroup>();
			List<PanelCardDetailController> _listCardNeedToShadow = new List<PanelCardDetailController>();
			List<PanelCardDetailController> _listCardNeedToHighlight = new List<PanelCardDetailController>();
			for(int i = 0; i < globalCardsPoolManager.listObjects.Count; i++){
				_listCardNeedToShadow.Add((PanelCardDetailController) globalCardsPoolManager.listObjects[i]);
			}
			
			for(int i = 0; i < pokerGamePlayData.listPlayerPlayingData.Count; i++){
				if(pokerGamePlayData.listPlayerPlayingData[i].currentState == PokerGamePlayData.Poker_PlayerPlayingData.State.STATEPOKER_FOLD){
					continue;
				}
				sbyte _indexChair = pokerGamePlayData.listPlayerPlayingData[i].indexChair;
				for(int j = 0; j < listPlayerGroup[_indexChair].ownCardPoolManager.listObjects.Count; j++){
					_listCardNeedToShadow.Add((PanelCardDetailController) listPlayerGroup[_indexChair].ownCardPoolManager.listObjects[j]);
				}
				_listPlayerGroupNeedToShadow.Add(listPlayerGroup[_indexChair]);
			}
			
			for(int i = 0; i < _finishGameData.listPlayerWin.Count; i++){
				PokerGamePlayData.Poker_PlayerPlayingData _playerData = pokerGamePlayData.listPlayerPlayingData[_finishGameData.listPlayerWin[i].circleIndexWin];
				sbyte _indexChair = _playerData.indexChair;
				_listPlayerGroupNeedToShadow.Remove(listPlayerGroup[_indexChair]);

				// string _tmpaaa = "";
				// for(int j = 0; j < _playerData.highLightCardsResult.Count; j++){
				// 	_tmpaaa += _playerData.highLightCardsResult[j] + "|";
				// }
				// Debug.Log(_playerData.userData.nameShowInGame + " - " + _tmpaaa);

				for(int j = 0; j < _playerData.highLightCardsResult.Count; j++){
					bool _tmpFlag = false;
					for(int m = 0; m < pokerGamePlayData.globalCards.Count; m ++){
						if(pokerGamePlayData.globalCards[m] == _playerData.highLightCardsResult[j]){
							_listCardNeedToShadow.Remove((PanelCardDetailController) globalCardsPoolManager.listObjects[m]);
							_listCardNeedToHighlight.Add((PanelCardDetailController) globalCardsPoolManager.listObjects[m]);
							_tmpFlag = true;
							break;
						}
					}
					if(!_tmpFlag){
						for(int m = 0; m < _playerData.ownCards.Count; m ++){
							if(_playerData.ownCards[m] == _playerData.highLightCardsResult[j]){
								_listCardNeedToShadow.Remove((PanelCardDetailController) listPlayerGroup[_indexChair].ownCardPoolManager.listObjects[m]);
								_listCardNeedToHighlight.Add((PanelCardDetailController) listPlayerGroup[_indexChair].ownCardPoolManager.listObjects[m]);
								break;
							}
						}
					}
				}
			}
			
			List<int> _tmpListA = new List<int>();
			for(int i = 0; i < _finishGameData.listPlayerWin.Count; i++){
				PokerGamePlayData.Poker_PlayerPlayingData _playerData = pokerGamePlayData.listPlayerPlayingData[_finishGameData.listPlayerWin[i].circleIndexWin];				
				for(int m = 0; m < _playerData.ownCards.Count; m ++){
					for(int j = 0; j < _playerData.highLightCardsResult.Count; j++){
						if(_playerData.ownCards[m] == _playerData.highLightCardsResult[j]){
							_tmpListA.Add(j);
							break;
						}
					}
				}
			}

			for(int i = 0; i < _listCardNeedToShadow.Count; i++){
				_listCardNeedToShadow[i].SetUpShadow(true);
			}
			for(int i = 0; i < _listPlayerGroupNeedToShadow.Count; i++){
				_listPlayerGroupNeedToShadow[i].panelPlayerInfo.SetShadow(true);
			}
			bool _tmpFinished = false;

			if(Poker_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
				MyAudioManager.instance.PlaySfx(myAudioInfo.sfx_HighlighCard);
			}

			for(int i = 0; i < _listCardNeedToHighlight.Count; i++){
				_listCardNeedToHighlight[i].transform.SetAsLastSibling();
				_listCardNeedToHighlight[i].SetUpLoopHighlight(()=>{_tmpFinished = true;});
			}
			for(int i = 0; i < _tmpListA.Count; i++){
				_listCardNeedToHighlight[_tmpListA[i]].transform.SetAsLastSibling();
			}

			UiManager.panelTypeCardResult.Show(PokerGamePlayData.GetStringTypeCardResult(pokerGamePlayData.listPlayerPlayingData[_finishGameData.listPlayerWin[0].circleIndexWin].typeCardResult));

			for(int i = 0; i < pokerGamePlayData.listPlayerPlayingData.Count; i++){
				if(pokerGamePlayData.listPlayerPlayingData[i].goldWinOrReturn > 0){
					sbyte _indexChair = pokerGamePlayData.listPlayerPlayingData[i].indexChair;
					
					Vector2 _tmpStartPoint = UiManager.txtTotalBet.transform.position;
					Vector2 _tmpEndPoint = listPlayerGroup[_indexChair].panelPlayerInfo.transform.position;
					
					StartCoroutine(MyConstant.DoActionShowPopupWinGold(panelBonusGoldPrefab, effectPoolManager, myCanvas.transform
						, _tmpEndPoint, 1.1f, pokerGamePlayData.listPlayerPlayingData[i].goldWinOrReturn, ()=>{
							if(listPlayerGroup[_indexChair].isInitialized){
								listPlayerGroup[_indexChair].panelPlayerInfo.RefreshGoldInfo();
							}
						}));
					StartCoroutine(MyConstant.DoActionShowEffectGoldFly(goldPrefab, effectPoolManager, sortingLayerManager.sortingLayerInfo_GoldObject
						, _tmpStartPoint, _tmpEndPoint, 10, 1f, 0.8f, ()=>{
							if(Poker_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
								MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Gold);
							}
						}));
				}
			}

			yield return new WaitUntil(()=>_tmpFinished);
		}else{
			if(_finishGameData.listPlayerWin.Count > 1){
				#if TEST
				Debug.LogError(">>> BUG logic listPlayerWin.count : " + _finishGameData.listPlayerWin.Count);
				#endif
			}
			List<Poker_PlayerGroup> _listPlayerGroupNeedToShadow = new List<Poker_PlayerGroup>();
			for(int i = 0; i < pokerGamePlayData.listPlayerPlayingData.Count; i++){
				if(pokerGamePlayData.listPlayerPlayingData[i].currentState == PokerGamePlayData.Poker_PlayerPlayingData.State.STATEPOKER_FOLD){
					continue;
				}
				sbyte _indexChair = pokerGamePlayData.listPlayerPlayingData[i].indexChair;
				_listPlayerGroupNeedToShadow.Add(listPlayerGroup[_indexChair]);
			}
			for(int i = 0; i < _finishGameData.listPlayerWin.Count; i++){
				PokerGamePlayData.Poker_PlayerPlayingData _playerData = pokerGamePlayData.listPlayerPlayingData[_finishGameData.listPlayerWin[i].circleIndexWin];
				sbyte _indexChair = _playerData.indexChair;
				_listPlayerGroupNeedToShadow.Remove(listPlayerGroup[_indexChair]);
			}
			for(int i = 0; i < _listPlayerGroupNeedToShadow.Count; i++){
				_listPlayerGroupNeedToShadow[i].panelPlayerInfo.SetShadow(true);
			}

			for(int i = 0; i < pokerGamePlayData.listPlayerPlayingData.Count; i++){
				if(pokerGamePlayData.listPlayerPlayingData[i].goldWinOrReturn > 0){
					sbyte _indexChair = pokerGamePlayData.listPlayerPlayingData[i].indexChair;
					
					Vector2 _tmpStartPoint = UiManager.txtTotalBet.transform.position;
					Vector2 _tmpEndPoint = listPlayerGroup[_indexChair].panelPlayerInfo.transform.position;
					
					StartCoroutine(MyConstant.DoActionShowPopupWinGold(panelBonusGoldPrefab, effectPoolManager, myCanvas.transform
						, _tmpEndPoint, 1.1f, pokerGamePlayData.listPlayerPlayingData[i].goldWinOrReturn, ()=>{
							if(listPlayerGroup[_indexChair].isInitialized){
								listPlayerGroup[_indexChair].panelPlayerInfo.RefreshGoldInfo();
							}
						}));
					StartCoroutine(MyConstant.DoActionShowEffectGoldFly(goldPrefab, effectPoolManager, sortingLayerManager.sortingLayerInfo_GoldObject
						, _tmpStartPoint, _tmpEndPoint, 10, 1f, 0.8f, ()=>{
							if(Poker_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
								MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Gold);
							}
						}));
				}
			}
		}
		yield return Yielders.Get(2f);
		UiManager.SetGlobalBet(0);
		UiManager.panelTypeCardResult.Hide();
		yield return UiManager.ClearAllCards();
		UiManager.panelTotalBet.gameObject.SetActive(false);
		pokerGamePlayData.totalRaiseInTurn = 0;
		pokerGamePlayData.totalBet = 0;
		pokerGamePlayData.listPlayerPlayingData.Clear();
		pokerGamePlayData.listSessionIdPlaying.Clear();
		pokerGamePlayData.globalCards.Clear();

		for(int i = 0; i < listPlayerGroup.Count; i++){
			if(!listPlayerGroup[i].isInitialized){
				continue;
			}
			listPlayerGroup[i].HideAndClear();
		}

		for(int i = 0; i < listPlayerGroup.Count; i++){
			if(pokerGamePlayData.listSessionIdOnChair[i] >= 0){
				UserDataInGame _userData = pokerGamePlayData.GetUserDataInGameFromListGlobal(pokerGamePlayData.listSessionIdOnChair[i]);
				if(_userData != null){
					listPlayerGroup[i].InitData(_userData);
				}
			}
		}
		UiManager.RefreshUIPlayerRole();
		UiManager.RefreshUIButtonSitDown();
		UiManager.myBarController.ResetData();
		UiManager.myBarController.RefreshUI();
		UiManager.panelSupport.ResetData();

		_finishGameData = null;
		pokerGamePlayData.processFinishGameData.RemoveAt(0);
		// #if TEST
		// Debug.Log("<color=green> End process Finish Game: " + pokerGamePlayData.processFinishGameData.Count +"</color>");
		// #endif

		pokerGamePlayData.currentGameState = PokerGamePlayData.GameState.STATUS_WAIT_FOR_PLAYER;
	}

	IEnumerator DoActionPlayerChat(){
		// #if TEST
		// Debug.Log("<color=green> Start process PlayerChat </color>");
		// #endif
		PokerGamePlayData.PlayerChat_Data _playerChatData = pokerGamePlayData.processPlayerChatData[0];
		screenChat.AddMessage(_playerChatData.sessionId, _playerChatData.strMess, pokerGamePlayData.listGlobalPlayerData);
        this.ShowPopupChat(_playerChatData.sessionId, _playerChatData.strMess);
		_playerChatData = null;
		pokerGamePlayData.processPlayerChatData.RemoveAt(0);
		// #if TEST
		// Debug.Log("<color=green> End process PlayerChat: " + unoGamePlayData.processUnoPlayerChatData.Count +"</color>");
		// #endif
		yield break;
	}

	IEnumerator DoActionPlayerAddGold(){
		// #if TEST
		// Debug.Log("<color=green> Start process PlayerAddGold </color>");
		// #endif
		PokerGamePlayData.PlayerAddGold_Data _playerAddGoldData = pokerGamePlayData.processPlayerAddGoldData[0];
		pokerGamePlayData.UpdateGoldAgain(_playerAddGoldData.sessionId, _playerAddGoldData.goldLast);
		SetUpPlayerAddGold(_playerAddGoldData.sessionId, _playerAddGoldData.reason, _playerAddGoldData.goldAdd, _playerAddGoldData.goldLast);
		_playerAddGoldData = null;
		pokerGamePlayData.processPlayerAddGoldData.RemoveAt(0);
		// #if TEST
		// Debug.Log("<color=green> End process PlayerAddGold: " + unoGamePlayData.processUnoPlayerChatData.Count +"</color>");
		// #endif
		yield break;
	}

	IEnumerator DoActionPlayerSetParent(){
		PokerGamePlayData.PlayerSetParent_Data _playerSetParentData = pokerGamePlayData.processPlayerSetParentData[0];
		pokerGamePlayData.SetUpActionPlayerSetParent(_playerSetParentData);
		_playerSetParentData = null;
		pokerGamePlayData.processPlayerSetParentData.RemoveAt(0);
		yield break;
	}

	// IEnumerator DoActionShowEffGoldFly(Vector2 _startPoint, Vector2 _endPoint, int _numGold){
	// 	Vector2 _newStartPoint = Vector2.zero;
	// 	for(int i = 0; i < _numGold; i++){
	// 		_newStartPoint.x = Random.Range(_startPoint.x - 0.2f, _startPoint.x + 0.2f);
	// 		_newStartPoint.y = Random.Range(_startPoint.y - 0.2f, _startPoint.y + 0.2f);

	// 		GoldObjectController _gold = LeanPool.Spawn(goldPrefab, _newStartPoint, Quaternion.identity).GetComponent<GoldObjectController>();
	// 		effectPoolManager.AddObject(_gold);
	// 		_gold.InitData(sortingLayerManager.sortingLayerInfo_GoldObject, 1f);
	// 		StartCoroutine(_gold.DoActionMoveAndSelfDestruction(_endPoint, 0.8f, LeanTweenType.easeInBack));
	// 		if(_numGold > 1){
	// 			yield return null;
	// 		}
	// 	}
	// }

	// IEnumerator DoActionShowPopupWinGold(PanelPlayerInfoInGameController _panelPlayerInfo, Vector2 _pos, long _goldAdd){
	// 	yield return Yielders.Get(1.1f);
	// 	PanelBonusGoldInGameController _tmpPanelGoldBonus = LeanPool.Spawn(panelBonusGoldPrefab.gameObject, _pos, Quaternion.identity, myCanvas.transform).GetComponent<PanelBonusGoldInGameController>();
	// 	effectPoolManager.AddObject(_tmpPanelGoldBonus);
	// 	_tmpPanelGoldBonus.transform.position = _pos;
	// 	_tmpPanelGoldBonus.Show(_goldAdd);
	// 	_panelPlayerInfo.RefreshGoldInfo();
	// 	if(this.CanPlayMusicAndSfx()){
	// 		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Gold);
	// 	}
	// }

	public void SetUpPlayerAddGold(short _sessionid, int _reason, long _goldAdd, long _goldLast){
		Vector3 _posStartPanelGoldBonus = Vector3.zero;
		bool _showEffect = false;
		bool _checkContinue = true;

		pokerGamePlayData.UpdateGoldAgain(_sessionid, _goldLast);
		if(_sessionid == DataManager.instance.userData.sessionId){
			if(UiManager.myPanelUserInfo.currentState == PanelUserInfoInGameController.State.Show){
				_showEffect = true;
				_checkContinue = false;
				_posStartPanelGoldBonus = UiManager.myPanelUserInfo.userAvata.transform.position;
				UiManager.myPanelUserInfo.RefreshGoldInfo();
			}else{
				UiManager.myPanelUserInfo.RefreshGoldInfo(true);
			}
		}
		
		if(_checkContinue){
			if(pokerGamePlayData.currentGameState == PokerGamePlayData.GameState.STATUS_WAIT_FOR_PLAYER){
				int _indexChair = pokerGamePlayData.listSessionIdOnChair.IndexOf(_sessionid);
				if(_indexChair >= 0){
					_showEffect = true;
					_posStartPanelGoldBonus = listPlayerGroup[_indexChair].panelPlayerInfo.imgAvatar.transform.position;
					listPlayerGroup[_indexChair].userData.gold = _goldLast;
					listPlayerGroup[_indexChair].panelPlayerInfo.RefreshGoldInfo();
				}
			}else{
				if(pokerGamePlayData.CheckIsPlaying(_sessionid)){
					int _index = pokerGamePlayData.listSessionIdPlaying.IndexOf(_sessionid);
					if(_index >= 0){
						int _indexChair = pokerGamePlayData.listPlayerPlayingData[_index].indexChair;
						if(pokerGamePlayData.listSessionIdOnChair[_indexChair] == _sessionid
							&& listPlayerGroup[_indexChair].isInitialized){
							_showEffect = true;
							_posStartPanelGoldBonus = listPlayerGroup[_indexChair].panelPlayerInfo.imgAvatar.transform.position;
							listPlayerGroup[_indexChair].userData.gold = _goldLast;
							listPlayerGroup[_indexChair].panelPlayerInfo.RefreshGoldInfo();
						}
					}
				}
			}
		}

		if(_showEffect){
			StartCoroutine(MyConstant.DoActionShowPopupWinGold(panelBonusGoldPrefab, effectPoolManager, myCanvas.transform
				, _posStartPanelGoldBonus, 0f, _goldAdd));
		}
	}
	#endregion

	#region On Button Click
	// [ContextMenu("TESTADDBET")]
	// public void TESTADDBET(){
	// 	SetUpPlayerAddGold(DataManager.instance.userData.sessionId, 0, 1000, DataManager.instance.userData.gold + 1000);
	// }
	public void OnButtonSitDown(int _indexChair){
		if(pokerGamePlayData.currentGameState == PokerGamePlayData.GameState.STATUS_WAIT_FOR_PLAYER){
			if(System.DateTime.Now.AddSeconds(1f) >= pokerGamePlayData.nextTimeToStartGame
				&& System.DateTime.Now <= pokerGamePlayData.nextTimeToStartGame){
				return;
			}
			if(System.DateTime.Now.AddSeconds(1f) >= pokerGamePlayData.nextTimeToStartGame.AddSeconds(1f)
				&& System.DateTime.Now <= pokerGamePlayData.nextTimeToStartGame.AddSeconds(1f)){
				return;
			}
		}

		if(UiManager.myBarController.timeCanPress > System.DateTime.Now){
			return;
		}
		UiManager.myBarController.timeCanPress = System.DateTime.Now.AddSeconds(0.5f);

		if(this.CanPlayMusicAndSfx()){
            MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
        }

		Poker_RealTimeAPI.instance.SendMessageSitDown((byte)_indexChair);
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

	#region For Test
	public void TestPlayersJoin(){
		StartCoroutine(DoActionTestPlayersJoin());
	}

	IEnumerator DoActionTestPlayersJoin(){
		UserDataInGame _userData = DataManager.instance.userData.CastToUserDataInGame();
		for(int i = 0; i < listPlayerGroup.Count; i++){
			listPlayerGroup[i].InitData(_userData);
			yield return null;
		}
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

		pokerGamePlayData.UpdateGoldAgain(DataManager.instance.userData.sessionId, DataManager.instance.userData.gold);
		UiManager.myPanelUserInfo.RefreshGoldInfo(true);

		if(pokerGamePlayData.currentGameState == PokerGamePlayData.GameState.STATUS_WAIT_FOR_PLAYER){
			int _tmpIndexChair = pokerGamePlayData.listSessionIdOnChair.IndexOf(DataManager.instance.userData.sessionId);
			if(_tmpIndexChair >= 0){
				listPlayerGroup[_tmpIndexChair].userData.gold = DataManager.instance.userData.gold;
				listPlayerGroup[_tmpIndexChair].panelPlayerInfo.RefreshGoldInfo(true);
			}
		}else{
			if(pokerGamePlayData.CheckIsPlaying(DataManager.instance.userData.sessionId)){
				int _index = pokerGamePlayData.listSessionIdPlaying.IndexOf(DataManager.instance.userData.sessionId);
				if(_index >= 0){
					int _indexChair = pokerGamePlayData.listPlayerPlayingData[_index].indexChair;
					if(pokerGamePlayData.listSessionIdOnChair[_indexChair] == DataManager.instance.userData.sessionId){
						listPlayerGroup[_indexChair].userData.gold = DataManager.instance.userData.gold;
						listPlayerGroup[_indexChair].panelPlayerInfo.RefreshGoldInfo(true);
					}
				}
			}
		}
		
		UiManager.myBarController.RefreshUI();
	}

	private void OnDestroy() {
		StopAllCoroutines();
		pokerGamePlayData = null;
		Poker_RealTimeAPI.SelfDestruction();
		instance = null;
	}
}

[System.Serializable]public class Poker_AudioInfo
{
	[Header("Playback")]
    public AudioClip bgm;
	[Header("Sfx")]
	public AudioClip sfx_DealCard;
	public AudioClip sfx_Card;
	public AudioClip sfx_Bet;
	public AudioClip sfx_Check;
	public AudioClip sfx_Fold;
	public AudioClip sfx_HighlighCard;
	public AudioClip sfx_TogglePanel;
	public AudioClip sfx_Notification;
	public AudioClip sfx_PopupChat;
}

public class Poker_CallbackManager
{
	public System.Action onDestructAllObject;
}

[System.Serializable] public class Poker_SortingLayerManager
{
   public MySortingLayerInfo sortingLayerInfo_GoldObject;
}

#if UNITY_EDITOR
[CustomEditor(typeof(Poker_GamePlay_Manager))]
public class Test_Poker_GamePlay_Manager : Editor
{
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		Poker_GamePlay_Manager myScript = (Poker_GamePlay_Manager) target;
		
		GUILayout.Space(30);
		GUILayout.Label(">>> For Test <<<");

		if (GUILayout.Button ("Players Join")) {
			myScript.TestPlayersJoin();
		}
		if (GUILayout.Button ("Deal Global Cards")) {
			myScript.UiManager.TestDealGlobalCard();
		}
		if (GUILayout.Button ("Deal Card To Players")) {
			myScript.UiManager.TestDealCardToPlayers();
		}
		if (GUILayout.Button ("Show All Cards Of Players")) {
			myScript.UiManager.TestShowAllCardsOfPlayers();
		}
		if (GUILayout.Button ("Show Clear All Cards")) {
			myScript.UiManager.ClearAllCards();
		}
	}
}
#endif