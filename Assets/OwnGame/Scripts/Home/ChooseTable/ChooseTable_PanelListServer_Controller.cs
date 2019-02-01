using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseTable_PanelListServer_Controller : MonoBehaviour {

	[SerializeField] RectTransform myContent;
	[SerializeField] GameObject prefabRoomOption;
	[SerializeField] ScrollRect scrollRectOfList;
	public List<ChooseTable_PanelServerOptionInfo_Controller> listRoom{ get; set;}
	public ChooseTable_PanelServerOptionInfo_Controller currentRoom{ get; set;}
	public ChooseTableScreenController chooseTableScreen{get;set;}
	public bool isInstalled;
	
	// public void InitData(ChooseTableScreenController _chooseTableScreen){
	// 	if(!isInstalled){
	// 		chooseTableScreen = _chooseTableScreen;
	// 		MiniGameDetail _gameDetail = DataManager.instance.miniGameData.currentMiniGameDetail;
	// 		listRoom = new List<ChooseTable_PanelServerOptionInfo_Controller>();
	// 		if(_gameDetail.roomData.listRoomServerDetail == null || _gameDetail.roomData.listRoomServerDetail.Count == 0){
	// 			#if TEST
	// 			Debug.LogError(">>> BUG Logic: listRoomServerDetail is NULL");
	// 			#endif
	// 			return;
	// 		}
	// 		for(int i = 0; i < _gameDetail.roomData.listRoomServerDetail.Count; i++){
	// 			ChooseTable_PanelServerOptionInfo_Controller _roomInfo = ((GameObject) Instantiate (prefabRoomOption, myContent.transform, false)).GetComponent<ChooseTable_PanelServerOptionInfo_Controller> ();
	// 			_roomInfo.InitData (this, _gameDetail.roomData.listRoomServerDetail[i]);
	// 			if(_roomInfo.initDataError){
	// 				Destroy(_roomInfo.gameObject);
	// 				continue;
	// 			}else{
	// 				if(_gameDetail.roomData.currentRoomDetail.IsEqual(_gameDetail.roomData.listRoomServerDetail[i])){
	// 					currentRoom = _roomInfo;
	// 					currentRoom.SetFocus();
	// 				}
	// 			}
	// 			listRoom.Add (_roomInfo);
	// 			_roomInfo.transform.SetAsFirstSibling();
	// 		}

	// 		isInstalled = true;
	// 	}
	// }

	public void InitData(ChooseTableScreenController _chooseTableScreen){
		if(!isInstalled){
			chooseTableScreen = _chooseTableScreen;
			MiniGameDetail _gameDetail = DataManager.instance.miniGameData.currentMiniGameDetail;
			listRoom = new List<ChooseTable_PanelServerOptionInfo_Controller>();

			for(int i = 0; i < _gameDetail.listServerDetail_Normal.Count; i++){
				ChooseTable_PanelServerOptionInfo_Controller _roomInfo = ((GameObject) Instantiate (prefabRoomOption, myContent.transform, false)).GetComponent<ChooseTable_PanelServerOptionInfo_Controller> ();
				_roomInfo.InitData (this, _gameDetail.listServerDetail_Normal[i]);
				if(_roomInfo.initDataError){
					Destroy(_roomInfo.gameObject);
					continue;
				}else{
					if(_gameDetail.currentServerDetail.IsEqual(_gameDetail.listServerDetail_Normal[i])){
						currentRoom = _roomInfo;
						currentRoom.SetFocus();
					}
				}
				listRoom.Add (_roomInfo);
				_roomInfo.transform.SetAsFirstSibling();
			}
			
			for(int i = 0; i < _gameDetail.listServerDetail_Error.Count; i++){
				ChooseTable_PanelServerOptionInfo_Controller _roomInfo = ((GameObject) Instantiate (prefabRoomOption, myContent.transform, false)).GetComponent<ChooseTable_PanelServerOptionInfo_Controller> ();
				_roomInfo.InitData (this, _gameDetail.listServerDetail_Error[i]);
				if(_roomInfo.initDataError){
					Destroy(_roomInfo.gameObject);
					continue;
				}else{
					if(_gameDetail.currentServerDetail.IsEqual(_gameDetail.listServerDetail_Error[i])){
						currentRoom = _roomInfo;
						currentRoom.SetFocus();
					}
				}
				listRoom.Add (_roomInfo);
				_roomInfo.transform.SetAsFirstSibling();
			}
			
			isInstalled = true;
		}
	}

	public void FocusRoom(SubServerDetail _serverDetail, bool _moveList){
		if(_serverDetail == null){
			return;
		}
		bool _findSuccess = false;
		for(int i = 0; i < listRoom.Count; i ++){
			if(listRoom[i].serverDetail.IsEqual(_serverDetail)){
				currentRoom.SetUnFocus();
				listRoom[i].SetFocus();
				currentRoom = listRoom[i];
				_findSuccess = true;
				break;
			}
		}
		if(_findSuccess){
			if(_moveList){
				MyConstant.ScrollRectHorizontalFocusCenterItem(scrollRectOfList, currentRoom.gameObject);
			}
		}else{
			#if TEST
			SubServerDetail _tmpServerDetail = DataManager.instance.subServerData.GetSubServerDetail(_serverDetail.subServerId);
			if(_tmpServerDetail != null){
				Debug.LogError("Không tìm thấy ServerDetail (0) : " + _serverDetail.subServerName + "|" +_serverDetail.subServerId + "|" +_tmpServerDetail.subServerName + "|" + _tmpServerDetail.subServerId);
			}else{
				Debug.LogError("Không tìm thấy ServerDetail (1) : " + _serverDetail.subServerName + "|" +_serverDetail.subServerId);
			}
			#endif
		}
		
	}

	public void SelfDestruction(){
		if(listRoom != null && listRoom.Count > 0){
			for(int i = 0; i < listRoom.Count; i++){
				Destroy(listRoom[i].gameObject);
			}
			listRoom.Clear();
		}
		isInstalled = false;
	}
}
