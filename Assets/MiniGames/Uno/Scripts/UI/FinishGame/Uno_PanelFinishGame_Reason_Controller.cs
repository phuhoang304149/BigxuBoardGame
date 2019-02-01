using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Uno_PanelFinishGame_Reason_Controller : MonoBehaviour {
	enum State{
		Hide, Show
	}
	State currentState;

	[SerializeField] CanvasGroup myCanvasGroup;
	[SerializeField] Text myText;

	LTDescr tweenCanvasGroup, tweenMyText;
	IEnumerator actionShowOrHide; 

	private void Awake() {
		currentState = State.Hide;
		myCanvasGroup.alpha = 0f;
		myCanvasGroup.blocksRaycasts = false;
		myText.transform.localScale = Vector3.one * 0.5f;
		myText.text = string.Empty;
	}
	
	public Coroutine Show(UnoGamePlayData.Uno_FinishGame_Data _finishGameData){
		if(currentState == State.Show){
			return null;
		}
		currentState = State.Show;
		myCanvasGroup.blocksRaycasts = true;
		myText.transform.localScale = Vector3.one * 0.5f;

		bool _isMePlayingAndWin = false;
		for(int i = 0; i < _finishGameData.listPlayersData.Count; i++){
			if(_finishGameData.listPlayersData[i].isWin){
				UnoGamePlayData.Uno_FinishGame_Data.Player_Data _playerFinish = _finishGameData.listPlayersData[i];
				UnoGamePlayData.Uno_PlayerPlayingData _playerPlayingData = Uno_GamePlay_Manager.instance.unoGamePlayData.listPlayerPlayingData[_playerFinish.indexCircle];
				if(_playerPlayingData.isMe
					&& Uno_GamePlay_Manager.instance.unoGamePlayData.CheckIsPlaying(_playerPlayingData.userData.sessionId)){
					_isMePlayingAndWin = true;
				}
			}
		}

		string _stResult = string.Empty;

		switch(_finishGameData.reasonFinish){
		case UnoGamePlayData.Uno_FinishGame_Data.Reason.PlayerWin:
			if(_isMePlayingAndWin){
				_stResult = MyLocalize.GetString("Uno/FinishGame_Reason_Victory");
			}else{
				_stResult = MyLocalize.GetString("Uno/FinishGame_Reason_Finish");
			}
			break;
		case UnoGamePlayData.Uno_FinishGame_Data.Reason.OnePlayerInTable:
			_stResult = MyLocalize.GetString("Uno/FinishGame_Reason_Finish");
			break;
		case UnoGamePlayData.Uno_FinishGame_Data.Reason.NoGlobalCards:
			_stResult = MyLocalize.GetString("Uno/FinishGame_Reason_NoGlobalCards");
			break;
		case UnoGamePlayData.Uno_FinishGame_Data.Reason.TimeOut:
			_stResult = MyLocalize.GetString("Uno/FinishGame_Reason_TimeOut");
			break;
		default:
			_stResult = MyLocalize.GetString("Uno/FinishGame_Reason_Finish");
			break;
		}
		myText.text = _stResult.ToUpper();

		if(tweenCanvasGroup != null){
			LeanTween.cancel(tweenCanvasGroup.uniqueId);
			tweenCanvasGroup = null;
		}
		if(tweenMyText != null){
			LeanTween.cancel(tweenMyText.uniqueId);
			tweenMyText = null;
		}
		if(actionShowOrHide != null){
			StopCoroutine(actionShowOrHide);
			actionShowOrHide = null;
		}
		actionShowOrHide = DoActionShow();
		return StartCoroutine(actionShowOrHide);
	}

	IEnumerator DoActionShow(){
		bool _isFinished = false;
		tweenCanvasGroup = LeanTween.alphaCanvas(myCanvasGroup, 1f, 0.1f).setOnComplete(()=>{
			tweenCanvasGroup = null;
		});
		tweenMyText = LeanTween.scale(myText.gameObject, Vector3.one, 0.4f).setEase(LeanTweenType.easeOutBack).setOnComplete(()=>{
			tweenMyText = null;
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
		if(tweenMyText != null){
			LeanTween.cancel(tweenMyText.uniqueId);
			tweenMyText = null;
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
		tweenMyText = LeanTween.scale(myText.gameObject, Vector3.zero, 0.4f).setEase(LeanTweenType.easeInBack).setOnComplete(()=>{
			tweenMyText = null;
			tweenCanvasGroup = LeanTween.alphaCanvas(myCanvasGroup, 0f, 0.1f).setOnComplete(()=>{
				tweenCanvasGroup = null;
				myCanvasGroup.blocksRaycasts = false;
				_isFinished = true;
			});
		});
		yield return new WaitUntil(()=>_isFinished);
	}

	// [ContextMenu("TEST Show")]
	// public void TestShow(){
	// 	Show(UnoGamePlayData.Uno_FinishGame_Data.Reason.PlayerWin);
	// }

	// [ContextMenu("TEST Hide")]
	// public void TestHide(){
	// 	Hide();
	// }
}
