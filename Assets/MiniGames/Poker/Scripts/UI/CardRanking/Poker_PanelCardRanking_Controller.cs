using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Poker_PanelCardRanking_Controller : MySimplePanelController {

	public enum State{
		Hide, Show
	}
	public State currentState{get;set;}
	[SerializeField] CanvasGroup myCanvasGroup;
	[SerializeField] List<Text> listPercentTypeCard;
	[SerializeField] RectTransform rectTransformPanelMainContainer;
	[SerializeField] ScrollRect myScrollRectMainContent;

	[Header("Setting")]
	[SerializeField] float timeTweenMainContent;
	
	LTDescr moveTween, myTweenCanvasGroup;
	float showPosX, hidePosX;

	private void Awake() {
		currentState = State.Hide;
		myCanvasGroup.alpha = 0f;
		myCanvasGroup.blocksRaycasts = false;
	}

	IEnumerator Start(){
		yield return Yielders.EndOfFrame;
		float _sizeW = rectTransformPanelMainContainer.sizeDelta.x;
		showPosX = 0f - _sizeW - 60f;
		hidePosX = 0f + 60f;
	}

	public override Coroutine Show (){
		if(currentState == State.Show){
			return null;
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
		});
		return null;
	}

	public void TogglePanel(){
		if(Poker_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
			MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
			MyAudioManager.instance.PlaySfx(Poker_GamePlay_Manager.instance.myAudioInfo.sfx_TogglePanel);
		}
		if(currentState == State.Show){
			Hide();
		}else{
			Show();
		}
	}
}
