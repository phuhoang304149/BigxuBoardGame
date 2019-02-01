using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BOL_Survival_Screen : BOL_UI_Offline {
	public Button button;
	public override UIType mySceneType {
		get {
			return UIType.survival;
		}
	}

	public override void InitData() {

	}
	public void HomeClick() {
		if (BOL_MainControl_Offline.instance.currentScreen != null) {
			BOL_MainControl_Offline.instance.lateScreen
			= BOL_MainControl_Offline.instance.currentScreen;
			BOL_MainControl_Offline.instance.lateScreen.Hide();
			BOL_MainControl_Offline.instance.currentScreen = null;
		}
		BOL_MainControl_Offline.instance.currentScreen =
		BOL_MainControl_Offline.instance.GetScreen(UIType.home);
		BOL_MainControl_Offline.instance.currentScreen.Show();
		BOL_MainControl_Offline.instance.currentScreen.InitData();
	}
}
