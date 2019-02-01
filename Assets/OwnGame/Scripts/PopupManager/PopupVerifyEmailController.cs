using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupVerifyEmailController : IPopupController {
	[SerializeField] Text textTitle;
	[SerializeField] InputField inputEmail;
	[SerializeField] Text textPlaceHolderEmail;
	[SerializeField] InputField inputConfirmEmail;
	[SerializeField] Text textPlaceHolderConfirmEmail;
	
	[SerializeField] Text textSubmitButton, textCancelButton;

	string strTitle, strFieldEmail, strFieldConfirmEmail;

	System.Action<string> onSubmit;
	System.Action onCancel;

	public override void ResetData ()
	{
		base.ResetData ();
		onSubmit = null;
		onCancel = null;
	}

	public void Init(System.Action<string> _onSubmit, System.Action _onCancel, System.Action _onClose = null){
		strTitle = MyLocalize.GetString("RegisAndLogin/Field_VerifyEmail");
		strFieldEmail = MyLocalize.GetString("RegisAndLogin/Field_Email");
		strFieldConfirmEmail = MyLocalize.GetString("RegisAndLogin/Field_ConfirmEmail");
		
		textTitle.text = strTitle;

		inputEmail.gameObject.SetActive(true);
		inputEmail.text = string.Empty;
		textPlaceHolderEmail.text = strFieldEmail + "...";

		inputConfirmEmail.gameObject.SetActive(true);
		inputConfirmEmail.text = string.Empty;
		textPlaceHolderConfirmEmail.text = strFieldConfirmEmail + "...";
		
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
		string _confirmEmail = inputConfirmEmail.text.Trim();

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
		if (!_email.IsAvailableEmail ()) {
			PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kWarning)
				, MyLocalize.GetString("RegisAndLogin/EmailIsInvalid")
				, string.Empty
				, MyLocalize.GetString(MyLocalize.kOk));
			return;
		}
		if(!_email.Equals(_confirmEmail)){
			PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kWarning)
				, MyLocalize.GetString("RegisAndLogin/ConfirmEmailNotMatch")
				, string.Empty
				, MyLocalize.GetString(MyLocalize.kOk));
			return;
		}

		Hide(()=>{
			if(onSubmit != null){
				onSubmit(_email);
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
