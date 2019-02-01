using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;

public class DragonTiger_GamePlay_Manager : ISubGamePlayManager {

	public static DragonTiger_GamePlay_Manager instance;

	public enum GameState
    {
        Bet,
        ShowResult
    }
    public GameState currentGameState { get; set; }

	public enum IndexBet
    {
		Tiger = -1,
        Tie = 0,
		Dragon = 1
    }
	ScreenChatController screenChat{get;set;}
	[SerializeField] PanelListChipDetailController panelListChip;
	[SerializeField] PanelClockInGameController clock;
	[SerializeField] PanelUserInfoInGameController panelUserInGame;
	[SerializeField] Transform panelCardContainer;
	[SerializeField] MyArrowFocusController arrowFocusGetGold;
	[SerializeField] DragonTiger_PanelHistory_Controller panelHistory;
	[SerializeField] DragonTiger_Panel_TableBetOptionDetail_Controller panelTableBetDragon;
	[SerializeField] DragonTiger_Panel_TableBetOptionDetail_Controller panelTableBetTie;
	[SerializeField] DragonTiger_Panel_TableBetOptionDetail_Controller panelTableBetTiger;

	[SerializeField] GameObject iconNotificationChat;
	[SerializeField] GameObject btnSetting;
	[SerializeField] GameObject btnMiniGame;
	[SerializeField] GameObject btnShop;
	[SerializeField] GameObject btnChat;
	
	public DragonTigerCasinoData dragonTigerCasinoData;
	public DragonTiger_SortingLayerManager sortingLayerManager;

	public List<CardDetail> listCardDetail;

	[Header("Place Holders")]
	public RectTransform panelHistoryPlaceHolder;
	[SerializeField] Transform cardDragonDealPlaceHolder;
	[SerializeField] Transform cardDragonShowPlaceHolder;
	[SerializeField] Transform cardTigerDealPlaceHolder;
	[SerializeField] Transform cardTigerShowPlaceHolder;
	[SerializeField] Transform iconGoldHolder;
	[SerializeField] Transform betDragon_PlaceHolder;
	[SerializeField] Transform betTie_PlaceHolder;
	[SerializeField] Transform betTiger_PlaceHolder;
	[SerializeField] Transform showEffWinGold_Dragon_PlaceHolder;
	[SerializeField] Transform showEffWinGold_Tie_PlaceHolder;
	[SerializeField] Transform showEffWinGold_Tiger_PlaceHolder;
	[SerializeField] Transform showEffPanelGoldBonusEffPlaceHolder;

	[Header("Prefabs")]
	[SerializeField] GameObject screenChatPrefab;
	[SerializeField] GameObject cardPrefab;
	[SerializeField] GameObject goldPrefab;
	[SerializeField] GameObject chipPrefab;
	[SerializeField] GameObject panelBonusGoldPrefab;

	[Header("Audio Info")]
    public DragonTiger_AudioInfo myAudioInfo;

	public DragonTiger_CallbackManager callbackManager;

	PanelCardDetailController cardDragon, cardTiger;

	public MySimplePoolManager effectPoolManager;

	Vector3 posistionSpawnCard;
	IEnumerator actionRunProcessPlaying, actionRunProcessNonPlaying, actionCheckFocusIconGetGold;
	List<IEnumerator> listProcessPlaying;
	List<IEnumerator> listProcessNonPlaying;
	IEnumerator actionDealCardFirstGame;

	private void Awake() {
		instance = this;
	}

	public override void InitData(bool _isFullScreen, bool _connectFirst, System.Action _onFinished = null){
		currentGameState = GameState.Bet;
		
		panelUserInGame.InitData();
		iconNotificationChat.SetActive(false);

		effectPoolManager = new MySimplePoolManager();
		listProcessPlaying = new List<IEnumerator>();
		listProcessNonPlaying = new List<IEnumerator>();

		screenChat = ((GameObject) Instantiate(screenChatPrefab, transform)).GetComponent<ScreenChatController>();

		isFullScreen = _isFullScreen;
		if(isFullScreen){
			ratioScale = 1f;
			myContainer.localScale = Vector3.one * ratioScale;
			btnClose.gameObject.SetActive(false);
			btnMiniGame.SetActive(true);
			btnShop.SetActive(true);
			btnSetting.SetActive(true);
			btnChat.SetActive(true);
		}else{
			ratioScale = 0.8f;
			myContainer.localScale = Vector3.one * ratioScale;
			btnClose.gameObject.SetActive(true);
			btnClose.transform.position = btnClose_PlaceHolder.position;
			
			Vector3 _tmpPosUserInfo = panelUserInGame.transform.position;
			_tmpPosUserInfo.x -= 0.4f;
			panelUserInGame.transform.position = _tmpPosUserInfo;

			btnMiniGame.SetActive(false);
			btnShop.SetActive(false);
			btnSetting.SetActive(true);
			btnChat.SetActive(true);
		}

		posistionSpawnCard = CoreGameManager.instance.currentSceneManager.mainCamera.transform.position;
		posistionSpawnCard.y += CoreGameManager.instance.currentSceneManager.mainCamera.sizeOfCamera.y/2 + 3f;
		posistionSpawnCard.z = 0f;

		cardDragon = null;
		cardTiger = null;

		dragonTigerCasinoData = new DragonTigerCasinoData();

		panelListChip.InitData();
		
		StartCoroutine(DoActionRun(_connectFirst, _onFinished));
	}

