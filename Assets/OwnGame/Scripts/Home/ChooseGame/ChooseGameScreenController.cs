using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseGameScreenController : UIHomeScreenController {

	public override UIType myType{
		get{ 
			return UIType.ChooseGame;
		}
	}

	[SerializeField] Transform mainContainer;
	[SerializeField] ScrollRect listGameScrollRect;
    [SerializeField] RectTransform topbarPanel;
    [SerializeField] RectTransform bottomBarPanel;
    [SerializeField] ChooseGame_PanelListGame_Controller panelListGame;
	[SerializeField] BottomBar_PanelUserInfo_Controller panelUserInfo;
	[SerializeField] MyArrowFocusController arrowFocusBtnGetGold;
	[SerializeField] ParticleSystem iconWarningUpdate;
	[SerializeField] ParticleSystem iconWarningConfigInfo;

    [Header("Setting")]
    [SerializeField] float timeTweenTopAndBottomBar;

	bool firstInit;

    #region Init / Show / Hide
    public override void InitData()
    {
        if (!HomeManager.hasShowTopAndBottomBar)
        {
            Vector2 _pos = topbarPanel.offsetMax;
            _pos.y = 120f;
            topbarPanel.offsetMax = _pos;

			_pos = topbarPanel.offsetMin;
            _pos.y = 0f;
            topbarPanel.offsetMin = _pos;

            _pos = bottomBarPanel.offsetMin;
            _pos.y = -120f;
            bottomBarPanel.offsetMin = _pos;

			_pos = bottomBarPanel.offsetMax;
            _pos.y = 0f;
            bottomBarPanel.offsetMax = _pos;
        }
		panelUserInfo.InitData();
        panelListGame.InitData();
		iconWarningUpdate.gameObject.SetActive(false);
		iconWarningConfigInfo.gameObject.SetActive(false);
		arrowFocusBtnGetGold.Hide();

		HomeManager.instance.myCallbackManager.onLoadDataGoldGemFinished += RefreshPanelMyCashInfo;
		HomeManager.instance.myCallbackManager.onLoadEmailInfoFinished += RefreshNotification;

		onPressBack = () => {
			PopupManager.Instance.CreatePopupDialog(MyLocalize.GetString(MyLocalize.kWarning)
				, MyLocalize.GetString(MyLocalize.kAskForQuit)
				, string.Empty
				, MyLocalize.GetString(MyLocalize.kYes)
				, MyLocalize.GetString(MyLocalize.kNo)
				, () =>{
					Application.Quit();
				}, null);
		};
		CoreGameManager.instance.RegisterNewCallbackPressBackKey (onPressBack);
	}

	public void RefreshPanelMyCashInfo(){
		panelUserInfo.RefreshPanelMyCashInfo();
		RefreshNotification();
	}

    public override void RefreshData ()
	{
		panelUserInfo.RefreshData ();
		RefreshNotification();
	}

	public override void Show ()
	{
		if(!firstInit){
			StartCoroutine(DoShowLate());
			firstInit = true;
		}else{
			DoShow();
		}
	}

	IEnumerator DoShowLate(){
		mainContainer.gameObject.SetActive(false);
		yield return Yielders.EndOfFrame;
		mainContainer.gameObject.SetActive(true);
		listGameScrollRect.horizontalNormalizedPosition = 0f;
		DoShow();
	}

	void DoShow(){
		base.Show ();
		if (!HomeManager.hasShowTopAndBottomBar) {
			HomeManager.hasShowTopAndBottomBar = true;
            LeanTween.moveY(topbarPanel, 0f, timeTweenTopAndBottomBar).setEase(LeanTweenType.easeOutSine).setOnComplete(()=>{
				SetupAfterShow();
			});
            LeanTween.moveY(bottomBarPanel, 5f, timeTweenTopAndBottomBar).setEase(LeanTweenType.easeOutSine);
        }else{
			SetupAfterShow();
		}
        panelUserInfo.Show ();
	}

	void SetupAfterShow(){
		RefreshData();
		if(HomeManager.showAnnouncement){
			HomeManager.showAnnouncement = false;
			HomeManager.instance.ShowAnnouncement();
		}
		if(HomeManager.getGoldAndGemInfoAgain){
			HomeManager.getGoldAndGemInfoAgain = false;
			HomeManager.instance.LoadDataGoldGemFromServer();
		}
		if(HomeManager.getEmailInfoAgain){
			HomeManager.getEmailInfoAgain = false;
			HomeManager.instance.LoadEmailInfoFromServer();
		}
	}

	public override void Hide ()
	{
		if (onPressBack != null) {
			CoreGameManager.instance.RemoveCurrentCallbackPressBackKey (onPressBack);
		}
		if(HomeManager.instance.myCallbackManager.onLoadDataGoldGemFinished != null){
			HomeManager.instance.myCallbackManager.onLoadDataGoldGemFinished -= RefreshPanelMyCashInfo;
		}
		if(HomeManager.instance.myCallbackManager.onLoadEmailInfoFinished != null){
			HomeManager.instance.myCallbackManager.onLoadEmailInfoFinished -= RefreshNotification;
		}

		arrowFocusBtnGetGold.Hide();

		panelUserInfo.Hide ();
		
		base.Hide ();
	}
	#endregion

	void RefreshNotification(){
		if(DataManager.instance.haveNewVersion){
			if(!iconWarningUpdate.gameObject.activeSelf){
				iconWarningUpdate.gameObject.SetActive(true);
				iconWarningUpdate.Play();
			}
		}else{
			if(iconWarningUpdate.gameObject.activeSelf){
				iconWarningUpdate.gameObject.SetActive(false);
			}
		}

		if(DataManager.instance.userData.databaseId == UserData.DatabaseType.DATABASEID_BIGXU
			&& string.IsNullOrEmpty(DataManager.instance.userData.emailShow)){
			iconWarningConfigInfo.gameObject.SetActive(true);
			iconWarningConfigInfo.Play();
		}else{
			iconWarningConfigInfo.gameObject.SetActive(false);
		}

		if(DataManager.instance.userData.gold < 2000){
			arrowFocusBtnGetGold.Show();
		}else{
			bool _flag = false;
			for(int i = 0; i < DataManager.instance.purchaseReceiptData.listPurchaseReceiptDetail.Count; i++){
				if(!DataManager.instance.purchaseReceiptData.listPurchaseReceiptDetail[i].isDone){
					_flag = true;
					break;
				}
			}
			if(_flag){
				arrowFocusBtnGetGold.Show();
			}else{
				arrowFocusBtnGetGold.Hide();
			}
		}
	}

	#region On Button Clicked
	public void OnButtonGetGoldClicked(){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
 		HomeManager.instance.ChangeScreen (UIType.GetGold);
	}

	public void OnButtonLeaderBoardClicked(){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
		HomeManager.instance.ChangeScreen (UIType.Leaderboard);
    }

	public void OnButtonLuckyWheelClicked(){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
		HomeManager.instance.ChangeScreen (UIType.LuckyWheel);
    }

	public void OnButtonMiniGamesClicked(){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
		HomeManager.instance.ChangeScreen (UIType.SubGame);
    }

	public void OnButtonSettingClicked(){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
		HomeManager.instance.ChangeScreen (UIType.SettingScreen);
	}
	#endregion
}
