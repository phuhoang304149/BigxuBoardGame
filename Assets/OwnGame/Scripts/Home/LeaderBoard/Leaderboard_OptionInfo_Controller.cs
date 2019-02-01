using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;

public class Leaderboard_OptionInfo_Controller : MySimplePoolObjectController {

	[SerializeField] Text txtRank;
	[SerializeField] Image imgIconAcc;
	[SerializeField] RawImage imgAvatar;
	[SerializeField] Text txtNameShow;
	[SerializeField] Text txtUserId;
	[SerializeField] Text txtTotalGold;
	// [SerializeField] Text txtTotalGem;

	[Header("Setting")]
	[SerializeField] int maxLengthOfUserName;

	UserData userData;

	public void InitData(int _rank, UserData _userData){
		userData = _userData;
		txtRank.text = _rank.ToString();
		if(_rank == 1){
			txtRank.text += "st";
		}else if(_rank == 2){
			txtRank.text += "nd";
		}else if(_rank == 3){
			txtRank.text += "rd";
		}else{
			txtRank.text += "th";
		}

		Sprite _iconDatabaseID = userData.GetIconDatabaseID();
		if(_iconDatabaseID != null){
			imgIconAcc.gameObject.SetActive(true);
			imgIconAcc.sprite = _iconDatabaseID;
		}else{
			imgIconAcc.gameObject.SetActive(false);
		}

		txtNameShow.text = MyConstant.ConvertString(userData.nameShowInGame , maxLengthOfUserName);
		txtUserId.text = "ID " + userData.userId;
		txtTotalGold.text = MyConstant.GetMoneyString(userData.gold);
		// txtTotalGem.text = MyConstant.GetMoneyString(_userData.gem);
		
		userData.LoadAvatar(this, imgAvatar.rectTransform.rect.width, imgAvatar.rectTransform.rect.height, (_avatar) => {
			try{
				if(_avatar != null){
					imgAvatar.texture = _avatar;
				}
			}catch{}
		});
	}
}
