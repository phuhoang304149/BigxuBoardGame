using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;

public class LeaderboardScreenController : UIHomeScreenController {

	public override UIType myType{
		get{ 
			return UIType.Leaderboard;
		}
	}

	public override bool isSubScreen{
		get{ 
			return true;
		}
	}
	
	[SerializeField] Transform mainContainer;
	[SerializeField] Transform myOptionContent;
	[SerializeField] Text txtEmpty;
	[SerializeField] Transform panelLoading;
	[SerializeField] Text txtTitleLeaderboard;
	[SerializeField] Text txtLastTimeUpdated;

	[Header("Prefab")]
	[SerializeField] GameObject prefabLeaderboardOption;

	[Header("Setting")]
	[SerializeField] float timeShowScreen;
	[SerializeField] float timeHideScreen;

	MySimplePoolManager optionInfoPoolManager;
	LTDescr tweenCanvasGroup, tweenMainContainer;
	bool canLoadDataFromSv;

	private void Awake() {
		panelLoading.gameObject.SetActive(false);
		txtEmpty.gameObject.SetActive(false);
		base.Hide();
	}

	#region Init / Show / Hide
	public override void InitData (){
		txtTitleLeaderboard.text = MyLocalize.GetString("Global/Leaderboard_TopGold");
		System.DateTime _lastTimeUpdatedTop = DataManager.instance.leaderboardData.lastTimeUpdateTop;
		txtLastTimeUpdated.text = "Updated at: " + string.Format("{0:00}/{1:00}/{2} - {3:00}:{4:00}", _lastTimeUpdatedTop.Day, _lastTimeUpdatedTop.Month, _lastTimeUpdatedTop.Year, _lastTimeUpdatedTop.Hour, _lastTimeUpdatedTop.Minute);
		// txtTitleRank.text = MyLocalize.GetString("Global/Rank");
		// txtTitleInfo.text = MyLocalize.GetString("Global/Info");
		// txtTitleGold.text = MyLocalize.GetString("Global/Gold");

		optionInfoPoolManager = new MySimplePoolManager();

		if(System.DateTime.Now < DataManager.instance.leaderboardData.nextTimeToGetNewList){
			canLoadDataFromSv = false;
			if(DataManager.instance.leaderboardData.topGold.Count > 0){
				CreatePanels();
			}else{
				txtEmpty.gameObject.SetActive(true);
			}
		}else{
			canLoadDataFromSv = true;
		}

		onPressBack = () => {
			HomeManager.instance.ChangeScreen (myLastType);
		};
		CoreGameManager.instance.RegisterNewCallbackPressBackKey (onPressBack);
	}

	void CreatePanels(){
		if(optionInfoPoolManager != null && optionInfoPoolManager.listObjects.Count > 0){
			optionInfoPoolManager.ClearAllObjectsNow();
		}
		optionInfoPoolManager = new MySimplePoolManager(); 
		int _numberTop = DataManager.instance.leaderboardData.topGold.Count;
		if(_numberTop > 10){
			_numberTop = 10;
		}
		for(int i = 0; i < _numberTop; i ++){
			Leaderboard_OptionInfo_Controller _tmpPanel = LeanPool.Spawn(prefabLeaderboardOption, Vector3.zero, Quaternion.identity, myOptionContent).GetComponent<Leaderboard_OptionInfo_Controller>();
			_tmpPanel.InitData(i + 1, DataManager.instance.leaderboardData.topGold[i]);
			optionInfoPoolManager.AddObject(_tmpPanel);
		}
	}

