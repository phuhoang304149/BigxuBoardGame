using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
*	Uno_History_OptionType00_Controller : Option dành cho player không show bài
**/
public class Uno_History_OptionType00_Controller : MySimplePoolObjectController {

	[SerializeField] Image imgStar;
	[SerializeField] Text txtGoldBonus;
	[SerializeField] RawImage imgAvatar;
	[SerializeField] Text txtName;
	[SerializeField] Text txtPoint;
	[SerializeField] Transform panelShadow;

	[Header("Setting")]
	[SerializeField] int maxLengthOfUserName;

	public UserDataInGame data{get;set;}

	public override void ResetData(){
		if(data != null){
			data = null;
		}
	}

	public void InitData(UserDataInGame _userData, bool _isWin, long _goldBonus, int _point){
		data = _userData;

		if(_isWin){
			imgStar.gameObject.SetActive(true);
			panelShadow.gameObject.SetActive(false);
			txtGoldBonus.text = "+" + MyConstant.GetMoneyString(_goldBonus, 9999);
			txtName.color = Color.yellow;
			txtPoint.color = Color.yellow;
		}else{
			imgStar.gameObject.SetActive(false);
			panelShadow.gameObject.SetActive(true);
			txtGoldBonus.text = string.Empty;
			txtName.color = Color.white;
			txtPoint.color = Color.white;
		}

		txtName.text = MyConstant.ConvertString(data.nameShowInGame, maxLengthOfUserName);
		txtPoint.text = "" + MyConstant.GetMoneyString(_point);

		imgAvatar.texture = CoreGameManager.instance.gameInfomation.otherInfo.avatarDefault;
		data.LoadAvatar(this, imgAvatar.rectTransform.rect.width, imgAvatar.rectTransform.rect.height, (_avatar) => {
			try{
				if(_avatar != null){
					imgAvatar.texture = _avatar;
				}
			}catch{}
		});
	}
}
