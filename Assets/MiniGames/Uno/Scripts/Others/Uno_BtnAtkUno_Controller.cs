using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Uno_BtnAtkUno_Controller : MonoBehaviour {

	public enum State{
		Hide, Show
	}
	public State currentState{get;set;}
	[SerializeField] CanvasGroup myCanvasGroup;
	[SerializeField] ParticleSystem myGlow;

	[Header("Setting")]
	[SerializeField] float timeTween;

	LTDescr tweenCanvasGroup, tweenScale;
	
	private void Awake() {
		LeanTween.cancel(gameObject);
		currentState = State.Hide;
		myCanvasGroup.alpha = 0f;
		myCanvasGroup.blocksRaycasts = false;
		transform.localScale = Vector3.one * 0.5f;
		myGlow.gameObject.SetActive(false);
	}

	[ContextMenu("Show")]
	public void Show(){
		if(currentState == State.Show){
			return;
		}
		if(tweenCanvasGroup != null){
			LeanTween.cancel(tweenCanvasGroup.uniqueId);
		}
		if(tweenScale != null){
			LeanTween.cancel(tweenScale.uniqueId);
		}

		currentState = State.Show;
		myCanvasGroup.alpha = 0f;
		transform.localScale = Vector3.one * 0.5f;

		tweenCanvasGroup = LeanTween.alphaCanvas(myCanvasGroup, 1f, timeTween).setOnComplete(()=>{
			tweenCanvasGroup = null;
		});
		tweenScale = LeanTween.scale(gameObject, Vector3.one, timeTween).setEase(LeanTweenType.easeOutBack).setOnComplete(()=>{
			myGlow.gameObject.SetActive(true);
			myGlow.Play();
			myCanvasGroup.blocksRaycasts = true;
			tweenScale = null;
		});
	}

	public void Hide(){
		if(currentState == State.Hide){
			return;
		}
		if(tweenCanvasGroup != null){
			LeanTween.cancel(tweenCanvasGroup.uniqueId);
		}
		if(tweenScale != null){
			LeanTween.cancel(tweenScale.uniqueId);
		}
		currentState = State.Hide;
		myGlow.gameObject.SetActive(false);
		myCanvasGroup.blocksRaycasts = false;
		tweenCanvasGroup = LeanTween.alphaCanvas(myCanvasGroup, 0f, timeTween).setOnComplete(()=>{
			tweenCanvasGroup = null;
		});
		tweenScale = LeanTween.scale(gameObject, Vector3.zero, timeTween).setEase(LeanTweenType.easeInBack).setOnComplete(()=>{
			tweenScale = null;
		});
	}
}
