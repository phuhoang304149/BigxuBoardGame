using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] public class BOL_HeroData {

	public List<BOL_HeroDetail> listHeroDetail;
	public bool isInitialized;

	public BOL_HeroData(){}
	public void InitData(){
		listHeroDetail = new List<BOL_HeroDetail>();
		for(int i = 0; i < CoreGameManager.instance.gameInfomation.listHeroInfo.Count; i++){
			BOL_HeroInfo _info = CoreGameManager.instance.gameInfomation.listHeroInfo[i];
			if(_info.canEnable){
				AddNewHeroDetail(_info);
			}
		}
		isInitialized = true;
	}

	void AddNewHeroDetail(BOL_HeroInfo _heroInfo){
		if(listHeroDetail == null){
			listHeroDetail = new List<BOL_HeroDetail>();
		}
		listHeroDetail.Add(new BOL_HeroDetail(_heroInfo));
	}

	/// <summary>
	/// Checks the update for new version.
	/// 	- Check dữ liệu mới add thêm vào khi có biến check new version
	/// </summary>
	public void CheckWhenLogin(){
		// --- Check For Update New --- //
		if(listHeroDetail == null || listHeroDetail.Count == 0){
			InitData();
		}else{
			for(int i = 0; i < CoreGameManager.instance.gameInfomation.listHeroInfo.Count; i++){
				BOL_HeroInfo _info = CoreGameManager.instance.gameInfomation.listHeroInfo[i];
				if(_info != null){
					if(_info.canEnable){
						bool _canAddNew = true;
						for(int j = 0; j < listHeroDetail.Count; j++){
							if(_info.myType == listHeroDetail[j].myInfo.myType){
								_canAddNew = false;
								break;
							}
						}
						if(_canAddNew){
							AddNewHeroDetail(_info);
						}
					}
				}
			}
		}
		// --------------------------- //
	}
}

[System.Serializable] public class BOL_HeroDetail {
	public BOL_HeroInfo.Type heroType;

	public BOL_HeroInfo myInfo{
		get{ 
			if (_myInfo == null) {
				_myInfo = CoreGameManager.instance.gameInfomation.GetHeroInfo (heroType);
			}
			return _myInfo;
		}
	}
	BOL_HeroInfo _myInfo;

	public bool unlockMe;
	public long exp;
	public long level;
	public bool unlockAtk2;
	public bool unlockSkill01;
	public bool unlockSkill02;
	public bool unlockUlti;

	public BOL_HeroDetail(){}

	public BOL_HeroDetail(BOL_HeroInfo _heroInfo){
		_myInfo = _heroInfo;
		heroType = myInfo.myType;
		unlockMe = myInfo.autoUnlockAtFirst;
		exp = 0;
		unlockAtk2 = false;
		unlockSkill01 = false;
		unlockSkill02 = false;
		unlockUlti = false;
	}

}