using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;

public class Uno_UIManager : MonoBehaviour {

	[Header("Place Holder")]
	public List<PlaceHolderPanelOtherPlayerInfo> listPlaceHolderPanelPlayerInfo_Wating;
	[SerializeField] PlaceHolderPanelOtherPlayerInfo placeHolderMyPanelInfo_Playing;
	public List<Transform> listPanelContainPlayerCardsCompactHolder;
	[SerializeField] List<PlaceHolderPanelOtherPlayerInfo> listPlaceHolderPanelPlayerInfo_Playing;
	[SerializeField] List<Transform> listPanelContainPlayerCardsHolder;
	[SerializeField] List<Transform> listPanelContainBtnAtkUnoHolder;
	[SerializeField] Transform panelContainOwnCardHolder;
	[SerializeField] Transform panelGlobalCardsHolder;

	[Header("Panels")]
	public Text txtTableInfo;
	public List<Text> listTxtPlayerNumberCards;
	[SerializeField] Text txtTableBet;
	public Transform panelContainGlobalCards;
	public Transform panelContainAllPlayerCards;
	public PanelUserInfoInGameController myPanelUserInfo;
	public List<Uno_PanelPlayerBetInfo_Controller> listPanelBet;
	public Uno_PanelPlayerBetInfo_Controller panelTotalBet;
	public Uno_MyBar_Controller myBarController;
	public Uno_PanelChooseColor_Controller panelChooseColor;
	[SerializeField] MyArrowFocusController arrowFocusGetGold;
	[SerializeField] Transform dealer;
	[SerializeField] Text txtTimeCountDownToStartGame;
	[SerializeField] Text txtTimeCountDownToStopGame;
	public RectTransform panelRotate;
	public Uno_Background_Controller unoBackground;
	public Uno_CircleTurn_Controller unoCircleTurn;
	public Uno_PanelHistory_Controller panelHistory;
	public PanelTapToSkipController panelTapToSkip;

	[Header("Finish Game")]
	public Uno_PanelFinishGame_Reason_Controller panelReasonFinishGame;
	public Uno_PanelFinishGame_ScoreBoard_Controller panelScoreBoardFinishGame;

	[Header("Setting")]
	public int numCardsCompact = 8;
	public Uno_SortingLayerManager sortingLayerManager;
	[SerializeField] float timeStepChangeView;
	public Vector2 sizeCardDefault;

	[Header("Prefabs")]
	[SerializeField] GameObject cardPrefab;
	[SerializeField] GameObject playerCardHolderPrefab;
	[SerializeField] GameObject ownCardHolderPrefab;
	[SerializeField] GameObject goldPrefab;
	[SerializeField] GameObject effAtkUnoSwordPrefab;
	[SerializeField] GameObject effAtkUnoHitPrefab;
	[SerializeField] GameObject panelBonusGoldPrefab;

	public TransformPlaceHolder panelGlobalCardsHolderCatched{get;set;}
	public List<TransformPlaceHolder> listPanelBtnAtkUnoHolderCatched{get;set;}
	public MySimplePoolManager goldObjectPoolManager;
	public MySimplePoolManager cardsGlobalPoolManager;
	public MySimplePoolManager effectPoolManager;
	
	public UnoGamePlayData unoGamePlayData{
		get{
			return Uno_GamePlay_Manager.instance.unoGamePlayData;
		}
	}

	float posShowMyPanelUserInfo, posHideMyPanelUserInfo;
	IEnumerator actionChangeView, actionPanelPlayerInfoFollowTmpPanel;
	IEnumerator actionCountDownToStartGame, actionCountDownToStopGame;
	IEnumerator actionMoveToPosPlayingOrWaiting;
	LTDescr tweenMyPanelUserInfo, tweenBtnStandUp;
	
	long virtualTableBet, realTableBet;
	IEnumerator actionTweenTableBet;
	

	public bool isChangingView{
		get{
			return (actionChangeView != null);
		}
	}

	public bool isChangingPosPlayingOrWaiting{
		get{
			return (actionMoveToPosPlayingOrWaiting != null);
		}
	}

	public void InitFirst(){
		goldObjectPoolManager = new MySimplePoolManager();
		cardsGlobalPoolManager = new MySimplePoolManager(5);
		effectPoolManager = new MySimplePoolManager();

		for(int i = 0; i < listTxtPlayerNumberCards.Count; i++){
			listTxtPlayerNumberCards[i].text = string.Empty;
			listTxtPlayerNumberCards[i].gameObject.SetActive(false);
		}

		string _tmpTableInfo = "";
		_tmpTableInfo += DataManager.instance.miniGameData.currentMiniGameDetail.currentServerDetail.subServerName + "\n";
		_tmpTableInfo += string.Format("Table {0:00}", DataManager.instance.miniGameData.currentMiniGameDetail.tableData.currentTableDetail.tableId);
		txtTableInfo.text = _tmpTableInfo;
		
		txtTimeCountDownToStartGame.text = string.Empty;
		txtTimeCountDownToStopGame.text = "0:00";

		panelGlobalCardsHolderCatched = new TransformPlaceHolder(panelGlobalCardsHolder);
		Destroy(panelGlobalCardsHolder.gameObject);

		listPanelBtnAtkUnoHolderCatched = new List<TransformPlaceHolder>();
		for(int i = 0; i < listPanelContainBtnAtkUnoHolder.Count; i++){
			listPanelBtnAtkUnoHolderCatched.Add(new TransformPlaceHolder(listPanelContainBtnAtkUnoHolder[i]));
		}
		for(int i = 0; i < listPanelContainBtnAtkUnoHolder.Count; i++){
			Destroy(listPanelContainBtnAtkUnoHolder[i].gameObject);
		}

		for(int i = 0; i < Uno_GamePlay_Manager.instance.listPlayerGroup.Count; i ++){
			Uno_GamePlay_Manager.instance.listPlayerGroup[i].InitFirst(i);
		}

		RectTransform _tmp = myPanelUserInfo.GetComponent<RectTransform>();
		float _sizeH = _tmp.sizeDelta.y;
		posShowMyPanelUserInfo = _tmp.localPosition.y;
		posHideMyPanelUserInfo = posShowMyPanelUserInfo - _sizeH;
		myPanelUserInfo.InitData();
		
		myBarController.HideAllButtons();
		myBarController.HideBtnUno();
	}

