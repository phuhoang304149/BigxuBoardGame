using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;
using UnityEngine.SceneManagement;
using UnityEditor;
using System;

public class BOL_Arcade_Screen : BOL_UI_Offline {
	public List<GameObject> listHero;
	public List<CanvasGroup> listAnimationChoice;
	public int enemyHero { get; set; }
	public int playerHero { get; set; }
	public GameObject BattleScene;
	public int lastChoice;
	public GameObject heroTemp;
	public List<GameObject> listHeroChoice;
	public override UIType mySceneType {
		get {
			return UIType.arcade;
		}
	}

	public override void InitData() {
		base.InitData();
        BOL_Battle_Screen.instance.SelfDestruction();
		BOL_MainControl_Offline.instance.BattleScene.SetActive(false);
		BOL_MainControl_Offline.instance.PanelGame.SetActive(false);
		myLastType = UIType.home;
		if (MainControlArcade.instance != null) {
			MainControlArcade.instance.SelfDestruction();
		}
		BOL_MainControl_Offline.instance.typeUI = UIType.home;
		Debugs.LogBlue(BOL_MainControl_Offline.instance.typeUI.ToString());
		for (int i = 0; i < listHeroChoice.Count; i++) {
			listHeroChoice[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(!GameInformation.instance.listHeroInfo[i].autoUnlockAtFirst);
		}
	}
	public override void ResetData() {
		lastChoice = 0;
		if (heroTemp != null) {
			LeanPool.Despawn(heroTemp);
			heroTemp = null;
		}
		for (int i = 0; i < listAnimationChoice.Count;i++){
			listAnimationChoice[i].alpha = 0;
        }
	}
	public override void SelfDestruction() {
#if TEST
		Debug.Log("hủy");
#endif
	}
	public void CreateHero(int heroChoice) {
		if (GameInformation.instance.listHeroInfo[heroChoice].autoUnlockAtFirst) {
			if (heroTemp != null) {
				LeanPool.Despawn(heroTemp);
				heroTemp = null;
			}
			listAnimationChoice[lastChoice].alpha = 0;
			listAnimationChoice[heroChoice].alpha = 1;
			lastChoice = heroChoice;
			heroTemp = LeanPool.Spawn(listHero[heroChoice], Vector3.zero, Quaternion.identity);
		} else {
			PopupManager.Instance.CreateToast("hero is locked");
		}
	}
    
    
	public void StartGame() {
		BOL_MainControl_Offline.instance.BattleScene.SetActive(true);
		BOL_MainControl_Offline.instance.PanelGame.SetActive(true);
		ResetData();
		BOL_MainControl_Offline.instance.currentScreen.Hide();
		BOL_MainControl_Offline.instance.typeUI = UIType.arcade;
        //int heroComp = UnityEngine.Random.Range(0, 11);
		int heroComp = 1;
        
		BOL_Battle_Screen.instance.playerHeroPrefab = listHero[lastChoice];
		BOL_Battle_Screen.instance.compHeroPrefab = listHero[heroComp];
		BOL_Battle_Screen.instance.InitData();
        ArcadeGame.instance.heroInfoPlayer = GameInformation.instance.listHeroInfo[lastChoice];
		ArcadeGame.instance.heroInfoComp = GameInformation.instance.listHeroInfo[heroComp];
	}
	public void HomeClick() {
		if (BOL_MainControl_Offline.instance.currentScreen != null) {
			BOL_MainControl_Offline.instance.lateScreen
			= BOL_MainControl_Offline.instance.currentScreen;
			BOL_MainControl_Offline.instance.lateScreen.Hide();
			BOL_MainControl_Offline.instance.currentScreen = null;
			ResetData();
		}
		BOL_MainControl_Offline.instance.currentScreen =
		BOL_MainControl_Offline.instance.GetScreen(UIType.home);
		BOL_MainControl_Offline.instance.currentScreen.Show();
		BOL_MainControl_Offline.instance.currentScreen.InitData();
	}
}
