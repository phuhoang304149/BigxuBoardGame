using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyArrowFocusController : MonoBehaviour {
	
	enum StyleMove{
		MoveUp, MoveDown, MoveLeft, MoveRight
	}
	[SerializeField] StyleMove styleMove;

	public enum State{
		Hide, Show
	}
	public State myState{get;set;}

	[SerializeField] Image myImage;
	[SerializeField] float deltaMove;
	[SerializeField] float timeMove;

	Vector3 localPositionSaved;

	private void Awake() {
		localPositionSaved = transform.localPosition;

		myState = State.Hide;
		Color _c = myImage.color;
		_c.a = 0f;
		myImage.color = _c;
	}

	public void Show(){
		if(myState == State.Show){
			return;
		}
		myState = State.Show;
		LeanTween.cancel(gameObject);
		LeanTween.cancel(myImage.gameObject);

		transform.localPosition = localPositionSaved;
		transform.localScale = Vector3.one;

		LeanTween.alpha(myImage.rectTransform, 1f, 0.1f);
		LeanTween.scaleX(gameObject, 0.8f, timeMove).setEase(LeanTweenType.easeInOutSine).setLoopPingPong(-1);
		LeanTween.scaleY(gameObject, 1.2f, timeMove).setEase(LeanTweenType.easeInOutSine).setLoopPingPong(-1);
		
		switch(styleMove){
		case StyleMove.MoveDown:
			LeanTween.moveLocalY(gameObject, localPositionSaved.y - deltaMove, timeMove).setEase(LeanTweenType.easeInOutSine).setLoopPingPong(-1);
			break;
		case StyleMove.MoveUp:
			LeanTween.moveLocalY(gameObject, localPositionSaved.y + deltaMove, timeMove).setEase(LeanTweenType.easeInOutSine).setLoopPingPong(-1);
			break;
		case StyleMove.MoveLeft:
			LeanTween.moveLocalX(gameObject, localPositionSaved.x - deltaMove, timeMove).setEase(LeanTweenType.easeInOutSine).setLoopPingPong(-1);
			break;
		case StyleMove.MoveRight:
			LeanTween.moveLocalX(gameObject, localPositionSaved.x + deltaMove, timeMove).setEase(LeanTweenType.easeInOutSine).setLoopPingPong(-1);
			break;
		}
	}

	public void Hide(){
		if(myState == State.Hide){
			return;
		}
		myState = State.Hide;
		LeanTween.cancel(gameObject);
		LeanTween.cancel(myImage.gameObject);

		Color _c = myImage.color;
		_c.a = 0f;
		myImage.color = _c;

		transform.localPosition = localPositionSaved;
		transform.localScale = Vector3.one;
	}
}