	public Coroutine ChangeView(Uno_PlayerGroup _myPlayerGroup){
		if(isChangingView){
			return null;
		}
		
		if(_myPlayerGroup != null){
			if(_myPlayerGroup.viewIndex != 2){
				actionChangeView = DoActionChangeView(_myPlayerGroup);
				return StartCoroutine(actionChangeView);
			}else{
				_myPlayerGroup.ShowButtonStandUp(false);
				return null;
			}
 		}else{
			if(Uno_GamePlay_Manager.instance.listPlayerGroup[0].realIndex != Uno_GamePlay_Manager.instance.listPlayerGroup[0].viewIndex){
				actionChangeView = DoActionChangeView(null);
				return StartCoroutine(actionChangeView);
			}
		}
		
		return null;
	}

	IEnumerator DoActionChangeView(Uno_PlayerGroup _myPlayerGroup){ // nếu _myViewIndex = -1 tức là mình đã đứng dậy
		int _tmpDelta = 0;
		int _deltaAngleZ = 0;
		bool _isFinished = false;
		if(_myPlayerGroup == null){
			_tmpDelta = Uno_GamePlay_Manager.instance.listPlayerGroup[0].viewIndex;
			for(int i = 0; i < Uno_GamePlay_Manager.instance.listPlayerGroup.Count; i++){
				int _newIndexView = Uno_GamePlay_Manager.instance.listPlayerGroup[i].realIndex;
				Uno_GamePlay_Manager.instance.listPlayerGroup[i].viewIndex = _newIndexView;
			}
		}else{
			if(_myPlayerGroup.viewIndex < 2){
				_tmpDelta = 2 - _myPlayerGroup.viewIndex;
			}else if(_myPlayerGroup.viewIndex > 2){
				_tmpDelta = 2 + 4 - _myPlayerGroup.viewIndex;
			}
			for(int i = 0; i < Uno_GamePlay_Manager.instance.listPlayerGroup.Count; i++){
				int _newIndexView = (Uno_GamePlay_Manager.instance.listPlayerGroup[i].viewIndex + _tmpDelta)%4;
				Uno_GamePlay_Manager.instance.listPlayerGroup[i].viewIndex = _newIndexView;
			}
		}
		_deltaAngleZ = _tmpDelta * 90;

		yield return null;
		for(int i = 0; i < Uno_GamePlay_Manager.instance.listPlayerGroup.Count; i++){
			LeanTween.move(Uno_GamePlay_Manager.instance.listPlayerGroup[i].panelContainerPlayerInfo.gameObject, panelRotate.GetChild(i).position, timeStepChangeView).setEase(LeanTweenType.easeOutSine);
		}
		yield return Yielders.Get(timeStepChangeView);

		actionPanelPlayerInfoFollowTmpPanel = DoActionPanelPlayerInfoFollowTmpPanel();
		StartCoroutine(actionPanelPlayerInfoFollowTmpPanel);

		_isFinished = false;
		// float _z = 0;

		if(_myPlayerGroup == null){
			// _z = 0 - _deltaAngleZ;
			LeanTween.rotateAround(panelRotate.gameObject, Vector3.forward, _deltaAngleZ, timeStepChangeView).setOnComplete(()=>{
				_isFinished = true;
			}).setEase(LeanTweenType.easeOutSine);
		}else{
			// _z = panelRotate.rotation.eulerAngles.z - _deltaAngleZ;
			LeanTween.rotateAround(panelRotate.gameObject, Vector3.forward, - _deltaAngleZ, timeStepChangeView).setOnComplete(()=>{
				_isFinished = true;
			}).setEase(LeanTweenType.easeOutSine);
		}
		
		yield return new WaitUntil(()=>_isFinished);

		StopCoroutine(actionPanelPlayerInfoFollowTmpPanel);
		actionPanelPlayerInfoFollowTmpPanel = null;

		for(int i = 0; i < Uno_GamePlay_Manager.instance.listPlayerGroup.Count; i++){
			LeanTween.move(Uno_GamePlay_Manager.instance.listPlayerGroup[i].panelContainerPlayerInfo.gameObject, listPlaceHolderPanelPlayerInfo_Wating[Uno_GamePlay_Manager.instance.listPlayerGroup[i].viewIndex].transform.position, timeStepChangeView).setEase(LeanTweenType.easeOutSine);
		}
		yield return Yielders.Get(timeStepChangeView);
		
		if(_myPlayerGroup != null){
			_myPlayerGroup.ShowButtonStandUp(false);
		}

		actionChangeView = null;
	}
	IEnumerator DoActionPanelPlayerInfoFollowTmpPanel(){
		Vector3 _velocity = Vector3.zero;
		Vector3 _pos = Vector3.zero;
		while(true){
			yield return Yielders.FixedUpdate;
			for(int i = 0; i < Uno_GamePlay_Manager.instance.listPlayerGroup.Count; i++){
				_pos = Vector3.SmoothDamp(Uno_GamePlay_Manager.instance.listPlayerGroup[i].panelContainerPlayerInfo.position, panelRotate.GetChild(i).position, ref _velocity, Time.fixedDeltaTime);
				Uno_GamePlay_Manager.instance.listPlayerGroup[i].panelContainerPlayerInfo.position = _pos;
			}
		}
	}

