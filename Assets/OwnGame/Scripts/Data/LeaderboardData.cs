using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] public class LeaderboardData {
	public List<UserData> topGold;
	public DateTime lastTimeUpdateTop;
	public DateTime nextTimeToGetNewList;
	public bool isInitialized;

	public LeaderboardData(){}

	public void InitData(){
		topGold = new List<UserData> ();
		lastTimeUpdateTop = DateTime.Now;
		nextTimeToGetNewList = DateTime.Now;
		isInitialized = true;
	}

	public void SortListTopGoldAgain(){
		if(topGold == null || topGold.Count == 0){
			Debug.LogError("topGold is null or empty");
			return;
		}
		topGold.Sort(delegate(UserData x, UserData y)
        {   
			var _result = y.gold.CompareTo(x.gold);
			// Debug.Log(_result + " - " + x.gold + " - " + y.gold);
            return _result;
        });
	}

	public void CheckWhenLogin(){
		if(lastTimeUpdateTop == System.DateTime.MinValue){
			lastTimeUpdateTop = System.DateTime.Now;
		}
		nextTimeToGetNewList = DateTime.Now;
		if(topGold == null){
			topGold = new List<UserData> ();
		}
	}
}