	IEnumerator DoActionRun(bool _connectFirst, System.Action _onFinished = null){
		yield return null;
		panelLoading.SetActive(true);
		
		InitAllCallback();

		actionRunProcessPlaying = DoActionRunProcessPlaying();
		StartCoroutine(actionRunProcessPlaying);

		actionRunProcessNonPlaying = DoActionRunProcessNonPlaying();
		StartCoroutine(actionRunProcessNonPlaying);

		actionCheckFocusIconGetGold = DoActionCheckFocusIconGetGold();
		StartCoroutine(actionCheckFocusIconGetGold);

		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_MINIGAME_LONGHO_GET_GAMEINFO, (_mess)=>{
           	dragonTigerCasinoData.InitDataWhenGetTableInfo(_mess);
		   	if(_mess.avaiable() > 0){
				#if TEST
				Debug.Log (">>> Chua doc het CMD : " + _mess.getCMDName ());
				#endif
			}

			dragonTigerCasinoData.CheckListHistoryAgain();

			AchievementDetail _achievementDetail = DataManager.instance.achievementData.GetAchievementDetail(IMiniGameInfo.Type.DragonTigerCasino);
			if(_achievementDetail != null){
				_achievementDetail.countWin = dragonTigerCasinoData.winAchievement;
				_achievementDetail.countDraw = dragonTigerCasinoData.tieAchievement;
				_achievementDetail.countLose = dragonTigerCasinoData.loseAchievement;
				// Debug.Log(dragonTigerCasinoData.winAchievement + " - " + dragonTigerCasinoData.tieAchievement + " - " + dragonTigerCasinoData.loseAchievement);
			}

			RefreshUITableBet(true);

			RegisterActionUpdateResultGame();
			RegisterActionMeAddBet();
            RegisterActionUpdateTableBet();
			RegisterActionPlayerChat();
			RegisterActionPlayerAddGold();
			RegisterActionSetParentInfo();
			RegisterActionAlertUpdateServer();
        });
		if(_connectFirst){
			GlobalRealTimeSendingAPI.SendMessageJoinToMiniGame(DataManager.instance.miniGameData.currentSubGameDetail.myInfo);
		}
		NetworkGlobal.instance.instanceRealTime.ResumeReceiveMessage();
        yield return new WaitUntil(() => dragonTigerCasinoData != null && dragonTigerCasinoData.hasLoadGameInfo);
		
		panelLoading.SetActive(false);

		actionDealCardFirstGame = DoActionDealCardAtFirst();
		StartCoroutine(actionDealCardFirstGame);

		MyAudioManager.instance.PlayMusic(myAudioInfo.bgm);

		onPressBack = ()=>{
			if(SettingScreenController.instance.currentState == UIHomeScreenController.State.Show){
				MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
				SettingScreenController.instance.Hide();
			}else{
				OnButtonSettingClicked();
			}
		};
		CoreGameManager.instance.RegisterNewCallbackPressBackKey (onPressBack);

