using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Uno_PanelPlayerBetInfo_Controller : MonoBehaviour {
	
	public enum State{
		Hide, Show
	}
	public State currentState{get;set;}

	[SerializeField] CanvasGroup myCanvasGroup;
	public Image imgIconChip;
	[SerializeField] Text txtMyBet;

	long realBet, virtualBet;
	IEnumerator actionTweenBet;

	private void Awake() {
		currentState = State.Hide;

		myCanvasGroup.alpha = 0f;
		myCanvasGroup.blocksRaycasts = false;

		realBet = 0;
		virtualBet = 0;

		txtMyBet.text = "0";
	}

	void ResetData(){
		StopAllCoroutines();
		LeanTween.cancel(gameObject);
		actionTweenBet = null;

		realBet = 0;
		virtualBet = 0;

		txtMyBet.text = "0";
	}

	public void SetBet(long _bet, bool _updateNow = true){
		realBet = _bet;
		if(_updateNow){
			if(actionTweenBet != null){
				StopCoroutine(actionTweenBet);
				actionTweenBet = null;
			}
			virtualBet = realBet;
			txtMyBet.text = MyConstant.GetMoneyString(virtualBet, 9999);
		}else{
			if(actionTweenBet != null){
				StopCoroutine(actionTweenBet);
				actionTweenBet = null;
			}
			actionTweenBet = MyConstant.TweenValue(virtualBet, realBet, 5, (_valueUpdate)=>{
				virtualBet = _valueUpdate;
				txtMyBet.text = MyConstant.GetMoneyString(virtualBet, 9999);
			}, (_valueFinish)=>{
				virtualBet = _valueFinish;
				txtMyBet.text = MyConstant.GetMoneyString(virtualBet, 9999);
				actionTweenBet = null;
			});
			StartCoroutine(actionTweenBet);
		}
	}

	public Coroutine Show(bool _updateNow = true){
		currentState = State.Show;
		if(_updateNow){
			transform.localScale = Vector3.one;
			myCanvasGroup.alpha = 1f;
			return null;
		}
		return StartCoroutine(DoAcionShow(false));
	}

	public IEnumerator DoAcionShow(bool _updateNow = true){
		currentState = State.Show;
		if(_updateNow){
			transform.localScale = Vector3.one;
			myCanvasGroup.alpha = 1f;
			yield break;
		}
		bool _isFinished = false;
		float _time = 0.2f;
		transform.localScale = Vector3.one * 0.5f;
		LeanTween.alphaCanvas(myCanvasGroup, 1f, _time/2);
		LeanTween.scale(gameObject, Vector3.one, _time).setEase(LeanTweenType.easeOutBack).setOnComplete(()=>{
			_isFinished = true;
		});
		yield return new WaitUntil(()=>_isFinished);
	}

	public Coroutine Hide(bool _updateNow = true){
		currentState = State.Hide;
		if(_updateNow){
			myCanvasGroup.alpha = 0f;
			ResetData();
			return null;
		}
		return StartCoroutine(DoAcionHide(false));
	}

	public IEnumerator DoAcionHide(bool _updateNow = true){
		currentState = State.Hide;
		if(_updateNow){
			myCanvasGroup.alpha = 0f;
			ResetData();
			yield break;
		}
		bool _isFinished = false;
		float _time = 0.2f;
		LeanTween.alphaCanvas(myCanvasGroup, 0f, _time/2);
		LeanTween.scale(gameObject, Vector3.one * 0.5f, _time).setEase(LeanTweenType.easeInBack).setOnComplete(()=>{
			_isFinished = true;
		});
		yield return new WaitUntil(()=>_isFinished);
		ResetData();
	}
}
