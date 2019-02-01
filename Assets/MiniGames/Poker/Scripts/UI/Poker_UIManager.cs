using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;

public class Poker_UIManager : MonoBehaviour {

	[Header("Panels")]
	public Transform panelTotalBet;
	public Text txtTotalBet;
	[SerializeField] Text txtTableInfo;
	[SerializeField] Text txtTimeCountDownToStartGame;
	[SerializeField] Transform panelRoleBigBlind;
	[SerializeField] Transform panelRoleSmallBlind;
	public Poker_PanelTypeCardResult_Controller panelTypeCardResult;
	public Poker_MyBar_Controller myBarController;
	[SerializeField] MyArrowFocusController arrowFocusGetGold;
	[SerializeField] List<Text> listIndexCircle;
	public PanelUserInfoInGameController myPanelUserInfo;
	public Poker_PanelHistory_Controller panelHistory;
	public Poker_PanelCardRanking_Controller panelCardRanking;
	public Poker_PanelSupport_Controller panelSupport;

	[Header("Place Holder")]
	[SerializeField] Transform placeHolder_SpawnCard;
	[SerializeField] List<Transform> placeHolder_GlobalCards;
	[SerializeField] Transform panelGroupPlayerHolderContainer;

	[Header("Prefabs")]
	[SerializeField] GameObject cardPrefab;
	
	IEnumerator actionCountDownToStartGame;

	public PokerGamePlayData pokerGamePlayData{
		get{
			return Poker_GamePlay_Manager.instance.pokerGamePlayData;
		}
	}
	public MySimplePoolManager globalCardsPoolManager{
		get{
			return Poker_GamePlay_Manager.instance.globalCardsPoolManager;
		}
	}

	TransformPlaceHolder placeHolder_SpawnCard_Catched;
	List<TransformPlaceHolder> placeHolder_GlobalCards_Catched;

	long realBet, virtualBet;
	IEnumerator actionTweenBet;
	LTDescr tweenMyPanelUserInfo;

	float posShowMyPanelUserInfo, posHideMyPanelUserInfo;

	public void InitFirst(){
		string _tmpTableInfo = "";
		_tmpTableInfo += DataManager.instance.miniGameData.currentMiniGameDetail.currentServerDetail.subServerName + "\n";
		_tmpTableInfo += string.Format("Table {0:00}", DataManager.instance.miniGameData.currentMiniGameDetail.tableData.currentTableDetail.tableId);
		txtTableInfo.text = _tmpTableInfo;

		panelTotalBet.gameObject.SetActive(false);
		txtTotalBet.text = "0";
		txtTimeCountDownToStartGame.text = "";
		placeHolder_SpawnCard_Catched = new TransformPlaceHolder(placeHolder_SpawnCard);

		placeHolder_GlobalCards_Catched = new List<TransformPlaceHolder>();
		for(int i = 0; i < placeHolder_GlobalCards.Count; i ++) {
			TransformPlaceHolder _placeHolderCatched = new TransformPlaceHolder(placeHolder_GlobalCards[i]);
			placeHolder_GlobalCards_Catched.Add(_placeHolderCatched);
		}

		Destroy(placeHolder_SpawnCard.gameObject);
		Destroy(placeHolder_GlobalCards[0].parent.gameObject);
		Destroy(panelGroupPlayerHolderContainer.gameObject);
		placeHolder_GlobalCards.Clear();

		for(int i = 0; i < Poker_GamePlay_Manager.instance.listPlayerGroup.Count; i ++){
			Poker_GamePlay_Manager.instance.listPlayerGroup[i].InitFirst();
		}

		panelRoleBigBlind.gameObject.SetActive(false);
		panelRoleSmallBlind.gameObject.SetActive(false);

		RectTransform _tmp = myPanelUserInfo.GetComponent<RectTransform>();
		float _sizeH = _tmp.sizeDelta.y;
		posShowMyPanelUserInfo = _tmp.localPosition.y;
		posHideMyPanelUserInfo = posShowMyPanelUserInfo - _sizeH;
		myPanelUserInfo.InitData();

		myBarController.Hide();
	}

