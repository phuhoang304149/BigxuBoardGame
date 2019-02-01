using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] public class BOL_GameData {

    public BOL_HeroData heroData;
    public bool isInitialized;

    public BOL_GameData(){}

    public void InitData(){
		heroData = new BOL_HeroData();
        heroData.InitData();
		isInitialized = true;
	}
}
