using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook;
using Facebook.Unity;

[System.Serializable]
public class UserData {

	public const int maxLengthListAvatarID = 28;

	// public const sbyte DATABASEID_DEVICE = 0;
	// public const sbyte DATABASEID_BIGXU = 1;
	// public const sbyte DATABASEID_FACEBOOK = 2;
	// public const sbyte DATABASEID_GOOGLE = 3;
	// public const sbyte DATABASEID_TWITTER = 4;
	// public const sbyte DATABASEID_ZALO = 5;
	// public const sbyte DATABASEID_CHINA = 6;

	public enum DatabaseType {
		DATABASEID_DEVICE = 0,
		DATABASEID_BIGXU = 1,
		DATABASEID_FACEBOOK = 2,
		DATABASEID_GOOGLE = 3,
		DATABASEID_TWITTER = 4,
		DATABASEID_ZALO = 5,
		DATABASEID_CHINA = 6,
		UNKNOWN
	}

    public short sessionId{
		get{
			return _sessionId;
		}set{
			_sessionId = value;
		}
	}
	[System.NonSerialized] protected short _sessionId;

	public DatabaseType databaseId;
	public long userId;
	public string username;
	public string password;
	public string nameShowInGame;
	public sbyte avatarid;
	public string emailShow;

	public string deviceId;

	public long facebookId;
	public string tokenFBIdOfBusiness;

	public string googleData_mail;
	public string googleData_linkAvatar;

	public Texture2D myAvatar{ 
		get{
			return _myAvatar;
		}set{
			_myAvatar = value;
		}
	}
	[System.NonSerialized] private Texture2D _myAvatar;

	public Texture2D myAvatarDownloaded{ // avatar download từ facebook hoặc google
		get{
			return _myAvatarDownloaded;
		}set{
			_myAvatarDownloaded = value;
		}
	}
	[System.NonSerialized] private Texture2D _myAvatarDownloaded;

	public long gold;

	/// <summary>
	/// TotalBetInGameInfo: quản lý các tiền cược trong các game cược tiền như đua thú, long hổ, bầu cua
	/// </summary>
	public class TotalBetInGameInfo{ 
		public IMiniGameInfo.Type gameType;
		public long totalBet;

		public TotalBetInGameInfo(IMiniGameInfo.Type _gameType){
			gameType = _gameType;
			totalBet = 0;
		}
	}
	[System.NonSerialized] public List<TotalBetInGameInfo> listTotalBetInGameInfo;

	public long timeCreateAccount;
	public long lastTimePlay;
	public bool isInitialized;
	
	public UserData(){}

	public void InitData(){
		// #if TEST
		// Debug.Log ("--- Init UserData! ---");
        // #endif
        userId = 0; // chưa đăng nhập lần nào
		username = string.Empty;
		nameShowInGame = "Unknown";
		password = string.Empty;
		
		deviceId = string.Empty;
		facebookId = -1;
		tokenFBIdOfBusiness = string.Empty;
		emailShow = string.Empty;

		googleData_mail = string.Empty;
		googleData_linkAvatar = string.Empty;

		gold = 0;
		isInitialized = true;
	}

	public void GetMoreUserData(MessageReceiving _messageReceiving){
		databaseId = (DatabaseType) _messageReceiving.readByte();
		switch (databaseId){
		case UserData.DatabaseType.DATABASEID_DEVICE:      
			deviceId = _messageReceiving.readString();
			string _device_model = _messageReceiving.readString();
			break;
		case UserData.DatabaseType.DATABASEID_BIGXU:
			username = _messageReceiving.readString();
			break;
		case UserData.DatabaseType.DATABASEID_FACEBOOK:
			facebookId = _messageReceiving.readLong();
			tokenFBIdOfBusiness = _messageReceiving.readString();
			break;
		case UserData.DatabaseType.DATABASEID_GOOGLE:
			googleData_mail = _messageReceiving.readString();
			googleData_linkAvatar = _messageReceiving.readString();
			break;
		}

		userId = _messageReceiving.readLong ();
		nameShowInGame = _messageReceiving.readString ();
		if (string.IsNullOrEmpty (nameShowInGame)) {
			nameShowInGame = "Unknown";
		}
		avatarid = _messageReceiving.readByte ();
        gold = _messageReceiving.readLong ();
		timeCreateAccount = _messageReceiving.readLong();
		lastTimePlay = _messageReceiving.readLong ();
	}

