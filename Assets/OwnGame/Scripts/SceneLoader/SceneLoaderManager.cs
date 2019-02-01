using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;


public class SceneLoaderManager : MonoBehaviour {

	public static SceneLoaderManager instance{
		get{
			return ins;
		}
	}
	private static SceneLoaderManager ins;

	public enum State{
		Hide,
		Show
	}
	public State currentState{ get; set;}

	[SerializeField] CanvasGroup myCanvasGroup;
	[SerializeField] Canvas myCanvas;
	// [SerializeField] GraphicRaycaster gpRaycaster;
	// [SerializeField] GameObject loadingParticle;
	

	void Awake() {
		if (ins != null && ins != this) {
			Destroy(this.gameObject);
			return;
		}
		ins = this;
		DontDestroyOnLoad (this.gameObject);

		Hide();
	}
	[ContextMenu("Show")]
	void Show(bool _updateNow = true, System.Action _onFinished = null){
        if (myCanvas.worldCamera == null){
            myCanvas.worldCamera = Camera.main;
        }
		
		currentState = State.Show;
		myCanvasGroup.blocksRaycasts = true;
		if(_updateNow){
			myCanvasGroup.alpha = 1f;
			if(_onFinished != null){
				_onFinished();
			}
		}else{
			LeanTween.alphaCanvas(myCanvasGroup, 1f, 0.2f).setOnComplete(()=>{
				if(_onFinished != null){
					_onFinished();
				}
			});
		}
		
		// loadingParticle.SetActive(true);
	}

	void Hide(bool _updateNow = true, System.Action _onFinished = null){
		if (myCanvas.worldCamera == null){
            myCanvas.worldCamera = Camera.main;
        }

		currentState = State.Hide;
		myCanvasGroup.blocksRaycasts = false;
		if(_updateNow){
			myCanvasGroup.alpha = 0f;
			if(_onFinished != null){
				_onFinished();
			}
		}else{
			LeanTween.alphaCanvas(myCanvasGroup, 0f, 0.2f).setOnComplete(()=>{
				if(_onFinished != null){
					_onFinished();
				}
			});
		}
		// loadingParticle.SetActive(false);
	}

	public Coroutine LoadScene(string _nameScene){
		return StartCoroutine(DoActionLoadScene(_nameScene));
	}

	IEnumerator DoActionLoadScene(string _nameScene){
		bool _isFinished = false;
		Show(false, ()=>{
			_isFinished = true;
		});
		yield return new WaitUntil(()=>_isFinished);
		yield return Yielders.EndOfFrame;
		
		DateTime _timeStart = DateTime.UtcNow;
		CoroutineChain.StopAll();
		LeanTween.cancelAll(true);
		CoreGameManager.instance.ClearAllCallbackPressBackKey();
		PopupManager.Instance.UnActiveAllPopups ();
		LoadingCanvasController.instance.Hide();
		GetGoldScreenController.instance.ForcedHide();
		SettingScreenController.instance.ForcedHide();
		ChooseSubGameScreenController.instance.ForcedHide();
		MyAudioManager.instance.StopAll();

		CoreGameManager.instance.currentSceneManager = null;

		// AudioManager.PauseMusic ();
		// AudioManager.instance.isStopPlayingNewSound = true;

		var asyncLoad = SceneManager.LoadSceneAsync (_nameScene, LoadSceneMode.Single);
		yield return new WaitUntil (() => asyncLoad.isDone);
		
		if(myCanvas.worldCamera == null){
			myCanvas.worldCamera = Camera.main;
		}

		yield return Resources.UnloadUnusedAssets ();

		yield return new WaitUntil (()=>CoreGameManager.instance.currentSceneManager != null && CoreGameManager.instance.currentSceneManager.canShowScene);
		
		long _timeLoadScene = (long) (DateTime.UtcNow - _timeStart).TotalMilliseconds;
		
		while(_timeLoadScene < 1000){
			yield return null;
			_timeLoadScene = (long) (DateTime.UtcNow - _timeStart).TotalMilliseconds;
		}

		// AudioManager.instance.isStopPlayingNewSound = false;
		Hide(false);
		
		yield break;
	}
}
