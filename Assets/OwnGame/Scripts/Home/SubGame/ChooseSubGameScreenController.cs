using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseSubGameScreenController : UIHomeScreenController {

	public override UIType myType{
		get{ 
			return UIType.SubGame;
		}
	}

	public override bool isSubScreen{
		get{ 
			return true;
		}
	}

	public static ChooseSubGameScreenController instance{
		get{
			return ins;
		}
	}
	private static ChooseSubGameScreenController ins;

	public enum MyState{
		ChooseGame, ChooseServer
	}
	MyState myCurrentState;
	[SerializeField] Canvas myCanvas;
	public RectTransform myRectTransform;
	[SerializeField] SubGame_ChooseGame_Controller panelChooseGame; 
	public SubGame_ListServer_Controller panelListServer; 

	MySimplePanelController currentPanel;

	MiniGameDetail currentSubGameDetail;

	IEnumerator actionLoginFbAgain;

	bool canTouch;

	void Awake(){
		if (ins != null && ins != this) { 
			Destroy(this.gameObject); 
			return;
		}
		ins = this;
		DontDestroyOnLoad (this.gameObject);

		base.Hide();
		panelChooseGame.ResetData();
		panelListServer.ResetData();
	}

	public override void ResetData(){
		currentPanel = null;
		currentSubGameDetail = null;
	}

	#region Init / Show / Hide
	public override void InitData(){
		myCurrentState = MyState.ChooseGame;
		currentPanel = panelChooseGame;
		currentPanel.InitData();

		onPressBack = () => {
			if(canTouch){
				canTouch = false;
				if(CoreGameManager.instance.currentSceneManager.mySceneType == IMySceneManager.Type.Home){
					HomeManager.instance.ChangeScreen (myLastType);
				}else{
					Hide();
				}
			}
		};
		CoreGameManager.instance.RegisterNewCallbackPressBackKey (onPressBack);
	}

	public override void Show ()
	{
		if(myCanvas.worldCamera == null && CoreGameManager.instance.currentSceneManager != null){
			myCanvas.worldCamera = CoreGameManager.instance.currentSceneManager.cameraForConsumableScreen.mainCamera;
		}else{
			myCanvas.worldCamera = Camera.main;
		}
		base.Show();
		canTouch = false;
		StartCoroutine(DoActionShow());
	}
	IEnumerator DoActionShow(){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_TogglePanel);
		yield return currentPanel.Show();
		canTouch = true;
	}

	public override void Hide ()
	{	
		currentState = State.Hide;
		if (onPressBack != null) {
			CoreGameManager.instance.RemoveCurrentCallbackPressBackKey (onPressBack);
			onPressBack = null;
		}
		StartCoroutine(DoActionHide());
	}
	
	IEnumerator DoActionHide(){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_TogglePanel);
		if(actionLoginFbAgain != null){
			StopCoroutine(actionLoginFbAgain);
			actionLoginFbAgain = null;
		}
		if(currentPanel != null){
			yield return currentPanel.Hide();
		}
		base.Hide();
		ResetData();
	}

	public void ForcedHide(){
		currentState = State.Hide;
		if (onPressBack != null) {
			CoreGameManager.instance.RemoveCurrentCallbackPressBackKey (onPressBack);
			onPressBack = null;
		}
		base.Hide();
		if(currentPanel != null){
			currentPanel.Hide();
		}
		ResetData();
	}

	#endregion

	#region On Button Clicked
	public void OnButtonBackClicked(){
		if(!canTouch){
			return;
		}
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
		if (onPressBack != null) {
			CoreGameManager.instance.RemoveCurrentCallbackPressBackKey (onPressBack);
			onPressBack ();
			onPressBack = null;
		}
	}

	public void OnChooseGame(IMiniGameInfo _gameInfo){
		if(!canTouch){
			return;
		}
		if(myCurrentState != MyState.ChooseGame){
			return;
		}
		#if TEST
		Debug.Log("Chọn game: " + _gameInfo.gameType.ToString());
		#endif
		if(_gameInfo.gameType != IMiniGameInfo.Type.DragonTigerCasino
			&& _gameInfo.gameType != IMiniGameInfo.Type.Koprok){
			#if TEST
			Debug.LogError("Game chưa setup");
			#endif
			PopupManager.Instance.CreateToast (MyLocalize.GetString("Global/CommingSoon"));
			return;
		}
		if(DataManager.instance.miniGameData.currentSubGameDetail != null){
			if(DataManager.instance.miniGameData.currentSubGameDetail.gameType == _gameInfo.gameType){
				return;
			}
		}
		currentSubGameDetail = DataManager.instance.miniGameData.GetMiniGameDetail(_gameInfo.gameType);
		if(currentSubGameDetail == null){
			#if TEST
			Debug.LogError("currentGameDetail is null");
			#endif
			return;
		}
		if(NetworkGlobal.instance.instanceRealTime == null || !NetworkGlobal.instance.instanceRealTime.isRunning){
			StartCoroutine(DoActionChooseServer());
		}else{
			if(CoreGameManager.instance.currentSceneManager.mySceneType == IMySceneManager.Type.SubGamePlayScene){
				StartCoroutine(DoActionOpenAnotherGamePlay());
			}else{
				StartCoroutine(DoActionOpenPopupGamePlay());
			}
		}
	}

	public void OnChooseServer(SubServerDetail _serverDetail){
		if(!canTouch){
			return;
		}
		if(currentSubGameDetail == null){
			#if TEST
			Debug.LogError("currentGameDetail is null");
			#endif
			return;
		}
		if(myCurrentState != MyState.ChooseServer){
			return;
		}
		#if TEST
		Debug.Log("Chọn server: " + _serverDetail.subServerName);
		#endif
		currentSubGameDetail.currentServerDetail = _serverDetail;

		// SceneLoaderManager.instance.LoadScene(MyConstant.SCENE_SUBGAMEPLAY);

		LoadingCanvasController.instance.Show(-1, false, null, ()=>{
			NetworkGlobal.instance.StopRealTime();
		});
		SetProcessPlayNowFromHomeScene();
		NetworkGlobal.instance.RunRealTime (currentSubGameDetail.currentServerDetail, OnCreateConnectionError, OnPlayNowCreateConnectionSuccess, OnDisconnect, OnServerFull);
	}
	#endregion

	public IEnumerator DoActionChooseServer(){
		canTouch = false;
		bool _initFinished = false;
		panelListServer.InitData(()=>{
			_initFinished = true;
		});
		yield return new WaitUntil(()=>_initFinished);
		yield return currentPanel.Hide();
		myCurrentState = MyState.ChooseServer;
		currentPanel = panelListServer;
		yield return currentPanel.Show();
		canTouch = true;
	}

	/// <summary>
	/// DoActionChooseGame : Hàm mở màn hình sub gameplay dưới dạng popup
	/// </summary>
	/// <returns></returns>
	public IEnumerator DoActionOpenPopupGamePlay(){
		// LoadingCanvasController.instance.Show();
		// var _object = _gameInfo.gameManagerPrefab.LoadAsync();
		// yield return _object.IsDone;
		canTouch = false;
		currentSubGameDetail.currentServerDetail = DataManager.instance.miniGameData.currentMiniGameDetail.currentServerDetail;
		DataManager.instance.miniGameData.currentSubGameDetail = currentSubGameDetail;

		CoreGameManager.instance.currentSubGamePlay = Instantiate ((GameObject) DataManager.instance.miniGameData.currentSubGameDetail.myInfo.gameManagerPrefab.Load()).GetComponent<ISubGamePlayManager>();
		yield return null;
		CoreGameManager.instance.currentSubGamePlay.InitData(false, true);
		CoreGameManager.instance.currentSubGamePlay.Show();
		Hide();
	}

	public IEnumerator DoActionOpenAnotherGamePlay(){
		if(CoreGameManager.instance.currentSubGamePlay == null){
			Debug.LogError("currentSubGamePlay is null");
			yield break;
		}
		// LoadingCanvasController.instance.Show();
		// var _object = _gameInfo.gameManagerPrefab.LoadAsync();
		// yield return _object.IsDone;
		canTouch = false;
		currentSubGameDetail.currentServerDetail = DataManager.instance.miniGameData.currentSubGameDetail.currentServerDetail;
		CoreGameManager.instance.currentSubGamePlay.LeftGameAndHide();
		DataManager.instance.miniGameData.currentSubGameDetail = currentSubGameDetail;
		CoreGameManager.instance.currentSubGamePlay = Instantiate ((GameObject) DataManager.instance.miniGameData.currentSubGameDetail.myInfo.gameManagerPrefab.Load()).GetComponent<ISubGamePlayManager>();
		yield return null;
		CoreGameManager.instance.currentSubGamePlay.InitData(true, true);
		CoreGameManager.instance.currentSubGamePlay.Show();
		Hide();
	}

	#region Play Now In Home Scene
	void SetProcessPlayNowFromHomeScene(){
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_TABLE_JOIN_TO_MINIGAME_STATE, (MessageReceiving _mess)=>{
			LoadingCanvasController.instance.Hide();
			sbyte _result = _mess.readByte();
			if(_result > 0){
				NetworkGlobal.instance.instanceRealTime.PauseReceiveMessage();

				if(!DataManager.instance.hadRatingBefore){
					DataManager.instance.remindRating_GoldUserCatched = DataManager.instance.userData.gold;
				}

				SetUserDataAgainFromServer(_mess);
				
				DataManager.instance.miniGameData.currentSubGameDetail = currentSubGameDetail;

				SceneLoaderManager.instance.LoadScene(MyConstant.SCENE_SUBGAMEPLAY);
			}else{
				SetDataWhenReceivingDataError(_result);
				// Debug.LogError("aaaaaaaaaa");
				if (NetworkGlobal.instance.instanceRealTime != null) {
					NetworkGlobal.instance.instanceRealTime.onDisconnect = null;
					NetworkGlobal.instance.instanceRealTime.closeConnection();
				}
			}
		});
	}

	void SetUserDataAgainFromServer(MessageReceiving _mess){
		DataManager.instance.userData.GetMoreUserData(_mess);
	}

	void SetDataWhenReceivingDataError(sbyte _result){
		// private static final byte JOIN_TABLE_SUCCESS					=1;
		// private static final byte PLAYER_INVALIDATE_DATA				=-1;
		// private static final byte PLAYER_NOT_FOUND						=-2;
		// private static final byte PLAYER_USERNAME_NOT_FOUND				=-3;
		// private static final byte PLAYER_WRONG_PASSWORD					=-4;
		// private static final byte PLAYER_ERROR_GOLD						=-5;
		// private static final byte PLAYER_ERROR_DATABASE_MAIN_SERVER		=-6;
		
		// private static final byte ROOM_NOT_FOUND						=-10;
		// private static final byte ROOM_FULL_CREATE_PASSWORD_FAIL		=-11;//Chỉ dùng cho createPassword
		// private static final byte ROOM_PLAYNOW_FAIL						=-12;//Chỉ dùng cho playnow
		
		// private static final byte TABLE_NOT_FOUND						=-20;//Chỉ dùng cho joinTable
		// private static final byte TABLE_WRONG_PASSWORD					=-21;//Chỉ dùng cho joinTable
		// private static final byte TABLE_LAST_SESSION_EXIST				=-22;//Chỉ dùng cho joinTable
		// private static final byte TABLE_FULL_POSITION					=-23;//Chỉ dùng cho joinTable

		switch (_result){
		case -1: // PLAYER_INVALIDATE_DATA
			#if TEST
			Debug.LogError("Không tìm được playerinfo");
			#endif
			break;
		case -2: // PLAYER_NOT_FOUND
			#if TEST
			Debug.LogError("PLAYER_NOT_FOUND");
			#endif
			break;
		case -3: // PLAYER_USERNAME_NOT_FOUND
			#if TEST
			Debug.LogError("PLAYER_USERNAME_NOT_FOUND");
			#endif
			break;
		case -4: // PLAYER_WRONG_PASSWORD
			#if TEST
			Debug.LogError("PLAYER_WRONG_PASSWORD");
			#endif
			break;
		case -5: // NotEnoughMoney
			#if TEST
			Debug.LogError("Không đủ tiền cược và có người đang trong đó");
			#endif
			break;
		case -6: // PLAYER_ERROR_DATABASE_MAIN_SERVER
			#if TEST
			Debug.LogError("PLAYER_ERROR_DATABASE_MAIN_SERVER");
			#endif
			break;
		case -10: // ROOM_NOT_FOUND
			#if TEST
			Debug.LogError("ROOM_NOT_FOUND");
			#endif
			break;
		case -11: // ROOM_CREATE_PASSWORD_FAIL Chỉ dùng cho createPassword
			#if TEST
			Debug.LogError("ROOM_CREATE_PASSWORD_FAIL");
			#endif
			break;
		case -12: // ROOM_PLAYNOW_FAIL Chỉ dùng cho playnow
			#if TEST
			Debug.LogError("ROOM_PLAYNOW_FAIL");
			#endif
			break;
		case -20: // TABLE_NOT_FOUND Chỉ dùng cho joinTable
			#if TEST
			Debug.LogError("TABLE_NOT_FOUND");
			#endif
			break;
		case -21: // TABLE_WRONG_PASSWORD Chỉ dùng cho joinTable
			#if TEST
			Debug.LogError("TABLE_WRONG_PASSWORD");
			#endif
			break;
		case -22: // TABLE_LAST_SESSION_EXIST Chỉ dùng cho joinTable
			#if TEST
			Debug.LogError("TABLE_LAST_SESSION_EXIST");
			#endif
			break;
		case -23: // TABLE_FULL_POSITION Chỉ dùng cho joinTable
			#if TEST
			Debug.LogError("TABLE_FULL_POSITION");
			#endif
			break;
		default:
			#if TEST
			Debug.LogError("ChosseTableError: " + _result);
			#endif
			break;
		}

		LoadingCanvasController.instance.Hide();
		PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
				, MyLocalize.GetString("ChooseTable/JoinTableFailed")
				, _result.ToString()
				, MyLocalize.GetString(MyLocalize.kOk));
	} 

	void OnCreateConnectionError(int _error){
		LoadingCanvasController.instance.Hide();
		PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
			, MyLocalize.GetString(MyLocalize.kConnectionError)
			, _error.ToString()
			, MyLocalize.GetString(MyLocalize.kOk));
	}
	void OnPlayNowCreateConnectionSuccess(){
		if(currentSubGameDetail == null){
			#if TEST
			Debug.LogError("currentMiniGameDetail is null");
			#endif
			return;
		}
		if(DataManager.instance.userData.databaseId == UserData.DatabaseType.DATABASEID_FACEBOOK
			&& !FacebookAPI.IsLoggedIn()){
			#if TEST
			Debug.LogError(">>> Chưa Login FB mà");
			#endif
			if (actionLoginFbAgain == null) {
				actionLoginFbAgain = FacebookAPI.DoActionLoginFb (null, ()=>{
					actionLoginFbAgain = null;
				});
				StartCoroutine (actionLoginFbAgain);
			}
			return;
		}
		GlobalRealTimeSendingAPI.SendMessagePlayNowMiniGame(currentSubGameDetail.myInfo.gameId);
	}
	void OnServerFull(){
		// #if TEST
		// Debug.LogError("Server đầy");
		// #endif
		LoadingCanvasController.instance.Hide();
		PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kWarning)
			, MyLocalize.GetString(MyLocalize.kServerFull)
			, string.Empty
			, MyLocalize.GetString(MyLocalize.kOk));
	}

	void OnDisconnect(){
		LoadingCanvasController.instance.Hide();
		if(HomeManager.instance != null){
			return;
		}
		PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
			, MyLocalize.GetString(MyLocalize.kConnectionError)
			, string.Empty
			, MyLocalize.GetString(MyLocalize.kOk)
			, () =>
			{
				if(CoreGameManager.instance.currentSceneManager.mySceneType != IMySceneManager.Type.Home){
					CoreGameManager.instance.SetUpOutRoomFromSubGamePlayAndBackToChooseGameScreen();
				}
			});
	}
	#endregion
}
