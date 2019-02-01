using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]  public class KoprokData : MyGamePlayData {
	
	public class Koprok_MeAddBet_Data {
		public bool isAddOk;
		public sbyte indexBet;
		public short chipIndex;
		public long goldAdd;
		public long myBet;
		public short countBet;
		public long globalBet;
		public long myTotalBet;
		public List<long> tableMyBet; 
		public long myGOLD;

		public Koprok_MeAddBet_Data(MessageReceiving _mess){
			isAddOk = _mess.readBoolean();
			tableMyBet = new List<long>();
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

				for(int i = 0; i < 6; i++){
					tableMyBet.Add(_mess.readLong());
				}

				myGOLD = _mess.readLong();
				myTotalBet = _mess.readLong();
			}
		}
	}
	public List<Koprok_MeAddBet_Data> processMeAddBetData;

	public class Koprok_UpdateTableBet_Data {
		public sbyte indexBet;
		public short tableCount;
		public long tableBet;
		public Koprok_UpdateTableBet_Data(MessageReceiving _mess){
			indexBet = _mess.readByte();
			tableCount = _mess.readShort();
			tableBet = _mess.readLong();
			// Debug.Log(">>> " + _indexBet + " - " + _tableCount + " - " + _tableBet);
		}
	}
	public List<Koprok_UpdateTableBet_Data> processUpdateTableBetData;

	public class Koprok_Result_Data{
        // byte[3] dice
		// byte caseCheck
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

		public List<Koprok_GamePlay_Manager.IndexBet> dice;
		public sbyte caseCheck;
		public long gold_Limit;
		public long betUnit;
		public long GOLD;
		public long goldProcess; // dùng cho achievement
		public int achievement; // -> cập nhật lại achievement cho game (nếu goldProcess > 0 thì cập nhật win, = 0 thì cập nhật hòa, < 0 cập nhật thua)
		public System.DateTime nextTimeToShowResult;
		public Koprok_Result_Data (MessageReceiving _mess, KoprokData _koprokData){
			dice = new List<Koprok_GamePlay_Manager.IndexBet>();
			for(int i = 0; i < 3; i++){
				dice.Add((Koprok_GamePlay_Manager.IndexBet) _mess.readByte());
			}
			caseCheck = _mess.readByte();
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
			nextTimeToShowResult = System.DateTime.Now.AddMilliseconds(_koprokData.duringTime);
		}
	}
	public List<Koprok_Result_Data> processResultData;

	[System.Serializable] public class Koprok_History_Data{
		public List<Koprok_GamePlay_Manager.IndexBet> dice;

		public Koprok_History_Data(params Koprok_GamePlay_Manager.IndexBet[] _dice){
			dice = new List<Koprok_GamePlay_Manager.IndexBet>();
			for(int i = 0; i < _dice.Length; i++){
				dice.Add(_dice[i]);
			}
		}
	}
	public List<Koprok_History_Data> listHistory;

	public int duringTime;
	public System.DateTime nextTimeToShowResult;

	public int winAchievement;
	public int tieAchievement;
	public int loseAchievement;

	public List<short> tableCount;
	public List<long> tableGlobalBet;
	public List<long> tableMyBet;

	public List<Koprok_GamePlay_Manager.IndexBet> myIndexBet;

	public bool hasLoadGameInfo;

	public KoprokData(){
		processPlayerAddGoldData = new List<PlayerAddGold_Data>();
		processSubGamePlayerChatData = new List<SubGame_PlayerChat_Data>();
		processPlayerSetParentData = new List<PlayerSetParent_Data>();

		processMeAddBetData = new List<Koprok_MeAddBet_Data>();
		processUpdateTableBetData = new List<Koprok_UpdateTableBet_Data>();
		processResultData = new List<Koprok_Result_Data>();

		hasLoadGameInfo = false;
	}

	public void InitDataWhenGetTableInfo(MessageReceiving _mess){
		DataManager.instance.userData.AddNewTotalBetInGameInfo(IMiniGameInfo.Type.Koprok);

		winAchievement = _mess.readInt();
  		tieAchievement = _mess.readInt();
   		loseAchievement = _mess.readInt();

		int _timeLeft = _mess.readInt();
		duringTime = _mess.readInt();
		nextTimeToShowResult = System.DateTime.Now.AddMilliseconds(_timeLeft);

		sbyte buffer_History = _mess.readByte();
		listHistory = new List<Koprok_History_Data>();
		for(int i = 0; i < buffer_History; i ++){
			Koprok_GamePlay_Manager.IndexBet _slot1 = (Koprok_GamePlay_Manager.IndexBet) _mess.readByte();
			Koprok_GamePlay_Manager.IndexBet _slot2 = (Koprok_GamePlay_Manager.IndexBet) _mess.readByte();
			Koprok_GamePlay_Manager.IndexBet _slot3 = (Koprok_GamePlay_Manager.IndexBet) _mess.readByte();
			listHistory.Add(new Koprok_History_Data(_slot1, _slot2, _slot3));
		}

		tableCount = new List<short>();
		tableGlobalBet = new List<long>();
		tableMyBet = new List<long>();

		for(int i = 0; i < 6; i ++){
			tableCount.Add(_mess.readShort());
		}

		for(int i = 0; i < 6; i ++){
			tableGlobalBet.Add(_mess.readLong());
			tableMyBet.Add(0);
		}

		myIndexBet = new List<Koprok_GamePlay_Manager.IndexBet>();

		hasLoadGameInfo = true;
	}

	public void SetDataUpdateTableBet(MessageReceiving _mess){
		Koprok_UpdateTableBet_Data _data = new Koprok_UpdateTableBet_Data(_mess);
		processUpdateTableBetData.Add(_data);
	}

	public void SetDataWhenShowResult(MessageReceiving _mess){
		Koprok_Result_Data _data = new Koprok_Result_Data(_mess, this);
		processResultData.Add(_data);
	}

	public void SetDataMeAddBet(MessageReceiving _mess){
		Koprok_MeAddBet_Data _data = new Koprok_MeAddBet_Data(_mess);
		processMeAddBetData.Add(_data);
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
		for(int i = 0; i < 6; i++){
			tableCount[i] = 0;
			tableGlobalBet[i] = 0;
			tableMyBet[i] = 0;
		}
	}

	public void CheckListHistoryAgain(){
		int _limit = 20;
		if(listHistory.Count > _limit){
			int _tmpDelta = listHistory.Count - _limit;
			for(int i = 0; i < _tmpDelta; i ++){
				listHistory.RemoveAt(listHistory.Count - 1);
			}
		}
	}
}
