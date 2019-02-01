using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EmojiUI;
using Lean.Pool;

public class GetGoldScreen_PanelInstallApp_OptionInfo_Controller : MySimplePoolObjectController {

	[SerializeField] RawImage imgIconGame;
	[SerializeField] Text txtInfo_Title;
	[SerializeField] Text txtInfo_Description;
	[SerializeField] EmojiText txtInfo_KeyWork;
	[SerializeField] Text txtReward_Quantity;
	[SerializeField] Text txtReward_CountDown;
	[SerializeField] Image panelFocus;
	[SerializeField] Text txtEffCountDown;
	[SerializeField] ShakeController countDownShakeController;

	GetGoldScreen_PanelInstallApp_Controller panelInstallApp;
	InstallAppDetail appDetail;
	IEnumerator actionCountDown, actionShowTxtEff;

	bool checkAppInstallOrNot;

	public override void ResetData(){
		StopAllCoroutines();
		CancelInvoke();
		actionCountDown = null;
		actionShowTxtEff = null;
		LeanTween.cancel(imgIconGame.rectTransform);
		LeanTween.cancel(panelFocus.rectTransform);
       	
		appDetail = null;
		txtInfo_Title.text = string.Empty;
		txtInfo_Description.text = string.Empty;
		txtReward_CountDown.text = string.Empty;
		txtInfo_KeyWork.text = string.Empty;
		txtReward_Quantity.text = string.Empty;
		txtEffCountDown.text = string.Empty;

		txtReward_CountDown.rectTransform.rotation = Quaternion.identity;
		txtReward_CountDown.rectTransform.localScale = Vector3.one;
		txtReward_CountDown.color = Color.black;

		Color _c = imgIconGame.color;
		_c.a = 0f;
		imgIconGame.color = _c;

		_c = panelFocus.color;
		_c.a = 0f;
		panelFocus.color = _c;
		
		checkAppInstallOrNot = false;
    }

	void ScrollCellIndex (int _index) 
    {	
		if(panelInstallApp == null){
			panelInstallApp = (GetGoldScreen_PanelInstallApp_Controller) GetGoldScreenController.instance.panelInstallApp;
		}
		if(panelInstallApp == null){
			Debug.LogError("panelInstallApp is NULL");
			return;
		}
		// string _name = "PanelInstallApp_OptionInfo_" + _index.ToString ();
		// gameObject.name = name;
		InitData(panelInstallApp.listCurrentAppDetail[_index]);
	}

	public void InitData(InstallAppDetail _appDetail){
		appDetail = _appDetail;
		txtInfo_Title.text = appDetail.textTitle;
		txtInfo_Description.text = appDetail.textDescription;
		txtInfo_Description.text = appDetail.textDescription.Replace("\\n", "\n");
		if(!string.IsNullOrEmpty(appDetail.textkeySearch)){
			txtInfo_KeyWork.text = "🔎" + appDetail.textkeySearch;
		}else{
			txtInfo_KeyWork.text = string.Empty;
		}
		txtReward_Quantity.text = appDetail.reward.quantity.ToString();

		if(appDetail.myIcon == null){
			StartCoroutine(MyConstant.DownloadIcon(appDetail.linkIcon ,(_texture)=>{
				if(_texture != null){
					appDetail.myIcon = _texture;
				}

				imgIconGame.texture = appDetail.myIcon;
				LeanTween.alpha(imgIconGame.rectTransform, 1f, 0.2f);
				if(appDetail.myOriginalDetail != null){
					if(_texture != null){
						appDetail.myOriginalDetail.myIcon = _texture;
					}
				}
			}));
		}else{
			imgIconGame.texture = appDetail.myIcon;
			Color _c = imgIconGame.color;
			_c.a = 1f;
			imgIconGame.color = _c;
		}
		txtEffCountDown.text = string.Empty;
		RefreshData();
	}

	public void RefreshData(){
		if(appDetail.currentState == InstallAppDetail.State.None){
			DateTime _dt = DateTime.Now.AddMilliseconds(appDetail.timeKeep);
			TimeSpan _timeCountDown = _dt - DateTime.Now;
			txtReward_CountDown.text = string.Format("{0:00}:{1:00}:{2:00}", _timeCountDown.Hours + (_timeCountDown.Days * 24), _timeCountDown.Minutes, _timeCountDown.Seconds);
		}else if(appDetail.currentState == InstallAppDetail.State.Checking){
			if(DateTime.Now >= appDetail.timeToGetReward){
				txtEffCountDown.text = string.Empty;
				LeanTween.alpha(panelFocus.rectTransform, 0.4f, 0.2f);
				txtReward_CountDown.text = "00:00:00";
			}else{
				Color _c = panelFocus.color;
				_c.a = 0f;
				panelFocus.color = _c;

				TimeSpan _timeCountDown = appDetail.timeToGetReward - DateTime.Now;
				txtReward_CountDown.text = string.Format("{0:00}:{1:00}:{2:00}", _timeCountDown.Hours + (_timeCountDown.Days * 24), _timeCountDown.Minutes, _timeCountDown.Seconds);
				if(actionCountDown != null){
					StopCoroutine(actionCountDown);
					actionCountDown = null;
				}
				actionCountDown = DoActionCountDown();
				StartCoroutine(actionCountDown);

				if(actionShowTxtEff != null){
					StopCoroutine(actionShowTxtEff);
					actionShowTxtEff = null;
				}
				actionShowTxtEff = DoActionShowTextEff();
				StartCoroutine(actionShowTxtEff);
			}
		}
	}
	
