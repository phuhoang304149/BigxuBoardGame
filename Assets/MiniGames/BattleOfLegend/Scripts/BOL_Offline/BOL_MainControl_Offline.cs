using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
public class BOL_MainControl_Offline : MonoBehaviour {
	public static BOL_MainControl_Offline instance {
		get {
			return ins;
		}
	}
	static BOL_MainControl_Offline ins;
	public BOL_MainControl_Offline() { }
	public GameObject BattleScene;
	public GameObject PanelGame;
	public BOL_UI_Offline.UIType typeUI;
	public BOL_ListScreenInHome listScreens;
	public BOL_UI_Offline currentScreen { get; set; }
	public BOL_UI_Offline lateScreen { get; set; }
	public Transform mainScreeenHolder;
    
	private void Awake() {
		ins = this;
	}
	private void Start() {
		currentScreen = GetScreen(BOL_UI_Offline.UIType.home);
		StartCoroutine(DoAction());
	}
	IEnumerator DoAction() {
		yield return currentScreen = GetScreen(BOL_UI_Offline.UIType.home);
		currentScreen.InitData();
	}
	public BOL_UI_Offline GetScreen(BOL_UI_Offline.UIType _typeScreen) {
		switch (_typeScreen) {
			case BOL_UI_Offline.UIType.home:
				return listScreens.BOL_home_offline;
			case BOL_UI_Offline.UIType.arcade:
				return listScreens.BOL_arcade;
			case BOL_UI_Offline.UIType.campaign:
				return listScreens.BOL_campaign;
			case BOL_UI_Offline.UIType.survival:
				return listScreens.BOL_survival;
			case BOL_UI_Offline.UIType.timed_mode:
				return listScreens.BOL_timed_mode;
			case BOL_UI_Offline.UIType.tutorial:
				return listScreens.BOL_tutorial;
		}
		Debug.LogError("NULL Screen: " + _typeScreen.ToString());
		return null;
	}
	void OnDestroy() {
		ins = null;
	}
	public void SelfDestruction() {
		listScreens._BOL_home_offline = null;
		listScreens._BOL_campaign = null;
		listScreens._BOL_arcade = null;
		listScreens._BOL_survival = null;
		listScreens._BOL_timed_mode = null;
		listScreens._BOL_tutorial = null;
	}
	public void Back2LastScene() {
		if (instance.currentScreen != null) {
			instance.currentScreen.Hide();
			instance.currentScreen.ResetData();
			instance.currentScreen = null;
		}
		Debugs.LogBlue(typeUI.ToString());
		instance.currentScreen = GetScreen(typeUI);
		currentScreen.Show();
		currentScreen.InitData();
	}

}
[System.Serializable]
public class BOL_ListScreenInHome {
	[Header("Prefabs")]
	public GameObject BOL_home_offline_prefab;
	public GameObject BOL_campaign_prefab;
	public GameObject BOL_arcade_prefab;
	public GameObject BOL_survival_prefab;
	public GameObject BOL_timed_mode_prefab;
	public GameObject BOL_tutorial_prefab;
	[Header("function")]
	public BOL_UI_Offline _BOL_home_offline;
	public BOL_UI_Offline _BOL_campaign;
	public BOL_UI_Offline _BOL_arcade;
	public BOL_UI_Offline _BOL_survival;
	public BOL_UI_Offline _BOL_timed_mode;
	public BOL_UI_Offline _BOL_tutorial;
	public BOL_UI_Offline BOL_home_offline {
		get {
			if (_BOL_home_offline == null) {
				_BOL_home_offline
				= LeanPool.Spawn(BOL_home_offline_prefab, Vector3.zero, Quaternion.identity, BOL_MainControl_Offline.instance.mainScreeenHolder).GetComponent<BOL_UI_Offline>();
			}
			return _BOL_home_offline;
		}
	}
	public BOL_UI_Offline BOL_campaign {
		get {
			if (_BOL_campaign == null) {
				_BOL_campaign
				= LeanPool.Spawn(BOL_campaign_prefab, Vector3.zero, Quaternion.identity, BOL_MainControl_Offline.instance.mainScreeenHolder).GetComponent<BOL_UI_Offline>();
			}
			return _BOL_campaign;
		}
	}
	public BOL_UI_Offline BOL_arcade {
		get {
			if (_BOL_arcade == null) {
				_BOL_arcade
				 = LeanPool.Spawn(BOL_arcade_prefab, Vector3.zero, Quaternion.identity, BOL_MainControl_Offline.instance.mainScreeenHolder).GetComponent<BOL_UI_Offline>();
			}
			return _BOL_arcade;
		}
	}
	public BOL_UI_Offline BOL_survival {
		get {
			if (_BOL_survival == null) {
				_BOL_survival
				= LeanPool.Spawn(BOL_survival_prefab, Vector3.zero, Quaternion.identity, BOL_MainControl_Offline.instance.mainScreeenHolder).GetComponent<BOL_UI_Offline>();
			}
			return _BOL_survival;
		}
	}
	public BOL_UI_Offline BOL_timed_mode {
		get {
			if (_BOL_timed_mode == null) {
				_BOL_timed_mode
				= LeanPool.Spawn(BOL_timed_mode_prefab, Vector3.zero, Quaternion.identity, BOL_MainControl_Offline.instance.mainScreeenHolder).GetComponent<BOL_UI_Offline>();
			}
			return _BOL_timed_mode;
		}
	}
	public BOL_UI_Offline BOL_tutorial {
		get {
			if (_BOL_tutorial == null) {
				_BOL_tutorial
				= LeanPool.Spawn(BOL_tutorial_prefab, Vector3.zero, Quaternion.identity, BOL_MainControl_Offline.instance.mainScreeenHolder).GetComponent<BOL_UI_Offline>();
			}
			return _BOL_tutorial;
		}
	}
}
