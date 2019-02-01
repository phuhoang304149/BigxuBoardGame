using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Facebook.Unity;
public class InstallAppAndroid : MonoBehaviour {
// 	public Button missionInstallApp;
// 	public Button panelInstallApp;
// 	short numberCampagne;
// 	public GameObject objectPreference;
// 	public Transform contentPanelListApp;
// 	List<InfoApplication> listAppReceive = new List<InfoApplication>();
// 	List<Button> listBtnAppReceive = new List<Button>();
// 	[SerializeField]
// 	public string abc = "";
// 	void Start() {
// 		missionInstallApp.onClick.AddListener(() => {
// 			NetworkGlobal.instance.RunOnehit(SendInstallAppAndroid, ReceiveInstallAppAndroid, Static.ipConnect, Static.port, null);// cài đặt app
// 		});
// 		panelInstallApp.onClick.AddListener(() => {
// 			StopCoundownApp();
// 			SceneManager.UnloadSceneAsync((int)Scene.ChoiceScene.Rewards);
// 		});
// 	}
// 	void CreateListApplication() {
// 		for (int i = 0; i < numberCampagne; i++) {
// 			GameObject newObject = Instantiate(objectPreference) as GameObject;
// 			newObject.SetActive(true);
// 			InfoApplication buttonScr = newObject.GetComponent<InfoApplication>();
// 			buttonScr.ids = listAppReceive[i].ids;
// 			buttonScr.package_names = listAppReceive[i].package_names;
// 			buttonScr.key_searchs = listAppReceive[i].key_searchs;
// 			buttonScr.count_installs = listAppReceive[i].count_installs;
// 			buttonScr.sum_installs = listAppReceive[i].sum_installs;
// 			buttonScr.money_pay_to_users = listAppReceive[i].money_pay_to_users;
// 			buttonScr.is_ratings = listAppReceive[i].is_ratings;
// 			buttonScr.text_titles = listAppReceive[i].text_titles;
// 			buttonScr.text_contents = listAppReceive[i].text_contents;
// 			buttonScr.link_icons = listAppReceive[i].link_icons;
// 			buttonScr.time_keeps = listAppReceive[i].time_keeps;
// 			buttonScr.link_reports = listAppReceive[i].link_reports;

// 			// show Info app
// 			StartCoroutine(DownloadIcon(buttonScr.link_icons, buttonScr.icon));
// 			buttonScr.goldReward.text = buttonScr.money_pay_to_users.ToString();
// 			buttonScr.content.text = listAppReceive[i].text_contents;
// 			buttonScr.title.text = listAppReceive[i].text_titles;
// 			newObject.transform.SetParent(contentPanelListApp, false);
// 			listBtnAppReceive.Add(newObject.GetComponent<Button>());
// 			// check install app and comit app
// 			if (isInstallAppAndroid(buttonScr.package_names)) {
// 				ComitApp(buttonScr.package_names, buttonScr.time_keeps);
// 			}
// 			abc += "\n" + buttonScr.package_names + " casecheck " + PlayerPrefs.GetString("casecheck" + buttonScr.package_names, "not install or time out ");
// 		}

// 		CreateToast.instance.ShowPopup("alerts", abc, true, false, null);
// 		CountDownAppInstall(listBtnAppReceive);
// 	}
// 	void AddClickInButton() {
// 		Debug.Log(listBtnAppReceive.Count);
// 		foreach (Button button in listBtnAppReceive) {
// 			button.onClick.AddListener(() => {
// 				int position = listBtnAppReceive.IndexOf(button);
// 			});
// 		}
// 	}
// 	void ComitApp(string url, long timekeep) {
// 		NetworkGlobal.instance.RunOnehit(
// 			() => {
// 				MessageSending messageSending = new MessageSending(CMD_ONEHIT.Game_Forward_Bonus_AndroidInstall_Commit);
// 				messageSending.writeByte(Convert.ToByte(DataManager.instance.userData.databaseid));
// 				messageSending.writeLong(Convert.ToInt64(DataManager.instance.userData.userid));
// 				messageSending.writeString(url);
// 				return messageSending;
// 			},
// 			(MessageReceiving messageReceiving) => {
// 				sbyte caseCheck = messageReceiving.readByte();
// 				switch (caseCheck) {
// 					case 1:
// 						int goldAdd = messageReceiving.readInt();
// 						long goldResult = messageReceiving.readLong();
// 						PlayerPrefs.SetString("fisrtSetup" + url, NetworkGlobal.instance.currentTimeMillis.ToString());
// 						PlayerPrefs.SetString("casecheck" + url, caseCheck.ToString());
// 						break;
// 					case -1:
// 						Debug.Log("nhiêm vụ không còn tồn tại ");
// 						PlayerPrefs.DeleteKey("casecheck" + url);
// 						break;
// 					case -2:
// 						Debug.Log("lỗi database của sever " + caseCheck);
// 						PlayerPrefs.SetString("casecheck" + url, caseCheck.ToString());
// 						break;
// 					case -3:
// 						Debug.Log("đã cài rồi " + caseCheck);
// 						PlayerPrefs.SetString("casecheck" + url, caseCheck.ToString());
// 						break;
// 					case -4:
// 						Debug.Log("nhiêm vụ hết hạn " + caseCheck);
// 						PlayerPrefs.DeleteKey("casecheck" + url);
// 						break;
// 					case -5:
// 						Debug.Log("không lấy đươc thông tin user(chưa đăng ký) " + caseCheck);
// 						PlayerPrefs.SetString("casecheck" + url, caseCheck.ToString());
// 						break;
// 				}
// 			}, Static.ipConnect, Static.port, null
// 		);
// 	}

// 	public MessageSending SendInstallAppAndroid() {
// 		MessageSending messageSending = new MessageSending(CMD_ONEHIT.Game_GetList_CampagneInstallAndroid);
// 		return messageSending;
// 	}
// 	public void ReceiveInstallAppAndroid(MessageReceiving messageReceiving) {
// 		numberCampagne = messageReceiving.readShort();
// 		for (int i = 0; i < numberCampagne; i++) {
// 			int id = messageReceiving.readInt();
// 			string package_name = messageReceiving.readString();
// 			string key_search = messageReceiving.readString();
// 			int count_install = messageReceiving.readInt();
// 			int sum_install = messageReceiving.readInt();
// 			int money_pay_to_user = messageReceiving.readInt();
// 			bool is_rating = messageReceiving.readBoolean();
// 			string text_title = messageReceiving.readString();// tên app
// 			string text_content = messageReceiving.readString();// chua thuc hien hay da thuc hien
// 			string link_icon = messageReceiving.readString();// link icon
// 			long time_keep = messageReceiving.readLong();
// 			string link_report = messageReceiving.readString();

// 			listAppReceive.Add(new InfoApplication() {
// 				ids = id,
// 				package_names = package_name, key_searchs = key_search, count_installs = count_install,
// 				sum_installs = sum_install, money_pay_to_users = money_pay_to_user, is_ratings = is_rating,
// 				text_titles = text_title, text_contents = text_content, link_icons = link_icon, time_keeps = time_keep,
// 				link_reports = link_report
// 			});
// 		}
// 		CreateListApplication();
// 		AddClickInButton();
// 	}
// 	public bool isInstallAppAndroid(string bundleID) {
// 		AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
// 		AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
// 		AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");
// 		AndroidJavaObject launchIntent = null;
// 		try {
// 			launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", bundleID);
// 		} catch (Exception ex) {
// 			Debug.Log("exception" + ex.Message);
// 		}
// 		if (launchIntent == null)
// 			return false;
// 		return true;
// 	}
// 	IEnumerator DownloadIcon(string url, Image imageToDisplay) {
// 		// Start a download of the given URL
// 		var www = new WWW(url);
// 		// wait until the download is done
// 		yield return www;
// 		// Create a texture in DXT1 format
// 		Texture2D texture = new Texture2D(www.texture.width, www.texture.height, TextureFormat.DXT1, false);

// 		// assign the downloaded image to sprite
// 		www.LoadImageIntoTexture(texture);
// 		Rect rec = new Rect(0, 0, texture.width, texture.height);
// 		Sprite spriteToUse = Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 100);
// 		imageToDisplay.sprite = spriteToUse;
// 		www.Dispose();
// 		www = null;
// 	}
// 	IEnumerator CountDown(List<Button> listBtnAppReceives) {
// 		yield return null;
// 		while (true) {
// 			long currentTime = NetworkGlobal.instance.currentTimeMillis;
// 			for (int i = 0; i < listBtnAppReceives.Count; i++) {
// 				string url = listBtnAppReceives[i].GetComponent<InfoApplication>().package_names;
// 				int casecheck = Convert.ToInt32(PlayerPrefs.GetString("casecheck" + url, "0"));
// 				long timekeepApp = listBtnAppReceives[i].GetComponent<InfoApplication>().time_keeps;
// 				if (isInstallAppAndroid(url)) {
// 					if (casecheck == 1) {
// 						long timeFirstSetup = Convert.ToInt64(PlayerPrefs.GetString("fisrtSetup" + url));
// 						long timeKeepAppInDevice = currentTime - timeFirstSetup;
// 						long timedelay = timekeepApp - timeKeepAppInDevice;
// 						ShowTime(listBtnAppReceives[i].GetComponent<InfoApplication>().showTimes, timedelay);
// 					} else if (casecheck == -3) {
// 						long timeFirstSetup = Convert.ToInt64(PlayerPrefs.GetString("fisrtSetup" + url, "0"));
// 						if (timeFirstSetup == 0) {// dell game cài lại
// 							PlayerPrefs.SetString("fisrtSetup" + url, NetworkGlobal.instance.currentTimeMillis.ToString());
// 						} else {// app vẫn còn trong máy
// 							long timeKeepAppInDevice = currentTime - timeFirstSetup;
// 							long timedelay = timekeepApp - timeKeepAppInDevice;
// 							if(timedelay>1000){
// 								ShowTime(listBtnAppReceives[i].GetComponent<InfoApplication>().showTimes, timedelay);
// 							}else{
// 								PlayerPrefs.SetString("fisrtSetup" + url, NetworkGlobal.instance.currentTimeMillis.ToString());

// 							}
// 						}
// 					}
// 				}
// 			}
// 			yield return Yielders.Get(1);
// 			//while (true) {
// 			//	long currentTime = NetworkGlobal.instance.currentTimeMillis;
// 			//	for (int i = 0; i < listBtnAppReceives.Count; i++) {
// 			//		if (isInstallAppAndroid(listBtnAppReceives[i].GetComponent<InfoApplication>().package_names)) {
// 			//			long timeKeepInDevice = currentTime - Convert.ToInt64(PlayerPrefs.GetString("fisrtSetup" + listBtnAppReceives[i].GetComponent<InfoApplication>().package_names));
// 			//			long timedelay = listBtnAppReceives[i].GetComponent<InfoApplication>().time_keeps  - timeKeepInDevice;
// 			//			if (timedelay > 1000) {
// 			//				TimeSpan t = TimeSpan.FromMilliseconds(timedelay);
// 			//				string answer = string.Format("{0:D2}h:{1:D2}m:{2:D2}s", t.Hours, t.Minutes, t.Seconds);
// 			//				listBtnAppReceives[i].GetComponent<InfoApplication>().showTimes.text = answer;
// 			//				listBtnAppReceives[i].GetComponent<InfoApplication>().showTimes.color = Color.black;
// 			//			}
// 			//		}else{
// 			//			long timeFistSetup = Convert.ToInt64(PlayerPrefs.GetString("fisrtSetup" + listBtnAppReceives[i].GetComponent<InfoApplication>().package_names,"0"));
// 			//			if(timeFistSetup!=0){
// 			//				PlayerPrefs.SetString("fisrtSetup" + listBtnAppReceives[i].GetComponent<InfoApplication>().package_names, NetworkGlobal.instance.currentTimeMillis.ToString());
// 			//			}
// 			//			listBtnAppReceives[i].GetComponent<InfoApplication>().showTimes.text = "install++";
// 			//          listBtnAppReceives[i].GetComponent<InfoApplication>().showTimes.color = Color.red;
// 			//		}
// 			//	}
// 			//	yield return Yielders.Get(1);
// 			//}
// 			//while(timerelease > 1000) {
// 			//	yield return Yielders.Get(1);
// 			//	timerelease -= 1000;
// 			//	TimeSpan t = TimeSpan.FromMilliseconds(timerelease);
// 			//	string answer = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
// 			//				t.Hours,
// 			//				t.Minutes,
// 			//				t.Seconds
// 			//				);
// 			//	timecountdown.text = answer;
// 			//}
// 		}
// 	}
// 	void ShowTime(Text textShow,long timeShow){
// 		TimeSpan t = TimeSpan.FromMilliseconds(timeShow);
//         string answer = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",t.Hours,t.Minutes,t.Seconds);
// 		textShow.text = answer;
// 	}
// 	IEnumerator mCountDown;
// 	void CountDownAppInstall(List<Button> listBtn) {
// 		if (mCountDown == null) {
// 			mCountDown = CountDown(listBtn);
// 			StartCoroutine(mCountDown);
// 		} else {
// 			StopCoroutine(mCountDown);
// 			mCountDown = null;
// 		}
// 	}
// 	void StopCoundownApp() {
// 		if (mCountDown != null) {
// 			StopCoroutine(mCountDown);
// 			mCountDown = null;
// 		}
// 	}

// 	public void CheckAppInstalledIphone(string urlScheme, long id){
//     	StartCoroutine(OpenApp(urlScheme, id));
//     }
//     void OnApplicationPause() {
//         leftApp = true;
//     }
//     IEnumerator OpenApp(string url, long idapp) {
//         Application.OpenURL(url);
//         yield return new WaitForSeconds(1);
//         if (leftApp) {
//             leftApp = false;
//             CreateToast.instance.ShowPopup("alert", "da cai facebook", true, true, null);
//         } else {
//             Application.OpenURL("https://itunes.apple.com/app/apple-store/id" + idapp);
//         }
//     }
}
