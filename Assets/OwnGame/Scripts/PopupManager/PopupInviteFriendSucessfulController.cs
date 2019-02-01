using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupInviteFriendSucessfulController : IPopupController {

	System.Action onSubmit;
    [SerializeField] Text textSubmitButton;

    [Header ("Panel Child Info")]
	[SerializeField] Text child_txtGoldBonus;
	[SerializeField] RawImage child_imgAvatar_00;
	[SerializeField] Text child_txtName_00;
	[SerializeField] Image child_imgDatabaseType_00;
	[SerializeField] RawImage child_imgAvatar_01;
	[SerializeField] Text child_txtName_01;
	[SerializeField] Image child_imgDatabaseType_01;
	[SerializeField] int child_maxLengthOfUserName = 15;

	[Header ("Panel Parent Info")]
	[SerializeField] Text parent_txtGoldBonus;
	[SerializeField] RawImage parent_imgAvatar_00;
	[SerializeField] Text parent_txtName_00;
	[SerializeField] Image parent_imgDatabaseType_00;
	[SerializeField] RawImage parent_imgAvatar_01;
	[SerializeField] Text parent_txtName_01;
	[SerializeField] Image parent_imgDatabaseType_01;
	[SerializeField] int parent_maxLengthOfUserName = 15;
	
	Coroutine child_actionLoadAvatar, parent_actionLoadAvatar;

    public override void ResetData(){
		base.ResetData ();
		onSubmit = null;

		if(child_actionLoadAvatar != null){
            StopCoroutine(child_actionLoadAvatar);
            child_actionLoadAvatar = null;
        }

		if(parent_actionLoadAvatar != null){
            StopCoroutine(parent_actionLoadAvatar);
            parent_actionLoadAvatar = null;
        }
	}
	
    public void InitData(UserData _childInfo, UserData _parentInfo, long _goldBonus, string _textSubmitButton, System.Action _onSubmit = null, System.Action _onClose = null){
        if(!string.IsNullOrEmpty(_textSubmitButton)){
			textSubmitButton.text = _textSubmitButton;
		}else{
			textSubmitButton.text = MyLocalize.GetString(MyLocalize.kOk);
		}

		InitChildInfo(_childInfo, _goldBonus);
		InitParentInfo(_parentInfo, _goldBonus);

        onSubmit = _onSubmit;
		onClose = _onClose;
		CoreGameManager.instance.RegisterNewCallbackPressBackKey (OnButtonSubmitClicked);

		Show();
	}

    void InitChildInfo(UserData _childInfo, long _goldBonus){
		string _childName = MyConstant.ConvertString(_childInfo.nameShowInGame, child_maxLengthOfUserName);
		child_txtName_00.text = _childName;
		child_txtName_01.text = _childName;

		child_txtGoldBonus.text = "+" + MyConstant.GetMoneyString(_goldBonus, 9999);
		
		Sprite _iconDatabaseID = _childInfo.GetIconDatabaseID();
		if(_iconDatabaseID != null){
			child_imgDatabaseType_00.gameObject.SetActive(true);
			child_imgDatabaseType_01.gameObject.SetActive(true);
			
			child_imgDatabaseType_00.sprite = _iconDatabaseID;
			child_imgDatabaseType_01.sprite = _iconDatabaseID;
		}else{
			child_imgDatabaseType_00.gameObject.SetActive(false);
			child_imgDatabaseType_01.gameObject.SetActive(false);
		}

		if(child_actionLoadAvatar != null){
			StopCoroutine(child_actionLoadAvatar);
			child_actionLoadAvatar = null;
		}
		
		child_imgAvatar_00.texture = GameInformation.instance.otherInfo.avatarDefault;
		child_imgAvatar_01.texture = GameInformation.instance.otherInfo.avatarDefault;
		child_actionLoadAvatar = _childInfo.LoadAvatar(this, child_imgAvatar_00.rectTransform.rect.width, child_imgAvatar_00.rectTransform.rect.height,
			(_avatar) =>
			{
				try{
					if(_avatar != null){
						child_imgAvatar_00.texture = _avatar;
						child_imgAvatar_01.texture = _avatar;
					}
				}catch{}
				child_actionLoadAvatar = null;
			});
	}

	void InitParentInfo(UserData _parentInfo, long _goldBonus){
		string _childName = MyConstant.ConvertString(_parentInfo.nameShowInGame, parent_maxLengthOfUserName);
		parent_txtName_00.text = _childName;
		parent_txtName_01.text = _childName;

		parent_txtGoldBonus.text = "+" + MyConstant.GetMoneyString(_goldBonus, 9999);
		
		Sprite _iconDatabaseID = _parentInfo.GetIconDatabaseID();
		if(_iconDatabaseID != null){
			parent_imgDatabaseType_00.gameObject.SetActive(true);
			parent_imgDatabaseType_01.gameObject.SetActive(true);

			parent_imgDatabaseType_00.sprite = _iconDatabaseID;
			parent_imgDatabaseType_01.sprite = _iconDatabaseID;
		}else{
			parent_imgDatabaseType_00.gameObject.SetActive(false);
			parent_imgDatabaseType_01.gameObject.SetActive(false);
		}

		if(parent_actionLoadAvatar != null){
			StopCoroutine(parent_actionLoadAvatar);
			parent_actionLoadAvatar = null;
		}
		
		parent_imgAvatar_00.texture = GameInformation.instance.otherInfo.avatarDefault;
		parent_imgAvatar_01.texture = GameInformation.instance.otherInfo.avatarDefault;
		parent_actionLoadAvatar = _parentInfo.LoadAvatar(this, parent_imgAvatar_00.rectTransform.rect.width, parent_imgAvatar_00.rectTransform.rect.height,
			(_avatar) =>
			{
				try{
					if(_avatar != null){
						parent_imgAvatar_00.texture = _avatar;
						parent_imgAvatar_01.texture = _avatar;
					}
				}catch{}
				parent_actionLoadAvatar = null;
			});
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
