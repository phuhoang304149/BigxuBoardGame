using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Poker_SupportOptionInfo_Controller : MonoBehaviour {

	[SerializeField] Text txtTypeCard;
	[SerializeField] Text txtPercent;

	public void ResetData(){
		txtPercent.text = "??%";
	}

	public void InitData(float _percent){
		string _show = "";
		int _tmpValue = (int) (_percent * 100);
		if(_tmpValue % 100 == 0){
			_show = _percent.ToFixed(0);
		}else if(_tmpValue % 10 == 0){
			_show = _percent.ToFixed(1);
		}else{
			_show = _percent.ToFixed(2);
		}
		txtPercent.text = _show + "%";
	}
}
