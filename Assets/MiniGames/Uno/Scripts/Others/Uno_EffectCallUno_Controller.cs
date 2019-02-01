using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class Uno_EffectCallUno_Controller : MonoBehaviour {

	public enum State{
		Hide, Show
	}
	public State currentState{get;set;}
	
	[SerializeField] CanvasGroup myCanvasGroup;

	[Header("Setting")]
	[SerializeField] float timeShowEffect;

	[Header("Prefabs")]
	[SerializeField] GameObject particleEffPrefab;

	private void Awake() {
		LeanTween.cancel(gameObject);
		currentState = State.Hide;
		myCanvasGroup.alpha = 0f;
		transform.localScale = Vector3.one * 0.5f;
	}

	public void Show() {
		if(currentState == State.Show){
			return;
		}

		currentState = State.Show;
		myCanvasGroup.alpha = 0f;
		transform.localScale = Vector3.one * 0.5f;

		MySimplePoolObjectController _effect = LeanPool.Spawn(particleEffPrefab, Vector3.zero, Quaternion.identity).GetComponent<MySimplePoolObjectController>();
		Uno_GamePlay_Manager.instance.UIManager.effectPoolManager.AddObject(_effect);
		_effect.transform.position = transform.position;

		if(Uno_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
            MyAudioManager.instance.PlaySfx(Uno_GamePlay_Manager.instance.myAudioInfo.sfx_CallUno);
        }

		LeanTween.alphaCanvas(myCanvasGroup, 1f, timeShowEffect).setOnComplete(()=>{
			LeanTween.alphaCanvas(myCanvasGroup, 0.8f, 0.5f).setLoopPingPong(-1);
		});
		LeanTween.scale(gameObject, Vector3.one * 1f, timeShowEffect).setEase(LeanTweenType.easeOutBack).setOnComplete(()=>{
			LeanTween.scale(gameObject, Vector3.one * 0.8f, 0.5f).setEase(LeanTweenType.easeInOutSine).setLoopPingPong(-1);
		});
	}

	public void Hide(){
		if(currentState == State.Hide){
			return;
		}
		LeanTween.cancel(gameObject);
		
		currentState = State.Hide;
		LeanTween.alphaCanvas(myCanvasGroup, 0f, timeShowEffect);
		LeanTween.scale(gameObject, Vector3.one * 0.5f, timeShowEffect).setEase(LeanTweenType.easeInBack);
	}
}
