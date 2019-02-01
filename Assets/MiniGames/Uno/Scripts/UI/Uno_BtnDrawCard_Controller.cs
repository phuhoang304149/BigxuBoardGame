using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Uno_BtnDrawCard_Controller : MonoBehaviour {

	[SerializeField] CanvasGroup myCanvasGroup;
	[SerializeField] Image imgHighlight;
	[SerializeField] MyArrowFocusController arrowFocus;

	LTDescr tweenCanvasGroupBtnDraw, tweenHighlight;

	public void Show(bool _updateNow = true){
        if(tweenCanvasGroupBtnDraw != null){
            LeanTween.cancel(tweenCanvasGroupBtnDraw.uniqueId);
            tweenCanvasGroupBtnDraw = null;
        }
		if(tweenHighlight != null){
            LeanTween.cancel(tweenHighlight.uniqueId);
            tweenHighlight = null;
        }
		

        myCanvasGroup.blocksRaycasts = true;
		Color _c = imgHighlight.color;
		_c.a = 0.2f;
		imgHighlight.color = _c;

        if(_updateNow){
            myCanvasGroup.alpha = 1f;
        }else{
			myCanvasGroup.alpha = 0f;
			tweenCanvasGroupBtnDraw = LeanTween.alphaCanvas(myCanvasGroup, 1f, 0.1f).setOnComplete(()=>{
                tweenCanvasGroupBtnDraw = null;
            });
        }
		tweenHighlight = LeanTween.alpha(imgHighlight.rectTransform, 1f, 0.4f).setEase(LeanTweenType.easeInOutSine).setLoopPingPong(-1);
		
		arrowFocus.Show();
	}

	 public void Hide(bool _updateNow = true){
        myCanvasGroup.blocksRaycasts = false;
        if(_updateNow){
			if(tweenCanvasGroupBtnDraw != null){
				LeanTween.cancel(tweenCanvasGroupBtnDraw.uniqueId);
				tweenCanvasGroupBtnDraw = null;
			}
			if(tweenHighlight != null){
				LeanTween.cancel(tweenHighlight.uniqueId);
				tweenHighlight = null;
			}
			arrowFocus.Hide();
            myCanvasGroup.alpha = 0f;
        }else{
			if(tweenCanvasGroupBtnDraw != null){
				LeanTween.cancel(tweenCanvasGroupBtnDraw.uniqueId);
				tweenCanvasGroupBtnDraw = null;
			}

            tweenCanvasGroupBtnDraw = LeanTween.alphaCanvas(myCanvasGroup, 0f, 0.1f).setOnComplete(()=>{
                tweenCanvasGroupBtnDraw = null;

				if(tweenHighlight != null){
					LeanTween.cancel(tweenHighlight.uniqueId);
					tweenHighlight = null;
				}
				arrowFocus.Hide();
            });
        }
    }
}
