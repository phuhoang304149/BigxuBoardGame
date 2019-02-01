using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class Poker_PanelSupport_Controller : MySimplePanelController {

	public enum State{
		Hide, Show
	}
	public State currentState{get;set;}

	[SerializeField] CanvasGroup myCanvasGroup;
	[SerializeField] List<Poker_SupportOptionInfo_Controller> listOptions;

	[Header("Setting")]
	[SerializeField] float timeTweenMainContent;

	PokerGamePlayData.Poker_PlayerPlayingData dataPlaying;
	LTDescr myTweenCanvasGroup;
	float[] valueCardRanking;
	private PokerGamePlayData pokerGamePlayData{
		get{
			return Poker_GamePlay_Manager.instance.pokerGamePlayData;
		}
	}

	void Awake(){
		valueCardRanking = new float [listOptions.Count];
		currentState = State.Hide;
		myCanvasGroup.alpha = 0f;
		myCanvasGroup.blocksRaycasts = false;

		for(int i = 0; i < listOptions.Count; i++){
			listOptions[i].ResetData();
			valueCardRanking[i] = 0f;
		}
	}

	public override void ResetData (){
		if(dataPlaying == null){
			return;
		}
		dataPlaying = null;

		for(int i = 0; i < listOptions.Count; i++){
			listOptions[i].ResetData();
			valueCardRanking[i] = 0f;
		}
	}

	public void InitData (PokerGamePlayData.Poker_PlayerPlayingData _data){
		dataPlaying = _data;
		for(int i = 0; i < listOptions.Count; i++){
			listOptions[i].ResetData();
		}
	}

	public void RefreshData (bool _hasNewTurn){
		if(pokerGamePlayData == null  || dataPlaying == null
			|| pokerGamePlayData.currentGameState == PokerGamePlayData.GameState.STATUS_WAIT_FOR_PLAYER
			|| dataPlaying.currentState == PokerGamePlayData.Poker_PlayerPlayingData.State.STATEPOKER_FOLD){
			return;
		}
		if(dataPlaying.ownCards == null || dataPlaying.ownCards.Count != 2 || dataPlaying.ownCards[0] < 0 || dataPlaying.ownCards[1] < 0 || pokerGamePlayData.globalCards == null){
			return;
		}

		if(_hasNewTurn && pokerGamePlayData.globalCards.Count >= 3){
			PokerGamePlayData.GetPercentTypeCard(dataPlaying.ownCards, pokerGamePlayData.globalCards, (_percent)=>{
				if(listOptions.Count != _percent.Length){
					#if TEST
					Debug.LogError("BUG Logic!");
					#endif
				}else{
					for(int i = 0; i < _percent.Length; i++){
						valueCardRanking[i] = _percent[i];
						listOptions[i].InitData(_percent[i]);
					}
				}
			});
		}
	}

	public override Coroutine Show (){
		if(currentState == State.Show){
			return null;
		}

		currentState = State.Show;
		if (myTweenCanvasGroup != null){
            LeanTween.cancel(myTweenCanvasGroup.uniqueId);
            myTweenCanvasGroup = null;
        }
        myTweenCanvasGroup = LeanTween.alphaCanvas(myCanvasGroup, 0.85f, timeTweenMainContent).setOnComplete(() => { myTweenCanvasGroup = null; });
		return null;
	}

	public override Coroutine Hide (){
		if(currentState == State.Hide){
			return null;
		}
		currentState = State.Hide;

		if (myTweenCanvasGroup != null){
            LeanTween.cancel(myTweenCanvasGroup.uniqueId);
            myTweenCanvasGroup = null;
        }
        myTweenCanvasGroup = LeanTween.alphaCanvas(myCanvasGroup, 0f, timeTweenMainContent).setOnComplete(() => { myTweenCanvasGroup = null; });
		return null;
	}
	
	public void TogglePanel(){
		if(Poker_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
			MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
			MyAudioManager.instance.PlaySfx(Poker_GamePlay_Manager.instance.myAudioInfo.sfx_TogglePanel);
		}
		if(currentState == State.Show){
			Hide();
		}else{
			Show();
		}
	}
}