	public void SetGlobalBet(long _bet, bool _updateNow = false){
		realBet = _bet;
		if(_updateNow){
			if(actionTweenBet != null){
				StopCoroutine(actionTweenBet);
				actionTweenBet = null;
			}
			virtualBet = realBet;
			txtTotalBet.text = MyConstant.GetMoneyString(virtualBet, 999999);
		}else{
			if(actionTweenBet != null){
				StopCoroutine(actionTweenBet);
				actionTweenBet = null;
			}
			actionTweenBet = MyConstant.TweenValue(virtualBet, realBet, 5, (_valueUpdate)=>{
				virtualBet = _valueUpdate;
				txtTotalBet.text = MyConstant.GetMoneyString(virtualBet, 999999);
			}, (_valueFinish)=>{
				virtualBet = _valueFinish;
				txtTotalBet.text = MyConstant.GetMoneyString(virtualBet, 999999);
				actionTweenBet = null;
			});
			StartCoroutine(actionTweenBet);
		}
	}

	public Coroutine DealGlobalCard(List<sbyte> _valueCards){
		if(globalCardsPoolManager.listObjects.Count >= 5){
			#if TEST
			Debug.LogError("Đã chia đủ 5 lá");
			#endif
			return null;
		}
		return StartCoroutine(DoActionDealGlobalCard(_valueCards));
	}

	IEnumerator DoActionDealGlobalCard(List<sbyte> _valueCards){
		List<ICardInfo> _tmpListCardInfo = new List<ICardInfo>();
		ICardInfo _cardInfo = null;
		List<PanelCardDetailController> _tmpListCard = new List<PanelCardDetailController>();
		PanelCardDetailController _card = null;
		for(int i = 0; i < _valueCards.Count; i ++){
			_cardInfo = Poker_GamePlay_Manager.instance.GetCardInfo(_valueCards[i]);
			if(_cardInfo == null){
				#if TEST
				Debug.LogError("_cardInfo is NULL : " + _valueCards[i]);
				#endif
				continue;
			}
			_tmpListCardInfo.Add(_cardInfo);

			_card = LeanPool.Spawn(cardPrefab, Vector3.zero, Quaternion.identity, Poker_GamePlay_Manager.instance.panelCardContainer).GetComponent<PanelCardDetailController>();
			_card.transform.position = placeHolder_SpawnCard_Catched.position;
			_card.ResizeAgain(Poker_GamePlay_Manager.instance.sizeCard.x, Poker_GamePlay_Manager.instance.sizeCard.y);
			_tmpListCard.Add(_card);
		}
		yield return Yielders.Get(0.1f);
		for(int i = 0; i < _tmpListCard.Count; i ++){
			CoroutineChain.Start
				.Sequential(_tmpListCard[i].Move(placeHolder_GlobalCards_Catched[globalCardsPoolManager.listObjects.Count].position, 0.1f, LeanTweenType.easeOutSine, Poker_GamePlay_Manager.instance.CanPlayMusicAndSfx() ? Poker_GamePlay_Manager.instance.myAudioInfo.sfx_DealCard : null)
					, _tmpListCard[i].Show(_tmpListCardInfo[i]));
			globalCardsPoolManager.AddObject((MySimplePoolObjectController) _tmpListCard[i]);
			yield return Yielders.Get(0.1f);
		}
	}

	public Coroutine DealPlayerCard(Poker_PlayerGroup _playerGroup){
		if(!_playerGroup.isInitialized){
			#if TEST
			Debug.LogError("Chưa Init Player");
			#endif
			return null;
		}
		if(_playerGroup.ownCardPoolManager.listObjects.Count != 0){
			#if TEST
			Debug.LogError("Đã chia đủ lá cho người chơi");
			#endif
			return null;
		}
		return StartCoroutine(DoActionDealPlayerCard(_playerGroup));
	}

