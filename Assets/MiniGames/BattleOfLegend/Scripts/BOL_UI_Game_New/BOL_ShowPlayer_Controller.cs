using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;

public class  BOL_ShowPlayer_Controller : MonoBehaviour {
	public Text nameText;
	public Text goldText;
	public RawImage imageAvatar;
	public int sessionid;
	public long userid;
	public string nameuser;
	public string goldUser;
	public UserDataInGame userData;
	public void InitData(UserDataInGame _userData) {
		nameText.text = MyConstant.ConvertString(_userData.nameShowInGame, 15);
		goldText.text = _userData.gold.ToString();
		userData = _userData;
		sessionid = _userData.sessionId;
		userid = _userData.userId;
		nameuser = _userData.nameShowInGame;
		goldUser = MyConstant.GetMoneyString(_userData.gold, 99999);

	}
	public void InitDataInGame(UserDataInGame _userData) {
		userData = _userData;
	}

	public void SetInfoWhenPress(int post) {
		//if (BOL_ShowPlayer_Manager.instance.listUserPlayGame[post] != null) {
		if (BOL_ShowPlayer_Manager.instance.listUserPlayGame[post].sessionId != -1) {
        #if TEST
        Debug.Log(BOL_ShowPlayer_Manager.instance.listUserPlayGame[post].facebookId);
            Debug.Log(BOL_ShowPlayer_Manager.instance.listUserPlayGame[post].userId);
        #endif
			
			PopupManager.Instance.CreatePopupPlayerInfo(BOL_ShowPlayer_Manager.instance.listUserPlayGame[post]);
		}
		//}else{
		//#if TEST
		//Debug.Log("chưa khởi tạo biến");
		//#endif
		//}
	}

	public void SelfDestrustion() {
		LeanPool.Despawn(gameObject);
	}
}
