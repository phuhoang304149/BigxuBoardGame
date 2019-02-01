using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GetGoldScreen_PanelSubsidy_Controller : MonoBehaviour {

	[SerializeField] Text txtGetGold_Small;
	[SerializeField] Text txtGetGold_Large;
	[SerializeField] Text txtCountDown;
	[SerializeField] ShakeController countDownShakeController;
	[SerializeField] Transform panelShadow;

	System.DateTime timeCanPressGetSubsidy;
	IEnumerator actionCountDown, actionCheckViewAds;
	LTDescr tweenScaleTxtCountDown, tweenColorTxtCountDown;
	LTDescr tweenHighlight;

	public void ResetData(){
		countDownShakeController.ResetData();
		if(actionCountDown != null){
			StopCoroutine(actionCountDown);
			actionCountDown = null;
		}
		if(actionCheckViewAds != null){
			StopCoroutine(actionCheckViewAds);
			actionCheckViewAds = null;
		}

		LeanTween.cancel(txtGetGold_Small.gameObject);
		LeanTween.cancel(txtGetGold_Large.gameObject);
		LeanTween.cancel(txtCountDown.gameObject);

		tweenScaleTxtCountDown = null;
		tweenColorTxtCountDown = null;
		tweenHighlight = null;

		txtCountDown.rectTransform.rotation = Quaternion.identity;
		txtCountDown.rectTransform.localScale = Vector3.one;
		txtCountDown.color = Color.black;
	}

	public void InitData(){
		ResetData();
		RefreshData();

		timeCanPressGetSubsidy = System.DateTime.Now;
	}

	void RefreshData(){
		if(actionCountDown != null){
			StopCoroutine(actionCountDown);
			actionCountDown = null;
		}
		if(tweenHighlight != null){
			LeanTween.cancel(tweenHighlight.uniqueId);
			tweenHighlight = null;
		}

		if(DataManager.instance.subsidyData.CanRecieveReward()){
			Color _c = Color.white;
			_c = txtGetGold_Small.color;
			_c.a = 0f;
			txtGetGold_Small.color = _c;

			_c = txtCountDown.color;
			_c.a = 0f;
			txtCountDown.color = _c;

			panelShadow.gameObject.SetActive(false);

			txtGetGold_Large.color = Color.black;
			tweenHighlight = LeanTween.colorText(txtGetGold_Large.rectTransform, Color.red, 0.3f).setEase(LeanTweenType.easeInOutCirc).setLoopPingPong(-1);
		}else{
			Color _c = txtGetGold_Small.color;
			_c.a = 1f;
			txtGetGold_Small.color = _c;

			_c = txtCountDown.color;
			_c.a = 1f;
			txtCountDown.color = _c;

			_c = txtGetGold_Large.color;
			_c.a = 0f;
			txtGetGold_Large.color = _c;

			panelShadow.gameObject.SetActive(true);

			System.TimeSpan _tmpDeltaTime = DataManager.instance.subsidyData.timeToGetReward - System.DateTime.Now;
			txtCountDown.text = string.Format("{0:00}:{1:00}", _tmpDeltaTime.Minutes + ((_tmpDeltaTime.Hours + (_tmpDeltaTime.Days * 24)) * 60), _tmpDeltaTime.Seconds);

			if(actionCountDown != null){
				StopCoroutine(actionCountDown);
				actionCountDown = null;
			}
			actionCountDown = DoActionCountDown();
			StartCoroutine(actionCountDown);
		}
	}

	IEnumerator DoActionCountDown(){
		System.TimeSpan _tmpTime;
		while(DataManager.instance.subsidyData.timeToGetReward > System.DateTime.Now){
			yield return Yielders.Get(1f);
			_tmpTime = DataManager.instance.subsidyData.timeToGetReward - System.DateTime.Now;
			txtCountDown.text = string.Format("{0:00}:{1:00}", _tmpTime.Minutes + ((_tmpTime.Hours + (_tmpTime.Days * 24)) * 60), _tmpTime.Seconds);
		}
		actionCountDown = null;
		RefreshData();
	}
	
	public void OnButtonGetSubsidy(){
		if(!GetGoldScreenController.instance.canTouch){
			return;
		}

		if(timeCanPressGetSubsidy > System.DateTime.Now){
			return;
		}
		timeCanPressGetSubsidy = System.DateTime.Now.AddSeconds(1f);

		if(DataManager.instance.subsidyData.hadCheckSv && !DataManager.instance.subsidyData.CanRecieveReward()){
			ShowEffCanNotGetReward();
			return;
		}

		if(DataManager.instance.subsidyData.countGetSubsidy < 5){
			SendMessageGetSubsidy();
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
						SendMessageGetSubsidy();
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
						SendMessageGetSubsidy();
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

		SendMessageGetSubsidy();
		actionCheckViewAds = null;
	}

	void SendMessageGetSubsidy(){
		SubServerDetail _serverDetail = GetGoldScreenController.instance.GetServerDetail();
		LoadingCanvasController.instance.Show(3, true);
		OneHitAPI.GetGoldSubsidy (_serverDetail, (_messageReceiving, _error) => {
			LoadingCanvasController.instance.Hide();
			if(_messageReceiving != null){
				DataManager.instance.subsidyData.RecieveReward(_messageReceiving, (_caseCheck)=>{
					if(_caseCheck == -2){
						DataManager.instance.subsidyData.hadCheckSv = true;
						ShowEffCanNotGetReward();
						RefreshData();
					}else if(_caseCheck == 1 || _caseCheck == 2){
						DataManager.instance.subsidyData.hadCheckSv = true;
						GetGoldScreenController.instance.RefreshMyGoldInfo(false);
						RefreshData();
						if(HomeManager.instance != null && HomeManager.instance.myCallbackManager != null
							&& HomeManager.instance.myCallbackManager.onLoadDataGoldGemFinished != null){
							HomeManager.instance.myCallbackManager.onLoadDataGoldGemFinished();
						}
					}
				});
			}else{
				#if TEST
				Debug.LogError("GetGoldSubsidy Error: " + _error);
				#endif
			}
		});
	}

	void ShowEffCanNotGetReward(){
		countDownShakeController.SetUpShakeWorldPoint(0.2f);
		tweenScaleTxtCountDown = LeanTween.scale(txtCountDown.gameObject, Vector3.one * 1.2f, 0.2f).setLoopPingPong(1).setEase(LeanTweenType.easeOutBack).setOnComplete(()=>{
			tweenScaleTxtCountDown = null;
		});
		tweenColorTxtCountDown = LeanTween.colorText(txtCountDown.rectTransform, Color.red, 0.2f).setLoopPingPong(1).setEase(LeanTweenType.easeOutBack).setOnComplete(()=>{
			tweenColorTxtCountDown = null;
		});
		CoreGameManager.instance.DoVibrate();
		
		MyAudioManager.instance.PlaySfx(GetGoldScreenController.instance.sfx_NotThisTime);
	}
}
