using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Facebook;
using Facebook.Unity;

public class PanelPlayerInfoInGameController : MonoBehaviour {

	public enum MyType{
		Available, // hiện danh
		Incognito // ẩn danh
	}
	MyType myType;

	public enum State{
		Show,
		Hide
	}
	public State currentState{get;set;}
	
	[SerializeField] CanvasGroup myCanvasGroup;
	public ShakeController myShakeController;
	[SerializeField] Transform panelContainer;
	[SerializeField] Transform panelShadow;
	public RawImage imgAvatar;
	[SerializeField] Image imgIconAcc;
	[SerializeField] Image imgLoading;
	[SerializeField] Text txtNameShow;
	[SerializeField] Text txtGold;

	[Header("Setting PopupChat")]
	public PopupChatManager.PopupChatPosType popupChatPosType;
	public Transform popupChat_PlaceHolder_Top;
	public Transform popupChat_PlaceHolder_Left;
	public Transform popupChat_PlaceHolder_Right;
	public Transform popupChat_PlaceHolder_Bottom;
	public UserDataInGame data{get;set;}

	public PopupChatController currentPopupChat{get;set;}

	IEnumerator actionRefreshGoldInfo, actionCountDown;
	Coroutine actionLoadAvatar;

    long virtualGold, realGold;

	double timeCountDown, maxTimeCountDown, ratioCountDown;
	int minGoldCheck = 9999;

	void Awake(){
		ResetData();
	}

	void ResetData(){
		StopAllCoroutines();
        actionRefreshGoldInfo = null;
		actionCountDown = null;

		myType = MyType.Available;

		myCanvasGroup.alpha = 0f;
		myCanvasGroup.blocksRaycasts = false;
		imgAvatar.texture = CoreGameManager.instance.gameInfomation.otherInfo.avatarDefault;
		txtNameShow.text = string.Empty;
		txtGold.text = string.Empty;

		timeCountDown = 0f;
		maxTimeCountDown = 0f;
		ratioCountDown = 0f;
		imgLoading.fillAmount = 0f;

		virtualGold = realGold = 0;
		panelShadow.gameObject.SetActive(false);

		DestroyPopUpChat(currentPopupChat);

		data = null;

		SetShadow(false);
	}

	public void ResizePanelContainer(float _ratio){
		panelContainer.localScale = Vector3.one * _ratio;
	}

	public void InitData(UserDataInGame _data){
		myType = MyType.Available;

		bool _canLoadAvatar = false;
		if(data == null){
			data = _data;
			_canLoadAvatar = true;
			RefreshGoldInfo(true);
		}else {
			if(!data.IsEqual(_data)){
				if(actionLoadAvatar != null){
					StopCoroutine(actionLoadAvatar);
					actionLoadAvatar = null;
				}
				_canLoadAvatar = false;
				RefreshGoldInfo(true);
			}else{
				data = _data;
				_canLoadAvatar = true;
				RefreshGoldInfo();
			}
		}

		Sprite _iconDatabaseID = data.GetIconDatabaseID();
		if(_iconDatabaseID != null){
			imgIconAcc.gameObject.SetActive(true);
			imgIconAcc.sprite = _iconDatabaseID;
		}else{
			imgIconAcc.gameObject.SetActive(false);
		}

		txtNameShow.text = MyConstant.ConvertString(data.nameShowInGame , 8);
		
		if(_canLoadAvatar){
			actionLoadAvatar = data.LoadAvatar(this, imgAvatar.rectTransform.rect.width, imgAvatar.rectTransform.rect.height, (_avatar) => {
				try{
					if(_avatar != null){
						imgAvatar.texture = _avatar;
					}
				}catch{}

				actionLoadAvatar = null;
			});
		}
	}

	public void InitAsIncognito(UserDataInGame _data, string _txtNameShow = ""){ // Init dưới dạng ẩn danh
		myType = MyType.Incognito;
		if(data != null){
			if(!data.IsEqual(_data)){
				if(actionLoadAvatar != null){
					StopCoroutine(actionLoadAvatar);
					actionLoadAvatar = null;
				}
			}
		}
		data = _data;
		imgAvatar.texture = CoreGameManager.instance.gameInfomation.otherInfo.avatarIncognito;
		txtGold.text = string.Empty;
		imgIconAcc.gameObject.SetActive(false);

		if(string.IsNullOrEmpty(_txtNameShow)){
			txtNameShow.text = MyLocalize.GetString("Global/Wating");
		}else{
			txtNameShow.text = _txtNameShow;
		}
		
		if(data != null){
			actionLoadAvatar = data.LoadAvatar(this, imgAvatar.rectTransform.rect.width, imgAvatar.rectTransform.rect.height, (_avatar) => {
				actionLoadAvatar = null;
			});
		}
	}

	public void Show(){
		if(data == null){
			if(myType == MyType.Incognito){
				myCanvasGroup.alpha = 0.5f;
				myCanvasGroup.blocksRaycasts = false;
			}else{
				#if TEST
				Debug.LogError("data is NULL");
				#endif
			}
			return;
		}
		currentState = State.Show;
		if(myType == MyType.Available){
			myCanvasGroup.alpha = 1f;
			myCanvasGroup.blocksRaycasts = true;
		}else{
			myCanvasGroup.alpha = 0.5f;
			myCanvasGroup.blocksRaycasts = false;
		}
	}

