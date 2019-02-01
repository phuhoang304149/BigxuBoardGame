using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;
using UnityEngine.SceneManagement;

public class GetGoldScreen_PanelDailyLogin_Controller : MySimplePanelController {

	[SerializeField] CanvasGroup myCanvasGroup;
	[SerializeField] Text txtEmpty;
	[SerializeField] RectTransform rewardContent;
	[SerializeField] RectTransform rewardContentHolder;
	[SerializeField] Transform panelFocusScreen;
	[SerializeField] Transform panelShare;
	[SerializeField] Text txtCountDown;
	[SerializeField] ShakeController countDownShakeController;

	[Header("Prefab")]
	[SerializeField] GameObject prefabPanelReward;

	MySimplePoolManager listPanelReward;
	
	IEnumerator actionCountDown;
	IEnumerator actionCheckViewAds;

	public override void ResetData(){
		StopAllCoroutines();
		actionCountDown = null;
		actionCheckViewAds = null;

		myCanvasGroup.alpha = 0f;
		myCanvasGroup.blocksRaycasts = false;

		panelFocusScreen.gameObject.SetActive(false);

		txtCountDown.rectTransform.rotation = Quaternion.identity;
		txtCountDown.rectTransform.localScale = Vector3.one;
		txtCountDown.color = Color.black;
		countDownShakeController.ResetData();

		if(listPanelReward != null){
			listPanelReward.ClearAllObjectsNow();
		}
		txtEmpty.gameObject.SetActive(false);
	}

	public override void InitData (System.Action _onFinished = null){
		if(listPanelReward == null){
			listPanelReward = new MySimplePoolManager();
		}else{
			listPanelReward.ClearAllObjectsNow();
		}
		if(DataManager.instance.dailyRewardData.listRewards.Count == 0){
			// txtEmpty.gameObject.SetActive(true);
			return;
		}
		// txtEmpty.gameObject.SetActive(false);

		float _ratioScaleMainContent = rewardContentHolder.rect.size.y / rewardContent.rect.size.y;
		_ratioScaleMainContent = Mathf.Clamp(_ratioScaleMainContent, 0.8f, 1.1f);
		rewardContent.transform.localScale = Vector3.one * _ratioScaleMainContent;

		GetGoldScreen_PanelDailyLogin_OptionRewardInfo_Controller _tmpPanel = null;
		for(int i = 0; i < DataManager.instance.dailyRewardData.listRewards.Count; i++){
			_tmpPanel = LeanPool.Spawn(prefabPanelReward, Vector3.zero, Quaternion.identity, rewardContent.transform).GetComponent<GetGoldScreen_PanelDailyLogin_OptionRewardInfo_Controller>();
			_tmpPanel.InitData(i, DataManager.instance.dailyRewardData.listRewards[i], OnButtonRecieveRewardClicked);
			listPanelReward.AddObject(_tmpPanel);
		}
		int _tmpIndex = DataManager.instance.dailyRewardData.listRewards.Count;
		_tmpPanel = LeanPool.Spawn(prefabPanelReward, Vector3.zero, Quaternion.identity, rewardContent.transform).GetComponent<GetGoldScreen_PanelDailyLogin_OptionRewardInfo_Controller>();
		_tmpPanel.InitData(DataManager.instance.dailyRewardData.listRewards.Count, DataManager.instance.dailyRewardData.listRewards[_tmpIndex - 1], OnButtonRecieveRewardClicked);
		listPanelReward.AddObject(_tmpPanel);
		
		panelShare.SetAsLastSibling();

		RefreshData();
	}

