using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseTable_PanelListTable_Controller : MonoBehaviour {

    [SerializeField] RectTransform myContent;
	[Header("Prefabs")]
    [SerializeField] GameObject prefabTableOption_Default;
	[SerializeField] GameObject prefabTableOption_AnimalRacing;
	[SerializeField] GameObject prefabTableOption_Bol;
	[SerializeField] GameObject prefabTableOption_Poker;
	[SerializeField] GameObject prefabTableOption_Uno;

	public List<ChooseTable_PanelTableOption_Controller> listTable{ get; set;}
	bool isInstalled;

    int numTableDefault;

	public ChooseTableScreenController chooseTableScreen{get;set;}

    public void InitData(ChooseTableScreenController _chooseTableScreen, bool _createWithRealData = true){
		if (!isInstalled) {
			chooseTableScreen = _chooseTableScreen;
			MiniGameDetail _gameDetail = DataManager.instance.miniGameData.currentMiniGameDetail;
			
			numTableDefault = _chooseTableScreen.numTableDefault;

            Vector2 _pos = myContent.offsetMin;
            _pos.x = 0f;
            myContent.offsetMin = _pos;

			if (listTable == null || listTable.Count == 0) {
				if(listTable == null){
					listTable = new List<ChooseTable_PanelTableOption_Controller> ();
				}
				
				GameObject _prefabTableOption = GetPrefabTableOptionInfo();
				for (int i = 0; i < numTableDefault; i++) {
					ChooseTable_PanelTableOption_Controller _tableInfo = ((GameObject)Instantiate (_prefabTableOption, myContent.transform, false)).GetComponent<ChooseTable_PanelTableOption_Controller> ();
					_tableInfo.InitData (chooseTableScreen, null);
					listTable.Add (_tableInfo);
				}
			}
			if(!_createWithRealData){
				Invoke("ResizeContent", 0.2f);
			}else{
				RoomDetail _roomDetail = null;
				if(_gameDetail.currentServerDetail != null){
					for(int i = 0; i < _gameDetail.currentServerDetail.listRoomDetail.Count; i++){
						if(_gameDetail.currentServerDetail.listRoomDetail[i].gameId == _gameDetail.myInfo.gameId){
							_roomDetail = _gameDetail.currentServerDetail.listRoomDetail[i];
							break;
						}
					}
				}
				
				if(_roomDetail == null){
					#if TEST
					Debug.LogError("_roomDetail is null: " + _gameDetail.myInfo.gameType.ToString());
					#endif
				}
				
				if(_gameDetail.currentServerDetail == null || _roomDetail == null || _gameDetail.myInfo.versionFeature < _roomDetail.versionRoom){
					Invoke("ResizeContent", 0.2f);
					isInstalled = true;
					return;
				}
				if (_gameDetail.tableData != null && _gameDetail.tableData.listTableDetail.Count != 0) {
					if (listTable.Count > _gameDetail.tableData.listTableDetail.Count) {
						for (int i = listTable.Count - 1; i >= _gameDetail.tableData.listTableDetail.Count; i --) {
							Destroy(listTable[i].gameObject);
							listTable.RemoveAt(i);
						}
					}

					GameObject _prefabTableOption = GetPrefabTableOptionInfo();
					for (int i = 0; i < _gameDetail.tableData.listTableDetail.Count; i++) {
						if (i < listTable.Count) {
							listTable[i].InitData (chooseTableScreen, _gameDetail.tableData.listTableDetail [i]);
						} else {
							ChooseTable_PanelTableOption_Controller _tableInfo = ((GameObject)Instantiate (_prefabTableOption, myContent.transform, false)).GetComponent<ChooseTable_PanelTableOption_Controller> ();
							_tableInfo.InitData (chooseTableScreen, _gameDetail.tableData.listTableDetail [i]);
							listTable.Add (_tableInfo);
						}
					}
				}

				Invoke("ResizeContent", 0.2f);
			}
			

            isInstalled = true;
		}
	}

    public void ResizeContent() {
		if(listTable == null || listTable.Count == 0){
			return;
		}
        // -- Set up lại size khung bao lại của list -- //
        RectTransform _r = listTable[listTable.Count - 1].GetComponent<RectTransform>();
        float _lastPosX = _r.offsetMax.x + 20f;
        myContent.sizeDelta = new Vector2(_lastPosX, myContent.sizeDelta.y);
        // -------------------------------------------- //
    }

    public void InitDataAgain(){
		isInstalled = false;
		InitData (chooseTableScreen);
	}

	public GameObject GetPrefabTableOptionInfo(){
		GameObject _prefabTableOption = prefabTableOption_Default;
		switch(DataManager.instance.miniGameData.currentMiniGameDetail.myInfo.gameType){
		case IMiniGameInfo.Type.AnimalRacing:
			_prefabTableOption = prefabTableOption_AnimalRacing;
			break;
		case IMiniGameInfo.Type.BattleOfLegend:
			_prefabTableOption = prefabTableOption_Bol;
			break;
		case IMiniGameInfo.Type.Poker:
			_prefabTableOption = prefabTableOption_Poker;
			break;
		case IMiniGameInfo.Type.Uno:
			_prefabTableOption = prefabTableOption_Uno;
			break;
		default:
			Debug.LogError(">>> Chưa có prefab: " +DataManager.instance.miniGameData.currentMiniGameDetail.myInfo.gameType.ToString());
			break;
		}
		return _prefabTableOption;
	}

	public void SelfDestruction(){
		if(listTable != null && listTable.Count > 0){
			for(int i = 0; i < listTable.Count; i++){
				Destroy(listTable[i].gameObject);
			}
			listTable.Clear();
		}
		isInstalled = false;
	}
}
