using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragonTiger_HistoryA_OptionInfo_Controller : MonoBehaviour {

	[SerializeField] Image imgBg;
	[SerializeField] Text txtInfo;
	
	[Header("Setting")]
	[SerializeField] Color colorPanelDragon;
	[SerializeField] Color colorPanelTie;
	[SerializeField] Color colorPanelTiger;
	[SerializeField] Color colorPanelDefault;


	public void InitData(DragonTiger_GamePlay_Manager.IndexBet _type){
		switch(_type){
		case DragonTiger_GamePlay_Manager.IndexBet.Dragon:
			imgBg.color = colorPanelDragon;
			txtInfo.text = "D";
			break;
		case DragonTiger_GamePlay_Manager.IndexBet.Tie:
			imgBg.color = colorPanelTie;
			txtInfo.text = "T";
			break;
		case DragonTiger_GamePlay_Manager.IndexBet.Tiger:
			imgBg.color = colorPanelTiger;
			txtInfo.text = "T";
			break;
		}
	}

	public void SetEmpty(){
		imgBg.color = colorPanelDefault;
		txtInfo.text = "";
	}
}
