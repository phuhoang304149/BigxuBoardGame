using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Poker_MyBar_Button_Controller : MonoBehaviour, IPointerUpHandler, IPointerExitHandler, IPointerDownHandler {
	[SerializeField] CanvasGroup myCanvasGroup;
	[SerializeField] CanvasGroup canvasGroupShadow;
	[SerializeField] Text txtContent;
    [SerializeField] UnityEvent onClick;
    protected bool isOnPointerExit, isPressed;
	LTDescr tweenMyCanvasGroup, tweenCanvasGroupShadow;
	System.DateTime timeToSetActionClicked;

	public bool interactable{
		get{
			return _interactable;
		}
	}
	bool _interactable;

	private void Awake() {
		SetInteractable(true);

		isOnPointerExit = false;
		isPressed = false;
		timeToSetActionClicked = System.DateTime.Now;
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
		isOnPointerExit = false;
		isPressed = false;
		timeToSetActionClicked = System.DateTime.Now;
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

	public void SetTextContent(string _text){
		txtContent.text = _text;
	}

	public string GetTextContent(){
		return txtContent.text;
	}

	public virtual void OnPointerDown(PointerEventData eventData){
// #if TEST
//         Debug.Log("Pressed");
// #endif

        isPressed = true;
        //TODO: Play Animation
		
		if(tweenMyCanvasGroup != null){
			LeanTween.cancel(tweenMyCanvasGroup.uniqueId);
		}

		tweenMyCanvasGroup = LeanTween.alphaCanvas(myCanvasGroup, 1f, 0.1f).setOnComplete(()=>{
			tweenMyCanvasGroup = null;
		});
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (!isPressed)
        {
            return;
        }
// #if TEST
//         Debug.Log("isOnPointerExit : " + isOnPointerExit);
// #endif
        isOnPointerExit = true;
    }

    //Do this when the mouse click on this selectable UI object is released.
    public virtual void OnPointerUp(PointerEventData eventData)
    {
		if(tweenMyCanvasGroup != null){
			LeanTween.cancel(tweenMyCanvasGroup.uniqueId);
		}
		tweenMyCanvasGroup = LeanTween.alphaCanvas(myCanvasGroup, 0.8f, 0.1f).setOnComplete(()=>{
			tweenMyCanvasGroup = null;
		});

        if (isOnPointerExit || !isPressed)
        {
// #if TEST
            Debug.Log("OnPointerUp return");
// #endif
            isPressed = false;
            isOnPointerExit = false;
            return;
        }
// #if TEST
//         Debug.Log("The mouse click was released");
// #endif

		isPressed = false;
		isOnPointerExit = false;
		if(System.DateTime.Now >= timeToSetActionClicked){
			timeToSetActionClicked.AddSeconds(0.5);
			if (onClick != null){
				onClick.Invoke();
			}
		}
    }
}
