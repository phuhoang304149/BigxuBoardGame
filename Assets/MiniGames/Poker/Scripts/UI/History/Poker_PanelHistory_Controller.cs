using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class Poker_PanelHistory_Controller : MySimplePanelController {
	public enum State{
		Hide, Show
	}
	public State currentState{get;set;}

	[SerializeField] CanvasGroup myCanvasGroup;
	[SerializeField] CanvasGroup canvasGroupGlobalCards;
	[SerializeField] CanvasGroup canvasGroupListUser;
	[SerializeField] CanvasGroup canvasGroupDataIsEmpty;
	[SerializeField] Transform itemContainer;
	[SerializeField] RectTransform rectTransformPanelMainContainer;
	[SerializeField] List<PanelCardDetailController> globalCards;

	[Header("Prefabs")]
	[SerializeField] GameObject optionPrefab;

	[Header("Setting")]
	[SerializeField] float timeTweenMainContent;

	private PokerGamePlayData.Poker_HistoryData pokerHistoryData{
		get{
			return Poker_GamePlay_Manager.instance.pokerGamePlayData.historyData;
		}
	}
	List<Poker_History_OptionInfo_Controller> listBetHistoryOptionDetail;
	LTDescr moveTween, myTweenCanvasGroup;
	float showPosX, hidePosX;
	long idData = -1;

	void Awake(){
		listBetHistoryOptionDetail = new List<Poker_History_OptionInfo_Controller>();
		ResetData();
	}

	IEnumerator Start(){
		yield return Yielders.EndOfFrame;
		float _sizeW = rectTransformPanelMainContainer.sizeDelta.x;
		showPosX = 0f - _sizeW - 60f;
		hidePosX = 0f + 60f;
	}

	public override void ResetData (){
		StopAllCoroutines();
		LeanTween.cancel(gameObject);

		moveTween = null;
		myTweenCanvasGroup = null;
		currentState = State.Hide;
		myCanvasGroup.alpha = 0f;
		myCanvasGroup.blocksRaycasts = false;

		canvasGroupGlobalCards.alpha = 0f;
		canvasGroupListUser.alpha = 0f;
		canvasGroupDataIsEmpty.alpha = 0f;
	}

	public override void InitData (System.Action _onFinished = null){
		if(pokerHistoryData == null){
			return;
		}
		if(idData == pokerHistoryData.id){
			return;
		}

		idData = pokerHistoryData.id;
		if(listBetHistoryOptionDetail == null){
			listBetHistoryOptionDetail = new List<Poker_History_OptionInfo_Controller>();
		}else{
			if(listBetHistoryOptionDetail.Count > 0){
				for(int i = 0; i < listBetHistoryOptionDetail.Count; i ++){
					listBetHistoryOptionDetail[i].SelfDestruction();
				}
				listBetHistoryOptionDetail.Clear();
			}
		}
		for(int i = 0; i < globalCards.Count; i ++){
			globalCards[i].ResetData();
		}

		ICardInfo _cardInfo = null;
		for(int i = 0; i < pokerHistoryData.globalCards.Count; i ++){
			if(pokerHistoryData.globalCards[i] < 0){
				continue;
			}
			_cardInfo = Poker_GamePlay_Manager.instance.GetCardInfo(pokerHistoryData.globalCards[i]);
			if(_cardInfo == null){
				Debug.LogError(">>> cardInfo is null : " + i + " - " + pokerHistoryData.globalCards[i]);
				continue;
			}
			globalCards[i].ShowNow(_cardInfo);
		}

		Poker_History_OptionInfo_Controller _historyDetail = null;
		for(int i = 0; i < pokerHistoryData.listPlayerPlayingData.Count; i ++){
			_historyDetail = LeanPool.Spawn(optionPrefab, Vector3.zero, Quaternion.identity, itemContainer.transform).GetComponent<Poker_History_OptionInfo_Controller>();
			bool _showHighlight = false;
			if(pokerHistoryData.circleIndexWin.Contains((sbyte) i)){
				_showHighlight = true;
			}
			_historyDetail.InitData (pokerHistoryData.listPlayerPlayingData[i], _showHighlight);
			listBetHistoryOptionDetail.Add(_historyDetail);
		}
	}

	public override Coroutine Show (){
		if(currentState == State.Show){
			return null;
		}

		if(pokerHistoryData == null){
			canvasGroupGlobalCards.alpha = 0f;
			canvasGroupListUser.alpha = 0f;
			canvasGroupDataIsEmpty.alpha = 1f;
		}else{
			canvasGroupGlobalCards.alpha = 1f;
			canvasGroupListUser.alpha = 1f;
			canvasGroupDataIsEmpty.alpha = 0f;
		}

		currentState = State.Show;
		myCanvasGroup.blocksRaycasts = true;

		if (myTweenCanvasGroup != null)
        {
            LeanTween.cancel(myCanvasGroup.gameObject, myTweenCanvasGroup.uniqueId);
            myTweenCanvasGroup = null;
        }
        myTweenCanvasGroup = LeanTween.alphaCanvas(myCanvasGroup, 1f, timeTweenMainContent).setOnComplete(() => { myTweenCanvasGroup = null; });

		if(moveTween != null){
			LeanTween.cancel(gameObject, moveTween.uniqueId);
			moveTween = null;
		}
		moveTween = LeanTween.moveLocalX(gameObject, showPosX, timeTweenMainContent).setEase(LeanTweenType.easeOutSine).setOnComplete(()=>{
			moveTween = null;
		});
		return null;
	}

	public override Coroutine Hide (){
		if(currentState == State.Hide){
			return null;
		}
		currentState = State.Hide;
		myCanvasGroup.blocksRaycasts = false;

		if (myTweenCanvasGroup != null)
        {
            LeanTween.cancel(myCanvasGroup.gameObject, myTweenCanvasGroup.uniqueId);
            myTweenCanvasGroup = null;
        }
        myTweenCanvasGroup = LeanTween.alphaCanvas(myCanvasGroup, 0f, timeTweenMainContent).setOnComplete(() => { 
            myTweenCanvasGroup = null; 
        });

		if(moveTween != null){
			LeanTween.cancel(gameObject, moveTween.uniqueId);
			moveTween = null;
		}
		moveTween = LeanTween.moveLocalX(gameObject, hidePosX, timeTweenMainContent).setEase(LeanTweenType.easeOutSine).setOnComplete(()=>{
			moveTween = null;
			
			ResetData();
		});
		return null;
	}

	public void TogglePanelHistory(){
		if(Poker_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
			MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
			MyAudioManager.instance.PlaySfx(Poker_GamePlay_Manager.instance.myAudioInfo.sfx_TogglePanel);
		}
		if(currentState == State.Show){
			Hide();
		}else{
			InitData();
			Show();
		}
	}
}
