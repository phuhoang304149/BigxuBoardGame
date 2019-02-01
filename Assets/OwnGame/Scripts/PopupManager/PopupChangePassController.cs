using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupChangePassController : IPopupController {
	[SerializeField] Text textTitle;
	[SerializeField] InputField fieldCurrentPassword;
	[SerializeField] Text textPlaceHolderCurrentPassword;
	[SerializeField] InputField fieldNewPassword;
	[SerializeField] Text textPlaceHolderNewPassword;
	[SerializeField] InputField fieldConfirmNewPassword;
	[SerializeField] Text textPlaceHolderConfirmNewPassword;
	[SerializeField] Text textSubmitButton, textCancelButton;

	string strTitle, strFieldCurrentPass, strFieldNewPass, strFieldConfirmNewPass;

	System.Action<string, string> onSubmit;
	System.Action onCancel;

	public override void ResetData ()
	{
		base.ResetData ();
		onSubmit = null;
		onCancel = null;
	}

	public void Init(System.Action<string, string> _onSubmit, System.Action _onCancel, System.Action _onClose = null){
		strTitle = MyLocalize.GetString("RegisAndLogin/Field_ChangePassword");
		strFieldCurrentPass = MyLocalize.GetString("RegisAndLogin/Field_CurrentPassword");
		strFieldNewPass = MyLocalize.GetString("RegisAndLogin/Field_NewPassword");
		strFieldConfirmNewPass = MyLocalize.GetString("RegisAndLogin/Field_ConfirmNewPassword");
		
		textTitle.text = strTitle;
		
		fieldCurrentPassword.text = string.Empty;
		fieldNewPassword.text = string.Empty;
		fieldConfirmNewPassword.text = string.Empty;

		textPlaceHolderCurrentPassword.text = strFieldCurrentPass + "...";
		textPlaceHolderNewPassword.text = strFieldNewPass + "...";
		textPlaceHolderConfirmNewPassword.text = strFieldConfirmNewPass + "...";

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

		string _currentPass = fieldCurrentPassword.text.Trim();
		string _newPass = fieldNewPassword.text.Trim();
		
		if (string.IsNullOrEmpty (_currentPass)) {
			PopupManager.Instance.CreateToast(
				string.Format(MyLocalize.GetString("RegisAndLogin/Message_PlsTypeYourField"), fieldCurrentPassword));
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

		string _confirmNewPass = fieldConfirmNewPassword.text.Trim();
		if(!_newPass.Equals(_confirmNewPass)){
			PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kWarning)
				, MyLocalize.GetString("RegisAndLogin/ConfirmPwNotMatch")
				, string.Empty
				, MyLocalize.GetString(MyLocalize.kOk));
			return;
		}

		Hide(()=>{
			if(onSubmit != null){
				onSubmit(_currentPass, _newPass);
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
