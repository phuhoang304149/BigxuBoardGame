using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChooseTableScreenController : UIHomeScreenController {

	public override UIType myType{
		get{ 
			return UIType.ChooseTable;
		}
	}

	[SerializeField] Image imgBanner;
	[SerializeField] Transform btnCreateTable;
	[SerializeField] Transform fieldFindTable;
	[SerializeField] BottomBar_PanelUserInfo_Controller panelUserInfo;
	[SerializeField] ChooseTable_PanelListTable_Controller panelListTable;
	[SerializeField] ChooseTable_PanelListServer_Controller panelListServer;
	[SerializeField] MyArrowFocusController arrowFocusBtnGetGold;
	[SerializeField] GameObject panelLoadingListTable;
	[SerializeField] ParticleSystem iconWarningConfigInfo;

	[Header("Setting")]
	public int numTableDefault;
	public float timeDelayToPressSelectServerOrTable; // tính bằng giây

	public System.DateTime timeCanPressSelectServerOrTable;
	IEnumerator actionLoginFbAgain;
	float posX_ImgBanner_Saved;
	bool firstInit;
	
	#region Init / Show / Hide
	public override void InitData ()
	{	
		NetworkGlobal.instance.listProcess = null;

		panelLoadingListTable.SetActive(false);

		timeCanPressSelectServerOrTable = System.DateTime.Now;

		if (DataManager.instance.miniGameData.currentMiniGameDetail == null) {
			Debug.LogError ("currentMiniGameDetail is NULL");
		} else {
			if(DataManager.instance.miniGameData.currentMiniGameDetail.currentServerDetail != null){
				bool _isNull = true;
				for(int i = 0; i < DataManager.instance.subServerData.listSubServerDetail.Count; i++){
					if(DataManager.instance.miniGameData.currentMiniGameDetail.currentServerDetail.IsEqual(DataManager.instance.subServerData.listSubServerDetail[i])){
						_isNull = false;
						break;
					}
				}
				if(_isNull){
					DataManager.instance.miniGameData.currentMiniGameDetail.currentServerDetail = null;
				}
			}

			if(DataManager.instance.miniGameData.currentMiniGameDetail.currentServerDetail == null){
				if(DataManager.instance.miniGameData.currentMiniGameDetail.listServerDetail_Normal.Count > 0){
					DataManager.instance.miniGameData.currentMiniGameDetail.currentServerDetail = DataManager.instance.miniGameData.currentMiniGameDetail.listServerDetail_Normal[0];
				}else if(DataManager.instance.miniGameData.currentMiniGameDetail.listServerDetail_Error.Count > 0){
					DataManager.instance.miniGameData.currentMiniGameDetail.currentServerDetail = DataManager.instance.miniGameData.currentMiniGameDetail.listServerDetail_Error[0];
				}else{
					#if TEST
					Debug.LogError ("listServerDetail is NULL");
					#endif
				}
			}
			imgBanner.sprite = DataManager.instance.miniGameData.currentMiniGameDetail.myInfo.gameBanner;
		}

		// --- show dữ liệu từ preference trước --- //
		panelListServer.InitData(this);
		if(DataManager.instance.miniGameData.currentMiniGameDetail.tableData.listTableDetail.Count > 0){
			panelListTable.InitData(this);
		}else{
			panelListTable.InitData(this, false);
		}
		// ---------------------------------------- //

		panelUserInfo.InitData();

		if(!firstInit){
			posX_ImgBanner_Saved = imgBanner.transform.position.x;
			if(posX_ImgBanner_Saved > 0f){
				posX_ImgBanner_Saved = 0f;
			}
		}

		if(DataManager.instance.miniGameData.currentMiniGameDetail.myInfo.gameType == IMiniGameInfo.Type.AnimalRacing){
			btnCreateTable.gameObject.SetActive(false);
			fieldFindTable.gameObject.SetActive(false);

			Vector3 _pos = imgBanner.transform.position;
			_pos.x = 0f;
			imgBanner.transform.position = _pos;
		}else{
			btnCreateTable.gameObject.SetActive(true);
			fieldFindTable.gameObject.SetActive(true);
			if(imgBanner.transform.position.x > 0f){
				Vector3 _pos = imgBanner.transform.position;
				_pos.x = 0f;
				imgBanner.transform.position = _pos;
			}else{
				Vector3 _pos = imgBanner.transform.position;
				_pos.x = posX_ImgBanner_Saved;
				imgBanner.transform.position = _pos;
			}
		}

		if(DataManager.instance.userData.gold < 2000){
			arrowFocusBtnGetGold.Show();
		}else{
			arrowFocusBtnGetGold.Hide();
		}
		if(DataManager.instance.userData.databaseId == UserData.DatabaseType.DATABASEID_BIGXU
			&& string.IsNullOrEmpty(DataManager.instance.userData.emailShow)){
			iconWarningConfigInfo.gameObject.SetActive(true);
			iconWarningConfigInfo.Play();
		}else{
			iconWarningConfigInfo.gameObject.SetActive(false);
		}

		HomeManager.instance.myCallbackManager.onLoadDataGoldGemFinished += RefreshPanelMyCashInfo;
		HomeManager.instance.myCallbackManager.onLoadEmailInfoFinished += RefreshData;

		if(HomeManager.showAnnouncement){
			HomeManager.showAnnouncement = false;
			HomeManager.instance.ShowAnnouncement();
		}
		if(HomeManager.getGoldAndGemInfoAgain){
			HomeManager.getGoldAndGemInfoAgain = false;
			HomeManager.instance.LoadDataGoldGemFromServer();
		}
		if(HomeManager.getEmailInfoAgain){
			HomeManager.getEmailInfoAgain = false;
			HomeManager.instance.LoadEmailInfoFromServer();
		}
		
		onPressBack = () => {
			HomeManager.instance.ChangeScreen (UIHomeScreenController.UIType.ChooseGame);
		};
		CoreGameManager.instance.RegisterNewCallbackPressBackKey (onPressBack);

		firstInit = true;
	}

	void RefreshPanelMyCashInfo(){
		panelUserInfo.RefreshPanelMyCashInfo();
		if(DataManager.instance.userData.gold < 2000){
			arrowFocusBtnGetGold.Show();
		}else{
			arrowFocusBtnGetGold.Hide();
		}
    }

	public override void LateInitData(){
		StartCoroutine(DoActionLateInitData());
	}

	IEnumerator DoActionLateInitData(){
		if(!panelListServer.isInstalled){
			while(!panelListServer.isInstalled){
				panelListServer.InitData(this);
				yield return Yielders.Get(0.1f);
			}
			if(onPressBack == null){
				onPressBack = () => {
					HomeManager.instance.ChangeScreen (UIHomeScreenController.UIType.ChooseGame);
				};
			}
		}

		bool _isSuccess = false;
		bool _isFinished = false;
		
		if(DataManager.instance.miniGameData.currentMiniGameDetail.currentServerDetail != null){
			bool _isNull = true;
			for(int i = 0; i < DataManager.instance.subServerData.listSubServerDetail.Count; i++){
				if(DataManager.instance.miniGameData.currentMiniGameDetail.currentServerDetail.IsEqual(DataManager.instance.subServerData.listSubServerDetail[i])){
					_isNull = false;
					break;
				}
			}
			if(_isNull){
				DataManager.instance.miniGameData.currentMiniGameDetail.currentServerDetail = null;
			}
		}

		List<SubServerDetail> _listSubServerDetail = new List<SubServerDetail>();
		for(int i = 0; i < DataManager.instance.miniGameData.currentMiniGameDetail.listServerDetail_Normal.Count; i ++){
			if(DataManager.instance.miniGameData.currentMiniGameDetail.currentServerDetail != null){
				if(!DataManager.instance.miniGameData.currentMiniGameDetail.listServerDetail_Normal[i].IsEqual(DataManager.instance.miniGameData.currentMiniGameDetail.currentServerDetail)){
					_listSubServerDetail.Add(DataManager.instance.miniGameData.currentMiniGameDetail.listServerDetail_Normal[i]);
				}
			}else{
				_listSubServerDetail.Add(DataManager.instance.miniGameData.currentMiniGameDetail.listServerDetail_Normal[i]);
			}
		}
		for(int i = 0; i < DataManager.instance.miniGameData.currentMiniGameDetail.listServerDetail_Error.Count; i ++){
			if(DataManager.instance.miniGameData.currentMiniGameDetail.currentServerDetail != null){
				if(!DataManager.instance.miniGameData.currentMiniGameDetail.listServerDetail_Error[i].IsEqual(DataManager.instance.miniGameData.currentMiniGameDetail.currentServerDetail)){
					_listSubServerDetail.Add(DataManager.instance.miniGameData.currentMiniGameDetail.listServerDetail_Error[i]);
				}
			}else{
				_listSubServerDetail.Add(DataManager.instance.miniGameData.currentMiniGameDetail.listServerDetail_Error[i]);
			}
		}

		if(DataManager.instance.miniGameData.currentMiniGameDetail.currentServerDetail != null){
			LoadSomeTablesFromSever(DataManager.instance.miniGameData.currentMiniGameDetail.currentServerDetail, ()=>{
				_isFinished = true;
				_isSuccess = true;
			}, (_errorCode)=>{
				_isFinished = true;
			});
			yield return new WaitUntil(()=>_isFinished);
			if(_isSuccess){
				if(currentState == State.Show){
					panelListServer.FocusRoom(DataManager.instance.miniGameData.currentMiniGameDetail.currentServerDetail, true);
					panelListTable.InitDataAgain();
				}
				yield break;
			}
		}

		for(int i = 0; i < _listSubServerDetail.Count; i ++){
			_isFinished = false;
			_isSuccess = false;
			LoadSomeTablesFromSever(_listSubServerDetail[i], ()=>{
				_isFinished = true;
				_isSuccess = true;
			}, (_errorCode)=>{
				_isFinished = true;
			});
			yield return new WaitUntil(()=>_isFinished);
			if(_isSuccess){
				DataManager.instance.miniGameData.currentMiniGameDetail.currentServerDetail = _listSubServerDetail[i];
				if(currentState == State.Show){
					panelListServer.FocusRoom(DataManager.instance.miniGameData.currentMiniGameDetail.currentServerDetail, true);
					panelListTable.InitDataAgain();
				}
				break;
			}
		}
	}

	public override void Show ()
	{
		panelUserInfo.Show ();
		base.Show ();
    }

	public override void RefreshData ()
	{
		panelUserInfo.RefreshData ();
		if(DataManager.instance.userData.gold < 2000){
			arrowFocusBtnGetGold.Show();
		}else{
			arrowFocusBtnGetGold.Hide();
		}
		if(DataManager.instance.userData.databaseId == UserData.DatabaseType.DATABASEID_BIGXU
			&& string.IsNullOrEmpty(DataManager.instance.userData.emailShow)){
			iconWarningConfigInfo.gameObject.SetActive(true);
			iconWarningConfigInfo.Play();
		}else{
			iconWarningConfigInfo.gameObject.SetActive(false);
		}
		base.RefreshData ();
	}

	public override void Hide ()
	{
		if (onPressBack != null) {
			CoreGameManager.instance.RemoveCurrentCallbackPressBackKey (onPressBack);
		}
		if(HomeManager.instance.myCallbackManager.onLoadDataGoldGemFinished != null){
			HomeManager.instance.myCallbackManager.onLoadDataGoldGemFinished -= RefreshPanelMyCashInfo;
		}
		if(HomeManager.instance.myCallbackManager.onLoadEmailInfoFinished != null){
			HomeManager.instance.myCallbackManager.onLoadEmailInfoFinished -= RefreshData;
		}
		base.Hide ();
		arrowFocusBtnGetGold.Hide();
		panelUserInfo.Hide ();
		panelListServer.SelfDestruction();
		panelListTable.SelfDestruction();
	}

	public void LoadSomeTablesFromSever(SubServerDetail _serverDetail, System.Action _onSuccess = null, System.Action<int> _onError = null){
		short _totalTables = (short) numTableDefault;
		for(int i = 0; i < _serverDetail.listRoomDetail.Count; i ++){
            if(DataManager.instance.miniGameData.currentMiniGameDetail.myInfo.gameId == _serverDetail.listRoomDetail[i].gameId){
                if(numTableDefault > _serverDetail.listRoomDetail[i].numberTable){
					_totalTables = _serverDetail.listRoomDetail[i].numberTable;
				}
				break;
            }
        }
		panelLoadingListTable.SetActive(true);
		OneHitAPI.GetListTableByGameID (DataManager.instance.miniGameData.currentMiniGameDetail.myInfo.gameId, 0, _totalTables, _serverDetail, (_messageReceiving, _error) => {
			panelLoadingListTable.SetActive(false);
			SetUpTableData(false, _serverDetail, _messageReceiving, _error, _onSuccess, _onError);
		});
	}

	public void LoadAllTablesFromSever(SubServerDetail _serverDetail, System.Action _onSuccess = null, System.Action<int> _onError = null){
		short _totalTables = (short) numTableDefault;
		for(int i = 0; i < _serverDetail.listRoomDetail.Count; i ++){
            if(DataManager.instance.miniGameData.currentMiniGameDetail.myInfo.gameId == _serverDetail.listRoomDetail[i].gameId){
                _totalTables = _serverDetail.listRoomDetail[i].numberTable;
				break;
            }
        }

		panelLoadingListTable.SetActive(true);
		OneHitAPI.GetListTableByGameID (DataManager.instance.miniGameData.currentMiniGameDetail.myInfo.gameId, 0, _totalTables, _serverDetail, (_messageReceiving, _error) => {
			panelLoadingListTable.SetActive(false);
			SetUpTableData(true, _serverDetail, _messageReceiving, _error, _onSuccess, _onError);
		});
	}

	void SetUpTableData(bool _isGetAll, SubServerDetail _serverDetail, MessageReceiving _messageReceiving, int _error, System.Action _onSuccess = null, System.Action<int> _onError = null){
		if(_messageReceiving != null){
			bool _checkCase = _messageReceiving.readBoolean();
			if(_checkCase){
				long _versionRoom = _messageReceiving.readLong();
				RoomDetail _roomDetail = null;
				for(int i = 0; i < _serverDetail.listRoomDetail.Count; i++){
					if(_serverDetail.listRoomDetail[i].gameId == DataManager.instance.miniGameData.currentMiniGameDetail.myInfo.gameId){
						_roomDetail = _serverDetail.listRoomDetail[i];
						break;
					}
				}
				if(_roomDetail == null){
					#if TEST
					Debug.LogError("_roomDetail is null: " + DataManager.instance.miniGameData.currentMiniGameDetail.myInfo.gameType.ToString());
					#endif
				}else{
					_roomDetail.versionRoom = _versionRoom;
				}
				// if(_roomDetail.myOriginalDetail != null){
				// 	_roomDetail.myOriginalDetail.versionServer = _versionServer;
				// }

				TableData _tableData = new TableData();
				string _gameNameRecieve = _messageReceiving.readString();
				short _maxNumberTable = _messageReceiving.readShort();
				_tableData.numberPlaying = _messageReceiving.readInt(); //số người đang chơi
				_tableData.maxViewer = _messageReceiving.readByte();
				_tableData.maxPlayer = _messageReceiving.readByte();
				short _numberTableGet = _messageReceiving.readShort();
				// GAMEID_UNO|59|0|8|4|20
				// Debug.Log(_gameNameRecieve + "|" + _maxNumberTable + "|" + _tableData.numberPlaying + "|" + _tableData.maxViewer + "|" + _tableData.maxPlayer + "|" + _numberTableGet);
				List<TableDetail> _listTableDetail = new List<TableDetail>();
				for(int i = 0; i < _numberTableGet; i ++){
					TableDetail _tableDetail = new TableDetail();
					_tableDetail.tableId = _messageReceiving.readShort();
					_tableDetail.isLockByPass = _messageReceiving.readBoolean();
					_tableDetail.status = _messageReceiving.readByte();
					_tableDetail.bet = _messageReceiving.readLong();
					_tableDetail.countViewer = _messageReceiving.readByte();
					_tableDetail.countPlaying = _messageReceiving.readByte();
					_listTableDetail.Add(_tableDetail);
				}

				if(DataManager.instance.miniGameData.currentMiniGameDetail.myInfo.versionFeature < _versionRoom){
					_serverDetail.beingError = false;
					if(currentState == State.Show){
						if(_onError != null){
							_onError(-98); // RoomOutOfDate
						}
					}
				}else{
					if(_isGetAll){
						List<TableDetail> _tableDetailCollection = new List<TableDetail>();
						for(int i = 0; i < _listTableDetail.Count; i ++){
							if(_tableData.CanTableBeEnable(_listTableDetail[i])){
								_tableDetailCollection.Add(_listTableDetail[i]);
								if(_tableDetailCollection.Count >= numTableDefault){
									break;
								}
							}
						}
						_tableData.listTableDetail = _tableDetailCollection;
					}else{
						_tableData.listTableDetail = _listTableDetail;
					}
					DataManager.instance.miniGameData.currentMiniGameDetail.tableData = _tableData;
					_serverDetail.beingError = false;
					if(currentState == State.Show){
						panelListTable.InitDataAgain ();
						if(_onSuccess != null){
							_onSuccess();
						}
					}
				}
			}else{
				#if TEST
				Debug.LogError("Lỗi sever trả về rỗng");
				#endif
				_serverDetail.beingError = true;
				_serverDetail.countConnectionError ++;
				if(currentState == State.Show){
					if(_onError != null){
						_onError(-99); // RoomIsNotAvailable
					}
				}
			}
		}else{
			#if TEST
			Debug.LogError("Room is not available");
			#endif
			_serverDetail.beingError = true;
			_serverDetail.countConnectionError ++;
			if(currentState == State.Show){
				if(_onError != null){
					_onError(_error);
				}
			}
		}
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

	public void OnButtonGetGoldClicked(){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
 		HomeManager.instance.ChangeScreen (UIType.GetGold);
	}

	public void OnButtonPlayNowClicked(){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);

		SubServerDetail _serverDetail = DataManager.instance.miniGameData.currentMiniGameDetail.currentServerDetail;

		if(_serverDetail == null){
			#if TEST
			Debug.LogError(">>> currentServerDetail is NULL");
			#endif
			return;
		}

		RoomDetail _roomDetail = null;
		for(int i = 0; i < _serverDetail.listRoomDetail.Count; i++){
			if(_serverDetail.listRoomDetail[i].gameId == DataManager.instance.miniGameData.currentMiniGameDetail.myInfo.gameId){
				_roomDetail = _serverDetail.listRoomDetail[i];
				break;
			}
		}
		if(_roomDetail == null){
			#if TEST
			Debug.LogError("_roomDetail is null: " + DataManager.instance.miniGameData.currentMiniGameDetail.myInfo.gameType.ToString());
			#endif
			return;
		}
		
		if(_roomDetail.versionRoom > DataManager.instance.miniGameData.currentMiniGameDetail.myInfo.versionFeature){
			#if TEST
			Debug.Log("<color=green> RoomOutOfDate: " + DataManager.instance.miniGameData.currentMiniGameDetail.myInfo.versionFeature + " - " + _roomDetail.versionRoom + " </color>");
			#endif
			PopupManager.Instance.CreatePopupDialog(MyLocalize.GetString(MyLocalize.kWarning)
				, MyLocalize.GetString("ChooseTable/RoomOutOfDate")
				, string.Empty
				, MyLocalize.GetString(MyLocalize.kUpdate)
				, MyLocalize.GetString(MyLocalize.kCancel)
				, () =>{
						//TODO : xử lý khi bấm nút update
					Application.OpenURL(MyConstant.linkApp);
				}, null);
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
		
		LoadingCanvasController.instance.Show(-1, false, null, ()=>{
			NetworkGlobal.instance.StopRealTime();
		});
		SetProcessPlayNow();
		NetworkGlobal.instance.RunRealTime (_serverDetail, OnCreateConnectionError, OnPlayNowCreateConnectionSuccess, null, OnServerFull);
	}

	public void OnChooseTable(TableDetail _tableDetail){
		DataManager.instance.miniGameData.currentMiniGameDetail.tableData.currentTableDetail = _tableDetail;
		
		SubServerDetail _serverDetail = DataManager.instance.miniGameData.currentMiniGameDetail.currentServerDetail;

		if(_serverDetail == null){
			#if TEST
			Debug.LogError(">>> currentServerDetail is NULL");
			#endif
			return;
		}

		RoomDetail _roomDetail = null;
		for(int i = 0; i < _serverDetail.listRoomDetail.Count; i++){
			if(_serverDetail.listRoomDetail[i].gameId == DataManager.instance.miniGameData.currentMiniGameDetail.myInfo.gameId){
				_roomDetail = _serverDetail.listRoomDetail[i];
				break;
			}
		}
		if(_roomDetail == null){
			#if TEST
			Debug.LogError("_roomDetail is null: " + DataManager.instance.miniGameData.currentMiniGameDetail.myInfo.gameType.ToString());
			#endif
			return;
		}
		
		if(_roomDetail.versionRoom > DataManager.instance.miniGameData.currentMiniGameDetail.myInfo.versionFeature){
			#if TEST
			Debug.Log("<color=green> RoomOutOfDate: " + DataManager.instance.miniGameData.currentMiniGameDetail.myInfo.versionFeature + " - " + _roomDetail.versionRoom + " </color>");
			#endif
			PopupManager.Instance.CreatePopupDialog(MyLocalize.GetString(MyLocalize.kWarning)
				, MyLocalize.GetString("ChooseTable/RoomOutOfDate")
				, string.Empty
				, MyLocalize.GetString(MyLocalize.kUpdate)
				, MyLocalize.GetString(MyLocalize.kCancel)
				, () =>{
						//TODO : xử lý khi bấm nút update
					Application.OpenURL(MyConstant.linkApp);
				}, null);
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

		LoadingCanvasController.instance.Show(-1, false, null, ()=>{
			NetworkGlobal.instance.StopRealTime();
		});
		SetProcessJoinToTable();
		NetworkGlobal.instance.RunRealTime (_serverDetail, OnCreateConnectionError, OnChooseTableCreateConnectionSuccess, null, OnServerFull);
	}

	public void OnButtonCreateTableClicked(){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);

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
		}

		PopupManager.Instance.CreatePopupCreateTable((_pass)=>{
			OnCreateTable(_pass);
		}, null);
	}

	void OnCreateTable(string _pass){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
		
		SubServerDetail _serverDetail = DataManager.instance.miniGameData.currentMiniGameDetail.currentServerDetail;

		if(_serverDetail == null){
			#if TEST
			Debug.LogError(">>> currentServerDetail is NULL");
			#endif
			return;
		}

		RoomDetail _roomDetail = null;
		for(int i = 0; i < _serverDetail.listRoomDetail.Count; i++){
			if(_serverDetail.listRoomDetail[i].gameId == DataManager.instance.miniGameData.currentMiniGameDetail.myInfo.gameId){
				_roomDetail = _serverDetail.listRoomDetail[i];
				break;
			}
		}
		if(_roomDetail == null){
			#if TEST
			Debug.LogError("_roomDetail is null: " + DataManager.instance.miniGameData.currentMiniGameDetail.myInfo.gameType.ToString());
			#endif
			return;
		}
		
		if(_roomDetail.versionRoom > DataManager.instance.miniGameData.currentMiniGameDetail.myInfo.versionFeature){
			#if TEST
			Debug.Log("<color=green> RoomOutOfDate: " + DataManager.instance.miniGameData.currentMiniGameDetail.myInfo.versionFeature + " - " + _roomDetail.versionRoom + " </color>");
			#endif
			PopupManager.Instance.CreatePopupDialog(MyLocalize.GetString(MyLocalize.kWarning)
				, MyLocalize.GetString("ChooseTable/RoomOutOfDate")
				, string.Empty
				, MyLocalize.GetString(MyLocalize.kUpdate)
				, MyLocalize.GetString(MyLocalize.kCancel)
				, () =>{
						//TODO : xử lý khi bấm nút update
					Application.OpenURL(MyConstant.linkApp);
				}, null);
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
		
		bool _havePass = false;
		if(!string.IsNullOrEmpty(_pass)){
			_havePass = true;
		}

		LoadingCanvasController.instance.Show(-1, false, null, ()=>{
			NetworkGlobal.instance.StopRealTime();
		});
		SetProcessCreatePrivateTable(_havePass);
		System.Action _onConnectionSuccess = ()=>{
			GlobalRealTimeSendingAPI.SendMessageCreatePassAndJoinTable(DataManager.instance.miniGameData.currentMiniGameDetail.myInfo.gameId
			, _pass);
		};
		NetworkGlobal.instance.RunRealTime (_serverDetail, OnCreateConnectionError, _onConnectionSuccess, null, OnServerFull);
	}

	public void OnButtonFindTableClicked(string _defaultId){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
		
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
		}
		
		PopupManager.Instance.CreatePopupJoinTable(_defaultId, (_id, _pass)=>{
			if(!string.IsNullOrEmpty(_id)){
				OnFindTableClicked(_id, _pass);
			}
		}, null);
	}

	void OnFindTableClicked(string _id, string _pass){
		SubServerDetail _serverDetail = DataManager.instance.miniGameData.currentMiniGameDetail.currentServerDetail;

		if(_serverDetail == null){
			#if TEST
			Debug.LogError(">>> currentServerDetail is NULL");
			#endif
			return;
		}

		RoomDetail _roomDetail = null;
		for(int i = 0; i < _serverDetail.listRoomDetail.Count; i++){
			if(_serverDetail.listRoomDetail[i].gameId == DataManager.instance.miniGameData.currentMiniGameDetail.myInfo.gameId){
				_roomDetail = _serverDetail.listRoomDetail[i];
				break;
			}
		}
		if(_roomDetail == null){
			#if TEST
			Debug.LogError("_roomDetail is null: " + DataManager.instance.miniGameData.currentMiniGameDetail.myInfo.gameType.ToString());
			#endif
			return;
		}
		
		if(_roomDetail.versionRoom > DataManager.instance.miniGameData.currentMiniGameDetail.myInfo.versionFeature){
			#if TEST
			Debug.Log("<color=green> RoomOutOfDate: " + DataManager.instance.miniGameData.currentMiniGameDetail.myInfo.versionFeature + " - " + _roomDetail.versionRoom + " </color>");
			#endif
			PopupManager.Instance.CreatePopupDialog(MyLocalize.GetString(MyLocalize.kWarning)
				, MyLocalize.GetString("ChooseTable/RoomOutOfDate")
				, string.Empty
				, MyLocalize.GetString(MyLocalize.kUpdate)
				, MyLocalize.GetString(MyLocalize.kCancel)
				, () =>{
						//TODO : xử lý khi bấm nút update
					Application.OpenURL(MyConstant.linkApp);
				}, null);
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

		TableDetail _tableDetail = null;
		for(int i = 0; i < DataManager.instance.miniGameData.currentMiniGameDetail.tableData.listTableDetail.Count; i++){
			if(DataManager.instance.miniGameData.currentMiniGameDetail.tableData.listTableDetail[i].tableId == short.Parse(_id)){
				_tableDetail = DataManager.instance.miniGameData.currentMiniGameDetail.tableData.listTableDetail[i];
				break;
			}
		}
		if(_tableDetail != null){
			DataManager.instance.miniGameData.currentMiniGameDetail.tableData.currentTableDetail = _tableDetail;			
			LoadingCanvasController.instance.Show(-1, false, null, ()=>{
				NetworkGlobal.instance.StopRealTime();
			});
			SetProcessJoinToTable();
			System.Action _onConnectionSuccess = ()=>{
				GlobalRealTimeSendingAPI.SendMessageJoinToTable(DataManager.instance.miniGameData.currentMiniGameDetail.myInfo.gameId
					, DataManager.instance.miniGameData.currentMiniGameDetail.tableData.currentTableDetail.tableId, _pass);
			};
			NetworkGlobal.instance.RunRealTime (_serverDetail, OnCreateConnectionError, _onConnectionSuccess, null, OnServerFull);
		}
	}
	#endregion

	#region Xử lý Logic chọn bàn
	void OnCreateConnectionError(int _error){
		LoadingCanvasController.instance.Hide();
		PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
			, MyLocalize.GetString(MyLocalize.kConnectionError)
			, _error.ToString()
			, MyLocalize.GetString(MyLocalize.kOk));
	}
	void OnPlayNowCreateConnectionSuccess(){
		if(DataManager.instance.miniGameData.currentMiniGameDetail == null){
			#if TEST
			Debug.LogError("currentMiniGameDetail is null");
			#endif
			return;
		}
		GlobalRealTimeSendingAPI.SendMessagePlayNowToTable(DataManager.instance.miniGameData.currentMiniGameDetail.myInfo.gameId);
	}
	void OnChooseTableCreateConnectionSuccess(){
		if(DataManager.instance.miniGameData.currentMiniGameDetail == null){
			#if TEST
			Debug.LogError("currentMiniGameDetail is null");
			#endif
			return;
		}
		GlobalRealTimeSendingAPI.SendMessageJoinToTable(DataManager.instance.miniGameData.currentMiniGameDetail.myInfo.gameId
		, DataManager.instance.miniGameData.currentMiniGameDetail.tableData.currentTableDetail.tableId, "");
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

		bool _showWarningDefault = true;
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
			_showWarningDefault = false;
			PopupManager.Instance.CreateToast(MyLocalize.GetString("Global/NotEnoughMoney"));
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
			_showWarningDefault = false;
			OnButtonFindTableClicked(DataManager.instance.miniGameData.currentMiniGameDetail.tableData.currentTableDetail.tableId.ToString());
			break;
		case -22: // TABLE_LAST_SESSION_EXIST Chỉ dùng cho joinTable
			#if TEST
			Debug.LogError("TABLE_LAST_SESSION_EXIST");
			#endif
			_showWarningDefault = false;
			// The user id already exists
			PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kWarning)
				, MyLocalize.GetString("ChooseTable/CreateTable_TABLE_LAST_SESSION_EXIST")
				, _result.ToString()
				, MyLocalize.GetString(MyLocalize.kOk));
			break;
		case -23: // TABLE_FULL_POSITION Chỉ dùng cho joinTable
			#if TEST
			Debug.LogError("TABLE_FULL_POSITION");
			#endif
			_showWarningDefault = false;
			PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kWarning)
				, MyLocalize.GetString(MyLocalize.kTableFull)
				, _result.ToString()
				, MyLocalize.GetString(MyLocalize.kOk));
			break;
		default:
			#if TEST
			Debug.LogError("ChosseTableError: " + _result);
			#endif
			break;
		}

		if(_showWarningDefault){
			PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
				, MyLocalize.GetString("ChooseTable/JoinTableFailed")
				, _result.ToString()
				, MyLocalize.GetString(MyLocalize.kOk));
		}
	}

	void SetProcessPlayNow(){
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_TABLE_PLAYNOW_TO_TABLE, (MessageReceiving _mess)=>{
			LoadingCanvasController.instance.Hide();
			sbyte _result = _mess.readByte();
			if(_result > 0){
				NetworkGlobal.instance.instanceRealTime.PauseReceiveMessage();

				TableDetail _tableDetail = new TableDetail();
				_tableDetail.tableId = _mess.readShort();
				DataManager.instance.miniGameData.currentMiniGameDetail.tableData.currentTableDetail = _tableDetail;

				SetUserDataAgainFromServer(_mess);
				ChangeSceneGamePlay();
			}else{
				SetDataWhenReceivingDataError(_result);
			}
		});
	}

	void SetProcessCreatePrivateTable(bool _havePass){
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_TABLE_CREATE_PASSWORD_TABLE, (MessageReceiving _mess)=>{
			LoadingCanvasController.instance.Hide();
			sbyte _result = _mess.readByte();
			if(_result > 0){
				NetworkGlobal.instance.instanceRealTime.PauseReceiveMessage();

				TableDetail _tableDetail = new TableDetail();
				_tableDetail.tableId = _mess.readShort();
				_tableDetail.isLockByPass = _havePass;
				DataManager.instance.miniGameData.currentMiniGameDetail.tableData.currentTableDetail = _tableDetail;

				SetUserDataAgainFromServer(_mess);
				
				ChangeSceneGamePlay();
			}else{
				SetDataWhenReceivingDataError(_result);
			}
		});
	}

	void SetProcessJoinToTable(){
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_TABLE_JOIN_TO_TABLE, (MessageReceiving _mess)=>{
			LoadingCanvasController.instance.Hide();
			sbyte _result = _mess.readByte();
			if(_result > 0){
				NetworkGlobal.instance.instanceRealTime.PauseReceiveMessage();
				SetUserDataAgainFromServer(_mess);
				ChangeSceneGamePlay();
			}else{
				SetDataWhenReceivingDataError(_result);
			}
		});
	}
	
	void SetUserDataAgainFromServer(MessageReceiving _mess){
		DataManager.instance.userData.GetMoreUserData(_mess);
	}

	void ChangeSceneGamePlay(){
		#if TEST
		NetworkGlobal.instance.SetProcessRealTime(CMD_REALTIME.S_SERVER_DEBUG_LOG, (_mess)=>{Debug.Log(_mess.readString());});
		#endif
		
		if(!DataManager.instance.hadRatingBefore){
			DataManager.instance.remindRating_GoldUserCatched = DataManager.instance.userData.gold;
		}

		switch(DataManager.instance.miniGameData.currentMiniGameDetail.myInfo.gameType){
		case IMiniGameInfo.Type.AnimalRacing:
			SceneLoaderManager.instance.LoadScene(MyConstant.SCENE_ANIMALRACING_PLAY);
			break;
		case IMiniGameInfo.Type.BattleOfLegend:
            SceneLoaderManager.instance.LoadScene(MyConstant.SCENE_BATTLE_PLAY);
			break;
		case IMiniGameInfo.Type.Poker:
			SceneLoaderManager.instance.LoadScene(MyConstant.SCENE_POKER_PLAY);
			break;
		case IMiniGameInfo.Type.Uno:
			SceneLoaderManager.instance.LoadScene(MyConstant.SCENE_UNO_PLAY);
			break;
		}
	}
	#endregion
}
