using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EmojiUI;
using Lean.Pool;
using UnityEngine.Serialization;

public class PopupChatController : MySimplePoolObjectController {
	[SerializeField] CanvasGroup myCanvasGroup;
	[SerializeField] PopupChatManager.PopupChatPosType posType;
	[SerializeField] RectTransform myRectTransform;
	[SerializeField] EmojiText txtContent;
	[SerializeField] EmojiText tmpTxtContent;

	[Header("Setting")]
	[SerializeField] Vector2 deltaSizeBonusForPanelTxtContent;
	[SerializeField] int maxTxtLenght;
	[SerializeField] float maxWidthPanelText = 110;
	[SerializeField] float minWidthPanelText = 30;

	private void Awake() {
		ResetData();
	}
	
	public override void ResetData(){
		myCanvasGroup.alpha = 0f;
		txtContent.text = string.Empty;
		tmpTxtContent.text = string.Empty;
		transform.localScale = Vector3.zero;
	}
	
	// private void Start() {
	// 	Invoke("TEST", 2f);
	// }

	// void TEST(){
	// 	LeanTween.scale(gameObject, Vector3.one, 0.2f).setEase(LeanTweenType.easeOutBack);
	// }

	public void InitData(string _chatConntent){
		string _tmpMess = MyConstant.ConvertString(_chatConntent, maxTxtLenght);
		txtContent.text = _tmpMess;
		tmpTxtContent.text = _tmpMess;
	}

	public void Show(){
		StartCoroutine(DoActionShow());
	}

	// [ContextMenu("aaaaa")]
	// void AAAAA(){
	// 	Vector2 _pos = myRectTransform.anchoredPosition;
	// 	Debug.Log("111 " + _pos.x);
	// 	_pos.x += myRectTransform.rect.width / 2f - 21f;
	// 	Debug.Log("222 " + _pos.x);
	// 	myRectTransform.anchoredPosition = _pos;

	// 	Vector2 _pos = myRectTransform.anchoredPosition;
	// 	Debug.Log("111 " + _pos.y);
	// 	_pos.y -= myRectTransform.rect.height / 2f - 21f;
	// 	Debug.Log("222 " + _pos.y);
	// 	myRectTransform.anchoredPosition = _pos;
	// }

	IEnumerator DoActionShow(){
		yield return null;
		Vector2 _tmpSize = Vector2.zero;
		if(tmpTxtContent.rectTransform.rect.width > maxWidthPanelText){
			_tmpSize = txtContent.rectTransform.sizeDelta;
			_tmpSize.x = maxWidthPanelText;
			txtContent.rectTransform.sizeDelta = _tmpSize;
		}else if(tmpTxtContent.rectTransform.rect.width < minWidthPanelText){
			_tmpSize = txtContent.rectTransform.sizeDelta;
			_tmpSize.x = minWidthPanelText;
			txtContent.rectTransform.sizeDelta = _tmpSize;
		}else{
			_tmpSize = txtContent.rectTransform.sizeDelta;
			_tmpSize.x = tmpTxtContent.rectTransform.rect.width;
			txtContent.rectTransform.sizeDelta = _tmpSize;
		}
		yield return null;
		ResizeAgain();
		yield return null;

		// --- Cân chỉnh lại vị trí cho đúng với dấu mốc trong khung chat --- //
		Vector3 _pos = myRectTransform.anchoredPosition;
		switch(posType){
		case PopupChatManager.PopupChatPosType.Top:
		case PopupChatManager.PopupChatPosType.Bottom:
			_pos.x += myRectTransform.rect.width / 2f - 21f;
			break;
		case PopupChatManager.PopupChatPosType.Left:
		case PopupChatManager.PopupChatPosType.Right:
			_pos.y -= myRectTransform.rect.height / 2f - 21f;
			break;
		default:
			Debug.LogError("Cần thêm vào PopupChatPosType: " + posType.ToString());
			break;
		}
		myRectTransform.anchoredPosition = _pos;
		// ------------------------------------------------------------------ //

		transform.localScale = Vector3.zero;
		myCanvasGroup.alpha = 1f;
		LeanTween.scale(gameObject, Vector3.one, 0.2f).setEase(LeanTweenType.easeOutBack);
	}

	void ResizeAgain(){
		Vector2 _tmpSize = txtContent.rectTransform.sizeDelta;
		_tmpSize.x += deltaSizeBonusForPanelTxtContent.x;
		_tmpSize.y += deltaSizeBonusForPanelTxtContent.y;
		myRectTransform.sizeDelta = _tmpSize;
	}

	[ContextMenu("TestResizeAgain")]
	void TestResizeAgain(){
		Vector2 _tmpSize = Vector2.zero;
		if(tmpTxtContent.rectTransform.rect.width > maxWidthPanelText){
			_tmpSize = txtContent.rectTransform.sizeDelta;
			_tmpSize.x = maxWidthPanelText;
			txtContent.rectTransform.sizeDelta = _tmpSize;
		}else if(tmpTxtContent.rectTransform.rect.width < minWidthPanelText){
			_tmpSize = txtContent.rectTransform.sizeDelta;
			_tmpSize.x = minWidthPanelText;
			txtContent.rectTransform.sizeDelta = _tmpSize;
		}else{
			_tmpSize = txtContent.rectTransform.sizeDelta;
			_tmpSize.x = tmpTxtContent.rectTransform.rect.width;
			txtContent.rectTransform.sizeDelta = _tmpSize;
		}

		_tmpSize.x += deltaSizeBonusForPanelTxtContent.x;
		_tmpSize.y += deltaSizeBonusForPanelTxtContent.y;
		myRectTransform.sizeDelta = _tmpSize;
	}

	// [ContextMenu("AAAA")]
	// void AAAA(){
	// 	Vector3 _pos = transform.position;
	// 	_pos.y = 0f;
	// 	transform.position = _pos;
	// }
}
