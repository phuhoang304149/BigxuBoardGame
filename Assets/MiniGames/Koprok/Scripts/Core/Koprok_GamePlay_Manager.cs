using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;

public class Koprok_GamePlay_Manager : ISubGamePlayManager {

	public static Koprok_GamePlay_Manager instance;

	public enum GameState
    {
        Bet,
        ShowResult
    }
    public GameState currentGameState { get; set; }

	public enum IndexBet
    {
		Nai = 0,
        Bau, Ga, Ca, Cua, Tom
    }

	ScreenChatController screenChat{get;set;}
	[SerializeField] PanelListChipDetailController panelListChip;
	[SerializeField] PanelUserInfoInGameController panelUserInGame;
	[SerializeField] PanelDiskShockController diskShock;
	[SerializeField] Koprok_PanelHistory_Controller panelHistory;
	[SerializeField] GridLayoutGroup myTableBetGridLayoutGroup;
	[SerializeField] List<Koprok_Panel_TableBetOptionDetail_Controller> listTableBet;
	[SerializeField] RectTransform panelShadowTable;
	[SerializeField] MyArrowFocusController arrowFocusGetGold;
	[SerializeField] GameObject iconNotificationChat;
	[SerializeField] GameObject btnSetting;
	[SerializeField] GameObject btnMiniGame;
	[SerializeField] GameObject btnShop;
	[SerializeField] GameObject btnChat;

	public KoprokData koprokData;
	public Koprok_SortingLayerManager sortingLayerManager;
	public List<Sprite> spriteIconBet;

	public Koprok_CallbackManager callbackManager;

	[Header("Place Holder")]
	[SerializeField] Transform showEffPanelGoldBonusEffPlaceHolder;
	[SerializeField] Transform iconGoldHolder;

	[Header("Prefabs")]
	[SerializeField] GameObject screenChatPrefab;
	[SerializeField] GameObject goldPrefab;
	[SerializeField] GameObject chipPrefab;
	[SerializeField] GameObject panelBonusGoldPrefab;

	[Header("Audio Info")]
    public Koprok_AudioInfo myAudioInfo;

	public MySimplePoolManager effectPoolManager;
	IEnumerator actionRunProcessPlaying, actionRunProcessNonPlaying, actionCheckFocusIconGetGold;
	List<IEnumerator> listProcessPlaying;
	List<IEnumerator> listProcessNonPlaying;

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

		koprokData = new KoprokData();

		panelListChip.InitData();

