using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupMessageController : IPopupController {
	System.Action onSubmit;
	[SerializeField] Text textTitle;
	[SerializeField] Text textMessage;
	[SerializeField] Text textSubmitButton;
	[SerializeField] Text txtErrorCode;

	public override void ResetData ()
	{
		base.ResetData ();
		onSubmit = null;
		txtErrorCode.text = string.Empty;
	}

	public void Init(string _textTitle, string _textMessage, string _errorCode, string _textSubmitButton, System.Action _onSubmit = null, System.Action _onClose = null){
		textTitle.text = _textTitle;
		textMessage.text = _textMessage;
		txtErrorCode.text = _errorCode;
		if(!string.IsNullOrEmpty(_textSubmitButton)){
			textSubmitButton.text = _textSubmitButton;
		}else{
			textSubmitButton.text = MyLocalize.GetString(MyLocalize.kOk);
		}
		onSubmit = _onSubmit;
		onClose = _onClose;
		CoreGameManager.instance.RegisterNewCallbackPressBackKey (OnButtonSubmitClicked);

		Show();
	}

	public void OnButtonSubmitClicked(){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);

		Hide(()=>{
			if(onSubmit != null){
				onSubmit.Invoke();
				onSubmit = null;
			}
			Close();
		});
	}

	public override void Close (){
		if(onClose != null){
			onClose.Invoke();
			onClose = null;
		}
		CoreGameManager.instance.RemoveCurrentCallbackPressBackKey (OnButtonSubmitClicked);
		SelfDestruction();
	}
}
