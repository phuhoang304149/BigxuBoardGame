using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MySimpleTableBetOptionDetailController : MonoBehaviour {
	[Space(10)]
	[SerializeField] Button myButton;
	[SerializeField] protected Text txtMyBet;
	[SerializeField] protected Text txtGlobalBet;
	protected long realGlobalBet, realMyBet, virtualGlobalBet, virtualMyBet;
	protected IEnumerator actionTweenGlobalBet, actionTweenMyBet;

	private void Awake() {
		myButton.onClick.AddListener(OnAddBet);
	}
	public void SetMyBet(long _myBet, long _minGoldCheck, bool _updateNow = false){
		realMyBet = _myBet;
		if(_updateNow){
			if(actionTweenMyBet != null){
				StopCoroutine(actionTweenMyBet);
				actionTweenMyBet = null;
			}
			virtualMyBet = realMyBet;
			txtMyBet.text = MyConstant.GetMoneyString(virtualMyBet, _minGoldCheck);
		}else{
			if(actionTweenMyBet != null){
				StopCoroutine(actionTweenMyBet);
				actionTweenMyBet = null;
			}
			actionTweenMyBet = MyConstant.TweenValue(virtualMyBet, realMyBet, 5, (_valueUpdate)=>{
				virtualMyBet = _valueUpdate;
				txtMyBet.text = MyConstant.GetMoneyString(virtualMyBet, _minGoldCheck);
			}, (_valueFinish)=>{
				virtualMyBet = _valueFinish;
				txtMyBet.text = MyConstant.GetMoneyString(virtualMyBet, _minGoldCheck);
				actionTweenMyBet = null;
			});
			StartCoroutine(actionTweenMyBet);
		}
	}

	public void SetGlobalBet(long _totalBet, long _minGoldCheck, bool _updateNow = false){
		realGlobalBet = _totalBet;
		if(_updateNow){
			if(actionTweenGlobalBet != null){
				StopCoroutine(actionTweenGlobalBet);
				actionTweenGlobalBet = null;
			}
			virtualGlobalBet = realGlobalBet;
			txtGlobalBet.text = MyConstant.GetMoneyString(virtualGlobalBet);
		}else{
			if(actionTweenGlobalBet != null){
				StopCoroutine(actionTweenGlobalBet);
				actionTweenGlobalBet = null;
			}
			actionTweenGlobalBet = MyConstant.TweenValue(virtualGlobalBet, realGlobalBet, 5, (_valueUpdate)=>{
				virtualGlobalBet = _valueUpdate;
				txtGlobalBet.text = MyConstant.GetMoneyString(virtualGlobalBet, _minGoldCheck);
			}, (_valueFinish)=>{
				virtualGlobalBet = _valueFinish;
				txtGlobalBet.text = MyConstant.GetMoneyString(virtualGlobalBet, _minGoldCheck);
				actionTweenGlobalBet = null;
			});
			StartCoroutine(actionTweenGlobalBet);
		}
	}

	public virtual void OnAddBet(){}
}
