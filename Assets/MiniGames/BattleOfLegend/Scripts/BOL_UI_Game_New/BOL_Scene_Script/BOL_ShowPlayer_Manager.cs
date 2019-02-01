using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using UnityEngine.UI;
using System;

public class BOL_ShowPlayer_Manager : MonoBehaviour {
	public static BOL_ShowPlayer_Manager instance {
		get {
			return ins;
		}
	}
	public static BOL_ShowPlayer_Manager ins;
	public List<UserDataInGame> listUserIngame;
	public UserDataInGame[] listUserPlayGame;
	public void Awake() {
		ins = this;
	}
	public void InitUserInroom(UserDataInGame _userdata) {
		listUserIngame.Add(_userdata);
	}
	public void AddPlayerPlayGame(int sessionidplaer, int chairpos) {
		for (int i = 0; i < listUserIngame.Count; i++) {
			if (sessionidplaer == listUserIngame[i].sessionId) {
				listUserPlayGame[chairpos] = listUserIngame[i];
				break;
			}
		}
	}
	public void RemovePLayerPlayGame(int sessionidplaer, int chairpos) {
		if (sessionidplaer == listUserPlayGame[chairpos].sessionId) {
#if TEST
			Debug.Log("remove" + sessionidplaer + "  " + chairpos);
#endif
			listUserPlayGame[chairpos] = new UserDataInGame();
		}
	}
	public void RefreshDataUserIngame(long userid, long newgold) {
		for (int i = 0; i < listUserIngame.Count; i++) {
			if (userid == listUserIngame[i].userId) {
				listUserIngame[i].gold = newgold;
				break;
			}
		}
	}
}
