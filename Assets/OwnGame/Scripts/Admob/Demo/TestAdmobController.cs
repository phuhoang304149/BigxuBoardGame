using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TestAdmobController : MonoBehaviour {

	public Button btnBanner;
	public Button btnInterstitial;
	public Button btnRewardsVideo;
	void Start() {
		OnclickInScene();
	}
	public void OnclickInScene() {
		btnBanner.onClick.AddListener(() => {
			AdmobController.instance.ShowBanner();
		});
		btnInterstitial.onClick.AddListener(() => {
			AdmobController.instance.ShowInterstitial();
		});
		btnRewardsVideo.onClick.AddListener(() => {
			AdmobController.instance.ShowRewardBasedVideo();
		});
	}
}