	IEnumerator DoActionDealPlayerCard(Poker_PlayerGroup _playerGroup){
		List<PanelCardDetailController> _tmpListCard = new List<PanelCardDetailController>();
		PanelCardDetailController _card = null;
		for(int i = 0; i < 2; i ++){
			_card = LeanPool.Spawn(cardPrefab, Vector3.zero, Quaternion.identity, Poker_GamePlay_Manager.instance.panelCardContainer).GetComponent<PanelCardDetailController>();
			_card.transform.position = placeHolder_SpawnCard_Catched.position;
			_card.ResizeAgain(Poker_GamePlay_Manager.instance.sizeCard.x, Poker_GamePlay_Manager.instance.sizeCard.y);
			_tmpListCard.Add(_card);
		}

		yield return Yielders.Get(0.1f);

		for(int i = 0; i < _tmpListCard.Count; i ++){
			if(!_playerGroup.isMe){
				if(Poker_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
					MyAudioManager.instance.PlaySfx(Poker_GamePlay_Manager.instance.myAudioInfo.sfx_DealCard);
				}
				CoroutineChain.Start
					.Parallel(_tmpListCard[i].Move(_playerGroup.cardCoverHoldersCatched[i].position, 0.1f, LeanTweenType.easeOutSine)
						, _tmpListCard[i].Rotate(_playerGroup.cardCoverHoldersCatched[i].rotation.eulerAngles, 0.1f)
						, _tmpListCard[i].ScaleTo(_playerGroup.cardCoverHoldersCatched[i].localScale, 0.1f, LeanTweenType.notUsed));
			}else{
				ICardInfo _cardInfo = null;
				
				for(int j = 0; j < pokerGamePlayData.listPlayerPlayingData.Count; j ++){
					if(pokerGamePlayData.listPlayerPlayingData[j].isMe
						&& pokerGamePlayData.listPlayerPlayingData[j].userData.IsEqual(_playerGroup.userData)){
						_cardInfo = Poker_GamePlay_Manager.instance.GetCardInfo(pokerGamePlayData.listPlayerPlayingData[j].ownCards[i]);
						if(_cardInfo == null){
							#if TEST
							Debug.LogError("_cardInfo is NULL : " + pokerGamePlayData.listPlayerPlayingData[j].ownCards[i]);
							#endif
							continue;
						}
						break;
					}
				}
				
				CoroutineChain.Start
					.Parallel(_tmpListCard[i].Move(_playerGroup.ownCardHoldersCatched[i].position, 0.1f, LeanTweenType.easeOutSine
								, Poker_GamePlay_Manager.instance.CanPlayMusicAndSfx() ? Poker_GamePlay_Manager.instance.myAudioInfo.sfx_DealCard : null)
						, _tmpListCard[i].Rotate(_playerGroup.ownCardHoldersCatched[i].rotation.eulerAngles, 0.1f)
						, _tmpListCard[i].ScaleTo(_playerGroup.ownCardHoldersCatched[i].localScale, 0.1f, LeanTweenType.notUsed))
					.Sequential(_tmpListCard[i].Show(_cardInfo));
			}
			_playerGroup.ownCardPoolManager.AddObject(_tmpListCard[i]);
			yield return Yielders.Get(0.1f);
		}
	}

	public Coroutine ShowCard(List<sbyte> _valueCards, Poker_PlayerGroup _playerGroup){
		if(_valueCards.Count != 2){
			#if TEST
			Debug.LogError("Không đủ 2 lá");
			#endif
			return null;
		}
		if(!_playerGroup.isInitialized){
			#if TEST
			Debug.LogError("Chưa Init Player");
			#endif
			return null;
		}
		if(_playerGroup.ownCardPoolManager == null || _playerGroup.ownCardPoolManager.listObjects.Count == 0){
			#if TEST
			Debug.LogError("Chưa chia bài");
			#endif
			return null;
		}
		return StartCoroutine(DoActionShowCard(_valueCards, _playerGroup));
	}
	IEnumerator DoActionShowCard(List<sbyte> _valueCards, Poker_PlayerGroup _playerGroup){
		ICardInfo _cardInfo = null;
		for(int i = 0; i < _valueCards.Count; i ++){
			_cardInfo = Poker_GamePlay_Manager.instance.GetCardInfo(_valueCards[i]);
			if(_cardInfo == null){
				#if TEST
				Debug.LogError("_cardInfo is NULL : " + _valueCards[i]);
				#endif
				continue;
			}
			PanelCardDetailController _panelCardDetail = (PanelCardDetailController) _playerGroup.ownCardPoolManager.listObjects[i];
			CoroutineChain.Start
					.Parallel(_panelCardDetail.Move(_playerGroup.cardOpenHoldersCatched[i].position, 0.1f, LeanTweenType.easeOutSine)
								, _panelCardDetail.Rotate(_playerGroup.cardOpenHoldersCatched[i].rotation.eulerAngles, 0.1f)
								, _panelCardDetail.ScaleTo(_playerGroup.cardOpenHoldersCatched[i].localScale, 0.1f, LeanTweenType.notUsed))
					.Sequential(_panelCardDetail.Show(_cardInfo));
		}
		yield return 1f;
	}

