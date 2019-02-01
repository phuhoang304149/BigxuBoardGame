using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;

public class AnimalRacing_Panel_BetHistoryOptionDetail_Controller : MonoBehaviour {

	public Image imgAnimal;
	public Text txtHistory;

	void ResetData(){}

	public void InitData(Sprite _avatar, short _scoreHistory, bool _isFirstElement = false){
		imgAnimal.sprite = _avatar;
		txtHistory.text = string.Format("<color=" + (_isFirstElement ? "yellow" : "white") + ">x{0}</color>", _scoreHistory.ToString());
	}

	public void SelfDestruction(){
		if(gameObject == null || !gameObject.activeSelf){
			return;
		}
		LeanPool.Despawn(gameObject);
	}

	protected void OnSpawn(){
	}

	protected void OnDespawn(){
		ResetData ();
	}
}
