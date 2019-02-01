using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragonTiger_Panel_TableBetOptionDetail_Controller : MySimpleTableBetOptionDetailController {

	[Space(10)]
	[SerializeField] DragonTiger_GamePlay_Manager.IndexBet indexBet;
	[SerializeField] Text txtCountBet;
	[SerializeField] RectTransform panelHighLight;
	[SerializeField] RectTransform panelShadow;

	long virtualCountBet, realCountBet;
	IEnumerator actionTweenCountBet;
	
	public void SetCountBet(short _countBet, bool _updateNow = false){
		realCountBet = _countBet;
		if(_updateNow){
			if(actionTweenCountBet != null){
				StopCoroutine(actionTweenCountBet);
				actionTweenCountBet = null;
			}
			virtualCountBet = realCountBet;
			txtCountBet.text = MyConstant.GetMoneyString(virtualCountBet, 9999);
		}else{
			if(actionTweenCountBet != null){
				StopCoroutine(actionTweenCountBet);
				actionTweenCountBet = null;
			}
			actionTweenCountBet = MyConstant.TweenValue(virtualCountBet, realCountBet, 5, (_valueUpdate)=>{
				virtualCountBet = _valueUpdate;
				txtCountBet.text = MyConstant.GetMoneyString(virtualCountBet, 9999);
			}, (_valueFinish)=>{
				virtualCountBet = _valueFinish;
				txtCountBet.text = MyConstant.GetMoneyString(virtualCountBet, 9999);
				actionTweenCountBet = null;
			});
			StartCoroutine(actionTweenCountBet);
		}
	}

	public void SetUpHighlight(System.Action _onFinished = null){
		LeanTween.alpha(panelHighLight, 0.4f, 0.2f).setLoopPingPong(8).setOnComplete(_onFinished);
	}

	public void SetShadow(bool _active){
		if(_active){
			LeanTween.alpha(panelShadow, 0.7f, 0.1f);
		}else{
			LeanTween.alpha(panelShadow, 0f, 0.1f);
		}
	}

	public override void OnAddBet(){
		#if TEST
		Debug.Log(">>> Đặt cược " + indexBet.ToString());
		#endif
		DragonTiger_GamePlay_Manager.instance.AddBet((sbyte) indexBet);
    }
}
