using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Koprok_Panel_TableBetOptionDetail_Controller : MySimpleTableBetOptionDetailController {

	[Space(10)]
    [SerializeField] Koprok_GamePlay_Manager.IndexBet indexBet;
    [SerializeField] RectTransform myRectTransform;
    [SerializeField] RectTransform panelInfoRectTransform;
	[SerializeField] Image imgAvatar;
	[SerializeField] Image imgBorder;
	[SerializeField] Text txtCountBet;
	[SerializeField] RectTransform panelHighLight;
	[SerializeField] RectTransform panelShadow;

	[Header("Setting")]
	[SerializeField] Color colorImgBorderWhenShowShadow;
    
	long virtualCountBet, realCountBet;
	IEnumerator actionTweenCountBet;

    public void SetSizeAgain(){
        float _ratioX = myRectTransform.sizeDelta.x / panelInfoRectTransform.sizeDelta.x;
        float _ratioY = myRectTransform.sizeDelta.y / panelInfoRectTransform.sizeDelta.y;

        Vector3 _size = Vector3.one;
        _size.x = _ratioX;
        _size.y = _ratioY;

        panelInfoRectTransform.localScale = _size;
    }

	public void SetCountBet(short _countBet, bool _updateNow = false){
		realCountBet = _countBet;
		if(_updateNow){
			if(actionTweenCountBet != null){
				StopCoroutine(actionTweenCountBet);
				actionTweenCountBet = null;
			}
			virtualCountBet = realCountBet;
			txtCountBet.text = "x" + MyConstant.GetMoneyString(virtualCountBet);
		}else{
			if(actionTweenCountBet != null){
				StopCoroutine(actionTweenCountBet);
				actionTweenCountBet = null;
			}
			actionTweenCountBet = MyConstant.TweenValue(virtualCountBet, realCountBet, 5, (_valueUpdate)=>{
				virtualCountBet = _valueUpdate;
				txtCountBet.text = "x" + MyConstant.GetMoneyString(virtualCountBet);
			}, (_valueFinish)=>{
				virtualCountBet = _valueFinish;
				txtCountBet.text = "x" + MyConstant.GetMoneyString(virtualCountBet);
				actionTweenCountBet = null;
			});
			StartCoroutine(actionTweenCountBet);
		}
	}

	public IEnumerator Highlight(){
		bool _isFinished = false;
		LeanTween.alpha(panelShadow, 0f, 0.1f);
		LeanTween.color(imgBorder.rectTransform, Color.white, 0.1f);
		LeanTween.alpha(panelHighLight, 0.4f, 0.2f).setLoopPingPong(8).setOnComplete(()=>{
			_isFinished = true;
		});
		yield return new WaitUntil(()=>_isFinished);
	}

	public void SetShadow(bool _active){
		if(_active){
			LeanTween.alpha(panelShadow, 0.7f, 0.1f);
			LeanTween.color(imgBorder.rectTransform, colorImgBorderWhenShowShadow, 0.1f);
		}else{
			LeanTween.alpha(panelShadow, 0f, 0.1f);
			LeanTween.color(imgBorder.rectTransform, Color.white, 0.1f);
		}
	}

    public override void OnAddBet(){
		Debug.Log(">>> Đặt cược " + indexBet.ToString());

		Koprok_GamePlay_Manager.instance.AddBet((sbyte) indexBet);
    }
}
