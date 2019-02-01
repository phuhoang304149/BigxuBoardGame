using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ToastController : IPopupController {

	public static Vector3 posDefault = new Vector3(0f, 1.5f, 0f);
	public static float maxSizeWDefault = 800f;
	public static Color colorTextDefault = Color.white;

	enum State{
		None, Begin, Idle, End
	}
	State currentState;

	[SerializeField] Text textInfo;
	[SerializeField] RectTransform myRectTransformMainContent;
	LTDescr myTweenCavasGroup, myTweenMainContent, myTweenScaleText;

	[Header("Setting")]
	[SerializeField] float timeDefaulShowBegin;
	[SerializeField] float timeDefaulShowIdle;
	[SerializeField] float timeDefaulShowEnd;

	bool speedUp;
	float timeShowBegin, timeShowIdle, timeShowEnd;


	public override void ResetData(){
		base.ResetData();

		currentState = State.None;
		LeanTween.cancel(textInfo.gameObject);
		LeanTween.cancel(myCanvasGroup.gameObject);
		LeanTween.cancel(myRectTransformMainContent.gameObject);

		myTweenCavasGroup = null;
		myTweenMainContent = null;
		myTweenScaleText = null;

		myCanvasGroup.alpha = 0f;

		speedUp = false;
	}

	public void Init(string _textInfo, Color _colorText, float _maxSizeW, System.Action _onClose = null){
		textInfo.text = _textInfo;
		textInfo.color = _colorText;
		onClose = _onClose;

		Vector2 _sz = myRectTransformMainContent.sizeDelta; 
		_sz.x = _maxSizeW;
		myRectTransformMainContent.sizeDelta = _sz;

		speedUp = false;
		timeShowBegin = timeDefaulShowBegin;
		timeShowIdle = timeDefaulShowIdle;
		timeShowEnd = timeDefaulShowEnd;

		myCanvasGroup.alpha = 0f;
		textInfo.transform.localScale = new Vector3(0.8f, 0.6f, 1f);
		myRectTransformMainContent.transform.localPosition = Vector3.zero;

		Show();
	}

	public override void Show(){
		SetBegin();
	}

	void SetBegin(){
		currentState = State.Begin;

		if(myTweenCavasGroup != null){
			LeanTween.cancel(myCanvasGroup.gameObject, myTweenCavasGroup.uniqueId);
		}
		myTweenCavasGroup = LeanTween.alphaCanvas(myCanvasGroup, 1f, timeShowBegin).setOnComplete(()=>{
			myTweenCavasGroup = null;
		});

		if(myTweenScaleText != null){
			LeanTween.cancel(textInfo.gameObject, myTweenScaleText.uniqueId);
		}
		myTweenScaleText = LeanTween.scale(textInfo.gameObject, Vector3.one, timeShowBegin).setOnComplete(()=>{
			myTweenScaleText = null;
		});

		if(myTweenMainContent != null){
			LeanTween.cancel(myRectTransformMainContent.gameObject, myTweenMainContent.uniqueId);
		}
		myTweenMainContent = LeanTween.moveLocalY(myRectTransformMainContent.gameObject, 5f, timeShowBegin).setOnComplete(()=>{
			myTweenMainContent = null;
			SetIdle();
		});
	}

	void SetIdle(){
		currentState = State.Idle;

		if(myTweenMainContent != null){
			LeanTween.cancel(myRectTransformMainContent.gameObject, myTweenMainContent.uniqueId);
		}
		myTweenMainContent = LeanTween.moveLocalY(myRectTransformMainContent.gameObject, 20f, timeShowIdle).setOnComplete(()=>{
			myTweenMainContent = null;
			SetEnd();
		});
	}

	void SetEnd(){
		currentState = State.End;

		if(myTweenMainContent != null){
			LeanTween.cancel(myRectTransformMainContent.gameObject, myTweenMainContent.uniqueId);
		}
		myTweenCavasGroup = LeanTween.alphaCanvas(myCanvasGroup, 0f, timeShowEnd).setOnComplete(()=>{
			myTweenCavasGroup = null;
			Close();
		});
	}

	public void SetSpeedUp(){
		if(speedUp){
			return;
		}
		speedUp = true;
		timeShowBegin /= 2;
		timeShowIdle /= 2;
		timeShowEnd /= 2;

		if(currentState == State.Begin){
			SetBegin();
		}else if(currentState == State.Idle){
			SetIdle();
		}else if(currentState == State.End){
			SetEnd();
		}else{
			Debug.LogError("BUG LOGIC!");
		}
	}

	public override void Close(){
		if(onClose != null){
			onClose();
			onClose = null;
		}
		SelfDestruction();
	}
}