	public override void RefreshData(){
		if(listPanelReward == null || listPanelReward.listObjects.Count == 0){
			return;
		}
		GetGoldScreen_PanelDailyLogin_OptionRewardInfo_Controller.State _panelState = GetGoldScreen_PanelDailyLogin_OptionRewardInfo_Controller.State.CanNotClaim;;
		bool _canRecieveReward = DataManager.instance.dailyRewardData.CanRecieveReward();
		bool _canReset = DataManager.instance.dailyRewardData.CanReset();
		if(_canReset){
			DataManager.instance.dailyRewardData.ResetLoginData();
			_canRecieveReward = true;
		}
		int _indexDay = -1;
		for(int i = 0; i < listPanelReward.listObjects.Count; i ++){
			_panelState = GetGoldScreen_PanelDailyLogin_OptionRewardInfo_Controller.State.CanNotClaim;
			if(i < DataManager.instance.dailyRewardData.currentDayLogin){
				_panelState = GetGoldScreen_PanelDailyLogin_OptionRewardInfo_Controller.State.HadClaimed;
			}else if(i > DataManager.instance.dailyRewardData.currentDayLogin){
				_panelState = GetGoldScreen_PanelDailyLogin_OptionRewardInfo_Controller.State.CanNotClaim;
			}else{
				if(_canRecieveReward){
					_panelState = GetGoldScreen_PanelDailyLogin_OptionRewardInfo_Controller.State.CanClaim;
					_indexDay = i;
				}else{
					_panelState = GetGoldScreen_PanelDailyLogin_OptionRewardInfo_Controller.State.CanNotClaim;
					if(i == listPanelReward.listObjects.Count - 1){
						if(DataManager.instance.dailyRewardData.lastDayLogin == DataManager.instance.dailyRewardData.currentDayLogin){
							_panelState = GetGoldScreen_PanelDailyLogin_OptionRewardInfo_Controller.State.HadClaimed;
						}
					}
				}
			}
			((GetGoldScreen_PanelDailyLogin_OptionRewardInfo_Controller) listPanelReward.listObjects[i]).RefreshData(_panelState);
		}
		if(_indexDay >= 0){
			((GetGoldScreen_PanelDailyLogin_OptionRewardInfo_Controller) listPanelReward.listObjects[_indexDay]).SetFocus();
			txtCountDown.rectTransform.rotation = Quaternion.identity;
			txtCountDown.rectTransform.localScale = Vector3.one;
			txtCountDown.color = Color.black;
			countDownShakeController.ResetData();
			txtCountDown.gameObject.SetActive(false);
		}else{
			for(int i = 0; i < listPanelReward.listObjects.Count; i++){
				((GetGoldScreen_PanelDailyLogin_OptionRewardInfo_Controller) listPanelReward.listObjects[i]).SetUnFocus();
			}

			System.TimeSpan _tmpTime = DataManager.instance.dailyRewardData.timeToGetReward.Subtract(System.DateTime.Now);
			txtCountDown.text = string.Format("{0:00}:{1:00}:{2:00}", _tmpTime.Hours + (_tmpTime.Days * 24), _tmpTime.Minutes, _tmpTime.Seconds);
			txtCountDown.gameObject.SetActive(true);

			if(actionCountDown != null){
				StopCoroutine(actionCountDown);
				actionCountDown = null;
			}
			actionCountDown = DoActionCountDown();
			StartCoroutine(actionCountDown);
		}
	}

	// IEnumerator WaitToSetFocus(int _indexDay){
	// 	yield return Yielders.EndOfFrame;
	// 	((GetGoldScreen_PanelDailyLogin_OptionRewardInfo_Controller) listPanelReward.listObjects[_indexDay]).SetFocus();
	// }
	
	// IEnumerator WaitToSetUnFocusAll(){
	// 	yield return Yielders.EndOfFrame;
	// 	((GetGoldScreen_PanelDailyLogin_OptionRewardInfo_Controller) listPanelReward.listObjects[_indexDay]).SetFocus();
	// }

	IEnumerator DoActionCountDown(){
		System.TimeSpan _tmpTime;
		while(DataManager.instance.dailyRewardData.timeToGetReward > System.DateTime.Now){
			yield return Yielders.Get(1f);
			_tmpTime = DataManager.instance.dailyRewardData.timeToGetReward.Subtract(System.DateTime.Now);
			txtCountDown.text = string.Format("{0:00}:{1:00}:{2:00}", _tmpTime.Hours + (_tmpTime.Days * 24), _tmpTime.Minutes, _tmpTime.Seconds);
		}
		RefreshData();
	}

	public override Coroutine Show (){
		myCanvasGroup.alpha = 1f;
		myCanvasGroup.blocksRaycasts = true;
		panelFocusScreen.gameObject.SetActive(true);
		return null;
	}

	public override Coroutine Hide (){
		ResetData();
		return null;
	}

	#region On Button Clicked
	public void OnButtonRecieveRewardClicked(){
		if(!GetGoldScreenController.instance.canTouch){
			return;
		}

		if(DataManager.instance.dailyRewardData.hadCheckSv && !DataManager.instance.dailyRewardData.CanRecieveReward()){
			ShowEffCanNotGetReward();
			return;
		}

		if(actionCheckViewAds == null){
			actionCheckViewAds = DoActionCheckViewAds();
			StartCoroutine(actionCheckViewAds);
		}
	}

