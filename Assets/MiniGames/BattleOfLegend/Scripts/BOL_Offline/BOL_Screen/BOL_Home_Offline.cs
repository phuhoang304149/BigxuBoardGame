using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Lean.Pool;

public class BOL_Home_Offline : BOL_UI_Offline {
	[Header("button on home screen")]
	public Button button_campaign;
	public Button button_arcade;
	public Button button_survival;
	public Button button_timed_mode;
	public Button button_tutorial;
	public override UIType mySceneType {
		get {
			return UIType.home;
		}
	}

	public override void InitData() {
		LeanTween.moveLocalX(button_campaign.gameObject, 450, 0.2f).setDelay(0);
		LeanTween.moveLocalX(button_arcade.gameObject, 450, 0.2f).setDelay(0.1f);
		LeanTween.moveLocalX(button_survival.gameObject, 450, 0.2f).setDelay(0.2f);
		LeanTween.moveLocalX(button_timed_mode.gameObject, 450, 0.2f).setDelay(0.3f);
		LeanTween.moveLocalX(button_tutorial.gameObject, 450, 0.2f).setDelay(0.4f);
	}
	public void TutorialClick() {
		ActionShowSceneHome(() => {
			if (BOL_MainControl_Offline.instance.currentScreen != null) {
				BOL_MainControl_Offline.instance.lateScreen
				= BOL_MainControl_Offline.instance.currentScreen;
				BOL_MainControl_Offline.instance.lateScreen.Hide();
				BOL_MainControl_Offline.instance.currentScreen = null;
			}
			BOL_MainControl_Offline.instance.currentScreen =
				BOL_MainControl_Offline.instance.GetScreen(UIType.tutorial);
			BOL_MainControl_Offline.instance.currentScreen.Show();
			BOL_MainControl_Offline.instance.currentScreen.InitData();
		});

	}
	public void CampaignClick() {
		ActionShowSceneHome(() => {
			if (BOL_MainControl_Offline.instance.currentScreen != null) {
				BOL_MainControl_Offline.instance.lateScreen
				= BOL_MainControl_Offline.instance.currentScreen;
				BOL_MainControl_Offline.instance.lateScreen.Hide();
				BOL_MainControl_Offline.instance.currentScreen = null;
			}
			BOL_MainControl_Offline.instance.currentScreen =
				BOL_MainControl_Offline.instance.GetScreen(UIType.campaign);
			BOL_MainControl_Offline.instance.currentScreen.Show();
			BOL_MainControl_Offline.instance.currentScreen.InitData();
		});
	}
	public void SurvivalClick() {
		ActionShowSceneHome(() => {
			if (BOL_MainControl_Offline.instance.currentScreen != null) {
				BOL_MainControl_Offline.instance.lateScreen
				= BOL_MainControl_Offline.instance.currentScreen;
				BOL_MainControl_Offline.instance.lateScreen.Hide();
				BOL_MainControl_Offline.instance.currentScreen = null;
			}
			BOL_MainControl_Offline.instance.currentScreen =
				BOL_MainControl_Offline.instance.GetScreen(UIType.survival);
			BOL_MainControl_Offline.instance.currentScreen.Show();
			BOL_MainControl_Offline.instance.currentScreen.InitData();
		});
	}
	public void TimeModeClick() {
		ActionShowSceneHome(() => {
			if (BOL_MainControl_Offline.instance.currentScreen != null) {
				BOL_MainControl_Offline.instance.lateScreen
				= BOL_MainControl_Offline.instance.currentScreen;
				BOL_MainControl_Offline.instance.lateScreen.Hide();
				BOL_MainControl_Offline.instance.currentScreen = null;
			}
			BOL_MainControl_Offline.instance.currentScreen =
				BOL_MainControl_Offline.instance.GetScreen(UIType.timed_mode);
			BOL_MainControl_Offline.instance.currentScreen.Show();
			BOL_MainControl_Offline.instance.currentScreen.InitData();
		});
	}
	public void ArcadeClick() {
		ActionShowSceneHome(() => {
			if (BOL_MainControl_Offline.instance.currentScreen != null) {
				BOL_MainControl_Offline.instance.lateScreen
				= BOL_MainControl_Offline.instance.currentScreen;
				BOL_MainControl_Offline.instance.lateScreen.Hide();
				BOL_MainControl_Offline.instance.currentScreen = null;
			}
			BOL_MainControl_Offline.instance.currentScreen =
				BOL_MainControl_Offline.instance.GetScreen(UIType.arcade);
			BOL_MainControl_Offline.instance.currentScreen.Show();
			BOL_MainControl_Offline.instance.currentScreen.InitData();
		});
	}

	public void ActionShowSceneHome(Action action) {
		LeanTween.moveLocalX(button_campaign.gameObject, 760, 0.2f).setDelay(0);
		LeanTween.moveLocalX(button_arcade.gameObject, 760, 0.2f).setDelay(0.1f);
		LeanTween.moveLocalX(button_survival.gameObject, 760, 0.2f).setDelay(0.2f);
		LeanTween.moveLocalX(button_timed_mode.gameObject, 760, 0.2f).setDelay(0.3f);
		LeanTween.moveLocalX(button_tutorial.gameObject, 760, 0.2f).setDelay(0.4f).setOnComplete(action);
	}
}
