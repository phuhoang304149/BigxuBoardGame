using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OneHitAPI  {

	static MessageSending messageGetSplashData_Android;
	static MessageSending messageGetSplashData_IOS;

	static MessageSending messageLoginWithLocalAccount;
	static MessageSending messageLoginWithDeviceID;
	static MessageSending messageLoginWithFacebookAccount;
	static MessageSending messageRegisterLocalAccount;

	static MessageSending message_BigxuAccount_ChangePassword;
	static MessageSending message_BigxuAccount_SetEmailSecurity;
	static MessageSending message_BigxuAccount_GetEmailSecurity;
	static MessageSending message_BigxuAccount_ForgotPassword;

	static MessageSending messageGetListTableByGameId;
	static MessageSending messageGetUserDetail;
	static MessageSending messageGetTopGold;
	static MessageSending messageChooseAvatar;
	static MessageSending messageGetGoldDaily;
	static MessageSending messageInviteFriend_SearchParentInfo;
	static MessageSending messageGetGoldSubsidy;
	static MessageSending messageGetGoldGemById;
	static MessageSending messageIAP_Android;
	static MessageSending messageTestIAP_Android;
	static MessageSending message_GetList_CampagneInstallAndroid;
	static MessageSending message_Forward_Bonus_AndroidInstall_Commit;
	static MessageSending message_BetToWin;


	#region Get Splash Screen Data
	public static void GetSplashData_Android(short _languageId, string _packageName, System.Action<MessageReceiving, int> _onFinished){
		if(CoreGameManager.instance.giaLapNgatKetNoi){
			if(_onFinished != null){
				_onFinished(null, 1);
			}
			return;
		}
		if(CoreGameManager.instance.giaLapMangChapChon){
			if(Random.Range(0, 100) < CoreGameManager.instance.giaLapTyLeRotMang){
				if(_onFinished != null){
					_onFinished(null, 1);
				}
				return;
			}
		}
		if (messageGetSplashData_Android == null) {
			messageGetSplashData_Android = new MessageSending (CMD_ONEHIT.SPLASH_Android);
		} else {
			messageGetSplashData_Android.ClearData ();
		}

		messageGetSplashData_Android.writeshort (_languageId);
		messageGetSplashData_Android.writeString (_packageName);

		#if TEST
		string _tmp = string.Empty; 
		_tmp += _languageId + "|" + _packageName;
		Debug.Log(">>>CMD GetSplashData_Android : " + messageGetSplashData_Android.getCMD() + "|" + _tmp);
		#endif
		NetworkGlobal.instance.StartOnehit (messageGetSplashData_Android,_onFinished);
	}

	public static void GetSplashData_IOS(short _languageId, string _packageName, System.Action<MessageReceiving, int> _onFinished){
		if(CoreGameManager.instance.giaLapNgatKetNoi){
			if(_onFinished != null){
				_onFinished(null, 1);
			}
			return;
		}
		if(CoreGameManager.instance.giaLapMangChapChon){
			if(Random.Range(0, 100) < CoreGameManager.instance.giaLapTyLeRotMang){
				if(_onFinished != null){
					_onFinished(null, 1);
				}
				return;
			}
		}
		if (messageGetSplashData_IOS == null) {
			messageGetSplashData_IOS = new MessageSending (CMD_ONEHIT.SPLASH_IOS);
		} else {
			messageGetSplashData_IOS.ClearData ();
		}

		messageGetSplashData_IOS.writeshort (_languageId);
		messageGetSplashData_IOS.writeString (_packageName);

		#if TEST
		string _tmp = string.Empty; 
		_tmp += _languageId + "|" + _packageName;
		Debug.Log(">>>CMD GetSplashData_IOS : " + messageGetSplashData_IOS.getCMD() + "|" + _tmp);
		#endif
		NetworkGlobal.instance.StartOnehit (messageGetSplashData_IOS, _onFinished);
	}
	#endregion

	#region Login API
	public static void LoginWithLocalAccount(long _userId, string _userName, string _pass, System.Action<MessageReceiving, int> _onFinished){
		if(CoreGameManager.instance.giaLapNgatKetNoi){
			if(_onFinished != null){
				_onFinished(null, 1);
			}
			return;
		}
		if(CoreGameManager.instance.giaLapMangChapChon){
			if(Random.Range(0, 100) < CoreGameManager.instance.giaLapTyLeRotMang){
				if(_onFinished != null){
					_onFinished(null, 1);
				}
				return;
			}
		}
		if (messageLoginWithLocalAccount == null) {
			messageLoginWithLocalAccount = new MessageSending (CMD_ONEHIT.Game_login_1_bigxu_account);
		} else {
			messageLoginWithLocalAccount.ClearData ();
		}
		messageLoginWithLocalAccount.writeLong (_userId);
		messageLoginWithLocalAccount.writeString (_userName);
		messageLoginWithLocalAccount.writeString (_pass);

		#if TEST
		Debug.Log(">>>CMD LoginWithLocalAccount : " + messageLoginWithLocalAccount.getCMD() + "|" + _userId + "|" + _userName + "|" + _pass);
		#endif

		NetworkGlobal.instance.StartOnehit (messageLoginWithLocalAccount, _onFinished);
	}

	public static void LoginWithDeviceID(long _userId, System.Action<MessageReceiving, int> _onFinished){
		if(CoreGameManager.instance.giaLapNgatKetNoi){
			if(_onFinished != null){
				_onFinished(null, 1);
			}
			return;
		}
		if(CoreGameManager.instance.giaLapMangChapChon){
			if(Random.Range(0, 100) < CoreGameManager.instance.giaLapTyLeRotMang){
				if(_onFinished != null){
					_onFinished(null, 1);
				}
				return;
			}
		}
		if (messageLoginWithDeviceID == null) {
			messageLoginWithDeviceID = new MessageSending (CMD_ONEHIT.Game_login_0_device_account);
		} else {
			messageLoginWithDeviceID.ClearData ();
		}
		string _deviceId = SystemInfo.deviceUniqueIdentifier;

		messageLoginWithDeviceID.writeLong (_userId);
		messageLoginWithDeviceID.writeString (_deviceId);
		messageLoginWithDeviceID.writeString (SystemInfo.deviceModel);

		#if TEST
		Debug.Log(">>>CMD LoginWithDeviceID : " + messageLoginWithDeviceID.getCMD() + "|" + _userId + "|" + _deviceId + "|" + SystemInfo.deviceModel);
		#endif

		NetworkGlobal.instance.StartOnehit (messageLoginWithDeviceID, _onFinished);
	}

	public static void LoginWithFacebookAccount(long _userId, string _fbToken, System.Action<MessageReceiving, int> _onFinished){
		if(CoreGameManager.instance.giaLapNgatKetNoi){
			if(_onFinished != null){
				_onFinished(null, 1);
			}
			return;
		}
		if(CoreGameManager.instance.giaLapMangChapChon){
			if(Random.Range(0, 100) < CoreGameManager.instance.giaLapTyLeRotMang){
				if(_onFinished != null){
					_onFinished(null, 1);
				}
				return;
			}
		}
		if (messageLoginWithFacebookAccount == null) {
			messageLoginWithFacebookAccount = new MessageSending (CMD_ONEHIT.Game_login_2_facebookid);
		} else {
			messageLoginWithFacebookAccount.ClearData ();
		}

		messageLoginWithFacebookAccount.writeLong (_userId);
		messageLoginWithFacebookAccount.writeString (_fbToken);

		#if TEST
		Debug.Log(">>>CMD LoginWithFacebookAccount : " + messageLoginWithFacebookAccount.getCMD() + "|" + _userId + "|" + _fbToken);
		#endif

		NetworkGlobal.instance.StartOnehit (messageLoginWithFacebookAccount, _onFinished);
	}
	#endregion

	public static void RegisterLocalAccount(string _userName, string _pass, System.Action<MessageReceiving, int> _onFinished){
		if(CoreGameManager.instance.giaLapNgatKetNoi){
			if(_onFinished != null){
				_onFinished(null, 1);
			}
			return;
		}
		if(CoreGameManager.instance.giaLapMangChapChon){
			if(Random.Range(0, 100) < CoreGameManager.instance.giaLapTyLeRotMang){
				if(_onFinished != null){
					_onFinished(null, 1);
				}
				return;
			}
		}
		if (messageRegisterLocalAccount == null) {
			messageRegisterLocalAccount = new MessageSending (CMD_ONEHIT.Game_Forward_BigxuAccount_Register);
		} else {
			messageRegisterLocalAccount.ClearData ();
		}
		messageRegisterLocalAccount.writeString (_userName);
		messageRegisterLocalAccount.writeString (_pass);

		#if TEST
		Debug.Log(">>>CMD Register : " + messageRegisterLocalAccount.getCMD() + "|" + _userName + "|" + _pass);
		#endif

		NetworkGlobal.instance.StartOnehit (messageRegisterLocalAccount, _onFinished);
	}

	public static void BigxuAccount_ChangePassword(string _currentPass, string _newPass, System.Action<MessageReceiving, int> _onFinished){
		if(CoreGameManager.instance.giaLapNgatKetNoi){
			if(_onFinished != null){
				_onFinished(null, 1);
			}
			return;
		}
		if(CoreGameManager.instance.giaLapMangChapChon){
			if(Random.Range(0, 100) < CoreGameManager.instance.giaLapTyLeRotMang){
				if(_onFinished != null){
					_onFinished(null, 1);
				}
				return;
			}
		}
		if (message_BigxuAccount_ChangePassword == null) {
			message_BigxuAccount_ChangePassword = new MessageSending (CMD_ONEHIT.Game_Forward_BigxuAccount_ChangePassword);
		} else {
			message_BigxuAccount_ChangePassword.ClearData ();
		}
		message_BigxuAccount_ChangePassword.writeLong (DataManager.instance.userData.userId);
		message_BigxuAccount_ChangePassword.writeString (_currentPass);
		message_BigxuAccount_ChangePassword.writeString (DataManager.instance.userData.username);
		message_BigxuAccount_ChangePassword.writeString (_newPass);

		#if TEST
		string _tmp = DataManager.instance.userData.userId + "|"
			+ DataManager.instance.userData.username + "|"
			+ _currentPass + "|"
			+ _newPass;
		Debug.Log(">>>CMD BigxuAccount_ChangePassword : " + message_BigxuAccount_ChangePassword.getCMD() + "|" + _tmp);
		#endif

		NetworkGlobal.instance.StartOnehit (message_BigxuAccount_ChangePassword, _onFinished);
	}

	public static void BigxuAccount_SetEmailSecurity(string _email, System.Action<MessageReceiving, int> _onFinished){
		if(CoreGameManager.instance.giaLapNgatKetNoi){
			if(_onFinished != null){
				_onFinished(null, 1);
			}
			return;
		}
		if(CoreGameManager.instance.giaLapMangChapChon){
			if(Random.Range(0, 100) < CoreGameManager.instance.giaLapTyLeRotMang){
				if(_onFinished != null){
					_onFinished(null, 1);
				}
				return;
			}
		}
		if (message_BigxuAccount_SetEmailSecurity == null) {
			message_BigxuAccount_SetEmailSecurity = new MessageSending (CMD_ONEHIT.Game_Forward_BigxuAccount_SetEmailSecurity);
		} else {
			message_BigxuAccount_SetEmailSecurity.ClearData ();
		}
		message_BigxuAccount_SetEmailSecurity.writeLong (DataManager.instance.userData.userId);
		message_BigxuAccount_SetEmailSecurity.writeString (DataManager.instance.userData.username);
		message_BigxuAccount_SetEmailSecurity.writeString (DataManager.instance.userData.password);
		message_BigxuAccount_SetEmailSecurity.writeString (_email);

		#if TEST
		string _tmp = DataManager.instance.userData.userId + "|"
			+ DataManager.instance.userData.username + "|"
			+ DataManager.instance.userData.password + "|"
			+ _email;
		Debug.Log(">>>CMD BigxuAccount_SetEmailSecurity : " + message_BigxuAccount_SetEmailSecurity.getCMD() + "|" + _tmp);
		#endif

		NetworkGlobal.instance.StartOnehit (message_BigxuAccount_SetEmailSecurity, _onFinished);
	}

	public static void BigxuAccount_GetEmailSecurity(System.Action<MessageReceiving, int> _onFinished){
		if(CoreGameManager.instance.giaLapNgatKetNoi){
			if(_onFinished != null){
				_onFinished(null, 1);
			}
			return;
		}
		if(CoreGameManager.instance.giaLapMangChapChon){
			if(Random.Range(0, 100) < CoreGameManager.instance.giaLapTyLeRotMang){
				if(_onFinished != null){
					_onFinished(null, 1);
				}
				return;
			}
		}
		if (message_BigxuAccount_GetEmailSecurity == null) {
			message_BigxuAccount_GetEmailSecurity = new MessageSending (CMD_ONEHIT.Game_Forward_BigxuAccount_GetEmailSecurity);
		} else {
			message_BigxuAccount_GetEmailSecurity.ClearData ();
		}
		message_BigxuAccount_GetEmailSecurity.writeLong (DataManager.instance.userData.userId);

		#if TEST
		string _tmp = DataManager.instance.userData.userId.ToString();
		Debug.Log(">>>CMD BigxuAccount_GetEmailSecurity : " + message_BigxuAccount_GetEmailSecurity.getCMD() + "|" + _tmp);
		#endif

		NetworkGlobal.instance.StartOnehit (message_BigxuAccount_GetEmailSecurity, _onFinished);
	}

	public static void BigxuAccount_ForgotPassword(string _email, string _userName, string _newPass, System.Action<MessageReceiving, int> _onFinished){
		if(CoreGameManager.instance.giaLapNgatKetNoi){
			if(_onFinished != null){
				_onFinished(null, 1);
			}
			return;
		}
		if(CoreGameManager.instance.giaLapMangChapChon){
			if(Random.Range(0, 100) < CoreGameManager.instance.giaLapTyLeRotMang){
				if(_onFinished != null){
					_onFinished(null, 1);
				}
				return;
			}
		}
		if (message_BigxuAccount_ForgotPassword == null) {
			message_BigxuAccount_ForgotPassword = new MessageSending (CMD_ONEHIT.Game_Forward_BigxuAccount_Backup_By_EmailSecurity);
		} else {
			message_BigxuAccount_ForgotPassword.ClearData ();
		}
		message_BigxuAccount_ForgotPassword.writeString (_email);
		message_BigxuAccount_ForgotPassword.writeString (_userName);
		message_BigxuAccount_ForgotPassword.writeString (_newPass);

		#if TEST
		string _tmp = _email + "|" + _userName + "|" + _newPass;
		Debug.Log(">>>CMD BigxuAccount_ForgotPassword : " + message_BigxuAccount_ForgotPassword.getCMD() + "|" + _tmp);
		#endif

		NetworkGlobal.instance.StartOnehit (message_BigxuAccount_ForgotPassword, _onFinished);
	}

	public static void GetListTableByGameID(short _gameId, short _indexBegin, short _numberTableGet, SubServerDetail _serverDetail, System.Action<MessageReceiving, int> _onFinished){
		if(CoreGameManager.instance.giaLapNgatKetNoi){
			if(_onFinished != null){
				_onFinished(null, 1);
			}
			return;
		}
		if(CoreGameManager.instance.giaLapMangChapChon){
			if(Random.Range(0, 100) < CoreGameManager.instance.giaLapTyLeRotMang){
				if(_onFinished != null){
					_onFinished(null, 1);
				}
				return;
			}
		}
		if (messageGetListTableByGameId == null) {
			messageGetListTableByGameId = new MessageSending (CMD_ONEHIT.Game_RoomTable_GetListTable_By_gameid);
		} else {
			messageGetListTableByGameId.ClearData ();
		}
		
		messageGetListTableByGameId.writeshort (_gameId);
		messageGetListTableByGameId.writeshort (_indexBegin);
		messageGetListTableByGameId.writeshort (_numberTableGet);
		
		#if TEST
		string _tmp = string.Empty;
		_tmp += _gameId + "|" + _indexBegin + "|" + _numberTableGet;
		Debug.Log(">>>CMD GetListTableByGameID : " + messageGetListTableByGameId.getCMD() + "|" + _tmp);
		#endif

		NetworkGlobal.instance.StartOnehit (messageGetListTableByGameId, _serverDetail, _onFinished);
	}

	public static void GetUserDetail(sbyte _databaseid, long _userid, List<short> _listGameId, System.Action<MessageReceiving, int> _onFinished){
		if(CoreGameManager.instance.giaLapNgatKetNoi){
			if(_onFinished != null){
				_onFinished(null, 1);
			}
			return;
		}
		if(CoreGameManager.instance.giaLapMangChapChon){
			if(Random.Range(0, 100) < CoreGameManager.instance.giaLapTyLeRotMang){
				if(_onFinished != null){
					_onFinished(null, 1);
				}
				return;
			}
		}
		if (messageGetUserDetail == null) {
			messageGetUserDetail = new MessageSending (CMD_ONEHIT.Game_Global_Get_BasePlayerInfo_And_FullAchievement_ByUserid);
		} else {
			messageGetUserDetail.ClearData ();
		}
		string _tmp = string.Empty;
		messageGetUserDetail.writeByte ((byte)_databaseid);
		messageGetUserDetail.writeLong (_userid);
		messageGetUserDetail.writeshort ((short)_listGameId.Count);
		_tmp += _databaseid + "|" + _userid + "|" + _listGameId.Count + "|";
		for(int i = 0; i < _listGameId.Count; i++){
			messageGetUserDetail.writeshort (_listGameId[i]);
			_tmp += _listGameId [i] + "_";
			if (i + 1 < _listGameId.Count) {
				_tmp += "_";
			}
		}

		#if TEST
		Debug.Log(">>>CMD GetUserDetail : " + messageGetUserDetail.getCMD() + "|" + _tmp);
		#endif

		NetworkGlobal.instance.StartOnehit (messageGetUserDetail, _onFinished);
	}

	public static void ChooseAvatar(UserData.DatabaseType _databaseid, long _userid, sbyte _avatarId, System.Action<MessageReceiving, int> _onFinished){
		if(CoreGameManager.instance.giaLapNgatKetNoi){
			if(_onFinished != null){
				_onFinished(null, 1);
			}
			return;
		}
		if(CoreGameManager.instance.giaLapMangChapChon){
			if(Random.Range(0, 100) < CoreGameManager.instance.giaLapTyLeRotMang){
				if(_onFinished != null){
					_onFinished(null, 1);
				}
				return;
			}
		}
		if (messageChooseAvatar == null) {
			messageChooseAvatar = new MessageSending (CMD_ONEHIT.Game_Change_Avatarid);
		} else {
			messageChooseAvatar.ClearData ();
		}
		messageChooseAvatar.writeByte ((byte)_databaseid);
		messageChooseAvatar.writeLong (_userid);
		messageChooseAvatar.writeByte ((byte)_avatarId);
		string _tmp = _databaseid + "|" + _userid + "|" + _avatarId;

		#if TEST
		Debug.Log(">>>CMD ChooseAvatar : " + messageChooseAvatar.getCMD() + "|" + _tmp);
		#endif

		NetworkGlobal.instance.StartOnehit (messageChooseAvatar, _onFinished);
	}

	public static void GetTopGold(System.Action<MessageReceiving, int> _onFinished){
		if(CoreGameManager.instance.giaLapNgatKetNoi){
			if(_onFinished != null){
				_onFinished(null, 1);
			}
			return;
		}
		if(CoreGameManager.instance.giaLapMangChapChon){
			if(Random.Range(0, 100) < CoreGameManager.instance.giaLapTyLeRotMang){
				if(_onFinished != null){
					_onFinished(null, 1);
				}
				return;
			}
		}
		if (messageGetTopGold == null) {
			messageGetTopGold = new MessageSending (CMD_ONEHIT.GameDatabase_Top_GetTopGold);
		} else {
			messageGetTopGold.ClearData ();
		}
		
		#if TEST
		Debug.Log(">>>CMD GetTopGold : " + messageGetTopGold.getCMD());
		#endif

		NetworkGlobal.instance.StartOnehit (messageGetTopGold, _onFinished);
	}

	public static void GetGoldDaily(SubServerDetail _serverDetail, System.Action<MessageReceiving, int> _onFinished){
		if(CoreGameManager.instance.giaLapNgatKetNoi){
			if(_onFinished != null){
				_onFinished(null, 1);
			}
			return;
		}
		if(CoreGameManager.instance.giaLapMangChapChon){
			if(Random.Range(0, 100) < CoreGameManager.instance.giaLapTyLeRotMang){
				if(_onFinished != null){
					_onFinished(null, 1);
				}
				return;
			}
		}
		if (messageGetGoldDaily == null) {
			messageGetGoldDaily = new MessageSending (CMD_ONEHIT.GameBonus_Get_Gold_Daily);
		} else {
			messageGetGoldDaily.ClearData ();
		}
		messageGetGoldDaily.writeByte ((byte)DataManager.instance.userData.databaseId);
		messageGetGoldDaily.writeLong (DataManager.instance.userData.userId);
		string _tmp = DataManager.instance.userData.databaseId + "|" + DataManager.instance.userData.userId;

		#if TEST
		Debug.Log(">>>CMD GetGoldDaily : " + messageGetGoldDaily.getCMD() + "|" + _tmp);
		#endif

		if(_serverDetail == null){
			NetworkGlobal.instance.StartOnehit (messageGetGoldDaily, _onFinished);
		}else{
			NetworkGlobal.instance.StartOnehit (messageGetGoldDaily, _serverDetail, _onFinished);
		}
	}

	public static void GetGoldSubsidy(SubServerDetail _serverDetail, System.Action<MessageReceiving, int> _onFinished){
		if(CoreGameManager.instance.giaLapNgatKetNoi){
			if(_onFinished != null){
				_onFinished(null, 1);
			}
			return;
		}
		if(CoreGameManager.instance.giaLapMangChapChon){
			if(Random.Range(0, 100) < CoreGameManager.instance.giaLapTyLeRotMang){
				if(_onFinished != null){
					_onFinished(null, 1);
				}
				return;
			}
		}
		if (messageGetGoldSubsidy == null) {
			messageGetGoldSubsidy = new MessageSending (CMD_ONEHIT.GameBonus_Get_Gold_Subsidy);
		} else {
			messageGetGoldSubsidy.ClearData ();
		}
		messageGetGoldSubsidy.writeByte ((byte)DataManager.instance.userData.databaseId);
		messageGetGoldSubsidy.writeLong (DataManager.instance.userData.userId);

		#if TEST
		string _tmp = DataManager.instance.userData.databaseId + "|" + DataManager.instance.userData.userId;
		Debug.Log(">>>CMD GetGoldSubsidy : " + messageGetGoldSubsidy.getCMD() + "|" + _tmp);
		#endif

		if(_serverDetail == null){
			NetworkGlobal.instance.StartOnehit (messageGetGoldSubsidy, _onFinished);
		}else{
			NetworkGlobal.instance.StartOnehit (messageGetGoldSubsidy, _serverDetail, _onFinished);
		}
	}

	public static void InviteFriend_SearchParentInfo(SubServerDetail _serverDetail, short _parentSessionId, System.Action<MessageReceiving, int> _onFinished){
		if(CoreGameManager.instance.giaLapNgatKetNoi){
			if(_onFinished != null){
				_onFinished(null, 1);
			}
			return;
		}
		if(CoreGameManager.instance.giaLapMangChapChon){
			if(Random.Range(0, 100) < CoreGameManager.instance.giaLapTyLeRotMang){
				if(_onFinished != null){
					_onFinished(null, 1);
				}
				return;
			}
		}
		if (messageInviteFriend_SearchParentInfo == null) {
			messageInviteFriend_SearchParentInfo = new MessageSending (CMD_ONEHIT.GameGlobal_GetMiniInfo_BySessionid);
		} else {
			messageInviteFriend_SearchParentInfo.ClearData ();
		}
		messageInviteFriend_SearchParentInfo.writeshort (_parentSessionId);

		#if TEST
		string _tmp = _parentSessionId.ToString();
		Debug.Log(">>>CMD InviteFriend_SearchParentInfo : " + messageInviteFriend_SearchParentInfo.getCMD() + "|" + _tmp);
		#endif

		if(_serverDetail == null){
			NetworkGlobal.instance.StartOnehit (messageInviteFriend_SearchParentInfo, _onFinished);
		}else{
			NetworkGlobal.instance.StartOnehit (messageInviteFriend_SearchParentInfo, _serverDetail, _onFinished);
		}
	}

	public static void GetGoldGemById(UserData.DatabaseType _databaseid, long _userid, System.Action<MessageReceiving, int> _onFinished){
		if(CoreGameManager.instance.giaLapNgatKetNoi){
			if(_onFinished != null){
				_onFinished(null, 1);
			}
			return;
		}
		if(CoreGameManager.instance.giaLapMangChapChon){
			if(Random.Range(0, 100) < CoreGameManager.instance.giaLapTyLeRotMang){
				if(_onFinished != null){
					_onFinished(null, 1);
				}
				return;
			}
		}
		if (messageGetGoldGemById == null) {
			messageGetGoldGemById = new MessageSending (CMD_ONEHIT.Game_Get_Gold_By_Id);
		} else {
			messageGetGoldGemById.ClearData ();
		}
		messageGetGoldGemById.writeByte ((byte)_databaseid);
		messageGetGoldGemById.writeLong (_userid);
		string _tmp = _databaseid + "|" + _userid;

		#if TEST
		Debug.Log(">>>CMD GetGoldGemById : " + messageGetGoldGemById.getCMD() + "|" + _tmp);
		#endif

		NetworkGlobal.instance.StartOnehit (messageGetGoldGemById, _onFinished);
	}
	
	public static void IAP_Android(SubServerDetail _serverDetail, byte _screenPurchase, string _productId, string _tokenPurchase, System.Action<MessageReceiving, int> _onFinished){
		if(CoreGameManager.instance.giaLapNgatKetNoi){
			if(_onFinished != null){
				_onFinished(null, 1);
			}
			return;
		}
		if(CoreGameManager.instance.giaLapMangChapChon){
			if(Random.Range(0, 100) < CoreGameManager.instance.giaLapTyLeRotMang){
				if(_onFinished != null){
					_onFinished(null, 1);
				}
				return;
			}
		}
		if (messageIAP_Android == null) {
			messageIAP_Android = new MessageSending (CMD_ONEHIT.GamePurchase_inapp_android);
		} else {
			messageIAP_Android.ClearData ();
		}

		messageIAP_Android.writeByte((byte) DataManager.instance.userData.databaseId);
		messageIAP_Android.writeLong(DataManager.instance.userData.userId);
		messageIAP_Android.writeByte(_screenPurchase);
		messageIAP_Android.writeString(Application.identifier);
		messageIAP_Android.writeString(_productId);
		messageIAP_Android.writeString(_tokenPurchase);

		#if TEST
		string _tmp = DataManager.instance.userData.databaseId + "|" 
					+ DataManager.instance.userData.userId + "|" 
					+ _screenPurchase + "|" 
					+ Application.identifier + "|" 
					+ _productId + "|" 
					+ _tokenPurchase;
		Debug.Log(">>>CMD IAP_Android : " + messageIAP_Android.getCMD() + "|" + _tmp);
		#endif

		if(_serverDetail == null){
			NetworkGlobal.instance.StartOnehit (messageIAP_Android, _onFinished);
		}else{
			NetworkGlobal.instance.StartOnehit (messageIAP_Android, _serverDetail, _onFinished);
		}
	}

	public static void TESTIAP(SubServerDetail _serverDetail, System.Action<MessageReceiving, int> _onFinished){
		// gửi sbyte databaseid
		// long userid
		if(CoreGameManager.instance.giaLapNgatKetNoi){
			if(_onFinished != null){
				_onFinished(null, 1);
			}
			return;
		}
		if(CoreGameManager.instance.giaLapMangChapChon){
			if(Random.Range(0, 100) < CoreGameManager.instance.giaLapTyLeRotMang){
				if(_onFinished != null){
					_onFinished(null, 1);
				}
				return;
			}
		}
		if (messageTestIAP_Android == null) {
			messageTestIAP_Android = new MessageSending (CMD_ONEHIT.TEST_ADD_GOLD);
		} else {
			messageTestIAP_Android.ClearData ();
		}

		messageTestIAP_Android.writeByte((byte) DataManager.instance.userData.databaseId);
		messageTestIAP_Android.writeLong(DataManager.instance.userData.userId);

		#if TEST
		string _tmp = DataManager.instance.userData.databaseId + "|" 
					+ DataManager.instance.userData.userId;
		Debug.Log(">>>CMD IAP_Android : " + messageTestIAP_Android.getCMD() + "|" + _tmp);
		#endif

		if(_serverDetail == null){
			NetworkGlobal.instance.StartOnehit (messageTestIAP_Android, _onFinished);
		}else{
			NetworkGlobal.instance.StartOnehit (messageTestIAP_Android, _serverDetail, _onFinished);
		}
	}

	public static void GetListCampagneInstallAndroid(System.Action<MessageReceiving, int> _onFinished){
		if(CoreGameManager.instance.giaLapNgatKetNoi){
			if(_onFinished != null){
				_onFinished(null, 1);
			}
			return;
		}
		if(CoreGameManager.instance.giaLapMangChapChon){
			if(Random.Range(0, 100) < CoreGameManager.instance.giaLapTyLeRotMang){
				if(_onFinished != null){
					_onFinished(null, 1);
				}
				return;
			}
		}
		if (message_GetList_CampagneInstallAndroid == null) {
			message_GetList_CampagneInstallAndroid = new MessageSending (CMD_ONEHIT.GameDatabase_Android_GetCampagneInstall);
		} else {
			message_GetList_CampagneInstallAndroid.ClearData ();
		}

		#if TEST
		Debug.Log(">>>CMD GetListCampagneInstallAndroid : " + message_GetList_CampagneInstallAndroid.getCMD());
		#endif

		NetworkGlobal.instance.StartOnehit (message_GetList_CampagneInstallAndroid, _onFinished);
	}

	public static void Forward_Bonus_AndroidInstall_Commit(string _packageName, System.Action<MessageReceiving, int> _onFinished){
		if(CoreGameManager.instance.giaLapNgatKetNoi){
			if(_onFinished != null){
				_onFinished(null, 1);
			}
			return;
		}
		if(CoreGameManager.instance.giaLapMangChapChon){
			if(Random.Range(0, 100) < CoreGameManager.instance.giaLapTyLeRotMang){
				if(_onFinished != null){
					_onFinished(null, 1);
				}
				return;
			}
		}
		if (message_Forward_Bonus_AndroidInstall_Commit == null) {
			message_Forward_Bonus_AndroidInstall_Commit = new MessageSending (CMD_ONEHIT.GameMission_AndroidInstall_Commit);
		} else {
			message_Forward_Bonus_AndroidInstall_Commit.ClearData ();
		}
		message_Forward_Bonus_AndroidInstall_Commit.writeByte((byte)DataManager.instance.userData.databaseId);
   		message_Forward_Bonus_AndroidInstall_Commit.writeLong(DataManager.instance.userData.userId);
		message_Forward_Bonus_AndroidInstall_Commit.writeString(_packageName);

		#if TEST
		string _tmp = DataManager.instance.userData.databaseId + "|" 
					+ DataManager.instance.userData.userId + "|" 
					+ _packageName;
		Debug.Log(">>>CMD Forward_Bonus_AndroidInstall_Commit : " + message_Forward_Bonus_AndroidInstall_Commit.getCMD() + "|" + _tmp);
		#endif

		NetworkGlobal.instance.StartOnehit (message_Forward_Bonus_AndroidInstall_Commit, _onFinished);
	}

	public static void BetToWin(long _bet
			, List<BetToWinValueDetail> _listBetToWinInfo
			, List<long> _listBetToWinValue
			, System.Action<MessageReceiving, int> _onFinished){
		if(CoreGameManager.instance.giaLapNgatKetNoi){
			if(_onFinished != null){
				_onFinished(null, 1);
			}
			return;
		}
		if(CoreGameManager.instance.giaLapMangChapChon){
			if(Random.Range(0, 100) < CoreGameManager.instance.giaLapTyLeRotMang){
				if(_onFinished != null){
					_onFinished(null, 1);
				}
				return;
			}
		}
		if (message_BetToWin == null) {
			message_BetToWin = new MessageSending (CMD_ONEHIT.Game_BetToWin_NoAchievement);
		} else {
			message_BetToWin.ClearData ();
		}
		message_BetToWin.writeLong(_bet);
		message_BetToWin.writeshort((short) _listBetToWinInfo.Count);
		for(int i = 0; i < _listBetToWinInfo.Count; i ++){
			message_BetToWin.writeInt(_listBetToWinInfo[i].weight);
			message_BetToWin.writeLong(_listBetToWinValue[i]);
		}
		message_BetToWin.writeByte((byte)DataManager.instance.userData.databaseId);
   		message_BetToWin.writeLong(DataManager.instance.userData.userId);
		switch (DataManager.instance.userData.databaseId) {
		case UserData.DatabaseType.DATABASEID_DEVICE:
			message_BetToWin.writeString (DataManager.instance.userData.deviceId);
			// message_BetToWin.writeString (SystemInfo.deviceModel);
			break;
		case UserData.DatabaseType.DATABASEID_BIGXU:
			message_BetToWin.writeString (DataManager.instance.userData.username);
			message_BetToWin.writeString (DataManager.instance.userData.password);
			break;
		case UserData.DatabaseType.DATABASEID_FACEBOOK:
			message_BetToWin.writeString(DataManager.instance.userData.tokenFBIdOfBusiness);
			break;
		default:
			#if TEST
			Debug.LogError(">>> Lỗi databaseId: " + DataManager.instance.userData.databaseId);
			#endif
			break;
		}

		#if TEST
		string _tmp = _bet + "|"
					+ DataManager.instance.userData.databaseId + "|" 
					+ DataManager.instance.userData.userId + "|" ;
		Debug.Log(">>>CMD BetToWin : " + message_BetToWin.getCMD() + "|" + _tmp);
		#endif

		NetworkGlobal.instance.StartOnehit (message_BetToWin, _onFinished);
	}
}
