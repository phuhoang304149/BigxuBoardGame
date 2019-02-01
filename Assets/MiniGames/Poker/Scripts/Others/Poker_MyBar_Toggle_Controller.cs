using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Poker_MyBar_Toggle_Controller : MonoBehaviour , IPointerUpHandler, IPointerDownHandler {

	[SerializeField] CanvasGroup myCanvasGroup;
	[SerializeField] CanvasGroup canvasGroupShadow;
	public Toggle myToggle;

	LTDescr tweenMyCanvasGroup, tweenCanvasGroupShadow;

	public bool interactable{
		get{
			return _interactable;
		}
	}
	bool _interactable;

	private void Awake() {
		SetInteractable(true);
	}

	private void OnDisable() {
		if(tweenMyCanvasGroup != null){
			LeanTween.cancel(tweenMyCanvasGroup.uniqueId);
			tweenMyCanvasGroup = null;
		}
		if(tweenCanvasGroupShadow != null){
			LeanTween.cancel(tweenCanvasGroupShadow.uniqueId);
			tweenCanvasGroupShadow = null;
		}
	}

	public void SetInteractable(bool _flag, bool _updateNow = true){
		if(tweenMyCanvasGroup != null){
			LeanTween.cancel(tweenMyCanvasGroup.uniqueId);
			tweenMyCanvasGroup = null;
		}
		if(tweenCanvasGroupShadow != null){
			LeanTween.cancel(tweenCanvasGroupShadow.uniqueId);
			tweenCanvasGroupShadow = null;
		}

		_interactable = _flag;

		float _alphaMyCanvasGroup = 0f;
		float _alphaCanvasGroupShadow = 0f;
		if(interactable){
			_alphaMyCanvasGroup = 0.8f;
			_alphaCanvasGroupShadow = 0f;
			myCanvasGroup.blocksRaycasts = true;
		}else{
			_alphaMyCanvasGroup = 0.8f;
			_alphaCanvasGroupShadow = 0.8f;
			myCanvasGroup.blocksRaycasts = false;
		}
		
		if(_updateNow){
			myCanvasGroup.alpha = _alphaMyCanvasGroup;
			canvasGroupShadow.alpha = _alphaCanvasGroupShadow;
		}else{
			tweenMyCanvasGroup = LeanTween.alphaCanvas(myCanvasGroup, _alphaMyCanvasGroup, 0.1f).setOnComplete(()=>{
				tweenMyCanvasGroup = null;
			});
			tweenCanvasGroupShadow = LeanTween.alphaCanvas(canvasGroupShadow, _alphaCanvasGroupShadow, 0.1f).setOnComplete(()=>{
				tweenCanvasGroupShadow = null;
			});
		}
	}

	public virtual void OnPointerDown(PointerEventData eventData){
// #if TEST
//         Debug.Log("Pressed");
// #endif
		
		if(tweenMyCanvasGroup != null){
			LeanTween.cancel(tweenMyCanvasGroup.uniqueId);
		}

		tweenMyCanvasGroup = LeanTween.alphaCanvas(myCanvasGroup, 1f, 0.1f).setOnComplete(()=>{
			tweenMyCanvasGroup = null;
		});
    }

	public virtual void OnPointerUp(PointerEventData eventData)
    {
		// #if TEST
		//         Debug.Log("Release");
		// #endif
				
		if(tweenMyCanvasGroup != null){
			LeanTween.cancel(tweenMyCanvasGroup.uniqueId);
		}
		tweenMyCanvasGroup = LeanTween.alphaCanvas(myCanvasGroup, 0.8f, 0.1f).setOnComplete(()=>{
			tweenMyCanvasGroup = null;
		});
    }
}
