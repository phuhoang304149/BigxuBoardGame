using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Uno_MyBar_Controller : MonoBehaviour {
    [SerializeField] CanvasGroup canvasGroupBtnUno;
    [SerializeField] ParticleSystem glowBtnUno;
	[SerializeField] CanvasGroup canvasGroupBtnPut;
	[SerializeField] CanvasGroup canvasGroupBtnSkip;
    [SerializeField] Uno_BtnDrawCard_Controller btnDrawCardUno;

    [Header("Setting")]
    public float alphaMyBtnWhenNotActive;
	public float alphaMyBtnWhenActive;

    public Uno_GamePlay_Manager gamePlayManager{
        get{
            return Uno_GamePlay_Manager.instance;
        }
    }

    public System.DateTime nextTimeToTouch;

    int indexCard = -1;
    int cardValue = -1;
	public sbyte stateDrawCard{get;set;} // 0: chưa bấm gì, 1: đã bấm và chờ cmt trả về, 2: đã bấm

    LTDescr tweenCanvasGroupBtnDraw;

    void Awake(){
        nextTimeToTouch = System.DateTime.Now;
    }

    [ContextMenu("TEST")]
    void TEST(){
        Debug.Log(System.DateTime.Now + " -- " + System.DateTime.UtcNow);
    }

    void SetMyButton(CanvasGroup _btnCanvasGroup, float _alpha, bool _blocksRaycasts, bool _updateNow = true){
		_btnCanvasGroup.blocksRaycasts = _blocksRaycasts;
		if(_updateNow){
			_btnCanvasGroup.alpha = _alpha;
		}else{
            LeanTween.cancel(_btnCanvasGroup.gameObject);
			LeanTween.alphaCanvas(_btnCanvasGroup, _alpha, 0.1f);
		}
	}

    [ContextMenu("ShowBtnUno")]
    public void ShowBtnUno(){
        if(Uno_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
            MyAudioManager.instance.PlaySfx(Uno_GamePlay_Manager.instance.myAudioInfo.sfx_BtnUnoOrAtkUnoAppear);
        }
        canvasGroupBtnUno.blocksRaycasts = true;
        glowBtnUno.gameObject.SetActive(true);
        glowBtnUno.Play();
        LeanTween.cancel(canvasGroupBtnUno.gameObject);
        LeanTween.alphaCanvas(canvasGroupBtnUno, 1f, 0.1f);
    }

    public void HideBtnUno(){
        canvasGroupBtnUno.blocksRaycasts = false;
        glowBtnUno.gameObject.SetActive(false);
        LeanTween.cancel(canvasGroupBtnUno.gameObject);
        LeanTween.alphaCanvas(canvasGroupBtnUno, 0f, 0.1f);
    }

    // public void SetBtnDraw(float _alpha, bool _blocksRaycasts, bool _updateNow = true){
    //     SetMyButton(canvasGroupBtnDrawCard, _alpha, _blocksRaycasts, _updateNow);
    // }

    public void ShowBtnDraw(bool _updateNow = true){
        btnDrawCardUno.Show(_updateNow);
    }

    public void HideBtnDraw(bool _updateNow = true){
        btnDrawCardUno.Hide(_updateNow);
    }

    public void SetBtnSkip(float _alpha, bool _blocksRaycasts, bool _updateNow = true){
        SetMyButton(canvasGroupBtnSkip, _alpha, _blocksRaycasts, _updateNow);
    }

    public void SetBtnPut(float _alpha, bool _blocksRaycasts, bool _updateNow = true){
        SetMyButton(canvasGroupBtnPut, _alpha, _blocksRaycasts, _updateNow);
    }

    public void HideAllButtons(){
        HideBtnDraw();
		SetBtnSkip(0f, false);
		SetBtnPut(0f, false);
    }

    public void RefreshAllMyButton(){
        stateDrawCard = 0;
		if(gamePlayManager == null
			|| !gamePlayManager.unoGamePlayData.CheckIsMyTurn()){
            HideAllButtons();
			return;
		}
        if(gamePlayManager.unoGamePlayData.globalCards.Count != 0){
            ShowBtnDraw(false);
        }
		SetBtnPut(alphaMyBtnWhenNotActive, false);
		SetBtnSkip(alphaMyBtnWhenNotActive, false);
        if(gamePlayManager.myPlayerGroup.ownCardPoolManager.listObjects.Count <= 2){
            int _index = gamePlayManager.unoGamePlayData.listSessionIdPlaying.IndexOf(DataManager.instance.userData.sessionId);
            if(!gamePlayManager.unoGamePlayData.listPlayerPlayingData[_index].hasCalledUno){
                bool _tmpCanCallUno = false;
                for(int i = 0; i < gamePlayManager.myPlayerGroup.ownCardPoolManager.listObjects.Count; i++){
                    if(((PanelCardUnoDetailController) gamePlayManager.myPlayerGroup.ownCardPoolManager.listObjects[i]).canPut){
                        _tmpCanCallUno = true;
                        break;
                    }
                }
                if(_tmpCanCallUno){
                    ShowBtnUno();
                }
            }
        }
	}

    #region On Button Clicked
    public void OnButtonPutCard(){
        if(gamePlayManager == null){
            #if TEST
            Debug.LogError(">>> BUG LOGIC PutCard (0)");
            #endif
            return;
        }
		if(gamePlayManager.unoGamePlayData == null){
            #if TEST
            Debug.LogError(">>> BUG LOGIC PutCard (1)");
            #endif
			return;
		}
		if(gamePlayManager.unoGamePlayData.currentGameState != UnoGamePlayData.GameState.STATUS_PLAYING){
			#if TEST
            Debug.LogError(">>> BUG LOGIC PutCard (2)");
            #endif
            return;
		}
        if(gamePlayManager.myPlayerGroup == null){
            #if TEST
            Debug.LogError(">>> BUG LOGIC PutCard (3)");
            #endif
            return;
        }
		if(gamePlayManager.myPlayerGroup.currentCardUnoFocusing == null){
            #if TEST
            Debug.LogError(">>> BUG LOGIC PutCard (4)");
            #endif
			return;
		}
        if(System.DateTime.Now < nextTimeToTouch){
            #if TEST
            Debug.LogError(">>> Chưa được bấm");
            #endif
            return;
        }
        nextTimeToTouch = System.DateTime.Now.AddSeconds(0.5f);

        if(Uno_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
            MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
        }
        List<PanelCardUnoDetailController> _ownListPanelCard = new List<PanelCardUnoDetailController>();
		for(int i = 0; i < gamePlayManager.myPlayerGroup.ownCardPoolManager.listObjects.Count; i++){
            _ownListPanelCard.Add((PanelCardUnoDetailController) gamePlayManager.myPlayerGroup.ownCardPoolManager.listObjects[i]);
        }
        indexCard = _ownListPanelCard.IndexOf(gamePlayManager.myPlayerGroup.currentCardUnoFocusing);
        cardValue = gamePlayManager.myPlayerGroup.currentCardUnoFocusing.cardValue;
        if(cardValue == UnoGamePlayData.WILD_COLOR || cardValue == UnoGamePlayData.WILD_DRAW4){
            gamePlayManager.UIManager.panelChooseColor.Show((_indexColor)=>{
                if(_indexColor != -1){
                    if(cardValue == UnoGamePlayData.WILD_COLOR){
                        if(_indexColor == 0){
                            cardValue = UnoGamePlayData.WILD_COLOR_RED;
                        }else if(_indexColor == 1){
                            cardValue = UnoGamePlayData.WILD_COLOR_GREEN;
                        }else if(_indexColor == 2){
                            cardValue = UnoGamePlayData.WILD_COLOR_BLUE;
                        }else if(_indexColor == 3){
                            cardValue = UnoGamePlayData.WILD_COLOR_YELLOW;
                        }else{
                            #if TEST
                            Debug.LogError(">>> IndexColor sai nên chọn mặc định: " + _indexColor);
                            #endif
                            cardValue = UnoGamePlayData.WILD_COLOR_YELLOW;
                        }
                    }else{
                        if(_indexColor == 0){
                            cardValue = UnoGamePlayData.WILD_DRAW4_RED;
                        }else if(_indexColor == 1){
                            cardValue = UnoGamePlayData.WILD_DRAW4_GREEN;
                        }else if(_indexColor == 2){
                            cardValue = UnoGamePlayData.WILD_DRAW4_BLUE;
                        }else if(_indexColor == 3){
                            cardValue = UnoGamePlayData.WILD_DRAW4_YELLOW;
                        }else{
                            #if TEST
                            Debug.LogError(">>> IndexColor sai nên chọn mặc định: " + _indexColor);
                            #endif
                            cardValue = UnoGamePlayData.WILD_DRAW4_YELLOW;
                        }
                    }
                    Uno_RealTimeAPI.instance.SendMessagePutCard(cardValue, indexCard);
                }
                gamePlayManager.UIManager.panelChooseColor.Hide(_indexColor);

                indexCard = -1;
                cardValue = -1;
            });
        }else{
            Uno_RealTimeAPI.instance.SendMessagePutCard(cardValue, indexCard);
            indexCard = -1;
            cardValue = -1;
        }
    }

    public void OnButtonDrawCard(){
        if(gamePlayManager == null){
            #if TEST
            Debug.LogError(">>> BUG LOGIC DrawCard (0)");
            #endif
            return;
        }
		if(gamePlayManager.unoGamePlayData == null){
            #if TEST
            Debug.LogError(">>> BUG LOGIC DrawCard (1)");
            #endif
			return;
		}
		if(gamePlayManager.unoGamePlayData.currentGameState != UnoGamePlayData.GameState.STATUS_PLAYING){
			#if TEST
            Debug.LogError(">>> BUG LOGIC DrawCard (2)");
            #endif
            return;
		}
        if(System.DateTime.Now < nextTimeToTouch){
            #if TEST
            Debug.LogError(">>> Chưa được bấm");
            #endif
            return;
        }
        nextTimeToTouch = System.DateTime.Now.AddSeconds(0.5f);

        if(Uno_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
            MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
        }
        if(stateDrawCard == 0){
            stateDrawCard = 1;
        }else if(stateDrawCard == 2){
            return;
        }
        Uno_RealTimeAPI.instance.SendMessageGetCard();
    }

    public void OnButtonSkip(){
        if(gamePlayManager == null){
            #if TEST
            Debug.LogError(">>> BUG LOGIC Skip (0)");
            #endif
            return;
        }
		if(gamePlayManager.unoGamePlayData == null){
            #if TEST
            Debug.LogError(">>> BUG LOGIC Skip (1)");
            #endif
			return;
		}
		if(gamePlayManager.unoGamePlayData.currentGameState != UnoGamePlayData.GameState.STATUS_PLAYING){
			#if TEST
            Debug.LogError(">>> BUG LOGIC Skip (2)");
            #endif
            return;
		}
        if(System.DateTime.Now < nextTimeToTouch){
            #if TEST
            Debug.LogError(">>> Chưa được bấm");
            #endif
            return;
        }
        nextTimeToTouch = System.DateTime.Now.AddSeconds(0.5f);

        if(Uno_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
            MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
        }
        if(stateDrawCard != 2){
            return;
        }
        Uno_RealTimeAPI.instance.SendMessageSkipTurn();
    }

    public void OnButtonUno(){
        if(gamePlayManager == null){
            #if TEST
            Debug.LogError(">>> BUG LOGIC Uno (0)");
            #endif
            return;
        }
		if(gamePlayManager.unoGamePlayData == null){
            #if TEST
            Debug.LogError(">>> BUG LOGIC Uno (1)");
            #endif
			return;
		}
		if(gamePlayManager.unoGamePlayData.currentGameState != UnoGamePlayData.GameState.STATUS_PLAYING){
			#if TEST
            Debug.LogError(">>> BUG LOGIC Uno (2)");
            #endif
            return;
		}
        if(System.DateTime.Now < nextTimeToTouch){
            #if TEST
            Debug.LogError(">>> Chưa được bấm");
            #endif
            return;
        }
        nextTimeToTouch = System.DateTime.Now.AddSeconds(0.5f);

        if(Uno_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
            MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
        }
        Uno_RealTimeAPI.instance.SendMessageCallUno();
    }
    #endregion
}
