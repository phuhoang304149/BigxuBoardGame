using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Uno_Background_Controller : MonoBehaviour {

	[SerializeField] Image imgGlow_00;
	[SerializeField] Image imgGlow_01;

	[Header("Glow Color")]
	[SerializeField] Color colorBgGlow_Red;
	[SerializeField] Color colorBgGlow_Green;
	[SerializeField] Color colorBgGlow_Blue;
	[SerializeField] Color colorBgGlow_Yellow;

	[Header("Setting")]
	[Tooltip("Thời gian Tween Color (tính theo giây)")][SerializeField] float tweenColorTime;

	private void Awake() {
		Hide();
	}

	public void Hide(bool _updateNow = true){
		if(!_updateNow){
			LeanTween.alpha(imgGlow_00.rectTransform, 0f, tweenColorTime);
			LeanTween.alpha(imgGlow_01.rectTransform, 0f, tweenColorTime);
		}else{
			Color _c = imgGlow_00.color;
			_c.a = 0f;
			imgGlow_00.color = _c;

			_c = imgGlow_01.color;
			_c.a = 0f;
			imgGlow_01.color = _c;
		}
	}

	public IEnumerator DoActionSetColor(UnoGamePlayData.BackgroundColor _bgColor, bool _updateNow = true){
		Color _c = colorBgGlow_Yellow;
		switch(_bgColor){
		case UnoGamePlayData.BackgroundColor.Red:
			_c = colorBgGlow_Red;
			break;
		case UnoGamePlayData.BackgroundColor.Green:
			_c = colorBgGlow_Green;
			break;
		case UnoGamePlayData.BackgroundColor.Blue:
			_c = colorBgGlow_Blue;
			break;
		case UnoGamePlayData.BackgroundColor.Yellow:
			_c = colorBgGlow_Yellow;
			break;
		}
		if(_updateNow){
			imgGlow_00.color = _c;

			Color _c1 = imgGlow_01.color;
			_c1.a = 1f;
			imgGlow_01.color = _c1;
			yield break;
		}else{
			bool _isFinished = false;
			LeanTween.color(imgGlow_00.rectTransform, _c, tweenColorTime).setOnComplete(()=>{
				_isFinished = true;
			});
			LeanTween.alpha(imgGlow_01.rectTransform, 1f, tweenColorTime);
			yield return new WaitUntil(()=>_isFinished);
		}
	}
}
