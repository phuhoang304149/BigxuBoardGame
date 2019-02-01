using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PanelCardDetailController : MySimplePoolObjectController {
	
	public enum State{
		Hide, Show
	}
	public State currentState{get;set;}
	
	[SerializeField] protected RectTransform myRectTransform;
	[SerializeField] protected RectTransform panelContainerRectTransform;
	[SerializeField] protected Image imgCardCover;
	[SerializeField] Text txtCardValue;
	[SerializeField] Image imgSmallCardKind;
	[SerializeField] Image imgBigCardKind;
	[SerializeField] protected Image panelHighLight;
	[SerializeField] protected Image panelShadow;
	[SerializeField] public CardHolderController cardHolder;

	protected IEnumerator actionMoveLocal;
	protected LTDescr tweenMoveLocal;

	private void Awake() {
		ResetData();
	}

	public override void ResetData(){
		StopAllCoroutines();
		LeanTween.cancel(gameObject);
		LeanTween.cancel(panelContainerRectTransform.gameObject);
		LeanTween.cancel(panelShadow.rectTransform);
		LeanTween.cancel(panelHighLight.rectTransform);

		actionMoveLocal = null;

		if(tweenMoveLocal != null){
			LeanTween.cancel(tweenMoveLocal.uniqueId);
			tweenMoveLocal = null;
		}
		
		currentState = State.Hide;
		imgCardCover.gameObject.SetActive(true);
		txtCardValue.gameObject.SetActive(false);
		imgSmallCardKind.gameObject.SetActive(false);
		imgBigCardKind.gameObject.SetActive(false);
		
		imgSmallCardKind.sprite = null;
		imgBigCardKind.sprite = null;
		txtCardValue.text = string.Empty;
		txtCardValue.color = Color.white;

		transform.localScale = Vector3.one;
		transform.localRotation = Quaternion.identity;
		panelContainerRectTransform.transform.localScale = Vector3.one;

		Color _c = panelHighLight.color;
		_c.a = 0f;
		panelHighLight.color = _c;

		SetUpShadow(false, true);

		DestroyCardHolder();
	}
	
	public void ResizeAgain(float _w, float _h){
		Vector2 _ratioScale = GetRatioScale(_w, _h);
		Vector2 _newSize = new Vector2(_w, _h);
		myRectTransform.sizeDelta = _newSize;

		Vector3 _localSize = panelContainerRectTransform.transform.localScale;
		_localSize.x = _ratioScale.x;
		_localSize.y = _ratioScale.y;
		panelContainerRectTransform.transform.localScale = _localSize;
	}

	public Vector2 GetRatioScale(float _w, float _h){
		Vector2 _ratioScale = Vector2.one;
		Vector2 _newSize = new Vector2(_w, _h);
		Vector2 _sz_01 = panelContainerRectTransform.sizeDelta;

		_ratioScale.x = _newSize.x / _sz_01.x;
		if(_ratioScale.x < 1f){
			_ratioScale.x = 1f;
		}
		_ratioScale.y = _newSize.y / _sz_01.y;
		if(_ratioScale.y < 1f){
			_ratioScale.y = 1f;
		}
		return _ratioScale;
	}

	[ContextMenu("ResizeAgain")]
	public void ResizeAgain(){
		Vector2 _sz_00 = myRectTransform.sizeDelta;
		Vector2 _sz_01 = panelContainerRectTransform.sizeDelta;

		float _ratioX = _sz_00.x / _sz_01.x;
		// if(_ratioX < 1f){
		// 	_ratioX = 1f;
		// }
		float _ratioY = _sz_00.y / _sz_01.y;
		// if(_ratioY < 1f){
		// 	_ratioY = 1f;
		// }

		Vector3 _localSize = panelContainerRectTransform.transform.localScale;
		_localSize.x = _ratioX;
		_localSize.y = _ratioY;
		panelContainerRectTransform.transform.localScale = _localSize;
	}

	public void ResetLocalPos(){
		transform.position = panelContainerRectTransform.transform.position;
		panelContainerRectTransform.transform.localPosition = Vector3.zero;
	}

	public void SetCardHolder(CardHolderController _cardHolder){
		cardHolder = _cardHolder;
	}

	public void DestroyCardHolder(){
		if(cardHolder != null){
			cardHolder.SelfDestruction();
			cardHolder = null;
		}
	}

	public void ShowNow(ICardInfo _cardInfo){
		// if(currentState == State.Show){
		// 	return;
		// }
		currentState = State.Show;
		imgCardCover.gameObject.SetActive(false);
		txtCardValue.gameObject.SetActive(true);
		imgSmallCardKind.gameObject.SetActive(true);
		imgBigCardKind.gameObject.SetActive(true);
		imgSmallCardKind.sprite = _cardInfo.imgKind;
		imgBigCardKind.sprite = _cardInfo.imgKind;
		txtCardValue.text = _cardInfo.strValue;
		txtCardValue.color = _cardInfo.colorValue;
	}

	public IEnumerator Show(ICardInfo _cardInfo, AudioClip _sfx = null){
		if(currentState == State.Show){
			yield break;
		}
		currentState = State.Show;
		float _saveX = transform.localScale.x;
		bool _isFinished = false;
		if(_sfx != null){
			MyAudioManager.instance.PlaySfx(_sfx);
		}
		LeanTween.scaleX(gameObject, 0f, 0.1f).setOnComplete(()=>{
			_isFinished = true;
		});
		yield return new WaitUntil(()=>_isFinished);

		ShowNow(_cardInfo);

		_isFinished = false;
		LeanTween.scaleX(gameObject, _saveX, 0.1f).setOnComplete(()=>{
			_isFinished = true;
		});
		yield return new WaitUntil(()=>_isFinished);
	}

	public virtual IEnumerator Hide(AudioClip _sfx = null){
		if(currentState == State.Hide){
			yield break;
		}
		currentState = State.Hide;
		bool _isFinished = false;
		if(_sfx != null){
			MyAudioManager.instance.PlaySfx(_sfx);
		}
		LeanTween.scaleX(gameObject, 0f, 0.1f).setOnComplete(()=>{
			_isFinished = true;
		});
		yield return new WaitUntil(()=>_isFinished);

		imgCardCover.gameObject.SetActive(true);
		txtCardValue.gameObject.SetActive(false);
		imgSmallCardKind.gameObject.SetActive(false);
		imgBigCardKind.gameObject.SetActive(false);

		imgSmallCardKind.sprite = null;
		imgBigCardKind.sprite = null;
		txtCardValue.text = string.Empty;
		txtCardValue.color = Color.white;

		_isFinished = false;
		LeanTween.scaleX(gameObject, 1f, 0.1f).setOnComplete(()=>{
			_isFinished = true;
		});
		yield return new WaitUntil(()=>_isFinished);
	}

	public IEnumerator Move(Vector2 _pos, float _timeMove, LeanTweenType _leanTweenType, AudioClip _sfx = null){
		bool _isFinished = false;
		if(_sfx != null){
			MyAudioManager.instance.PlaySfx(_sfx);
		}
		LeanTween.move(gameObject, _pos, _timeMove).setOnComplete(()=>{
			_isFinished = true;
		}).setEase(_leanTweenType);
		yield return new WaitUntil(()=>_isFinished);
	}
	
	public Coroutine MoveLocal(Vector2 _pos, float _timeMove, LeanTweenType _leanTweenType){
		if(actionMoveLocal != null){
			StopCoroutine(actionMoveLocal);
			actionMoveLocal = null;
		}
		if(tweenMoveLocal != null){
			LeanTween.cancel(tweenMoveLocal.uniqueId);
			tweenMoveLocal = null;
		}
		actionMoveLocal = DoActionMoveLocal(_pos, _timeMove, _leanTweenType);
		return StartCoroutine(actionMoveLocal);
	}
	IEnumerator DoActionMoveLocal(Vector2 _pos, float _timeMove, LeanTweenType _leanTweenType){
		bool _isFinished = false;
		tweenMoveLocal = LeanTween.moveLocal(panelContainerRectTransform.gameObject, _pos, _timeMove).setOnComplete(()=>{
			_isFinished = true;
			tweenMoveLocal = null;
		}).setEase(_leanTweenType);
		yield return new WaitUntil(()=>_isFinished);
		actionMoveLocal = null;
	}

	public IEnumerator Rotate(Vector3 _rot, float _timeRotate){
		bool _isFinished = false;
		LeanTween.rotate(gameObject, _rot, _timeRotate).setOnComplete(()=>{
			_isFinished = true;
		});
		yield return new WaitUntil(()=>_isFinished);
	}

	public IEnumerator ScaleTo(Vector2 _scale, float _time, LeanTweenType _leanTweenType, AudioClip _sfx = null){
		bool _isFinished = false;
		if(_sfx != null){
			MyAudioManager.instance.PlaySfx(_sfx);
		}
		LeanTween.scale(gameObject, _scale, _time).setOnComplete(()=>{
			_isFinished = true;
		}).setEase(_leanTweenType);
		yield return new WaitUntil(()=>_isFinished);
	}

	public IEnumerator MoveToCardHolder(float _time, LeanTweenType _leanTweenType){
		if(cardHolder == null){
			#if TEST
			Debug.LogError("cardHolder is null");
			#endif
			yield break;
		}
		yield return StartCoroutine(Move(cardHolder.transform.position, _time, _leanTweenType));
	}

	public void SetUpLoopHighlight(System.Action _onFinished = null){
		LeanTween.alpha(panelHighLight.rectTransform, 1f, 0.4f).setLoopPingPong(4).setOnComplete(_onFinished);
	}

	public void SetUpShadow(bool _active, bool _showNow = false){
		float _alpha = 0f;
		if(_active){
			_alpha = 0.7f;
		}
		if(_showNow){
			Color _c = panelShadow.color;
			_c.a = _alpha;
			panelShadow.color = _c;
		}else{
			LeanTween.alpha(panelShadow.rectTransform, _alpha, 0.1f);
		}
	}

	public override void SelfDestruction(){
		DestroyCardHolder();
		base.SelfDestruction();
	}
}
