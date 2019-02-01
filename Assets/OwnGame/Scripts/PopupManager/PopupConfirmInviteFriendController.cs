using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupConfirmInviteFriendController : IPopupController {

	System.Action onSubmit, onCancel;
	[SerializeField] Text textTitle;
	[SerializeField] Text textSubmitButton, textCancelButton;

	[SerializeField] RawImage imgAvatar;
	[SerializeField] Text txtName;
	[SerializeField] Image imgDatabaseType;

	[Header("Setting")]
	[SerializeField] int maxLengthOfUserName = 25;

	Coroutine actionLoadAvatar;

	public override void ResetData ()
	{
		base.ResetData ();
		onClose = null;
		onCancel = null;

		myCanvasGroup.alpha = 0;
		myCanvasGroup.blocksRaycasts = false;

		if(actionLoadAvatar != null){
            StopCoroutine(actionLoadAvatar);
            actionLoadAvatar = null;
        }

		imgAvatar.texture = GameInformation.instance.otherInfo.avatarDefault;
		imgDatabaseType.sprite = GameInformation.instance.otherInfo.iconAccDevice;
		txtName.text = "Unknown";
	}

	public void InitData(UserData _userData, string _textSubmitButton, string _textCancelButton, System.Action _onSubmit, System.Action _onCancel, System.Action _onClose = null){
		textTitle.text = MyLocalize.GetString("InviteFriend/TitleConfirm");
		txtName.text = MyConstant.ConvertString(_userData.nameShowInGame, maxLengthOfUserName);
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

		Sprite _iconDatabaseID = DataManager.instance.userData.GetIconDatabaseID();
		if(_iconDatabaseID != null){
			imgDatabaseType.gameObject.SetActive(true);
			imgDatabaseType.sprite = _iconDatabaseID;
		}else{
			imgDatabaseType.gameObject.SetActive(false);
		}

		if(actionLoadAvatar != null){
			StopCoroutine(actionLoadAvatar);
			actionLoadAvatar = null;
		}
		
		actionLoadAvatar = _userData.LoadAvatar(this, imgAvatar.rectTransform.rect.width, imgAvatar.rectTransform.rect.height,
			(_avatar) =>
			{
				try{
					if(_avatar != null){
						imgAvatar.texture = _avatar;
					}
				}catch{}
				actionLoadAvatar = null;
			});

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