		if(_onFinished != null){
			_onFinished();
		}
	}

	void InitAllCallback(){
		callbackManager = new DragonTiger_CallbackManager();

		callbackManager.onDestructAllObject = ()=>{
			effectPoolManager.ClearAllObjectsNow();
			panelListChip.SelfDestruction();
			if(cardDragon != null){
				cardDragon.SelfDestruction();
				cardDragon = null;
			}
			if(cardTiger != null){
				cardTiger.SelfDestruction();
				cardTiger = null;
			}
			screenChat.SelfDestruction();

			if(onPressBack != null){
				CoreGameManager.instance.RemoveCurrentCallbackPressBackKey(onPressBack);
				onPressBack = null;
			}
		};

		callbackManager.onStartShowBet += ()=>{
			panelUserInGame.RefreshGoldInfo();
			panelListChip.SetFocusChipAgain();
			RefreshUITableBet();
			StartCountDown();
			DealCards(0.5f);
		};

		callbackManager.onStartShowResult += ()=>{
			panelHistory.Hide();
		};

		screenChat.onSendMessage = (_mess) =>
        {
            DragonTiger_RealTimeAPI.instance.SendMessageChat(_mess);
        };
        screenChat.onStartShow += HideIconNotificationChat;
		screenChat.onHasNewMessage += ShowIconNotificationChat;
	}

	void ShowIconNotificationChat(){
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
            if(dragonTigerCasinoData != null){
                MyGamePlayData.AlertUpdateServer_Data _data = new MyGamePlayData.AlertUpdateServer_Data(_mess);
                System.TimeSpan _timeSpanRemain = _data.timeToUpdateServer - System.DateTime.Now;                
                PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kWarning)
                    , string.Format(MyLocalize.GetString("System/Message_ServerMaintenance"), _timeSpanRemain.Minutes, _timeSpanRemain.Seconds)
                    , string.Empty
                    , MyLocalize.GetString(MyLocalize.kOk));
            }
        });
    }
	void RegisterActionUpdateResultGame(){
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_MINIGAME_LONGHO_PROCESS_RESULT, (_mess) =>
        {
			if(dragonTigerCasinoData != null){
				dragonTigerCasinoData.SetDataWhenShowResult(_mess);
				listProcessPlaying.Add(DoActionShowResult());
			}
		});
	}

	void RegisterActionUpdateTableBet(){
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_MINIGAME_LONGHO_TABLE_BET, (_mess) =>
        {	
			if(dragonTigerCasinoData != null){
				dragonTigerCasinoData.SetDataUpdateTableBet(_mess);
				listProcessNonPlaying.Add(DoActionCheckUpdateTableBet());
			}
        });
	}

	void RegisterActionMeAddBet(){
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_MINIGAME_LONGHO_ADDBET, (_mess) =>
        {
			if(dragonTigerCasinoData != null){
				dragonTigerCasinoData.SetDataMeAddBet(_mess);
				listProcessNonPlaying.Add(DoActionCheckMeAddBet());
			}
        });
	}

	void RegisterActionPlayerChat(){
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_MINIGAME_LONGHO_CHAT_ALL, (_mess) =>
        {   
			if(dragonTigerCasinoData != null){
				dragonTigerCasinoData.SetPlayerChatData(_mess);
				listProcessNonPlaying.Add(DoActionPlayerChat());
			}
        });
	}

	void RegisterActionPlayerAddGold(){
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_onPlayerAddGold, (_mess) =>
        {
			if(dragonTigerCasinoData != null){
				dragonTigerCasinoData.SetPlayerAddGoldData(_mess);
				listProcessNonPlaying.Add(DoActionPlayerAddGold());
			}
        });
    }

	void RegisterActionSetParentInfo(){
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_SET_PARENT, (_mess) => 
		{
			if(dragonTigerCasinoData != null){
				dragonTigerCasinoData.SetDataWhenSetParent(_mess);
				listProcessNonPlaying.Add(DoActionPlayerSetParent());
			}
		});
	}
	#endregion

	#region Logic Game
	IEnumerator DoActionRunProcessPlaying(){
		while(true){
			if(dragonTigerCasinoData == null || !dragonTigerCasinoData.hasLoadGameInfo || actionDealCardFirstGame != null){
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
			if(dragonTigerCasinoData == null || !dragonTigerCasinoData.hasLoadGameInfo || actionDealCardFirstGame != null){
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
		if(!btnShop.activeSelf){
			actionCheckFocusIconGetGold = null;
			yield break;
		}
		while(true){
			if(dragonTigerCasinoData == null || !dragonTigerCasinoData.hasLoadGameInfo){
				yield return null;
				continue;
			}
			if(btnShop.activeSelf){
				if(DataManager.instance.userData.GetGoldView() <= 0){
					arrowFocusGetGold.Show();
				}else{
					arrowFocusGetGold.Hide();
				}
			}
			yield return Yielders.Get(1f);
		}
	}

	IEnumerator DoActionDealCardAtFirst(){
		StartCountDown();
		yield return DealCards(1.2f);
        actionDealCardFirstGame = null;
	}

	IEnumerator DoActionPlayerChat(){
		// #if TEST
		// Debug.Log("<color=green> Start process PlayerChat </color>");
		// #endif
		DragonTigerCasinoData.SubGame_PlayerChat_Data _playerChatData = dragonTigerCasinoData.processSubGamePlayerChatData[0];
		screenChat.AddMessage(_playerChatData.userData, _playerChatData.contentChat);
		_playerChatData = null;
		dragonTigerCasinoData.processSubGamePlayerChatData.RemoveAt(0);
		// #if TEST
		// Debug.Log("<color=green> End process PlayerChat: " + dragonTigerCasinoData.processSubGamePlayerChatData.Count +"</color>");
		// #endif
		yield break;
	}

	IEnumerator DoActionPlayerAddGold(){
		// #if TEST
		// Debug.Log("<color=green> Start process PlayerAddGold </color>");
		// #endif
		DragonTigerCasinoData.PlayerAddGold_Data _playerAddGoldData = dragonTigerCasinoData.processPlayerAddGoldData[0];
		SetUpPlayerAddGold(_playerAddGoldData.sessionId, _playerAddGoldData.reason, _playerAddGoldData.goldAdd, _playerAddGoldData.goldLast);
		_playerAddGoldData = null;
		dragonTigerCasinoData.processPlayerAddGoldData.RemoveAt(0);
		// #if TEST
		// Debug.Log("<color=green> End process PlayerAddGold: " + dragonTigerCasinoData.processPlayerAddGoldData.Count +"</color>");
		// #endif
		yield break;
	}

	IEnumerator DoActionPlayerSetParent(){
		DragonTigerCasinoData.PlayerSetParent_Data _playerSetParentData = dragonTigerCasinoData.processPlayerSetParentData[0];
		dragonTigerCasinoData.SetUpActionPlayerSetParent(_playerSetParentData);
		_playerSetParentData = null;
		dragonTigerCasinoData.processPlayerSetParentData.RemoveAt(0);
		yield break;
	}

	IEnumerator DoActionCheckMeAddBet(){
		DragonTigerCasinoData.DragonTiger_MeAddBet_Data _meAddBetData = dragonTigerCasinoData.processMeAddBetData[0];
		
		if(currentGameState == GameState.Bet){
			yield return StartCoroutine(DoActionMeAddBet(_meAddBetData));
		}else{
			listProcessPlaying.Add(DoActionMeAddBet(_meAddBetData));
		}

		_meAddBetData = null;
		dragonTigerCasinoData.processMeAddBetData.RemoveAt(0);
	}

	IEnumerator DoActionMeAddBet(DragonTigerCasinoData.DragonTiger_MeAddBet_Data _meAddBetData){
		DataManager.instance.userData.gold = _meAddBetData.myGOLD;
		DataManager.instance.userData.SetTotalBetInGameInfo(IMiniGameInfo.Type.DragonTigerCasino, _meAddBetData.myTotalBet);

		if(_meAddBetData.isAddOk){
			if(_meAddBetData.indexBet == (sbyte) IndexBet.Dragon){
				dragonTigerCasinoData.tableCountDragon = _meAddBetData.countBet;
				dragonTigerCasinoData.tableGlobalBetDragon = _meAddBetData.globalBet;
				dragonTigerCasinoData.tableMyBetDragon = _meAddBetData.myBet;
			}else if(_meAddBetData.indexBet == (sbyte) IndexBet.Tiger){
				dragonTigerCasinoData.tableCountTiger = _meAddBetData.countBet;
				dragonTigerCasinoData.tableGlobalBetTiger = _meAddBetData.globalBet;
				dragonTigerCasinoData.tableMyBetTiger = _meAddBetData.myBet;
			}else{
				dragonTigerCasinoData.tableCountTie = _meAddBetData.countBet;
				dragonTigerCasinoData.tableGlobalBetTie = _meAddBetData.globalBet;
				dragonTigerCasinoData.tableMyBetTie = _meAddBetData.myBet;
			}

			if(currentGameState == GameState.Bet){
				panelUserInGame.RefreshGoldInfo();
				ShowEffPlayerAddBet(_meAddBetData.chipIndex, _meAddBetData.indexBet);
				RefreshUITableBet();
				panelListChip.SetFocusChipAgain();
			}else{
				#if TEST
				Debug.LogError(">>> Không phải state Bet");
				#endif
			}
		}else{
			#if TEST
			Debug.LogError(">>> ADDBET failed");
			#endif
			dragonTigerCasinoData.tableMyBetDragon = _meAddBetData.betDragon;
			dragonTigerCasinoData.tableMyBetTiger = _meAddBetData.betTiger;
			dragonTigerCasinoData.tableMyBetTie = _meAddBetData.betTie;

			if(currentGameState == GameState.Bet){
				panelUserInGame.RefreshGoldInfo();
				RefreshUITableBet();
				panelListChip.SetFocusChipAgain();
			}else{
				#if TEST
				Debug.LogError(">>> Không phải state Bet");
				#endif
			}
		}
		
		yield break;
	}

	IEnumerator DoActionCheckUpdateTableBet(){
		DragonTigerCasinoData.DragonTiger_UpdateTableBet_Data _updateTableBetData = dragonTigerCasinoData.processUpdateTableBetData[0];

		if(currentGameState == GameState.Bet){
			yield return StartCoroutine(DoActionUpdateTableBet(_updateTableBetData));
		}else{
			listProcessPlaying.Add(DoActionUpdateTableBet(_updateTableBetData));
		}

		_updateTableBetData = null;
		dragonTigerCasinoData.processUpdateTableBetData.RemoveAt(0);
	}

	IEnumerator DoActionUpdateTableBet(DragonTigerCasinoData.DragonTiger_UpdateTableBet_Data _updateTableBetData){
		// Debug.Log(">>> " + _indexBet + " - " + _tableCount + " - " + _tableBet);

		if(_updateTableBetData.indexBet == (sbyte) IndexBet.Dragon){
			dragonTigerCasinoData.tableCountDragon = _updateTableBetData.tableCount;
			dragonTigerCasinoData.tableGlobalBetDragon = _updateTableBetData.tableBet;
		}else if(_updateTableBetData.indexBet == (sbyte) IndexBet.Tiger){
			dragonTigerCasinoData.tableCountTiger = _updateTableBetData.tableCount;
			dragonTigerCasinoData.tableGlobalBetTiger = _updateTableBetData.tableBet;
		}else{
			dragonTigerCasinoData.tableCountTie = _updateTableBetData.tableCount;
			dragonTigerCasinoData.tableGlobalBetTie = _updateTableBetData.tableBet;
		}
		if(currentGameState == GameState.Bet){
			RefreshUITableBet();
		}else{
			#if TEST
			Debug.LogError(">>> Không phải state Bet");
			#endif
		}
		yield break;
	}

	Coroutine DealCards(float _timeDelay){
		return StartCoroutine(DoActionDealCards(_timeDelay));
	}

	IEnumerator DoActionDealCards(float _timeDelay){
		System.TimeSpan _tmpDeltaTime = dragonTigerCasinoData.nextTimeToShowResult - System.DateTime.Now;
		if(_tmpDeltaTime.TotalSeconds <= 5){
			cardDragon = LeanPool.Spawn(cardPrefab, cardDragonDealPlaceHolder.position, Quaternion.identity, panelCardContainer).GetComponent<PanelCardDetailController>();
			cardDragon.transform.position = cardDragonDealPlaceHolder.position;
			cardDragon.ResizeAgain(100, 150f);
			cardTiger = LeanPool.Spawn(cardPrefab, cardTigerDealPlaceHolder.position, Quaternion.identity, panelCardContainer).GetComponent<PanelCardDetailController>();
			cardTiger.transform.position = cardTigerDealPlaceHolder.position;
			cardTiger.ResizeAgain(100, 150f);
		}else{
			cardDragon = LeanPool.Spawn(cardPrefab, Vector3.zero, Quaternion.identity, panelCardContainer).GetComponent<PanelCardDetailController>();
			cardDragon.transform.position = posistionSpawnCard;
			cardDragon.ResizeAgain(100, 150f);
			cardTiger = LeanPool.Spawn(cardPrefab, Vector3.zero, Quaternion.identity, panelCardContainer).GetComponent<PanelCardDetailController>();
			cardTiger.transform.position = posistionSpawnCard;
			cardTiger.ResizeAgain(100, 150f);
			yield return Yielders.Get(_timeDelay);
			yield return CoroutineChain.Start
				.Sequential(
					cardDragon.Move(cardDragonDealPlaceHolder.position, 0.3f, LeanTweenType.easeOutSine, myAudioInfo.sfx_DealCard),
					cardTiger.Move(cardTigerDealPlaceHolder.position, 0.3f, LeanTweenType.easeOutSine, myAudioInfo.sfx_DealCard));
		}
	}

	void StartCountDown(){
		System.TimeSpan _tmpDeltaTime = dragonTigerCasinoData.nextTimeToShowResult - System.DateTime.Now;
		clock.StartCountDown(_tmpDeltaTime.TotalSeconds, null);
	}

	IEnumerator DoActionShowResult()
    {
		if (currentGameState == GameState.ShowResult)
        {
            Debug.LogError("Đang show result rồi");
            yield break;
        }

		DragonTigerCasinoData.DragonTiger_Result_Data _resultData = dragonTigerCasinoData.processResultData[0];

		currentGameState = GameState.ShowResult;

		// ---- Merge dữ liệu ---- //
		int _cardDragon = ((int) _resultData.cardDragon) % 13;
		int _cardTiger = ((int) _resultData.cardTiger) % 13;
		if(_cardDragon > _cardTiger){
			dragonTigerCasinoData.listHistory.Add((int) IndexBet.Dragon);
		}else if(_cardDragon < _cardTiger){
			dragonTigerCasinoData.listHistory.Add((int) IndexBet.Tiger);
		}else{
			dragonTigerCasinoData.listHistory.Add((int) IndexBet.Tie);
		}
		dragonTigerCasinoData.CheckListHistoryAgain();

		dragonTigerCasinoData.ResetTableBet();
		dragonTigerCasinoData.nextTimeToShowResult = _resultData.nextTimeToShowResult;

		DataManager.instance.userData.gold = _resultData.GOLD;
		DataManager.instance.userData.SetTotalBetInGameInfo(IMiniGameInfo.Type.DragonTigerCasino, 0);

		if(_resultData.caseCheck == 1){
			AchievementDetail _achievementDetail = DataManager.instance.achievementData.GetAchievementDetail(IMiniGameInfo.Type.DragonTigerCasino);
			if(_achievementDetail != null){
				if(_resultData.goldProcess > 0){
					_achievementDetail.countWin = _resultData.achievement;
				}else if(_resultData.goldProcess < 0){
					_achievementDetail.countLose = _resultData.achievement;
				}else{
					_achievementDetail.countDraw = _resultData.achievement;
				}
			}else{
				Debug.LogError(">>> _achievementDetail is null");
			}
		}else if(_resultData.caseCheck == -88){
			PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
					, MyLocalize.GetString("Error/GamePlay_BetError_0")
					, _resultData.caseCheck.ToString()
					, MyLocalize.GetString(MyLocalize.kOk));
		}
		// ----------------------- //
		
		if(callbackManager != null && callbackManager.onStartShowResult != null){
			callbackManager.onStartShowResult();
		}

		ICardInfo _cardDragonInfo = GetCardInfo(_resultData.cardDragon);
		if(_cardDragonInfo == null){
			Debug.LogError("_cardDragonInfo is null");
		}
		ICardInfo _cardTigerInfo = GetCardInfo(_resultData.cardTiger);
		if(_cardTigerInfo == null){
			Debug.LogError("_cardTigerInfo is null");
		}

		float _timeShow = 0.2f;

		yield return CoroutineChain.Start
			.Sequential(cardDragon.Move(cardDragonShowPlaceHolder.position, _timeShow, LeanTweenType.easeInSine, myAudioInfo.sfx_Card)
				, cardTiger.Move(cardTigerShowPlaceHolder.position, _timeShow, LeanTweenType.easeInSine, myAudioInfo.sfx_Card))
			.Sequential(cardDragon.ScaleTo(Vector2.one * 2f, _timeShow, LeanTweenType.easeInSine, myAudioInfo.sfx_Card)
				, cardTiger.ScaleTo(Vector2.one * 2f, _timeShow, LeanTweenType.easeInSine, myAudioInfo.sfx_Card))
			.Parallel(cardDragon.Show(_cardDragonInfo, myAudioInfo.sfx_Card)
				, cardTiger.Show(_cardTigerInfo, myAudioInfo.sfx_Card));
		
		yield return Yielders.Get(1f);

		Vector2 _start = Vector2.zero;
		bool _isFinished = false;

		if(_cardDragon > _cardTiger){
			_start = showEffWinGold_Dragon_PlaceHolder.position;
			yield return cardTiger.ScaleTo(Vector2.one, _timeShow, LeanTweenType.easeInSine, myAudioInfo.sfx_Card);
			cardTiger.SetUpShadow(true);
			panelTableBetTiger.SetShadow(true);
			panelTableBetTie.SetShadow(true);

			MyAudioManager.instance.PlaySfx(myAudioInfo.sfx_ShowResult);
			cardDragon.SetUpLoopHighlight(()=>{
				_isFinished = true;
			});
			yield return new WaitUntil(()=>_isFinished);
			yield return cardDragon.ScaleTo(Vector2.one, _timeShow, LeanTweenType.easeInSine, myAudioInfo.sfx_Card);
			// panelTableBetDragon.SetUpHighlight();
		}else if(_cardDragon < _cardTiger){
			_start = showEffWinGold_Tiger_PlaceHolder.position;
			yield return cardDragon.ScaleTo(Vector2.one, _timeShow, LeanTweenType.easeInSine, myAudioInfo.sfx_Card);
			cardDragon.SetUpShadow(true);

			panelTableBetDragon.SetShadow(true);
			panelTableBetTie.SetShadow(true);

			MyAudioManager.instance.PlaySfx(myAudioInfo.sfx_ShowResult);
			cardTiger.SetUpLoopHighlight(()=>{
				_isFinished = true;
			});
			yield return new WaitUntil(()=>_isFinished);
			yield return cardTiger.ScaleTo(Vector2.one, _timeShow, LeanTweenType.easeInSine, myAudioInfo.sfx_Card);
			// panelTableBetTiger.SetUpHighlight();
		}else{
			_start = showEffWinGold_Tie_PlaceHolder.position;
			yield return CoroutineChain.Start
				.Parallel(cardDragon.ScaleTo(Vector2.one, _timeShow, LeanTweenType.easeInSine, myAudioInfo.sfx_Card)
					, cardTiger.ScaleTo(Vector2.one, _timeShow, LeanTweenType.easeInSine, myAudioInfo.sfx_Card));
			
			panelTableBetDragon.SetShadow(true);
			panelTableBetTiger.SetShadow(true);

			MyAudioManager.instance.PlaySfx(myAudioInfo.sfx_ShowResult);
			panelTableBetTie.SetUpHighlight(()=>{
				_isFinished = true;
			});
			yield return new WaitUntil(()=>_isFinished);
		}

		if(_resultData.betUnit > 0){
			Vector2 _end = iconGoldHolder.position;
			StartCoroutine(MyConstant.DoActionShowPopupWinGold(panelBonusGoldPrefab, effectPoolManager, myCanvas.transform
				, showEffPanelGoldBonusEffPlaceHolder.position, 1.1f, _resultData.betUnit
				, ()=>{
					panelUserInGame.RefreshGoldInfo();
				}));
			yield return StartCoroutine(MyConstant.DoActionShowEffectGoldFly(goldPrefab, effectPoolManager, sortingLayerManager.sortingLayerInfo_GoldObject
				, _start, _end, 10, ratioScale, 0.8f
				, ()=>{
					MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Gold);
				}));
		}

		panelTableBetDragon.SetShadow(false);
		panelTableBetTie.SetShadow(false);
		panelTableBetTiger.SetShadow(false);

		cardDragon.SetUpShadow(false);
		cardTiger.SetUpShadow(false);

		yield return CoroutineChain.Start
			.Parallel(cardDragon.Move(posistionSpawnCard, 0.3f, LeanTweenType.easeInSine, myAudioInfo.sfx_Card),
				cardTiger.Move(posistionSpawnCard, 0.3f, LeanTweenType.easeInSine, myAudioInfo.sfx_Card));
		cardDragon.SelfDestruction();
		cardDragon = null;
		cardTiger.SelfDestruction();
		cardTiger = null;

		if(callbackManager != null && callbackManager.onEndShowResult != null){
			callbackManager.onEndShowResult();
		}

		_resultData = null;
		dragonTigerCasinoData.processResultData.RemoveAt(0);

		SetBetAgain();
	}

	void SetBetAgain(){
		currentGameState = GameState.Bet;
		if(callbackManager != null && callbackManager.onStartShowBet != null){
			callbackManager.onStartShowBet();
		}
	}

	/// <summary>
	/// Đặt cược
	/// </summary>
	/// <param name="long"> value = 1</param>
	/// <param name="hòa"> value = 0</param>
	/// <param name="hổ"> value = -1</param>
	public void AddBet(sbyte _indexBet){
		if(!this.CanAddBet()){
			return;
		}
		if(panelListChip == null || panelListChip.currentChip == null){
			if(panelListChip.currentChip == null){
				PopupManager.Instance.CreateToast(MyLocalize.GetString("Global/PlsSelectChip"));
			}
			return;
		}
		DragonTiger_RealTimeAPI.instance.SendMessageAddBet((byte) _indexBet, (short) panelListChip.currentChip.index, panelListChip.currentChip.chipInfo.value);
	}

	public override void LeftGameAndHide(){
		NetworkGlobal.instance.SendMessageRealTime (new MessageSending (CMD_REALTIME.C_MINIGAME_LONGHO_LEFT_GAME));
        if(callbackManager != null && callbackManager.onDestructAllObject != null){
			callbackManager.onDestructAllObject();
		}
		StopAllCoroutines();
		
		Hide();
		MyAudioManager.instance.StopAll();
		DataManager.instance.miniGameData.currentSubGameDetail = null;
	}

	public void SetUpPlayerAddGold(short _sessionid, int _reason, long _goldAdd, long _GOLD){
		if(_sessionid != DataManager.instance.userData.sessionId){
			return;
		}
		
		DataManager.instance.userData.gold = _GOLD;

		StartCoroutine(MyConstant.DoActionShowPopupWinGold(panelBonusGoldPrefab, effectPoolManager, myCanvas.transform, showEffPanelGoldBonusEffPlaceHolder.position, 0, _goldAdd));

		panelUserInGame.RefreshGoldInfo();
		panelListChip.SetFocusChipAgain();
	}
	#endregion

	#region Refresh UI
	void RefreshUITableBet(bool _isNow = false){
		if(currentGameState != GameState.Bet){
			return;
		}
		panelTableBetDragon.SetCountBet(dragonTigerCasinoData.tableCountDragon, true);
		panelTableBetDragon.SetGlobalBet(dragonTigerCasinoData.tableGlobalBetDragon, 999999999, _isNow);
		panelTableBetDragon.SetMyBet(dragonTigerCasinoData.tableMyBetDragon, 999999999, _isNow);

		panelTableBetTie.SetCountBet(dragonTigerCasinoData.tableCountTie, true);
		panelTableBetTie.SetGlobalBet(dragonTigerCasinoData.tableGlobalBetTie, 999999999, _isNow);
		panelTableBetTie.SetMyBet(dragonTigerCasinoData.tableMyBetTie, 999999999, _isNow);

		panelTableBetTiger.SetCountBet(dragonTigerCasinoData.tableCountTiger, true);
		panelTableBetTiger.SetGlobalBet(dragonTigerCasinoData.tableGlobalBetTiger, 999999999, _isNow);
		panelTableBetTiger.SetMyBet(dragonTigerCasinoData.tableMyBetTiger, 999999999, _isNow);
	}
	#endregion

	#region Utilities
	public ICardInfo GetCardInfo(int _value){
		if(listCardDetail == null || listCardDetail.Count == 0){
			return null;
		}
		for(int i = 0; i < listCardDetail.Count; i ++){
			if(listCardDetail[i].cardId == _value){
				return listCardDetail[i].cardInfo;
			}
		}
		return null;
	}

	// IEnumerator DoActionShowEffWinGold(Vector2 _startPoint, Vector2 _endPoint, int _numGold){
	// 	Vector2 _newStartPoint = Vector2.zero;
	// 	for(int i = 0; i < _numGold; i++){
	// 		_newStartPoint.x = Random.Range(_startPoint.x - 0.2f, _startPoint.x + 0.2f);
	// 		_newStartPoint.y = Random.Range(_startPoint.y - 0.2f, _startPoint.y + 0.2f);
	// 		GoldObjectController _gold = LeanPool.Spawn(goldPrefab, _newStartPoint, Quaternion.identity).GetComponent<GoldObjectController>();
	// 		effectPoolManager.AddObject(_gold);
	// 		_gold.InitData(sortingLayerManager.sortingLayerInfo_GoldObject, ratioScale);
	// 		StartCoroutine(_gold.DoActionMoveAndSelfDestruction(_endPoint, 0.8f, LeanTweenType.easeInBack));
	// 		if(_numGold > 1){
	// 			yield return null;
	// 		}
	// 	}
	// }

	// IEnumerator DoActionShowPopupWinGold(long _goldAdd){
	// 	yield return Yielders.Get(1.1f);
	// 	PanelBonusGoldInGameController _tmpPanelGoldBonus = LeanPool.Spawn(panelBonusGoldPrefab.gameObject, showEffPanelGoldBonusEffPlaceHolder.position, Quaternion.identity, myCanvas.transform).GetComponent<PanelBonusGoldInGameController>();
	// 	effectPoolManager.AddObject(_tmpPanelGoldBonus);
	// 	_tmpPanelGoldBonus.transform.position = showEffPanelGoldBonusEffPlaceHolder.position;
	// 	_tmpPanelGoldBonus.Show(_goldAdd);
	// 	panelUserInGame.RefreshGoldInfo();
	// 	MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Gold);
	// }

	public void ShowEffPlayerAddBet(short _chipIndex, sbyte _indexBet){
		if(currentGameState != GameState.Bet){
			return;
		}
		if(_chipIndex == -1){
			Debug.LogError(">>> chipIndex = -1");
			return;
		}
		
		Vector2 _posStart = iconGoldHolder.position;
		Vector2 _posEnd = Vector2.zero;
		if(_indexBet == (sbyte) IndexBet.Dragon){
			_posEnd = betDragon_PlaceHolder.position;
		}else if(_indexBet == (sbyte) IndexBet.Tiger){
			_posEnd = betTiger_PlaceHolder.position;
		}else{
			_posEnd = betTie_PlaceHolder.position;
		}
		_posEnd.x = Random.Range(_posEnd.x - 0.2f, _posEnd.x + 0.2f);
		_posEnd.y = Random.Range(_posEnd.y - 0.2f, _posEnd.y + 0.2f);
		
		IChipInfo _chipInfo = panelListChip.listChipDetail[_chipIndex].chipInfo;
		ChipObjectController _tmpChip = LeanPool.Spawn(chipPrefab.gameObject, _posStart, Quaternion.identity).GetComponent<ChipObjectController>();
		effectPoolManager.AddObject(_tmpChip);
		_tmpChip.SetData(_chipInfo, sortingLayerManager.sortingLayerInfo_ChipObject , ratioScale);
		_tmpChip.SetUpMoveToTableBet(_posEnd);
		_tmpChip.RegisCallbackJustMoveFinished(_indexBet, (_i)=>{
			MyAudioManager.instance.PlaySfx(myAudioInfo.sfx_ThrowChip);
		});
	}
	#endregion

	#region On Button Clicked 
	public void OnButtonShowHistory(){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);

		if(panelHistory.currentState == DragonTiger_PanelHistory_Controller.State.Show){
			MyAudioManager.instance.PlaySfx(myAudioInfo.sfx_TogglePanelHistory);
			panelHistory.Hide();
		}else{
			if(this.CanShowHistory()){
				MyAudioManager.instance.PlaySfx(myAudioInfo.sfx_TogglePanelHistory);
				panelHistory.InitData();
				panelHistory.Show();
			}
		}
	}

	public void OnButtonSettingClicked(){
        MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);

        SettingScreenController.instance.InitData();
        SettingScreenController.instance.Show();
        SettingScreenController.instance.LateInitData();
        SettingScreenController.instance.btnOutRoom.onClick.AddListener(OnButtonOutRoomClicked);
    }

	public void OnButtonOutRoomClicked(){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
		DataManager.instance.userData.RemoveTotalBetInGameInfo(IMiniGameInfo.Type.DragonTigerCasino);
		CoreGameManager.instance.SetUpOutRoomFromSubGamePlayAndBackToChooseGameScreen();

		// PopupManager.Instance.CreatePopupDialog(MyLocalize.GetString(MyLocalize.kWarning)
		// 		, MyLocalize.GetString("System/AskForOutRoom")
		// 		, string.Empty
		// 		, MyLocalize.GetString(MyLocalize.kYes)
		// 		, MyLocalize.GetString(MyLocalize.kNo)
		// 		, () =>{
		// 			CoreGameManager.instance.SetUpOutRoomFromSubGamePlayAndBackToChooseGameScreen();
		// 		}, null);
	}
	public void OnButtonMiniGamesClicked()
    {
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);

        ChooseSubGameScreenController.instance.InitData();
        ChooseSubGameScreenController.instance.Show();
        ChooseSubGameScreenController.instance.LateInitData();
    }
	public void OnButtonShopClicked()
    {
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);

        GetGoldScreenController.instance.InitData();
        GetGoldScreenController.instance.Show();
        GetGoldScreenController.instance.LateInitData();
    }
	public void OnButtonChatClicked()
    {
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
        screenChat.Show();
    }
	public void OnButtonCloseClicked()
    {
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
		DataManager.instance.userData.RemoveTotalBetInGameInfo(IMiniGameInfo.Type.DragonTigerCasino);
		LeftGameAndHide();
    }
	#endregion

	private void OnDestroy() {
		StopAllCoroutines();
		DataManager.instance.userData.RemoveTotalBetInGameInfo(IMiniGameInfo.Type.DragonTigerCasino);
		dragonTigerCasinoData = null;
        DragonTiger_RealTimeAPI.SelfDestruction();
		instance = null;
	}
}

public class DragonTiger_CallbackManager
{
	public System.Action onDestructAllObject;
    public System.Action onStartShowBet;
	public System.Action onStartShowResult;
	public System.Action onEndShowResult;
}

[System.Serializable] public class DragonTiger_SortingLayerManager
{
   public MySortingLayerInfo sortingLayerInfo_ChipObject;
   public MySortingLayerInfo sortingLayerInfo_GoldObject;
}

[System.Serializable]public class DragonTiger_AudioInfo
{
	[Header("Playback")]
    public AudioClip bgm;
	[Header("Sfx")]
	public AudioClip sfx_DealCard;
	public AudioClip sfx_Card;
	public AudioClip sfx_ThrowChip;
	public AudioClip sfx_ShowResult;
	public AudioClip sfx_TogglePanelHistory;
}