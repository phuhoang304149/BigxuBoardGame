using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Uno_FinishGame_ScoreBoard_Option_Controller : MySimplePoolObjectController {
	[SerializeField] Image imgStar;
	[SerializeField] Text txtGoldBonus;
	[SerializeField] Text txtName;
	[SerializeField] Text txtPoint;
	[SerializeField] Transform panelShadow;

	[Header("Setting")]
	[SerializeField] int maxLengthOfUserName;

	public void InitData(bool _isWin, string _name, long _goldBonus, int _point){
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
		txtName.text = MyConstant.ConvertString(_name, maxLengthOfUserName);
		txtPoint.text = "" + MyConstant.GetMoneyString(_point);
	}
}
