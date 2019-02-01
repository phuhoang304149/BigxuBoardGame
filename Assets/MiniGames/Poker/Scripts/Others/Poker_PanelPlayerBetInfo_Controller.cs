using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Poker_PanelPlayerBetInfo_Controller : MonoBehaviour {

	[SerializeField] CanvasGroup myCanvasGroup;
	public Image imgIconChip;
	[SerializeField] Text txtMyBet;

	long realBet, virtualBet;
	IEnumerator actionTweenBet;

	private void Awake() {
		Hide();
	}

	void ResetData(){
		StopAllCoroutines();
		actionTweenBet = null;

		realBet = 0;
		virtualBet = 0;

		txtMyBet.text = "0";
	}

	public void SetBet(long _bet, bool _updateNow = false){
		realBet = _bet;
		if(_updateNow){
			if(actionTweenBet != null){
				StopCoroutine(actionTweenBet);
				actionTweenBet = null;
			}
			virtualBet = realBet;
			txtMyBet.text = MyConstant.GetMoneyString(virtualBet, 9999);
		}else{
			if(actionTweenBet != null){
				StopCoroutine(actionTweenBet);
				actionTweenBet = null;
			}
			actionTweenBet = MyConstant.TweenValue(virtualBet, realBet, 5, (_valueUpdate)=>{
				virtualBet = _valueUpdate;
				txtMyBet.text = MyConstant.GetMoneyString(virtualBet, 9999);
			}, (_valueFinish)=>{
				virtualBet = _valueFinish;
				txtMyBet.text = MyConstant.GetMoneyString(virtualBet, 9999);
				actionTweenBet = null;
			});
			StartCoroutine(actionTweenBet);
		}
	}

	public void Show(){
		myCanvasGroup.alpha = 1f;
	}

	public void Hide(){
		myCanvasGroup.alpha = 0f;
		ResetData();
	}
}
