using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Coffee.UIExtensions;

public class ChooseGame_PanelGameOption_Controller : MonoBehaviour {

	[SerializeField] UIEffect avatarGameUiEffect;
	[SerializeField] UIEffect glowUiEffect;
	IMiniGameInfo myMiniGameInfo;
	public short gameId { get; set; }

	int tmpCountChooseGameFailed;

	public void InitData(IMiniGameInfo _myMiniGameInfo){
        myMiniGameInfo = _myMiniGameInfo;
        gameId = myMiniGameInfo.gameId;

		if(myMiniGameInfo.canEnable){
			avatarGameUiEffect.effectFactor = 0f;
			avatarGameUiEffect.effectColor = Color.white;
			glowUiEffect.effectFactor = 0f;
			glowUiEffect.effectColor = Color.white;
		}else{
			avatarGameUiEffect.effectFactor = 1f;
			avatarGameUiEffect.effectColor = Color.gray;
			glowUiEffect.effectFactor = 1f;
			glowUiEffect.effectColor = Color.gray;
		}

		tmpCountChooseGameFailed = 0;
	}

	public void OnButtonSelectClicked(){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);

		if (myMiniGameInfo == null) {
			#if TEST
        	Debug.LogError (">>> myMiniGameInfo is null");
        	#endif
			return;
		}
		if (!myMiniGameInfo.canEnable) {
			PopupManager.Instance.CreateToast (MyLocalize.GetString("Global/CommingSoon"));
			return;
		}
        #if TEST
        Debug.Log (">>> Chọn gameType: " + myMiniGameInfo.gameType);
        #endif

		DataManager.instance.miniGameData.SetCurrentMiniGameDetail (gameId);

		if(DataManager.instance.miniGameData.currentMiniGameDetail == null){
			#if TEST
        	Debug.LogError (">>> currentMiniGameDetail is null: ");
        	#endif
			return;
		}

		DataManager.instance.miniGameData.currentMiniGameDetail.SortListServerDetailAgain();

		SetCurrentRoomDetailAndChangeScreen();
	}

	void SetCurrentRoomDetailAndChangeScreen(){
		List<SubServerDetail> _listSubServerDetail = new List<SubServerDetail>();
		for(int i = 0; i < DataManager.instance.miniGameData.currentMiniGameDetail.listServerDetail_Normal.Count; i ++){
			_listSubServerDetail.Add(DataManager.instance.miniGameData.currentMiniGameDetail.listServerDetail_Normal[i]);
		}
		for(int i = 0; i < DataManager.instance.miniGameData.currentMiniGameDetail.listServerDetail_Error.Count; i ++){
			_listSubServerDetail.Add(DataManager.instance.miniGameData.currentMiniGameDetail.listServerDetail_Error[i]);
		}

		if(_listSubServerDetail.Count > 0){
			bool _forcedUpdate = true;
			long _versionMiniGame = DataManager.instance.miniGameData.currentMiniGameDetail.myInfo.versionFeature;
			for(int i = 0; i < _listSubServerDetail.Count; i ++){
				for(int j = 0; j < _listSubServerDetail[i].listRoomDetail.Count; j++){
					if(_listSubServerDetail[i].listRoomDetail[j].gameId != DataManager.instance.miniGameData.currentMiniGameDetail.myInfo.gameId){
						continue;
					}
					if(_versionMiniGame >=  _listSubServerDetail[i].listRoomDetail[j].versionRoom){
						_forcedUpdate = false;
					}
					if(_forcedUpdate){
						#if TEST
						Debug.Log("<color=green> RoomOutOfDate: " + _versionMiniGame + " - " + _listSubServerDetail[i].listRoomDetail[j].versionRoom + " </color>");
						#endif
					}else{
						break;
					}
				}
			}
			if(_forcedUpdate){
				PopupManager.Instance.CreatePopupDialog(MyLocalize.GetString(MyLocalize.kWarning)
				, MyLocalize.GetString(MyLocalize.kForcedUpdate)
				, string.Empty
				, MyLocalize.GetString(MyLocalize.kUpdate)
				, MyLocalize.GetString(MyLocalize.kCancel)
				, () =>{
					Application.OpenURL(MyConstant.linkApp);
				}, null);
				return;
			}

			HomeManager.instance.ChangeScreen (UIHomeScreenController.UIType.ChooseTable);
		}else{
			#if TEST
        	Debug.LogError (">>> Không có Room nào hết!");
        	#endif
		}
	}
}
