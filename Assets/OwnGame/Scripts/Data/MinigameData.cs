using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MiniGameData {
	public List<MiniGameDetail> listMiniGameDetail;
	
	public MiniGameDetail currentMiniGameDetail{
		get{
			return _currentMiniGameDetail;
		}
		set{
			_currentMiniGameDetail = value;
		}
	}
	private MiniGameDetail _currentMiniGameDetail;

	public MiniGameDetail currentSubGameDetail{
		get{
			return _currentSubGameDetail;
		}set{
			_currentSubGameDetail = value;
		}
	}
	private MiniGameDetail _currentSubGameDetail;

	public bool isInitialized;

	public MiniGameData(){}
	
	public void InitData(){
		listMiniGameDetail = new List<MiniGameDetail> ();
		for(int i = 0; i < CoreGameManager.instance.gameInfomation.listMiniGames.Count; i++){
			IMiniGameInfo _info = CoreGameManager.instance.gameInfomation.listMiniGames[i];
			if(_info.canEnable){
				AddNewGameDetail(_info.gameType);
			}
		}
		isInitialized = true;
	}

	/// <summary>
	/// Checks the update for new version.
	/// 	- Check dữ liệu mới add thêm vào khi có biến check new version
	/// </summary>
	public void CheckWhenLogin(){
		// --- Check For Update New --- //
		if(listMiniGameDetail == null || listMiniGameDetail.Count == 0){
			InitData();
		}else{
			for(int i = 0; i < CoreGameManager.instance.gameInfomation.listMiniGames.Count; i++){
				IMiniGameInfo _info = CoreGameManager.instance.gameInfomation.listMiniGames[i];
				if(_info != null){
					if(_info.canEnable){
						bool _canAddNew = true;
						for(int j = 0; j < listMiniGameDetail.Count; j++){
							if(_info.gameId == listMiniGameDetail[j].myInfo.gameId){
								_canAddNew = false;
								break;
							}
						}
						if(_canAddNew){
							AddNewGameDetail(_info.gameType);
						}
					}else{
						for(int j = 0; j < listMiniGameDetail.Count; j++){
							if(_info.gameId == listMiniGameDetail[j].myInfo.gameId){
								listMiniGameDetail.RemoveAt(j);
								break;
							}
						}
					}
				}
			}

			for(int i = 0; i < listMiniGameDetail.Count; i++){
				listMiniGameDetail[i].CheckWhenLogin();
			}
		}
		// --------------------------- //
	}

	void AddNewGameDetail(IMiniGameInfo.Type _gameType){
		if(listMiniGameDetail == null){
			listMiniGameDetail = new List<MiniGameDetail>();
		}
		listMiniGameDetail.Add(new MiniGameDetail(_gameType));
	}

	public void SetCurrentMiniGameDetail(short _gameId){
		for (int i = 0; i < listMiniGameDetail.Count; i++) {
			if (listMiniGameDetail [i].myInfo.gameId == _gameId) {
				currentMiniGameDetail = listMiniGameDetail [i];
				return;
			}
		}
		Debug.LogError ("SetCurrentMiniGameDetail return null: " + _gameId);
	}

	public MiniGameDetail GetMiniGameDetail(IMiniGameInfo.Type _gameType){
		if(listMiniGameDetail == null || listMiniGameDetail.Count == 0){
			return null;
		}
		for(int i = 0; i < listMiniGameDetail.Count; i ++){
			if(listMiniGameDetail[i].gameType == _gameType){
				return listMiniGameDetail[i];
			}
		}
		return null;
	}
}

[System.Serializable]
public class MiniGameDetail {
	public IMiniGameInfo myInfo{
		get{ 
			if (_myInfo == null) {
				_myInfo = CoreGameManager.instance.gameInfomation.GetMiniGameInfo (gameType);
			}
			return _myInfo;
		}
	}
	IMiniGameInfo _myInfo;

	public IMiniGameInfo.Type gameType;
	public TableData tableData;

	public List<SubServerDetail> listServerDetail_Normal{
		get{
			return _listServerDetail_Normal;
		}set{
			_listServerDetail_Normal = value;
		}
	}
	List<SubServerDetail> _listServerDetail_Normal;

	public List<SubServerDetail> listServerDetail_Error{
		get{
			return _listServerDetail_Error;
		}set{
			_listServerDetail_Error = value;
		}
	}
	List<SubServerDetail> _listServerDetail_Error;

	public SubServerDetail currentServerDetail{
		get{
			return _currentServerDetail;
		}
		set{
			_currentServerDetail = value;
		}
	}
	SubServerDetail _currentServerDetail;

	public MiniGameDetail (IMiniGameInfo.Type _gameType){
		tableData = new TableData();
		listServerDetail_Normal = new List<SubServerDetail>();
		listServerDetail_Error = new List<SubServerDetail>();
		
		gameType = _gameType;
	}

	public void SortListServerDetailAgain(){
		List<SubServerDetail> _listServerDetail = new List<SubServerDetail>();
		List<SubServerDetail> _listServerDetail_Normal = new List<SubServerDetail>();
		List<SubServerDetail> _listServerDetail_Error = new List<SubServerDetail>();

		if(myInfo.isSubGame){
			for(int i = 0; i < DataManager.instance.subServerData.listSubServerDetail.Count; i ++){
				_listServerDetail.Add(DataManager.instance.subServerData.listSubServerDetail[i]);
			}
		}else{
			for(int i = 0; i < DataManager.instance.subServerData.listSubServerDetail.Count; i ++){
				if(DataManager.instance.subServerData.listSubServerDetail[i].IsContainMiniGame(this)){
					_listServerDetail.Add(DataManager.instance.subServerData.listSubServerDetail[i]);
				}
			}
		}

		_listServerDetail.Sort(delegate (SubServerDetail _serverSau, SubServerDetail _serverTruoc) // y.CompareTo(x) = -1 là dịch về phía sau
        {
            // -1 là dịch về phía trước
			long _timeDelayOfServerTruoc = _serverTruoc.timeDelay;
			long _timeDelayOfServerSau = _serverSau.timeDelay;

            if(_timeDelayOfServerTruoc == -1){
                return 1;
            }

            if(_timeDelayOfServerSau > _timeDelayOfServerTruoc){
                return 1;
            }else if(_timeDelayOfServerSau == _timeDelayOfServerTruoc){
                if(_serverSau.countConnectionError > _serverTruoc.countConnectionError){
                    return 1;
                }else if(_serverSau.countConnectionError == _serverTruoc.countConnectionError){
                    return 0;
                }else{
                    return -1;
                }
            }else{
                return -1;
            }
            // Debug.Log(y + " - " + x + " - " + y.CompareTo(x));
            // return y.CompareTo(x);
        });

		for(int i = 0; i < _listServerDetail.Count; i++){
			// Debug.Log(">>> " + _listServerDetail[i].subServerName);
			if(_listServerDetail[i].beingError){
				_listServerDetail_Error.Add(_listServerDetail[i]);
			}else{
				_listServerDetail_Normal.Add(_listServerDetail[i]);
			}
		}

		listServerDetail_Normal = _listServerDetail_Normal;
		listServerDetail_Error = _listServerDetail_Error;
	}

	public MiniGameDetail ShallowCopy()
    {
       MiniGameDetail other = (MiniGameDetail) this.MemberwiseClone();
       return other;
    }

	public void CheckWhenLogin(){
		if(tableData == null){
			tableData = new TableData();
		}else{
			tableData.CheckWhenLogin();
		}
	}
}