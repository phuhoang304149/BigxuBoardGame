using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOL_Player_Data {
	public static BOL_Player_Data instance {
		get {
			if (ins == null) {
				ins = new BOL_Player_Data();
			}
			return ins;
		}
	}

	static BOL_Player_Data ins;
	public BOL_Player_Data() { }

	public static void SelfDestruction() {
		ins = null;
	}

	#region Variable
	public static sbyte databaseid;

	public string deviceString;
	public string deviceModel;
	public string username;
	public long facebookid;
	public string facebooktoken;

	public long userid;
	public string nameShow;
	public sbyte avatarid;
	public long gold;
	public long gem;
	public long gold_debt;
	public long gem_debt;
	public long time_create_account;
	public long lastTimePlay;

	int win;
	int tie;
	int lose;
	#endregion
	public void SetPlayerInfo(MessageReceiving message) {
		databaseid = message.readByte();
		switch (databaseid) {
			case (sbyte)Constant.Databaseid.DATABASEID_DEVICE:
				deviceString = message.readString();
				deviceModel = message.readString();
				break;
			case (sbyte)Constant.Databaseid.DATABASEID_BIGXU:
				username = message.readString();
				break;
			case (sbyte)Constant.Databaseid.DATABASEID_FACEBOOK:
				facebookid = message.readLong();
				facebooktoken = message.readString();
				break;
		}
		userid = message.readLong();
		nameShow = message.readString(); ;
		avatarid = message.readByte();
		gold = message.readLong();
		gem = message.readLong();
		gold_debt = message.readLong();
		gem_debt = message.readLong();
		time_create_account = message.readLong();
		lastTimePlay = message.readLong();
	}
	public void SetPlayerInGame(MessageReceiving message) {
		databaseid = message.readByte();
		userid = message.readLong();
		avatarid = message.readByte();
		gold = message.readLong();
		nameShow = message.readString();
		win = message.readInt();
		tie = message.readInt();
		lose = message.readInt();
		if(databaseid==(sbyte)Constant.Databaseid.DATABASEID_FACEBOOK){
			facebookid = message.readLong();
		}
        
	}
}
