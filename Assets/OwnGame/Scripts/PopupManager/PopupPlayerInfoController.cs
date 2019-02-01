using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupPlayerInfoController : IPopupController {

	[SerializeField] RawImage imgAvatar;
	[SerializeField] Image imgIconAcc;
	[SerializeField] Text txtNameShow;
	[SerializeField] Text txtGold;
	[SerializeField] Text txtAchievement_Win;
	[SerializeField] Text txtAchievement_Tie;
	[SerializeField] Text txtAchievement_Lose;

	[Header("Setting")]
	[SerializeField] int maxLengthOfUserName;
	UserDataInGame data;
	public override void ResetData ()

	{
		base.ResetData ();
		data = null;
	}

	public void Init(UserDataInGame _data, System.Action _onClose = null){
		data = _data;

		Sprite _iconDatabaseID = data.GetIconDatabaseID();
		if(_iconDatabaseID != null){
			imgIconAcc.gameObject.SetActive(true);
			imgIconAcc.sprite = _iconDatabaseID;
		}else{
			imgIconAcc.gameObject.SetActive(false);
		}

		txtNameShow.text = MyConstant.ConvertString(data.nameShowInGame , maxLengthOfUserName);
		txtGold.text = MyConstant.GetMoneyString(data.gold, 999999999);

		txtAchievement_Win.text = MyConstant.GetMoneyString(data.win);
		txtAchievement_Tie.text = MyConstant.GetMoneyString(data.tie);
		txtAchievement_Lose.text = MyConstant.GetMoneyString(data.lose);

		data.LoadAvatar(this, imgAvatar.rectTransform.rect.width, imgAvatar.rectTransform.rect.height, (_avatar) => {
			try{
				if(_avatar != null){
					imgAvatar.texture = _avatar;
				}
			}catch{}
		});

		onClose = _onClose;
		CoreGameManager.instance.RegisterNewCallbackPressBackKey (Close);

		Show();
	}

	public override void Close (){
		Hide(()=>{
			if(onClose != null){
				onClose.Invoke();
				onClose = null;
			}
			CoreGameManager.instance.RemoveCurrentCallbackPressBackKey (Close);		
			SelfDestruction();
		});
	}
}
