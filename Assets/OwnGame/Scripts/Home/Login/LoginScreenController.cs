using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Facebook;
using Facebook.Unity;
using Coffee.UIExtensions;

public class LoginScreenController : UIHomeScreenController
{
	public override UIType myType{
		get{ 
			return UIType.LoginScreen;
		}
	}

	enum PanelLoginState{
		Hide, Show
	}
	PanelLoginState panelLoginState;

	enum PanelRegisterState{
		Hide, Show
	}
	PanelRegisterState panelRegisterState;

	[SerializeField] Transform bgOriginal;
	[SerializeField] Transform bgCaptured;
	[SerializeField] CanvasGroup canvasGroupBgCaptured;

	[Header("Panel Login")]
	[SerializeField] CanvasGroup canvasGroupPanelLogin;
	[SerializeField] InputField login_fieldUserName;
	[SerializeField] InputField login_fieldPass;
	

	[Header("Panel Register")]
	[SerializeField] CanvasGroup canvasGroupPanelRegister;
	[SerializeField] Button register_btnRegister;
	[SerializeField] InputField register_fieldUserName;
	[SerializeField] InputField register_fieldPass;
	[SerializeField] InputField register_fieldConfirmPass;
	[SerializeField] Toggle register_toggleAgreePolicy;
	

	IEnumerator actionLoginFaceBook;

	#region Init / Show / Hide
	[ContextMenu("init data")]
	public override void InitData (){
		panelLoginState = PanelLoginState.Show;
		canvasGroupPanelLogin.alpha = 1f;
		canvasGroupPanelLogin.blocksRaycasts = true;
		login_fieldUserName.text = string.Empty;
		login_fieldPass.text = string.Empty;

		bgOriginal.gameObject.SetActive(true);
		bgCaptured.gameObject.SetActive(false);
		canvasGroupBgCaptured.alpha = 0f;

		panelRegisterState = PanelRegisterState.Hide;
		canvasGroupPanelRegister.alpha = 0f;
		canvasGroupPanelRegister.blocksRaycasts = false;
		register_fieldUserName.text = string.Empty;
		register_fieldPass.text = string.Empty;
		register_fieldConfirmPass.text = string.Empty;
		register_toggleAgreePolicy.isOn = true;

		onPressBack = () => {
			PopupManager.Instance.CreatePopupDialog(MyLocalize.GetString(MyLocalize.kWarning)
				, MyLocalize.GetString(MyLocalize.kAskForQuit)
				, string.Empty
				, MyLocalize.GetString(MyLocalize.kYes)
				, MyLocalize.GetString(MyLocalize.kNo)
				, () =>{
					Application.Quit();
				}, null);
		};
		if (onPressBack != null) {
			CoreGameManager.instance.RegisterNewCallbackPressBackKey (onPressBack);
		}
	}

	public override void Show (){
		base.Show();
		StartCoroutine(DoActionShow());
	}

	IEnumerator DoActionShow(){
		yield return Yielders.EndOfFrame;
		bgCaptured.gameObject.SetActive(true);
		yield return Yielders.EndOfFrame;
		LeanTween.alphaCanvas(canvasGroupBgCaptured, 1f, 0.1f).setOnComplete(()=>{
			bgOriginal.gameObject.SetActive(false);
		});
	}

	public override void Hide ()
	{
		if (onPressBack != null) {
			CoreGameManager.instance.RemoveCurrentCallbackPressBackKey (onPressBack);
		}
		if (actionLoginFaceBook != null) {
			StopCoroutine (actionLoginFaceBook);
			actionLoginFaceBook = null;
		}
		base.Hide ();
	}
	#endregion

	void SetUpChangeScreenChooseGame(){
		HomeManager.showAnnouncement = true;
		HomeManager.getGoldAndGemInfoAgain = true;
		HomeManager.getEmailInfoAgain = true;
		HomeManager.instance.ChangeScreen (UIType.ChooseGame);
	}

	#region On Button Clicked
	/*public void OnButtonFakeIpClicked ()
	{
		// --- FOR TEST --- //
		NetworkGlobal.instance.ipOneHit = "192.168.1.5";
		NetworkGlobal.instance.ipRealTime = "192.168.1.5";
		// ---------------- //
	}*/

	public void Login_OnButtonLoginClicked ()
	{
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);