	public Coroutine ClearAllCards(){
		return StartCoroutine(DoActionClearAllCards());
	}

	IEnumerator DoActionClearAllCards(){
		yield return CoroutineChain.Start
			.Parallel(DoActionClearAllGlobalCards(), DoActionClearAllPlayerCards());
		yield return Yielders.Get(0.1f);
		globalCardsPoolManager.ClearAllObjectsNow();
		for(int i = 0; i < Poker_GamePlay_Manager.instance.listPlayerGroup.Count; i ++){
			if(!Poker_GamePlay_Manager.instance.listPlayerGroup[i].isInitialized){
				continue;
			}
			Poker_GamePlay_Manager.instance.listPlayerGroup[i].ownCardPoolManager.ClearAllObjectsNow();
		}
	}

	IEnumerator DoActionClearAllGlobalCards(){
		if(globalCardsPoolManager != null && globalCardsPoolManager.listObjects.Count > 0){
			for(int i = 0; i < globalCardsPoolManager.listObjects.Count; i ++){
				yield return CoroutineChain.Start
					.Parallel(((PanelCardDetailController) globalCardsPoolManager.listObjects[i]).Move(placeHolder_SpawnCard_Catched.position, 0.1f, LeanTweenType.easeOutSine, Poker_GamePlay_Manager.instance.CanPlayMusicAndSfx() ? Poker_GamePlay_Manager.instance.myAudioInfo.sfx_Card : null)
								, ((PanelCardDetailController) globalCardsPoolManager.listObjects[i]).Rotate(placeHolder_SpawnCard_Catched.rotation.eulerAngles, 0.1f));
			}
		}
	}

	IEnumerator DoActionClearAllPlayerCards(){
		for(int i = 0; i < Poker_GamePlay_Manager.instance.listPlayerGroup.Count; i ++){
			if(!Poker_GamePlay_Manager.instance.listPlayerGroup[i].isInitialized){
				continue;
			}
			for(int j = 0; j < Poker_GamePlay_Manager.instance.listPlayerGroup[i].ownCardPoolManager.listObjects.Count; j++){
				PanelCardDetailController _cardDetail = (PanelCardDetailController) Poker_GamePlay_Manager.instance.listPlayerGroup[i].ownCardPoolManager.listObjects[j];
				CoroutineChain.Start
					.Parallel(_cardDetail.Move(placeHolder_SpawnCard_Catched.position, 0.1f, LeanTweenType.easeOutSine, Poker_GamePlay_Manager.instance.CanPlayMusicAndSfx() ? Poker_GamePlay_Manager.instance.myAudioInfo.sfx_Card : null)
								, _cardDetail.Rotate(placeHolder_SpawnCard_Catched.rotation.eulerAngles, 0.1f));
				yield return null;
			}
		}
		yield return Yielders.Get(0.5f);
	}

	#region Refresh UI
	public void RefreshAllUINow(){
		if(pokerGamePlayData == null || !pokerGamePlayData.hasLoadTableInfo){
			Debug.LogError(">>> pokerGamePlayData is NULL");
			return;
		}

		if(pokerGamePlayData.currentGameState == PokerGamePlayData.GameState.STATUS_WAIT_FOR_PLAYER){
			panelTotalBet.gameObject.SetActive(false);
		}else{
			panelTotalBet.gameObject.SetActive(false);
		}

		RefreshUIIndexCircle();
		RefreshAllPlayerGroupUINow();
		RefreshGlobalCardUINow();
		RefreshUIButtonSitDown();
		myBarController.RefreshUI();
	}

