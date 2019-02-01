using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupJoinTableController : IPopupController {

	System.Action<string, string> onSubmit;
	System.Action onCancel;
	[SerializeField] Text textTitle;
	[SerializeField] Text textTitleEnterId;
	[SerializeField] InputField inputEnterId;
	[SerializeField] Text textPlaceHolderEnterId;
	[SerializeField] Text textTitleEnterPass;
	[SerializeField] InputField inputEnterPass;
	[SerializeField] Text textPlaceHolderEnterPass;
	[SerializeField] Text textSubmitButton, textCancelButton;

	public override void ResetData ()
	{
		base.ResetData ();
		onSubmit = null;
		onCancel = null;
	}

	public void Init(string _defaultId, System.Action<string, string> _onSubmit, System.Action _onCancel, System.Action _onClose = null){
		textTitle.text = MyLocalize.GetString("ChooseTable/CreateTable_TitleJoinTable");

		textTitleEnterId.text = MyLocalize.GetString("ChooseTable/CreateTable_TitEnterId");
		textPlaceHolderEnterId.text = MyLocalize.GetString("ChooseTable/CreateTable_PlaceHolder_EnterID");

		textTitleEnterPass.text = MyLocalize.GetString("ChooseTable/CreateTable_TitEnterPass");
		textPlaceHolderEnterPass.text = MyLocalize.GetString("ChooseTable/CreateTable_PlaceHolder_EnterPass");

		textSubmitButton.text = MyLocalize.GetString(MyLocalize.kOk);
		textCancelButton.text = MyLocalize.GetString(MyLocalize.kCancel);
		inputEnterPass.text = "";
		inputEnterId.text = _defaultId;
		
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
				onSubmit(inputEnterId.text, inputEnterPass.text);
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
