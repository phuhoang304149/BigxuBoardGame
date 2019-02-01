using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterScreenController : UIHomeScreenController {

	public override UIType myType{
		get{ 
			return UIType.RegisterScreen;
		}
	}
	public InputField fieldUserName;
	public InputField fieldPass;

	#region Init / Show / Hide
	public override void InitData ()
	{
		onPressBack = () => {
			HomeManager.instance.ChangeScreen (UIType.LoginScreen);
		};
		if (onPressBack != null) {
			CoreGameManager.instance.RegisterNewCallbackPressBackKey (onPressBack);
		}
	}

	public override void Hide ()
	{
		if (onPressBack != null) {
			CoreGameManager.instance.RemoveCurrentCallbackPressBackKey (onPressBack);
		}
		base.Hide ();
	}
	#endregion

	#region On Button Clicked
	public void OnButtonRegisterClicked(){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);

		string _userName = fieldUserName.text;
		if (!_userName.IsAvailableUserNameAndPass ()) {
			PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
				, MyLocalize.GetString(MyLocalize.kUserNameIsInvalid)
				, string.Empty
				, MyLocalize.GetString(MyLocalize.kOk));
			return;
		}
		string _pass = fieldPass.text;
		if (!_pass.IsAvailableUserNameAndPass ()) {
			PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
				, MyLocalize.GetString(MyLocalize.kPassIsInvalid_01)
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
					string _userNameRecieve = _messageReceiving.readString(); // --> không cần thiết
					string _passRecieve = _messageReceiving.readString(); // --> không cần thiết

					DataManager.instance.userData.databaseId = UserData.DatabaseType.DATABASEID_BIGXU;
					DataManager.instance.userData.userId = _messageReceiving.readLong ();
					DataManager.instance.userData.username = fieldUserName.text;
					DataManager.instance.userData.password = fieldPass.text;
					DataManager.instance.userData.nameShowInGame = DataManager.instance.userData.username;
					DataManager.instance.userData.avatarid = _messageReceiving.readByte ();
					DataManager.instance.userData.gold = _messageReceiving.readLong ();
					DataManager.instance.userData.timeCreateAccount = _messageReceiving.readLong();
					// DataManager.instance.userData.lastTimePlay = _messageReceiving.readLong ();
					#if TEST
					Debug.Log (">>> ReceiveRegister: (Thành công) : " + DataManager.instance.userData.userId);
					#endif

					HomeManager.instance.ChangeScreen (UIType.ChooseGame);
					break;
				default:
					#if TEST
					Debug.LogError (">>> Trả caseCheck tào lao: " + _caseCheck);
					#endif
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