	void RefreshGlobalCardUINow(){
		if(pokerGamePlayData.globalCards.Count == 0){
			return;
		}
		ICardInfo _cardInfo = null;
		PanelCardDetailController _card = null;
		Vector3 _pos = Vector3.zero;
		for(int i = 0; i < pokerGamePlayData.globalCards.Count; i ++){
			_cardInfo = Poker_GamePlay_Manager.instance.GetCardInfo(pokerGamePlayData.globalCards[i]);
			if(_cardInfo == null){
				#if TEST
				Debug.LogError("_cardInfo is NULL : " + pokerGamePlayData.globalCards[i]);
				#endif
				continue;
			}

			_pos = placeHolder_GlobalCards_Catched[globalCardsPoolManager.listObjects.Count].position;

			_card = LeanPool.Spawn(cardPrefab, Vector3.zero, Quaternion.identity, Poker_GamePlay_Manager.instance.panelCardContainer).GetComponent<PanelCardDetailController>();
			_card.transform.position = _pos;
			_card.ResizeAgain(Poker_GamePlay_Manager.instance.sizeCard.x, Poker_GamePlay_Manager.instance.sizeCard.y);
			_card.ShowNow(_cardInfo);
			globalCardsPoolManager.AddObject(_card);
		}
	}

	void RefreshAllPlayerGroupUINow(){
		if(pokerGamePlayData.currentGameState == PokerGamePlayData.GameState.STATUS_WAIT_FOR_PLAYER){
			for(int i = 0; i < pokerGamePlayData.listSessionIdOnChair.Count; i++){
				if(pokerGamePlayData.listSessionIdOnChair[i] < 0){ // ghế trống
					continue;
				}
				for(int j = 0; j < pokerGamePlayData.listGlobalPlayerData.Count; j++){
					if(pokerGamePlayData.listSessionIdOnChair[i] == pokerGamePlayData.listGlobalPlayerData[j].sessionId){
						Poker_GamePlay_Manager.instance.listPlayerGroup[i].InitData(pokerGamePlayData.listGlobalPlayerData[j]);
						break;
					}
				}
			}
		}else{ // đang chơi thì hiện thông tin người chơi và chia bài cho họ
			List<int> _listIndexChairPlaying = new List<int>();
			Vector3 _pos = Vector3.zero;
			Quaternion _rot = Quaternion.identity;
			PanelCardDetailController _card = null;
			for(int i = 0; i < pokerGamePlayData.listPlayerPlayingData.Count; i++){
				_listIndexChairPlaying.Add(pokerGamePlayData.listPlayerPlayingData[i].indexChair);
				int _indexChair = pokerGamePlayData.listPlayerPlayingData[i].indexChair;
				
				Poker_PlayerGroup _playerGroup = Poker_GamePlay_Manager.instance.listPlayerGroup[_indexChair];
				_playerGroup.InitData(pokerGamePlayData.listPlayerPlayingData[i].userData);
				_playerGroup.ShowPlayerState(pokerGamePlayData.listPlayerPlayingData[i].currentState, true);
				if(pokerGamePlayData.listPlayerPlayingData[i].turnBet > 0){	
					_playerGroup.myPanelBet.SetBet(pokerGamePlayData.listPlayerPlayingData[i].turnBet, true);
					_playerGroup.myPanelBet.Show();
				}

				if(pokerGamePlayData.listPlayerPlayingData[i].currentState != PokerGamePlayData.Poker_PlayerPlayingData.State.STATEPOKER_FOLD){
					for(int j = 0; j < _playerGroup.cardCoverHoldersCatched.Count; j ++){
						_pos = _playerGroup.cardCoverHoldersCatched[j].position;
						_rot = _playerGroup.cardCoverHoldersCatched[j].rotation;

						_card = LeanPool.Spawn(cardPrefab, Vector3.zero, Quaternion.identity, Poker_GamePlay_Manager.instance.panelCardContainer).GetComponent<PanelCardDetailController>();
						_card.transform.position = _pos;
						_card.transform.rotation = _rot;
						_card.ResizeAgain(Poker_GamePlay_Manager.instance.sizeCard.x, Poker_GamePlay_Manager.instance.sizeCard.y);
						_playerGroup.ownCardPoolManager.AddObject(_card);
					}
				}

				if(i == 0){
					panelRoleSmallBlind.gameObject.SetActive(true);
					panelRoleSmallBlind.position = _playerGroup.myPanelBet.imgIconChip.transform.position;
				}else if(i == 1){
					panelRoleBigBlind.gameObject.SetActive(true);
					panelRoleBigBlind.position = _playerGroup.myPanelBet.imgIconChip.transform.position;
				}
			}
			// --- hiện các thông tin người chơi đang chờ --- //
			List<int> _listIndexChairWaiting = new List<int>();
			for(int i = 0; i < pokerGamePlayData.numberChairs; i++){
				if(!_listIndexChairPlaying.Contains(i)){
					_listIndexChairWaiting.Add(i);
				}
			}
			for(int i = 0; i < _listIndexChairWaiting.Count; i++){
				int _indexChair = _listIndexChairWaiting[i];
				short _sessionId = pokerGamePlayData.listSessionIdOnChair[_indexChair];
				UserDataInGame _userData = pokerGamePlayData.GetUserDataInGameFromListGlobal(_sessionId);
				if(_userData == null){
					continue;
				}
				
				Poker_PlayerGroup _playerGroup = Poker_GamePlay_Manager.instance.listPlayerGroup[_indexChair];
				_playerGroup.InitAsIncognito(_userData);
			}
		}
	}

