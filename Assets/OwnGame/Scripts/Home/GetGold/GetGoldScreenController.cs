using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetGoldScreenController : UIHomeScreenController {

	public static GetGoldScreenController instance{
		get{
			return ins;
		}
	}
	private static GetGoldScreenController ins;

	public override UIType myType{
		get{ 
			return UIType.GetGold;
		}
	}

	public override bool isSubScreen{
		get{ 
			return true;
		}
	}

	public enum Tab{
		DailyLogin, InstallApp, BuyGold, InviteFriend
	}
	public Tab currentTab{get;set;}

	[SerializeField] Canvas myCanvas;
	[Header("Panel subscreen")]
	public MySimplePanelController panelDailyLogin;
	public MySimplePanelController panelInstallApp;	
	public MySimplePanelController panelBuyGold;
	public MySimplePanelController panelInviteFriend;
	public GetGoldScreen_PanelSubsidy_Controller panelSubsidy;
	[SerializeField] GameObject tabSwitchInstallApp;

	[Header("Others")]
	[SerializeField] Text txtUserInfo_Gold;
	[SerializeField] Transform mainContainer;
	[SerializeField] Transform panelWarningAtTabBuyGold;


	[Header("Setting")]
	[SerializeField] float timeShowScreen;
	[SerializeField] float timeHideScreen;
	
	[Header("Audio Info")]
	public AudioClip sfx_NotThisTime;

	public MySimplePanelController currentPanel{get;set;}
	bool isSceneGame;
	
	long virtualMyGold, realMyGold;
	IEnumerator actionTweenMyGoldInfo, actionTweenMyGemInfo;
	LTDescr tweenCanvasGroup, tweenMainContainer;
	public bool canTouch{get;set;}

	private void Awake(){
		if (ins != null && ins != this) { 
			Destroy(this.gameObject); 
			return;
		}
		ins = this;
		DontDestroyOnLoad (this.gameObject);

		base.Hide();
		ResetData();
	}

	public override void ResetData(){
		StopAllCoroutines();
		
		if(tweenCanvasGroup != null){
			LeanTween.cancel(tweenCanvasGroup.uniqueId);
			tweenCanvasGroup = null;
		}
		if(tweenMainContainer != null){
			LeanTween.cancel(tweenMainContainer.uniqueId);
			tweenMainContainer = null;
		}

		actionTweenMyGoldInfo = null;
		actionTweenMyGemInfo = null;

		panelDailyLogin.ResetData();
		panelInstallApp.ResetData();
		panelBuyGold.ResetData();
		panelSubsidy.ResetData();
		panelInviteFriend.ResetData();

		canTouch =false;
	}
	
	#region Init / Show / Hide
	public override void InitData ()
	{
		currentTab = Tab.DailyLogin;

		switch(currentTab){
		case Tab.DailyLogin:
			currentPanel = panelDailyLogin;
			break;
		case Tab.InstallApp:
			currentPanel = panelInstallApp;
			break;
		case Tab.BuyGold:
			currentPanel = panelBuyGold;
			break;
		case Tab.InviteFriend:
			currentPanel = panelInviteFriend;
			break;
		}
		currentPanel.InitData();
		panelSubsidy.InitData();

		bool _haveSaleOffBuyGold = false;
		for(int i = 0; i < DataManager.instance.IAPProductData.listProductDetail.Count; i++){
			if(System.DateTime.Now < DataManager.instance.IAPProductData.listProductDetail[i].discount_time_finish){
				_haveSaleOffBuyGold = true;
				break;
			}
		}
		panelWarningAtTabBuyGold.gameObject.SetActive(_haveSaleOffBuyGold);
		if(!_haveSaleOffBuyGold){
			bool _flag = false;
			for(int i = 0; i < DataManager.instance.purchaseReceiptData.listPurchaseReceiptDetail.Count; i++){
				if(!DataManager.instance.purchaseReceiptData.listPurchaseReceiptDetail[i].isDone){
					_flag = true;
					break;
				}
			}
			panelWarningAtTabBuyGold.gameObject.SetActive(_flag);
		}

		if(HomeManager.instance != null){
			// tabSwitchInstallApp.SetActive(true);
			isSceneGame = false;
		}else{
			// tabSwitchInstallApp.SetActive(false);
			isSceneGame = true;
		}
		tabSwitchInstallApp.SetActive(false);

		onPressBack = () => {
			if(HomeManager.instance != null){
				HomeManager.instance.ChangeScreen (myLastType);
			}else{
				Hide();
			}
		};
		CoreGameManager.instance.RegisterNewCallbackPressBackKey (onPressBack);
	}

	public override void LateInitData(){
		RefreshMyGoldInfo();
	}

	public override void Show ()
	{
		if(currentState == State.Show){
			return;
		}
		if(myCanvas.worldCamera == null && CoreGameManager.instance.currentSceneManager != null){
			myCanvas.worldCamera = CoreGameManager.instance.currentSceneManager.cameraForConsumableScreen.mainCamera;
		}else{
			myCanvas.worldCamera = Camera.main;
		}
		
		currentState = State.Show;
		myCanvasGroup.alpha = 0f;
		myCanvasGroup.blocksRaycasts = true;
		currentPanel.Show();

		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_TogglePanel);
		
		if(tweenCanvasGroup != null){
			LeanTween.cancel(tweenCanvasGroup.uniqueId);
			tweenCanvasGroup = null;
		}
		tweenCanvasGroup = LeanTween.alphaCanvas(myCanvasGroup, 1f, timeShowScreen).setEase(LeanTweenType.easeOutBack).setOnComplete(()=>{
			tweenCanvasGroup = null;
			canTouch = true;
		});

		if(tweenMainContainer != null){
			LeanTween.cancel(tweenMainContainer.uniqueId);
			tweenMainContainer = null;
		}
		Vector3 _pos = Vector3.zero;
		_pos.x = 250f;
		mainContainer.localPosition = _pos;
		tweenMainContainer = LeanTween.moveLocalX(mainContainer.gameObject, 0f, timeShowScreen).setEase(LeanTweenType.easeOutBack).setOnComplete(()=>{
			tweenMainContainer = null;
		});
	}

	public override void Hide()
	{
		if(currentState == State.Hide){
			return;
		}

		if (onPressBack != null) {
			CoreGameManager.instance.RemoveCurrentCallbackPressBackKey (onPressBack);
			onPressBack = null;
		}

		currentState = State.Hide;
		canTouch = false;
		myCanvasGroup.blocksRaycasts = false;
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_TogglePanel);

		if(tweenCanvasGroup != null){
			LeanTween.cancel(tweenCanvasGroup.uniqueId);
			tweenCanvasGroup = null;
		}
		tweenCanvasGroup = LeanTween.alphaCanvas(myCanvasGroup, 0f, timeHideScreen).setOnComplete(()=>{
			base.Hide();
			ResetData();
		}).setEase(LeanTweenType.easeInBack);
		
		if(tweenMainContainer != null){
			LeanTween.cancel(tweenMainContainer.uniqueId);
			tweenMainContainer = null;
		}
		tweenMainContainer = LeanTween.moveLocalX(mainContainer.gameObject, -250f, timeHideScreen).setEase(LeanTweenType.easeInBack).setOnComplete(()=>{
			tweenMainContainer = null;
		});
	}

	public void ForcedHide(){
		currentState = State.Hide;
		if (onPressBack != null) {
			CoreGameManager.instance.RemoveCurrentCallbackPressBackKey (onPressBack);
			onPressBack = null;
		}

		base.Hide();
		ResetData();
	}
	#endregion

	public void RefreshMyGoldInfo(bool _updateNow = true){
		if(currentState == State.Hide){
			return;
		}

		realMyGold = DataManager.instance.userData.gold - DataManager.instance.userData.GetTotalBetInGamePlay();
        if(realMyGold < 0){
            #if TEST
            Debug.LogError("Bug Logic gold");
            #endif
            realMyGold = 0;
        };

		if(_updateNow){
			if(actionTweenMyGoldInfo != null){
				StopCoroutine(actionTweenMyGoldInfo);
				actionTweenMyGoldInfo = null;
			}
			virtualMyGold = realMyGold;
			txtUserInfo_Gold.text = MyConstant.GetMoneyString(virtualMyGold, 9999);
		}else{
			MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Gold);
			
			if(actionTweenMyGoldInfo != null){
				StopCoroutine(actionTweenMyGoldInfo);
				actionTweenMyGoldInfo = null;
			}
			actionTweenMyGoldInfo = MyConstant.TweenValue(virtualMyGold, realMyGold, 5, (_valueUpdate)=>{
				virtualMyGold = _valueUpdate;
				txtUserInfo_Gold.text = MyConstant.GetMoneyString(virtualMyGold, 9999);
			}, (_valueFinish)=>{
				virtualMyGold = _valueFinish;
				txtUserInfo_Gold.text = MyConstant.GetMoneyString(virtualMyGold, 9999);
				actionTweenMyGoldInfo = null;
			});
			StartCoroutine(actionTweenMyGoldInfo);
		}
	}

	public SubServerDetail GetServerDetail(){
		SubServerDetail _serverDetail = null;
		if(isSceneGame){
			if(CoreGameManager.instance.currentSceneManager.mySceneType == IMySceneManager.Type.SubGamePlayScene){
				if(DataManager.instance.miniGameData.currentSubGameDetail == null){
					#if TEST
					Debug.LogError("Bug Logic (0)");
					#endif
				}else{
					_serverDetail = DataManager.instance.miniGameData.currentSubGameDetail.currentServerDetail;
				}
			}else{
				if(DataManager.instance.miniGameData.currentMiniGameDetail == null){
					#if TEST
					Debug.LogError("Bug Logic (1)");
					#endif
				}else{
					_serverDetail = DataManager.instance.miniGameData.currentMiniGameDetail.currentServerDetail;
				}
			}
		}
		return _serverDetail;
	}

	#region On Button Clicked
	public void OnButtonBackClicked(){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
		if (onPressBack != null) {
			CoreGameManager.instance.RemoveCurrentCallbackPressBackKey (onPressBack);
			onPressBack ();
			onPressBack = null;
		}
	}
	public void OnButtonSwitchDailyLoginClicked(){
		if(currentTab == Tab.DailyLogin){
			return;
		}
		currentTab = Tab.DailyLogin;
		currentPanel.Hide();
		currentPanel = panelDailyLogin;
		currentPanel.InitData();
		currentPanel.Show();

		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
	}

	public void OnButtonSwitchInstallAppClicked(){
		if(currentTab == Tab.InstallApp){
			return;
		}
		currentTab = Tab.InstallApp;
		currentPanel.Hide();
		currentPanel = panelInstallApp;
		currentPanel.InitData();
		currentPanel.Show();

		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
	}

	public void OnButtonSwitchBuyGoldClicked(){
		if(currentTab == Tab.BuyGold){
			return;
		}
		currentTab = Tab.BuyGold;
		currentPanel.Hide();
		currentPanel = panelBuyGold;
		currentPanel.InitData();
		currentPanel.Show();

		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
	}

	public void OnButtonInviteFriendClicked(){
		if(currentTab == Tab.InviteFriend){
			return;
		}
		currentTab = Tab.InviteFriend;
		currentPanel.Hide();
		currentPanel = panelInviteFriend;
		currentPanel.InitData();
		currentPanel.Show();

		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
	}
	#endregion
}