	void LoadDataFromServer(){
		panelLoading.gameObject.SetActive(true);

		OneHitAPI.GetTopGold((_messageReceiving, _error) =>
		{
			if(HomeManager.instance != null){
				panelLoading.gameObject.SetActive(false);
			}
			if(_messageReceiving != null){
				long _tmpDeltaTimeUpdateTop = _messageReceiving.readLong();
				sbyte _numberTop = _messageReceiving.readByte();

				if(_numberTop > 0){
					List<UserData> _listTopGold = new List<UserData>();
					for(int i = 0; i < _numberTop; i ++){
						UserData _userData = new UserData();
						_userData.InitData();
						_userData.GetMoreUserData(_messageReceiving);
						_listTopGold.Add(_userData);
					}
					// 36134 - 1/1/1970 7:00:36 AM
					System.DateTime _start = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
					long _currentMillisecondsLastUpdate = MyConstant.currentTimeMilliseconds - _tmpDeltaTimeUpdateTop;
					DataManager.instance.leaderboardData.lastTimeUpdateTop = _start.AddMilliseconds(_currentMillisecondsLastUpdate).ToLocalTime();
					DataManager.instance.leaderboardData.nextTimeToGetNewList = System.DateTime.Now.AddHours(1);
					DataManager.instance.leaderboardData.topGold = _listTopGold;
					DataManager.instance.leaderboardData.SortListTopGoldAgain();
					// Debug.Log(_tmpDeltaTimeUpdateTop + " - " + DataManager.instance.leaderboardData.lastTimeUpdateTop);
				}

				if(HomeManager.instance != null){
					if(currentState == State.Show){
						System.DateTime _lastTimeUpdatedTop = DataManager.instance.leaderboardData.lastTimeUpdateTop;
						txtLastTimeUpdated.text = "Updated at: " + string.Format("{0:00}/{1:00}/{2} - {3:00}:{4:00}", _lastTimeUpdatedTop.Day, _lastTimeUpdatedTop.Month, _lastTimeUpdatedTop.Year, _lastTimeUpdatedTop.Hour, _lastTimeUpdatedTop.Minute);
						if(DataManager.instance.leaderboardData.topGold.Count > 0){
							CreatePanels();
						}else{
							txtEmpty.gameObject.SetActive(true);
						}
					}
				}
			}else{
				if(HomeManager.instance != null){
					if(currentState == State.Show){
						if(DataManager.instance.leaderboardData.topGold.Count > 0){
							CreatePanels();
						}else{
							txtEmpty.gameObject.SetActive(true);
						}
					}
				}
			}
		});
	}

	public override void Show ()
	{
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
			if(canLoadDataFromSv){
				LoadDataFromServer();
			}
		});

		if(tweenMainContainer != null){
			LeanTween.cancel(tweenMainContainer.uniqueId);
			tweenMainContainer = null;
		}
		Vector3 _pos = Vector3.zero;
		_pos.x = 250f;
		mainContainer.localPosition = _pos;
		tweenMainContainer = LeanTween.moveLocalX(mainContainer.gameObject, 0f, timeShowScreen).setEase(LeanTweenType.easeOutBack).setOnComplete(()=>{
			tweenMainContainer = null;
		});
	}

	public override void Hide ()
	{	
		if(currentState == State.Hide){
			return;
		}
		
		if (onPressBack != null) {
			CoreGameManager.instance.RemoveCurrentCallbackPressBackKey (onPressBack);
			onPressBack = null;
		}

		currentState = State.Hide;
		myCanvasGroup.blocksRaycasts = false;
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_TogglePanel);

		if(tweenCanvasGroup != null){
			LeanTween.cancel(tweenCanvasGroup.uniqueId);
			tweenCanvasGroup = null;
		}
		tweenCanvasGroup = LeanTween.alphaCanvas(myCanvasGroup, 0f, timeHideScreen).setOnComplete(()=>{
			optionInfoPoolManager.ClearAllObjectsNow();
			tweenCanvasGroup = null;
			panelLoading.gameObject.SetActive(false);
			txtEmpty.gameObject.SetActive(false);
			base.Hide();
		}).setEase(LeanTweenType.easeInBack);
		
		if(tweenMainContainer != null){
			LeanTween.cancel(tweenMainContainer.uniqueId);
			tweenMainContainer = null;
		}
		tweenMainContainer = LeanTween.moveLocalX(mainContainer.gameObject, -250f, timeHideScreen).setEase(LeanTweenType.easeInBack).setOnComplete(()=>{
			tweenMainContainer = null;
		});
	}
	#endregion

	#region On Button Clicked
	public void OnButtonBackClicked(){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);

		if (onPressBack != null) {
			CoreGameManager.instance.RemoveCurrentCallbackPressBackKey (onPressBack);
			onPressBack ();
			onPressBack = null;
		}
	}
	#endregion
}
