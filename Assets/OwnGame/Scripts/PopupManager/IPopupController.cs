using UnityEngine;
using System.Collections;
using Lean.Pool;

public class IPopupController : MySimplePoolObjectController {

	[SerializeField] protected CanvasGroup myCanvasGroup;
	[SerializeField] protected Transform mainContainer;
	public System.Action onClose;

	LTDescr tweenCanvasGroup, tweenMainContainer;

	public override void ResetData(){
		onClose = null;
		if(tweenCanvasGroup != null){
			LeanTween.cancel(tweenCanvasGroup.uniqueId);
			tweenCanvasGroup = null;
		}

		if(tweenMainContainer != null){
			LeanTween.cancel(tweenMainContainer.uniqueId);
			tweenMainContainer = null;
		}
	}

	public virtual void Show(){
		myCanvasGroup.alpha = 0f;
		myCanvasGroup.blocksRaycasts = true;

		if(tweenCanvasGroup != null){
			LeanTween.cancel(tweenCanvasGroup.uniqueId);
			tweenCanvasGroup = null;
		}
		tweenCanvasGroup = LeanTween.alphaCanvas(myCanvasGroup, 1f, 0.2f).setEase(LeanTweenType.easeOutBack).setOnComplete(()=>{
			tweenCanvasGroup = null;
		});

		if(tweenMainContainer != null){
			LeanTween.cancel(tweenMainContainer.uniqueId);
			tweenMainContainer = null;
		}
		mainContainer.localScale = Vector3.one * 0.6f;
		tweenMainContainer = LeanTween.scale(mainContainer.gameObject, Vector3.one, 0.2f).setEase(LeanTweenType.easeOutBack).setOnComplete(()=>{
			tweenMainContainer = null;
		});
	}

	public virtual void Hide(System.Action _onFinished = null){
		myCanvasGroup.blocksRaycasts = false;
		if(tweenCanvasGroup != null){
			LeanTween.cancel(tweenCanvasGroup.uniqueId);
			tweenCanvasGroup = null;
		}
		tweenCanvasGroup = LeanTween.alphaCanvas(myCanvasGroup, 0f, 0.2f).setOnComplete(()=>{
			tweenCanvasGroup = null;
			if(_onFinished != null){
				_onFinished();
			}
		}).setEase(LeanTweenType.easeInBack);

		if(tweenMainContainer != null){
			LeanTween.cancel(tweenMainContainer.uniqueId);
			tweenMainContainer = null;
		}
		tweenMainContainer = LeanTween.scale(mainContainer.gameObject, Vector3.one * 0.6f, 0.2f).setEase(LeanTweenType.easeInBack).setOnComplete(()=>{
			tweenMainContainer = null;
		});
	}

	public virtual void Close(){}
}
