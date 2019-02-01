using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelClockInGameController : MonoBehaviour {

	[SerializeField] ShakeController myShakeController;
	[SerializeField] Text txtCountDown;
	[SerializeField] Color colorTxtCountDownYellow;
	[SerializeField] Color colorTxtCountDownRed;
	
	IEnumerator actionCountDown;

	bool hadSetVibrate, hadSetWarning;

	void ResetData(){
		hadSetVibrate = hadSetWarning = false;
		txtCountDown.color = Color.white;
		myShakeController.SetUpStopShake();
	}

	// void InitData(int _timeLeft){
	// 	timeLeft = Mathf.CeilToInt((float) (_timeLeft / 1000f)) - 1;
	// 	if(timeLeft <  0){
	// 		timeLeft = 0;
	// 	}
	// 	txtCountDown.text = string.Format("{0:00}", Mathf.CeilToInt(timeLeft));
	// }

	public void StopShowCountDown(){
		if(actionCountDown != null){
			StopCoroutine(actionCountDown);
			actionCountDown = null;
		}
		ResetData();
	}

	public void StartCountDown(double _timeLeft, System.Action _onFinished){
		if(actionCountDown != null){
			StopCoroutine(actionCountDown);
			actionCountDown = null;
		}
		actionCountDown = DoActionCountDownWithTimeLeft(_timeLeft, _onFinished);
		StartCoroutine(actionCountDown);
	}

	IEnumerator DoActionCountDownWithTimeLeft(double _timeLeft, System.Action _onFinished){
		double _tmpTime = _timeLeft;
		if(_tmpTime < 0){
			_tmpTime = 0;
		}
		txtCountDown.text = string.Format("{0:00}", (long) _tmpTime);
		
		while(_tmpTime > 0f){
			yield return null;
			_tmpTime -= Time.unscaledDeltaTime;
			if(!hadSetWarning && _tmpTime <= 5f){
				hadSetWarning = true;
				txtCountDown.color = colorTxtCountDownYellow;
			}
			if(!hadSetVibrate && _tmpTime <= 3f){
				hadSetVibrate = true;
				txtCountDown.color = colorTxtCountDownRed;
				myShakeController.SetUpShakeLocalPoint(-1);
			}
			if(_tmpTime < 0f){
				_tmpTime = 0f;
			}
			txtCountDown.text = string.Format("{0:00}", (long) _tmpTime);
		}

		txtCountDown.text = "00";
		yield return null;

		if(_onFinished != null){
			_onFinished();
		}

		ResetData();
	}
}
