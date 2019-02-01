using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;

public class GetGoldScreen_PanelInstallApp_Controller : MySimplePanelController {

	public enum State{
		Show, Hide
	}
	State currentState;

	[SerializeField] CanvasGroup myCanvasGroup;
	[SerializeField] Text txtEmpty;
	[SerializeField] Transform panelLoading;
	[SerializeField] Transform panelFocusScreen;
	[SerializeField] LoopScrollRect mainScrollRect;

	public List<InstallAppDetail> listCurrentAppDetail{get;set;}

	public System.DateTime timeCanPressGetReward;
	bool isInitialized;

	public override void ResetData(){
		StopAllCoroutines();
		currentState = State.Hide;
		myCanvasGroup.alpha = 0f;
		myCanvasGroup.blocksRaycasts = false;

		panelFocusScreen.gameObject.SetActive(false);

		if(listCurrentAppDetail != null && listCurrentAppDetail.Count > 0){
			listCurrentAppDetail.Clear();
		}
		panelLoading.gameObject.SetActive(false);
		txtEmpty.gameObject.SetActive(false);
		mainScrollRect.ClearCells();

		timeCanPressGetReward = System.DateTime.Now;

		isInitialized = false;
	}

	public override void InitData (System.Action _onFinished = null){
		if(listCurrentAppDetail == null){
			listCurrentAppDetail = new List<InstallAppDetail>();
		}
	}

	public override void RefreshData(){
		if(DataManager.instance.installAppData.listCurrentAppDetail.Count == 0){
			panelLoading.gameObject.SetActive(true);
			txtEmpty.gameObject.SetActive(true);
			GetListAppFromServer(()=>{
				if(DataManager.instance.installAppData.listCurrentAppDetail.Count == 0){
					txtEmpty.gameObject.SetActive(true);
				}else{
					txtEmpty.gameObject.SetActive(false);
				}
				panelLoading.gameObject.SetActive(false);
				CreatePanelAppInfo();
			});
		}else{
			for(int i = 0; i < DataManager.instance.installAppData.listCurrentAppDetail.Count; i++){
				if(DataManager.instance.installAppData.listCurrentAppDetail[i].currentState == InstallAppDetail.State.Done){
					DataManager.instance.installAppData.listAppDone.Add(DataManager.instance.installAppData.listCurrentAppDetail[i]);
					DataManager.instance.installAppData.listCurrentAppDetail.RemoveAt(i);
					i--;
					continue;
				}
			}
			CreatePanelAppInfo();
			if(System.DateTime.Now >= DataManager.instance.installAppData.nextTimeToGetDataFromSever){
				GetListAppFromServer();
			}
		}
	}

	public void ClearAndCreateNewPanelAppInfo(InstallAppDetail _currentAppDetail){
		StartCoroutine(DoActionClearAndCreateNewPanelAppInfo(_currentAppDetail));
	}

	IEnumerator DoActionClearAndCreateNewPanelAppInfo(InstallAppDetail _currentAppDetail){
		int _tmpIndexOfCurrentAppDetail = listCurrentAppDetail.IndexOf(_currentAppDetail);
		mainScrollRect.ClearCells();
		yield return Yielders.EndOfFrame;
		if(listCurrentAppDetail != null && listCurrentAppDetail.Count > 0){
			listCurrentAppDetail.Clear();
		}
		RefreshData();
		yield return null;
		if(listCurrentAppDetail != null && listCurrentAppDetail.Count > 0){
			if(_tmpIndexOfCurrentAppDetail >= listCurrentAppDetail.Count - 1){
				_tmpIndexOfCurrentAppDetail = listCurrentAppDetail.Count - 1;
			}
		}else{
			_tmpIndexOfCurrentAppDetail = 0;
		}
		mainScrollRect.SrollToCell(_tmpIndexOfCurrentAppDetail, 10000f);
	}

	void CreatePanelAppInfo(){
		for(int i = 0; i < DataManager.instance.installAppData.listCurrentAppDetail.Count; i ++){
			listCurrentAppDetail.Add(DataManager.instance.installAppData.listCurrentAppDetail[i].ShallowCopy());
		}
		mainScrollRect.totalCount = listCurrentAppDetail.Count;
        mainScrollRect.RefillCells();
	}

	void GetListAppFromServer(System.Action _onFinished = null){
		OneHitAPI.GetListCampagneInstallAndroid((_messageReceiving, _error)=>{
			if(_messageReceiving != null){
				short _numberCampagne = _messageReceiving.readShort();
				List<InstallAppDetail> _newListInstallApp = new List<InstallAppDetail>();
				bool _canAddToList = false;
				for(int i = 0; i < _numberCampagne; i ++){
					InstallAppDetail _detail = new InstallAppDetail(_messageReceiving);
					_canAddToList = true;
					for(int j = 0; j < DataManager.instance.installAppData.listAppDone.Count; j++){
						if(DataManager.instance.installAppData.listAppDone[j].IsEqual(_detail)){
							_canAddToList = false;
							break;
						}
					}
					if(_canAddToList){
						_newListInstallApp.Add(_detail);
					}
				}
				DataManager.instance.installAppData.listCurrentAppDetail = _newListInstallApp;
				// for(int i = 0; i < 1000; i++){
				// 	DataManager.instance.installAppData.listCurrentAppDetail.Add(DataManager.instance.installAppData.listCurrentAppDetail[0]);
				// }
				DataManager.instance.installAppData.nextTimeToGetDataFromSever = System.DateTime.Now.AddHours(1);
				if(currentState == State.Show){
					if(_onFinished != null){
						_onFinished();
					}
				}
			}else{
				#if TEST
				Debug.LogError("GetListCampagneInstallAndroid is Error: " + _error);
				#endif
			}
		});
	}

	public override Coroutine Show (){
		currentState = State.Show;
		myCanvasGroup.alpha = 1f;
		myCanvasGroup.blocksRaycasts = true;

		panelFocusScreen.gameObject.SetActive(true);

		RefreshData();
		return null;
	}

	public override Coroutine Hide (){
		ResetData();
		return null;
	}

}
