using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] public class MyGamePlayData{

	public class AlertUpdateServer_Data {
		public long timeRemain;
		public System.DateTime timeToUpdateServer;
		public AlertUpdateServer_Data(MessageReceiving _mess){
			timeRemain = _mess.readLong();
			timeToUpdateServer = System.DateTime.Now.AddMilliseconds(timeRemain);
		}
	}

	public class PlayerJoinGame_Data {
		public UserDataInGame userData;
		public short sessionId;
		public sbyte viewerId;
		public PlayerJoinGame_Data(MessageReceiving _mess){
			sessionId = _mess.readShort();
       		viewerId = _mess.readByte();
			userData = new UserDataInGame(_mess, sessionId, viewerId);
		}
	}
	public List<PlayerJoinGame_Data> processPlayerJoinGame;

	public class PlayerLeftGame_Data {
		public short sessionId;
		public PlayerLeftGame_Data(MessageReceiving _mess){
			sessionId = _mess.readShort();
		}
	}
	public List<PlayerLeftGame_Data> processPlayerLeftGame;

	public class PlayerSitDown_Data {
		public short sessionId;
		public sbyte indexChair;
		public PlayerSitDown_Data(MessageReceiving _mess){
			sessionId = _mess.readShort();
			indexChair = _mess.readByte();
		}
	}
	public List<PlayerSitDown_Data> processPlayerSitDown;

	public class MeSitDownFail_Data {
		public bool isSuccess;
		public sbyte chairId;
		public sbyte currentChairId;
		public long totalBet;
		public long myGold;
		public MeSitDownFail_Data(MessageReceiving _mess){
			isSuccess = _mess.readBoolean();
			if(!isSuccess){
				chairId = _mess.readByte();
				currentChairId = _mess.readByte();
				totalBet = _mess.readLong();
				myGold = _mess.readLong();
			}
		}
	}
	public List<MeSitDownFail_Data> processMeSitDownFail;

	public class PlayerStandUp_Data {
		public short sessionId;
		public sbyte indexChair;
		public bool isPlaying;
		public PlayerStandUp_Data(MessageReceiving _mess){
			sessionId = _mess.readShort();
			indexChair = _mess.readByte();
			isPlaying = false;
		}
	}
	public List<PlayerStandUp_Data> processPlayerStandUp;

	public class PlayerChat_Data {
		public short sessionId;
		public string strMess;
		public PlayerChat_Data(MessageReceiving _mess){
			sessionId = _mess.readShort();
            strMess = _mess.readString();
		}
	} 
	public List<PlayerChat_Data> processPlayerChatData;

	public class SubGame_PlayerChat_Data {
		public UserData.DatabaseType databaseid;
		public long userid;
		public sbyte avatarid;
		public string nameShow;
		public long fbId;
		public string contentChat;
		public UserDataInGame userData;

		public SubGame_PlayerChat_Data(MessageReceiving _mess){
			databaseid = (UserData.DatabaseType) _mess.readByte();
			userid = _mess.readLong();
			avatarid = _mess.readByte();
			nameShow = _mess.readString();
			fbId = -1;
			if(databaseid == UserData.DatabaseType.DATABASEID_FACEBOOK){
				fbId = _mess.readLong();
			}
			contentChat = _mess.readString();
			
			userData = new UserDataInGame(databaseid, userid, fbId, avatarid, nameShow);
		}
	} 
	public List<SubGame_PlayerChat_Data> processSubGamePlayerChatData;

	public class PlayerAddGold_Data {
		public short sessionId;
		public int reason;
		public long goldAdd;
		public long goldLast;
		public PlayerAddGold_Data(MessageReceiving _mess){
			sessionId = _mess.readShort();
            reason = _mess.readInt();
            goldAdd = _mess.readLong();
            goldLast = _mess.readLong();
		}
	} 
	public List<PlayerAddGold_Data> processPlayerAddGoldData;

	public class PlayerSetParent_Data {

		public class UserSetParentInfo{
			public UserData.DatabaseType databaseId;
			public long userId;
			public sbyte avatarId;
			public string nameShow;
			public string linkIcon;
			public long facebookId;
			public long GOLD;
		}

		public UserSetParentInfo myParentInfoBefore{get;set;}
		public UserSetParentInfo childInfo{get;set;}
		public UserSetParentInfo parentInfo{get;set;}
		
		public sbyte caseCheck;
		public long goldAddInvite;
		
		public PlayerSetParent_Data(MessageReceiving _mess){
			caseCheck = _mess.readByte();
			switch(caseCheck){
			case 0:
				myParentInfoBefore = new UserSetParentInfo();
				myParentInfoBefore.databaseId = (UserData.DatabaseType) _mess.readByte();
				myParentInfoBefore.userId = _mess.readLong();
				myParentInfoBefore.avatarId = _mess.readByte();
				myParentInfoBefore.nameShow = _mess.readString();
				myParentInfoBefore.facebookId = -1;
				if(myParentInfoBefore.databaseId == UserData.DatabaseType.DATABASEID_FACEBOOK){
					myParentInfoBefore.facebookId = _mess.readLong();
				}else if(myParentInfoBefore.databaseId == UserData.DatabaseType.DATABASEID_GOOGLE){
					myParentInfoBefore.linkIcon = _mess.readString();
				}
				// Debug.Log(">>> My Parent before: "
				// 	+ myParentInfoBefore.databaseId + "|"
				// 	+ myParentInfoBefore.userId + "|"
				// 	+ myParentInfoBefore.avatarId + "|"
				// 	+ myParentInfoBefore.nameShow + "|"
				// 	+ myParentInfoBefore.facebookId + "|"
				// 	+ myParentInfoBefore.linkIcon + "|"
				// 	+ myParentInfoBefore.GOLD);
				break;
			case 1:
				goldAddInvite = _mess.readLong();
				childInfo = new UserSetParentInfo();
				childInfo.databaseId = (UserData.DatabaseType) _mess.readByte();
				childInfo.userId = _mess.readLong();
				childInfo.GOLD = _mess.readLong();
				childInfo.avatarId = _mess.readByte();
				childInfo.nameShow = _mess.readString();
				childInfo.facebookId = -1;
				if(childInfo.databaseId == UserData.DatabaseType.DATABASEID_FACEBOOK){
					childInfo.facebookId = _mess.readLong();
				}else if(childInfo.databaseId == UserData.DatabaseType.DATABASEID_GOOGLE){
					childInfo.linkIcon = _mess.readString();
				}
				// Debug.Log(">>> Child: "
				// 	+ childInfo.databaseId + "|"
				// 	+ childInfo.userId + "|"
				// 	+ childInfo.avatarId + "|"
				// 	+ childInfo.nameShow + "|"
				// 	+ childInfo.facebookId + "|"
				// 	+ childInfo.linkIcon + "|"
				// 	+ childInfo.GOLD);
				
				parentInfo = new UserSetParentInfo();
				parentInfo.databaseId = (UserData.DatabaseType) _mess.readByte();
				parentInfo.userId = _mess.readLong();
				parentInfo.GOLD = _mess.readLong();
				parentInfo.avatarId = _mess.readByte();
				parentInfo.nameShow = _mess.readString();
				parentInfo.facebookId = -1;
				if(parentInfo.databaseId == UserData.DatabaseType.DATABASEID_FACEBOOK){
					parentInfo.facebookId = _mess.readLong();
				}else if(parentInfo.databaseId == UserData.DatabaseType.DATABASEID_GOOGLE){
					parentInfo.linkIcon = _mess.readString();
				}
				// Debug.Log(">>> Parent: "
				// 	+ parentInfo.databaseId + "|"
				// 	+ parentInfo.userId + "|"
				// 	+ parentInfo.avatarId + "|"
				// 	+ parentInfo.nameShow + "|"
				// 	+ parentInfo.facebookId + "|"
				// 	+ parentInfo.linkIcon + "|"
				// 	+ parentInfo.GOLD);
				break;
			default:
				Debug.LogError(">>> Không có caseCheck này: " + caseCheck);
				break;
			}
		}
	}
	public List<PlayerSetParent_Data> processPlayerSetParentData;

	public void SetUpActionPlayerSetParent(PlayerSetParent_Data _playerSetParentData){
		if(_playerSetParentData.caseCheck == 0){
			DataManager.instance.parentUserData = new UserData();
			DataManager.instance.parentUserData.InitData();
			DataManager.instance.parentUserData.databaseId = _playerSetParentData.myParentInfoBefore.databaseId;
			DataManager.instance.parentUserData.userId = _playerSetParentData.myParentInfoBefore.userId;
			DataManager.instance.parentUserData.avatarid = _playerSetParentData.myParentInfoBefore.avatarId;
			DataManager.instance.parentUserData.nameShowInGame = _playerSetParentData.myParentInfoBefore.nameShow;
			DataManager.instance.parentUserData.facebookId = _playerSetParentData.myParentInfoBefore.facebookId;

			PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kWarning)
				, MyLocalize.GetString("InviteFriend/YouHadBeenInvitedBefore")
				, string.Empty
				, MyLocalize.GetString(MyLocalize.kOk));

			if(GetGoldScreenController.instance.currentState == UIHomeScreenController.State.Show){
				if(GetGoldScreenController.instance.currentTab == GetGoldScreenController.Tab.InviteFriend){
					GetGoldScreenController.instance.panelInviteFriend.RefreshData();
				}
			}	
		}else if(_playerSetParentData.caseCheck == 1){
			bool _isCaseCheckError = false;
			UserData _child = new UserData();
			_child.InitData();
			_child.databaseId = _playerSetParentData.childInfo.databaseId;
			_child.userId = _playerSetParentData.childInfo.userId;
			_child.avatarid = _playerSetParentData.childInfo.avatarId;
			_child.nameShowInGame = _playerSetParentData.childInfo.nameShow;
			_child.facebookId = _playerSetParentData.childInfo.facebookId;

			UserData _parent = new UserData();
			_parent.InitData();
			_parent.databaseId = _playerSetParentData.parentInfo.databaseId;
			_parent.userId = _playerSetParentData.parentInfo.userId;
			_parent.avatarid = _playerSetParentData.parentInfo.avatarId;
			_parent.nameShowInGame = _playerSetParentData.parentInfo.nameShow;
			_parent.facebookId = _playerSetParentData.parentInfo.facebookId;

			if(DataManager.instance.userData.databaseId == _playerSetParentData.childInfo.databaseId
				&& DataManager.instance.userData.userId == _playerSetParentData.childInfo.userId){
				DataManager.instance.userData.gold = _playerSetParentData.childInfo.GOLD;
				DataManager.instance.parentUserData = _parent;
			} else if(DataManager.instance.userData.databaseId == _playerSetParentData.parentInfo.databaseId
				&& DataManager.instance.userData.userId == _playerSetParentData.parentInfo.userId){
				DataManager.instance.userData.gold = _playerSetParentData.parentInfo.GOLD;
			}else {
				#if TEST
				Debug.LogError("Bug Logic Player Set Parent");
				#endif
				_isCaseCheckError = true;
			}

			if(!_isCaseCheckError){
				if(GetGoldScreenController.instance.currentState == UIHomeScreenController.State.Show){
					if(GetGoldScreenController.instance.currentTab == GetGoldScreenController.Tab.InviteFriend){
						GetGoldScreenController.instance.panelInviteFriend.RefreshData();
					}
					GetGoldScreenController.instance.RefreshMyGoldInfo();
				}
				PopupManager.Instance.CreatePopupInviteFriendSuccessful(_child, _parent
					, _playerSetParentData.goldAddInvite, MyLocalize.GetString(MyLocalize.kOk), null);
			}
		}
	}
}
