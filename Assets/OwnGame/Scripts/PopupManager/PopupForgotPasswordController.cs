using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupForgotPasswordController : IPopupController {
	[SerializeField] Text textTitle;
	[SerializeField] InputField inputEmail;
	[SerializeField] Text textPlaceHolderEmail;
	[SerializeField] InputField inputUserName;
	[SerializeField] Text textPlaceHolderUserName;
	[SerializeField] InputField inputNewPassword;
	[SerializeField] Text textPlaceHolderNewPassword;
	[SerializeField] InputField inputConfirmNewPassword;
	[SerializeField] Text textPlaceHolderConfirmNewPassword;
	[SerializeField] Text textSubmitButton, textCancelButton;
	System.Action<string, string, string> onSubmit;
	System.Action onCancel;

	string strTitle, strFieldEmail, strFieldUserName, strFieldNewPassword, strFieldConfirmNewPassword;

	public override void ResetData ()
	{
		base.ResetData ();
		onSubmit = null;
		onCancel = null;
	}

	public void Init(System.Action<string, string, string> _onSubmit, System.Action _onCancel, System.Action _onClose = null){
		strTitle = MyLocalize.GetString("RegisAndLogin/Field_ForgotPassword");
		strFieldEmail = MyLocalize.GetString("RegisAndLogin/Field_Email");
		strFieldUserName = MyLocalize.GetString("RegisAndLogin/Field_UserName");
		strFieldNewPassword = MyLocalize.GetString("RegisAndLogin/Field_NewPassword");
		strFieldConfirmNewPassword = MyLocalize.GetString("RegisAndLogin/Field_ConfirmNewPassword");

		textTitle.text = strTitle;
		
		inputEmail.text = string.Empty;
		inputUserName.text = string.Empty;
		inputNewPassword.text = string.Empty;
		inputConfirmNewPassword.text = string.Empty;

		textPlaceHolderEmail.text = strFieldEmail + "...";
		textPlaceHolderUserName.text = strFieldUserName + "...";
		textPlaceHolderNewPassword.text = strFieldNewPassword + "...";
		textPlaceHolderConfirmNewPassword.text = strFieldConfirmNewPassword + "...";

		textSubmitButton.text = MyLocalize.GetString(MyLocalize.kOk);
		textCancelButton.text = MyLocalize.GetString(MyLocalize.kCancel);
		
		onClose = _onClose;
		onSubmit = _onSubmit;
		onCancel = _onCancel;

		CoreGameManager.instance.RegisterNewCallbackPressBackKey (OnButtonNoClicked);

		Show();
	}

	public void OnButtonYesClicked(){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);

		string _email = inputEmail.text.Trim();
		string _userName = inputUserName.text.Trim();
		string _newPass = inputNewPassword.text.Trim();
		string _confirmNewPass = inputConfirmNewPassword.text.Trim();

		if (string.IsNullOrEmpty (_email)) {
			PopupManager.Instance.CreateToast(
				string.Format(MyLocalize.GetString("RegisAndLogin/Message_PlsTypeYourField"), strFieldEmail));
			return;
		}
		if (_email.Length < 9) {
			PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kWarning)
				, MyLocalize.GetString("RegisAndLogin/EmailIsTooShort")
				, string.Empty
				, MyLocalize.GetString(MyLocalize.kOk));
			return;
		}
		if (!_email.Contains ("@")) {
			PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kWarning)
				, MyLocalize.GetString("RegisAndLogin/EmailIsInvalid")
				, string.Empty
				, MyLocalize.GetString(MyLocalize.kOk));
			return;
		}

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

		if (_newPass.Length < 3 || _newPass.Length > 25) {
			PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kWarning)
				, MyLocalize.GetString("RegisAndLogin/Password_Error_0")
				, string.Empty
				, MyLocalize.GetString(MyLocalize.kOk));
			return;
		}
		if (!_newPass.IsAvailableUserNameAndPass ()) {
			PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kWarning)
				, MyLocalize.GetString(MyLocalize.kPassIsInvalid_01)
				, string.Empty
				, MyLocalize.GetString(MyLocalize.kOk));
			return;
		}

		if(!_newPass.Equals(_confirmNewPass)){
			PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kWarning)
				, MyLocalize.GetString("RegisAndLogin/ConfirmPwNotMatch")
				, string.Empty
				, MyLocalize.GetString(MyLocalize.kOk));
			return;
		}

		Hide(()=>{
			if(onSubmit != null){
				onSubmit(_email, _userName, _newPass);
				onSubmit = null;
			}
			Close ();
		});
	}
	public void OnButtonNoClicked(){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);

		Hide(()=>{
			if(onCancel != null){
				onCancel();
				onCancel = null;
			}
			Close();
		});
	}

	public override void Close (){
		if(onClose != null){
			onClose.Invoke();
			onClose = null;
		}
		CoreGameManager.instance.RemoveCurrentCallbackPressBackKey (OnButtonNoClicked);
		SelfDestruction();
	}
}
