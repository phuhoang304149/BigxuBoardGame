using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Poker_PanelTypeCardResult_Controller : MonoBehaviour {

	enum State{
		Hide, Show
	}
	State currentState;

	[SerializeField] CanvasGroup myCanvasGroup;
	[SerializeField] Text textContent;

	private void Awake() {
		myCanvasGroup.alpha = 0f;
		currentState = State.Hide;
		transform.localScale = Vector3.one;
	}

	public void Show(string _content, bool _isNow = false){
		if(currentState == State.Show){
			return;
		}
		currentState = State.Show;
		textContent.text = _content;
		if(!_isNow){
			transform.localScale = Vector3.one * 0.5f;
			LeanTween.alphaCanvas(myCanvasGroup, 1f, 0.1f);
			LeanTween.scale(gameObject, Vector3.one, 0.1f).setEase(LeanTweenType.easeOutBack);
		}else{
			transform.localScale = Vector3.one;
			myCanvasGroup.alpha = 1f;
		}
	}

	public void Hide(bool _isNow = false){
		if(currentState == State.Hide){
			return;
		}
		currentState = State.Hide;
		if(!_isNow){
			LeanTween.alphaCanvas(myCanvasGroup, 0f, 0.1f);
		}else{
			myCanvasGroup.alpha = 0f;
		}
	}
}
