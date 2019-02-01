using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;

public class LuckyWheelController : UIHomeScreenController {

	public override UIType myType {
		get {
			return UIType.LuckyWheel;
		}
	}

	public override bool isSubScreen {
		get {
			return true;
		}
	}

	[System.Serializable] public class MyBetWin{
		public int idBetToWinValueDetail;
		public Text txtBet;
	}

	[SerializeField] Transform mainContainer;
	[SerializeField] RectTransform placeHolderMainContainer;
	[SerializeField] Text txtMyGoldInfo;
	[SerializeField] Transform startEffGoldFlyHolder;
	[SerializeField] Transform endEffGoldFlyHolder;
	[SerializeField] Transform showEffPanelGoldBonusEffPlaceHolder;
	
	[Header("Panel Bet")]
	[SerializeField] Text txtCurrentBet;
	[SerializeField] List<MyBetWin> listMyBetWinInfo;

	[Header("Wheel")]
	public Transform wheel;
	public float elements_Spread; 	// Empty space between elements

	[Header("Prefabs")]
	[SerializeField] GameObject goldPrefab;
	[SerializeField] GameObject panelBonusGoldPrefab;

	[Header("Setting")]
	public MySortingLayerInfo sortingLayerInfo_GoldObject;
	public bool centerOnElement = false;
	public float rotation_MinCycles = 2;  // how many full laps it does every time it plays
	public float rotation_MaxCycles = 4;
	public float rotation_MinTime = 3;  // how long it stays rotating
	public float rotation_MaxTime = 4;
	public float speedUpTime = 0.2f; // gia tốc
	public float friction = 5; // ma sát
	public float timeShowScreen;
	public float timeHideScreen;

	private bool mIsPlaying = false;
	private int mForceReward = -1;
	
	float deltaAngle, ratioScale;
	bool firstInit;
	long virtualMyGold, realMyGold, currentBet;
	List<long> listBetWinValue;
	int indexBet;
	LTDescr tweenCanvasGroup, tweenMainContainer, tweenWheelForever;
	IEnumerator actionTweenMyGoldInfo;
	System.Action<MessageReceiving, int> callbackGetBetWin;
	MySimplePoolManager effectPoolManager;

	#region Init / Show / Hide
	public override void InitData (){
		if(!firstInit){
			RectTransform _rectMainContainer = mainContainer.GetComponent<RectTransform>();
			ratioScale = placeHolderMainContainer.sizeDelta.x / _rectMainContainer.sizeDelta.x;
			// Debug.LogError(placeHolderMainContainer.sizeDelta.x + " - " + _rectMainContainer.sizeDelta.x + " - " + ratioScale);
			
			deltaAngle = 360f / listMyBetWinInfo.Count;

			Vector3 _pos = placeHolderMainContainer.transform.position;
			mainContainer.position = _pos;

			firstInit = true;
		}
		mIsPlaying = false;
		callbackGetBetWin = null;
		mForceReward = -1;
		wheel.rotation = Quaternion.identity;

		listBetWinValue = new List<long>();
		indexBet = 0;
		currentBet = GameInformation.instance.luckyWheelInfo.bet[indexBet];
		txtCurrentBet.text = MyConstant.GetMoneyString(currentBet, 9999);
		RefreshListBet();
		
		if(effectPoolManager == null){
			effectPoolManager = new MySimplePoolManager();
		}
		realMyGold = DataManager.instance.userData.gold;

		RefreshMyGoldInfo(true);

		onPressBack = () => {
			OnButtonBackClicked();
		};
		CoreGameManager.instance.RegisterNewCallbackPressBackKey (onPressBack);
	}

	public override void Show (){
		if(currentState == State.Show){
			return;
		}

		currentState = State.Show;
		myCanvasGroup.alpha = 0f;
		myCanvasGroup.blocksRaycasts = true;

		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_TogglePanel);

		if(tweenCanvasGroup != null){
			LeanTween.cancel(tweenCanvasGroup.uniqueId);
			tweenCanvasGroup = null;
		}
		tweenCanvasGroup = LeanTween.alphaCanvas(myCanvasGroup, 1f, timeShowScreen).setEase(LeanTweenType.easeOutBack).setOnComplete(()=>{
			tweenCanvasGroup = null;
		});
		
