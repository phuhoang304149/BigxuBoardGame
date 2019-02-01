using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalRacing_Panel_TableBet_Controller : MonoBehaviour {

	public List<AnimalRacing_Panel_TableBetOptionDetail_Controller> listBetOption;

	public void SetData(List<short> _listCurrentScore, List<long> _listMyBets, List<long> _listGlobalBets){
		if(listBetOption == null || listBetOption.Count != 9){
			Debug.LogError("Sai dữ liệu listBetOption!");
			return;
		}
		SetCurrentScore(_listCurrentScore, true);
		SetMyBet(_listMyBets, true);
		SetGlobalBet(_listGlobalBets, true);
	}

	public void SetCurrentScore(List<short> _listCurrentScore, bool _updateNow = false){
		if(_listCurrentScore == null || _listCurrentScore.Count != 9){
			Debug.LogError("Sai dữ liệu _listCurrentScore!");
			return;
		}
		short _tmpScore = 0;
		for(int i = 0; i < listBetOption.Count; i++){
			_tmpScore = _listCurrentScore[i];
			listBetOption[i].SetCurrentScore(_tmpScore, _updateNow);
		}
	}

	public void SetMyBet(List<long> _listMyBets, bool _updateNow = false){
		if(_listMyBets == null || _listMyBets.Count != 9){
			Debug.LogError("Sai dữ liệu _listMyBets!");
			return;
		}
		long _tmpBet = 0;
		for(int i = 0; i < listBetOption.Count; i++){
			_tmpBet = _listMyBets[i];
			listBetOption[i].SetMyBet(_tmpBet, 999999, _updateNow);
		}
	}

	public void SetGlobalBet(List<long> _listGlobalBets, bool _updateNow = false){
		if(_listGlobalBets == null || _listGlobalBets.Count != 9){
			Debug.LogError("Sai dữ liệu _listGlobalBets!");
			return;
		}
		long _tmpBet = 0;
		for(int i = 0; i < listBetOption.Count; i++){
			_tmpBet = _listGlobalBets[i];
			listBetOption[i].SetGlobalBet(_tmpBet, 999999, _updateNow);
		}
	}
}
