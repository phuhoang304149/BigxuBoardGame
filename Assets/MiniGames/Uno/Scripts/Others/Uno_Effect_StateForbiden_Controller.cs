using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Uno_Effect_StateForbiden_Controller : MonoBehaviour {

	enum State{
		Hide, Show
	}
	State currentState;
	[SerializeField] CanvasGroup myCanvasGroup;
	[SerializeField] Image imgIcon;

	[Header("Setting")]
	[SerializeField] float timeShowEffect;
	[SerializeField] Color colorIconRed;
	[SerializeField] Color colorIconGreen;
	[SerializeField] Color colorIconBlue;
	[SerializeField] Color colorIconYellow;

	

	private void Awake() {
		LeanTween.cancel(gameObject);
		currentState = State.Hide;
		myCanvasGroup.alpha = 0f;
		transform.localScale = Vector3.one * 0.5f;
	}

	public void Show(UnoGamePlayData.BackgroundColor _bgColor) {
		if(currentState == State.Show){
			return;
		}
		LeanTween.cancel(gameObject);

		currentState = State.Show;
		myCanvasGroup.alpha = 0f;
		transform.localScale = Vector3.one * 0.5f;

		switch(_bgColor){
		case UnoGamePlayData.BackgroundColor.Red:
			imgIcon.color = colorIconRed;
			break;
		case UnoGamePlayData.BackgroundColor.Green:
			imgIcon.color = colorIconGreen;
			break;
		case UnoGamePlayData.BackgroundColor.Blue:
			imgIcon.color = colorIconBlue;
			break;
		case UnoGamePlayData.BackgroundColor.Yellow:
			imgIcon.color = colorIconYellow;
			break;
		}

		LeanTween.alphaCanvas(myCanvasGroup, 1f, timeShowEffect);
		LeanTween.scale(gameObject, Vector3.one, timeShowEffect).setEase(LeanTweenType.easeOutBack);
	}

	public void Hide(){
		if(currentState == State.Hide){
			return;
		}
		LeanTween.cancel(gameObject);
		
		currentState = State.Hide;
		LeanTween.alphaCanvas(myCanvasGroup, 0f, timeShowEffect);
		LeanTween.scale(gameObject, Vector3.zero, timeShowEffect).setEase(LeanTweenType.easeInBack);
	}
}
