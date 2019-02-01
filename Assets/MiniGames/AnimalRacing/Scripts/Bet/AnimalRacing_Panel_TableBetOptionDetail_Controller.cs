using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AnimalRacing_Panel_TableBetOptionDetail_Controller : MySimpleTableBetOptionDetailController {

	[Space(10)]
	public AnimalRacing_AnimalController.AnimalType animalType;
	public Image imgAvatar;
	[SerializeField] Text txtScore;
	[SerializeField] RectTransform panelHighLight;
	[SerializeField] RectTransform panelShadow;
	public ParticleSystem glowParticle;

	long realScore, virtualScore;
	IEnumerator actionTweenCurrentScore;

	public void SetCurrentScore(short _currentScore, bool _updateNow = false){
		realScore = _currentScore;
		if(_updateNow){
			if(actionTweenCurrentScore != null){
				StopCoroutine(actionTweenCurrentScore);
				actionTweenCurrentScore = null;
			}
			virtualScore = realScore;
			txtScore.text = "x" + MyConstant.GetMoneyString(virtualScore, 9999);
		}else{
			if(actionTweenCurrentScore != null){
				StopCoroutine(actionTweenCurrentScore);
				actionTweenCurrentScore = null;
			}
			actionTweenCurrentScore = MyConstant.TweenValue(virtualScore, realScore, 5, (_valueUpdate)=>{
				virtualScore = _valueUpdate;
				txtScore.text = "x" + MyConstant.GetMoneyString(virtualScore, 9999);
			}, (_valueFinish)=>{
				virtualScore = _valueFinish;
				txtScore.text = "x" + MyConstant.GetMoneyString(virtualScore, 9999);
				actionTweenCurrentScore = null;
			});
			StartCoroutine(actionTweenCurrentScore);
		}
	}
	
	public void ShowGlow(int _n = 1, float _timeDelay = 0f){
		if(!glowParticle.gameObject.activeSelf){
			return;
		}
		if(_n == 1){
			if(glowParticle.gameObject.activeSelf){
				glowParticle.Play();
			}
			return;
		}
		StartCoroutine(DoActionShowGlow(_n, _timeDelay));
	}

	IEnumerator DoActionShowGlow(int _n, float _timeDelay){
		for(int i = 0; i < _n; i ++){
			if(glowParticle.gameObject.activeSelf){
				glowParticle.Play();
			}else{
				yield break;
			}
			yield return Yielders.Get(_timeDelay);
		}
	}

	public IEnumerator Highlight(){
		bool _isFinished = false;
		LeanTween.alpha(panelShadow, 0f, 0.1f);
		LeanTween.alpha(panelHighLight, 0.4f, 0.2f).setLoopPingPong(8).setOnComplete(()=>{
			_isFinished = true;
		});
		yield return new WaitUntil(()=>_isFinished);
	}

	public void SetShadow(bool _active){
		if(_active){
			LeanTween.alpha(panelShadow, 0.7f, 0.1f);
		}else{
			LeanTween.alpha(panelShadow, 0f, 0.1f);
		}
	}
	
	public override void OnAddBet(){
		if(!AnimalRacing_GamePlay_Manager.instance.CanAddBet()){
			return;
		}
		if(AnimalRacing_GamePlay_Manager.instance.betManager.panelListChip.currentChip != null){
			#if TEST
			Debug.Log(">>>> Đặt con " + animalType.ToString() + " với tiền cược là " + AnimalRacing_GamePlay_Manager.instance.betManager.panelListChip.currentChip.chipInfo.value);
			#endif
			AnimalRacing_RealTimeAPI.instance.SendMessageAddBet((byte) animalType, (short) AnimalRacing_GamePlay_Manager.instance.betManager.panelListChip.currentChip.index, AnimalRacing_GamePlay_Manager.instance.betManager.panelListChip.currentChip.chipInfo.value);
		}else{
			PopupManager.Instance.CreateToast(MyLocalize.GetString("Global/PlsSelectChip"));
		}
	}
}