	#region Behavior
	public Coroutine MoveAllToPosPlaying(bool _updateNow = true){
		if(actionMoveToPosPlayingOrWaiting != null){
			StopCoroutine(actionMoveToPosPlayingOrWaiting);
			actionMoveToPosPlayingOrWaiting = null;
		}
		actionMoveToPosPlayingOrWaiting = DoActionMoveAllToPosPlaying(_updateNow);
		return StartCoroutine(actionMoveToPosPlayingOrWaiting);
	}

	IEnumerator DoActionMoveAllToPosPlaying(bool _updateNow){
		List<IEnumerator> _listProgess = new List<IEnumerator>();
		for(int i = 0; i < Uno_GamePlay_Manager.instance.listPlayerGroup.Count; i++){
			Uno_PlayerGroup _playerGroup = Uno_GamePlay_Manager.instance.listPlayerGroup[i];
			if(_playerGroup.isInitialized){
				_listProgess.Add(DoActionMoveToPosPlaying(_playerGroup, _updateNow));
			}
		}
		yield return CoroutineChain.Start
			.Parallel(_listProgess.ToArray());
		actionMoveToPosPlayingOrWaiting = null;
	}

	public IEnumerator DoActionMoveToPosPlaying(Uno_PlayerGroup _playerGroup, bool _updateNow){
		if(!_playerGroup.isInitialized){
			yield break;
		}
		bool _isFinished = false;
		if(_playerGroup.isMe
			&& unoGamePlayData.CheckIsPlaying(_playerGroup.userData.sessionId)){
			if(_updateNow){
				_playerGroup.panelContainerPlayerInfo.transform.position = placeHolderMyPanelInfo_Playing.transform.position;
				_isFinished = true;
			}else{
				LeanTween.move(_playerGroup.panelContainerPlayerInfo.gameObject, placeHolderMyPanelInfo_Playing.transform.position, 0.2f).setOnComplete(()=>{
					_isFinished = true;
				});
			}
		}else{
			if(_updateNow){
				_playerGroup.panelContainerPlayerInfo.transform.position = listPlaceHolderPanelPlayerInfo_Playing[_playerGroup.viewIndex].transform.position;
				_isFinished = true;
			}else{
				LeanTween.move(_playerGroup.panelContainerPlayerInfo.gameObject, listPlaceHolderPanelPlayerInfo_Playing[_playerGroup.viewIndex].transform.position, 0.2f).setOnComplete(()=>{
					_isFinished = true;
				});
			}
		}
		yield return new WaitUntil(()=>_isFinished);
	}

	public Coroutine MoveAllToPosWaiting(bool _updateNow = true){
		if(actionMoveToPosPlayingOrWaiting != null){
			StopCoroutine(actionMoveToPosPlayingOrWaiting);
			actionMoveToPosPlayingOrWaiting = null;
		}
		actionMoveToPosPlayingOrWaiting = DoActionMoveAllToPosWaiting(_updateNow);
		return StartCoroutine(actionMoveToPosPlayingOrWaiting);
	}

	IEnumerator DoActionMoveAllToPosWaiting(bool _updateNow){
		for(int i = 0; i < Uno_GamePlay_Manager.instance.listPlayerGroup.Count; i++){
			Uno_PlayerGroup _playerGroup = Uno_GamePlay_Manager.instance.listPlayerGroup[i];
			// if(_playerGroup.isInitialized){
				if(_updateNow){
					_playerGroup.panelContainerPlayerInfo.transform.position = listPlaceHolderPanelPlayerInfo_Wating[_playerGroup.viewIndex].transform.position;
				}else{
					LeanTween.move(_playerGroup.panelContainerPlayerInfo.gameObject, listPlaceHolderPanelPlayerInfo_Wating[_playerGroup.viewIndex].transform.position, 0.2f);
				}
			// }
		}
		if(!_updateNow){
			yield return Yielders.Get(0.3f);
		}
		actionMoveToPosPlayingOrWaiting = null;
	}

	public CardHolderController CreateCardHolder(Uno_PlayerGroup _playerGroup){
		CardHolderController _cardHolder = null;
		if(_playerGroup.isMe
			&& unoGamePlayData.CheckIsPlaying(_playerGroup.userData.sessionId)){
			_cardHolder = LeanPool.Spawn(ownCardHolderPrefab, Vector3.zero, Quaternion.identity, panelContainOwnCardHolder).GetComponent<CardHolderController>();
		}else{
			_cardHolder = LeanPool.Spawn(playerCardHolderPrefab, Vector3.zero, Quaternion.identity, listPanelContainPlayerCardsHolder[_playerGroup.viewIndex]).GetComponent<CardHolderController>();
		}
		_cardHolder.transform.localRotation = Quaternion.identity;
		return _cardHolder;
	}

	public Coroutine DealPlayerCard(Uno_PlayerGroup _playerGroup, sbyte _valueCards = -1, float _timeDeal = 0f, System.Action _onFinished = null){
		if(!_playerGroup.isInitialized){
			#if TEST
			Debug.LogError("Chưa Init Player");
			#endif
			if(_onFinished != null){
				_onFinished();
			}
			return null;
		}
		return StartCoroutine(DoActionDealPlayerCard(_playerGroup, _valueCards, _timeDeal, _onFinished));
	}

