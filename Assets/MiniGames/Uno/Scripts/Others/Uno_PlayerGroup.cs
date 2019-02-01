using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable] public class Uno_PlayerGroup {

	public Transform panelContainerPlayerInfo;
	public PanelPlayerInfoInGameController panelPlayerInfo{get;set;}
	public PlaceHolderPanelOtherPlayerInfo placeHolderPanelPlayerInfo{get;set;}
	public MySimplePoolManager ownCardPoolManager;
	public PanelCardUnoDetailController currentCardUnoFocusing{get;set;}
	public CanvasGroup canvasGroupBtnSitDown;
	public CanvasGroup canvasGroupBtnStandUp;
	[SerializeField] Uno_BtnAtkUno_Controller btnAtkUno;

	[Header("Panel Effect State")]
	public Uno_EffectCallUno_Controller effCallUno;
	public Uno_Effect_StateForbiden_Controller effForbiden;

	[Header("Prefabs")]
	public GameObject panelPlayerInfoPrefab;

	public int realIndex;
	public int viewIndex;

	LTDescr tweenAlphaBtnSitDown;
	LTDescr tweenAlphaBtnStandUp;

	public bool isInitialized{get;set;}

	public bool isMe{
		get{
			if(userData == null){
				return false;
			}
			if(userData.sessionId != DataManager.instance.userData.sessionId){
				return false;
			}
			return true;
		}
	}

	public UserDataInGame userData{
		get{
			return panelPlayerInfo.data;
		}
	}

	public Uno_GamePlay_Manager gamePlayManager{
		get{
			return Uno_GamePlay_Manager.instance;
		}
	}

	public void InitFirst(int _index){
		isInitialized = false;
		realIndex = _index;
		viewIndex = _index;
		ownCardPoolManager = new MySimplePoolManager();
		
		placeHolderPanelPlayerInfo = gamePlayManager.UIManager.listPlaceHolderPanelPlayerInfo_Wating[realIndex];
		panelPlayerInfo = GameObject.Instantiate(panelPlayerInfoPrefab, panelContainerPlayerInfo.transform, false).GetComponent<PanelPlayerInfoInGameController>();
		panelPlayerInfo.transform.localPosition = Vector3.zero;
		panelPlayerInfo.transform.localScale = Vector3.one * 0.9f;
		panelPlayerInfo.transform.SetAsFirstSibling();
		panelPlayerInfo.popupChatPosType = placeHolderPanelPlayerInfo.popupChatPosType;
		panelPlayerInfo.transform.parent.position = placeHolderPanelPlayerInfo.transform.position;
		panelPlayerInfo.InitAsIncognito(null, MyLocalize.GetString("Global/Empty"));
		panelPlayerInfo.SetShadow(true);
		panelPlayerInfo.Show();
		effCallUno.Hide();
		effForbiden.Hide();
		
		ShowButtonSitDown();
		HideButtonStandUp();
	}

	public void InitData(UserDataInGame _userData){
		panelPlayerInfo.InitData(_userData);
		panelPlayerInfo.SetShadow(false);
		panelPlayerInfo.Show();
		ClearAllCards();
		isInitialized = true;
	}

	public void InitAsIncognito(UserDataInGame _userData){
		panelPlayerInfo.InitAsIncognito(_userData);
		panelPlayerInfo.SetShadow(true);
		panelPlayerInfo.Show();
		ClearAllCards();
		isInitialized = true;
	}

	public void OnFocusCard(PanelCardUnoDetailController _cardUnoDetail){
		if(!_cardUnoDetail.canPut){
			return;
		}
		if(Uno_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
            MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
        }
		if(currentCardUnoFocusing != null){
			currentCardUnoFocusing.HideHighlight();
			currentCardUnoFocusing.MoveLocal(Vector2.zero, 0.2f, LeanTweenType.easeOutBack);
		}
		if(currentCardUnoFocusing != _cardUnoDetail){
			if(currentCardUnoFocusing != null){
				currentCardUnoFocusing.HideHighlight();
			}
			_cardUnoDetail.MoveLocal(Vector2.up * 30f, 0.2f, LeanTweenType.easeOutBack);
			_cardUnoDetail.ShowHighlight();
			currentCardUnoFocusing = _cardUnoDetail;
		}else{
			currentCardUnoFocusing = null;
		}

		if(currentCardUnoFocusing != null){
			gamePlayManager.UIManager.myBarController.SetBtnPut(gamePlayManager.UIManager.myBarController.alphaMyBtnWhenActive, true, false);
		}else{
			gamePlayManager.UIManager.myBarController.SetBtnPut(gamePlayManager.UIManager.myBarController.alphaMyBtnWhenNotActive, false, false);
		}
	}

	public void HideAndClear(){
		panelPlayerInfo.InitAsIncognito(null, MyLocalize.GetString("Global/Empty"));
		panelPlayerInfo.SetShadow(true);
		panelPlayerInfo.Show();
		effCallUno.Hide();
		effForbiden.Hide();
		btnAtkUno.Hide();
		ClearAllCards();
		isInitialized = false;
	}

	public void ClearAllCards(){
		ownCardPoolManager.ClearAllObjectsNow();
	}
	
	#region Refresh UI
	public void ShowButtonSitDown(bool _showNow = true){
		if(tweenAlphaBtnSitDown != null){
			LeanTween.cancel(tweenAlphaBtnSitDown.uniqueId);
			tweenAlphaBtnSitDown = null;
		}
		if(_showNow){
			canvasGroupBtnSitDown.alpha = 1f;
			canvasGroupBtnSitDown.blocksRaycasts = true;
		}else{
			canvasGroupBtnSitDown.blocksRaycasts = true;
			tweenAlphaBtnSitDown = LeanTween.alphaCanvas(canvasGroupBtnSitDown, 1f, 0.1f).setOnComplete(()=>{
				tweenAlphaBtnSitDown = null;
			});
		}
	}

	public void HideButtonSitDown(bool _showNow = true){
		if(tweenAlphaBtnSitDown != null){
			LeanTween.cancel(tweenAlphaBtnSitDown.uniqueId);
			tweenAlphaBtnSitDown = null;
		}
		if(_showNow){
			canvasGroupBtnSitDown.alpha = 0f;
			canvasGroupBtnSitDown.blocksRaycasts = false;
		}else{
			canvasGroupBtnSitDown.blocksRaycasts = false;
			tweenAlphaBtnSitDown = LeanTween.alphaCanvas(canvasGroupBtnSitDown, 0f, 0.1f).setOnComplete(()=>{
				tweenAlphaBtnSitDown = null;
			});
		}
	}
	public void ShowButtonStandUp(bool _showNow = true){
		if(viewIndex == 3){
			Vector3 _pos = panelContainerPlayerInfo.position;
			_pos.x += 0.7f;
			_pos.y += 0.35f;
			canvasGroupBtnStandUp.transform.position = _pos;
		}else{
			Vector3 _pos = panelContainerPlayerInfo.position;
			_pos.x -= 0.7f;
			_pos.y += 0.35f;
			canvasGroupBtnStandUp.transform.position = _pos;
		}
		if(tweenAlphaBtnStandUp != null){
			LeanTween.cancel(tweenAlphaBtnStandUp.uniqueId);
			tweenAlphaBtnStandUp = null;
		}
		if(_showNow){
			canvasGroupBtnStandUp.alpha = 1f;
			canvasGroupBtnStandUp.blocksRaycasts = true;
		}else{
			canvasGroupBtnStandUp.blocksRaycasts = true;
			tweenAlphaBtnStandUp = LeanTween.alphaCanvas(canvasGroupBtnStandUp, 1f, 0.1f).setOnComplete(()=>{
				tweenAlphaBtnStandUp = null;
			});
		}
	}

	public void HideButtonStandUp(bool _showNow = true){
		if(tweenAlphaBtnStandUp != null){
			LeanTween.cancel(tweenAlphaBtnStandUp.uniqueId);
			tweenAlphaBtnStandUp = null;
		}
		if(_showNow){
			canvasGroupBtnStandUp.alpha = 0f;
			canvasGroupBtnStandUp.blocksRaycasts = false;
		}else{
			if(tweenAlphaBtnStandUp != null){
				LeanTween.cancel(tweenAlphaBtnStandUp.uniqueId);
				tweenAlphaBtnStandUp = null;
			}
			canvasGroupBtnStandUp.blocksRaycasts = false;
			tweenAlphaBtnStandUp = LeanTween.alphaCanvas(canvasGroupBtnStandUp, 0f, 0.1f).setOnComplete(()=>{
				tweenAlphaBtnStandUp = null;
			});
		}
	}

	public void ShowButtonAtkUno(){
		if(btnAtkUno.currentState == Uno_BtnAtkUno_Controller.State.Show){
			return;
		}
		Vector3 _pos = Uno_GamePlay_Manager.instance.UIManager.listPanelBtnAtkUnoHolderCatched[viewIndex].position;
		btnAtkUno.transform.position = _pos;
		btnAtkUno.Show();
	}

	public void HideButtonAtkUno(){
		btnAtkUno.Hide();
	}
	#endregion
}
