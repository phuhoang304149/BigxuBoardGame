using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

/**
* AnimalRacing_Panel_BetHistory_Controller: class show ra lịch sử của đặt cược
**/
public class AnimalRacing_Panel_BetHistory_Controller : MonoBehaviour {

	public List<AnimalRacing_Panel_BetHistoryOptionDetail_Controller> listBetHistoryOptionDetail{get;set;}
	public GameObject optionPrefab;
	public Transform myContent;

	void Awake(){
		listBetHistoryOptionDetail = new List<AnimalRacing_Panel_BetHistoryOptionDetail_Controller>();
	}

	public void SetDataHistory(AnimalRacing_AnimalController.AnimalType _animalType, short _scoreHistory, int _index){
		if(listBetHistoryOptionDetail == null){
			listBetHistoryOptionDetail = new List<AnimalRacing_Panel_BetHistoryOptionDetail_Controller>();
		}
		AnimalRacing_Panel_BetHistoryOptionDetail_Controller _historyDetail = LeanPool.Spawn(optionPrefab, Vector3.zero, Quaternion.identity, myContent.transform).GetComponent<AnimalRacing_Panel_BetHistoryOptionDetail_Controller>();
		AnimalRacing_AnimalInfo _animalInfo = null;
		for(int i = 0; i < AnimalRacing_GamePlay_Manager.instance.listAnimalInfo.Count; i++){
			if(AnimalRacing_GamePlay_Manager.instance.listAnimalInfo[i].animalType == _animalType){
				_animalInfo = AnimalRacing_GamePlay_Manager.instance.listAnimalInfo[i];
				break;
			}
		}
		if(_animalInfo == null){
			Debug.LogError(">>> _animalInfo is NULL");
			return;
		}
		
		_historyDetail.InitData (_animalInfo.mySprite, _scoreHistory, (_index == 0 ? true : false));
		listBetHistoryOptionDetail.Add(_historyDetail);
	}

	public void ResetData(){
		if(listBetHistoryOptionDetail == null || listBetHistoryOptionDetail.Count == 0){
			return;
		}
		for(int i = 0; i < listBetHistoryOptionDetail.Count; i++){
			listBetHistoryOptionDetail[i].SelfDestruction();
		}
		listBetHistoryOptionDetail.Clear();
	}
}