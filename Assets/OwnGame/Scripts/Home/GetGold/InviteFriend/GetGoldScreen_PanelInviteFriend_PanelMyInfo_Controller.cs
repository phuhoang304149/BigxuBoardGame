using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetGoldScreen_PanelInviteFriend_PanelMyInfo_Controller : MonoBehaviour {

	[SerializeField] CanvasGroup myCanvasGroup;
	[SerializeField] Text txtYouHaveBeenInvitedBy;
	[SerializeField] RawImage imgAvatar;
	[SerializeField] Text txtName;
	[SerializeField] Image imgDatabaseType;
	[SerializeField] Text txtMyInviteCode;

	[Header("Setting")]
	[SerializeField] int maxLengthOfUserName = 25;

	Coroutine actionLoadAvatar;

	public void ResetData(){
		myCanvasGroup.alpha = 0f;
		myCanvasGroup.blocksRaycasts = false;

		if(actionLoadAvatar != null){
            StopCoroutine(actionLoadAvatar);
            actionLoadAvatar = null;
        }

		imgAvatar.texture = GameInformation.instance.otherInfo.avatarDefault;
		imgDatabaseType.sprite = GameInformation.instance.otherInfo.iconAccDevice;
		txtName.text = "Unknown";
	}

	public void InitData(){
		txtYouHaveBeenInvitedBy.text = MyLocalize.GetString("InviteFriend/YouHaveBeenInvitedBy");
		txtMyInviteCode.text = string.Format(MyLocalize.GetString("InviteFriend/YourInvitationCode"), DataManager.instance.userData.sessionId);
		txtName.text = MyConstant.ConvertString(DataManager.instance.parentUserData.nameShowInGame, maxLengthOfUserName);
		
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
		
		actionLoadAvatar = DataManager.instance.parentUserData.LoadAvatar(this, imgAvatar.rectTransform.rect.width, imgAvatar.rectTransform.rect.height,
			(_avatar) =>
			{
				try{
					if(_avatar != null){
						imgAvatar.texture = _avatar;
					}
				}catch{}
				actionLoadAvatar = null;
			});
	}

	public void Show(){
		myCanvasGroup.alpha = 1f;
		myCanvasGroup.blocksRaycasts = true;
	}

	public void Hide(){
		myCanvasGroup.alpha = 0f;
		myCanvasGroup.blocksRaycasts = false;
		ResetData();
	}
}
