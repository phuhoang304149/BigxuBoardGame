using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Uno_CircleTurn_Controller : MonoBehaviour {

	public enum State{
		Hide, Show
	}
	public State myState{get;set;}

	[SerializeField] SpriteRenderer mySprite;

	[Header("Glow Color")]
	[SerializeField] Color colorBgRed;
	[SerializeField] Color colorBgGreen;
	[SerializeField] Color colorBgBlue;
	[SerializeField] Color colorBgYellow;


	[Header("Setting")]
	[SerializeField] float rotTime;
	[Tooltip("Thời gian Tween Color (tính theo giây)")][SerializeField] float tweenTime;
	[SerializeField] float alphaWhenActive;

	LTDescr tweenRotateAround;

	void Awake(){
		myState = State.Hide;

		Color _c = mySprite.color;
		_c.a = 0f;
		mySprite.color = _c;
	}

	public void Show(bool _updateNow = true){
		if(myState == State.Show){
			return;
		}
		myState = State.Show;
		if(_updateNow){
			Color _c = mySprite.color;
			_c.a = alphaWhenActive;
			mySprite.color = _c;
		}else{
			LeanTween.alpha(mySprite.gameObject, alphaWhenActive, tweenTime);
		}
	}

	public void Hide(bool _updateNow = true){
		if(myState == State.Hide){
			return;
		}
		myState = State.Hide;
		if(_updateNow){
			Color _c = mySprite.color;
			_c.a = 0f;
			mySprite.color = _c;
		}else{
			LeanTween.alpha(mySprite.gameObject, 0f, tweenTime);
		}
	}

	public IEnumerator DoActionSetColor(UnoGamePlayData.BackgroundColor _bgColor, bool _updateNow = true){
		Color _c = colorBgYellow;
		switch(_bgColor){
		case UnoGamePlayData.BackgroundColor.Red:
			_c = colorBgRed;
			break;
		case UnoGamePlayData.BackgroundColor.Green:
			_c = colorBgGreen;
			break;
		case UnoGamePlayData.BackgroundColor.Blue:
			_c = colorBgBlue;
			break;
		case UnoGamePlayData.BackgroundColor.Yellow:
			_c = colorBgYellow;
			break;
		}
		if(myState == State.Hide){
			_c.a = 0f;
		}else{
			_c.a = alphaWhenActive;
		}
		if(_updateNow){
			mySprite.color = _c;
			yield break;
		}else{
			bool _isFinished = false;
			LeanTween.color(mySprite.gameObject, _c, tweenTime).setOnComplete(()=>{
				_isFinished = true;
			});
			yield return new WaitUntil(()=>_isFinished);
		}
	}

	public Coroutine SetTurnDirection(UnoGamePlayData.TurnDirection _turnDirection, bool _updateNow = true){
		if(_updateNow){
			ChangeDirection(_turnDirection);
		}else{
			return StartCoroutine(DoActionSetTurnDirection(_turnDirection));
		}
		return null;
	}

	public IEnumerator DoActionSetTurnDirection(UnoGamePlayData.TurnDirection _turnDirection, bool _updateNow = true){
		if(_updateNow){
			ChangeDirection(_turnDirection);
			yield break;
		}
		bool _isFinished = false;
		LeanTween.scale(gameObject, Vector3.zero, tweenTime).setOnComplete(()=>{
			ChangeDirection(_turnDirection);
			_isFinished = true;
		});
		yield return new WaitUntil(()=>_isFinished);

		_isFinished = false;
		LeanTween.scale(gameObject, Vector3.one, tweenTime).setOnComplete(()=>{
			_isFinished = true;
		});
		yield return new WaitUntil(()=>_isFinished);
	}

	void ChangeDirection(UnoGamePlayData.TurnDirection _turnDirection){
		if(_turnDirection == UnoGamePlayData.TurnDirection.ClockWise){
			if(tweenRotateAround != null){
				LeanTween.cancel(tweenRotateAround.uniqueId);
			}
			if(mySprite.transform.localScale.x < 0){
				Vector3 _localScale = mySprite.transform.localScale;
				_localScale.x *= -1;
				mySprite.transform.localScale = _localScale;
			}
			tweenRotateAround = LeanTween.rotateAround(gameObject, Vector3.forward, -360f, rotTime).setLoopCount(-1);
		}else{
			if(tweenRotateAround != null){
				LeanTween.cancel(tweenRotateAround.uniqueId);
			}
			if(mySprite.transform.localScale.x > 0){
				Vector3 _localScale = mySprite.transform.localScale;
				_localScale.x *= -1;
				mySprite.transform.localScale = _localScale;
			}
			tweenRotateAround = LeanTween.rotateAround(gameObject, Vector3.forward, 360f, rotTime).setLoopCount(-1);
		}
	}

	// void Start(){
	// 	// Vector3 _localScale = transform.localScale;
	// 	// _localScale.x *= -1;
	// 	// transform.localScale = _localScale;
	// 	if(myDirection == MyDirection.ClockWise){
	// 		tweenRotateAround = LeanTween.rotateAround(gameObject, Vector3.forward, -360f, rotTime).setLoopCount(-1);
	// 	}else{
	// 		Vector3 _localScale = transform.localScale;
	// 		_localScale.x *= -1;
	// 		transform.localScale = _localScale;
	// 		tweenRotateAround = LeanTween.rotateAround(gameObject, Vector3.forward, 360f, rotTime).setLoopCount(-1);
	// 	}
	// }
}