		if(tweenMainContainer != null){
			LeanTween.cancel(tweenMainContainer.uniqueId);
			tweenMainContainer = null;
		}
		mainContainer.localScale = Vector3.one * (0.6f * ratioScale);
		tweenMainContainer = LeanTween.scale(mainContainer.gameObject, Vector3.one * ratioScale, timeShowScreen).setEase(LeanTweenType.easeOutBack).setOnComplete(()=>{
			tweenMainContainer = null;
		});
	}

	public override void Hide (){
		if(currentState == State.Hide){
			return;
		}
		if (onPressBack != null) {
			CoreGameManager.instance.RemoveCurrentCallbackPressBackKey (onPressBack);
			onPressBack = null;
		}
		
		currentState = State.Hide;
		myCanvasGroup.blocksRaycasts = false;

		callbackGetBetWin = null;
		if(actionTweenMyGoldInfo != null){
			StopCoroutine(actionTweenMyGoldInfo);
			actionTweenMyGoldInfo = null;
		}
		if(tweenWheelForever != null){
			LeanTween.cancel(tweenWheelForever.uniqueId);
			tweenWheelForever = null;
		}

		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_TogglePanel);

		if(tweenCanvasGroup != null){
			LeanTween.cancel(tweenCanvasGroup.uniqueId);
			tweenCanvasGroup = null;
		}
		tweenCanvasGroup = LeanTween.alphaCanvas(myCanvasGroup, 0f, timeHideScreen).setOnComplete(()=>{
			if(effectPoolManager != null){
				effectPoolManager.ClearAllObjectsNow();
			}
			tweenCanvasGroup = null;
			base.Hide();
		}).setEase(LeanTweenType.easeInBack);

		if(tweenMainContainer != null){
			LeanTween.cancel(tweenMainContainer.uniqueId);
			tweenMainContainer = null;
		}
		tweenMainContainer = LeanTween.scale(mainContainer.gameObject, Vector3.one * (0.6f * ratioScale), timeHideScreen).setEase(LeanTweenType.easeInBack).setOnComplete(()=>{
			tweenMainContainer = null;
		});
	}
	#endregion

	private void GetRandomElement( ref int ElementIdx, out float Angle ){
		int forceElement = ElementIdx;
		ElementIdx = 0;
		Angle = -90; // điểm neo tại vị trí góc 90 độ
		
		float RndPos = forceElement>=0 ? listMyBetWinInfo.Count+1 : Random.value * listMyBetWinInfo.Count;
		
		Angle -= deltaAngle/2;

		for (int i=0; i<listMyBetWinInfo.Count; ++i)
		{
			RndPos --;
			
			if (RndPos<0 || (forceElement>=0 && i==forceElement)){
				if (centerOnElement){
					Angle += 0.5f * deltaAngle;
				}else{
					Angle += Mathf.Lerp(0.2f, 0.8f, Random.value) * (deltaAngle - elements_Spread);
				}
				ElementIdx = i;
				Angle += 90;
				return;
			}else{
				Angle += deltaAngle;
			}
		}
	}

	private IEnumerator DoPlay( int TargetElementIdx, float ElementAngle , long _rewardValue = -1){
		if (mIsPlaying){
			yield break;
		}
		mIsPlaying = true;
		
		float _InitialAngle = wheel.rotation.eulerAngles.z;
		
		float _TotalRotation = Mathf.DeltaAngle (_InitialAngle, ElementAngle);
		if (_TotalRotation < 0)
			_TotalRotation += 360;
		
		float _NumCycles = Random.Range (rotation_MinCycles, rotation_MaxCycles);
		_TotalRotation += Mathf.CeilToInt (_NumCycles) * 360;
		
		float _TotalTime = Random.Range (rotation_MinTime, rotation_MaxTime);
		float _InitialTime = Time.time;

		bool _finished = false;
		while(true){
			_finished = UpdateRotation( _InitialAngle, _TotalRotation, _InitialTime, _TotalTime, speedUpTime );

			if (_finished)
				break;
			else
				yield return null;
			
			// The coroutine was asked to be stopped
			if (!mIsPlaying){
				yield break;
			}
		}
		yield return Yielders.Get(0.1f);

		if(_rewardValue > 0){
			StartCoroutine(MyConstant.DoActionShowPopupWinGold(panelBonusGoldPrefab, effectPoolManager, transform
				, showEffPanelGoldBonusEffPlaceHolder.position, 1f, _rewardValue
				, ()=>{
					RefreshMyGoldInfo();
				}));
			yield return StartCoroutine(MyConstant.DoActionShowEffectGoldFly(goldPrefab, effectPoolManager, sortingLayerInfo_GoldObject
				, startEffGoldFlyHolder.position, endEffGoldFlyHolder.position, 10, 1f, 0.5f
				, ()=>{
					MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Gold);
				}));
			// yield return PopupManager.Instance.CreatePopupReward(_rewardDetail);
		}

		mIsPlaying = false;
	}

	private bool UpdateRotation( float initialAngle, float totalRotation, float initialTime, float totalTime, float _speedUpTime ){
		float _elapsedTime = Time.time - initialTime;
		float _dt = Mathf.Clamp01(_elapsedTime / totalTime);
			
		float _centerT = _speedUpTime / totalTime; // Point at which it changes from EasyIn to EasyOut
		
		if (_dt<_centerT)	// Start slowly and speedup
		{
			float _t = (_dt/_centerT);
			_dt = Mathf.Pow(_t, friction)*_centerT;  // Easy Out
		}
		else
		{
			// Ends slowly
			float _d = (1-_centerT);
			float _t = (_dt-_centerT)/_d;
			_dt = 1-Mathf.Pow(1-_t,friction); // Easy In
			_dt = _dt*_d+_centerT;
		}
			
		float _angle = initialAngle + _dt*totalRotation;

		// Almost there, skip the rest (the slow down at the end makes the final part too slow)
		if (Mathf.Abs (_angle - (initialAngle + totalRotation)) < 0.5f)
		{
			_angle = initialAngle + totalRotation;
			_dt = 1;
		}
		wheel.rotation = Quaternion.Euler (0, 0, _angle);

		return _dt >= 1;
	}

	public void RefreshListBet(){
		listBetWinValue.Clear();
		long _bet = 0;
		for(int i  = 0; i < GameInformation.instance.luckyWheelInfo.listDetail.Count; i++){
			_bet = (long) Mathf.Round(currentBet * GameInformation.instance.luckyWheelInfo.listDetail[i].ratioWin);
			listBetWinValue.Add(_bet);
		}
		for(int i = 0; i < listMyBetWinInfo.Count; i ++){
			for(int j = 0; j < GameInformation.instance.luckyWheelInfo.listDetail.Count; j ++){
				if(listMyBetWinInfo[i].idBetToWinValueDetail == GameInformation.instance.luckyWheelInfo.listDetail[j].id){
					listMyBetWinInfo[i].txtBet.text = MyConstant.GetMoneyString(listBetWinValue[j], 99999);
				}
			}
		}
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
				int _idWinOriginal = GameInformation.instance.luckyWheelInfo.listDetail[_indexWin].id;
				List<int> _listIndex = new List<int>();
				for(int i = 0; i < listMyBetWinInfo.Count; i ++){
					if(listMyBetWinInfo[i].idBetToWinValueDetail == _idWinOriginal){
						_listIndex.Add(i);
					}
				}
				
				if(_listIndex.Count == 0){
					#if TEST
					Debug.LogError("BUG Logic (0)");
					#endif
					RefreshMyGoldInfo();

					float _TotalAngle;
					GetRandomElement (ref mForceReward, out _TotalAngle );
					
					StartCoroutine (DoPlay (mForceReward, _TotalAngle));
					mForceReward = -1;
				}else{
					// RewardDetail _rewardDetail = new RewardDetail(IItemInfo.ItemType.Gold, listBetWinValue[_indexWin]);

					mForceReward = _listIndex[Random.Range(0, _listIndex.Count)];
					float _TotalAngle;
					GetRandomElement (ref mForceReward, out _TotalAngle );
					
					StartCoroutine (DoPlay (mForceReward, _TotalAngle, listBetWinValue[_indexWin]));
					mForceReward = -1;
				}
			}else{
				float _TotalAngle;
				GetRandomElement (ref mForceReward, out _TotalAngle );
				
				StartCoroutine (DoPlay (mForceReward, _TotalAngle));
				mForceReward = -1;
			}
		}else{
			DataManager.instance.userData.gold = realMyGold;
			RefreshMyGoldInfo();
			
			PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
					, MyLocalize.GetString("Error/ConnectionError")
					, _error.ToString()
					, MyLocalize.GetString(MyLocalize.kOk));

			float _TotalAngle;
			GetRandomElement (ref mForceReward, out _TotalAngle );
			
			StartCoroutine (DoPlay (mForceReward, _TotalAngle));
			mForceReward = -1;
		}
	}

	// IEnumerator DoActionShowEffWinGold(Vector2 _startPoint, Vector2 _endPoint, int _numGold){
	// 	Vector2 _newStartPoint = Vector2.zero;
	// 	for(int i = 0; i < _numGold; i++){
	// 		_newStartPoint.x = Random.Range(_startPoint.x - 0.2f, _startPoint.x + 0.2f);
	// 		_newStartPoint.y = Random.Range(_startPoint.y - 0.2f, _startPoint.y + 0.2f);
	// 		GoldObjectController _gold = LeanPool.Spawn(goldPrefab, _newStartPoint, Quaternion.identity).GetComponent<GoldObjectController>();
	// 		effectPoolManager.AddObject(_gold);
	// 		_gold.InitData(sortingLayerInfo_GoldObject, 1);
	// 		StartCoroutine(_gold.DoActionMoveAndSelfDestruction(_endPoint, 0.5f, LeanTweenType.easeInBack, GameInformation.instance.globalAudioInfo.sfx_Gold));
	// 		if(_numGold > 1){
	// 			yield return Yielders.Get(0.06f);
	// 		}
	// 	}
	// }

	// IEnumerator DoActionShowPopupWinGold(float _timeDelay, long _goldAdd){
	// 	yield return Yielders.Get(_timeDelay);
	// 	PanelBonusGoldInGameController _tmpPanelGoldBonus = LeanPool.Spawn(panelBonusGoldPrefab.gameObject, showEffPanelGoldBonusEffPlaceHolder.position, Quaternion.identity, transform).GetComponent<PanelBonusGoldInGameController>();
	// 	effectPoolManager.AddObject(_tmpPanelGoldBonus);
	// 	_tmpPanelGoldBonus.transform.position = showEffPanelGoldBonusEffPlaceHolder.position;
	// 	_tmpPanelGoldBonus.Show(_goldAdd);
	// 	RefreshMyGoldInfo();
	// }

	#region On Button Clicked
	public void OnButtonSpinClicked(){
		if(mIsPlaying){
			return;
		}
		if(tweenWheelForever != null){
			return;
		}

		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);

		if(DataManager.instance.userData.gold < currentBet){
			PopupManager.Instance.CreateToast(MyLocalize.GetString("Global/NotEnoughMoney"));
			return;
		}
		
		callbackGetBetWin = CallbackGetBetWin;
		tweenWheelForever = LeanTween.rotateAround(wheel.gameObject, Vector3.forward, 360f, 0.5f).setEase(LeanTweenType.easeInSine).setOnComplete(()=>{
			tweenWheelForever = LeanTween.rotateAround(wheel.gameObject, Vector3.forward, 360f, 0.3f).setLoopCount(-1);
		});

		DataManager.instance.userData.gold -= currentBet;
		RefreshMyGoldInfo();

		// BetToWinTool _tmp = new BetToWinTool(currentBet,listBetWinValue.ToArray());
		// _tmp.ProcessWeight();
		// _tmp.Trace();

		OneHitAPI.BetToWin(currentBet, GameInformation.instance.luckyWheelInfo.listDetail, listBetWinValue
			, (_mess, _error) => {
				if(tweenWheelForever != null){
					LeanTween.cancel(tweenWheelForever.uniqueId);
					tweenWheelForever = null;
				}
				if(callbackGetBetWin != null){
					callbackGetBetWin(_mess, _error);
				}
			});
	}

	public void OnButtonBackClicked(){
		if(mIsPlaying){
			return;
		}
		if(tweenWheelForever != null){
			return;
		}
		
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);

		DataManager.instance.userData.gold = realMyGold;
		HomeManager.instance.ChangeScreen (myLastType);

		if (onPressBack != null) {
			CoreGameManager.instance.RemoveCurrentCallbackPressBackKey (onPressBack);
			onPressBack = null;
		}
	}

	public void OnBtnChangeBetClicked(int _flag){ // 0: giảm cược, 1: tăng cược
		if(mIsPlaying){
			return;
		}
		if(tweenWheelForever != null){
			return;
		}

		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);

		if(_flag == 0){
			indexBet --;
			if(indexBet < 0){
				indexBet = 0;
			}else{
				currentBet = GameInformation.instance.luckyWheelInfo.bet[indexBet];
				txtCurrentBet.text = MyConstant.GetMoneyString(currentBet, 9999);
				RefreshListBet();
			}
		}else if(_flag == 1){
			indexBet ++;
			if(indexBet > GameInformation.instance.luckyWheelInfo.bet.Count - 1){
				indexBet = GameInformation.instance.luckyWheelInfo.bet.Count - 1;
			}else{
				currentBet = GameInformation.instance.luckyWheelInfo.bet[indexBet];
				txtCurrentBet.text = MyConstant.GetMoneyString(currentBet, 9999);
				RefreshListBet();
			}
		}
	}
	#endregion
}
