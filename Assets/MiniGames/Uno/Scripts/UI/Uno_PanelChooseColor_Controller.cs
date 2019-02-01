using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Uno_PanelChooseColor_Controller : MonoBehaviour {

	public enum State{
		Hide, Show
	}
	public State currentState{get;set;}

	[SerializeField] CanvasGroup myCanvasGroup;
	[SerializeField] List<Transform> listOptions;

	System.Action<int> onSelected;

	void Awake(){
		currentState = State.Hide;

		myCanvasGroup.alpha = 0f;
		myCanvasGroup.blocksRaycasts = false;

		for(int i = 0; i < listOptions.Count; i++){
			listOptions[i].localScale = Vector3.zero;
		}
	}

	[ContextMenu("TEST")]
	void TESTShow(){
		Show((_index)=>{
			Hide(_index);
		});
	}

	public void Show(System.Action<int> _onSelected){
		if(currentState == State.Show){
			return;
		}
		currentState = State.Show;
		myCanvasGroup.blocksRaycasts = true;

		for(int i = 0; i < listOptions.Count; i++){
			listOptions[i].localScale = Vector3.zero;
		}

		onSelected = _onSelected;
		StartCoroutine(DoActionShow());
	}
	
	IEnumerator DoActionShow(){
		LeanTween.alphaCanvas(myCanvasGroup, 1f, 0.1f);
		for(int i = 0; i < listOptions.Count; i ++){
			if(Uno_GamePlay_Manager.instance.CanPlayMusicAndSfx()){
				MyAudioManager.instance.PlaySfx(Uno_GamePlay_Manager.instance.myAudioInfo.sfx_PanelChooseColorAppear);
			}
			LeanTween.scale(listOptions[i].gameObject, Vector3.one, 0.2f).setEase(LeanTweenType.easeOutBack);
			yield return Yielders.Get(0.1f);
		}
	}

	public void Hide(int _indexFocus){
		if(currentState == State.Hide){
			return;
		}
		currentState = State.Hide;
		myCanvasGroup.blocksRaycasts = false;
		onSelected = null;
		
		StartCoroutine(DoActionHide(_indexFocus));
	}

	IEnumerator DoActionHide(int _indexFocus){
		if(_indexFocus == -1){
			for(int i = 0; i < listOptions.Count; i ++){
				LeanTween.scale(listOptions[i].gameObject, Vector3.zero, 0.2f).setEase(LeanTweenType.easeInBack);
				yield return Yielders.Get(0.1f);
			}
			bool _isFinished = false;
			LeanTween.alphaCanvas(myCanvasGroup, 0f, 0.05f).setOnComplete(()=>{
				_isFinished = true;
			});
			yield return new WaitUntil(()=>_isFinished);
		}else{
			int _start = (_indexFocus + 1) % listOptions.Count;
			int _count = 0;
			while(_count < listOptions.Count - 1){
				LeanTween.scale(listOptions[_start].gameObject, Vector3.zero, 0.2f).setEase(LeanTweenType.easeInBack);
				_start ++;
				_start = _start % listOptions.Count;
				while(_start == _indexFocus){
					_start ++;
					_start = _start % listOptions.Count;
				}
				_count ++;
				yield return Yielders.Get(0.1f);
			}
			LeanTween.scale(listOptions[_indexFocus].gameObject, Vector3.one * 1.5f, 0.2f).setEase(LeanTweenType.easeOutBack);
			LeanTween.scale(listOptions[_indexFocus].gameObject, Vector3.zero, 0.2f).setEase(LeanTweenType.easeInBack).setDelay(0.5f);
			LeanTween.alphaCanvas(myCanvasGroup, 0f, 0.05f).setDelay(0.7f);
			yield return Yielders.Get(0.5f); 
		}
	}

	public void OnSelectColor(int _index){
		if(currentState == State.Hide){
			return;
		}
		if(onSelected != null){
			onSelected(_index);
		}
	}
}
