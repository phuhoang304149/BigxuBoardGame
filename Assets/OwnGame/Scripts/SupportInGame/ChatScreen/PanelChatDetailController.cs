using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;
using EmojiUI;

public class PanelChatDetailController : MySimplePoolObjectController {

	[SerializeField] CanvasGroup myCanvasGroup;
	[SerializeField] Text txtUserName;
	[SerializeField] RawImage imgAvatar;
	[SerializeField] RectTransform panelTxtContent;
	[SerializeField] EmojiText txtContent;
	[SerializeField] EmojiText tmpTxtContent;

	[Header("Setting")]
	[SerializeField] Vector2 deltaSizeBonusForPanelTxtContent;
	[SerializeField] int maxLengthOfUserName;
	[SerializeField] int maxWidthSizeTxtContent;
	[SerializeField] int minWidthSizeTxtContent;

	private void Awake() {
		ResetData();
	}

	public override void ResetData(){
		myCanvasGroup.alpha = 0f;
		txtUserName.text = string.Empty;
		txtContent.text = string.Empty;
		tmpTxtContent.text = string.Empty;
		imgAvatar.texture = GameInformation.instance.otherInfo.avatarDefault;
	}

	public void InitData(UserDataInGame _userData, string _chatConntent, System.Action<MySimplePoolObjectController> _onSelfDestruction){
		txtUserName.text = MyConstant.ConvertString(_userData.nameShowInGame, maxLengthOfUserName);
		_userData.LoadAvatar(this, imgAvatar.rectTransform.rect.width, imgAvatar.rectTransform.rect.height,
            (_avatar) =>
            {
				try{
					if(_avatar != null){
						imgAvatar.texture = _avatar;
					}
				}catch{}
            });
		txtContent.text = _chatConntent;
		tmpTxtContent.text = _chatConntent;
		onSelfDestruction = _onSelfDestruction;
	}

	public void Show(){
		StartCoroutine(DoActionShow());
	}
	
	IEnumerator DoActionShow(){
		yield return null;
		Vector2 _tmpSize = Vector2.zero;
		if(tmpTxtContent.rectTransform.rect.width > maxWidthSizeTxtContent){
			_tmpSize = txtContent.rectTransform.sizeDelta;
			_tmpSize.x = maxWidthSizeTxtContent;
			txtContent.rectTransform.sizeDelta = _tmpSize;
		}else if(tmpTxtContent.rectTransform.rect.width < minWidthSizeTxtContent){
			_tmpSize = txtContent.rectTransform.sizeDelta;
			_tmpSize.x = minWidthSizeTxtContent;
			txtContent.rectTransform.sizeDelta = _tmpSize;
		}else{
			_tmpSize = txtContent.rectTransform.sizeDelta;
			_tmpSize.x = tmpTxtContent.rectTransform.rect.width;
			txtContent.rectTransform.sizeDelta = _tmpSize;
		}
		yield return null;
		ResizeAgain();
		yield return Yielders.EndOfFrame;
		myCanvasGroup.alpha = 1f;
	}

	[ContextMenu("ResizeAgain")]
	void ResizeAgain(){
		Vector2 _tmpSize = txtContent.rectTransform.sizeDelta;
		_tmpSize.x += deltaSizeBonusForPanelTxtContent.x;
		_tmpSize.y += deltaSizeBonusForPanelTxtContent.y;
		panelTxtContent.sizeDelta = _tmpSize;
	}
}
