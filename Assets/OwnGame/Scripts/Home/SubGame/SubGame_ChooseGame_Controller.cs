using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubGame_ChooseGame_Controller : MySimplePanelController {

	[SerializeField] CanvasGroup myCanvasGroup;
	[SerializeField] Transform panelBg;
	[SerializeField] RectTransform panelIconSubGame;
	[SerializeField] List<SubGame_ChooseGame_Option_Controller> listGames;

	[Header("Setting")]
	[SerializeField] float timeShowScreen;
	[SerializeField] float timeTweenListGame;
	[SerializeField] float timeHideScreen;

	bool isInitialized;

	public override void ResetData(){
		StopAllCoroutines();
		myCanvasGroup.alpha = 0f;
		myCanvasGroup.blocksRaycasts = false;
		isInitialized = false;
	}

	public override void InitData(System.Action _onFinished = null){
		if(!isInitialized){
			for(int i = 0; i < listGames.Count; i ++){
				listGames[i].InitData(ChooseSubGameScreenController.instance.OnChooseGame);
			}
			isInitialized = true;
		}
	}

	public override Coroutine Show(){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_TogglePanel);

		myCanvasGroup.blocksRaycasts = true;
		return StartCoroutine(DoActionShow());
	}
	IEnumerator DoActionShow(){
		bool _tmpFinished = false;
		LeanTween.alphaCanvas(myCanvasGroup, 1f, timeShowScreen).setEase(LeanTweenType.easeOutBack);

		LeanTween.alpha(panelIconSubGame, 1f, timeTweenListGame).setEase(LeanTweenType.easeOutBack);
		panelBg.localScale = Vector3.zero;
		LeanTween.scale(panelBg.gameObject, Vector3.one, timeTweenListGame).setEase(LeanTweenType.easeOutBack).setOnComplete(()=>{
			_tmpFinished = true;
		});
		for(int i = 0; i < listGames.Count; i ++){
			listGames[i].Show(timeTweenListGame);
		}
		yield return new WaitUntil(()=>_tmpFinished);
	}

	public override Coroutine Hide(){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_TogglePanel);

		myCanvasGroup.blocksRaycasts = false;
		return StartCoroutine(DoActionHide());
	}

	IEnumerator DoActionHide(){
		bool _tmpFinished = false;
		LeanTween.alphaCanvas(myCanvasGroup, 0f, timeHideScreen).setEase(LeanTweenType.easeInBack).setDelay(timeTweenListGame - timeShowScreen);
		
		LeanTween.alpha(panelIconSubGame, 0f, timeTweenListGame).setEase(LeanTweenType.easeInBack);
		LeanTween.scale(panelBg.gameObject, Vector3.zero, timeTweenListGame).setEase(LeanTweenType.easeInBack).setOnComplete(()=>{
			_tmpFinished = true;
		});
		for(int i = 0; i < listGames.Count; i ++){
			listGames[i].Hide(timeTweenListGame);
		}
		yield return new WaitUntil(()=>_tmpFinished);
	}

	public Coroutine QuickHide(){
		myCanvasGroup.blocksRaycasts = false;
		return StartCoroutine(DoActionQuickHide());
	}

	IEnumerator DoActionQuickHide(){
		bool _tmpFinished = false;
		LeanTween.alphaCanvas(myCanvasGroup, 0f, timeHideScreen).setEase(LeanTweenType.easeInBack).setOnComplete(()=>{
			_tmpFinished = true;
		});
		yield return new WaitUntil(()=>_tmpFinished);
	}
}
