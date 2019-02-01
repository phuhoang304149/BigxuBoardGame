using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupCreateTableController : IPopupController {

	System.Action<string> onSubmit;
	System.Action onCancel;
	[SerializeField] Text textTitle;
	[SerializeField] Text textSetPass;
	[SerializeField] Text textPlaceHolderTypePass;
	[SerializeField] InputField inputTypePass;
	[SerializeField] Text textSubmitButton, textCancelButton;

	public override void ResetData ()
	{
		base.ResetData ();
		onSubmit = null;
		onCancel = null;
	}

	public void Init(System.Action<string> _onSubmit, System.Action _onCancel, System.Action _onClose = null){
		textTitle.text = MyLocalize.GetString("ChooseTable/CreateTable_TitleCreateTable");
		textSetPass.text = MyLocalize.GetString("ChooseTable/CreateTable_TitSetPass");
		textPlaceHolderTypePass.text = MyLocalize.GetString("ChooseTable/CreateTable_PlaceHolder_EnterPass");
		textSubmitButton.text = MyLocalize.GetString(MyLocalize.kOk);
		textCancelButton.text = MyLocalize.GetString(MyLocalize.kCancel);
		inputTypePass.text = "";
		
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
				onSubmit(inputTypePass.text);
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
