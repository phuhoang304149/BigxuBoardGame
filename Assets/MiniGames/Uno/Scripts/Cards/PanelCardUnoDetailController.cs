using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PanelCardUnoDetailController : PanelCardDetailController, IPointerDownHandler {
	
	[Header("Card Uno Detail")]
	[SerializeField] CanvasGroup myCanvasGroup;
	[SerializeField] Image imgBg;
	[SerializeField] RectTransform rectTransformBg;
	[SerializeField] Image imgOval;
	[SerializeField] Image imgMainIcon;
	[SerializeField] Shadow shadowImgMainIcon;
	[SerializeField] List<Image> imgMiniIcon;
	[SerializeField] List<Text> txtValue;
	[SerializeField] CardUnoInfo cardInfoDefault;

	[Header("Setting")]
	[SerializeField] Color bgColor_Red;
	[SerializeField] Color bgColor_Green;
	[SerializeField] Color bgColor_Blue;
	[SerializeField] Color bgColor_Yellow;

	public bool canPut{get;set;}
	public int cardValue{get;set;}
	public CardUnoInfo cardInfo{get;set;}
	public System.Action<PanelCardUnoDetailController> onPointerDown;

	public override void ResetData(){
		StopAllCoroutines();
		LeanTween.cancel(gameObject);
		LeanTween.cancel(panelShadow.rectTransform);
		LeanTween.cancel(panelHighLight.rectTransform);

		SetVisible();
		
		currentState = State.Hide;

		transform.localScale = Vector3.one;
		transform.localRotation = Quaternion.identity;
		panelContainerRectTransform.transform.localScale = Vector3.one;

		Color _c = panelShadow.color;
		_c.a = 0f;
		panelShadow.color = _c;

		_c = panelHighLight.color;
		_c.a = 0f;
		panelHighLight.color = _c;

		SetCard(cardInfoDefault, -1);

		onPointerDown = null;
		
		canPut = false;
	}

	public void SetVisible(){
		myCanvasGroup.alpha = 1f;
		myCanvasGroup.blocksRaycasts = true;
	}

	public void SetInVisible(){
		myCanvasGroup.alpha = 0f;
		myCanvasGroup.blocksRaycasts = false;
	}

	public void ShowNow(CardUnoInfo _cardInfo, int _cardId){
		if(currentState == State.Show){
			return;
		}
		currentState = State.Show;
		SetVisible();
		if(_cardInfo == null){
			SetCard(cardInfoDefault, -1);
		}else{
			SetCard(_cardInfo, _cardId);
		}
	}

	public IEnumerator Show(CardUnoInfo _cardInfo, int _cardId, float _timeShow){
		if(currentState == State.Show){
			yield break;
		}
		currentState = State.Show;
		SetVisible();
		float _saveX = transform.localScale.x;
		bool _isFinished = false;
		LeanTween.scaleX(gameObject, 0f, _timeShow/2f).setOnComplete(()=>{
			_isFinished = true;
		});
		yield return new WaitUntil(()=>_isFinished);

		SetCard(_cardInfo, _cardId);

		_isFinished = false;
		LeanTween.scaleX(gameObject, _saveX, _timeShow/2f).setOnComplete(()=>{
			_isFinished = true;
		});
		yield return new WaitUntil(()=>_isFinished);
	}

	void SetCard(CardUnoInfo _cardInfo, int _cardId){
		cardInfo = _cardInfo;
		cardValue = _cardId;
		imgBg.color = cardInfo.colorValue;
		imgOval.color = cardInfo.colorImgOval;
		imgMainIcon.sprite = cardInfo.imgMainIcon;
		shadowImgMainIcon.enabled = true;
		if(cardInfo.cardType != CardUnoInfo.CardType._Red_Draw2Cards
			&& cardInfo.cardType != CardUnoInfo.CardType._Green_Draw2Cards
			&& cardInfo.cardType != CardUnoInfo.CardType._Blue_Draw2Cards
			&& cardInfo.cardType != CardUnoInfo.CardType._Yellow_Draw2Cards
			&& cardInfo.cardType != CardUnoInfo.CardType._Special_Draw4Cards
			&& cardInfo.cardType != CardUnoInfo.CardType._Special_WildColor
			&& cardInfo.cardType != CardUnoInfo.CardType._CardCover){
			imgMainIcon.color = cardInfo.colorValue;
		}else{
			imgMainIcon.color = Color.white;
			if(cardInfo.cardType == CardUnoInfo.CardType._Special_WildColor
				|| cardInfo.cardType == CardUnoInfo.CardType._CardCover){
				shadowImgMainIcon.enabled = false;
			}
		}
		imgMainIcon.SetNativeSize();

		if(cardInfo.cardType != CardUnoInfo.CardType._CardCover){
			if(cardInfo.strValue.Equals("-1")){
				for(int i = 0; i < imgMiniIcon.Count; i++){
					imgMiniIcon[i].gameObject.SetActive(true);
					imgMiniIcon[i].sprite = cardInfo.imgMainIcon;
				}
				for(int i = 0; i < txtValue.Count; i++){
					txtValue[i].gameObject.SetActive(false);
				}
			}else{
				for(int i = 0; i < imgMiniIcon.Count; i++){
					imgMiniIcon[i].gameObject.SetActive(false);
				}
				for(int i = 0; i < txtValue.Count; i++){
					txtValue[i].gameObject.SetActive(true);
					txtValue[i].text = cardInfo.strValue;
				}
			}
		}else{
			for(int i = 0; i < imgMiniIcon.Count; i++){
				imgMiniIcon[i].gameObject.SetActive(false);
			}
			for(int i = 0; i < txtValue.Count; i++){
				txtValue[i].gameObject.SetActive(false);
			}
		}
	}

	public IEnumerator DoActionChangeBgColor(UnoGamePlayData.BackgroundColor _bgColor, bool _updateNow = true){
		Color _c = bgColor_Yellow;
		switch(_bgColor){
		case UnoGamePlayData.BackgroundColor.Red:
			_c = bgColor_Red;
			break;
		case UnoGamePlayData.BackgroundColor.Green:
			_c = bgColor_Green;
			break;
		case UnoGamePlayData.BackgroundColor.Blue:
			_c = bgColor_Blue;
			break;
		case UnoGamePlayData.BackgroundColor.Yellow:
			_c = bgColor_Yellow;
			break;
		}
		if(_updateNow){
			imgBg.color = _c;
			yield break;
		}else{
			bool _isFinished = false;
			LeanTween.color(rectTransformBg, _c, 0.2f).setOnComplete(()=>{
				_isFinished = true;
			});
			yield return new WaitUntil(()=>_isFinished);
		}
	}

	public override IEnumerator Hide(AudioClip _sfx = null){
		if(currentState == State.Hide){
			yield break;
		}
		currentState = State.Hide;
		float _saveX = transform.localScale.x;
		bool _isFinished = false;
		LeanTween.scaleX(gameObject, 0f, 0.1f).setOnComplete(()=>{
			_isFinished = true;
		});
		yield return new WaitUntil(()=>_isFinished);

		SetCard(cardInfoDefault, -1);

		_isFinished = false;
		LeanTween.scaleX(gameObject, _saveX, 0.1f).setOnComplete(()=>{
			_isFinished = true;
		});
		yield return new WaitUntil(()=>_isFinished);
	}
	
	public virtual void OnPointerDown(PointerEventData eventData)
    {
		if(onPointerDown != null){
			onPointerDown(this);
		}
    }

	public void SetCanPut(bool _flag, bool _setShadow = true){
		canPut = _flag;
		if(_setShadow){
			if(canPut){
				SetUpShadow(false);
			}else{
				SetUpShadow(true);
			}
		}
	}

	public void ShowHighlight(){
		LeanTween.cancel(panelHighLight.rectTransform);
		LeanTween.alpha(panelHighLight.rectTransform, 1f, 0.1f);
	}

	public void HideHighlight(){
		LeanTween.cancel(panelHighLight.rectTransform);
		LeanTween.alpha(panelHighLight.rectTransform, 0f, 0.1f);
	}

	// public float asdasdassd;
	// [ContextMenu("TEST AAAA")]
	// public void TESTAAAAA(){
	// 	MoveLocal(Vector2.up * asdasdassd, 0.2f, LeanTweenType.easeOutBack);
	// }

	// [ContextMenu("TEST BBBB")]
	// public void TESTBBB(){
	// 	MoveLocal(Vector2.zero, 0.2f, LeanTweenType.easeOutBack);
	// }
}