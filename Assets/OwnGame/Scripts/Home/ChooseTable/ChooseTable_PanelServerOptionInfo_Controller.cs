using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseTable_PanelServerOptionInfo_Controller : MonoBehaviour {

	[SerializeField] GameObject glow;
	[SerializeField] GameObject shadow;
    [SerializeField] Text txt_RoomName;
	[SerializeField] Text txt_RoomId;
	[SerializeField] Image panelLoading;
	[SerializeField] ParticleSystem particleLoading;

	public SubServerDetail serverDetail{get;set;}
	public bool initDataError{get;set;}
	LTDescr tweenPanelDelay;

	ChooseTable_PanelListServer_Controller panelListSvController;

	// public void InitData(ChooseTable_PanelListServer_Controller _panelListSvController, RoomServerDetail _roomDetail){
	// 	panelListSvController = _panelListSvController;
	// 	initDataError = false;
	// 	roomDetail = _roomDetail;
	// 	if(roomDetail.subServerDetail == null){
	// 		initDataError = true;
	// 	}else{
	// 		txt_RoomName.text = roomDetail.subServerDetail.subServerName;
	// 		txt_RoomId.text = string.Format("{0:00}", roomDetail.subServerDetail.subServerId) + "-" + roomDetail.subServerDetail.country.ToUpper();
	// 		glow.SetActive(false);
	// 		particleLoading.Stop();
	// 	}
	// }

	public void InitData(ChooseTable_PanelListServer_Controller _panelListSvController, SubServerDetail _serverDetail){
		panelListSvController = _panelListSvController;
		initDataError = false;
		serverDetail = _serverDetail;
		if(serverDetail == null){
			initDataError = true;
		}else{
			txt_RoomName.text = serverDetail.subServerName;
			txt_RoomId.text = string.Format("{0:00}", serverDetail.subServerId) + "-" + serverDetail.countryCode.ToUpper();
			glow.SetActive(false);
		}
		panelLoading.gameObject.SetActive(false);
		panelLoading.fillAmount = 0f;
	}

	void ShowDelaySelect(){
		if(tweenPanelDelay != null){
			LeanTween.cancel(tweenPanelDelay.uniqueId);
			tweenPanelDelay = null;
		}
		panelLoading.gameObject.SetActive(true);
		panelLoading.fillAmount = 1f;
		System.TimeSpan _ts = panelListSvController.chooseTableScreen.timeCanPressSelectServerOrTable - System.DateTime.Now;
		double _timeDelay = _ts.TotalSeconds;
		tweenPanelDelay = LeanTween.value(panelLoading.gameObject, panelLoading.fillAmount, 0f, (float) _timeDelay)
			.setOnUpdate((_value)=>{
				panelLoading.fillAmount = _value;
			})
			.setOnComplete(()=>{
				panelLoading.gameObject.SetActive(false);
				tweenPanelDelay = null;
			});
	}

	public void SetFocus(){
		if(shadow.activeSelf){
			HideShadow();
		}
		glow.SetActive(true);
	}

	public void SetUnFocus(){
		glow.SetActive(false);
	}

	public void ShowShadow(){
		shadow.SetActive(true);
	}

	public void HideShadow(){
		shadow.SetActive(false);
	}

	// public void OnButtonSelectClicked(){
	// 	#if TEST
	// 	Debug.Log (">>> Chọn Room: " + roomDetail.roomId + " - SubServerID: " + roomDetail.subServerId);
	// 	#endif
		
	// 	MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);

	// 	if(roomDetail.subServerDetail == null){
	// 		if(panelListSvController.currentRoom != this){
	// 			ShowShadow();
	// 			PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
	// 					, MyLocalize.GetString(MyLocalize.kRoomIsNotAvailable)
	// 					, "-99"
	// 					, MyLocalize.GetString(MyLocalize.kOk));
	// 		}
	// 		return;
	// 	}

	// 	#if TEST
	// 	Debug.Log (">>> Sub Server Detail: " + roomDetail.subServerDetail.subServerName + " - IP: " + roomDetail.subServerDetail.ip + " - port one hit: " + roomDetail.subServerDetail.portOnehit + " - port real time: " + roomDetail.subServerDetail.portRealtime);
	// 	#endif

	// 	particleLoading.Play();
	// 	panelListSvController.chooseTableScreen.LoadTableDataFromSever(serverDetail, ()=>{
	// 		particleLoading.Stop();
	// 		panelListSvController.currentRoom.SetUnFocus();
	// 		SetFocus();
	// 		panelListSvController.currentRoom = this;

	// 		DataManager.instance.miniGameData.currentMiniGameDetail.roomData.currentRoomDetail = roomDetail;
	// 	}, (_error)=>{
	// 		particleLoading.Stop();
	// 		if(panelListSvController.currentRoom != this){
	// 			ShowShadow();
	// 		}else{
	// 			// PopupManager.Instance.CreateToast(MyLocalize.GetString(MyLocalize.kRoomIsNotAvailable));
	// 		}

	// 		if(_error == -98){
	// 			#if TEST
	// 			Debug.Log("<color=green> RoomOutOfDate: " + DataManager.instance.miniGameData.currentMiniGameDetail.myInfo.versionFeature + " - " + roomDetail.versionServer + " </color>");
	// 			#endif
	// 			PopupManager.Instance.CreatePopupDialog(MyLocalize.GetString(MyLocalize.kWarning)
	// 				, MyLocalize.GetString("ChooseTable/RoomOutOfDate")
	// 				, string.Empty
	// 				, MyLocalize.GetString(MyLocalize.kUpdate)
	// 				, MyLocalize.GetString(MyLocalize.kCancel)
	// 				, () =>{
	// 						//TODO : xử lý khi bấm nút update
	// 					Application.OpenURL(MyConstant.linkApp);
	// 				}, null);
	// 		}else{
	// 			PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
	// 				, MyLocalize.GetString(MyLocalize.kRoomIsNotAvailable)
	// 				, _error.ToString()
	// 				, MyLocalize.GetString(MyLocalize.kOk));
	// 		}
	// 	});
	// }

	public void OnButtonSelectClicked(){
		if(panelListSvController.chooseTableScreen.timeCanPressSelectServerOrTable > System.DateTime.Now){
			return;
		}
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);

		if(serverDetail == null){
			if(panelListSvController.currentRoom != this){
				ShowShadow();
				PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
						, MyLocalize.GetString(MyLocalize.kRoomIsNotAvailable)
						, "-99"
						, MyLocalize.GetString(MyLocalize.kOk));
			}
			return;
		}
		#if TEST
		Debug.Log (">>> Chọn Room: " + serverDetail.subServerName + " - SubServerID: " + serverDetail.subServerId);
		#endif

		RoomDetail _roomDetail = null;
		for(int i = 0; i < serverDetail.listRoomDetail.Count; i++){
			if(serverDetail.listRoomDetail[i].gameId == DataManager.instance.miniGameData.currentMiniGameDetail.myInfo.gameId){
				_roomDetail = serverDetail.listRoomDetail[i];
				break;
			}
		}
		if(_roomDetail == null){
			#if TEST
			Debug.LogError("_roomDetail is null: " + DataManager.instance.miniGameData.currentMiniGameDetail.myInfo.gameType.ToString() + " - " + serverDetail.subServerName);
			#endif
			return;
		}

		panelListSvController.chooseTableScreen.timeCanPressSelectServerOrTable = System.DateTime.Now.AddSeconds(panelListSvController.chooseTableScreen.timeDelayToPressSelectServerOrTable);

		panelLoading.gameObject.SetActive(true);
		panelLoading.fillAmount = 1f;
		panelListSvController.chooseTableScreen.LoadAllTablesFromSever(serverDetail, ()=>{
			particleLoading.Stop();
			ShowDelaySelect();

			panelListSvController.currentRoom.SetUnFocus();
			SetFocus();
			panelListSvController.currentRoom = this;

			DataManager.instance.miniGameData.currentMiniGameDetail.currentServerDetail = serverDetail;
		}, (_error)=>{
			particleLoading.Stop();
			ShowDelaySelect();
			if(panelListSvController.currentRoom != this){
				ShowShadow();
			}else{
				// PopupManager.Instance.CreateToast(MyLocalize.GetString(MyLocalize.kRoomIsNotAvailable));
			}

			if(_error == -98){
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
			}else{
				PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
					, MyLocalize.GetString(MyLocalize.kRoomIsNotAvailable)
					, _error.ToString()
					, MyLocalize.GetString(MyLocalize.kOk));
			}
		});
	}
}
