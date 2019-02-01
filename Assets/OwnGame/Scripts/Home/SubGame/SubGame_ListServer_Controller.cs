using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;

public class SubGame_ListServer_Controller : MySimplePanelController {

	enum State{
		Hide, Show
	}
	State currentState;
	[SerializeField] CanvasGroup myCanvasGroup;
	[SerializeField] LoopScrollRect mainScrollRect;

	public List<SubServerDetail> listRoomDetail{get;set;}

	public override void ResetData(){
		StopAllCoroutines();
		currentState = State.Hide;
		myCanvasGroup.alpha = 0f;
		myCanvasGroup.blocksRaycasts = false;

		if(listRoomDetail != null && listRoomDetail.Count > 0){
			listRoomDetail.Clear();
		}
		
		mainScrollRect.ClearCells();
	}

	public override void InitData (System.Action _onFinished = null){
		if(listRoomDetail == null){
			listRoomDetail = new List<SubServerDetail>();
		}
		CreatePanelServerOptionInfo();
		if(_onFinished != null){
			_onFinished();
		}
	} 

	public override Coroutine Show (){
		if(currentState == State.Show){
			return null;
		}
		currentState = State.Show;
		myCanvasGroup.alpha = 1f;
		myCanvasGroup.blocksRaycasts = true;
		return null;
	}

	public override Coroutine Hide (){
		if(currentState == State.Hide){
			return null;
		}
		currentState = State.Hide;
		myCanvasGroup.alpha = 0f;
		myCanvasGroup.blocksRaycasts = false;
		ResetData();
		return null;
	}

	void CreatePanelServerOptionInfo(){
		for(int i = 0; i < DataManager.instance.subServerData.listSubServerDetail.Count; i ++){
			if(!DataManager.instance.subServerData.listSubServerDetail[i].beingError){
				listRoomDetail.Add(DataManager.instance.subServerData.listSubServerDetail[i]);
			}
		}
		for(int i = 0; i < DataManager.instance.subServerData.listSubServerDetail.Count; i ++){
			if(DataManager.instance.subServerData.listSubServerDetail[i].beingError){
				listRoomDetail.Add(DataManager.instance.subServerData.listSubServerDetail[i]);
			}
		}
		
		mainScrollRect.totalCount = listRoomDetail.Count;
        mainScrollRect.RefillCells();
	}
}