		string _userName = login_fieldUserName.text.Trim();
		if (_userName.Length < 3 || _userName.Length > 25) {
			PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kWarning)
				, MyLocalize.GetString("RegisAndLogin/Username_Error_0")
				, string.Empty
				, MyLocalize.GetString(MyLocalize.kOk));
			return;
		}
		if (!_userName.IsAvailableUserNameAndPass ()) {
			PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kWarning)
				, MyLocalize.GetString(MyLocalize.kUserNameIsInvalid)
				, string.Empty
				, MyLocalize.GetString(MyLocalize.kOk));
			return;
		}
		string _pass = login_fieldPass.text.Trim();
		if (_pass.Length < 3 || _pass.Length > 25) {
			PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kWarning)
				, MyLocalize.GetString("RegisAndLogin/Password_Error_0")
				, string.Empty
				, MyLocalize.GetString(MyLocalize.kOk));
			return;
		}
		if (!_pass.IsAvailableUserNameAndPass ()) {
			PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kWarning)
				, MyLocalize.GetString("RegisAndLogin/PassIsInvalid_01")
				, string.Empty
				, MyLocalize.GetString(MyLocalize.kOk));
			return;
		}

		LoadingCanvasController.instance.Show (-1, true);
		OneHitAPI.LoginWithLocalAccount (DataManager.instance.userData.userId, _userName, _pass, 
			(_messageReceiving, _error) => {
				LoadingCanvasController.instance.Hide ();
				if(_messageReceiving != null){
					sbyte _caseCheck = _messageReceiving.readByte (); 
					switch(_caseCheck){
					case -1: // Lỗi kết nối đến database.
						PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
							, MyLocalize.GetString(MyLocalize.kSystemError)
							, _caseCheck.ToString()
							, MyLocalize.GetString(MyLocalize.kOk));
						break;
					case -9: // sai password đối với bigxu account
						PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
							, MyLocalize.GetString("RegisAndLogin/PassIsInvalid_00")
							, _caseCheck.ToString()
							, MyLocalize.GetString(MyLocalize.kOk));
						break;
					case -8: // username không tồn tại đối với bigxu account
						PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
							, MyLocalize.GetString("RegisAndLogin/UserDoesnotExist")
							, _caseCheck.ToString()
							, MyLocalize.GetString(MyLocalize.kOk));
						break;
					case 1: // thành công
						DataManager.instance.userData.password = _pass;
						DataManager.instance.userData.GetMoreUserData(_messageReceiving);

						#if TEST
						Debug.Log (">>> ReceiveLogin (Thành công) : " + DataManager.instance.userData.userId + " - " + DataManager.instance.userData.gold);
						#endif
						SetUpChangeScreenChooseGame();
						break;
					default:
						#if TEST
						Debug.LogError (">>> Trả caseCheck tào lao: " + _caseCheck);
						#endif
						PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
							, MyLocalize.GetString(MyLocalize.kSystemError)
							, _caseCheck.ToString()
							, MyLocalize.GetString(MyLocalize.kOk));
						break;
					}
				}else{
					PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
						, MyLocalize.GetString(MyLocalize.kConnectionError)
						, _error.ToString()
						, MyLocalize.GetString(MyLocalize.kOk));
				}
			});
	}

	public void Login_OnButtonPlayAsGuestClicked(){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);

		LoadingCanvasController.instance.Show (-1, true);
		OneHitAPI.LoginWithDeviceID (DataManager.instance.userData.userId, 
			(_messageReceiving, _error) => {
				LoadingCanvasController.instance.Hide ();
				if(_messageReceiving != null){
					sbyte _caseCheck = _messageReceiving.readByte (); 
					switch(_caseCheck){
					case -1: // Lỗi kết nối đến database.
						PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
							, MyLocalize.GetString(MyLocalize.kSystemError)
							, _caseCheck.ToString()
							, MyLocalize.GetString(MyLocalize.kOk));
						break;
					case 1: // thành công
						DataManager.instance.userData.GetMoreUserData(_messageReceiving);

						#if TEST
						Debug.Log (">>> ReceiveLogin (Thành công) : " + DataManager.instance.userData.userId + " - " + DataManager.instance.userData.gold);
						#endif
						SetUpChangeScreenChooseGame();
						break;
					default:
						#if TEST
						Debug.LogError (">>> Trả caseCheck tào lao: " + _caseCheck);
						#endif
						PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
							, MyLocalize.GetString(MyLocalize.kSystemError)
							, _caseCheck.ToString()
							, MyLocalize.GetString(MyLocalize.kOk));
						break;
					}
				}else{
					PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
						, MyLocalize.GetString(MyLocalize.kConnectionError)
						, _error.ToString()
						, MyLocalize.GetString(MyLocalize.kOk));
				}
			});
	}

	public void Login_OnButtonLoginFbClicked(){
		#if TEST
		Debug.Log ("Login FB");
		#endif
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);

		if (actionLoginFaceBook == null) {
			actionLoginFaceBook = FacebookAPI.DoActionLoginFb (()=>{
				CallBackLoginFb();
			}, ()=>{
				actionLoginFaceBook = null;
			});
			StartCoroutine (actionLoginFaceBook);
		}
	}

	public void Login_OnButtonOpenPanelRegisterClicked(){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);

		if(panelRegisterState == PanelRegisterState.Show){
			return;
		}

		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_TogglePanel);

		panelRegisterState = PanelRegisterState.Show;
		canvasGroupPanelRegister.alpha = 0f;
		canvasGroupPanelRegister.blocksRaycasts = true;

		LeanTween.alphaCanvas(canvasGroupPanelRegister, 1f, 0.1f);

		canvasGroupPanelRegister.transform.localScale = Vector3.one * 0.6f;
		LeanTween.scale(canvasGroupPanelRegister.gameObject, Vector3.one, 0.2f).setEase(LeanTweenType.easeOutBack);
	}

	public void Login_OnButtonForgotPasswordClicked(){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);

		PopupManager.Instance.CreatePopupForgotPassword(
			(_email, _userName, _newPass) =>{
				// byte caseCheck (1 : đổi thành công)
				LoadingCanvasController.instance.Show (-1, true);
				OneHitAPI.BigxuAccount_ForgotPassword(_email, _userName, _newPass, (_messageReceiving, _error) => {
					LoadingCanvasController.instance.Hide ();
					if(_messageReceiving != null){
						sbyte _caseCheck = _messageReceiving.readByte();
						if(_caseCheck == 1){
							// #if TEST
							// Debug.Log (">>> Đổi pass thành công");
							// #endif
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

	public void Register_OnButtonRegisterClicked(){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);

		string _userName = register_fieldUserName.text.Trim();
		if (_userName.Length < 3 || _userName.Length > 25) {
			PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kWarning)
				, MyLocalize.GetString("RegisAndLogin/Username_Error_0")
				, string.Empty
				, MyLocalize.GetString(MyLocalize.kOk));
			return;
		}
		if (!_userName.IsAvailableUserNameAndPass ()) {
			PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kWarning)
				, MyLocalize.GetString(MyLocalize.kUserNameIsInvalid)
				, string.Empty
				, MyLocalize.GetString(MyLocalize.kOk));
			return;
		}
		string _pass = register_fieldPass.text.Trim();
		if (_pass.Length < 3 || _pass.Length > 25) {
			PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kWarning)
				, MyLocalize.GetString("RegisAndLogin/Password_Error_0")
				, string.Empty
				, MyLocalize.GetString(MyLocalize.kOk));
			return;
		}
		if (!_pass.IsAvailableUserNameAndPass ()) {
			PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kWarning)
				, MyLocalize.GetString("RegisAndLogin/PassIsInvalid_01")
				, string.Empty
				, MyLocalize.GetString(MyLocalize.kOk));
			return;
		}
		string _confirmPass = register_fieldConfirmPass.text;
		if(!_pass.Equals(_confirmPass)){
			PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kWarning)
				, MyLocalize.GetString("RegisAndLogin/ConfirmPwNotMatch")
				, string.Empty
				, MyLocalize.GetString(MyLocalize.kOk));
			return;
		}

		LoadingCanvasController.instance.Show (-1, true);
		OneHitAPI.RegisterLocalAccount (_userName, _pass, (_messageReceiving, _error) => {
			LoadingCanvasController.instance.Hide ();
			if(_messageReceiving != null){
				sbyte _caseCheck = _messageReceiving.readByte (); 
				switch(_caseCheck){
				case -1: // Lỗi kết nối đến database.
					PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
						, MyLocalize.GetString(MyLocalize.kSystemError)
						, _caseCheck.ToString()
						, MyLocalize.GetString(MyLocalize.kOk));
					break;
				case -2: // UserName đã tồn tại.
					long _tmpUserId = _messageReceiving.readLong();
					PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
						, MyLocalize.GetString(MyLocalize.kUsernameAlreadyExists)
						, _caseCheck.ToString()
						, MyLocalize.GetString(MyLocalize.kOk));
					break;
				case -3: // Lỗi không xác định của đăng ký.
					PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
						, MyLocalize.GetString(MyLocalize.kUnauthorizedError)
						, _caseCheck.ToString()
						, MyLocalize.GetString(MyLocalize.kOk));
					break;
				case -4: // Password quá dài.
					PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
						, MyLocalize.GetString(MyLocalize.kPassIsTooLong)
						, _caseCheck.ToString()
						, MyLocalize.GetString(MyLocalize.kOk));
					break;
				case 1:
					sbyte _databaseIdRecieve = _messageReceiving.readByte (); // --> không cần thiết

					DataManager.instance.userData.databaseId = UserData.DatabaseType.DATABASEID_BIGXU;
					DataManager.instance.userData.userId = _messageReceiving.readLong ();
					DataManager.instance.userData.username = _userName;
					DataManager.instance.userData.password = _pass;
					DataManager.instance.userData.nameShowInGame = _userName;
					DataManager.instance.userData.avatarid = _messageReceiving.readByte ();
					DataManager.instance.userData.gold = 0;
					DataManager.instance.userData.timeCreateAccount = _messageReceiving.readLong();
					#if TEST
					Debug.Log (">>> ReceiveRegister: (Thành công) : " + DataManager.instance.userData.userId);
					#endif

					SetUpChangeScreenChooseGame();
					break;
				default:
					#if TEST
					Debug.LogError (">>> Trả caseCheck tào lao: " + _caseCheck);
					#endif
					PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
							, MyLocalize.GetString(MyLocalize.kSystemError)
							, _caseCheck.ToString()
							, MyLocalize.GetString(MyLocalize.kOk));
					break;
				}
			}else{
				PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
					, MyLocalize.GetString(MyLocalize.kConnectionError)
					, _error.ToString()
					, MyLocalize.GetString(MyLocalize.kOk));
			}
		});
	}

	public void Register_OnBtnOpenPolicyClicked(){
		Application.OpenURL(MyConstant.linkPrivacyPolicy);
	}

	public void Register_OnBtnOpenTermOfServiceClicked(){
		Application.OpenURL(MyConstant.linkTermOfService);
	}
	#endregion

	#region Login Facebook
	void CallBackLoginFb(){
		Debug.Log("Login FB successful!");
		// AccessToken class will have session details
		var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
		Debug.Log("UserID: " + aToken.UserId);

		LoadingCanvasController.instance.Show (-1, true);
		OneHitAPI.LoginWithFacebookAccount (DataManager.instance.userData.userId, aToken.TokenString,
			(_messageReceiving, _error) => {
				LoadingCanvasController.instance.Hide ();
				if(_messageReceiving != null){
					sbyte _caseCheck = _messageReceiving.readByte (); 
					switch(_caseCheck){
					case -1: // Lỗi kết nối đến database.
						PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
							, MyLocalize.GetString(MyLocalize.kSystemError)
							, _caseCheck.ToString()
							, MyLocalize.GetString(MyLocalize.kOk));
						break;
					case 1: // thành công
						DataManager.instance.userData.GetMoreUserData(_messageReceiving);

						#if TEST
						Debug.Log (">>> ReceiveLogin (Thành công) : " + DataManager.instance.userData.userId + " - " + DataManager.instance.userData.gold);
						#endif
						SetUpChangeScreenChooseGame();
						break;
					default:
						#if TEST
						Debug.LogError (">>> Trả caseCheck tào lao: " + _caseCheck);
						#endif
						PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
							, MyLocalize.GetString(MyLocalize.kSystemError)
							, _caseCheck.ToString()
							, MyLocalize.GetString(MyLocalize.kOk));
						break;
					}
				}else{
					PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
						, MyLocalize.GetString(MyLocalize.kConnectionError)
						, _error.ToString()
						, MyLocalize.GetString(MyLocalize.kOk));
				}
			});
		
	}
	#endregion
}
