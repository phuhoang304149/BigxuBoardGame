using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubGame_ListServer_Option_Controller : MySimplePoolObjectController {

	[SerializeField] Text txtRoomName;
	SubGame_ListServer_Controller panelListServer;
	SubServerDetail serverDetail;

	public override void ResetData(){
		StopAllCoroutines();
		serverDetail = null;
    }

	void ScrollCellIndex (int _index) 
    {	
		if(panelListServer == null){
			panelListServer = ChooseSubGameScreenController.instance.panelListServer;
		}
		if(panelListServer == null){
			Debug.LogError("panelListServer is NULL");
			return;
		}
		// string _name = "PanelInstallApp_OptionInfo_" + _index.ToString ();
		// gameObject.name = name;
		InitData(panelListServer.listRoomDetail[_index]);
	}

	void InitData(SubServerDetail _serverDetail){
		serverDetail = _serverDetail;
		txtRoomName.text = serverDetail.subServerName + " " + string.Format("{0:00}", serverDetail.subServerId) + " - " + serverDetail.countryCode;
	}

	public void OnClicked(){
        if(serverDetail == null){
            return;
        }
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
		ChooseSubGameScreenController.instance.OnChooseServer(serverDetail);
    }
}