	public Coroutine LoadAvatarFromLink(MonoBehaviour _monoBehaviour, float _w, float _h, System.Action<Texture2D> _onFinished = null){
		if(_monoBehaviour == null){
			return null;
		}
		string _uri = string.Empty;
		switch(databaseId){
		case UserData.DatabaseType.DATABASEID_FACEBOOK:
			_uri = "https://graph.facebook.com/" + facebookId + "/picture?width=" + _w + "&height=" + _h;
			break;
		case UserData.DatabaseType.DATABASEID_GOOGLE:
			_uri = googleData_linkAvatar;
			break;
		default:
			return null;
		}
		return _monoBehaviour.StartCoroutine(DoActionLoadAvatarFromLink(_monoBehaviour, _uri, _w, _h, _onFinished));
	}

	IEnumerator DoActionLoadAvatarFromLink(MonoBehaviour _monoBehaviour, string _uri, float _w, float _h, System.Action<Texture2D> _onFinished = null){
		if (myAvatarDownloaded != null) {
			if (_onFinished != null) {
				_onFinished (myAvatarDownloaded);
			}
			yield break;
		}
		yield return _monoBehaviour.StartCoroutine(CoreGameManager.instance.DoActionLoadAvatar(_uri, _w, _h, (_texture) => {
			// if (result.Error != null){
			// 	Debug.Log("Error Response:\n" + result.Error);
			// }else if (!FB.IsLoggedIn){
			// 	Debug.Log("Login cancelled by Player");
			// }else{
			// 	myAvatarFb = result.Texture;
			// 	if (onLoadAvatarFinished != null) {
			// 		onLoadAvatarFinished (myAvatarFb);
			// 		onLoadAvatarFinished = null;
			// 	}
			// }
			myAvatarDownloaded = _texture;
		}));
		if (_onFinished != null) {
			_onFinished (myAvatarDownloaded);
		}
	}

	public Coroutine LoadAvatar(MonoBehaviour _monoBehaviour, float _w, float _h, System.Action<Texture2D> _onFinished = null){
		if(_monoBehaviour == null){
			return null;
		}
		return _monoBehaviour.StartCoroutine(DoActionLoadAvatar(_monoBehaviour, _w, _h, _onFinished));
	}

	IEnumerator DoActionLoadAvatar(MonoBehaviour _monoBehaviour, float _w, float _h, System.Action<Texture2D> _onFinished){
		if(_w < 128){
			_w = 128;
		}
		if(_h < 128){
			_h = 128;
		}
		sbyte _tmpAvatarid = (sbyte)(avatarid % maxLengthListAvatarID);

		if ((databaseId == DatabaseType.DATABASEID_FACEBOOK || databaseId == DatabaseType.DATABASEID_GOOGLE) 
			&& _tmpAvatarid == 0) {
			if (myAvatarDownloaded != null) {
				myAvatar = myAvatarDownloaded;
				if (_onFinished != null) {
					_onFinished (myAvatar);
				}
				yield break;
			}
			yield return LoadAvatarFromLink(_monoBehaviour, _w, _h, _onFinished);
		} else {
			IAvatarInfo _avatarInfo = CoreGameManager.instance.gameInfomation.GetAvatarInfo (_tmpAvatarid);
			if (_avatarInfo == null) {
				#if TEST
				Debug.LogError ("_avatarInfo is null: " + avatarid + " - " + _tmpAvatarid);
				#endif
				yield break;
			} 
			myAvatar = _avatarInfo.mySprite.texture;
			if(_onFinished != null){
				_onFinished (myAvatar);
			}
		}
	}

