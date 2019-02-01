using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook;
using Facebook.Unity;

[System.Serializable] public class UserDataInGame : UserData {

	public sbyte index;
	public int win;
	public int tie;
	public int lose;

	public UserDataInGame(){
		_sessionId = -1;
		facebookId = -1;
		index = -1;
	}

	public UserDataInGame(UserData.DatabaseType _databaseid, long _userid, long _fbId, sbyte _avatarid, string _nameShow){
		_sessionId = -1;
		index = -1;
		facebookId = _fbId;

		databaseId = _databaseid;
		userId = _userid;
		avatarid = _avatarid;
		nameShowInGame = _nameShow;
	}

	public UserDataInGame(MessageReceiving _mess, short _newSessionId = -1, sbyte _index = -1){
		_sessionId = _newSessionId;
		index = _index;
		databaseId = (UserData.DatabaseType) _mess.readByte();
		userId = _mess.readLong();
		avatarid = _mess.readByte();
		gold = _mess.readLong();
		nameShowInGame = _mess.readString();
		if (string.IsNullOrEmpty (nameShowInGame)) {
			nameShowInGame = "Unknown";
		}
		win = _mess.readInt();
		tie = _mess.readInt();
		lose = _mess.readInt();

		// Debug.Log(win + " - " + tie + " - " + lose);

		if(databaseId == UserData.DatabaseType.DATABASEID_FACEBOOK){
			facebookId = _mess.readLong();
		}else{
			facebookId = -1;
		}
	}

	public void InitData(UserData _userData){
		_sessionId = _userData.sessionId;
		databaseId = _userData.databaseId;
		userId = _userData.userId;
		avatarid = _userData.avatarid;
		gold = _userData.gold;
		nameShowInGame = _userData.nameShowInGame;
		facebookId = _userData.facebookId;

		myAvatar = _userData.myAvatar;
		myAvatarDownloaded = _userData.myAvatarDownloaded;
	}

	public bool IsEqual(UserDataInGame _other){
		if(_other == null){
			return false;
		}
		if(sessionId == _other.sessionId){
			return true;
		}
		return false;
	}

	public bool IsEqual(long _userId, UserData.DatabaseType _databaseId){
		if(userId == _userId
			&& databaseId == _databaseId){
			return true;
		}
		return false;
	}

	public bool IsEqual(short _otherSessionId){
		if(sessionId == _otherSessionId){
			return true;
		}
		return false;
	}

	public UserDataInGame ShallowCopy()
    {
       UserDataInGame other = (UserDataInGame) this.MemberwiseClone();
       return other;
    }
}
