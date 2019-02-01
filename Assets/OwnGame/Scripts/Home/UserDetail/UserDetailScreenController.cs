using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserDetailScreenController : UIHomeScreenController {
	
	public override UIType myType {
		get {
			return UIType.UserDetail;
		}
	}

	public override bool isSubScreen {
		get {
			return true;
		}
	}

	[SerializeField] Transform mainContainer;
	[SerializeField] RawImage imgAvata;
	[SerializeField] Image imgDatabaseType;
	[SerializeField] Text txtUserName;
	[SerializeField] Text txtUserId;
	[SerializeField] Text txtUserGold;
	[SerializeField] Transform panelContainEmail;
	[SerializeField] Text txtEmail;
	[SerializeField] Transform myAchievementDetailContent;
	[SerializeField] GameObject prefabAchievementOption;
	[SerializeField] Transform panelButtonsConfigAccount;

	[Header("Buttons")]
	[SerializeField] Text txtBtn_FieldChangePass;
	[SerializeField] Text txtBtn_FieldVerifyEmail;
	[SerializeField] ParticleSystem iconWarningVerifyEmail;

	[Header("Choose Avatar")]
	[SerializeField] List<RawImage> listAvatar;
	[SerializeField] Transform panelTick;

	[Header("Setting")]
	[SerializeField] float timeShowScreen;
	[SerializeField] float timeHideScreen;

	bool isInitializedListAchievement;
	List<UserDetail_PanelAchievementOption_Controller> listPanelAchievement;
	LTDescr tweenCanvasGroup, tweenMainContainer;
	sbyte avatarIdSaved;

	#region Init / Show / Hide
	public override void InitData ()
	{
		if (!isInitializedListAchievement) {
			listPanelAchievement = new List<UserDetail_PanelAchievementOption_Controller> ();
			for (int i = 0; i < DataManager.instance.achievementData.listAchievementDetail.Count; i++) {
				UserDetail_PanelAchievementOption_Controller _achievementInfo = ((GameObject)Instantiate (prefabAchievementOption, myAchievementDetailContent, false)).GetComponent<UserDetail_PanelAchievementOption_Controller> ();
				_achievementInfo.InitData (DataManager.instance.achievementData.listAchievementDetail[i]);
				listPanelAchievement.Add (_achievementInfo);
			}
			isInitializedListAchievement = true;
		}else{
			for (int i = 0; i < listPanelAchievement.Count; i++) {
				listPanelAchievement[i].InitData(DataManager.instance.achievementData.listAchievementDetail[i]);
			}
		}

		if (DataManager.instance.userData.databaseId == UserData.DatabaseType.DATABASEID_FACEBOOK) {
			listAvatar[0].texture = GameInformation.instance.otherInfo.avatarDefault;
			DataManager.instance.userData.LoadAvatarFromLink(CoreGameManager.instance, listAvatar[0].rectTransform.rect.width, listAvatar[0].rectTransform.rect.height, (_avatar) => {
				listAvatar[0].texture = _avatar;
			}); 
		}

		avatarIdSaved = DataManager.instance.userData.avatarid;

		LoadUserInfo ();
		
		if(DataManager.instance.userData.databaseId == UserData.DatabaseType.DATABASEID_BIGXU){
			panelButtonsConfigAccount.gameObject.SetActive(true);
			txtBtn_FieldChangePass.text = MyLocalize.GetString("RegisAndLogin/Field_ChangePassword");
			txtBtn_FieldVerifyEmail.text = MyLocalize.GetString("RegisAndLogin/Field_VerifyEmail");

			panelContainEmail.gameObject.SetActive(true);

			if(string.IsNullOrEmpty(DataManager.instance.userData.emailShow)){
				iconWarningVerifyEmail.gameObject.SetActive(true);
				iconWarningVerifyEmail.Play();
			}else{
				iconWarningVerifyEmail.gameObject.SetActive(false);
			}
		}else{
			panelContainEmail.gameObject.SetActive(false);
			panelButtonsConfigAccount.gameObject.SetActive(false);
		}
		RefreshEmailInfo();

		HomeManager.instance.myCallbackManager.onLoadEmailInfoFinished += CheckRefreshEmailInfo;

		onPressBack = () => {
			HomeManager.instance.ChangeScreen (myLastType);
			if(avatarIdSaved != DataManager.instance.userData.avatarid){
				OneHitAPI.ChooseAvatar (DataManager.instance.userData.databaseId, DataManager.instance.userData.userId, DataManager.instance.userData.avatarid, null);
			}
		};
		CoreGameManager.instance.RegisterNewCallbackPressBackKey (onPressBack);
	}

	public override void RefreshData ()
	{
		LoadUserInfo();
	}

	public override void Show ()
	{
		if(currentState == State.Show){
			return;
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

		StartCoroutine(DoActionShow());
	}
	
	IEnumerator DoActionShow(){
		yield return Yielders.EndOfFrame;
		sbyte _tmpAvatarid = (sbyte)(DataManager.instance.userData.avatarid % UserData.maxLengthListAvatarID);
		panelTick.position = listAvatar[_tmpAvatarid].transform.position;
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
		StopAllCoroutines();
		currentState = State.Hide;
		if(HomeManager.instance.myCallbackManager.onLoadEmailInfoFinished != null){
			HomeManager.instance.myCallbackManager.onLoadEmailInfoFinished -= CheckRefreshEmailInfo;
		}
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

	void CheckRefreshEmailInfo(){
		if(currentState == State.Show){
			iconWarningVerifyEmail.gameObject.SetActive(false);
			RefreshEmailInfo();
		}
	}

	void RefreshEmailInfo(){
		if(!panelContainEmail.gameObject.activeSelf){
			return;
		}
		string _tmpContent = MyLocalize.GetString("RegisAndLogin/Field_Email") + ": " + DataManager.instance.userData.emailShow;
		_tmpContent = MyConstant.ConvertString(_tmpContent, 25);
		txtEmail.text = _tmpContent;
	}

	/*void LoadDataFromSever(){
		OneHitAPI.GetUserDetail (DataManager.instance.userData.databaseId, DataManager.instance.userData.userId, listGameID, (_messageReceiving, _error) => {
			if(currentState == State.Show){
				if(_messageReceiving != null){
					sbyte _checkCase = _messageReceiving.readByte();
					if(_checkCase > 0){
						DataManager.instance.userData.GetMoreUserData(_messageReceiving);
						for (int i = 0; i < DataManager.instance.achievementData.listAchievementDetail.Count; i++) {
							DataManager.instance.achievementData.listAchievementDetail[i].countWin = _messageReceiving.readInt();
							DataManager.instance.achievementData.listAchievementDetail[i].countDraw = _messageReceiving.readInt();
							DataManager.instance.achievementData.listAchievementDetail[i].countLose = _messageReceiving.readInt();

							if(DataManager.instance.achievementData.listAchievementDetail[i].countWin < 0){
								DataManager.instance.achievementData.listAchievementDetail[i].countWin = 0;
							}
							if(DataManager.instance.achievementData.listAchievementDetail[i].countDraw < 0){
								DataManager.instance.achievementData.listAchievementDetail[i].countDraw = 0;
							}
							if(DataManager.instance.achievementData.listAchievementDetail[i].countLose < 0){
								DataManager.instance.achievementData.listAchievementDetail[i].countLose = 0;
							}
							listPanelAchievement[i].InitData(DataManager.instance.achievementData.listAchievementDetail[i]);
						}
						RefreshData();
					}else{
						Debug.LogError("Lỗi kết nối đến database");
					}
				}else{
					Debug.LogError("Error Code: " + _error);
				}
			}
		});
	}*/
	#endregion

	void LoadUserInfo(){
		DataManager.instance.userData.LoadAvatar(this, imgAvata.rectTransform.rect.width, imgAvata.rectTransform.rect.height, (_avatar) => {
			try{
				if(_avatar != null){
					imgAvata.texture = _avatar;
				}
			}catch{}
		});
		txtUserName.text = DataManager.instance.userData.nameShowInGame;
		txtUserId.text = DataManager.instance.userData.userId.ToString();
		txtUserGold.text = MyConstant.GetMoneyString(DataManager.instance.userData.gold, 9999);

		if(imgDatabaseType != null){
			Sprite _iconDatabaseID = DataManager.instance.userData.GetIconDatabaseID();
			if(_iconDatabaseID != null){
				imgDatabaseType.gameObject.SetActive(true);
				imgDatabaseType.sprite = _iconDatabaseID;
			}else{
				imgDatabaseType.gameObject.SetActive(false);
			}
        }
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

	// public void OnButtonChooseAvatarClicked(){
	// 	MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
	// 	HomeManager.instance.ChangeScreen (UIType.ChooseAvatar, false);
	// }

	public void OnButtonChooseAvatarClicked(int _avatarid){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);

		DataManager.instance.userData.avatarid = (sbyte)_avatarid;
		sbyte _tmpAvatarid = (sbyte)(DataManager.instance.userData.avatarid % UserData.maxLengthListAvatarID);
		panelTick.position = listAvatar[_tmpAvatarid].transform.position;
		LoadUserInfo();
	}

	public void OnBtnChangePasswordClicked(){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);

		PopupManager.Instance.CreatePopupChangePassword(
			(_currentPass, _newPass) =>{
				// byte caseCheck (1 : đổi thành công)
				LoadingCanvasController.instance.Show (-1, true);
				OneHitAPI.BigxuAccount_ChangePassword(_currentPass, _newPass, (_messageReceiving, _error) => {
					LoadingCanvasController.instance.Hide ();
					if(_messageReceiving != null){
						sbyte _caseCheck = _messageReceiving.readByte();
						if(_caseCheck == 1){
							// #if TEST
							// Debug.Log (">>> Đổi pass thành công");
							// #endif
							DataManager.instance.userData.password = _newPass;

							PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kMessage)
								, MyLocalize.GetString("RegisAndLogin/ChangePasswordSuccessful")
								, _caseCheck.ToString()
								, MyLocalize.GetString(MyLocalize.kOk));
						}else{
							// #if TEST
							// Debug.LogError (">>> Đổi pass thất bại: " + _caseCheck);
							// #endif
							PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kMessage)
								, MyLocalize.GetString("RegisAndLogin/ChangePasswordFailed")
								, _caseCheck.ToString()
								, MyLocalize.GetString(MyLocalize.kOk));
						}
					}else{
						PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
							, MyLocalize.GetString(MyLocalize.kConnectionError)
							, _error.ToString()
							, MyLocalize.GetString(MyLocalize.kOk));
					}
				});
			}, null);
	}

	public void OnBtnVerifyEmailClicked(){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);

		PopupManager.Instance.CreatePopupVerifyEmail(
			(_email) =>{
				LoadingCanvasController.instance.Show (-1, true);
				OneHitAPI.BigxuAccount_SetEmailSecurity(_email, (_messageReceiving, _error) => {
					LoadingCanvasController.instance.Hide ();
					if(_messageReceiving != null){
						sbyte _caseCheck = _messageReceiving.readByte();
						if(_caseCheck == 1){
							string _emailShow = _messageReceiving.readString();
							#if TEST
							Debug.Log (">>> Verify Email thành công: " + _emailShow);
							#endif
							DataManager.instance.userData.emailShow = _emailShow;
							iconWarningVerifyEmail.gameObject.SetActive(false);
							RefreshEmailInfo();
							PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kMessage)
								, MyLocalize.GetString("RegisAndLogin/VerifyEmailSuccessful")
								, _caseCheck.ToString()
								, MyLocalize.GetString(MyLocalize.kOk));
						}else{
							PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kMessage)
								, MyLocalize.GetString("RegisAndLogin/VerifyEmailFailed")
								, _caseCheck.ToString()
								, MyLocalize.GetString(MyLocalize.kOk));
						}
					}else{
						PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
							, MyLocalize.GetString(MyLocalize.kConnectionError)
							, _error.ToString()
							, MyLocalize.GetString(MyLocalize.kOk));
					}
				});
			}, null);
	}
	#endregion
}
