using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;

public class Uno_PanelHistory_Controller : MonoBehaviour {

	public enum State{
		Hide, Show
	}
	public State currentState{get;set;}

	[SerializeField] CanvasGroup myCanvasGroup;
	[SerializeField] Transform mainContent;
	[SerializeField] Transform panelDetailContainer;
	[SerializeField] Text txtPointTitle;
	[SerializeField] Text txtEmpty;
	
	[Header("Prefabs")]
	[SerializeField] GameObject historyOptionType00Prefab;
	[SerializeField] GameObject historyOptionType01Prefab;

	LTDescr tweenCanvasGroup, tweenMainContent;
	IEnumerator actionShowOrHide;
	public MySimplePoolManager historyOptionPoolManager;

	UnoGamePlayData.Uno_FinishGame_Data finishGameData;
	List<UnoGamePlayData.Uno_PlayerPlayingData> listPlayerPlayingData;
	long idDataShow = -1;

	private void Awake() {
		currentState = State.Hide;
		myCanvasGroup.alpha = 0f;
		myCanvasGroup.blocksRaycasts = false;
		historyOptionPoolManager = new MySimplePoolManager();
	}

	public void InitData (UnoGamePlayData.Uno_FinishGame_Data _finishGameData, List<UnoGamePlayData.Uno_PlayerPlayingData> _listPlayerPlayingData){
		if(_finishGameData == null || _listPlayerPlayingData == null || _listPlayerPlayingData.Count == 0){
			return;
		}
		finishGameData = _finishGameData;
		if(listPlayerPlayingData == null){
			listPlayerPlayingData = new List<UnoGamePlayData.Uno_PlayerPlayingData>();
		}else{
			listPlayerPlayingData.Clear();
		}
		for(int i = 0; i < _listPlayerPlayingData.Count; i ++){
			listPlayerPlayingData.Add(_listPlayerPlayingData[i]);
		}
	}

	public Coroutine Show(){
		if(currentState == State.Show){
			return null;
		}
		currentState = State.Show;
		myCanvasGroup.alpha = 0f;
		myCanvasGroup.blocksRaycasts = true;
		mainContent.localScale = Vector3.one * 0.5f;
		
		if(tweenCanvasGroup != null){
			LeanTween.cancel(tweenCanvasGroup.uniqueId);
			tweenCanvasGroup = null;
		}
		if(tweenMainContent != null){
			LeanTween.cancel(tweenMainContent.uniqueId);
			tweenMainContent = null;
		}
		
		if(actionShowOrHide != null){
			StopCoroutine(actionShowOrHide);
			actionShowOrHide = null;
		}

		if(finishGameData != null
			&& idDataShow != finishGameData.id){
			historyOptionPoolManager.ClearAllObjectsNow();
			for(int i = 0; i < finishGameData.listPlayersData.Count; i++){
				// Debug.Log(">>>>>>>>>>>>>>>>>> " + finishGameData.listPlayersData[i].indexCircle + "|" + listPlayerPlayingData.Count);
				UnoGamePlayData.Uno_PlayerPlayingData _playerFinishData = listPlayerPlayingData[finishGameData.listPlayersData[i].indexCircle];
				long _goldBonus = finishGameData.goldWin;
				if(finishGameData.listPlayersData[i].ownCards.Count == 0){
					Uno_History_OptionType00_Controller _option = LeanPool.Spawn(historyOptionType00Prefab, Vector3.zero, Quaternion.identity, panelDetailContainer).GetComponent<Uno_History_OptionType00_Controller>();
					_option.InitData(_playerFinishData.userData, finishGameData.listPlayersData[i].isWin, _goldBonus, finishGameData.listPlayersData[i].totalPoint);
					historyOptionPoolManager.AddObject(_option);
				}else{
					if(!finishGameData.listPlayersData[i].isWin){
						_goldBonus = _playerFinishData.totalBet;
					}
					Uno_History_OptionType01_Controller _option = LeanPool.Spawn(historyOptionType01Prefab, Vector3.zero, Quaternion.identity, panelDetailContainer).GetComponent<Uno_History_OptionType01_Controller>();
					_option.InitData(_playerFinishData.userData, finishGameData.listPlayersData[i].isWin, _goldBonus, finishGameData.listPlayersData[i].totalPoint, finishGameData.listPlayersData[i].ownCards);
					historyOptionPoolManager.AddObject(_option);
				}
			}
			idDataShow = finishGameData.id;
		}

		if(finishGameData == null
			|| finishGameData.listPlayersData.Count == 0){
			txtPointTitle.gameObject.SetActive(false);
			txtEmpty.gameObject.SetActive(true);
		}else{
			txtPointTitle.gameObject.SetActive(true);
			txtEmpty.gameObject.SetActive(false);
		}

		actionShowOrHide = DoActionShow();
		return StartCoroutine(actionShowOrHide);
	}

	IEnumerator DoActionShow(){
		bool _isFinished = false;
		tweenCanvasGroup = LeanTween.alphaCanvas(myCanvasGroup, 1f, 0.1f).setOnComplete(()=>{
			tweenCanvasGroup = null;
		});
		tweenMainContent = LeanTween.scale(mainContent.gameObject, Vector3.one, 0.2f).setEase(LeanTweenType.easeOutBack).setOnComplete(()=>{
			tweenMainContent = null;
			_isFinished = true;
		});
		yield return new WaitUntil(()=>_isFinished);
		actionShowOrHide = null;
	}

	public Coroutine Hide(){
		if(currentState == State.Hide){
			return null;
		}
		currentState = State.Hide;

		if(tweenCanvasGroup != null){
			LeanTween.cancel(tweenCanvasGroup.uniqueId);
			tweenCanvasGroup = null;
		}
		if(tweenMainContent != null){
			LeanTween.cancel(tweenMainContent.uniqueId);
			tweenMainContent = null;
		}
		if(actionShowOrHide != null){
			StopCoroutine(actionShowOrHide);
			actionShowOrHide = null;
		}
		actionShowOrHide = DoActionHide();
		return StartCoroutine(actionShowOrHide);
	}

	IEnumerator DoActionHide(){
		bool _isFinished = false;
		tweenCanvasGroup = LeanTween.alphaCanvas(myCanvasGroup, 0f, 0.2f).setOnComplete(()=>{
			tweenCanvasGroup = null;
			myCanvasGroup.blocksRaycasts = false;
			_isFinished = true;
		});
		tweenMainContent = LeanTween.scale(mainContent.gameObject, Vector3.one * 0.5f, 0.2f).setEase(LeanTweenType.easeInBack).setOnComplete(()=>{
			tweenMainContent = null;
		});
		yield return new WaitUntil(()=>_isFinished);
	}

	public void OnButtonHideClicked(){
		Hide();
	}

	public void TogglePanelHistory(){
		if(Uno_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
			MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
		}
		if(currentState == State.Show){
			if(Uno_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
				MyAudioManager.instance.PlaySfx(Uno_GamePlay_Manager.instance.myAudioInfo.sfx_HidePanel);	
			}
			Hide();
		}else{
			if(Uno_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
				MyAudioManager.instance.PlaySfx(Uno_GamePlay_Manager.instance.myAudioInfo.sfx_OpenScoreBoard);	
			}
			Show();
		}
	}
}
