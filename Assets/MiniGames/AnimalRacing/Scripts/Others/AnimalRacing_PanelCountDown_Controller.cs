using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimalRacing_PanelCountDown_Controller : MonoBehaviour {
	
	[SerializeField] CanvasGroup myCanvasGroup;
	[SerializeField] Text txtCountDown;
	float timeLeft;
	System.Action onFinished;

	public void ResetData(){
		onFinished = null;
		timeLeft = 0;
		txtCountDown.text = "";
		myCanvasGroup.alpha = 1f;
		myCanvasGroup.blocksRaycasts = true;
	}

	public void InitData(float _timeLeft, System.Action _onFinished){
		timeLeft = _timeLeft;
		txtCountDown.text = string.Format("{0:0}", Mathf.CeilToInt(timeLeft));
		onFinished = _onFinished;
	}

	public void Show(AudioClip _sfxCount = null, AudioClip _sfxCountFinish = null){
		myCanvasGroup.alpha = 1f;
		myCanvasGroup.blocksRaycasts = true;
		StartCoroutine(DoActionCountDown(_sfxCount, _sfxCountFinish));
	}

	IEnumerator DoActionCountDown(AudioClip _sfxCount = null, AudioClip _sfxCountFinish = null){
		int _tmpTime = (int) timeLeft;
		if(_sfxCount != null){
			if(AnimalRacing_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
				MyAudioManager.instance.PlaySfx(_sfxCount);
			}
		}
		while(_tmpTime > 0){
			yield return Yielders.Get(0.8f);
			_tmpTime --;
			if(_tmpTime <= 0){
				_tmpTime = 0;
				if(_sfxCountFinish != null){
					if(AnimalRacing_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
						MyAudioManager.instance.PlaySfx(_sfxCountFinish);
					}
				}
			}else{
				if(_sfxCount != null){
					if(AnimalRacing_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
						MyAudioManager.instance.PlaySfx(_sfxCount);
					}
				}
			}
			txtCountDown.text = string.Format("{0:0}", Mathf.CeilToInt(_tmpTime));
		}

		txtCountDown.text = "0";

		if(onFinished != null){
			onFinished();
		}

		Hide();
	}

	public void Hide(){
		ResetData();
		myCanvasGroup.alpha = 0f;
		myCanvasGroup.blocksRaycasts = false;
	}
}