	public void RefreshUIButtonSitDown(){
		if(pokerGamePlayData.CheckIsPlaying(DataManager.instance.userData.sessionId)
			|| pokerGamePlayData.listSessionIdOnChair.Contains(DataManager.instance.userData.sessionId)){
			for(int i = 0; i < Poker_GamePlay_Manager.instance.listPlayerGroup.Count; i++){
				Poker_GamePlay_Manager.instance.listPlayerGroup[i].HideButtonSitDown();
			}
			HideMyPanelUserInfo();
			return;
		}
		
		ShowMyPanelUserInfo();

		List<int> _listIndexChairPlaying = new List<int>();
		for(int i = 0; i < pokerGamePlayData.listPlayerPlayingData.Count; i++){
			int _indexChair = pokerGamePlayData.listPlayerPlayingData[i].indexChair;
			Poker_GamePlay_Manager.instance.listPlayerGroup[_indexChair].HideButtonSitDown();
			_listIndexChairPlaying.Add(_indexChair);
		}
		List<int> _listIndexChairWaiting = new List<int>();
		for(int i = 0; i < pokerGamePlayData.numberChairs; i++){
			if(!_listIndexChairPlaying.Contains(i)){
				_listIndexChairWaiting.Add(i);
			}
		}
		Poker_PlayerGroup _playerGroup = null;
		for(int i = 0; i < _listIndexChairWaiting.Count; i++){
			int _indexChair = _listIndexChairWaiting[i];
			short _sessionId = pokerGamePlayData.listSessionIdOnChair[_indexChair];
			_playerGroup = Poker_GamePlay_Manager.instance.listPlayerGroup[_indexChair];
			UserDataInGame _userData = pokerGamePlayData.GetUserDataInGameFromListGlobal(_sessionId);
			if(_userData == null){
				_playerGroup.ShowButtonSitDown();
			}else{
				_playerGroup.HideButtonSitDown();
			}
		}
	}

	public bool IsCountingDown(){
		return actionCountDownToStartGame != null;
	}

	public void StopShowCountDownStartGame(){
		if(actionCountDownToStartGame != null){
			StopCoroutine(actionCountDownToStartGame);
			actionCountDownToStartGame = null;
		}
		txtTimeCountDownToStartGame.text = string.Empty;
	}