		StartCoroutine(DoActionRun(_connectFirst, _onFinished));
	}

	IEnumerator DoActionRun(bool _connectFirst, System.Action _onFinished = null){
		yield return null;
		panelLoading.SetActive(true);
		
		StartCoroutine(ResizeEverythingAgain());

		InitAllCallback();

		actionRunProcessPlaying = DoActionRunProcessPlaying();
		StartCoroutine(actionRunProcessPlaying);

		actionRunProcessNonPlaying = DoActionRunProcessNonPlaying();
		StartCoroutine(actionRunProcessNonPlaying);

		actionCheckFocusIconGetGold = DoActionCheckFocusIconGetGold();
		StartCoroutine(actionCheckFocusIconGetGold);

		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_MINIGAME_BAUCUA_GET_GAMEINFO, (_mess)=>{
           	koprokData.InitDataWhenGetTableInfo(_mess);
		   	if(_mess.avaiable() > 0){
				#if TEST
				Debug.Log (">>> Chua doc het CMD : " + _mess.getCMDName ());
				#endif
			}

			koprokData.CheckListHistoryAgain();

			AchievementDetail _achievementDetail = DataManager.instance.achievementData.GetAchievementDetail(IMiniGameInfo.Type.Koprok);
			if(_achievementDetail != null){
				_achievementDetail.countWin = koprokData.winAchievement;
				_achievementDetail.countDraw = koprokData.tieAchievement;
				_achievementDetail.countLose = koprokData.loseAchievement;
				// Debug.Log(koprokData.winAchievement + " - " + koprokData.tieAchievement + " - " + koprokData.loseAchievement);
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
        yield return new WaitUntil(() => koprokData != null && koprokData.hasLoadGameInfo);
		panelLoading.SetActive(false);
        
		StartCountDown();

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

	void ResizeDiskShockAgain(){
		float _ratio = CoreGameManager.instance.currentSceneManager.mainCamera.sizeOfCamera.y * 0.25f / 5.62f;
		_ratio = Mathf.Clamp(_ratio, 0.25f, 0.4f);
		diskShock.SetOriginScaleAgain(_ratio, true);
	}

	IEnumerator ResizeEverythingAgain(){
		ResizeDiskShockAgain();

		RectTransform _canvasRectTransform = myCanvas.gameObject.GetComponent<RectTransform>();
		float _tmp = ((_canvasRectTransform.sizeDelta.y - 80f - 100f - 80f) - (1*20f)) / 2f; // (_h * 2 + 20) <= _tableBetRectTransform.sizeDelta.x - 120f;
		// Debug.Log(_canvasRectTransform.sizeDelta.y + " - " + _tmp);
		Vector2 _newSize = Vector2.one;
		_newSize.x = (int) Mathf.Clamp(_tmp, 190, 240);
		_newSize.y = (int) Mathf.Clamp(_tmp, 190, 240);
		myTableBetGridLayoutGroup.cellSize = _newSize;
		yield return null;
		for(int i = 0; i < listTableBet.Count; i++){
			listTableBet[i].SetSizeAgain();
		}
	}

	void InitAllCallback(){
		callbackManager = new Koprok_CallbackManager();

		callbackManager.onDestructAllObject = ()=>{
			effectPoolManager.ClearAllObjectsNow();
			panelListChip.SelfDestruction();
			
			panelHistory.SelfDestruction();
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
		};

		callbackManager.onStartShowResult += ()=>{
			panelHistory.Hide();
		};
		callbackManager.onEndShowResult += ()=>{
			if(koprokData.myIndexBet == null){
				koprokData.myIndexBet = new List<IndexBet>();
			}else{
				koprokData.myIndexBet.Clear();
			}
		};

		screenChat.onSendMessage = (_mess) =>
        {
            Koprok_RealTimeAPI.instance.SendMessageChat(_mess);
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
            if(koprokData != null){
                MyGamePlayData.AlertUpdateServer_Data _data = new MyGamePlayData.AlertUpdateServer_Data(_mess);
                System.TimeSpan _timeSpanRemain = _data.timeToUpdateServer - System.DateTime.Now;                
                PopupManager.Instance.CreateToast(string.Format(MyLocalize.GetString("System/Message_ServerMaintenance"), _timeSpanRemain.Minutes, _timeSpanRemain.Seconds));
            }
        });
    }
	void RegisterActionUpdateResultGame(){
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_MINIGAME_BAUCUA_PROCESS_RESULT, (_mess) =>
        {
			if(koprokData != null){
				koprokData.SetDataWhenShowResult(_mess);
				listProcessPlaying.Add(DoActionShowResult());
			}
		});
	}
	void RegisterActionMeAddBet(){
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_MINIGAME_BAUCUA_ADDBET, (_mess) =>
        {
			if(koprokData != null){
				koprokData.SetDataMeAddBet(_mess);
				listProcessNonPlaying.Add(DoActionCheckMeAddBet());
			}
        });
	}
	void RegisterActionUpdateTableBet(){
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_MINIGAME_BAUCUA_TABLE_BET, (_mess) =>
        {
			if(koprokData != null){
				koprokData.SetDataUpdateTableBet(_mess);
				listProcessNonPlaying.Add(DoActionCheckUpdateTableBet());
			}
        });
	}
	void RegisterActionPlayerChat(){
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_MINIGAME_BAUCUA_CHAT_ALL, (_mess) =>
        {   
			if(koprokData != null){
				koprokData.SetPlayerChatData(_mess);
				listProcessNonPlaying.Add(DoActionPlayerChat());
			}
        });
	}

	void RegisterActionPlayerAddGold()
    {
        NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_onPlayerAddGold, (_mess) =>
        {
			if(koprokData != null){
				koprokData.SetPlayerAddGoldData(_mess);
				listProcessNonPlaying.Add(DoActionPlayerAddGold());
			}
        });
    }

	void RegisterActionSetParentInfo(){
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_GAMEPLAY_SET_PARENT, (_mess) => 
		{
			if(koprokData != null){
				koprokData.SetDataWhenSetParent(_mess);
				listProcessNonPlaying.Add(DoActionPlayerSetParent());
			}
		});
	}
	#endregion

	#region Logic Game
	IEnumerator DoActionRunProcessPlaying(){
		while(true){
			if(koprokData == null || !koprokData.hasLoadGameInfo){
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
			if(koprokData == null || !koprokData.hasLoadGameInfo){
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
			if(koprokData == null || !koprokData.hasLoadGameInfo){
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

	void StartCountDown(){
		System.TimeSpan _tmpDeltaTime = koprokData.nextTimeToShowResult - System.DateTime.Now;
		diskShock.StartCountDown(_tmpDeltaTime.TotalSeconds, null);
	}

	IEnumerator DoActionPlayerChat(){
		// #if TEST
		// Debug.Log("<color=green> Start process PlayerChat </color>");
		// #endif
		KoprokData.SubGame_PlayerChat_Data _playerChatData = koprokData.processSubGamePlayerChatData[0];
		screenChat.AddMessage(_playerChatData.userData, _playerChatData.contentChat);
		_playerChatData = null;
		koprokData.processSubGamePlayerChatData.RemoveAt(0);
		// #if TEST
		// Debug.Log("<color=green> End process PlayerChat: " + koprokData.processSubGamePlayerChatData.Count +"</color>");
		// #endif
		yield break;
	}

	IEnumerator DoActionPlayerAddGold(){
		// #if TEST
		// Debug.Log("<color=green> Start process PlayerAddGold </color>");
		// #endif
		KoprokData.PlayerAddGold_Data _playerAddGoldData = koprokData.processPlayerAddGoldData[0];
		SetUpPlayerAddGold(_playerAddGoldData.sessionId, _playerAddGoldData.reason, _playerAddGoldData.goldAdd, _playerAddGoldData.goldLast);
		_playerAddGoldData = null;
		koprokData.processPlayerAddGoldData.RemoveAt(0);
		// #if TEST
		// Debug.Log("<color=green> End process PlayerAddGold: " + dragonTigerCasinoData.processPlayerAddGoldData.Count +"</color>");
		// #endif
		yield break;
	}

	IEnumerator DoActionPlayerSetParent(){
		KoprokData.PlayerSetParent_Data _playerSetParentData = koprokData.processPlayerSetParentData[0];
		koprokData.SetUpActionPlayerSetParent(_playerSetParentData);
		_playerSetParentData = null;
		koprokData.processPlayerSetParentData.RemoveAt(0);
		yield break;
	}

	IEnumerator DoActionCheckMeAddBet(){
		KoprokData.Koprok_MeAddBet_Data _meAddBetData = koprokData.processMeAddBetData[0];

		if(currentGameState == GameState.Bet){
			yield return StartCoroutine(DoActionMeAddBet(_meAddBetData));
		}else{
			listProcessPlaying.Add(DoActionMeAddBet(_meAddBetData));
		}

		_meAddBetData = null;
		koprokData.processMeAddBetData.RemoveAt(0);
	}

	IEnumerator DoActionMeAddBet(KoprokData.Koprok_MeAddBet_Data _meAddBetData){
		DataManager.instance.userData.gold = _meAddBetData.myGOLD;
		DataManager.instance.userData.SetTotalBetInGameInfo(IMiniGameInfo.Type.Koprok, _meAddBetData.myTotalBet);

		if(_meAddBetData.isAddOk){
			#if TEST
			Debug.Log(">>> MeAddBet: " + _meAddBetData.indexBet + " - " + _meAddBetData.countBet + " - " + _meAddBetData.globalBet + " - " + _meAddBetData.myBet + " - " + DataManager.instance.userData.gold);
			#endif
			koprokData.tableCount[_meAddBetData.indexBet] = _meAddBetData.countBet;
			koprokData.tableGlobalBet[_meAddBetData.indexBet] = _meAddBetData.globalBet;
			koprokData.tableMyBet[_meAddBetData.indexBet] = _meAddBetData.myBet;

			IndexBet _tmpParseToIndexBet = (IndexBet)_meAddBetData.indexBet;
			if(!koprokData.myIndexBet.Contains(_tmpParseToIndexBet)){
				koprokData.myIndexBet.Add(_tmpParseToIndexBet);
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
			for(int i = 0; i < _meAddBetData.tableMyBet.Count; i++){
				koprokData.tableMyBet[i] = _meAddBetData.tableMyBet[i];
			}

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
		KoprokData.Koprok_UpdateTableBet_Data _updateTableBetData = koprokData.processUpdateTableBetData[0];

		if(currentGameState == GameState.Bet){
			yield return StartCoroutine(DoActionUpdateTableBet(_updateTableBetData));
		}else{
			listProcessPlaying.Add(DoActionUpdateTableBet(_updateTableBetData));
		}

		_updateTableBetData = null;
		koprokData.processUpdateTableBetData.RemoveAt(0);
		yield break;
	}

	IEnumerator DoActionUpdateTableBet(KoprokData.Koprok_UpdateTableBet_Data _updateTableBetData){
		#if TEST
		Debug.Log(">>> UpdateTableBet: " + _updateTableBetData.indexBet + " - " + _updateTableBetData.tableCount + " - " + _updateTableBetData.tableBet + " - " + DataManager.instance.userData.gold);
		#endif
		koprokData.tableCount[_updateTableBetData.indexBet] = _updateTableBetData.tableCount;
		koprokData.tableGlobalBet[_updateTableBetData.indexBet] = _updateTableBetData.tableBet;
		
		if(currentGameState == GameState.Bet){
			RefreshUITableBet();
		}else{
			#if TEST
			Debug.LogError(">>> Không phải state Bet");
			#endif
		}
		yield break;
	}

	IEnumerator DoActionShowResult()
    {
		if (currentGameState == GameState.ShowResult)
        {
            Debug.LogError("Đang show result rồi");
            yield break;
        }

		KoprokData.Koprok_Result_Data _resultData = koprokData.processResultData[0];

		currentGameState = GameState.ShowResult;

		// ---- Merge dữ liệu ---- //
		Koprok_GamePlay_Manager.IndexBet _slot1 = (Koprok_GamePlay_Manager.IndexBet) _resultData.dice[0];
		Koprok_GamePlay_Manager.IndexBet _slot2 = (Koprok_GamePlay_Manager.IndexBet) _resultData.dice[1];
		Koprok_GamePlay_Manager.IndexBet _slot3 = (Koprok_GamePlay_Manager.IndexBet) _resultData.dice[2];
		koprokData.listHistory.Insert(0, new KoprokData.Koprok_History_Data(_slot1, _slot2, _slot3));
		
		koprokData.CheckListHistoryAgain();
		koprokData.ResetTableBet();
		koprokData.nextTimeToShowResult = _resultData.nextTimeToShowResult;
		DataManager.instance.userData.gold = _resultData.GOLD;
		DataManager.instance.userData.SetTotalBetInGameInfo(IMiniGameInfo.Type.Koprok, 0);

		if(_resultData.caseCheck == 1){
			AchievementDetail _achievementDetail = DataManager.instance.achievementData.GetAchievementDetail(IMiniGameInfo.Type.Koprok);
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

		List<Sprite> _listSpriteBetWin = new List<Sprite>();
		for(int i = 0; i < _resultData.dice.Count; i++){
			_listSpriteBetWin.Add(spriteIconBet[(int) _resultData.dice[i]]);
		}
		diskShock.SetItemInfo(_listSpriteBetWin, true, true);

		SetUpShadowTable(null, true);
		yield return StartCoroutine(diskShock.DoActionShowResult(myAudioInfo.sfx_DiceMove));
		yield return StartCoroutine(DoActionHighlightIndexBet(_resultData.dice));

		if(_resultData.betUnit > 0){
			StartCoroutine(MyConstant.DoActionShowPopupWinGold(panelBonusGoldPrefab, effectPoolManager, myCanvas.transform
				, showEffPanelGoldBonusEffPlaceHolder.position, 1.1f, _resultData.betUnit
				, ()=>{
					panelUserInGame.RefreshGoldInfo();
				}));
 			List<IndexBet> _myBetWin = new List<IndexBet>();
			for(int i = 0; i < _resultData.dice.Count; i++){
				if(koprokData.myIndexBet.Contains(_resultData.dice[i])){
					_myBetWin.Add(_resultData.dice[i]);
				}
			}
			yield return StartCoroutine(DoActionShowEffWinGoldAtIndex(_myBetWin, (Vector2) iconGoldHolder.position));
		}

		yield return StartCoroutine(diskShock.DoActionShockAgainAndReset(myAudioInfo.sfx_DiceMove, myAudioInfo.sfx_DiceShake));
		
		SetUpShadowTable(_resultData.dice, false);

		if(callbackManager != null && callbackManager.onEndShowResult != null){
			callbackManager.onEndShowResult();
		}

		_resultData = null;
		koprokData.processResultData.RemoveAt(0);
		
		SetBetAgain();
	}
	void SetBetAgain(){
		currentGameState = GameState.Bet;
		if(callbackManager != null && callbackManager.onStartShowBet != null){
			callbackManager.onStartShowBet();
		}
	}

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
		Koprok_RealTimeAPI.instance.SendMessageAddBet((byte) _indexBet, (short) panelListChip.currentChip.index, panelListChip.currentChip.chipInfo.value);
	}

	public override void LeftGameAndHide(){
		NetworkGlobal.instance.SendMessageRealTime (new MessageSending (CMD_REALTIME.C_MINIGAME_BAUCUA_LEFT_GAME));
        if(callbackManager != null && callbackManager.onDestructAllObject != null){
			callbackManager.onDestructAllObject();
		}
		StopAllCoroutines();
		Hide();
		MyAudioManager.instance.StopAll();
		DataManager.instance.miniGameData.currentSubGameDetail = null;
	}

	public void SetUpPlayerAddGold(short _sessionid, int _reason, long _goldAdd, long _goldLast){
		if(_sessionid != DataManager.instance.userData.sessionId){
			return;
		}
		DataManager.instance.userData.gold = _goldLast;

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
		if(koprokData == null || koprokData.tableCount.Count != 6 || koprokData.tableGlobalBet.Count != 6 || koprokData.tableMyBet.Count != 6){
			#if TEST
			Debug.LogError("Lỗi data koprokData !");
			#endif
			return;
		}
		for(int i = 0; i < listTableBet.Count; i++){
			listTableBet[i].SetCountBet(koprokData.tableCount[i], _isNow);
			listTableBet[i].SetGlobalBet(koprokData.tableGlobalBet[i], 999999, _isNow);
			listTableBet[i].SetMyBet(koprokData.tableMyBet[i], 999999, _isNow);
		}
	}
	#endregion

	#region Utilities
	public void SetUpShadowTable(List<IndexBet> _listIndexExcept, bool _active){
		if(_active){
			LeanTween.alpha(panelShadowTable, 0.7f, 0.1f);
		}else{
			LeanTween.alpha(panelShadowTable, 0f, 0.1f);
		}
		for(int i = 0; i < listTableBet.Count; i++){
			bool _tmp = true;
			if(_listIndexExcept != null){
				for(int j = 0; j < _listIndexExcept.Count; j++){
					if(i == (int)_listIndexExcept[j]){
						_tmp = false;
						break;
					}
				}
			}
			if(_tmp){
				listTableBet[i].SetShadow(_active);
			}
		}
	}
	IEnumerator DoActionHighlightIndexBet(List<IndexBet> _listIndex){
		IEnumerator[] _listIEnumerator = new IEnumerator[_listIndex.Count];
		for(int i = 0; i < _listIndex.Count; i++){
			_listIEnumerator[i] = listTableBet[(int)_listIndex[i]].Highlight();
		}

		MyAudioManager.instance.PlaySfx(myAudioInfo.sfx_ShowResult);

		yield return CoroutineChain.Start.Parallel(_listIEnumerator);
	}

	IEnumerator DoActionShowEffWinGoldAtIndex(List<IndexBet> _listIndex, Vector2 _endPoint){
		IEnumerator[] _listIEnumerator = new IEnumerator[_listIndex.Count];
		for(int i = 0; i < _listIndex.Count; i++){
			_listIEnumerator[i] = MyConstant.DoActionShowEffectGoldFly(goldPrefab, effectPoolManager, sortingLayerManager.sortingLayerInfo_GoldObject
				, listTableBet[(int)_listIndex[i]].transform.position, _endPoint, 10, ratioScale, 0.8f
				, ()=>{
					MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Gold);
				});
		}
		yield return CoroutineChain.Start.Parallel(_listIEnumerator);
	}

	public void ShowEffPlayerAddBet(short _chipIndex, sbyte _indexBet){
		if(currentGameState != GameState.Bet){
			return;
		}
		if(_chipIndex == -1){
			Debug.LogError(">>> chipIndex = -1");
			return;
		}
		
		Vector2 _posStart = iconGoldHolder.position;
		Vector2 _posEnd = listTableBet[_indexBet].transform.position;
		_posEnd.x = Random.Range(_posEnd.x - 0.2f, _posEnd.x + 0.2f);
		_posEnd.y = Random.Range(_posEnd.y - 0.2f, _posEnd.y + 0.2f);
		
		IChipInfo _chipInfo = panelListChip.listChipDetail[_chipIndex].chipInfo;
		ChipObjectController _tmpChip = LeanPool.Spawn(chipPrefab.gameObject, _posStart, Quaternion.identity).GetComponent<ChipObjectController>();
		effectPoolManager.AddObject(_tmpChip);
		_tmpChip.SetData(_chipInfo, sortingLayerManager.sortingLayerInfo_ChipObject , ratioScale);
		_tmpChip.SetUpMoveToTableBet(_posEnd);
		_tmpChip.RegisCallbackJustMoveFinished(_indexBet, (_i)=>{
			MyAudioManager.instance.PlaySfx(myAudioInfo.sfx_Bet);
		});
	}
	#endregion

	#region On Button Clicked
	public void OnButtonShowHistory(){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);

		if(panelHistory.currentState == Koprok_PanelHistory_Controller.State.Show){
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
		DataManager.instance.userData.RemoveTotalBetInGameInfo(IMiniGameInfo.Type.Koprok);
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
		DataManager.instance.userData.RemoveTotalBetInGameInfo(IMiniGameInfo.Type.Koprok);
		LeftGameAndHide();
    }
	#endregion

	private void OnDestroy() {
		StopAllCoroutines();
		DataManager.instance.userData.RemoveTotalBetInGameInfo(IMiniGameInfo.Type.Koprok);
		koprokData = null;
        Koprok_RealTimeAPI.SelfDestruction();
		instance = null;
	}
}

public class Koprok_CallbackManager
{
	public System.Action onDestructAllObject;
    public System.Action onStartShowBet;
	public System.Action onStartShowResult;
	public System.Action onEndShowResult;
}

[System.Serializable] public class Koprok_SortingLayerManager
{
   public MySortingLayerInfo sortingLayerInfo_ChipObject;
   public MySortingLayerInfo sortingLayerInfo_GoldObject;
}

[System.Serializable] public class Koprok_AudioInfo
{
	[Header("Playback")]
    public AudioClip bgm;
	[Header("Sfx")]
	public AudioClip sfx_DiceShake;
	public AudioClip sfx_DiceMove;
	public AudioClip sfx_ShowResult;
	public AudioClip sfx_Bet;
	public AudioClip sfx_Gold;
	public AudioClip sfx_TogglePanelHistory;
}