	IEnumerator DoActionDealPlayerCard(Uno_PlayerGroup _playerGroup, sbyte _valueCards, float _timeDeal, System.Action _onFinished){
		CardUnoInfo _cardInfo = null;
		if(_valueCards >= 0 && _playerGroup.isMe
			&& unoGamePlayData.CheckIsPlaying(DataManager.instance.userData.sessionId)){ 
			_cardInfo = Uno_GamePlay_Manager.instance.GetCardInfo(_valueCards);
		}

		CardHolderController _cardHolder = CreateCardHolder(_playerGroup);
		PanelCardUnoDetailController _card = LeanPool.Spawn(cardPrefab, Vector3.zero, Quaternion.identity, panelContainAllPlayerCards).GetComponent<PanelCardUnoDetailController>();
		_card.transform.position = dealer.position;
		_card.transform.rotation = dealer.rotation;
		if(_cardInfo != null){
			_card.ShowNow(_cardInfo, _valueCards);
		}
		_card.SetCardHolder(_cardHolder);
		_card.ResizeAgain(sizeCardDefault.x, sizeCardDefault.y);
		_playerGroup.ownCardPoolManager.AddObject(_card);
		yield return Yielders.EndOfFrame;

		bool _canCompactCard = true;
		if(_playerGroup.isMe
			&& unoGamePlayData.CheckIsPlaying(DataManager.instance.userData.sessionId)){
			_canCompactCard = false;
		}

		if(_timeDeal <= 0){
			for(int i = 0; i < _playerGroup.ownCardPoolManager.listObjects.Count; i++){
				PanelCardUnoDetailController _panelCardDetail = (PanelCardUnoDetailController) _playerGroup.ownCardPoolManager.listObjects[i];
				_panelCardDetail.transform.position = _panelCardDetail.cardHolder.transform.position;
				_panelCardDetail.transform.rotation = _panelCardDetail.cardHolder.transform.rotation;
				_panelCardDetail.transform.localScale = Vector2.one * _panelCardDetail.cardHolder.ratioScale;
			}
			if(_canCompactCard && _playerGroup.ownCardPoolManager.listObjects.Count > numCardsCompact){
				for(int i = 0; i < _playerGroup.ownCardPoolManager.listObjects.Count; i++){
					PanelCardUnoDetailController _panelCardDetail = (PanelCardUnoDetailController) _playerGroup.ownCardPoolManager.listObjects[i];
					_panelCardDetail.transform.position = listPanelContainPlayerCardsCompactHolder[_playerGroup.viewIndex].position;
					if(i == 0){
						_panelCardDetail.SetVisible();
					}else{
						_panelCardDetail.SetInVisible();
					}
				}
				listTxtPlayerNumberCards[_playerGroup.viewIndex].gameObject.SetActive(true);
				listTxtPlayerNumberCards[_playerGroup.viewIndex].text = _playerGroup.ownCardPoolManager.listObjects.Count.ToString();
			}else{
				listTxtPlayerNumberCards[_playerGroup.viewIndex].gameObject.SetActive(false);
			}
		}else{
			if(Uno_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
				MyAudioManager.instance.PlaySfx(Uno_GamePlay_Manager.instance.myAudioInfo.sfx_DealCard);
			}

			if(_canCompactCard && _playerGroup.ownCardPoolManager.listObjects.Count > numCardsCompact){
				for(int i = 0; i < _playerGroup.ownCardPoolManager.listObjects.Count - 1; i++){
					PanelCardUnoDetailController _panelCardDetail = (PanelCardUnoDetailController) _playerGroup.ownCardPoolManager.listObjects[i];
					StartCoroutine(_panelCardDetail.Move(listPanelContainPlayerCardsCompactHolder[_playerGroup.viewIndex].position, _timeDeal / 2f, LeanTweenType.easeOutSine));
				}
				yield return CoroutineChain.Start
					.Parallel(_card.Move(listPanelContainPlayerCardsCompactHolder[_playerGroup.viewIndex].position, _timeDeal, LeanTweenType.easeOutSine)
					, _card.Rotate(_cardHolder.transform.rotation.eulerAngles, _timeDeal)
					, _card.ScaleTo(Vector2.one * _cardHolder.ratioScale, _timeDeal, LeanTweenType.notUsed));
				
				for(int i = 0; i < _playerGroup.ownCardPoolManager.listObjects.Count; i++){
					PanelCardUnoDetailController _panelCardDetail = (PanelCardUnoDetailController) _playerGroup.ownCardPoolManager.listObjects[i];
					if(i == 0){
						_panelCardDetail.SetVisible();
					}else{
						_panelCardDetail.SetInVisible();
					}
				}
				listTxtPlayerNumberCards[_playerGroup.viewIndex].gameObject.SetActive(true);
				listTxtPlayerNumberCards[_playerGroup.viewIndex].text = _playerGroup.ownCardPoolManager.listObjects.Count.ToString();
			}else{
				CoroutineChain.Start
					.Parallel(_card.MoveToCardHolder(_timeDeal, LeanTweenType.easeOutSine)
					, _card.Rotate(_cardHolder.transform.rotation.eulerAngles, _timeDeal)
					, _card.ScaleTo(Vector2.one * _cardHolder.ratioScale, _timeDeal, LeanTweenType.notUsed));
				yield return Yielders.Get(_timeDeal / 2f);
				for(int i = 0; i < _playerGroup.ownCardPoolManager.listObjects.Count - 1; i++){
					PanelCardUnoDetailController _panelCardDetail = (PanelCardUnoDetailController) _playerGroup.ownCardPoolManager.listObjects[i];
					StartCoroutine(_panelCardDetail.MoveToCardHolder(_timeDeal / 2f, LeanTweenType.easeOutSine));
				}
				yield return Yielders.Get(_timeDeal / 2f);
			}
		}

		if(_valueCards >= 0 && _playerGroup.isMe
			&& unoGamePlayData.CheckIsPlaying(DataManager.instance.userData.sessionId)){
			_card.onPointerDown = _playerGroup.OnFocusCard;
		}
		if(_onFinished != null){
			_onFinished();
		}
	}

