using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Koprok_CheckLogic {
	public static bool CanAddBet(this Koprok_GamePlay_Manager _gamePlayManager){
		if (_gamePlayManager.currentGameState != Koprok_GamePlay_Manager.GameState.Bet)
		{
#if TEST
			Debug.LogError(">>> Không ở trong trang đặt cược nên ko setup");
#endif
			return false;
		}
		if (_gamePlayManager.koprokData == null)
		{
#if TEST
			Debug.LogError(">>> koprokData is NULL");
#endif
			return false;
		}

		if(_gamePlayManager.koprokData.nextTimeToShowResult < System.DateTime.Now.AddSeconds(2f)){
#if TEST
			Debug.LogError(">>> Hết thời gian đặt cược");
#endif
			return false;
		}
		
		return true;
	}

	public static bool CanShowHistory(this Koprok_GamePlay_Manager _gamePlayManager){
		if (_gamePlayManager.currentGameState != Koprok_GamePlay_Manager.GameState.Bet)
		{
#if TEST
			Debug.LogError(">>> Không ở trong trang đặt cược nên ko setup");
#endif
			return false;
		}
		if (_gamePlayManager.koprokData == null)
		{
#if TEST
			Debug.LogError(">>> koprokData is NULL");
#endif
			return false;
		}

		if(_gamePlayManager.koprokData.nextTimeToShowResult < System.DateTime.Now.AddSeconds(2f)){
#if TEST
			Debug.LogError(">>> Hết thời gian show history");
#endif
			return false;
		}
		return true;
	}
}
