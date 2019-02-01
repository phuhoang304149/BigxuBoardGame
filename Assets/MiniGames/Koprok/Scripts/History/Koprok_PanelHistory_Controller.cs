using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class Koprok_PanelHistory_Controller : MySimplePanelController {

	public enum State{
		Hide, Show
	}
	public State currentState{get;set;}

	[SerializeField] Transform itemContainer;
	[SerializeField] Transform arrow;
	[SerializeField] RectTransform rectTransformPanelMainContainer;

	[Header("Prefabs")]
	[SerializeField] GameObject optionPrefab;

	private KoprokData koprokData{
		get{
			return Koprok_GamePlay_Manager.instance.koprokData;
		}
	}
	List<Koprok_History_OptionInfo_Controller> listBetHistoryOptionDetail;
	LTDescr moveTween;
	float showPosX, hidePosX;
	
	void Awake(){
		listBetHistoryOptionDetail = new List<Koprok_History_OptionInfo_Controller>();
	}

	IEnumerator Start(){
		yield return Yielders.EndOfFrame;
		float _sizeW = rectTransformPanelMainContainer.sizeDelta.x;
		showPosX = 0f - _sizeW;
		hidePosX = 0f;
	}

	public override void ResetData (){
		StopAllCoroutines();
		LeanTween.cancel(gameObject);

		moveTween = null;
		currentState = State.Hide;

		if(listBetHistoryOptionDetail == null){
			listBetHistoryOptionDetail = new List<Koprok_History_OptionInfo_Controller>();
		}else{
			if(listBetHistoryOptionDetail.Count > 0){
				for(int i = 0; i < listBetHistoryOptionDetail.Count; i ++){
					listBetHistoryOptionDetail[i].SelfDestruction();
				}
				listBetHistoryOptionDetail.Clear();
			}
		}
	}

	public override void InitData (System.Action _onFinished = null){
		if(koprokData == null || koprokData.listHistory == null || koprokData.listHistory.Count == 0){
			return;
		}
		Sprite _imgSlot00 = null;
		Sprite _imgSlot01 = null;
		Sprite _imgSlot02 = null;
		Koprok_History_OptionInfo_Controller _historyDetail = null;
		for(int i = 0; i < koprokData.listHistory.Count; i ++){
			_historyDetail = LeanPool.Spawn(optionPrefab, Vector3.zero, Quaternion.identity, itemContainer.transform).GetComponent<Koprok_History_OptionInfo_Controller>();
			_imgSlot00 = Koprok_GamePlay_Manager.instance.spriteIconBet[(int) koprokData.listHistory[i].dice[0]];
			_imgSlot01 = Koprok_GamePlay_Manager.instance.spriteIconBet[(int) koprokData.listHistory[i].dice[1]];
			_imgSlot02 = Koprok_GamePlay_Manager.instance.spriteIconBet[(int) koprokData.listHistory[i].dice[2]];
			_historyDetail.InitData (_imgSlot00, _imgSlot01, _imgSlot02, (i == 0 ? 1f : 0.4f));
			listBetHistoryOptionDetail.Add(_historyDetail);
		}
	}

	public override Coroutine Show (){
		if(currentState == State.Show){
			return null;
		}

		currentState = State.Show;
		Vector3 _localScale = arrow.localScale;
		_localScale.x = -1f;
		arrow.localScale = _localScale;

		if(moveTween != null){
			LeanTween.cancel(gameObject, moveTween.uniqueId);
			moveTween = null;
		}
		moveTween = LeanTween.moveLocalX(gameObject, showPosX, 0.3f).setEase(LeanTweenType.easeOutSine).setOnComplete(()=>{
			moveTween = null;
			_localScale = arrow.localScale;
			_localScale.x = 1f;
			arrow.localScale = _localScale;
		});
		return null;
	}

	public override Coroutine Hide (){
		if(currentState == State.Hide){
			return null;
		}
		currentState = State.Hide;
		Vector3 _localScale = arrow.localScale;
		_localScale.x = 1f;
		arrow.localScale = _localScale;

		if(moveTween != null){
			LeanTween.cancel(gameObject, moveTween.uniqueId);
			moveTween = null;
		}
		moveTween = LeanTween.moveLocalX(gameObject, hidePosX, 0.3f).setEase(LeanTweenType.easeOutSine).setOnComplete(()=>{
			moveTween = null;
			_localScale = arrow.localScale;
			_localScale.x = -1f;
			arrow.localScale = _localScale;
			ResetData();
		});
		return null;
	}

	public void SelfDestruction(){
		if(listBetHistoryOptionDetail != null && listBetHistoryOptionDetail.Count > 0){
			for(int i = 0; i < listBetHistoryOptionDetail.Count; i ++){
				listBetHistoryOptionDetail[i].SelfDestruction();
			}
			listBetHistoryOptionDetail.Clear();
		}
	}
}
