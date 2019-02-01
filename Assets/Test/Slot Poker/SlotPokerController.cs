using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SlotPokerController : MonoBehaviour {

	[System.Serializable] class VideoPokerCardDetail{
		public PanelCardDetailController panelCard;
		public int indexCardDetail; // dùng để tham chiếu với listCardDetail
	}

	enum State{
		NewRound, 
		SpinAgain,
	}
	State myState;

	[SerializeField] Text txtMyGoldInfo;
	[SerializeField] List<Transform> listCol;
	[SerializeField] List<CanvasGroup> listCanvasGroupButtonHold;
	[SerializeField] List<CanvasGroup> listCanvasGroupButtonUnHold;
	[SerializeField] List<CanvasGroup> listTextHeld;
	[SerializeField] List<CardDetail> listCardDetail;
	[SerializeField] Text txtCardResult;
	
	[Header("Cards")]
	[SerializeField] List<VideoPokerCardDetail> cardsInCol_00;
	[SerializeField] List<VideoPokerCardDetail> cardsInCol_01;
	[SerializeField] List<VideoPokerCardDetail> cardsInCol_02;
	[SerializeField] List<VideoPokerCardDetail> cardsInCol_03;
	[SerializeField] List<VideoPokerCardDetail> cardsInCol_04;

	[Header("Settings")]
	[SerializeField] float wCard = 110f;
	[SerializeField] float hCard = 150f;
	[SerializeField] float deltaBetweenTwoCards = 30f;

	[Header("Prefabs")]
	[SerializeField] GameObject cardPrefabs;

	[Header("Variable")]
	public long currentBet;
	public List<long> listBetWinValue;

	SlotPokerGamePlayData videoPokerGamePlayData;
	IEnumerator actionSpin, actionTweenMyGoldInfo;
	List<int> listIndexGlobalCard;
	List<int> resultIndexCards;
	long virtualMyGold, realMyGold;
	System.Action<MessageReceiving, int> callbackGetBetWin;

	static int numCol = 5;
	bool isFirstSpin;

	private void Start() {
		if(listCanvasGroupButtonHold.Count != numCol){
			Debug.LogError(">>> Chưa kéo đủ " + numCol +" nút hold");
		}
		if(listCanvasGroupButtonUnHold.Count != numCol){
			Debug.LogError(">>> Chưa kéo đủ " + numCol +" nút UnHold");
		}
		
		if(listCol.Count != numCol){
			Debug.LogError(">>> Chưa kéo đủ " + numCol + " cột");
		}else{
			if(listCol[0].childCount != cardsInCol_00.Count){
				Debug.LogError(">>> Bug phần tử của Col 00");
			}
			if(listCol[1].childCount != cardsInCol_01.Count){
				Debug.LogError(">>> Bug phần tử của Col 01");
			}
			if(listCol[2].childCount != cardsInCol_02.Count){
				Debug.LogError(">>> Bug phần tử của Col 02");
			}
			if(listCol[3].childCount != cardsInCol_03.Count){
				Debug.LogError(">>> Bug phần tử của Col 03");
			}
			if(listCol[4].childCount != cardsInCol_04.Count){
				Debug.LogError(">>> Bug phần tử của Col 04");
			}
		}
		
		if(listCardDetail.Count != 52){
			Debug.LogError(">>> Chưa đủ bài");
		}
		for(int i = 0; i < listCanvasGroupButtonHold.Count; i++){
			listCanvasGroupButtonHold[i].interactable = false;
		}
		for(int i = 0; i < listCanvasGroupButtonUnHold.Count; i++){
			listCanvasGroupButtonUnHold[i].blocksRaycasts = false;
		}
		for(int i = 0; i < listTextHeld.Count; i++){
			listTextHeld[i].alpha = 0f;
		}

		txtCardResult.text = string.Empty;
		Color _c = txtCardResult.color;
		_c.a = 0;
		txtCardResult.color = _c;
		
		for(int i = 0; i < listCol.Count; i++){
			SetPos(listCol[i]);
		}
		myState = State.NewRound;
		isFirstSpin = true;

		resultIndexCards = new List<int>();
		listIndexGlobalCard = new List<int>(); // 0 -> 52
		for(int i = 0; i < listCardDetail.Count; i++){
			listIndexGlobalCard.Add(i);
		}

		SetCardRandomInFirst();

		realMyGold = DataManager.instance.userData.gold;
		RefreshMyGoldInfo(true);
		LoadGoldFromServer();

		videoPokerGamePlayData = new SlotPokerGamePlayData();

		currentBet = GameInformation.instance.slotPokerInfo.betDefault;
		listBetWinValue = new List<long>();
		long _bet = 0;
		for(int i  = 0; i < GameInformation.instance.slotPokerInfo.listDetail.Count; i++){
			_bet = (long) Mathf.Round(currentBet * GameInformation.instance.slotPokerInfo.listDetail[i].ratioWin);
			listBetWinValue.Add(_bet);
		}
		
		// StartCoroutine(DoActionTest());
		// StartCoroutine(DoActionTestAAAA());
	}

	void LoadGoldFromServer(){
		OneHitAPI.GetGoldGemById(DataManager.instance.userData.databaseId, DataManager.instance.userData.userId,
        (_messageReceiving, _error) => {
            if (_messageReceiving != null){
                DataManager.instance.userData.gold = _messageReceiving.readLong();
                #if TEST
                Debug.Log(">>>Recieving Data GetGoldGemById: Gold: " + DataManager.instance.userData.gold);
                #endif

				realMyGold = DataManager.instance.userData.gold;
                RefreshMyGoldInfo(true);
            }else{
                #if TEST
                Debug.LogError("Lỗi GetGoldGemById. Error code: " + _error);
                #endif
            }
        });
	}

	public void RefreshMyGoldInfo(bool _updateNow = false){
		if(_updateNow){
			if(actionTweenMyGoldInfo != null){
				StopCoroutine(actionTweenMyGoldInfo);
				actionTweenMyGoldInfo = null;
			}
			virtualMyGold = DataManager.instance.userData.gold;
			txtMyGoldInfo.text = MyConstant.GetMoneyString(virtualMyGold, 9999);
		}else{
			if(actionTweenMyGoldInfo != null){
				StopCoroutine(actionTweenMyGoldInfo);
				actionTweenMyGoldInfo = null;
			}
			actionTweenMyGoldInfo = MyConstant.TweenValue(virtualMyGold,  DataManager.instance.userData.gold, 5, (_valueUpdate)=>{
				virtualMyGold = _valueUpdate;
				txtMyGoldInfo.text = MyConstant.GetMoneyString(virtualMyGold, 9999);
			}, (_valueFinish)=>{
				virtualMyGold = _valueFinish;
				txtMyGoldInfo.text = MyConstant.GetMoneyString(virtualMyGold, 9999);
				actionTweenMyGoldInfo = null;
			});
			StartCoroutine(actionTweenMyGoldInfo);
		}
	}

	IEnumerator DoActionTestAAAA(){
		while(true){
			yield return Yielders.Get(2f);
			Debug.Log("-----------------------------");
			long _currentMilisecond = MyConstant.currentTimeMilliseconds;
			List<sbyte> _cards = new List<sbyte>();
			yield return StartCoroutine(videoPokerGamePlayData.FirstTime_GetCardsResult_PAIR((_cardsResult)=>{
				_cards = _cardsResult;
			}));
			for(int i = 0; i < _cards.Count; i++){
				for(int j = 0; j < listCardDetail.Count; j ++){
					if(_cards[i] == listCardDetail[j].cardId){
						Debug.Log(">>> " + listCardDetail[j].cardInfo.cardType.ToString());
					}
				}
			}
			Debug.Log(MyConstant.currentTimeMilliseconds - _currentMilisecond);
		}
	}

	IEnumerator DoActionTest(){
		yield return Yielders.Get(2f);
		Debug.Log(">>> Start Initdata");
		long _currentMilisecond = MyConstant.currentTimeMilliseconds;
		videoPokerGamePlayData = new SlotPokerGamePlayData();
		// videoPokerGamePlayData.GetListResultCardType(listCardDetail, new List<sbyte>());
		Debug.Log(videoPokerGamePlayData.totalCase + " -- " + (MyConstant.currentTimeMilliseconds - _currentMilisecond));
	
		// yield return Yielders.Get(2f);
		// Debug.Log(">>> Continue");
		// _currentMilisecond = MyConstant.currentTimeMilliseconds;
		// for(int i = 0; i < videoPokerGamePlayData.listResultCardType.Count; i++){
		// 	for(int j = 0; j < videoPokerGamePlayData.listResultCardType[i].listTypecards.Count; j++){

		// 	}
		// }
		
		// Debug.Log(MyConstant.currentTimeMilliseconds - _currentMilisecond);
	}

	void SetCardRandomInFirst(){
		int _tmpIndex = 0;
		int _indexCardDetail = 0;
		while(resultIndexCards.Count < numCol && listIndexGlobalCard.Count > 0){
			_tmpIndex = Random.Range(0, listIndexGlobalCard.Count);
			_indexCardDetail = listIndexGlobalCard[_tmpIndex];
			resultIndexCards.Add(_indexCardDetail);
			listIndexGlobalCard.RemoveAt(_tmpIndex);
		}
		if(resultIndexCards.Count < 5){
			Debug.LogError(">>> Bug Logic resultIndexCards.Count : " + resultIndexCards.Count);
		}else{
			for(int i = 0; i < resultIndexCards.Count; i++){
				Debug.Log(listCardDetail[resultIndexCards[i]].cardInfo.cardType);
			}

			SetCardsInColInFirst(0, cardsInCol_00);
			SetCardsInColInFirst(1, cardsInCol_01);
			SetCardsInColInFirst(2, cardsInCol_02);
			SetCardsInColInFirst(3, cardsInCol_03);
			SetCardsInColInFirst(4, cardsInCol_04);
		}

		ResetResultIndexCards();
	}

	void SetCardsInColInFirst(int _indexCol, List<VideoPokerCardDetail> _listVideoPokerCardDetail){
		for(int i = 0; i < _listVideoPokerCardDetail.Count; i++){
			if(i == 2){
				_listVideoPokerCardDetail[i].indexCardDetail = resultIndexCards[_indexCol];
				_listVideoPokerCardDetail[i].panelCard.ShowNow(listCardDetail[resultIndexCards[_indexCol]].cardInfo);
			}else{
				int _tmpIndex = Random.Range(0, listIndexGlobalCard.Count);
				int _indexCardDetail = listIndexGlobalCard[_tmpIndex];
				listIndexGlobalCard.RemoveAt(_tmpIndex);

				_listVideoPokerCardDetail[i].indexCardDetail = _indexCardDetail;
				_listVideoPokerCardDetail[i].panelCard.ShowNow(listCardDetail[_indexCardDetail].cardInfo);
			}
			_listVideoPokerCardDetail[i].panelCard.ResizeAgain(wCard, hCard);
		}
	}

	void ResetResultIndexCards(){
		if(resultIndexCards == null){
			resultIndexCards = new List<int>();
			for(int i = 0; i < numCol; i++){
				resultIndexCards[i] = -1;
			}
		}else{
			for(int i = 0; i < resultIndexCards.Count; i++){
				if(myState == State.NewRound || listCanvasGroupButtonHold[i].interactable){
					resultIndexCards[i] = -1;
				}
			}
		}
	}

	Coroutine StartSpin(){
		if(actionSpin == null){
			actionSpin = DoActionSpin();
			return StartCoroutine(actionSpin);
		}else{
			return null;
		}
	}

	IEnumerator DoActionSpin(){
		List<IEnumerator> _listAction = new List<IEnumerator>();

		for(int i = 0; i < numCol; i ++){
			if(myState == State.NewRound || listCanvasGroupButtonHold[i].interactable){
				_listAction.Add(DoActionTweenCol(listCol[i], i, 10 + i*4, 50 + i*8));
			}
		}

		if(_listAction.Count <= 0){
			Debug.LogError(">>> _listAction.Count = 0");
			yield break;
		}
		yield return CoroutineChain.Start
			.Parallel(_listAction.ToArray());

		// --- Show Effect Type Card Result --- //
		LeanTween.cancel(txtCardResult.gameObject);
		Color _c = txtCardResult.color;
		_c.a = 0;
		txtCardResult.color = _c;
		txtCardResult.transform.localScale = Vector3.one * 0.6f;

		List<sbyte> _cardResultValue = new List<sbyte>();
		for(int i = 0; i < resultIndexCards.Count; i ++){
			_cardResultValue.Add((sbyte)listCardDetail[resultIndexCards[i]].cardId);
		}
		PokerGamePlayData.TypeCardResult _typeCardResult = videoPokerGamePlayData.GetTypeCardResult(_cardResultValue);
		txtCardResult.text = PokerGamePlayData.GetStringTypeCardResult(_typeCardResult);
		
		bool _isFinished = false;
		float _timeShowEffectCardResult = 0.2f;
		LeanTween.alphaText(txtCardResult.rectTransform, 1f, _timeShowEffectCardResult).setEase(LeanTweenType.easeOutBack).setOnComplete(()=>{
			_isFinished = true;
		});
		LeanTween.scale(txtCardResult.gameObject, Vector3.one, _timeShowEffectCardResult).setEase(LeanTweenType.easeOutBack);
		yield return new WaitUntil(()=>_isFinished);
		yield return Yielders.Get(1f);
		_isFinished = false;
		LeanTween.alphaText(txtCardResult.rectTransform, 0f, 0.2f).setEase(LeanTweenType.easeInBack).setOnComplete(()=>{
			_isFinished = true;
		});
		LeanTween.scale(txtCardResult.gameObject, Vector3.zero, _timeShowEffectCardResult).setEase(LeanTweenType.easeInBack);
		yield return new WaitUntil(()=>_isFinished);
		// ----------------------------------------- //

		if(myState == State.NewRound){
			for(int i = 0; i < listCanvasGroupButtonHold.Count; i++){
				listCanvasGroupButtonHold[i].interactable = true;
			}
		}else{
			for(int i = 0; i < listCanvasGroupButtonHold.Count; i++){
				listCanvasGroupButtonHold[i].interactable = false;
			}
			for(int i = 0; i < listTextHeld.Count; i++){
				LeanTween.cancel(listTextHeld[i].gameObject);
				LeanTween.alphaCanvas(listTextHeld[i], 0f, 0.1f).setEase(LeanTweenType.easeInBack);
				LeanTween.scale(listTextHeld[i].gameObject, Vector3.zero, 0.1f).setEase(LeanTweenType.easeInBack);
			}
			myState = State.NewRound;
		}
		for(int i = 0; i < listCanvasGroupButtonUnHold.Count; i++){
			listCanvasGroupButtonUnHold[i].blocksRaycasts = false;
		}
		isFirstSpin = false;
		actionSpin = null;
	}

	// public void SetPosAgain(Transform _col){
	// 	int _index = 1;
	// 	float _moveToY = 0f - ((_index *hCard + hCard/2) + (_index*deltaBetweenTwoCards));
	// 	Vector3 _tmp = _col.localPosition;
	// 	_tmp.y = _moveToY;
	// 	_col.localPosition = _tmp;
	// }

	void SetPos(Transform _col){
		int _count = _col.childCount;
		// float _yStart = 0f - (_count * hCard + (_count - 1) * deltaBetweenTwoCards) / 2f;
		float _yStart = 0f;

		Vector3 _tmp = Vector3.zero;
		for(int i = 0; i < _count; i++){
			_tmp = _col.GetChild(i).localPosition;
			_tmp.y = _yStart + (i * (hCard + deltaBetweenTwoCards)) + hCard / 2f;
			_col.GetChild(i).localPosition = _tmp;
		}
	}

	IEnumerator DoActionTweenCol(Transform _col, int _indexCol, int _numIndexMove, int _stepMove){
		int _childColCount = _col.childCount;
		float _s = (_numIndexMove - 1) * (hCard + deltaBetweenTwoCards);

		// float _v = hCard; // 1 giây đi đươc quãng = 1 chiều cao lá bài
		// float _timeMove = _s / _v;

		float _deltaV = ((_s - (hCard + deltaBetweenTwoCards)) / _stepMove);

		float _yDisappear = 0f + hCard/2f;

		if(resultIndexCards[_indexCol] == -1){
			float _timeMovePerIndex = ((hCard + deltaBetweenTwoCards) / _deltaV) * Time.fixedDeltaTime;
			while(resultIndexCards[_indexCol] == -1){
				bool _isFinished = false;
				for(int i = 0; i < _childColCount; i ++){
					float _y = _col.GetChild(i).localPosition.y - hCard - deltaBetweenTwoCards;
					LeanTween.moveLocalY(_col.GetChild(i).gameObject, _y, _timeMovePerIndex).setOnComplete(()=>{
						_isFinished = true;
					});
				}
				yield return new WaitUntil(()=>_isFinished);

				float _yMax = _col.GetChild(0).localPosition.y;
				for(int i = 0; i < _childColCount; i ++){
					_yMax = _col.GetChild(0).localPosition.y;
					for(int j = 0; j < _childColCount; j ++){
						if(_col.GetChild(j).localPosition.y > _yMax){
							_yMax = _col.GetChild(j).localPosition.y;
						}
					}
				}

				Transform _lastChild = _col.GetChild(0);
				for(int i = 0; i < _childColCount; i ++){
					if(_col.GetChild(i).localPosition.y < _lastChild.localPosition.y){
						_lastChild = _col.GetChild(i);
					}
				}

				Vector3 _tmp = _lastChild.localPosition;
				_tmp.y = _yMax + hCard + deltaBetweenTwoCards;
				_lastChild.localPosition = _tmp;
			}
		}

		// --- SetUp bài cần ra cách _numIndexMove 3 lá --- //
		int _flagChangeResultCard = 0;
		int _tmpCountIndexMoveToChangeResultCard = 0;
		int _indexMoveChangeResultCard = _numIndexMove - 3;
		
		if(_indexMoveChangeResultCard <= 0){
			_flagChangeResultCard = -1;
		}else{
			if(resultIndexCards.Count == 0){
				_flagChangeResultCard = -1;
			}
		}
		// ------------------------------------------------ //
		
		int _tmpStep = 0;
		while(true){
			yield return null;
			for(int i = 0; i < _childColCount; i ++){
				Vector3 _tmp = _col.GetChild(i).localPosition;
				if(_tmp.y <= _yDisappear){
					if(_flagChangeResultCard >= 0){
						_tmpCountIndexMoveToChangeResultCard ++;
						if(_tmpCountIndexMoveToChangeResultCard == _indexMoveChangeResultCard){
							_flagChangeResultCard = 1;
						}
					}
					switch(_indexCol){
					case 0:
						SetRandomNewCard(cardsInCol_00[i], _flagChangeResultCard == 1 ? resultIndexCards[_indexCol] : -1);
						break;
					case 1:
						SetRandomNewCard(cardsInCol_01[i], _flagChangeResultCard == 1 ? resultIndexCards[_indexCol] : -1);
						break;
					case 2:
						SetRandomNewCard(cardsInCol_02[i], _flagChangeResultCard == 1 ? resultIndexCards[_indexCol] : -1);
						break;
					case 3:
						SetRandomNewCard(cardsInCol_03[i], _flagChangeResultCard == 1 ? resultIndexCards[_indexCol] : -1);
						break;
					case 4:
						SetRandomNewCard(cardsInCol_04[i], _flagChangeResultCard == 1 ? resultIndexCards[_indexCol] : -1);
						break;
					}
					float _yMax = _col.GetChild(0).localPosition.y;
					for(int j = 0; j < _childColCount; j ++){
						if(_col.GetChild(j).localPosition.y > _yMax){
							_yMax = _col.GetChild(j).localPosition.y;
						}
					}
					_tmp.y = _yMax + hCard + deltaBetweenTwoCards;

					if(_flagChangeResultCard == 1){
						_flagChangeResultCard = -1;
					}
					_col.GetChild(i).localPosition = _tmp;
				}
			}

			for(int i = 0; i < _childColCount; i ++){
				Vector3 _tmp = _col.GetChild(i).localPosition;
				_tmp.y -= _deltaV;
				_col.GetChild(i).localPosition = _tmp;
			}
			_tmpStep ++;
			if(_tmpStep >= _stepMove){
				yield return null;
				break;
			}
		}

		// Debug.LogError(_indexCol + " - " + _tmpCountIndexMoveToChangeResultCard + " - " + _indexMoveChangeResultCard);
		for(int i = 0; i < _childColCount; i ++){
			float _y = _col.GetChild(i).localPosition.y - hCard - deltaBetweenTwoCards;
			LeanTween.moveLocalY(_col.GetChild(i).gameObject, _y, 0.3f).setEase(LeanTweenType.easeOutBack);
		}
		yield return Yielders.Get(0.4f);

		// --- Chỉnh lại position của phần tử trong cột --- //
		float _yMax02 = _col.GetChild(0).localPosition.y;
		for(int i = 0; i < _childColCount; i ++){
			_yMax02 = _col.GetChild(0).localPosition.y;
			for(int j = 0; j < _childColCount; j ++){
				if(_col.GetChild(j).localPosition.y > _yMax02){
					_yMax02 = _col.GetChild(j).localPosition.y;
				}
			}
		}

		Transform _lastChild02 = _col.GetChild(0);
		for(int i = 0; i < _childColCount; i ++){
			if(_col.GetChild(i).localPosition.y < _lastChild02.localPosition.y){
				_lastChild02 = _col.GetChild(i);
			}
		}

		Vector3 _tmp02 = _lastChild02.localPosition;
		_tmp02.y = _yMax02 + hCard + deltaBetweenTwoCards;
		_lastChild02.localPosition = _tmp02;
		// -------------------------------------------------- //

		yield return null;
	}

	void SetRandomNewCard(VideoPokerCardDetail _videoPokerCardDetail, int _forcedIndexCardDetail = -1){
		int _indexCardCatched = _videoPokerCardDetail.indexCardDetail;

		int _indexCardDetail = 0;
		bool _canRandomIndexCardDetail = false;
		if(_forcedIndexCardDetail == -1){
			_canRandomIndexCardDetail = true;
		}else{
			if(!resultIndexCards.Contains(_forcedIndexCardDetail)){
				#if TEST
				Debug.LogError(">>> Chưa setup vào resultIndexCards: " + _forcedIndexCardDetail);
				#endif
				_canRandomIndexCardDetail = true;
			}
		}
		if(_canRandomIndexCardDetail){
			int _tmpIndex = Random.Range(0, listIndexGlobalCard.Count);
			_indexCardDetail = listIndexGlobalCard[_tmpIndex];
			listIndexGlobalCard.RemoveAt(_tmpIndex);
		}else{
			_indexCardDetail = _forcedIndexCardDetail;
		}
		

		bool _canAddToListIndexGlobalCard = false;
		if(resultIndexCards.Count == 0){
			_canAddToListIndexGlobalCard = true;
		}else if(resultIndexCards.Count == numCol){
			_canAddToListIndexGlobalCard = true;
			for(int i = 0; i < resultIndexCards.Count; i++){
				if(resultIndexCards[i] == _indexCardCatched){
					_canAddToListIndexGlobalCard = false;
					break;
				}
			}
		}else{
			#if TEST
			Debug.LogError(">>> Lỗi list số lượng resultIndexCards: " + resultIndexCards.Count);
			#endif
		}

		if(_canAddToListIndexGlobalCard){
			listIndexGlobalCard.Add(_indexCardCatched);
		}

		// Debug.Log(">>> " + _indexCardDetail + " - " + _indexCardCatched);

		_videoPokerCardDetail.panelCard.ShowNow(listCardDetail[_indexCardDetail].cardInfo);
		_videoPokerCardDetail.indexCardDetail = _indexCardDetail;
	}

	void CHEAT(){
		ResetResultIndexCards();
		List<ICardInfo.CardType> _listCheatCardType = new List<ICardInfo.CardType>(); 
		_listCheatCardType.Add(ICardInfo.CardType._10_4);
		_listCheatCardType.Add(ICardInfo.CardType._J_4);
		_listCheatCardType.Add(ICardInfo.CardType._Q_4);
		_listCheatCardType.Add(ICardInfo.CardType._K_4);
		_listCheatCardType.Add(ICardInfo.CardType._A_4);

		for(int i = 0; i < _listCheatCardType.Count; i ++){
			bool _isExist = false;
			for(int j = 0; j < listCardDetail.Count; j ++){
				if(listCardDetail[j].cardInfo.cardType == _listCheatCardType[i]){
					resultIndexCards[i] = j;
					_isExist = true;
					break;
				}
			}
			if(!_isExist){
				Debug.LogError("Card " + _listCheatCardType[i].ToString() + " không có trong listCardDetail");
			}
		}
		
		for(int i = 0; i < resultIndexCards.Count; i++){
			for(int j = 0; j < listIndexGlobalCard.Count; j++){
				if(listIndexGlobalCard[j] == resultIndexCards[i]){
					listIndexGlobalCard.RemoveAt(j);
					break;
				}
			}
		}
	}

	void RandomNewResultIndexCards(){
		ResetResultIndexCards();

		List<int> _tmpNewListIndexGlobalCard = new List<int>(); // 0 -> 52
		for(int i = 0; i < listCardDetail.Count; i++){
			_tmpNewListIndexGlobalCard.Add(i);
		}
		for(int i = 0; i < numCol; i++){
			if(myState == State.NewRound || listCanvasGroupButtonHold[i].interactable){
				int	_tmpIndex = Random.Range(0, _tmpNewListIndexGlobalCard.Count);
				int	_indexCardDetail = _tmpNewListIndexGlobalCard[_tmpIndex];
				resultIndexCards[i] = _indexCardDetail;
				_tmpNewListIndexGlobalCard.RemoveAt(_tmpIndex);
			}
		}

		for(int i = 0; i < resultIndexCards.Count; i++){
			for(int j = 0; j < listIndexGlobalCard.Count; j++){
				if(listIndexGlobalCard[j] == resultIndexCards[i]){
					listIndexGlobalCard.RemoveAt(j);
					break;
				}
			}
		}

		for(int i = 0; i < resultIndexCards.Count; i++){
			Debug.Log(listCardDetail[resultIndexCards[i]].cardInfo.cardType);
		}
	}

	void CallbackGetBetWin(MessageReceiving _mess, int _error){
		if(_mess != null){
			sbyte _caseCheck = _mess.readByte();
			short _indexWin = _mess.readShort();
			long _goldAdd = 0;
			bool _getReward = false;
			switch(_caseCheck){
			case 0: // lỗi server gold
				DataManager.instance.userData.gold = realMyGold;
				RefreshMyGoldInfo();
				
				PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kWarning)
					, MyLocalize.GetString("Error/ConnectionError")
					, _caseCheck.ToString()
					, MyLocalize.GetString(MyLocalize.kOk));
				break;
			case -2: // ko đủ tiền cược
				realMyGold = _mess.readLong();
				DataManager.instance.userData.gold = realMyGold;
				RefreshMyGoldInfo();

				PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kWarning)
					, MyLocalize.GetString("Global/NotEnoughMoney")
					, _caseCheck.ToString()
					, MyLocalize.GetString(MyLocalize.kOk));
				break;
			case 1: // xử lý thành công
				_getReward = true;
				_goldAdd = _mess.readLong();
				realMyGold = _mess.readLong();
				DataManager.instance.userData.gold = realMyGold;
				#if TEST
				Debug.Log(">>> BetToWin: " + _indexWin + "|" + _goldAdd + "|" + realMyGold);
				#endif
				break;
			default:
				DataManager.instance.userData.gold = realMyGold;
				RefreshMyGoldInfo();
				#if TEST
				Debug.LogError("BUG Logic (1): " + _caseCheck);
				#endif
				break;
			}
			
			if(_getReward){
				
			}else{
				
			}
		}else{
			DataManager.instance.userData.gold = realMyGold;
			RefreshMyGoldInfo();
			
			PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
					, MyLocalize.GetString("Error/ConnectionError")
					, _error.ToString()
					, MyLocalize.GetString(MyLocalize.kOk));
		}
	}
	
	#region On Button Clicked
	public void OnBtnSpinClicked(){
		if(actionSpin != null){
			return;
		}

		ResetResultIndexCards();
		
		callbackGetBetWin = CallbackGetBetWin;

		// CHEAT();
		StartSpin();
		// StartCoroutine(SpinCheat());

		OneHitAPI.BetToWin(currentBet, GameInformation.instance.slotPokerInfo.listDetail, listBetWinValue
			, (_mess, _error) => {
				if(callbackGetBetWin != null){
					callbackGetBetWin(_mess, _error);
				}
			});
	}

	public void OnBtnHoldColClicked(int _indexCol){
		if(actionSpin != null){
			return;
		}
		if(!listCanvasGroupButtonHold[_indexCol].interactable){
			return;
		}
		int _tmpCount = 0;
		for(int i = 0; i < listCanvasGroupButtonHold.Count; i++){
			if(!listCanvasGroupButtonHold[i].interactable){
				_tmpCount ++;
			}
		}
		if(_tmpCount >= listCanvasGroupButtonHold.Count - 1){
			return;
		}

		myState = State.SpinAgain;
		listCanvasGroupButtonHold[_indexCol].interactable = false;
		listCanvasGroupButtonUnHold[_indexCol].blocksRaycasts = true;

		LeanTween.cancel(listTextHeld[_indexCol].gameObject);
		LeanTween.alphaCanvas(listTextHeld[_indexCol], 1f, 0.2f).setEase(LeanTweenType.easeOutBack);
		listTextHeld[_indexCol].transform.localScale = Vector3.one * 0.6f;
		LeanTween.scale(listTextHeld[_indexCol].gameObject, Vector3.one, 0.2f).setEase(LeanTweenType.easeOutBack);
	}

	public void OnBtnUnHoldColClicked(int _indexCol){
		if(actionSpin != null){
			return;
		}
		if(!listCanvasGroupButtonUnHold[_indexCol].blocksRaycasts){
			return;
		}
		listCanvasGroupButtonHold[_indexCol].interactable = true;
		listCanvasGroupButtonUnHold[_indexCol].blocksRaycasts = false;

		LeanTween.cancel(listTextHeld[_indexCol].gameObject);
		LeanTween.alphaCanvas(listTextHeld[_indexCol], 0f, 0.2f).setEase(LeanTweenType.easeInBack);
		LeanTween.scale(listTextHeld[_indexCol].gameObject, Vector3.zero, 0.2f).setEase(LeanTweenType.easeInBack);

		int _tmpCount = 0;
		for(int i = 0; i < listCanvasGroupButtonHold.Count; i++){
			if(listCanvasGroupButtonHold[i].interactable){
				_tmpCount ++;
			}
		}
		if(_tmpCount == numCol){
			myState = State.NewRound;
		}
	}
	#endregion

	public void GetInfo(){
		List<long> _listBetWinValue = new List<long>();
		long _bet = 0;
		long _currentBet = GameInformation.instance.slotPokerInfo.betDefault;
		for(int i  = 0; i < GameInformation.instance.slotPokerInfo.listDetail.Count; i++){
			_bet = (long) Mathf.Round(_currentBet * GameInformation.instance.slotPokerInfo.listDetail[i].ratioWin);
			_listBetWinValue.Add(_bet);
		}

		BetToWinTool _tmp = new BetToWinTool(_currentBet, _listBetWinValue.ToArray());
		_tmp.ProcessWeight();
		_tmp.Trace();
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(SlotPokerController))]
public class SlotPokerController_Editor : Editor
{
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		SlotPokerController myScript = (SlotPokerController) target;
		
		GUILayout.Space(30);
		GUILayout.Label(">>> For Test <<<");

		if (GUILayout.Button ("Get Info")) {
			myScript.GetInfo();
		}
	}
}
#endif