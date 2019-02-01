using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;
public class AnimalRacing_Bet_Manager : MonoBehaviour {

	public enum State{
		Hide,
		Show
	}
	public State myCurrentState{get;set;}

	public CanvasGroup myCanvasGroup;
	public PanelUserInfoInGameController panelUserInGame;
	public AnimalRacing_Panel_BetHistory_Controller betHistoryController;
	public Text txtServerName;
	public Text txtTableId;
	public PanelClockInGameController clock;
	public List<PlaceHolderPanelOtherPlayerInfo> listPlaceHolderPanelOtherPlayerInfo;
	public List<PanelPlayerInfoInGameController> listOtherPlayerInfo{get;set;}
	public AnimalRacing_Panel_TableBet_Controller panelTableBet;
	public PanelListChipDetailController panelListChip;

	[Header("Prefab")]
	public ChipObjectController chipPrefab;
	public GameObject goldObjectPrefab;
	public PanelBonusGoldInGameController panelBonusGoldPrefab;
	public PanelPlayerInfoInGameController panelOtherPlayerInfoPrefab;

	public List<ChipObjectController> listChipObjectOnBetTable{get;set;}
	public MySimplePoolManager effectPoolManager;

	private void Awake() {
		if(listPlaceHolderPanelOtherPlayerInfo.Count != 14){
			Debug.LogError("Xem lại listPlayerInfo");
		}
		myCurrentState = State.Show;
	}

	public void InitData(){
		effectPoolManager = new MySimplePoolManager();
		listOtherPlayerInfo = new List<PanelPlayerInfoInGameController>();

		for(int i = 0; i < listPlaceHolderPanelOtherPlayerInfo.Count; i++){
			PanelPlayerInfoInGameController _tmpPanelInfo = (PanelPlayerInfoInGameController) Instantiate(panelOtherPlayerInfoPrefab, listPlaceHolderPanelOtherPlayerInfo[i].transform, false);
			_tmpPanelInfo.transform.position = listPlaceHolderPanelOtherPlayerInfo[i].transform.position;
			_tmpPanelInfo.transform.localScale = Vector3.one * listPlaceHolderPanelOtherPlayerInfo[i].ratioScale;
			_tmpPanelInfo.popupChatPosType = listPlaceHolderPanelOtherPlayerInfo[i].popupChatPosType;
			listOtherPlayerInfo.Add(_tmpPanelInfo);
		}

		txtTableId.text = string.Format("Table {0:00}", DataManager.instance.miniGameData.currentMiniGameDetail.tableData.currentTableDetail.tableId);
		txtServerName.text = DataManager.instance.miniGameData.currentMiniGameDetail.currentServerDetail.subServerName;
		panelUserInGame.InitData();

		panelListChip.InitData();
	}
	
	public void Show(bool _isFirstShow = false){
		// gameObject.SetActive(true);
		myCurrentState = State.Show;
		myCanvasGroup.alpha = 1f;
		myCanvasGroup.blocksRaycasts = true;
		Vector3 _posCam = AnimalRacing_GamePlay_Manager.instance.mainCamera.transform.position;
		_posCam.x = 0f;
		_posCam.y = 0f;
		AnimalRacing_GamePlay_Manager.instance.mainCamera.transform.position = _posCam;

		if(!_isFirstShow){
			if(AnimalRacing_GamePlay_Manager.instance.callbackManager.onStartSetUpAfterShowResult != null){
				AnimalRacing_GamePlay_Manager.instance.callbackManager.onStartSetUpAfterShowResult();
				AnimalRacing_GamePlay_Manager.instance.callbackManager.onStartSetUpAfterShowResult = null;
			}
		}
	}
	public void Hide(){
		myCurrentState = State.Hide;
		myCanvasGroup.alpha = 0f;
		myCanvasGroup.blocksRaycasts = false;
		// gameObject.SetActive(false);
 	}

