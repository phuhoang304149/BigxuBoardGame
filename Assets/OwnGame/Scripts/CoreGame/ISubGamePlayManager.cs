using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ISubGamePlayManager : MonoBehaviour {
	
	public enum State{
		Hide, Show
	}
	public State currentState{get;set;}
	[SerializeField] protected Canvas myCanvas;
	[SerializeField] protected CanvasGroup myCanvasGroup;
	[SerializeField] protected Transform myContainer;
	[SerializeField] protected Button btnClose;
	[SerializeField] protected Transform btnClose_PlaceHolder;

	public GameObject panelLoading;
	public bool isFullScreen{get;set;}
	public float ratioScale{get;set;}
	public System.Action onPressBack;

	private void OnEnable() {
		if(myCanvas.worldCamera == null && CoreGameManager.instance.currentSceneManager != null){
			myCanvas.worldCamera = CoreGameManager.instance.currentSceneManager.cameraForConsumableScreen.mainCamera;
		}else{
			myCanvas.worldCamera = Camera.main;
		}
		currentState = State.Hide;
		myCanvasGroup.alpha = 0f;
		myCanvasGroup.blocksRaycasts = false;
	}

	public virtual void InitData(bool _isFullScreen, bool _connectFirst, System.Action _onFinished = null){}

	public virtual Coroutine Show(){
		return StartCoroutine(DoActionShow());
	}
	IEnumerator DoActionShow(){
		bool _tmpFinished = false;
		LeanTween.alphaCanvas(myCanvasGroup, 1f, 0.1f).setOnComplete(()=>{
			_tmpFinished = true;
		});
		yield return new WaitUntil(()=>_tmpFinished);
		myCanvasGroup.blocksRaycasts = true;
	}

	public virtual Coroutine Hide (){
		return StartCoroutine(DoActionHide());
	}
	IEnumerator DoActionHide(){
		myCanvasGroup.blocksRaycasts = false;
		bool _tmpFinished = false;
		LeanTween.alphaCanvas(myCanvasGroup, 0f, 0.1f).setOnComplete(()=>{
			_tmpFinished = true;
		});
		yield return new WaitUntil(()=>_tmpFinished);
		if(CoreGameManager.instance.currentSceneManager != null
			&& CoreGameManager.instance.currentSceneManager.mySceneType != IMySceneManager.Type.Home){
			CoreGameManager.instance.currentSceneManager.RefreshAgainWhenCloseSubGamePlay();
		}
		Destroy(gameObject);
	}

	public virtual void LeftGameAndHide(){}

}
