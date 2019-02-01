using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnnouncementController : MonoBehaviour {

	[SerializeField] CanvasGroup myCanvasgroup;
	[SerializeField] RectTransform panelContainer;
	[SerializeField] Text txtAnnouncement;
	[SerializeField] List<string> listAnnouncement{ get; set;}
	LTDescr tweenRollText, tweenShow, tweenHide;
	IEnumerator actionRollText;

	void Awake(){
		myCanvasgroup.alpha = 0f;
		listAnnouncement = new List<string> ();
	}

	void ResetData(){
		StopAllCoroutines ();
		LeanTween.cancel (gameObject);

		if (tweenRollText!= null){
			LeanTween.cancel (tweenRollText.uniqueId);
			tweenRollText = null;
		}
		if (tweenHide != null) {
			LeanTween.cancel (tweenHide.uniqueId);
			tweenHide = null;
		}
		if (tweenShow != null) {
			LeanTween.cancel (tweenShow.uniqueId);
			tweenShow = null;
		}

		actionRollText = null;

		myCanvasgroup.alpha = 0f;

		if (listAnnouncement == null) {
			listAnnouncement = new List<string> ();
		}else{
			listAnnouncement.Clear ();
		}
	}

	public void InitData(List<string> _listAnnouncement){
		listAnnouncement = _listAnnouncement;
	}

	public void Show(float _timeDelayFirst = 0f){
		if(listAnnouncement == null || listAnnouncement.Count == 0){
			return;
		}
		
		if (actionRollText == null) {
			actionRollText = DoActionRollText (_timeDelayFirst);
			StartCoroutine (actionRollText);
		}

		if (tweenHide != null) {
			LeanTween.cancel (tweenHide.uniqueId);
			tweenHide = null;
		}
		if (myCanvasgroup.alpha != 1f && tweenShow == null) {
			tweenShow = LeanTween.value (myCanvasgroup.alpha, 1f, 0.2f).setOnUpdate((_value)=>{
				myCanvasgroup.alpha = _value;
			}).setOnComplete(()=>{
				tweenShow = null;
			}).setDelay(_timeDelayFirst);
		}
	}

	IEnumerator DoActionRollText(float _timeDelay = 0){
		if (tweenRollText != null) {
			actionRollText = null;
			yield break;
		}
		if (listAnnouncement == null || listAnnouncement.Count > 0) {
			if(_timeDelay > 0){
				yield return Yielders.Get(_timeDelay);
			}

			int _tmpIndex = 0;
			Vector2 _pos = Vector2.zero;

			while(true){
				txtAnnouncement.text = listAnnouncement [_tmpIndex];
				_pos = txtAnnouncement.rectTransform.offsetMin;
				_pos.x = panelContainer.rect.width;
				txtAnnouncement.rectTransform.offsetMin = _pos;
				yield return Yielders.EndOfFrame;

				tweenRollText = LeanTween.moveX (txtAnnouncement.rectTransform, 0 - txtAnnouncement.rectTransform.rect.width, 10f).setOnComplete (() => {
					_tmpIndex ++;
					if(_tmpIndex >= listAnnouncement.Count){
						_tmpIndex = 0;
					}
					tweenRollText = null;
				});
				yield return new WaitUntil(()=> tweenRollText == null);
				yield return Yielders.Get(1f);
			}
		} else {
			yield return null;
			actionRollText = null;
		}
	}

	public void Hide(bool _hideNow = true){
		if (_hideNow) {
			myCanvasgroup.alpha = 0f;
			ResetData ();
		} else {
			if (tweenShow != null) {
				LeanTween.cancel (tweenShow.uniqueId);
				tweenShow = null;
			}
			if (myCanvasgroup.alpha != 0f && tweenHide == null) {
				tweenHide = LeanTween.value (myCanvasgroup.alpha, 0f, 0.2f).setOnUpdate((_value)=>{
					myCanvasgroup.alpha = _value;
				}).setOnComplete(()=>{
					tweenHide = null;
					ResetData();
				});
			}
		}
	}
}