	public IEnumerator DoActionPrepareToStartGame(){
		panelTotalBet.SetBet(0);
		panelTotalBet.Show(false);
		
		List<IEnumerator> _listActionShowEffGoldFly = new List<IEnumerator>();
		for(int i = 0; i < unoGamePlayData.listPlayerPlayingData.Count; i++){
			int _indexChair = unoGamePlayData.listPlayerPlayingData[i].indexChair;
			Uno_PlayerGroup _playerGroup = Uno_GamePlay_Manager.instance.listPlayerGroup[_indexChair];
			Vector2 _startPoint = _playerGroup.panelPlayerInfo.transform.position;
			Vector2 _endPoint = panelTotalBet.imgIconChip.transform.position;

			_listActionShowEffGoldFly.Add(MyConstant.DoActionShowEffectGoldFly(goldPrefab, goldObjectPoolManager, sortingLayerManager.sortingLayerInfo_GoldObject
				, _startPoint, _endPoint, 10, 1f, 0.8f, ()=>{
					if(Uno_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
						MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Gold);
					}
				}));
		}

		CoroutineChain.Start
			.Parallel(_listActionShowEffGoldFly.ToArray());
		yield return Yielders.Get(1.1f);
		if(Uno_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
			MyAudioManager.instance.PlaySfx(Uno_GamePlay_Manager.instance.myAudioInfo.sfx_Bet);
		}
		panelTotalBet.SetBet(unoGamePlayData.totalBet, false);
		SetTableBet(unoGamePlayData.currentBet);
		yield return Yielders.Get(1f);
		yield return panelTotalBet.Hide(false);
	}

	public IEnumerator DoActionShowGoldWinToPlayerAtFinishGame(UnoGamePlayData.Uno_FinishGame_Data _finishGameData, System.Action _onFinished){
		double _totalBet = unoGamePlayData.totalBet * 0.95;
		panelTotalBet.SetBet((long) _totalBet);
		yield return panelTotalBet.Show(false);
		yield return null;

		List<IEnumerator> _listActionShowEffGoldFly = new List<IEnumerator>();
		List<Uno_PlayerGroup> _tmpListPlayerGroup = new List<Uno_PlayerGroup>();
		for(int i = 0; i < _finishGameData.listPlayersData.Count; i ++){
			UnoGamePlayData.Uno_FinishGame_Data.Player_Data _playerFinish = _finishGameData.listPlayersData[i];
			if(!_playerFinish.isWin){
				continue;
			}
			UnoGamePlayData.Uno_PlayerPlayingData _playerPlayingData = unoGamePlayData.listPlayerPlayingData[_playerFinish.indexCircle];
			int _indexChair = _playerPlayingData.indexChair;
			Uno_PlayerGroup _playerGroup = Uno_GamePlay_Manager.instance.listPlayerGroup[_indexChair];
			_tmpListPlayerGroup.Add(_playerGroup);
			Vector2 _startPoint = panelTotalBet.imgIconChip.transform.position;
			Vector2 _endPoint = _playerGroup.panelPlayerInfo.transform.position;

			_listActionShowEffGoldFly.Add(MyConstant.DoActionShowEffectGoldFly(goldPrefab, goldObjectPoolManager, sortingLayerManager.sortingLayerInfo_GoldObject
				, _startPoint, _endPoint, 10, 1f, 0.8f, ()=>{
					if(Uno_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
						MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Gold);
					}
				}));
		}
		CoroutineChain.Start
			.Parallel(_listActionShowEffGoldFly.ToArray());
		yield return Yielders.Get(1.1f);
		panelTotalBet.SetBet(0, false);

		if(_tmpListPlayerGroup.Count > 0){
			if(Uno_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
				MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Gold);
			}
		}

		for(int i = 0; i < _tmpListPlayerGroup.Count; i++){
			Vector3 _posStartPanelGoldBonus = _tmpListPlayerGroup[i].panelPlayerInfo.imgAvatar.transform.position;
			StartCoroutine(MyConstant.DoActionShowPopupWinGold(panelBonusGoldPrefab, effectPoolManager, Uno_GamePlay_Manager.instance.myCanvas.transform
				, _posStartPanelGoldBonus, 0f, _finishGameData.goldWin));
			_tmpListPlayerGroup[i].panelPlayerInfo.RefreshGoldInfo();
		}
		if(myPanelUserInfo.currentState == PanelUserInfoInGameController.State.Show){
			myPanelUserInfo.RefreshGoldInfo();
		}else{
			myPanelUserInfo.RefreshGoldInfo(true);
		}
		
		yield return Yielders.Get(0.5f);
		yield return panelTotalBet.Hide(false);
		if(_onFinished != null){
			_onFinished();
		}
	}

	// public IEnumerator DoActionShowEffGoldFly(Vector2 _startPoint, Vector2 _endPoint, int _numGold){
	// 	Vector2 _newStartPoint = Vector2.zero;
	// 	for(int i = 0; i < _numGold; i++){
	// 		_newStartPoint.x = Random.Range(_startPoint.x - 0.2f, _startPoint.x + 0.2f);
	// 		_newStartPoint.y = Random.Range(_startPoint.y - 0.2f, _startPoint.y + 0.2f);
	// 		GoldObjectController _gold = LeanPool.Spawn(goldPrefab, _newStartPoint, Quaternion.identity).GetComponent<GoldObjectController>();
	// 		goldObjectPoolManager.AddObject(_gold);
	// 		_gold.InitData(sortingLayerManager.sortingLayerInfo_GoldObject, 1f);
	// 		StartCoroutine(_gold.DoActionMoveAndSelfDestruction(_endPoint, 0.8f, LeanTweenType.easeInBack));
	// 		if(_numGold > 1){
	// 			yield return null;
	// 		}
	// 	}
	// }

	public Coroutine ClearAllCards(List<PanelCardUnoDetailController> _listCards){
		return StartCoroutine(DoActionClearAllCards(_listCards));
	}

	IEnumerator DoActionClearAllCards(List<PanelCardUnoDetailController> _listCards){
		for(int i = 0; i < _listCards.Count; i++){
			if(Uno_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
				MyAudioManager.instance.PlaySfx(Uno_GamePlay_Manager.instance.myAudioInfo.sfx_Card);
			}
			_listCards[i].DestroyCardHolder();
			_listCards[i].SetVisible();
			CoroutineChain.Start
				.Parallel(_listCards[i].Move(dealer.position, 0.2f, LeanTweenType.easeOutSine)
					, _listCards[i].Rotate(dealer.transform.rotation.eulerAngles, 0.2f));
			yield return null;
		}
		yield return Yielders.Get(1f);
	}

	[ContextMenu("ShowTestBtnAtkUno")]
	public void ShowTestBtnAtkUno(){
		for(int i = 0; i < Uno_GamePlay_Manager.instance.listPlayerGroup.Count; i ++){
			Uno_GamePlay_Manager.instance.listPlayerGroup[i].ShowButtonAtkUno();
		}
	}

	[ContextMenu("ShowTestEffAtkUno")]
	public void ShowTestEffAtkUno(){
		ShowEffAtkUno(Uno_GamePlay_Manager.instance.listPlayerGroup[0], Uno_GamePlay_Manager.instance.listPlayerGroup[1]);
	}

	public Coroutine ShowEffAtkUno(Uno_PlayerGroup _playerAtk, Uno_PlayerGroup _playerBeAttacked){
		return StartCoroutine(DoActionShowEffAtkUno(_playerAtk, _playerBeAttacked));
	}

	IEnumerator DoActionShowEffAtkUno(Uno_PlayerGroup _playerAtk, Uno_PlayerGroup _playerBeAttacked){
		Uno_EffectAttackUno_Sword_Controller _effectSword = LeanPool.Spawn(effAtkUnoSwordPrefab, Vector3.zero, Quaternion.identity).GetComponent<Uno_EffectAttackUno_Sword_Controller>();
		effectPoolManager.AddObject(_effectSword);
		_effectSword.transform.position = _playerAtk.panelPlayerInfo.transform.position;
		
		if(Uno_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
            MyAudioManager.instance.PlaySfx(Uno_GamePlay_Manager.instance.myAudioInfo.sfx_AtkUno_Charge);
        }

		yield return _effectSword.SetUpMove(_playerBeAttacked.panelPlayerInfo.transform.position);
		
		if(Uno_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
            MyAudioManager.instance.PlaySfx(Uno_GamePlay_Manager.instance.myAudioInfo.sfx_AtkUno_Hit);
        }
		
		MySimplePoolObjectController _effectHit = LeanPool.Spawn(effAtkUnoHitPrefab, Vector3.zero, Quaternion.identity).GetComponent<MySimplePoolObjectController>();
		effectPoolManager.AddObject(_effectHit);
		_effectHit.transform.position = _playerBeAttacked.panelPlayerInfo.transform.position;
		_playerBeAttacked.panelPlayerInfo.myShakeController.SetUpShakeLocalPoint(0.3f, 10f);
		yield return Yielders.Get(0.5f);
	}

	public void SetUpPlayerAddGold(short _sessionid, int _reason, long _goldAdd, long _goldLast){
		Vector3 _posStartPanelGoldBonus = Vector3.zero;
		bool _showEffect = false;
		bool _checkContinue = true;
		
		if(_sessionid == DataManager.instance.userData.sessionId){
			if(myPanelUserInfo.currentState == PanelUserInfoInGameController.State.Show){
				_showEffect = true;
				_checkContinue = false;
				_posStartPanelGoldBonus = myPanelUserInfo.userAvata.transform.position;
				myPanelUserInfo.RefreshGoldInfo(false);
			}else{
				myPanelUserInfo.RefreshGoldInfo(true);
			}
		}
		
		if(_checkContinue){
			if(unoGamePlayData.currentGameState == UnoGamePlayData.GameState.STATUS_WAIT_FOR_PLAYER){
				int _indexChair = unoGamePlayData.listSessionIdOnChair.IndexOf(_sessionid);
				if(_indexChair >= 0){
					_showEffect = true;
					_posStartPanelGoldBonus = Uno_GamePlay_Manager.instance.listPlayerGroup[_indexChair].panelPlayerInfo.imgAvatar.transform.position;
					Uno_GamePlay_Manager.instance.listPlayerGroup[_indexChair].userData.gold = _goldLast;
					Uno_GamePlay_Manager.instance.listPlayerGroup[_indexChair].panelPlayerInfo.RefreshGoldInfo();
				}
			}else{
				if(unoGamePlayData.CheckIsPlaying(_sessionid)){
					int _index = unoGamePlayData.listSessionIdPlaying.IndexOf(_sessionid);
					if(_index >= 0){
						int _indexChair = unoGamePlayData.listPlayerPlayingData[_index].indexChair;
						if(unoGamePlayData.listSessionIdOnChair[_indexChair] == _sessionid
							&& Uno_GamePlay_Manager.instance.listPlayerGroup[_indexChair].isInitialized){
							_showEffect = true;
							_posStartPanelGoldBonus = Uno_GamePlay_Manager.instance.listPlayerGroup[_indexChair].panelPlayerInfo.imgAvatar.transform.position;
							Uno_GamePlay_Manager.instance.listPlayerGroup[_indexChair].userData.gold = _goldLast;
							Uno_GamePlay_Manager.instance.listPlayerGroup[_indexChair].panelPlayerInfo.RefreshGoldInfo();
						}
					}
				}
			}
		}

		if(_showEffect){
			StartCoroutine(MyConstant.DoActionShowPopupWinGold(panelBonusGoldPrefab, effectPoolManager, Uno_GamePlay_Manager.instance.myCanvas.transform
				, _posStartPanelGoldBonus, 0f, _goldAdd));
		}
	}
	#endregion

	#region Refresh UI
	public void RefreshAllUINow (){
		if(unoGamePlayData.currentGameState == UnoGamePlayData.GameState.STATUS_PLAYING){
			unoCircleTurn.Show();
			StartCoroutine(unoCircleTurn.DoActionSetTurnDirection(unoGamePlayData.turnDirection));

 			StartCoroutine(unoBackground.DoActionSetColor(unoGamePlayData.currentColor));
		}
		StartCoroutine(unoCircleTurn.DoActionSetColor(unoGamePlayData.currentColor));
		
		RefreshAllPlayerGroupUINow();
		RefreshUIButtonSitDown();
		SetTableBet(unoGamePlayData.currentBet, true);

		if(unoGamePlayData.lastCardPut > 0){
			CardUnoInfo _cardInfo = null;
			bool _isThisWildCard = false;
			if(unoGamePlayData.IsWildCardColor(unoGamePlayData.lastCardPut)){
				_cardInfo = Uno_GamePlay_Manager.instance.GetCardInfo(CardUnoInfo.CardType._Special_WildColor);
				_isThisWildCard = true;
			}else if(unoGamePlayData.IsWildCardDraw(unoGamePlayData.lastCardPut)){
				_cardInfo = Uno_GamePlay_Manager.instance.GetCardInfo(CardUnoInfo.CardType._Special_Draw4Cards);
				_isThisWildCard = true;
			}else{
				_cardInfo = Uno_GamePlay_Manager.instance.GetCardInfo(unoGamePlayData.lastCardPut);
			}

			if(_cardInfo == null){
				#if TEST
				Debug.LogError(">>> Không tìm thấy cardInfo (0): " + unoGamePlayData.lastCardPut);
				#endif
				return;
			}

			PanelCardUnoDetailController _card = LeanPool.Spawn(cardPrefab, Vector3.zero, Quaternion.identity, panelContainGlobalCards).GetComponent<PanelCardUnoDetailController>();
			_card.transform.position = panelGlobalCardsHolderCatched.position;
			Vector3 _rot = panelGlobalCardsHolderCatched.rotation.eulerAngles;
			_rot.z += Random.Range(-60f, 60f);
			_card.transform.rotation = Quaternion.Euler(_rot);
			_card.ResizeAgain(sizeCardDefault.x, sizeCardDefault.y);
			_card.ShowNow(_cardInfo, unoGamePlayData.lastCardPut);
			if(_isThisWildCard){
				StartCoroutine(_card.DoActionChangeBgColor(unoGamePlayData.currentColor));
			}
			cardsGlobalPoolManager.AddObject(_card);
		}
	}

	void RefreshAllPlayerGroupUINow(){
		if(unoGamePlayData.currentGameState == UnoGamePlayData.GameState.STATUS_WAIT_FOR_PLAYER){
			for(int i = 0; i < unoGamePlayData.listSessionIdOnChair.Count; i++){
				if(unoGamePlayData.listSessionIdOnChair[i] < 0){ // ghế trống
					continue;
				}
				for(int j = 0; j < unoGamePlayData.listGlobalPlayerData.Count; j++){
					if(unoGamePlayData.listSessionIdOnChair[i] == unoGamePlayData.listGlobalPlayerData[j].sessionId){
						Uno_GamePlay_Manager.instance.listPlayerGroup[i].InitData(unoGamePlayData.listGlobalPlayerData[j]);
						Uno_GamePlay_Manager.instance.listPlayerGroup[i].panelPlayerInfo.transform.localScale = Vector3.one * listPlaceHolderPanelPlayerInfo_Wating[i].ratioScale;
						break;
					}
				}
			}
			MoveAllToPosWaiting();
		}else{
			List<int> _listIndexChairPlaying = new List<int>();
			for(int i = 0; i < unoGamePlayData.listPlayerPlayingData.Count; i++){
				_listIndexChairPlaying.Add(unoGamePlayData.listPlayerPlayingData[i].indexChair);
				int _indexChair = unoGamePlayData.listPlayerPlayingData[i].indexChair;
				
				Uno_PlayerGroup _playerGroup = Uno_GamePlay_Manager.instance.listPlayerGroup[_indexChair];
				_playerGroup.InitData(unoGamePlayData.listPlayerPlayingData[i].userData);
				if(_playerGroup.isMe){
					#if TEST
					Debug.LogError(">>> Bug logic (0)");
					#endif
					_playerGroup.panelPlayerInfo.transform.localScale = Vector3.one * placeHolderMyPanelInfo_Playing.ratioScale;
				}else{
					_playerGroup.panelPlayerInfo.transform.localScale = Vector3.one * listPlaceHolderPanelPlayerInfo_Playing[_playerGroup.viewIndex].ratioScale;
				}

				for(int j = 0; j < unoGamePlayData.listPlayerPlayingData[i].ownCards.Count; j ++){
					DealPlayerCard(_playerGroup, unoGamePlayData.listPlayerPlayingData[i].ownCards[j], 0f, null);
				}
			}
			MoveAllToPosPlaying();

			// --- hiện các thông tin người chơi đang chờ --- //
			List<int> _listIndexChairWaiting = new List<int>();
			for(int i = 0; i < unoGamePlayData.numberChairs; i++){
				if(!_listIndexChairPlaying.Contains(i)){
					_listIndexChairWaiting.Add(i);
				}
			}
			for(int i = 0; i < _listIndexChairWaiting.Count; i++){
				int _indexChair = _listIndexChairWaiting[i];
				short _sessionId = unoGamePlayData.listSessionIdOnChair[_indexChair];
				UserDataInGame _userData = unoGamePlayData.GetUserDataInGameFromListGlobal(_sessionId);
				if(_userData == null){
					continue;
				}
				Uno_PlayerGroup _playerGroup = Uno_GamePlay_Manager.instance.listPlayerGroup[_indexChair];
				_playerGroup.InitAsIncognito(_userData);
				LeanTween.scale(_playerGroup.panelPlayerInfo.gameObject, Vector3.one * listPlaceHolderPanelPlayerInfo_Wating[_playerGroup.realIndex].ratioScale, 0.2f)
					.setEase(LeanTweenType.easeOutBack);
			}
		}
	}

	public void ShowMyPanelUserInfo(){
		if(tweenMyPanelUserInfo != null){
			LeanTween.cancel(tweenMyPanelUserInfo.uniqueId);
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
			LeanTween.cancel(tweenMyPanelUserInfo.uniqueId);
			tweenMyPanelUserInfo = null;
		}
		tweenMyPanelUserInfo = LeanTween.moveLocalY(myPanelUserInfo.gameObject, posHideMyPanelUserInfo, 0.2f).setEase(LeanTweenType.easeOutBack).setOnComplete(()=>{
			tweenMyPanelUserInfo = null;
			myPanelUserInfo.Hide();
		});
	}

	public void RefreshUIButtonSitDown(){
		if(unoGamePlayData.CheckIsPlaying(DataManager.instance.userData.sessionId)
			|| unoGamePlayData.listSessionIdOnChair.Contains(DataManager.instance.userData.sessionId)){
			for(int i = 0; i < Uno_GamePlay_Manager.instance.listPlayerGroup.Count; i++){
				Uno_GamePlay_Manager.instance.listPlayerGroup[i].HideButtonSitDown();
			}
			HideMyPanelUserInfo();
			return;
		}
		
		ShowMyPanelUserInfo();

		List<int> _listIndexChairPlaying = new List<int>();
		for(int i = 0; i < unoGamePlayData.listPlayerPlayingData.Count; i++){
			int _indexChair = unoGamePlayData.listPlayerPlayingData[i].indexChair;
			Uno_GamePlay_Manager.instance.listPlayerGroup[_indexChair].HideButtonSitDown();
			_listIndexChairPlaying.Add(_indexChair);
		}
		List<int> _listIndexChairWaiting = new List<int>();
		for(int i = 0; i < unoGamePlayData.numberChairs; i++){
			if(!_listIndexChairPlaying.Contains(i)){
				_listIndexChairWaiting.Add(i);
			}
		}
		Uno_PlayerGroup _playerGroup = null;
		for(int i = 0; i < _listIndexChairWaiting.Count; i++){
			int _indexChair = _listIndexChairWaiting[i];
			short _sessionId = unoGamePlayData.listSessionIdOnChair[_indexChair];
			_playerGroup = Uno_GamePlay_Manager.instance.listPlayerGroup[_indexChair];
			UserDataInGame _userData = unoGamePlayData.GetUserDataInGameFromListGlobal(_sessionId);
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

	public void StopShowCountDownStartGame(){
		if(actionCountDownToStartGame != null){
			StopCoroutine(actionCountDownToStartGame);
			actionCountDownToStartGame = null;
		}
		txtTimeCountDownToStartGame.text = string.Empty;
	}

	public void ShowCountDownStopGame(double _timeCountDown){
		if(_timeCountDown < 1){
			return;
		}
		if(actionCountDownToStopGame != null){
			StopCoroutine(actionCountDownToStopGame);
			actionCountDownToStopGame = null;
		}
		actionCountDownToStopGame = DoActionCountDownStopGame(_timeCountDown);
		StartCoroutine(actionCountDownToStopGame);
	}

	IEnumerator DoActionCountDownStopGame(double _timeCountDown){
		double _timeLeft = _timeCountDown;
		long _minutes = (long) _timeLeft / 60;
		long _seconds = (long) _timeLeft % 60;
		txtTimeCountDownToStopGame.text = string.Format("{0:0}:{1:00}", _minutes, _seconds);

		while(_timeLeft > 0f){
			yield return null;
			_timeLeft -= Time.unscaledDeltaTime;
			
			if(_timeLeft < 0){
				_timeLeft = 0;
			}
			_minutes = (long) _timeLeft / 60;
			_seconds = (long) _timeLeft % 60;

			txtTimeCountDownToStopGame.text = string.Format("{0:0}:{1:00}", _minutes, _seconds);
		}
		txtTimeCountDownToStopGame.text = "0:00";

		actionCountDownToStopGame = null;
	}

	public void StopShowCountDownStopGame(){
		if(actionCountDownToStopGame != null){
			StopCoroutine(actionCountDownToStopGame);
			actionCountDownToStopGame = null;
		}
		txtTimeCountDownToStopGame.text = "0:00";
	}

	public void ShowArrowFocusGetGold(){
		if(arrowFocusGetGold.myState == MyArrowFocusController.State.Hide){
			if(Uno_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
				MyAudioManager.instance.PlaySfx(Uno_GamePlay_Manager.instance.myAudioInfo.sfx_Notification);
			}
		}
		arrowFocusGetGold.Show();
	}

	public void HideArrowFocusGetGold(){
		arrowFocusGetGold.Hide();
	}

	public void SetTableBet(long _bet, bool _updateNow = false){
		realTableBet = _bet;
		if(_updateNow){
			if(actionTweenTableBet != null){
				StopCoroutine(actionTweenTableBet);
				actionTweenTableBet = null;
			}
			virtualTableBet = realTableBet;
			txtTableBet.text = "Bet: " + MyConstant.GetMoneyString(virtualTableBet, 9999);
		}else{
			if(actionTweenTableBet != null){
				StopCoroutine(actionTweenTableBet);
				actionTweenTableBet = null;
			}
			actionTweenTableBet = MyConstant.TweenValue(virtualTableBet, realTableBet, 5, (_valueUpdate)=>{
				virtualTableBet = _valueUpdate;
				txtTableBet.text = "Bet: " + MyConstant.GetMoneyString(virtualTableBet, 9999);
			}, (_valueFinish)=>{
				virtualTableBet = _valueFinish;
				txtTableBet.text = "Bet: " + MyConstant.GetMoneyString(virtualTableBet, 9999);
				actionTweenTableBet = null;
			});
			StartCoroutine(actionTweenTableBet);
		}
	}
	#endregion

	private void OnDestroy() {
		StopAllCoroutines();
	}
}