	public void SetUpPlayerAddBet(short _sessionid, short _chipIndex, sbyte _indexBet, long _GOLD){
		if(!AnimalRacing_GamePlay_Manager.instance.CanShowPlayerAddBet()){
			return;
		}
		if(_chipIndex == -1){
			Debug.LogError(">>> chipIndex = -1");
			return;
		}
		
		Vector3 _posStart = Vector3.zero;
		Vector3 _posEnd = panelTableBet.listBetOption[_indexBet].imgAvatar.transform.position;
		_posEnd.x = Random.Range(_posEnd.x - 0.2f, _posEnd.x + 0.2f);
		_posEnd.y = Random.Range(_posEnd.y - 0.2f, _posEnd.y + 0.2f);

		bool _isError = true;
		if(_sessionid == DataManager.instance.userData.sessionId){
			_posStart = panelUserInGame.userAvata.transform.position;

			panelUserInGame.RefreshGoldInfo();
			panelListChip.SetFocusChipAgain();
			
			_isError = false;
		}else{
			for(int i = 0; i < listOtherPlayerInfo.Count; i ++){
				if(listOtherPlayerInfo[i].data != null){
					if(listOtherPlayerInfo[i].data.sessionId == _sessionid){
						_posStart = listOtherPlayerInfo[i].transform.position;
						listOtherPlayerInfo[i].RefreshGoldInfo();
						_isError = false;
						break;
					}
				}
			}
		}
		if(!_isError){
			IChipInfo _chipInfo = panelListChip.listChipDetail[_chipIndex].chipInfo;
			ChipObjectController _tmpChip = LeanPool.Spawn(chipPrefab.gameObject, _posStart, Quaternion.identity).GetComponent<ChipObjectController>();
			effectPoolManager.AddObject(_tmpChip);
			_tmpChip.SetData(_chipInfo, AnimalRacing_GamePlay_Manager.instance.sortingLayerManager.sortingLayerInfo_ChipObject, 1f);
			_tmpChip.RegisCallbackJustMoveFinished(_indexBet, (_i)=>{
				panelTableBet.listBetOption[_i].ShowGlow();
				if(AnimalRacing_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
					MyAudioManager.instance.PlaySfx(AnimalRacing_GamePlay_Manager.instance.myAudioInfo.sfx_Bet);
				}
			});
			_tmpChip.SetUpMoveToTableBet(_posEnd);
		}
	}

	public void SetUpAfterShowResult(){
		StartCoroutine(DoActionAfterShowResult());
	}

	IEnumerator DoActionAfterShowResult(){
		RefreshUIPanelUserData();
		RefreshUIPanelListPlayerViewer();
		RefreshUIPanelListHistory();
		
		yield return Yielders.EndOfFrame;
		yield return StartCoroutine(DoActionShowEffectUpdateGold());
		
		RefreshUIPanelTableBet();
		RefreshUIPanelCurrentScore();
	}

