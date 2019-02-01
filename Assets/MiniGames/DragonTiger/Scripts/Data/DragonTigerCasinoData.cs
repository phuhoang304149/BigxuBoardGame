using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] public class DragonTigerCasinoData : MyGamePlayData {

	public class DragonTiger_MeAddBet_Data {
		public bool isAddOk;
		public sbyte indexBet;
		public short chipIndex;
		public long goldAdd;
		public long myBet;
		public short countBet;
		public long globalBet;
		public long betDragon;
		public long betTie;
		public long betTiger;
		public long myGOLD;
		public long myTotalBet;

		public DragonTiger_MeAddBet_Data(MessageReceiving _mess){
			isAddOk = _mess.readBoolean();
			myGOLD = 0;
			if(isAddOk){
				indexBet = _mess.readByte();
				chipIndex = _mess.readShort();
				goldAdd = _mess.readLong();
				myBet = _mess.readLong();
				countBet = _mess.readShort();
				globalBet = _mess.readLong();

				myGOLD = _mess.readLong();
				myTotalBet = _mess.readLong();
			}else{
				indexBet = _mess.readByte();
				chipIndex = _mess.readShort();
				goldAdd = _mess.readLong();

				betDragon = _mess.readLong();
				betTie = _mess.readLong();
				betTiger = _mess.readLong();

				myGOLD = _mess.readLong();
				myTotalBet = _mess.readLong();
			}
		}
	}
	public List<DragonTiger_MeAddBet_Data> processMeAddBetData;

	public class DragonTiger_UpdateTableBet_Data {
		public sbyte indexBet;
		public short tableCount;
		public long tableBet;
		public DragonTiger_UpdateTableBet_Data(MessageReceiving _mess){
			indexBet = _mess.readByte();
			tableCount = _mess.readShort();
			tableBet = _mess.readLong();
			// Debug.Log(">>> " + _indexBet + " - " + _tableCount + " - " + _tableBet);
		}
	}
	public List<DragonTiger_UpdateTableBet_Data> processUpdateTableBetData;

	public class DragonTiger_Result_Data{
		public sbyte cardDragon;
		public sbyte cardTiger;
		public sbyte caseCheck;
		public long betUnit;
		public long gold_Limit;
		public long GOLD;
		public long goldProcess; // dùng cho achievement
		public int achievement; // -> cập nhật lại achievement cho game (nếu goldProcess > 0 thì cập nhật win, = 0 thì cập nhật hòa, < 0 cập nhật thua)
		public System.DateTime nextTimeToShowResult;

		public DragonTiger_Result_Data (MessageReceiving _mess, DragonTigerCasinoData _dragonTigerCasinoData){
			cardDragon = _mess.readByte();
			cardTiger = _mess.readByte();
			caseCheck = _mess.readByte();

			// ***Xét trường hợp:
			// caseCheck=69  : không đặt cược
			// caseCheck=1  : xử lý game thành công
			// 	long gold_Limit : tổng cược
			// 	long gold_Win : tiền xử lý
			// 	long goldAdd : tiền được cộng do server xử lý
			// 	long GOLD
			// 	int achievement (dựa vào gold_Win)
			// caseCheck=-99 : lỗi serverGold
			// caseCheck=-88 : lỗi không đủ tiền cược ⟶ đọc tiếp
			// 	long gold_Limit : tổng tiền đã đặt
			// 	long GOLD

			switch(caseCheck){
			case 1: // xử lý game thành công
				gold_Limit = _mess.readLong();
				goldProcess = _mess.readLong();
				long goldAdd = _mess.readLong();
				GOLD = _mess.readLong();
				achievement = _mess.readInt();
				betUnit = gold_Limit + goldProcess;
				#if TEST
				Debug.Log(">>> " + gold_Limit + "|" + goldProcess + "|" + goldAdd + "|" + GOLD + "|" + achievement + "|" + betUnit);
				#endif
				break;
			case 69: // không đặt cược
				#if TEST
				Debug.LogError("Người chơi đang không đặt cược");
				#endif
				GOLD = DataManager.instance.userData.gold;
				break;
			case -99: // lỗi serverGold
				#if TEST
				Debug.LogError("Lỗi serverGold");
				#endif
				GOLD = DataManager.instance.userData.gold;
				break;
			case -88: // lỗi không đủ tiền cược khi cược đồng thời ở 2 thiết bị khác nhau, ở 2 sv khác nhau
				#if TEST
				Debug.LogError("lỗi không đủ tiền cược khi cược đồng thời ở 2 thiết bị khác nhau, ở 2 sv khác nhau.");
				#endif
				gold_Limit = _mess.readLong();
				GOLD = _mess.readLong();
				break;
			default:
				#if TEST
				Debug.LogError("Lỗi không xác định : " + caseCheck);
				#endif
				GOLD = DataManager.instance.userData.gold;
				break;
			}

			nextTimeToShowResult = System.DateTime.Now.AddMilliseconds(_dragonTigerCasinoData.duringTime);
		}
    }
	public List<DragonTiger_Result_Data> processResultData;
	
	public int duringTime;
	public System.DateTime nextTimeToShowResult;

	/// <summary>
	/// listHistory :
	/// 	- giá trị = 1: Long
	/// 	- giá trị = 0: Huề
	/// 	- giá trị = -1: Hổ
	/// </summary>
   	public List<sbyte> listHistory;
   	public short tableCountDragon;
	public short tableCountTiger;
	public short tableCountTie;
   	public long tableGlobalBetDragon;
	public long tableMyBetDragon;
   	public long tableGlobalBetTie;
	public long tableMyBetTie;
   	public long tableGlobalBetTiger;
   	public long tableMyBetTiger;
	public bool hasLoadGameInfo;

	public int winAchievement;
	public int tieAchievement;
	public int loseAchievement;
	
	public DragonTigerCasinoData(){
		listHistory = new List<sbyte>();

		processPlayerAddGoldData = new List<PlayerAddGold_Data>();
		processPlayerSetParentData = new List<PlayerSetParent_Data>();
		processSubGamePlayerChatData = new List<SubGame_PlayerChat_Data>();

		processMeAddBetData = new List<DragonTiger_MeAddBet_Data>();
		processUpdateTableBetData = new List<DragonTiger_UpdateTableBet_Data>();
		processResultData = new List<DragonTiger_Result_Data>();

		hasLoadGameInfo = false;
	}

	public void InitDataWhenGetTableInfo(MessageReceiving _mess){
		DataManager.instance.userData.AddNewTotalBetInGameInfo(IMiniGameInfo.Type.DragonTigerCasino);

		winAchievement = _mess.readInt();
  		tieAchievement = _mess.readInt();
   		loseAchievement = _mess.readInt();

		int _timeLeft = _mess.readInt();
		duringTime = _mess.readInt();
		nextTimeToShowResult = System.DateTime.Now.AddMilliseconds(_timeLeft);

        sbyte buffer_History = _mess.readByte();
		listHistory = new List<sbyte>();
		for(int i = 0; i < buffer_History; i ++){
			listHistory.Add(_mess.readByte());
		}
		tableCountDragon = _mess.readShort();
		tableGlobalBetDragon = _mess.readLong();
		tableCountTie = _mess.readShort();
		tableGlobalBetTie = _mess.readLong();
		tableCountTiger = _mess.readShort();
		tableGlobalBetTiger = _mess.readLong();

		tableMyBetDragon = 0;
		tableMyBetTie = 0;
		tableMyBetTiger = 0;

		hasLoadGameInfo = true;
	}

	public void SetDataMeAddBet(MessageReceiving _mess){
		DragonTiger_MeAddBet_Data _data = new DragonTiger_MeAddBet_Data(_mess);
		processMeAddBetData.Add(_data);
	}

	public void SetDataUpdateTableBet(MessageReceiving _mess){
		DragonTiger_UpdateTableBet_Data _data = new DragonTiger_UpdateTableBet_Data(_mess);
		processUpdateTableBetData.Add(_data);
	}

	public void SetDataWhenShowResult(MessageReceiving _mess){
		DragonTiger_Result_Data _data = new DragonTiger_Result_Data(_mess, this);
		processResultData.Add(_data);
	}

	public void SetPlayerChatData(MessageReceiving _mess){
		SubGame_PlayerChat_Data _data = new SubGame_PlayerChat_Data(_mess);
		processSubGamePlayerChatData.Add(_data);
	}

	public void SetPlayerAddGoldData(MessageReceiving _mess){
		PlayerAddGold_Data _data = new PlayerAddGold_Data(_mess);
		processPlayerAddGoldData.Add(_data);
	}

	public void SetDataWhenSetParent(MessageReceiving _mess){
		PlayerSetParent_Data _data = new PlayerSetParent_Data(_mess);
		processPlayerSetParentData.Add(_data);
	}

	public void ResetTableBet(){
		tableCountDragon = 0;
		tableGlobalBetDragon = 0;
		tableCountTie = 0;
		tableGlobalBetTie = 0;
		tableCountTiger = 0;
		tableGlobalBetTiger = 0;

		tableMyBetDragon = 0;
		tableMyBetTie = 0;
		tableMyBetTiger = 0;
	}

	public void CheckListHistoryAgain(){
		int _limit = 40;
		if(listHistory.Count > _limit){
			int _tmpDelta = listHistory.Count - _limit;
			for(int i = 0; i < _tmpDelta; i ++){
				listHistory.RemoveAt(0);
			}
		}
	}
}
