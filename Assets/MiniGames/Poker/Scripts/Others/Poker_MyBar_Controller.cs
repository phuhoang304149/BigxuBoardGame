using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Poker_MyBar_Controller : MonoBehaviour {

	const string strRAISE = "RAISE";
	const string strCALL = "CALL";
	const string strCHECK = "CHECK";
	const string strALLIN = "ALL-IN";

	enum State{
		Hide, Show
	}
	State currentState;

	[SerializeField] CanvasGroup myCanvasGroup;
	[SerializeField] Poker_MyBar_Toggle_Controller toggleAutoCheckOrFold;
	[SerializeField] Poker_MyBar_Toggle_Controller toggleAutoCheck;
	[SerializeField] Poker_MyBar_Toggle_Controller toggleCallAny;
	[SerializeField] Poker_MyBar_Button_Controller btnCallOrCheck;
	[SerializeField] Poker_MyBar_Button_Controller btnStandUp;
	[SerializeField] Poker_MyBar_Button_Controller btnFold;
	[SerializeField] Poker_MyBar_Button_Controller btnBet;
	[SerializeField] Poker_MyBar_PanelBetDetail_Controller panelBetDetail;

	public PokerGamePlayData pokerGamePlayData{
		get{
			return Poker_GamePlay_Manager.instance.pokerGamePlayData;
		}
	}

	PokerGamePlayData.Poker_PlayerPlayingData dataPlaying;
	sbyte myIndexCircle;

	long myBet;
	long defaultBet;
	long myDeltaBet;
	public System.DateTime timeCanPress;

	private void Awake() {
		currentState = State.Hide;
		myCanvasGroup.blocksRaycasts = false;
		myCanvasGroup.alpha = 0f;
		ResetData();
		timeCanPress = System.DateTime.Now;
	}

	public void ResetData(){
		dataPlaying = null;
	}

	public void InitData(PokerGamePlayData.Poker_PlayerPlayingData _data, sbyte _indexCircle){
		toggleAutoCheckOrFold.myToggle.isOn = false;
		toggleAutoCheck.myToggle.isOn = false;
		toggleCallAny.myToggle.isOn = false;

		dataPlaying = _data;
		myIndexCircle = _indexCircle;
		
		defaultBet = 0;
		myBet = 0;
		myDeltaBet = 0;
	}

	public void RefreshUI(){
		defaultBet = 0;
		myBet = 0;
		myDeltaBet = 0;
		if(pokerGamePlayData.listSessionIdOnChair.Contains(DataManager.instance.userData.sessionId)){
			Show();
			ShowButtonStandUp();
		}else{
			Hide();
		}
		if(currentState == State.Hide){
			return;
		}
		if(dataPlaying == null){
			HideButtonFold();
			HideButtonCallOrCheck();
			HideButtonBet();
			HideBetDetail();

			HideToggleAutoCheckOrFold();
			HideToggleAutoCheck();
			HideToggleCallAny();
			return;
		}

		if(pokerGamePlayData.currentGameState == PokerGamePlayData.GameState.STATUS_FINISHGAME){
			HideButtonStandUp(false);
			HideToggleAutoCheckOrFold(false);
			HideToggleAutoCheck(false);
			HideToggleCallAny(false);
			
			HideButtonFold();
			HideButtonCallOrCheck();
			HideButtonBet();
			HideBetDetail();
			return;
		}
		if(dataPlaying.currentState == PokerGamePlayData.Poker_PlayerPlayingData.State.STATEPOKER_FOLD
			|| dataPlaying.currentState == PokerGamePlayData.Poker_PlayerPlayingData.State.STATEPOKER_ALLIN){
			HideToggleAutoCheckOrFold(false);
			HideToggleAutoCheck(false);
			HideToggleCallAny(false);
			
			HideButtonFold();
			HideButtonCallOrCheck();
			HideButtonBet();
			HideBetDetail();
			return;
		}

		if(pokerGamePlayData.currentCircle != myIndexCircle){
			CheckShowToggleGroupAuto();
			
			HideButtonFold();
			HideButtonCallOrCheck();
			HideButtonBet();
			HideBetDetail();
		}else{
			long _deltaBet = pokerGamePlayData.maxBet - dataPlaying.totalBet;

			if(toggleAutoCheckOrFold.myToggle.isOn || toggleAutoCheck.myToggle.isOn || toggleCallAny.myToggle.isOn){
				if(_deltaBet > 0 && toggleAutoCheck.myToggle.isOn){
					// -- Check xuống dưới -- //
					toggleAutoCheckOrFold.myToggle.isOn = false;
					toggleAutoCheck.myToggle.isOn = false;
					toggleCallAny.myToggle.isOn = false;
				}else{
					CheckShowToggleGroupAuto();

					HideButtonFold();
					HideButtonCallOrCheck();
					HideButtonBet();
					HideBetDetail();

					// -- Xử lý auto -- //
					SetUpAutoActionBet();
					// ---------------- //
					return;
				}
			}

			HideToggleAutoCheckOrFold();
			HideToggleAutoCheck();
			HideToggleCallAny();

			ShowButtonFold();
			ShowButtonBet();
			
			if(_deltaBet == 0){ // trường hợp trước đó chưa ai raise hết
				// Debug.Log("Refresh myBar trường hợp trước đó chưa ai raise hết");
				ShowButtonCallOrCheck();
				btnCallOrCheck.SetTextContent(strCHECK);

				defaultBet = pokerGamePlayData.betDefault;
				ShowCheckBetDetailAndBtnBet();
			}else if(_deltaBet > 0){ // trường hợp trước đó có người raise
				// Debug.Log("Refresh myBar trường hợp trước đó có người raise");
				if(pokerGamePlayData.totalRaiseInTurn == 0){ // trường hợp trước đó người chơi đã cược cho vai trò của SB và BB
					defaultBet = _deltaBet * 3;
				}else{
					defaultBet = _deltaBet * 2;
				}
				if(dataPlaying.userData.gold > _deltaBet){ // mình dư tiền để theo
					ShowButtonCallOrCheck();
					btnCallOrCheck.SetTextContent(strCALL);
				}else{ 
					HideButtonCallOrCheck(false);
					btnCallOrCheck.SetTextContent(strCHECK);
				}
				ShowCheckBetDetailAndBtnBet();
			}else{
				#if TEST
				Debug.LogError(">>> BUG Logic: " + dataPlaying.totalBet + " - " + pokerGamePlayData.maxBet);
				#endif
			}
		}
	}

	public void Show(){
		if(currentState == State.Show){
			return;
		}
		currentState = State.Show;
		myCanvasGroup.blocksRaycasts = true;
		LeanTween.alphaCanvas(myCanvasGroup, 1f, 0.1f);
	}
	public void Hide(){
		if(currentState == State.Hide){
			return;
		}
		currentState = State.Hide;
		myCanvasGroup.blocksRaycasts = false;
		LeanTween.alphaCanvas(myCanvasGroup, 0f, 0.1f);
		ResetData();
	}

	void ShowCheckBetDetailAndBtnBet(){
		int _tmpCount = 0;
		for(int i = 0; i < pokerGamePlayData.listPlayerPlayingData.Count; i++){
			if(pokerGamePlayData.listPlayerPlayingData[i].currentState == PokerGamePlayData.Poker_PlayerPlayingData.State.STATEPOKER_FOLD
			|| pokerGamePlayData.listPlayerPlayingData[i].currentState == PokerGamePlayData.Poker_PlayerPlayingData.State.STATEPOKER_ALLIN){
				_tmpCount ++;
			}
		}
		if(_tmpCount >= pokerGamePlayData.listPlayerPlayingData.Count - 1){ // trường hợp chỉ còn mình cược
			HideBetDetail(false);
			if(btnCallOrCheck.interactable){
				HideButtonBet(false);
			}
			if(dataPlaying.userData.gold <= defaultBet){ // trường hợp gold người chơi < minbet
				defaultBet = dataPlaying.userData.gold;

				panelBetDetail.betDetailSlider.value = 1f;
				panelBetDetail.betDetailInputField.text = "" + defaultBet;
				panelBetDetail.txtRealBetDetailInfo.text = MyConstant.GetMoneyString(defaultBet);

				btnBet.SetTextContent(strALLIN);
				myDeltaBet = 0;
			}else{ 
				panelBetDetail.betDetailInputField.text = "" + defaultBet;
				panelBetDetail.txtRealBetDetailInfo.text = MyConstant.GetMoneyString(defaultBet);

				btnBet.SetTextContent(strRAISE);
				myDeltaBet = dataPlaying.userData.gold - defaultBet;
			}
			myBet = defaultBet;
			return;
		}

		if(dataPlaying.userData.gold <= defaultBet){ // trường hợp gold người chơi < minbet
			defaultBet = dataPlaying.userData.gold;
			HideBetDetail(false);
			panelBetDetail.betDetailSlider.value = 1f;
			panelBetDetail.betDetailInputField.text = "" + defaultBet;
			panelBetDetail.txtRealBetDetailInfo.text = MyConstant.GetMoneyString(defaultBet);

			btnBet.SetTextContent(strALLIN);

			myDeltaBet = 0;
		}else{ 
			ShowBetDetail();
			panelBetDetail.betDetailInputField.text = "" + defaultBet;
			panelBetDetail.txtRealBetDetailInfo.text = MyConstant.GetMoneyString(defaultBet);

			btnBet.SetTextContent(strRAISE);

			myDeltaBet = dataPlaying.userData.gold - defaultBet;
		}
		myBet = defaultBet;
	}

	void ShowButtonStandUp(){
		btnStandUp.gameObject.SetActive(true);
		btnStandUp.SetInteractable(true);
	}
	void HideButtonStandUp(bool _setActiveFalse = true){
		if(_setActiveFalse){
			btnStandUp.gameObject.SetActive(false);
		}else{
			btnStandUp.gameObject.SetActive(true);
			btnStandUp.SetInteractable(false);
		}
	}

	void ShowButtonFold(){
		btnFold.gameObject.SetActive(true);
		btnFold.SetInteractable(true);
	}
	void HideButtonFold(bool _setActiveFalse = true){
		if(_setActiveFalse){
			btnFold.gameObject.SetActive(false);
		}else{
			btnFold.gameObject.SetActive(true);
			btnFold.SetInteractable(false);
		}
	}

	void ShowButtonCallOrCheck(){
		btnCallOrCheck.gameObject.SetActive(true);
		btnCallOrCheck.SetInteractable(true);
		btnCallOrCheck.SetTextContent(strCHECK);
	}
	void HideButtonCallOrCheck(bool _setActiveFalse = true){
		if(_setActiveFalse){
			btnCallOrCheck.gameObject.SetActive(false);
		}else{
			btnCallOrCheck.gameObject.SetActive(true);
			btnCallOrCheck.SetInteractable(false);
		}
		btnCallOrCheck.SetTextContent(strCHECK);
	}

	void ShowButtonBet(){
		btnBet.gameObject.SetActive(true);
		btnBet.SetInteractable(true);
		btnBet.SetTextContent(strRAISE);
	}
	void HideButtonBet(bool _setActiveFalse = true){
		if(_setActiveFalse){
			btnBet.gameObject.SetActive(false);
		}else{
			btnBet.gameObject.SetActive(true);
			btnBet.SetInteractable(false);
		}
		btnBet.SetTextContent(strRAISE);
	}

	void ShowBetDetail(){
		panelBetDetail.gameObject.SetActive(true);
		panelBetDetail.SetInteractable(true);
		panelBetDetail.betDetailSlider.value = 0f;
		panelBetDetail.betDetailInputField.text = string.Empty;
	}
	void HideBetDetail(bool _setActiveFalse = true){
		if(_setActiveFalse){
			panelBetDetail.gameObject.SetActive(false);
		}else{
			panelBetDetail.gameObject.SetActive(true);
			panelBetDetail.SetInteractable(false);
		}
		panelBetDetail.betDetailSlider.value = 0f;
		panelBetDetail.betDetailInputField.text = string.Empty;
	}

	void ShowToggleAutoCheckOrFold(){
		toggleAutoCheckOrFold.gameObject.SetActive(true);
		toggleAutoCheckOrFold.SetInteractable(true);
	}
	void HideToggleAutoCheckOrFold(bool _setActiveFalse = true , bool _isBlockRaycasts = false){
		if(_setActiveFalse){
			toggleAutoCheckOrFold.gameObject.SetActive(false);
		}else{
			toggleAutoCheckOrFold.gameObject.SetActive(true);
			toggleAutoCheckOrFold.SetInteractable(_isBlockRaycasts);
		}
	}

	void ShowToggleCallAny(){
		toggleCallAny.gameObject.SetActive(true);
		toggleCallAny.SetInteractable(true);
	}
	void HideToggleCallAny(bool _setActiveFalse = true, bool _isBlockRaycasts = false){
		if(_setActiveFalse){
			toggleCallAny.gameObject.SetActive(false);
		}else{
			toggleCallAny.gameObject.SetActive(true);
			toggleCallAny.SetInteractable(_isBlockRaycasts);
		}
	}

	void ShowToggleAutoCheck(){
		toggleAutoCheck.gameObject.SetActive(true);
		toggleAutoCheck.SetInteractable(true);
	}
	void HideToggleAutoCheck(bool _setActiveFalse = true, bool _isBlockRaycasts = false){
		if(_setActiveFalse){
			toggleAutoCheck.gameObject.SetActive(false);
		}else{
			toggleAutoCheck.gameObject.SetActive(true);
			toggleAutoCheck.SetInteractable(_isBlockRaycasts);
		}
	}

	public void CheckShowToggleGroupAuto(){
		if(dataPlaying == null){
			return;
		}

		if(toggleAutoCheckOrFold.myToggle.isOn){
			ShowToggleAutoCheckOrFold();
			HideToggleAutoCheck(false, true);
			HideToggleCallAny(false, true);
		}else if(toggleAutoCheck.myToggle.isOn){
			ShowToggleAutoCheck();
			HideToggleAutoCheckOrFold(false, true);
			HideToggleCallAny(false, true);
		}else if(toggleCallAny.myToggle.isOn){
			ShowToggleCallAny();
			HideToggleAutoCheckOrFold(false, true);
			HideToggleAutoCheck(false, true);
		}else{
			ShowToggleAutoCheckOrFold();
			ShowToggleAutoCheck();
			ShowToggleCallAny();
		}
	}

	void SetUpAutoActionBet(){
		long _deltaBet = pokerGamePlayData.maxBet - dataPlaying.totalBet;
		if(_deltaBet == 0){
			if(toggleAutoCheckOrFold.myToggle.isOn || toggleAutoCheck.myToggle.isOn || toggleCallAny.myToggle.isOn){
				Poker_RealTimeAPI.instance.SendMessageSetBet(_deltaBet);
			}
		}else if(_deltaBet > 0){
			if(toggleCallAny.myToggle.isOn){
				Poker_RealTimeAPI.instance.SendMessageSetBet(_deltaBet);
			}else if(toggleAutoCheck.myToggle.isOn){
				// Không làm gì hết
			}else{
				Poker_RealTimeAPI.instance.SendMessageSetBet(-1);
			}
		}else{
			#if TEST
			Debug.LogError(">>> BUG Logic SetUpAutoActionBet");
			#endif
		}

		toggleAutoCheckOrFold.myToggle.isOn = false;
		toggleAutoCheck.myToggle.isOn = false;
		toggleCallAny.myToggle.isOn = false;
		HideToggleAutoCheckOrFold(false);
		HideToggleAutoCheck(false);
		HideToggleCallAny(false);
	}
	#region On Button Clicked
	public void OnChangeValueBetDetailSlider(){
		// Debug.Log(betDetailSlider.value);
		
		if(dataPlaying == null){
			panelBetDetail.betDetailSlider.value = 0;
			return;
		}
		if(pokerGamePlayData.currentCircle != myIndexCircle 
			|| dataPlaying.currentState == PokerGamePlayData.Poker_PlayerPlayingData.State.STATEPOKER_FOLD
			|| dataPlaying.currentState == PokerGamePlayData.Poker_PlayerPlayingData.State.STATEPOKER_ALLIN){
			panelBetDetail.betDetailSlider.value = 0;
		}
		
		myBet = defaultBet + (long) (panelBetDetail.betDetailSlider.value * myDeltaBet);
		panelBetDetail.betDetailInputField.text = "" + myBet;
		panelBetDetail.txtRealBetDetailInfo.text = MyConstant.GetMoneyString(myBet);

		if(panelBetDetail.betDetailSlider.value == 1f){
			btnBet.SetTextContent(strALLIN);
		}else{
			btnBet.SetTextContent(strRAISE);
		}
	}

	public void OnChangeValueBetDetailInput(){
		if(!panelBetDetail.betDetailInputField.isFocused){
			return;
		}
		panelBetDetail.txtRealBetDetailInfo.text = panelBetDetail.betDetailInputField.text;
	}

	public void OnEndEditValueBetDetailInput(){
		// Debug.Log(panelBetDetail.betDetailInputField.text);
		
		if(dataPlaying == null){
			panelBetDetail.betDetailInputField.text = string.Empty;
			return;
		}
		if(pokerGamePlayData.currentCircle != myIndexCircle 
			|| dataPlaying.currentState == PokerGamePlayData.Poker_PlayerPlayingData.State.STATEPOKER_FOLD
			|| dataPlaying.currentState == PokerGamePlayData.Poker_PlayerPlayingData.State.STATEPOKER_ALLIN){
			panelBetDetail.betDetailInputField.text = string.Empty;
		}
		myBet = long.Parse(panelBetDetail.betDetailInputField.text);
		if(myBet < defaultBet){
			myBet = defaultBet;
		}
		if(myBet > dataPlaying.userData.gold){
			myBet = dataPlaying.userData.gold;
		}
		panelBetDetail.betDetailInputField.text = "" + myBet;
		panelBetDetail.txtRealBetDetailInfo.text = MyConstant.GetMoneyString(myBet);
		if(myDeltaBet == 0){
			panelBetDetail.betDetailSlider.value = 1f;
			btnBet.SetTextContent(strALLIN);
		}else{
			panelBetDetail.betDetailSlider.value = (myBet - defaultBet) / myDeltaBet;
			if(panelBetDetail.betDetailSlider.value == 1f){
				btnBet.SetTextContent(strALLIN);
			}else{
				btnBet.SetTextContent(strRAISE);
			}
		}
	}
	public void OnButtonStandUpClicked(){
		if(pokerGamePlayData.currentGameState == PokerGamePlayData.GameState.STATUS_WAIT_FOR_PLAYER){
			if(System.DateTime.Now.AddSeconds(1f) >= pokerGamePlayData.nextTimeToStartGame
				&& System.DateTime.Now <= pokerGamePlayData.nextTimeToStartGame){
				return;
			}
			if(System.DateTime.Now.AddSeconds(1f) >= pokerGamePlayData.nextTimeToStartGame.AddSeconds(1f)
				&& System.DateTime.Now <= pokerGamePlayData.nextTimeToStartGame.AddSeconds(1f)){
				return;
			}
		}

		if(timeCanPress > System.DateTime.Now){
			return;
		}
		timeCanPress = System.DateTime.Now.AddSeconds(0.5f);

		if(Poker_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
            MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
        }

		Poker_RealTimeAPI.instance.SendMessageStandUp();
	}
	public void OnButtonFoldClicked(){
		if(timeCanPress > System.DateTime.Now){
			return;
		}
		timeCanPress = System.DateTime.Now.AddSeconds(0.5f);

		if(Poker_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
            MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
        }
		Poker_RealTimeAPI.instance.SendMessageSetBet(-1);
	}
	public void OnButtonCallOrCheckClicked(){
		// if(string.IsNullOrEmpty(txtBtnCallOrCheck.text)){
		// 	#if TEST
		// 	Debug.LogError("txtBtnCallOrCheck.text is null");
		// 	#endif
		// 	return;
		// }
		if(timeCanPress > System.DateTime.Now){
			return;
		}
		timeCanPress = System.DateTime.Now.AddSeconds(0.5f);

		if(Poker_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
            MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
        }
		if(btnCallOrCheck.GetTextContent().Equals(strCHECK)){
			Poker_RealTimeAPI.instance.SendMessageSetBet(0);
		}else{
			long _value = pokerGamePlayData.maxBet - dataPlaying.totalBet;
			Poker_RealTimeAPI.instance.SendMessageSetBet(_value);
		}
	}
	public void OnButtonBetlicked(){
		// if(string.IsNullOrEmpty(txtBtnBet.text)){
		// 	#if TEST
		// 	Debug.LogError("txtBtnBet.text is null");
		// 	#endif
		// 	return;
		// }
		if(timeCanPress > System.DateTime.Now){
			return;
		}
		timeCanPress = System.DateTime.Now.AddSeconds(0.5f);
		
		if(Poker_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
            MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
        }
		Poker_RealTimeAPI.instance.SendMessageSetBet(myBet);
	}
	#endregion
}
