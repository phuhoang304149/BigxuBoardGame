using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragonTiger_PanelHistory_Controller : MySimplePanelController {

	public enum State{
		Hide, Show
	}
	[HideInInspector] public State currentState;
	[SerializeField] RectTransform myRectTransform;

	[Header("History A")]
	[SerializeField] CanvasGroup canvasGroupHistoryA;
	[SerializeField] RectTransform rectTransformHistoryA;
	[SerializeField] GridLayoutGroup gridLayoutGroupA;

	[Header("History B")]
	[SerializeField] CanvasGroup canvasGroupHistoryB;
	[SerializeField] RectTransform rectTransformHistoryB;
	[SerializeField] GridLayoutGroup gridLayoutGroupB;

	[Header("Prefab")]
	[SerializeField] GameObject panelHistoryA_OptionInfo_Prefab;
	[SerializeField] GameObject panelHistoryB_OptionInfo_Prefab;
	
	List<DragonTiger_HistoryA_OptionInfo_Controller> listPanelHistoryAOption;
	List<DragonTiger_HistoryB_OptionInfo_Controller> listPanelHistoryBOption;

	private DragonTigerCasinoData dragonTigerCasinoData{
		get{
			return DragonTiger_GamePlay_Manager.instance.dragonTigerCasinoData;
		}
	}

	int rowCountOptionHistoryA, colCountOptionHistoryA;
	int rowCountOptionHistoryB, colCountOptionHistoryB;

	float startPosX;
	LTDescr moveTween;

	private void Awake() {
		currentState = State.Hide;
		canvasGroupHistoryA.alpha = 0f;
		canvasGroupHistoryB.alpha = 0f;
	}

	IEnumerator Start(){
		yield return Yielders.EndOfFrame;
		float _sizeW = myRectTransform.sizeDelta.x;
		startPosX = 0f + (rectTransformHistoryA.sizeDelta.x + rectTransformHistoryB.sizeDelta.x);

		Vector2 _offsetMin = myRectTransform.offsetMin;
		_offsetMin.x = startPosX;
		_offsetMin.y = DragonTiger_GamePlay_Manager.instance.panelHistoryPlaceHolder.offsetMin.y;
		myRectTransform.offsetMin = _offsetMin;

		Vector2 _offsetMax = myRectTransform.offsetMax;
		_offsetMax.x = startPosX + _sizeW;
		_offsetMax.y = DragonTiger_GamePlay_Manager.instance.panelHistoryPlaceHolder.offsetMax.y;
		myRectTransform.offsetMax = _offsetMax;

		Vector3 _pos = transform.position;
		_pos.y = DragonTiger_GamePlay_Manager.instance.panelHistoryPlaceHolder.transform.position.y;
		transform.position = _pos;

		yield return Yielders.EndOfFrame;
		// Debug.Log((rectTransformHistoryA.sizeDelta.y / gridLayoutGroupA.cellSize.y) + " - " + (rectTransformHistoryA.sizeDelta.x / gridLayoutGroupA.cellSize.x));

		listPanelHistoryAOption = new List<DragonTiger_HistoryA_OptionInfo_Controller>();
		rowCountOptionHistoryA = Mathf.FloorToInt(rectTransformHistoryA.sizeDelta.y / gridLayoutGroupA.cellSize.y);
		colCountOptionHistoryA = Mathf.FloorToInt(rectTransformHistoryA.sizeDelta.x / gridLayoutGroupA.cellSize.x);
		int _totalOption = rowCountOptionHistoryA * colCountOptionHistoryA;
		DragonTiger_HistoryA_OptionInfo_Controller _tmpOptionAController;
		for(int i = 0; i < _totalOption; i ++){
			_tmpOptionAController = Instantiate(panelHistoryA_OptionInfo_Prefab, rectTransformHistoryA.transform).GetComponent<DragonTiger_HistoryA_OptionInfo_Controller>();
			listPanelHistoryAOption.Add(_tmpOptionAController);
		}

		listPanelHistoryBOption = new List<DragonTiger_HistoryB_OptionInfo_Controller>();
		rowCountOptionHistoryB = Mathf.FloorToInt(rectTransformHistoryB.sizeDelta.y / gridLayoutGroupB.cellSize.y);
		colCountOptionHistoryB = Mathf.FloorToInt(rectTransformHistoryB.sizeDelta.x / gridLayoutGroupB.cellSize.x);
		_totalOption = rowCountOptionHistoryB * colCountOptionHistoryB;
		DragonTiger_HistoryB_OptionInfo_Controller _tmpOptionBController;
		for(int i = 0; i < _totalOption; i ++){
			_tmpOptionBController = Instantiate(panelHistoryB_OptionInfo_Prefab, rectTransformHistoryB.transform).GetComponent<DragonTiger_HistoryB_OptionInfo_Controller>();
			listPanelHistoryBOption.Add(_tmpOptionBController);
		}
	}

	public override void ResetData (){
		StopAllCoroutines();
		LeanTween.cancel(gameObject);
		moveTween = null;

		currentState = State.Hide;
		canvasGroupHistoryA.alpha = 0f;
		canvasGroupHistoryB.alpha = 0f;
	}

	public override void InitData (System.Action _onFinished = null){
		if(dragonTigerCasinoData.listHistory == null || dragonTigerCasinoData.listHistory.Count == 0){
			return;
		}
		
		InitPanelHistoryA();
		InitPanelHistoryB();
	}

	void InitPanelHistoryA(){
		List<sbyte> _listHistoryA = new List<sbyte>();
		for(int i = 0; i < dragonTigerCasinoData.listHistory.Count; i ++){
			_listHistoryA.Add(dragonTigerCasinoData.listHistory[i]);
		}
		int _limitOptionHistoryA = listPanelHistoryAOption.Count - rowCountOptionHistoryA;
		if(_listHistoryA.Count > _limitOptionHistoryA){
			int _tmpDelta = _listHistoryA.Count - _limitOptionHistoryA;
			for(int i = 0; i < _tmpDelta; i ++){
				_listHistoryA.RemoveAt(0);
			}
		}

		for(int i = 0; i < _listHistoryA.Count; i ++){
			listPanelHistoryAOption[i].InitData((DragonTiger_GamePlay_Manager.IndexBet) _listHistoryA[i]);
		}
		for(int i = _listHistoryA.Count; i < listPanelHistoryAOption.Count; i++){
			listPanelHistoryAOption[i].SetEmpty();
		}
	}

	void InitPanelHistoryB(){
		List<sbyte> _listHistoryB = new List<sbyte>();
		sbyte _lastValue = -99;
		for(int i = 0; i < dragonTigerCasinoData.listHistory.Count; i ++){
			if(i == 0){
				_listHistoryB.Add(dragonTigerCasinoData.listHistory[i]);
			}else{
				if(_lastValue == dragonTigerCasinoData.listHistory[i]){
					_listHistoryB.Add(dragonTigerCasinoData.listHistory[i]);
				}else{
					int _tmpIndex = (_listHistoryB.Count % rowCountOptionHistoryB);
					for(int j = _tmpIndex; j < rowCountOptionHistoryB; j++){
						_listHistoryB.Add(-99);
					}
					_listHistoryB.Add(dragonTigerCasinoData.listHistory[i]);
				}
			}
			_lastValue = dragonTigerCasinoData.listHistory[i];
		}

		int _limitOptionHistoryB = listPanelHistoryBOption.Count - rowCountOptionHistoryB;
		while(_listHistoryB.Count > _limitOptionHistoryB){
			for(int i = 0; i < rowCountOptionHistoryB; i ++){
				_listHistoryB.RemoveAt(0);
			}
		}

		for(int i = 0; i < _listHistoryB.Count; i ++){
			if(_listHistoryB[i] == -99){
				listPanelHistoryBOption[i].SetEmpty();
			}else{
				listPanelHistoryBOption[i].InitData((DragonTiger_GamePlay_Manager.IndexBet) _listHistoryB[i]);
			}
		}
		for(int i = _listHistoryB.Count; i < listPanelHistoryBOption.Count; i++){
			listPanelHistoryBOption[i].SetEmpty();
		}
	}

	public override void RefreshData (){}

	// public void Toggle(){
	// 	if(currentState == State.Show){
	// 		Hide();
	// 	}else{
	// 		InitData();
	// 		Show();
	// 	}
	// }

	public override Coroutine Show (){
		if(currentState == State.Show){
			return null;
		}

		currentState = State.Show;
		canvasGroupHistoryA.alpha = 1f;
		canvasGroupHistoryB.alpha = 1f;

		if(moveTween != null){
			LeanTween.cancel(gameObject, moveTween.uniqueId);
			moveTween = null;
		}
		moveTween = LeanTween.moveLocalX(gameObject, 0f, 0.3f).setEase(LeanTweenType.easeOutSine).setOnComplete(()=>{
			moveTween = null;
		});
		return null;
	}

	public override Coroutine Hide (){
		if(currentState == State.Hide){
			return null;
		}
		currentState = State.Hide;
		if(moveTween != null){
			LeanTween.cancel(gameObject, moveTween.uniqueId);
			moveTween = null;
		}
		moveTween = LeanTween.moveLocalX(gameObject, startPosX, 0.3f).setEase(LeanTweenType.easeOutSine).setOnComplete(()=>{
			moveTween = null;
			ResetData();
		});
		return null;
	}

}
