using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserDetail_PanelAchievementOption_Controller : MonoBehaviour {

	public Image iconGame;
	public Text txtNameGame;
	public Text txtCountWin;
	public Text txtCountDraw;
	public Text txtCountLose;
	AchievementDetail achievementData;

	public void InitData(AchievementDetail _achievementData){
		achievementData = _achievementData;
		var _obj = achievementData.myGameInfo.gameAvatar.Load();
		if(_obj != null){
			iconGame.sprite = (Sprite) _obj;
		}
		txtNameGame.text = achievementData.myGameInfo.myName;
		txtCountWin.text = achievementData.countWin.ToString();
		txtCountDraw.text = achievementData.countDraw.ToString();
		txtCountLose.text = achievementData.countLose.ToString();
	}
}