	public void ShowCountDownStartGame(double _timeCountDown){
		if(_timeCountDown < 1f){
			return;
		}
		if(actionCountDownToStartGame != null){
			StopCoroutine(actionCountDownToStartGame);
			actionCountDownToStartGame = null;
		}
		actionCountDownToStartGame = DoActionCountDownStartGame(_timeCountDown);
		StartCoroutine(actionCountDownToStartGame);
	}

	IEnumerator DoActionCountDownStartGame(double _timeCountDown){
		double _timeLeft = _timeCountDown;
		txtTimeCountDownToStartGame.text = string.Format("{0:00}", Mathf.CeilToInt((float) _timeLeft));

		while(_timeLeft > 0f){
			yield return null;
			_timeLeft -= Time.unscaledDeltaTime;
			
			if(_timeLeft < 0f){
				_timeLeft = 0f;
			}
			txtTimeCountDownToStartGame.text = string.Format("{0:00}", Mathf.CeilToInt((float) _timeLeft));
		}
		txtTimeCountDownToStartGame.text = string.Empty;

		actionCountDownToStartGame = null;
	}

	public void RefreshUIIndexCircle(){
		for(int i = 0; i < listIndexCircle.Count; i++){
			listIndexCircle[i].text = "";
		}
		if(pokerGamePlayData == null || pokerGamePlayData.listPlayerPlayingData == null){
			return;
		}
		for(int i = 0; i < pokerGamePlayData.listPlayerPlayingData.Count; i++){
			sbyte _indexChair = pokerGamePlayData.listPlayerPlayingData[i].indexChair;
			listIndexCircle[_indexChair].text = "" + i;
		}
	}

	public void RefreshUIPlayerRole(){
		if(pokerGamePlayData == null || pokerGamePlayData.listPlayerPlayingData == null){
			#if TEST
			Debug.LogError("Lỗi dữ liệu chưa init");
			#endif
			return;
		}
		if(pokerGamePlayData.listPlayerPlayingData.Count == 0){
			panelRoleSmallBlind.gameObject.SetActive(false);
			panelRoleBigBlind.gameObject.SetActive(false);
		}
		for(int i = 0; i < pokerGamePlayData.listPlayerPlayingData.Count && i <= 1; i++){
			int _indexChair = pokerGamePlayData.listPlayerPlayingData[i].indexChair;
			Poker_PlayerGroup _playerGroup = Poker_GamePlay_Manager.instance.listPlayerGroup[_indexChair];

			if(i == 0){
				panelRoleSmallBlind.gameObject.SetActive(true);
				panelRoleSmallBlind.position = _playerGroup.myPanelBet.imgIconChip.transform.position;
			}else if(i == 1){
				panelRoleBigBlind.gameObject.SetActive(true);
				panelRoleBigBlind.position = _playerGroup.myPanelBet.imgIconChip.transform.position;
			}
		}
	}
	
	public void RefreshUIPanelPlayerPlayingBet(bool _updateNow = false){
		if(pokerGamePlayData == null || pokerGamePlayData.listPlayerPlayingData == null){
			#if TEST
			Debug.LogError("Lỗi dữ liệu chưa init");
			#endif
			return;
		}
		for(int i = 0; i < pokerGamePlayData.listPlayerPlayingData.Count; i++){
			if(pokerGamePlayData.listPlayerPlayingData[i].turnBet > 0){
				int _indexChair = pokerGamePlayData.listPlayerPlayingData[i].indexChair;
				Poker_PlayerGroup _playerGroup = Poker_GamePlay_Manager.instance.listPlayerGroup[_indexChair];
				if(!_playerGroup.isInitialized){
					#if TEST
					Debug.LogError("Lỗi chưa init player group");
					#endif
				}
				_playerGroup.myPanelBet.SetBet(pokerGamePlayData.listPlayerPlayingData[i].turnBet, _updateNow);
				_playerGroup.myPanelBet.Show();
			}
		}
		panelTotalBet.gameObject.SetActive(true);
		SetGlobalBet(pokerGamePlayData.totalBet);
	}

