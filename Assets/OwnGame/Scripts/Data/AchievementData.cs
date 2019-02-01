using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AchievementData {

	public List<AchievementDetail> listAchievementDetail;
	public bool isInitialized;

	public AchievementData(){}

	public void InitData(){
		listAchievementDetail = new List<AchievementDetail> ();

		for (int i = 0; i < CoreGameManager.instance.gameInfomation.listMiniGames.Count; i++) {
			if (CoreGameManager.instance.gameInfomation.listMiniGames [i].canEnable) {
				AddNewAchievementDetail(CoreGameManager.instance.gameInfomation.listMiniGames [i].gameType);
			}
		}

		isInitialized = true;
	}

	void AddNewAchievementDetail(IMiniGameInfo.Type _gameType){
		if(listAchievementDetail == null){
			listAchievementDetail = new List<AchievementDetail>();
		}
		AchievementDetail _achievementDetail = new AchievementDetail (_gameType);
		listAchievementDetail.Add (_achievementDetail);
	}

	public AchievementDetail GetAchievementDetail(IMiniGameInfo.Type _gameType){
		if(listAchievementDetail == null || listAchievementDetail.Count == 0){
			return null;
		}
		for(int i = 0; i < listAchievementDetail.Count; i++){
			if(listAchievementDetail[i].myGameInfo.gameType == _gameType){
				return listAchievementDetail[i];
			}
		}
		return null;
	}

	/// <summary>
	/// Checks the update for new version.
	/// 	- Check dữ liệu mới add thêm vào khi có biến check new version
	/// </summary>
	public void CheckWhenLogin(){
		// --- Check For Update New --- //
		if(listAchievementDetail == null){
			listAchievementDetail = new List<AchievementDetail>();
		}
		for(int i = 0; i < CoreGameManager.instance.gameInfomation.listMiniGames.Count; i++){
			IMiniGameInfo _info = CoreGameManager.instance.gameInfomation.listMiniGames[i];
			if(_info != null){
				if(_info.canEnable){
					bool _canAddAchievement = true;
					for(int j = 0; j < listAchievementDetail.Count; j++){
						if(_info.gameType == listAchievementDetail[j].myGameInfo.gameType){
							_canAddAchievement = false;
							break;
						}
					}
					if(_canAddAchievement){
						AddNewAchievementDetail(_info.gameType);
					}
				}else{
					for(int j = 0; j < listAchievementDetail.Count; j++){
						if(_info.gameType == listAchievementDetail[j].myGameInfo.gameType){
							listAchievementDetail.RemoveAt(j);
							break;
						}
					}
				}
			}
		}
		// --------------------------- //
	}

	public AchievementData ShallowCopy()
    {
       AchievementData other = (AchievementData) this.MemberwiseClone();
       return other;
    }
}

[System.Serializable]
public class AchievementDetail{
	public IMiniGameInfo myGameInfo{
		get{ 
			if (_myGameInfo == null) {
				_myGameInfo = CoreGameManager.instance.gameInfomation.GetMiniGameInfo (gameType);
			}
			return _myGameInfo;
		}
	}
	IMiniGameInfo _myGameInfo;

	public IMiniGameInfo.Type gameType;
	public int countWin;
	public int countDraw;
	public int countLose;

	public AchievementDetail(){}

	public AchievementDetail(IMiniGameInfo.Type _gameType){
		gameType = _gameType;
		countWin = 0;
		countDraw = 0;
		countLose = 0;
	}
}