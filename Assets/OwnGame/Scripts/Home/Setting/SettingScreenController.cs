using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingScreenController : UIHomeScreenController {

	public static SettingScreenController instance{
		get{
			return ins;
		}
	}
	private static SettingScreenController ins;

	public override UIType myType {
		get {
			return UIType.SettingScreen;
		}
	}

	public override bool isSubScreen {
		get {
			return true;
		}
	}
	[SerializeField] Canvas myCanvas;
	[SerializeField] Text txtTitleSetting;
	[Header("Toggle Music")]
	[SerializeField] Slider toggleMusic;
	[SerializeField] Text txtTitleMusic;
	[SerializeField] Text txtMusicStatus;
	[SerializeField] Image imgBgToggleMusic;

	[Header("Toggle Sfx")]
	[SerializeField] Slider toggleSfx;
	[SerializeField] Text txtTitleSfx;
	[SerializeField] Text txtSfxStatus;
	[SerializeField] Image imgBgToggleSfx;

	[Header("Toggle Vibrate")]
	[SerializeField] Slider toggleVibrate;
	[SerializeField] Text txtTitleVibrate;
	[SerializeField] Text txtVibrateStatus;
	[SerializeField] Image imgBgToggleVibrate;
	
	[Header("Others")]
	[SerializeField] Transform mainContainer;
	[SerializeField] Transform panelButton00; // panel chứa nút contact + logout
	[SerializeField] Transform panelButton01; // oanel chứa nút out room
	[SerializeField] Text txtBtnContactUs;
	[SerializeField] Text txtBtnLogOut;
	public Button btnOutRoom;
	[SerializeField] Text txtBtnOutRoom;
	[SerializeField] Button btnDownLoad;
	[SerializeField] Text txtBtnDownloadNewVersion;

	[Header("Setting")]
	[SerializeField] float timeShowScreen;
	[SerializeField] float timeHideScreen;

	LTDescr tweenCanvasGroup, tweenMainContainer;
	string strOn, strOff;

	private void Awake(){
		if (ins != null && ins != this) { 
			Destroy(this.gameObject); 
			return;
		}
		ins = this;
		DontDestroyOnLoad (this.gameObject);

		base.Hide();
	}

	#region Init / Show / Hide
	public override void InitData ()
	{	
		strOn = MyLocalize.GetString("Global/On").ToUpper();
		strOff = MyLocalize.GetString("Global/Off").ToUpper();

		txtTitleSetting.text = MyLocalize.GetString("Global/Setting");
		txtBtnContactUs.text = MyLocalize.GetString("Global/ContactUs");
		txtBtnLogOut.text = MyLocalize.GetString("Global/Logout");

		txtTitleMusic.text = MyLocalize.GetString("Global/Music");
		txtTitleSfx.text = MyLocalize.GetString("Global/Sound");
		txtTitleVibrate.text = MyLocalize.GetString("Global/Vibrate");
		txtBtnOutRoom.text = MyLocalize.GetString("Global/OutRoom");
		
		if(CoreGameManager.instance.currentSceneManager.mySceneType == IMySceneManager.Type.Home){
			if(DataManager.instance.haveNewVersion){
				btnDownLoad.gameObject.SetActive(true);
				txtBtnDownloadNewVersion.text = MyLocalize.GetString("Global/UpgradeNewVersion");
			}else{
				btnDownLoad.gameObject.SetActive(false);
			}
			panelButton00.gameObject.SetActive(true);
			panelButton01.gameObject.SetActive(false);
		}else{
			btnDownLoad.gameObject.SetActive(false);
			panelButton00.gameObject.SetActive(false);

			if(CoreGameManager.instance.currentSceneManager.mySceneType == IMySceneManager.Type.SubGamePlayScene){
				panelButton01.gameObject.SetActive(true);
			}else {
				if(CoreGameManager.instance.currentSubGamePlay != null){
					panelButton01.gameObject.SetActive(false);
				}else{
					panelButton01.gameObject.SetActive(true);
				}
			}
		}

		btnOutRoom.onClick.RemoveAllListeners();

		if (DataManager.instance.musicStatus == 1) {
			toggleMusic.value = toggleMusic.maxValue;
			txtMusicStatus.text = strOn;
			imgBgToggleMusic.color = Color.green;
		} else {
			toggleMusic.value = toggleMusic.minValue;
			txtMusicStatus.text = strOff;
			imgBgToggleMusic.color = Color.red;
		}
		if (DataManager.instance.sfxStatus == 1) {
			toggleSfx.value = toggleSfx.maxValue;
			txtSfxStatus.text = strOn;
			imgBgToggleSfx.color = Color.green;
		} else {
			toggleSfx.value = toggleSfx.minValue;
			txtSfxStatus.text = strOff;
			imgBgToggleSfx.color = Color.red;
		}
		if (DataManager.instance.vibrationStatus == 1) {
			toggleVibrate.value = toggleVibrate.maxValue;
			txtVibrateStatus.text = strOn;
			imgBgToggleVibrate.color = Color.green;
		} else {
			toggleVibrate.value = toggleVibrate.minValue;
			txtVibrateStatus.text = strOff;
			imgBgToggleVibrate.color = Color.red;
		}

		onPressBack = () => {
			if(HomeManager.instance != null){
				HomeManager.instance.ChangeScreen (myLastType);
			}else{
				Hide();
			}
		};
		CoreGameManager.instance.RegisterNewCallbackPressBackKey (onPressBack);
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

		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_TogglePanel);
		
		if(tweenCanvasGroup != null){
			LeanTween.cancel(tweenCanvasGroup.uniqueId);
			tweenCanvasGroup = null;
		}
		tweenCanvasGroup = LeanTween.alphaCanvas(myCanvasGroup, 1f, timeShowScreen).setEase(LeanTweenType.easeOutBack).setOnComplete(()=>{
			tweenCanvasGroup = null;
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

	public override void Hide ()
	{
		if(currentState == State.Hide){
			return;
		}
		if (onPressBack != null) {
			CoreGameManager.instance.RemoveCurrentCallbackPressBackKey (onPressBack);
			onPressBack = null;
		}
		
		btnOutRoom.onClick.RemoveAllListeners();

		currentState = State.Hide;
		myCanvasGroup.blocksRaycasts = false;
		
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_TogglePanel);

		if(tweenCanvasGroup != null){
			LeanTween.cancel(tweenCanvasGroup.uniqueId);
			tweenCanvasGroup = null;
		}
		tweenCanvasGroup = LeanTween.alphaCanvas(myCanvasGroup, 0f, timeHideScreen).setOnComplete(()=>{
			tweenCanvasGroup = null;
			base.Hide();
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
		btnOutRoom.onClick.RemoveAllListeners();

		if(tweenCanvasGroup != null){
			LeanTween.cancel(tweenCanvasGroup.uniqueId);
			tweenCanvasGroup = null;
		}
		if(tweenMainContainer != null){
			LeanTween.cancel(tweenMainContainer.uniqueId);
			tweenMainContainer = null;
		}

		base.Hide();
	}
	#endregion

	#region On Button Clicked
	public void OnClickToggleMusic(){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
		// Debug.Log("ToggleMusic");
		if (toggleMusic.value == toggleMusic.minValue) {
			DataManager.instance.musicStatus = 1;
			toggleMusic.value = toggleMusic.maxValue;
			txtMusicStatus.text = strOn;
			imgBgToggleMusic.color = Color.green;
			MyAudioManager.instance.ResumeMusic();
		} else {
			DataManager.instance.musicStatus = 0;
			toggleMusic.value = toggleMusic.minValue;
			txtMusicStatus.text = strOff;
			imgBgToggleMusic.color = Color.red;
			MyAudioManager.instance.PauseMusic();
		}
	}
	public void OnClickToggleSfx(){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
		// Debug.Log("ToggleSfx");
		if (toggleSfx.value == toggleSfx.minValue) {
			DataManager.instance.sfxStatus = 1;
			toggleSfx.value = toggleSfx.maxValue;
			txtSfxStatus.text = strOn;
			imgBgToggleSfx.color = Color.green;
		} else {
			DataManager.instance.sfxStatus = 0;
			toggleSfx.value = toggleSfx.minValue;
			txtSfxStatus.text = strOff;
			imgBgToggleSfx.color = Color.red;
			MyAudioManager.instance.StopAllSfx();
		}
	}
	public void OnClickToggleVibrate(){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
		// Debug.Log("ToggleVibrate");
		if (toggleVibrate.value == toggleVibrate.minValue) {
			DataManager.instance.vibrationStatus = 1;
			toggleVibrate.value = toggleVibrate.maxValue;
			txtVibrateStatus.text = strOn;
			imgBgToggleVibrate.color = Color.green;
			CoreGameManager.instance.DoVibrate();
		} else {
			DataManager.instance.vibrationStatus = 0;
			toggleVibrate.value = toggleVibrate.minValue;
			txtVibrateStatus.text = strOff;
			imgBgToggleVibrate.color = Color.red;
		}
	}
	public void OnClickContactUs(){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
		Application.OpenURL(MyConstant.linkContactUs);
	}
	public void OnClickLogOut(){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
		PopupManager.Instance.CreatePopupDialog(MyLocalize.GetString(MyLocalize.kWarning)
			, MyLocalize.GetString("System/AskForLogOut")
			, string.Empty
			, MyLocalize.GetString(MyLocalize.kOk)
			, MyLocalize.GetString(MyLocalize.kCancel)
			, ()=>{
				DataManager.instance.userData = new UserData();
				DataManager.instance.userData.InitData();

				DataManager.instance.parentUserData = new UserData();

				DataManager.instance.achievementData = new AchievementData();
				DataManager.instance.achievementData.InitData();

				DataManager.instance.dailyRewardData.ResetLoginData();
				DataManager.instance.subsidyData.ResetLoginData();

				DataManager.instance.installAppData = new InstallAppData();
				DataManager.instance.installAppData.InitData();

				DataManager.instance.purchaseReceiptData = new PurchaseReceiptData();
				DataManager.instance.purchaseReceiptData.InitData();

				DataManager.instance.remindRating_GoldUserCatched = 0;

				if(FacebookAPI.IsLoggedIn()){
					FacebookAPI.LogOut();
				}
				
				HomeManager.getGoldAndGemInfoAgain = true;
				HomeManager.hasShowTopAndBottomBar = false;
				HomeManager.myCurrentState = HomeManager.State.LogOutAndBackToLoginScreen;
				SceneLoaderManager.instance.LoadScene(MyConstant.SCENE_HOME);
			}, null);
		
	}
	public void OnClickDownLoadNewVersion(){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
		Application.OpenURL(MyConstant.linkApp);
	}
	public void OnButtonBackClicked(){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
		
		if (onPressBack != null) {
			CoreGameManager.instance.RemoveCurrentCallbackPressBackKey (onPressBack);
			onPressBack ();
			onPressBack = null;
		}
	}
	#endregion
}