using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupDialogController : IPopupController {
	System.Action onSubmit, onCancel;
	[SerializeField] Text textTitle;
	[SerializeField] Text textMessage;
	[SerializeField] Text txtErrorCode;
	[SerializeField] Text textSubmitButton, textCancelButton;

	public override void ResetData ()
	{
		base.ResetData ();
		txtErrorCode.text = string.Empty;
		onClose = null;
		onCancel = null;
	}

	public void Init(string _textTitle, string _textMessage, string _errorCode, string _textSubmitButton, string _textCancelButton, System.Action _onSubmit, System.Action _onCancel, System.Action _onClose = null){
		textTitle.text = _textTitle;
		textMessage.text = _textMessage;
		txtErrorCode.text = _errorCode;
		if(!string.IsNullOrEmpty(_textSubmitButton)){
			textSubmitButton.text = _textSubmitButton;
		}else{
			textSubmitButton.text = MyLocalize.GetString(MyLocalize.kYes);
		}
		if(!string.IsNullOrEmpty(_textCancelButton)){
			textCancelButton.text = _textCancelButton;
		}else{
			textCancelButton.text = MyLocalize.GetString(MyLocalize.kNo);
		}
		onClose = _onClose;
		onSubmit = _onSubmit;
		onCancel = _onCancel;

		CoreGameManager.instance.RegisterNewCallbackPressBackKey (OnButtonNoClicked);

		Show();
	}

	public void OnButtonYesClicked(){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);

		Hide(()=>{
			if(onSubmit != null){
				onSubmit();
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
			Close ();
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