	public void Hide(){
		currentState = State.Hide;
		myCanvasGroup.alpha = 0f;
		myCanvasGroup.blocksRaycasts = false; 
		ResetData();
	}

	public void AddPopUpChat(PopupChatController _popUpChat){
		if(currentPopupChat != null){
			DestroyPopUpChat(currentPopupChat);
		}
		currentPopupChat = _popUpChat;
		currentPopupChat.onSelfDestruction += DestroyPopUpChat;
	}

	public void DestroyPopUpChat(MySimplePoolObjectController _popUpChat){
		if(currentPopupChat != _popUpChat){
			#if TEST
			Debug.LogError("BUG Logic");
			#endif
		}
		if(currentPopupChat != null){
			currentPopupChat.onSelfDestruction -= DestroyPopUpChat;
			currentPopupChat.SelfDestruction();
		}
		currentPopupChat = null;
	}

	/* public void StopAllMyCoroutine(){
		if(data == null){
			return;
		}
        StopAllCoroutines();
        actionRefreshGoldInfo = null;
		actionCountDown = null;

		realGold = data.GetGoldView();
        virtualGold = realGold;

		timeCountDown = 0f;
		maxTimeCountDown = 0f;
		ratioCountDown = 0f;
		imgLoading.fillAmount = 0f;
	}*/

	public void RefreshGoldInfo(bool _updateNow = false){
		if(data == null){
			return;
		}
        if(txtGold == null){
            return;
        }

        realGold = data.GetGoldView();

        if(_updateNow){
			if(actionRefreshGoldInfo != null){
                StopCoroutine(actionRefreshGoldInfo);
                actionRefreshGoldInfo = null;
            }
			virtualGold = realGold;
			txtGold.text = MyConstant.GetMoneyString(virtualGold, minGoldCheck);
		}else{
            if(actionRefreshGoldInfo != null){
                StopCoroutine(actionRefreshGoldInfo);
                actionRefreshGoldInfo = null;
            }
            actionRefreshGoldInfo = MyConstant.TweenValue(virtualGold, realGold, 5, (_valueUpdate)=>{
                virtualGold = _valueUpdate;
                txtGold.text = MyConstant.GetMoneyString(virtualGold, minGoldCheck);
            }, (_valueFinish)=>{
                virtualGold = _valueFinish;
                txtGold.text = MyConstant.GetMoneyString(virtualGold, minGoldCheck);
                actionRefreshGoldInfo = null;
            });
            StartCoroutine(actionRefreshGoldInfo);
        }
    }
	
	public void StartCountDown(double _timeCountDown, double _maxTimeCountDown, System.Action _onFinished = null){
		if(_timeCountDown <= 0){
			if(_onFinished != null){
				_onFinished();
			}
			return;
		}
		timeCountDown = _timeCountDown;
		maxTimeCountDown = _maxTimeCountDown;
		ratioCountDown = timeCountDown / maxTimeCountDown;
		ratioCountDown = Mathf.Clamp((float) ratioCountDown, 0f, 1f);
		imgLoading.fillAmount = (float) ratioCountDown;
		if(ratioCountDown >= 0.5f){
			imgLoading.color = Color.green;
		}else if(ratioCountDown >= 0.2f){
			imgLoading.color = Color.yellow;
		}else{
			imgLoading.color = Color.red;
		}
		if(actionCountDown != null){
			StopCoroutine(actionCountDown);
			actionCountDown = null;
		}
		actionCountDown = DoActionStartCountDown(_onFinished);
		StartCoroutine(actionCountDown);
	}

	IEnumerator DoActionStartCountDown(System.Action _onFinished){
		while(timeCountDown > 0f){
			yield return null;
			timeCountDown -= Time.unscaledDeltaTime;
			if(timeCountDown < 0f){
				timeCountDown = 0f;
			}
			ratioCountDown = timeCountDown / maxTimeCountDown;
			ratioCountDown = Mathf.Clamp((float) ratioCountDown, 0f, 1f);
			imgLoading.fillAmount = (float) ratioCountDown;
			if(ratioCountDown >= 0.5f){
				imgLoading.color = Color.green;
			}else if(ratioCountDown >= 0.2f){
				imgLoading.color = Color.yellow;
			}else{
				imgLoading.color = Color.red;
			}
		}

		timeCountDown = 0f;
		maxTimeCountDown = 0f;
		ratioCountDown = 0f;
		imgLoading.fillAmount = 0f;

		yield return null;

		if(_onFinished != null){
			_onFinished();
		}
		actionCountDown = null;
	}

	public void StopCountDown(){
		if(actionCountDown != null){
			StopCoroutine(actionCountDown);
			actionCountDown = null;
		}
		timeCountDown = 0f;
		maxTimeCountDown = 0f;
		ratioCountDown = 0f;
		imgLoading.fillAmount = 0f;
	}

	public void OnButtonShowInfo(){
		if(currentState != State.Show){
			return;
		}
		if(data == null){
			return;
		}
		PopupManager.Instance.CreatePopupPlayerInfo(data);
	}

	public void SetShadow(bool _active){
		if(currentState != State.Show){
			return;
		}
		panelShadow.gameObject.SetActive(_active);
	}
}