	IEnumerator DoActionCountDown(){
		System.TimeSpan _tmpTime;
		while(DateTime.Now < appDetail.timeToGetReward){
			yield return Yielders.Get(1f);
			_tmpTime = appDetail.timeToGetReward.Subtract(System.DateTime.Now);
			txtReward_CountDown.text = string.Format("{0:00}:{1:00}:{2:00}", _tmpTime.Hours + (_tmpTime.Days * 24), _tmpTime.Minutes, _tmpTime.Seconds);
		}
		actionCountDown = null;
		RefreshData();
	}

	IEnumerator DoActionShowTextEff(){
		int _tmpCount = 3;
		string _tmpTextEffContent = string.Empty;
		while(DateTime.Now < appDetail.timeToGetReward){
			_tmpTextEffContent = string.Empty;
			for(int i = 0; i < _tmpCount; i ++){
				_tmpTextEffContent += ">";
			}
			txtEffCountDown.text = _tmpTextEffContent;
			yield return Yielders.Get(0.2f);
			_tmpCount --;
			if(_tmpCount < 0){
				_tmpCount = 3;
			}
		}
		actionShowTxtEff = null;
	}
	
	public void OnClicked(){
		if(!GetGoldScreenController.instance.canTouch){
			return;
		}
		
		if(((GetGoldScreen_PanelInstallApp_Controller) GetGoldScreenController.instance.panelInstallApp).timeCanPressGetReward > System.DateTime.Now){
			return;
		}

        if(appDetail == null){
            return;
        }

		((GetGoldScreen_PanelInstallApp_Controller) GetGoldScreenController.instance.panelInstallApp).timeCanPressGetReward = System.DateTime.Now.AddSeconds(0.5f);

		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);

		if(appDetail.currentState == InstallAppDetail.State.None){
			if(!MyConstant.IsAppInstalled(appDetail.packageName)){
				string _url = string.Empty;
				#if UNITY_ANDROID
					_url = "market://details?id=" + appDetail.packageName;
				#elif UNITY_IOS
					//TODO: setup URL cho mở app trên IOS
					_url = "itms-apps://itunes.apple.com/app/id" + appDetail.packageName;
				#endif
				if(!string.IsNullOrEmpty(_url)){
					Application.OpenURL(_url);
					checkAppInstallOrNot = true;
				}
			}else{
				appDetail.StartChecking();
				if(appDetail.myOriginalDetail != null){
					appDetail.myOriginalDetail.StartChecking();
				}
				RefreshData();
			}
		}else if(appDetail.currentState == InstallAppDetail.State.Checking){
			
			if(!appDetail.CanRecieveReward()){
				countDownShakeController.SetUpShakeWorldPoint(0.2f);
				LeanTween.scale(txtReward_CountDown.gameObject, Vector3.one * 1.1f, 0.2f).setLoopPingPong(1).setEase(LeanTweenType.easeOutBack);
				LeanTween.colorText(txtReward_CountDown.rectTransform, Color.red, 0.2f).setLoopPingPong(1).setEase(LeanTweenType.easeOutBack);
				CoreGameManager.instance.DoVibrate();

				MyAudioManager.instance.PlaySfx(GetGoldScreenController.instance.sfx_NotThisTime);
				return;
			}
			appDetail.RecieveReward(()=>{
				GetGoldScreenController.instance.RefreshMyGoldInfo(false);
				if(HomeManager.instance != null && HomeManager.instance.myCallbackManager != null
					&& HomeManager.instance.myCallbackManager.onLoadDataGoldGemFinished != null){
					HomeManager.instance.myCallbackManager.onLoadDataGoldGemFinished();
				}
			}, ()=>{
				if(panelInstallApp != null){
					panelInstallApp.ClearAndCreateNewPanelAppInfo(appDetail);
				}
			});
		}
    }

	void OnApplicationFocus(bool _hasFocus)
    {
		if(_hasFocus){
			if(checkAppInstallOrNot){
				if(appDetail.currentState == InstallAppDetail.State.None){
					if(MyConstant.IsAppInstalled(appDetail.packageName)){
						appDetail.StartChecking();
						if(appDetail.myOriginalDetail != null){
							appDetail.myOriginalDetail.StartChecking();
						}
					}
				}
				checkAppInstallOrNot = false;
			}
			RefreshData();
		}
    }
}