	public IEnumerator DoActionShowEffectUpdateGold(){
		AnimalRacingData _animalRacingData = AnimalRacing_GamePlay_Manager.instance.animalRacingData;
		Vector3 _posStartGoldWin = Vector3.zero;
		Vector3 _posEndGoldWin = Vector3.zero;
		Vector3 _posStartPanelGoldBonus = Vector3.zero;

		int _animalIndexWin = (int) _animalRacingData.currentResultData.animalWin;
		_posStartGoldWin = panelTableBet.listBetOption[_animalIndexWin].imgAvatar.transform.position;

		SetUpShadowTable(_animalRacingData.currentResultData.animalWin, true);

		if(AnimalRacing_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
			MyAudioManager.instance.PlaySfx(AnimalRacing_GamePlay_Manager.instance.myAudioInfo.sfx_HighlightPanelWin);
		}
		yield return DoActionHighlightIndexBet(_animalRacingData.currentResultData.animalWin);

		if(_animalRacingData.currentResultData.listGoldUpdate.Count > 0){
			bool _needWait = false;
			for(int i = 0; i < _animalRacingData.currentResultData.listGoldUpdate.Count; i++){
				if(_animalRacingData.currentResultData.listGoldUpdate[i].betUnit <= 0){
					continue;
				}
				if(_animalRacingData.currentResultData.listGoldUpdate[i].sessionId == DataManager.instance.userData.sessionId){
					_needWait = true;
					_posStartPanelGoldBonus = panelUserInGame.userAvata.transform.position;
					_posEndGoldWin = panelUserInGame.userAvata.transform.position;

					StartCoroutine(MyConstant.DoActionShowPopupWinGold(panelBonusGoldPrefab.gameObject, effectPoolManager, myCanvasGroup.transform
						, _posStartPanelGoldBonus, 1.1f, _animalRacingData.currentResultData.listGoldUpdate[i].betUnit
						, ()=>{
							panelUserInGame.RefreshGoldInfo();
							if(AnimalRacing_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
								MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Gold);
							}
						}));
					StartCoroutine(MyConstant.DoActionShowEffectGoldFly(goldObjectPrefab, effectPoolManager, AnimalRacing_GamePlay_Manager.instance.sortingLayerManager.sortingLayerInfo_GoldObject
						, _posStartGoldWin, _posEndGoldWin, 10, 1f, 0.8f));
				}else{
					for(int j = 0; j < listOtherPlayerInfo.Count; j++){
						if(listOtherPlayerInfo[j].data != null
							&& listOtherPlayerInfo[j].data.IsEqual(_animalRacingData.currentResultData.listGoldUpdate[i].sessionId)){
							_needWait = true;
							_posStartPanelGoldBonus = listOtherPlayerInfo[j].transform.position;
							_posEndGoldWin = listOtherPlayerInfo[j].transform.position;
							
							StartCoroutine(MyConstant.DoActionShowPopupWinGold(panelBonusGoldPrefab.gameObject, effectPoolManager, myCanvasGroup.transform
								, _posStartPanelGoldBonus, 1.1f, _animalRacingData.currentResultData.listGoldUpdate[i].betUnit
								, ()=>{
									if(listOtherPlayerInfo[j].data != null
										&& listOtherPlayerInfo[j].currentState == PanelPlayerInfoInGameController.State.Show){
										listOtherPlayerInfo[j].RefreshGoldInfo();
										if(AnimalRacing_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
											MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Gold);
										}
									}
								}));
							StartCoroutine(MyConstant.DoActionShowEffectGoldFly(goldObjectPrefab, effectPoolManager, AnimalRacing_GamePlay_Manager.instance.sortingLayerManager.sortingLayerInfo_GoldObject
								, _posStartGoldWin, _posEndGoldWin, 10, 1f, 0.8f));
							break;
						}
					}
				}
			}
			if(_needWait){
				yield return Yielders.Get(2f);
			}
			SetUpShadowTable(_animalRacingData.currentResultData.animalWin, false);
		}else{
			SetUpShadowTable(_animalRacingData.currentResultData.animalWin, false);
		}

		panelListChip.SetFocusChipAgain();

		if(AnimalRacing_GamePlay_Manager.instance.callbackManager.onEndSetUpAfterShowResult != null){
			AnimalRacing_GamePlay_Manager.instance.callbackManager.onEndSetUpAfterShowResult();
			AnimalRacing_GamePlay_Manager.instance.callbackManager.onEndSetUpAfterShowResult = null;
		}
	}

	// IEnumerator DoActionShowEffWinGold(Vector2 _startPoint, Vector2 _endPoint, int _numGold){
	// 	Vector2 _newStartPoint = Vector2.zero;
	// 	for(int i = 0; i < _numGold; i++){
	// 		_newStartPoint.x = Random.Range(_startPoint.x - 0.2f, _startPoint.x + 0.2f);
	// 		_newStartPoint.y = Random.Range(_startPoint.y - 0.2f, _startPoint.y + 0.2f);

	// 		GoldObjectController _gold = LeanPool.Spawn(goldObjectPrefab, _newStartPoint, Quaternion.identity).GetComponent<GoldObjectController>();
	// 		effectPoolManager.AddObject(_gold);
	// 		_gold.InitData(AnimalRacing_GamePlay_Manager.instance.sortingLayerManager.sortingLayerInfo_GoldObject, 1f);
	// 		StartCoroutine(_gold.DoActionMoveAndSelfDestruction(_endPoint, 0.8f, LeanTweenType.easeInBack));
	// 		if(_numGold > 1){
	// 			yield return null;
	// 		}
	// 	}
	// }

	// IEnumerator DoActionShowPopupWinGold(Vector2 _pos, long _goldAdd){
	// 	yield return Yielders.Get(1.1f);
	// 	PanelBonusGoldInGameController _tmpPanelGoldBonus = LeanPool.Spawn(panelBonusGoldPrefab.gameObject, _pos, Quaternion.identity, myCanvasGroup.transform).GetComponent<PanelBonusGoldInGameController>();
	// 	effectPoolManager.AddObject(_tmpPanelGoldBonus);
	// 	_tmpPanelGoldBonus.transform.position = _pos;
	// 	_tmpPanelGoldBonus.Show(_goldAdd);
	// 	panelUserInGame.RefreshGoldInfo();
	// 	if(AnimalRacing_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
	// 		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Gold);
	// 	}
	// }

	// IEnumerator DoActionShowPopupWinGold(PanelPlayerInfoInGameController _panelPlayerInfo, Vector2 _pos, long _goldAdd){
	// 	yield return Yielders.Get(0.6f);
	// 	PanelBonusGoldInGameController _tmpPanelGoldBonus = LeanPool.Spawn(panelBonusGoldPrefab.gameObject, _pos, Quaternion.identity, myCanvasGroup.transform).GetComponent<PanelBonusGoldInGameController>();
	// 	effectPoolManager.AddObject(_tmpPanelGoldBonus);
	// 	_tmpPanelGoldBonus.transform.position = _pos;
	// 	_tmpPanelGoldBonus.Show(_goldAdd);
	// 	_panelPlayerInfo.RefreshGoldInfo();
	// 	if(AnimalRacing_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
	// 		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Gold);
	// 	}
	// }

	public void SetUpShadowTable(AnimalRacing_AnimalController.AnimalType _indexExcept, bool _active){
		for(int i = 0; i < panelTableBet.listBetOption.Count; i++){
			if(panelTableBet.listBetOption[i].animalType != _indexExcept){
				panelTableBet.listBetOption[i].SetShadow(_active);
			}
		}
	}

	IEnumerator DoActionHighlightIndexBet(AnimalRacing_AnimalController.AnimalType _index){
		yield return panelTableBet.listBetOption[(int)_index].Highlight();
	}

	///<summary>
	/// SetUpPlayerAddGold :hàm setup khi người chơi nạp tiền trong lúc chơi
	///</summary>
	public void SetUpPlayerAddGold(short _sessionid, int _reason, long _goldAdd, long _goldLast){
		bool _isInStateBet = true;
		if(myCurrentState != State.Show){
			_isInStateBet = false;
		}
		Vector3 _posStartPanelGoldBonus = Vector3.zero;
		// PanelBonusGoldInGameController _tmpPanelGoldBonus = null;
		bool _showEffect = false;

		if(_sessionid == DataManager.instance.userData.sessionId){
			_posStartPanelGoldBonus = panelUserInGame.userAvata.transform.position;

			DataManager.instance.userData.gold = _goldLast;

			if(_isInStateBet){
				_showEffect = true;
				panelUserInGame.RefreshGoldInfo();
				panelListChip.SetFocusChipAgain();
			}else{
				panelUserInGame.RefreshGoldInfo(true);
			}
		}else{
			List<PanelPlayerInfoInGameController> _tmplistOtherPlayerInfo = listOtherPlayerInfo;
			for(int i = 0; i < _tmplistOtherPlayerInfo.Count; i ++){
				if(_tmplistOtherPlayerInfo[i].data != null){
					if(_tmplistOtherPlayerInfo[i].data.IsEqual(_sessionid)){
						_posStartPanelGoldBonus = _tmplistOtherPlayerInfo[i].transform.position;

						_tmplistOtherPlayerInfo[i].data.gold = _goldLast;
						
						if(_isInStateBet){
							_showEffect = true;
							_tmplistOtherPlayerInfo[i].RefreshGoldInfo();
						}else{
							_tmplistOtherPlayerInfo[i].RefreshGoldInfo(true);
						}
						break;
					}
				}
			}
		}

		if(_showEffect){
			StartCoroutine(MyConstant.DoActionShowPopupWinGold(panelBonusGoldPrefab.gameObject, effectPoolManager, myCanvasGroup.transform
				, _posStartPanelGoldBonus, 0f, _goldAdd));

			// _tmpPanelGoldBonus = LeanPool.Spawn(panelBonusGoldPrefab.gameObject, _posStartPanelGoldBonus, Quaternion.identity, myCanvasGroup.transform).GetComponent<PanelBonusGoldInGameController>();
			// effectPoolManager.AddObject(_tmpPanelGoldBonus);
			// _tmpPanelGoldBonus.transform.position = _posStartPanelGoldBonus;
			// _tmpPanelGoldBonus.Show(_goldAdd);
		}
	}

	public void RefreshUIPanelTableBet(bool _updateNow = false){
		if(AnimalRacing_GamePlay_Manager.instance.animalRacingData == null){
			#if TEST
			Debug.LogError(">>> animalRacingData is NULL");
			#endif
			return;
		}
		if(myCurrentState != State.Show){
			#if TEST
			Debug.LogError(">>> Không ở trong trang đặt cược nên ko RefreshUIPanelTableBet");
			#endif
			return;
		}
		if(AnimalRacing_GamePlay_Manager.instance.currentState != AnimalRacing_GamePlay_Manager.State.Bet){
			#if TEST
			Debug.LogError(">>> Không ở trong trang đặt cược nên ko RefreshUIPanelTableBet");
			#endif
			return;
		}
		
		panelTableBet.SetMyBet(AnimalRacing_GamePlay_Manager.instance.animalRacingData.listMyBets, _updateNow);
		panelTableBet.SetGlobalBet(AnimalRacing_GamePlay_Manager.instance.animalRacingData.listGlobalBets, _updateNow);
	}

	public void RefreshUIPanelCurrentScore(bool _updateNow = false){
		if(AnimalRacing_GamePlay_Manager.instance.animalRacingData == null){
			#if TEST
			Debug.LogError(">>> animalRacingData is NULL");
			#endif
			return;
		}
		if(myCurrentState != State.Show){
			#if TEST
			Debug.LogError(">>> Không ở trong trang đặt cược nên ko RefreshUIPanelCurrentScore");
			#endif
			return;
		}
		if(AnimalRacing_GamePlay_Manager.instance.currentState != AnimalRacing_GamePlay_Manager.State.Bet){
			#if TEST
			Debug.LogError(">>> Không ở trong trang đặt cược nên ko RefreshUIPanelTableBet");
			#endif
			return;
		}
		
		panelTableBet.SetCurrentScore(AnimalRacing_GamePlay_Manager.instance.animalRacingData.listCurrentScore, _updateNow);
	}

	public void RefreshUIPanelUserData(){
		if(myCurrentState != State.Show){
			Debug.LogError(">>> Không ở trong trang đặt cược nên ko RefreshUIPanelCurrentScore");
			return;
		}
		panelListChip.RefreshListChips();
	}

	public void RefreshUIPanelListPlayerViewer(){
		if(AnimalRacing_GamePlay_Manager.instance.animalRacingData == null){
			Debug.LogError(">>> animalRacingData is NULL");
			return;
		}
		if(myCurrentState != State.Show){
			Debug.LogError(">>> Không ở trong trang đặt cược nên ko RefreshUIPanelListPlayerViewer");
			return;
		}
		
		for(int i = 0; i < AnimalRacing_GamePlay_Manager.instance.animalRacingData.listOtherPlayerData.Count; i++){
			// Debug.LogError(">>> " + AnimalRacing_GamePlay_Manager.animalRacingData.listOtherPlayerData[i].sessionId + " - " + AnimalRacing_GamePlay_Manager.animalRacingData.listOtherPlayerData[i].nameShowInGame);
			if(AnimalRacing_GamePlay_Manager.instance.animalRacingData.listOtherPlayerData[i].sessionId != -1){
				listOtherPlayerInfo[i].InitData(AnimalRacing_GamePlay_Manager.instance.animalRacingData.listOtherPlayerData[i]);
				listOtherPlayerInfo[i].Show();
			}else{
				listOtherPlayerInfo[i].Hide();
			}
		}
	}

	public void RefreshUIPanelListHistory(){
		if(AnimalRacing_GamePlay_Manager.instance.animalRacingData == null){
			Debug.LogError(">>> animalRacingData is NULL");
			return;
		}
		if(myCurrentState != State.Show){
			Debug.LogError(">>> Không ở trong trang đặt cược nên ko RefreshUIPanelCurrentScore");
			return;
		}
		if(AnimalRacing_GamePlay_Manager.instance.animalRacingData.listHistoryData != null){
			betHistoryController.ResetData();
			for(int i = 0; i < AnimalRacing_GamePlay_Manager.instance.animalRacingData.listHistoryData.Count; i++){
				AnimalRacingData.HistoryData _tmpHistory = AnimalRacing_GamePlay_Manager.instance.animalRacingData.listHistoryData[i];
				betHistoryController.SetDataHistory(_tmpHistory.animalType, _tmpHistory.score, i);
			}
		}
	}

	private void OnDestroy() {
        StopAllCoroutines();
    }
}