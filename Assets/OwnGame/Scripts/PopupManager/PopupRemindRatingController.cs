using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupRemindRatingController : IPopupController {
	[SerializeField] Text textTitle;
	[SerializeField] Text textMessage;
	[SerializeField] Text textSubmitButton, textCancelButton;
	System.Action onSubmit, onCancel;

	public override void ResetData ()
	{
		base.ResetData ();
		onClose = null;
		onCancel = null;
	}

	public void Init(System.Action _onSubmit, System.Action _onCancel, System.Action _onClose = null){
		textTitle.text = MyLocalize.GetString("System/Message_RemindRating_00");
		textMessage.text = MyLocalize.GetString("System/Message_RemindRating_01");

		textSubmitButton.text = MyLocalize.GetString("Global/RateUs");
		textCancelButton.text = MyLocalize.GetString(MyLocalize.kCancel);
		
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