	public void RefreshUIPanelPlayerPlayingInfo(bool _updateNow = false){
		if(pokerGamePlayData == null || pokerGamePlayData.listPlayerPlayingData == null){
			#if TEST
			Debug.LogError("Lỗi dữ liệu chưa init");
			#endif
			return;
		}
		for(int i = 0; i < pokerGamePlayData.listPlayerPlayingData.Count; i++){
			int _indexChair = pokerGamePlayData.listPlayerPlayingData[i].indexChair;
			Poker_PlayerGroup _playerGroup = Poker_GamePlay_Manager.instance.listPlayerGroup[_indexChair];
			if(!_playerGroup.isInitialized){
				#if TEST
				Debug.LogError("Lỗi chưa init player group");
				#endif
			}
			if(_updateNow){
				_playerGroup.panelPlayerInfo.RefreshGoldInfo(true);
			}else{
				_playerGroup.panelPlayerInfo.RefreshGoldInfo();
			}
		}
	}

	public void ShowMyPanelUserInfo(){
		if(tweenMyPanelUserInfo != null){
			LeanTween.cancel(myPanelUserInfo.gameObject, tweenMyPanelUserInfo.uniqueId);
			tweenMyPanelUserInfo = null;
		}
		myPanelUserInfo.Show();
		myPanelUserInfo.RefreshGoldInfo(true);
		tweenMyPanelUserInfo = LeanTween.moveLocalY(myPanelUserInfo.gameObject, posShowMyPanelUserInfo, 0.2f).setEase(LeanTweenType.easeInBack).setOnComplete(()=>{
			tweenMyPanelUserInfo = null;
		});
	}

	public void HideMyPanelUserInfo(){
		if(tweenMyPanelUserInfo != null){
			LeanTween.cancel(myPanelUserInfo.gameObject, tweenMyPanelUserInfo.uniqueId);
			tweenMyPanelUserInfo = null;
		}
		tweenMyPanelUserInfo = LeanTween.moveLocalY(myPanelUserInfo.gameObject, posHideMyPanelUserInfo, 0.2f).setEase(LeanTweenType.easeOutBack).setOnComplete(()=>{
			tweenMyPanelUserInfo = null;
			myPanelUserInfo.Hide();
		});
	}

	public void ShowArrowFocusGetGold(){
		if(arrowFocusGetGold.myState == MyArrowFocusController.State.Hide){
			if(Poker_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
				MyAudioManager.instance.PlaySfx(Poker_GamePlay_Manager.instance.myAudioInfo.sfx_Notification);
			}
		}
		arrowFocusGetGold.Show();
	}

	public void HideArrowFocusGetGold(){
		arrowFocusGetGold.Hide();
	}
	#endregion

	#region For Test
	public void TestDealGlobalCard(){
		List<sbyte> _valueCards = new List<sbyte>();
		_valueCards.Add(12);
		_valueCards.Add(0);
		_valueCards.Add(1);
		_valueCards.Add(2);
		_valueCards.Add(3);
		DealGlobalCard(_valueCards);
	}

	public void TestDealCardToPlayers(){
		StartCoroutine(DoActionTestDealCardToPlayers());
	}

	IEnumerator DoActionTestDealCardToPlayers(){
		for(int i = 0; i < Poker_GamePlay_Manager.instance.listPlayerGroup.Count; i++){
			DealPlayerCard(Poker_GamePlay_Manager.instance.listPlayerGroup[i]);
			yield return Yielders.Get(0.1f);
		}
	}

	public void TestShowAllCardsOfPlayers(){
		List<sbyte> _valueCards = new List<sbyte>();
		for(int i = 0; i < Poker_GamePlay_Manager.instance.listPlayerGroup.Count; i++){
			_valueCards = new List<sbyte>();
			_valueCards.Add((sbyte)Random.Range(0, 52));
			_valueCards.Add((sbyte)Random.Range(0, 52));
			ShowCard (_valueCards, Poker_GamePlay_Manager.instance.listPlayerGroup[i]);
		}
	}
	#endregion

	private void OnDestroy() {
		StopAllCoroutines();
	}
}