	public Sprite GetIconDatabaseID(){
		switch(databaseId){
		case UserData.DatabaseType.DATABASEID_BIGXU:
			return GameInformation.instance.otherInfo.iconAccBigXu;
		case UserData.DatabaseType.DATABASEID_DEVICE:
			return GameInformation.instance.otherInfo.iconAccDevice;
		case UserData.DatabaseType.DATABASEID_GOOGLE:
			return GameInformation.instance.otherInfo.iconAccGoogle;
		case UserData.DatabaseType.DATABASEID_FACEBOOK:
			return GameInformation.instance.otherInfo.iconAccFb;
		default:
			return null;
		}
	}

	[System.NonSerialized] UserDataInGame _userDataInGame;
	public UserDataInGame CastToUserDataInGame(){ // chỉ để đọc dữ liệu thôi chứ ko đè chồng dc
		if(_userDataInGame == null){
			_userDataInGame = new UserDataInGame();
		}
		_userDataInGame.InitData(this);
       	return _userDataInGame;
	}

	public void AddNewTotalBetInGameInfo(IMiniGameInfo.Type _gameType){
		if(listTotalBetInGameInfo == null){
			listTotalBetInGameInfo = new List<TotalBetInGameInfo>();
			listTotalBetInGameInfo.Add(new TotalBetInGameInfo(_gameType));
		}else{
			bool _isExist = false;
			for(int i = 0; i < listTotalBetInGameInfo.Count; i++){
				if(listTotalBetInGameInfo[i].gameType == _gameType){
					_isExist = true;
					break;
				}
			}
			if(!_isExist){
				listTotalBetInGameInfo.Add(new TotalBetInGameInfo(_gameType));
			}
		}
	}

	public void SetTotalBetInGameInfo(IMiniGameInfo.Type _gameType, long _totalBet){
		if(listTotalBetInGameInfo == null){
			#if TEST
			Debug.LogError(">>> listTotalBetInGameInfo is null");
			#endif
		}else{
			for(int i = 0; i < listTotalBetInGameInfo.Count; i++){
				if(listTotalBetInGameInfo[i].gameType == _gameType){
					listTotalBetInGameInfo[i].totalBet = _totalBet;
					return;
				}
			}
			#if TEST
			Debug.LogError(">>> Không tìm được game " +  _gameType.ToString());
			#endif
		}
	}

	public TotalBetInGameInfo GetTotalBetInGameInfo(IMiniGameInfo.Type _gameType){
		if(listTotalBetInGameInfo == null || listTotalBetInGameInfo.Count == 0){
			#if TEST
			Debug.LogError(">>> listTotalBetInGameInfo is null or count = 0");
			#endif
			return null;
		}
		for(int i = 0; i < listTotalBetInGameInfo.Count; i++){
			if(listTotalBetInGameInfo[i].gameType == _gameType){
				return listTotalBetInGameInfo[i];
			}
		}
		return null;
	}

	public long GetTotalBetInGamePlay(){
		if(listTotalBetInGameInfo == null){
			return 0;
		}
		long _totalBet = 0;
		for(int i = 0; i < listTotalBetInGameInfo.Count; i++){
			_totalBet += listTotalBetInGameInfo[i].totalBet;
		}
		return _totalBet;
	}

	public void RemoveTotalBetInGameInfo(IMiniGameInfo.Type _gameType){
		if(listTotalBetInGameInfo == null){
			return;
		}
		for(int i = 0; i < listTotalBetInGameInfo.Count; i++){
			if(listTotalBetInGameInfo[i].gameType == _gameType){
				listTotalBetInGameInfo.RemoveAt(i);
				break;
			}
		}
	}

	public long GetGoldView(){
		long _goldView = gold - GetTotalBetInGamePlay();
        if(_goldView < 0){
            #if TEST
            Debug.LogError("Bug Logic gold");
            #endif
            _goldView = 0;
        };
		return _goldView;
	}
}