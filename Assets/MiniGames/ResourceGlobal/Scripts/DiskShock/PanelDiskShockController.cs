using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelDiskShockController : MonoBehaviour {

	enum State{
		CountDown, ShowResult, ShockAgain
	}
	State currentState;

	[SerializeField] ShakeController diskShakeController;
	[SerializeField] ShakeController textCountDownShakeController;
	[SerializeField] RectTransform diskUpRectTransform;
	[SerializeField] List<Image> listItems;
	[SerializeField] Text txtCountDown;
	
	int timeLeft;
	bool hadSetVibrate, hadSetWarning, hadLockScreen;

	[Header("Setting")]
	[SerializeField] float ratioScaleDefault;
	[SerializeField] Vector2 deltaSize;
	[SerializeField] Color colorTxtCountDownYellow;
	[SerializeField] Color colorTxtCountDownRed;

	List<Vector3> posItemHolder;
	IEnumerator actionCountDown;

	private void Awake() {
		posItemHolder = new List<Vector3>();
		for(int i = 0; i < listItems.Count; i++){
			posItemHolder.Add(listItems[i].transform.localPosition);
		}
		ResetData();
	}
	
	void ResetData(){
		hadSetVibrate = hadSetWarning = hadLockScreen = false;
		transform.localScale = Vector3.one * ratioScaleDefault;
		
		// Color _c = Color.black;
		// _c.a = 0f;
		// txtCountDown.color = _c;

		diskShakeController.SetUpStopShake();
		diskShakeController.transform.localPosition = Vector3.zero;
		diskShakeController.transform.localRotation = Quaternion.identity;

		textCountDownShakeController.SetUpStopShake();
		textCountDownShakeController.transform.localPosition = Vector3.zero;
		textCountDownShakeController.transform.localRotation = Quaternion.identity;

		for(int i = 0; i < listItems.Count; i++){
			listItems[i].sprite = null;
			listItems[i].transform.localRotation = Quaternion.identity;
			listItems[i].transform.localPosition = posItemHolder[i];
		}
	}

	public void SetOriginScaleAgain(float _ratio, bool _scaleNow = false){
		ratioScaleDefault = _ratio;
		if(_scaleNow){
			transform.localScale = Vector3.one * ratioScaleDefault;
		}
	}

	public void SetItemInfo(List<Sprite> _listImgItem, bool _randomLocalPos = false, bool _randomRot = false){
		if(_listImgItem == null || listItems == null){
			#if TEST
			Debug.LogError(">>> List listItems Null");
			#endif
			return;
		}
		if(_listImgItem.Count != listItems.Count){
			#if TEST
			Debug.LogError(">>> List listItems Null");
			#endif
			return;
		}
		for(int i = 0; i < listItems.Count; i++){
			listItems[i].sprite = _listImgItem[i];
			if(_randomLocalPos){
				Vector3 _pos = posItemHolder[i];
				_pos.x += Random.Range(0f - deltaSize.x/2, 0 + deltaSize.x/2);
				_pos.y += Random.Range(0f - deltaSize.y/2, 0 + deltaSize.y/2);
				listItems[i].transform.localPosition = _pos;
			}
			if(_randomRot){
				float zAngle = Random.Range(0f, 360f);
				listItems[i].transform.rotation = Quaternion.Euler (0f,0f,zAngle);
			}
		}
	}

	public void StopShowCountDown(){
		if(actionCountDown != null){
			StopCoroutine(actionCountDown);
			actionCountDown = null;
		}

		Color _c = Color.black;
		_c.a = 1f;
		txtCountDown.color = _c;
		
		diskShakeController.SetUpStopShake();
		diskShakeController.transform.localPosition = Vector3.zero;
		diskShakeController.transform.localRotation = Quaternion.identity;
		textCountDownShakeController.SetUpStopShake();
		textCountDownShakeController.transform.localPosition = Vector3.zero;
		textCountDownShakeController.transform.localRotation = Quaternion.identity;
		hadSetVibrate = hadSetWarning = hadLockScreen = false;
	}

	public void StartCountDown(double _timeLeft, System.Action _onFinished){
		hadSetVibrate = hadSetWarning = hadLockScreen = false;
		Color _c = Color.black;
		_c.a = 1f;
		txtCountDown.color = _c;
		diskShakeController.SetUpStopShake();
		diskShakeController.transform.localPosition = Vector3.zero;
		diskShakeController.transform.localRotation = Quaternion.identity;
		textCountDownShakeController.SetUpStopShake();
		textCountDownShakeController.transform.localPosition = Vector3.zero;
		textCountDownShakeController.transform.localRotation = Quaternion.identity;

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
				textCountDownShakeController.SetUpShakeLocalPoint(-1);
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

		diskShakeController.SetUpStopShake();
		diskShakeController.transform.localPosition = Vector3.zero;
		diskShakeController.transform.localRotation = Quaternion.identity;
		textCountDownShakeController.SetUpStopShake();
		textCountDownShakeController.transform.localPosition = Vector3.zero;
		textCountDownShakeController.transform.localRotation = Quaternion.identity;
		hadSetVibrate = hadSetWarning = hadLockScreen = false;
	}

	public IEnumerator DoActionShowResult(AudioClip _sfxMove = null){
		StopShowCountDown();

		Color _c = txtCountDown.color;
		_c.a = 0f;
		txtCountDown.color = _c;
		
		if(_sfxMove != null){
			MyAudioManager.instance.PlaySfx(_sfxMove);
		}

		bool _isFinished = false;
		LeanTween.scale(gameObject, Vector3.one, 0.5f).setEase(LeanTweenType.easeInSine).setOnComplete(()=>{
			_isFinished = true;
		});
		yield return new WaitUntil(()=>_isFinished);
		yield return Yielders.Get(0.5f);

		if(_sfxMove != null){
			MyAudioManager.instance.PlaySfx(_sfxMove);
		}

		LeanTween.scale(diskUpRectTransform.gameObject, Vector3.one * 1.2f, 0.3f).setEase(LeanTweenType.easeOutSine);
		LeanTween.alpha(diskUpRectTransform, 0f, 0.3f).setEase(LeanTweenType.easeOutSine);
		yield return Yielders.Get(1.5f);

		if(_sfxMove != null){
			MyAudioManager.instance.PlaySfx(_sfxMove);
		}

		_isFinished = false;
		LeanTween.scale(gameObject, Vector3.one * ratioScaleDefault, 0.5f).setEase(LeanTweenType.easeOutSine).setOnComplete(()=>{
			_isFinished = true;
		});
		yield return new WaitUntil(()=>_isFinished);
	}

	public IEnumerator DoActionShockAgainAndReset(AudioClip _sfxMove = null, AudioClip _sfxShake = null){
		if(_sfxMove != null){
			MyAudioManager.instance.PlaySfx(_sfxMove);
		}
		
		bool _isFinished = false;
		LeanTween.scale(gameObject, Vector3.one, 0.5f).setEase(LeanTweenType.easeInSine).setOnComplete(()=>{
			_isFinished = true;
		});
		yield return new WaitUntil(()=>_isFinished);
		yield return Yielders.Get(0.2f);

		if(_sfxMove != null){
			MyAudioManager.instance.PlaySfx(_sfxMove);
		}

		LeanTween.scale(diskUpRectTransform.gameObject, Vector3.one, 0.3f).setEase(LeanTweenType.easeInSine);
		LeanTween.alpha(diskUpRectTransform, 1f, 0.3f).setEase(LeanTweenType.easeInSine);
		yield return Yielders.Get(0.7f);

		if(_sfxShake != null){
			MyAudioManager.instance.PlaySfx(_sfxShake);
		}

		diskShakeController.SetUpShakeLocalPoint(1f);
		yield return Yielders.Get(1f);

		diskShakeController.SetUpStopShake();
		diskShakeController.transform.localPosition = Vector3.zero;
		diskShakeController.transform.localRotation = Quaternion.identity;

		yield return Yielders.Get(0.5f);

		if(_sfxMove != null){
			MyAudioManager.instance.PlaySfx(_sfxMove);
		}

		_isFinished = false;
		LeanTween.scale(gameObject, Vector3.one * ratioScaleDefault, 0.5f).setEase(LeanTweenType.easeOutSine).setOnComplete(()=>{
			_isFinished = true;
		});
		yield return new WaitUntil(()=>_isFinished);
		yield return Yielders.Get(0.1f);
	}
}
