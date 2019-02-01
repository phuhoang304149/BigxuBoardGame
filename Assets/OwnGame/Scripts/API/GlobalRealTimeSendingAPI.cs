using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalRealTimeSendingAPI {

	static MessageSending messagePlayNowToTable;
	static MessageSending messagePlayNowMiniGame;
	static MessageSending messageSetParent;
	static MessageSending messageJoinToMiniGame;
	static MessageSending messageJoinToTable;
	static MessageSending messageCreatePassAndJoinTable;
	static MessageSending messageCallBotUno;

	public static void SendMessagePlayNowToTable(short _gameId){
		if (messagePlayNowToTable == null) {
			messagePlayNowToTable = new MessageSending (CMD_REALTIME.C_TABLE_PLAYNOW_TO_TABLE);
		} else {
			messagePlayNowToTable.ClearData ();
		}

		messagePlayNowToTable.writeshort (_gameId);
		messagePlayNowToTable.writeByte ((byte) DataManager.instance.userData.databaseId);
		messagePlayNowToTable.writeLong (DataManager.instance.userData.userId);
		switch (DataManager.instance.userData.databaseId) {
		case UserData.DatabaseType.DATABASEID_DEVICE:
			messagePlayNowToTable.writeString (DataManager.instance.userData.deviceId);
			messagePlayNowToTable.writeString (SystemInfo.deviceModel);
			break;
		case UserData.DatabaseType.DATABASEID_BIGXU:
			messagePlayNowToTable.writeString (DataManager.instance.userData.username);
			messagePlayNowToTable.writeString (DataManager.instance.userData.password);
			break;
		case UserData.DatabaseType.DATABASEID_FACEBOOK:
			messagePlayNowToTable.writeString (DataManager.instance.userData.tokenFBIdOfBusiness);
			break;
		}

		#if TEST
		string _tmp = string.Empty;
		_tmp += _gameId;
		Debug.Log(">>>CMD PlayNowToTable : " + messagePlayNowToTable.getCMD() + "|" + _tmp);
		#endif

		NetworkGlobal.instance.SendMessageRealTime (messagePlayNowToTable);
	}

	public static void SendMessagePlayNowMiniGame(short _gameId){
		if (messagePlayNowMiniGame == null) {
			messagePlayNowMiniGame = new MessageSending (CMD_REALTIME.C_TABLE_JOIN_TO_MINIGAME_STATE);
		} else {
			messagePlayNowMiniGame.ClearData ();
		}

		messagePlayNowMiniGame.writeshort (_gameId);
		messagePlayNowMiniGame.writeByte ((byte) DataManager.instance.userData.databaseId);
		messagePlayNowMiniGame.writeLong (DataManager.instance.userData.userId);
		switch (DataManager.instance.userData.databaseId) {
		case UserData.DatabaseType.DATABASEID_DEVICE:
			messagePlayNowMiniGame.writeString (DataManager.instance.userData.deviceId);
			messagePlayNowMiniGame.writeString (SystemInfo.deviceModel);
			break;
		case UserData.DatabaseType.DATABASEID_BIGXU:
			messagePlayNowMiniGame.writeString (DataManager.instance.userData.username);
			messagePlayNowMiniGame.writeString (DataManager.instance.userData.password);
			break;
		case UserData.DatabaseType.DATABASEID_FACEBOOK:
			// messagePlayNowMiniGame.writeString (Facebook.Unity.AccessToken.CurrentAccessToken.TokenString);
			messagePlayNowMiniGame.writeString (DataManager.instance.userData.tokenFBIdOfBusiness);
			break;
		}

		#if TEST
		string _tmp = string.Empty;
		_tmp += _gameId;
		Debug.Log(">>>CMD PlayNowMiniGame : " + messagePlayNowMiniGame.getCMD() + "|" + _tmp);
		#endif

		NetworkGlobal.instance.SendMessageRealTime (messagePlayNowMiniGame);
	}

	public static void SendMessageJoinToMiniGame(IMiniGameInfo _gameInfo){
		if(!_gameInfo.isSubGame){
			Debug.LogError(">>> " + _gameInfo.gameType.ToString() + " không phải subgame");
			return;
		}
		
		switch(_gameInfo.gameType){
		case IMiniGameInfo.Type.DragonTigerCasino:
			messageJoinToMiniGame = new MessageSending (CMD_REALTIME.C_MINIGAME_LONGHO_JOIN_GAME);
			break;
		case IMiniGameInfo.Type.Koprok:
			messageJoinToMiniGame = new MessageSending (CMD_REALTIME.C_MINIGAME_BAUCUA_JOIN_GAME);
			break;
		default:
			messageJoinToMiniGame = null;
			Debug.LogError(">>> Chưa setup API game " + _gameInfo.gameType.ToString());
			break;
		}
		if(messageJoinToMiniGame != null){
			NetworkGlobal.instance.SendMessageRealTime (messageJoinToMiniGame);
		}
	}

	public static void SendMessageJoinToTable(short _gameId, short _tableid, string _passwordTable = ""){
		if (messageJoinToTable == null) {
			messageJoinToTable = new MessageSending (CMD_REALTIME.C_TABLE_JOIN_TO_TABLE);
		} else {
			messageJoinToTable.ClearData ();
		}

		messageJoinToTable.writeshort (_gameId);
		messageJoinToTable.writeshort (_tableid);
		messageJoinToTable.writeString (_passwordTable);
		messageJoinToTable.writeByte ((byte) DataManager.instance.userData.databaseId);
		messageJoinToTable.writeLong (DataManager.instance.userData.userId);
		switch (DataManager.instance.userData.databaseId) {
		case UserData.DatabaseType.DATABASEID_DEVICE:
			messageJoinToTable.writeString (DataManager.instance.userData.deviceId);
			messageJoinToTable.writeString (SystemInfo.deviceModel);
			break;
		case UserData.DatabaseType.DATABASEID_BIGXU:
			messageJoinToTable.writeString (DataManager.instance.userData.username);
			messageJoinToTable.writeString (DataManager.instance.userData.password);
			break;
		case UserData.DatabaseType.DATABASEID_FACEBOOK:
			messageJoinToTable.writeString (DataManager.instance.userData.tokenFBIdOfBusiness);
			break;
		}

		#if TEST
		string _tmp = string.Empty;
		_tmp += _gameId + "|" + _tableid + "|" + _passwordTable;
		Debug.Log(">>>CMD JoinToTable : " + messageJoinToTable.getCMD() + "|" + _tmp);
		#endif

		NetworkGlobal.instance.SendMessageRealTime (messageJoinToTable);
	}

	public static void SendMessageCreatePassAndJoinTable(short _gameId, string _passwordTable){
		if (messageCreatePassAndJoinTable == null) {
			messageCreatePassAndJoinTable = new MessageSending (CMD_REALTIME.C_TABLE_CREATE_PASSWORD_TABLE);
		} else {
			messageCreatePassAndJoinTable.ClearData ();
		}

		messageCreatePassAndJoinTable.writeshort (_gameId);
		messageCreatePassAndJoinTable.writeString (_passwordTable);
		messageCreatePassAndJoinTable.writeByte ((byte) DataManager.instance.userData.databaseId);
		messageCreatePassAndJoinTable.writeLong (DataManager.instance.userData.userId);
		
		switch (DataManager.instance.userData.databaseId) {
		case UserData.DatabaseType.DATABASEID_DEVICE:
			messageCreatePassAndJoinTable.writeString (DataManager.instance.userData.deviceId);
			messageCreatePassAndJoinTable.writeString (SystemInfo.deviceModel);
			break;
		case UserData.DatabaseType.DATABASEID_BIGXU:
			messageCreatePassAndJoinTable.writeString (DataManager.instance.userData.username);
			messageCreatePassAndJoinTable.writeString (DataManager.instance.userData.password);
			break;
		case UserData.DatabaseType.DATABASEID_FACEBOOK:
			messageCreatePassAndJoinTable.writeString (DataManager.instance.userData.tokenFBIdOfBusiness);
			break;
		}

		#if TEST
		string _tmp = string.Empty;
		_tmp += _gameId + "|" + "|" + _passwordTable;
		Debug.Log(">>>CMD CreatePassAndJoinTable : " + messageCreatePassAndJoinTable.getCMD() + "|" + _tmp);
		#endif

		NetworkGlobal.instance.SendMessageRealTime (messageCreatePassAndJoinTable);
	}

	public static void SendMessageSetParent(short _parentSessionId){
		if (messageSetParent == null) {
			messageSetParent = new MessageSending (CMD_REALTIME.C_GAMEPLAY_SET_PARENT);
		} else {
			messageSetParent.ClearData ();
		}

		messageSetParent.writeshort (_parentSessionId);

		#if TEST
		string _tmp = string.Empty;
		_tmp += _parentSessionId;
		Debug.Log(">>>CMD SendMessageSetParent : " + messageSetParent.getCMD() + "|" + _tmp);
		#endif

		NetworkGlobal.instance.SendMessageRealTime (messageSetParent);
	}

	public static void SendMessageCallBotUno(byte _numBot){
		if (messageCallBotUno == null) {
			messageCallBotUno = new MessageSending (CMD_REALTIME.C_GAMEPLAY_INVITE_ROBOT);
		} else {
			messageCallBotUno.ClearData ();
		}

		messageCallBotUno.writeByte (_numBot);

		#if TEST
		string _tmp = string.Empty;
		_tmp += _numBot;
		Debug.Log(">>>CMD SendMessageCallBotUno : " + messageCallBotUno.getCMD() + "|" + _tmp);
		#endif

		NetworkGlobal.instance.SendMessageRealTime (messageCallBotUno);
	}
}