	IEnumerator DoActionCheckViewAds(){
		bool _isViewingAds = false;
		bool _isViewAdsSuccessfully = false;

		bool _canShowVideoAds = true;
		bool _canShowInterstitial = true;
		if(HomeManager.instance == null){
			_canShowVideoAds = false;
		}

		if(_canShowVideoAds){
			if(AdmobController.instance.usingVideoAds){
				if(AdmobController.instance.rewardBasedVideo.IsLoaded()){
					_isViewingAds = true;
					_isViewAdsSuccessfully = false;
					AdmobController.instance.ShowRewardBasedVideo(()=>{
						_isViewAdsSuccessfully = true;
					}, ()=>{
						_isViewingAds = false;
					});
					yield return new WaitUntil(()=>!_isViewingAds);
					if(_isViewAdsSuccessfully){
						SendMessageGetGoldDaily();
					}
					actionCheckViewAds = null;
					yield break;
				}else{
					AdmobController.instance.RequestRewardBasedVideo();
				}
			}else{
				#if TEST
				Debug.LogError("Không có sử dụng video ads");
				#endif
			}
		}

		if(_canShowInterstitial){
			if(AdmobController.instance.usingInterstitial){
				if(AdmobController.instance.interstitial.IsLoaded()){
					_isViewingAds = true;
					_isViewAdsSuccessfully = false;
					AdmobController.instance.ShowInterstitial(()=>{
						_isViewAdsSuccessfully = true;
					}, ()=>{
						_isViewingAds = false;
					});
					yield return new WaitUntil(()=>!_isViewingAds);
					if(_isViewAdsSuccessfully){
						SendMessageGetGoldDaily();
					}
					actionCheckViewAds = null;
					yield break;
				}else{
					AdmobController.instance.RequestRewardBasedVideo();
				}
			}else{
				#if TEST
				Debug.LogError("Không có sử dụng Interstitial");
				#endif
			}
		}

		SendMessageGetGoldDaily();
		actionCheckViewAds = null;
	}

	void SendMessageGetGoldDaily(){
		SubServerDetail _serverDetail = GetGoldScreenController.instance.GetServerDetail();
		OneHitAPI.GetGoldDaily (_serverDetail,(_messageReceiving, _error) => {
				if(_messageReceiving != null){
					DataManager.instance.dailyRewardData.RecieveReward(_messageReceiving, (_caseCheck)=>{
						if(_caseCheck == -2){
							DataManager.instance.dailyRewardData.hadCheckSv = true;
							ShowEffCanNotGetReward();
						}else if(_caseCheck == 1){
							DataManager.instance.dailyRewardData.hadCheckSv = true;
							GetGoldScreenController.instance.RefreshMyGoldInfo(false);
							if(HomeManager.instance != null && HomeManager.instance.myCallbackManager != null
								&& HomeManager.instance.myCallbackManager.onLoadDataGoldGemFinished != null){
								HomeManager.instance.myCallbackManager.onLoadDataGoldGemFinished();
							}
						}
					});
					RefreshData();
				}else{
					#if TEST
					Debug.LogError("GetGoldDaily Error: " + _error);
					#endif
				}
			}
		);
	}

	void ShowEffCanNotGetReward(){
		countDownShakeController.SetUpShakeWorldPoint(0.2f);
		LeanTween.scale(txtCountDown.gameObject, Vector3.one * 1.2f, 0.2f).setLoopPingPong(1).setEase(LeanTweenType.easeOutBack);
		LeanTween.colorText(txtCountDown.rectTransform, Color.red, 0.2f).setLoopPingPong(1).setEase(LeanTweenType.easeOutBack);
		CoreGameManager.instance.DoVibrate();

		MyAudioManager.instance.PlaySfx(GetGoldScreenController.instance.sfx_NotThisTime);
	}
	#endregion

	#region On Button Clicked
	public void OnButtonShareClick(){
		#if TEST
		Debug.Log("Share!");
		#endif
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);

		new NativeShare().SetTitle("Share & Invite").SetText("https://sites.google.com/view/bigxu-online/home").Share();
		
		// if(!FacebookAPI.IsLoggedIn()){
		// 	if(actionSigningFb == null){
		// 		actionSigningFb = FacebookAPI.DoActionLoginFb(()=>{
		// 			Debug.Log("Login FB successful!");
        //             var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
        //             Debug.Log("UserID: " + aToken.UserId);
		// 		}, ()=>{
		// 			actionSigningFb = null;
		// 		});
		// 		StartCoroutine(actionSigningFb);
		// 	}
		// }else{
		// 	FacebookAPI.Share("https://sites.google.com/view/bigxu-online/home"
		// 		, "Bigxu Online"
		// 		, ""
		// 		, "https://lh5.googleusercontent.com/dvN8T35wbc5MbLIDftVrYMDT2Z00HQujj6xOfqeT8zPduhmLgNpqO3aIf2WCHqitsW9w9kI=w170"
		// 		, (_result)=>{
		// 			if (_result.Cancelled) {
		// 				#if TEST
		// 				Debug.Log("Cancel!");
		// 				#endif
		// 			}else if (_result.Error != null) {
		// 				#if TEST
		// 				Debug.LogError("Share FB error: " + _result.Error);
		// 				#endif
		// 			}else{
		// 				#if TEST
		// 				Debug.Log("Share thành công!");
		// 				#endif
		// 			}
		// 		}
		// 	);
		// }
	}
	#endregion
}
