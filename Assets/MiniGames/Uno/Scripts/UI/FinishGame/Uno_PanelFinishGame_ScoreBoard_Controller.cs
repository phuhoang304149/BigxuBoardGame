using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class Uno_PanelFinishGame_ScoreBoard_Controller : MonoBehaviour {

	public enum State{
		Hide, Show
	}
	public State currentState{get;set;}

	[SerializeField] CanvasGroup myCanvasGroup;
	[SerializeField] Transform mainContent;
	[SerializeField] Transform panelDetailContainer;
	
	[Header("Prefabs")]
	[SerializeField] GameObject scoreBoardOptionPrefab;

	LTDescr tweenCanvasGroup, tweenMainContent;
	IEnumerator actionShowOrHide;
	public MySimplePoolManager scoreBoardOptionPoolManager;

	private void Awake() {
		currentState = State.Hide;
		myCanvasGroup.alpha = 0f;
		myCanvasGroup.blocksRaycasts = false;
		scoreBoardOptionPoolManager = new MySimplePoolManager();
	}

	public void InitData (UnoGamePlayData.Uno_FinishGame_Data _finishGameData){
		if(_finishGameData == null){
			return;
		}
		scoreBoardOptionPoolManager.ClearAllObjectsNow();
		for(int i = 0; i < _finishGameData.listPlayersData.Count; i++){
			Uno_FinishGame_ScoreBoard_Option_Controller _option = LeanPool.Spawn(scoreBoardOptionPrefab, Vector3.zero, Quaternion.identity, panelDetailContainer).GetComponent<Uno_FinishGame_ScoreBoard_Option_Controller>();
			UnoGamePlayData.Uno_PlayerPlayingData _playerFinishData = Uno_GamePlay_Manager.instance.unoGamePlayData.listPlayerPlayingData[_finishGameData.listPlayersData[i].indexCircle];
			_option.InitData(_finishGameData.listPlayersData[i].isWin, _playerFinishData.userData.nameShowInGame, _finishGameData.goldWin, _finishGameData.listPlayersData[i].totalPoint);
			scoreBoardOptionPoolManager.AddObject(_option);
		}
	}

	public Coroutine Show(){
		if(currentState == State.Show){
			return null;
		}
		currentState = State.Show;
		myCanvasGroup.alpha = 0f;
		myCanvasGroup.blocksRaycasts = true;
		mainContent.localScale = Vector3.one * 0.5f;
		
		if(tweenCanvasGroup != null){
			LeanTween.cancel(tweenCanvasGroup.uniqueId);
			tweenCanvasGroup = null;
		}
		if(tweenMainContent != null){
			LeanTween.cancel(tweenMainContent.uniqueId);
			tweenMainContent = null;
		}
		
		if(actionShowOrHide != null){
			StopCoroutine(actionShowOrHide);
			actionShowOrHide = null;
		}

		actionShowOrHide = DoActionShow();
		return StartCoroutine(actionShowOrHide);
	}

	IEnumerator DoActionShow(){
		bool _isFinished = false;
		tweenCanvasGroup = LeanTween.alphaCanvas(myCanvasGroup, 1f, 0.1f).setOnComplete(()=>{
			tweenCanvasGroup = null;
		});
		tweenMainContent = LeanTween.scale(mainContent.gameObject, Vector3.one, 0.2f).setEase(LeanTweenType.easeOutBack).setOnComplete(()=>{
			tweenMainContent = null;
			_isFinished = true;
		});
		yield return new WaitUntil(()=>_isFinished);
		actionShowOrHide = null;
	}

	public Coroutine Hide(){
		if(currentState == State.Hide){
			return null;
		}
		currentState = State.Hide;

		if(tweenCanvasGroup != null){
			LeanTween.cancel(tweenCanvasGroup.uniqueId);
			tweenCanvasGroup = null;
		}
		if(tweenMainContent != null){
			LeanTween.cancel(tweenMainContent.uniqueId);
			tweenMainContent = null;
		}
		if(actionShowOrHide != null){
			StopCoroutine(actionShowOrHide);
			actionShowOrHide = null;
		}
		actionShowOrHide = DoActionHide();
		return StartCoroutine(actionShowOrHide);
	}

	IEnumerator DoActionHide(){
		bool _isFinished = false;
		tweenCanvasGroup = LeanTween.alphaCanvas(myCanvasGroup, 0f, 0.2f).setOnComplete(()=>{
			tweenCanvasGroup = null;
			myCanvasGroup.blocksRaycasts = false;
			scoreBoardOptionPoolManager.ClearAllObjectsNow();
			_isFinished = true;
		});
		tweenMainContent = LeanTween.scale(mainContent.gameObject, Vector3.one * 0.5f, 0.2f).setEase(LeanTweenType.easeInBack).setOnComplete(()=>{
			tweenMainContent = null;
		});
		yield return new WaitUntil(()=>_isFinished);
	}

}
