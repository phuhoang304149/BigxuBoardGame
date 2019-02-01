using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragonTiger_HistoryB_OptionInfo_Controller : MonoBehaviour {

	[SerializeField] Image imgIcon;

	[Header("Setting")]
	[SerializeField] Color colorPanelDragon;
	[SerializeField] Color colorPanelTie;
	[SerializeField] Color colorPanelTiger;
	[SerializeField] Color colorPanelDefault;

	public void InitData(DragonTiger_GamePlay_Manager.IndexBet _type){
		switch(_type){
		case DragonTiger_GamePlay_Manager.IndexBet.Dragon:
			imgIcon.color = colorPanelDragon;
			break;
		case DragonTiger_GamePlay_Manager.IndexBet.Tie:
			imgIcon.color = colorPanelTie;
			break;
		case DragonTiger_GamePlay_Manager.IndexBet.Tiger:
			imgIcon.color = colorPanelTiger;
			break;
		}
	}

	public void SetEmpty(){
		imgIcon.color = colorPanelDefault;
	}
}
