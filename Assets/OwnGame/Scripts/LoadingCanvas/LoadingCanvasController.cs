using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingCanvasController : MonoBehaviour {

	static LoadingCanvasController ins;
	public static LoadingCanvasController instance{
		get{ 
			return ins;
		}
	}

	public enum State{
		Show, 
		Hide
	}
	public State currentState{ get; set;}

	[Header("Setting")]
	[SerializeField] CanvasGroup myCanvasGroup;
	[SerializeField] Canvas myCanvas;
	[SerializeField] ParticleSystem loadingParticle;
	IEnumerator actionWaitToHide;
	System.Action onFinishedTimeOut, onForcedHideNow;

	bool isLock, canPressButtonHide;

	void Awake(){
		if (ins != null && ins != this) {
			Destroy(this.gameObject);
			return;
		}
		ins = this;
		DontDestroyOnLoad (this.gameObject);

		Hide (true);
	}

	[ContextMenu("Show")]
	void TESTSHOW(){
		Show();
	}
	public void Show(float _timeOut = -1f, bool _isLock = false, System.Action _onFinishedTimeOut = null, System.Action _onForcedHideNow = null){
		if(currentState == State.Show){
			return;
		}
		if(myCanvas.worldCamera == null && CoreGameManager.instance.currentSceneManager != null){
			myCanvas.worldCamera = CoreGameManager.instance.currentSceneManager.cameraForConsumableScreen.mainCamera;
		}else{
			myCanvas.worldCamera = Camera.main;
		}
		isLock = _isLock;
		onForcedHideNow = _onForcedHideNow;
		onFinishedTimeOut = _onFinishedTimeOut;
		myCanvasGroup.alpha = 1f;
		myCanvasGroup.blocksRaycasts = true;
		loadingParticle.gameObject.SetActive (true);
		currentState = State.Show;
		
		if(_timeOut > 0f){
			if(actionWaitToHide != null){
				StopCoroutine(actionWaitToHide);
				actionWaitToHide = null;
			}
			actionWaitToHide = DoActionWaitToHide(_timeOut);
			StartCoroutine(actionWaitToHide);
		}
		Invoke("SetCanPressButtonHide", 0.5f);
	}

	void SetCanPressButtonHide(){
		canPressButtonHide = true;
	}

	IEnumerator DoActionWaitToHide(float _timeOut){
		yield return Yielders.Get(_timeOut);
		if(onFinishedTimeOut != null){
			onFinishedTimeOut();
			onFinishedTimeOut = null;
		}
		Hide();
	}

	public void OnButtonHide(){
		if(!canPressButtonHide){
			return;
		}
		if(isLock){
			return;
		}
		if(onForcedHideNow != null){
			onForcedHideNow();
			onForcedHideNow = null;
		}
		Hide();
	}

	[ContextMenu("Hide")]
	public void Hide(bool _forcedHide = false){
		if(!_forcedHide){
			if(currentState == State.Hide){
				return;
			}
		}
		
		CancelInvoke();
		StopAllCoroutines();
		actionWaitToHide = null;

		myCanvasGroup.alpha = 0f;
		myCanvasGroup.blocksRaycasts = false;
		loadingParticle.gameObject.SetActive (false);
		currentState = State.Hide;

		onFinishedTimeOut = null;
		onForcedHideNow = null;

		isLock = false;
		canPressButtonHide = false;
	}
}
