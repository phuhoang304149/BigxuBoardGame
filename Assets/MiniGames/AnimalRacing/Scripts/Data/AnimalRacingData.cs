using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] public class AnimalRacingData : MyGamePlayData {

    public class AnimalRacing_UpdateTableBet_Data {
        public List<long> listGlobalBet;
        public AnimalRacing_UpdateTableBet_Data(MessageReceiving _mess){
            listGlobalBet = new List<long>();
            for (int i = 0; i < 9; i++){
                listGlobalBet.Add(_mess.readLong());
            }
        }
    }
    public List<AnimalRacing_UpdateTableBet_Data> processUpdateTableBet;

    public class AnimalRacing_MeAddBet_Data {
        public bool caseCheck;
        public sbyte indexBet;
        public short chipIndex;
        public long goldAdd;
        public List<long> listGlobalBet;
        public long myGOLD;
        public long totalBet;
        public AnimalRacing_MeAddBet_Data(MessageReceiving _mess){
            caseCheck = _mess.readBoolean(); // false thì đọc tiếp
            if(!caseCheck){
                indexBet = _mess.readByte();
                chipIndex = _mess.readShort();
                goldAdd = _mess.readLong(); // biến dư

                listGlobalBet = new List<long>();
                for (int i = 0; i < 9; i++){
                    listGlobalBet.Add(_mess.readLong());
                }
                myGOLD = _mess.readLong(); // số gold trước khi cược (không cần lắm)
                totalBet = _mess.readLong();
            }
        }
    }
    public List<AnimalRacing_MeAddBet_Data> processMeAddBet;

    public class AnimalRacing_PlayerAddBet_Data {
        public short sessionid;
        public sbyte indexBet;
        public short chipIndex;
        public long goldAdd;
        public long playerBet;
        public long globalBet;
        public long myGOLD;
        public long totalBet;
        public AnimalRacing_PlayerAddBet_Data(MessageReceiving _mess){
            sessionid = _mess.readShort();
            indexBet = _mess.readByte();
            chipIndex = _mess.readShort();
            goldAdd = _mess.readLong(); // biến dư
            playerBet = _mess.readLong(); // số gold tại vị trí đặt cược (gold của người chơi đặt cược)
            globalBet = _mess.readLong(); // số gold tại vị trí đặt cược (gold global)
            
            myGOLD = _mess.readLong(); // số gold trước khi cược (không cần lắm)
            totalBet = _mess.readLong();
            // Debug.Log(goldAdd + " - " + playerBet + " - " + globalBet + " - " + goldPlayerCurrent + " - " + totalBet + " - " + goldPlayerRemain);
        }
    }
    public List<AnimalRacing_PlayerAddBet_Data> processPlayerAddBet;
    
    [System.Serializable] public class HistoryData{
        public AnimalRacing_AnimalController.AnimalType animalType;
        public short score;
    }

    public class AnimalRacing_Result_Data{
        public AnimalRacing_AnimalController.AnimalType animalWin; // vị trí về nhất
        public List<GoldUpdateData> listGoldUpdate;
        public sbyte [][] listAnimalRunData; // data mô tả cách chạy của 9 animal cược
        public List<short> newListCurrentScore;
        public List<long> newListGlobalBet;
        public List<long> newListMyBet;
    
        /// <summary>
        /// valueCheck :
        ///     - 69 : không có ai đặt cược
        ///     - -99: lỗi ServerGold
        ///     - 1 : xử lý tiền thành công
        /// </summary>
        public sbyte valueCheck;
        public System.DateTime nextTimeToShowResult;
        public AnimalRacing_Result_Data(MessageReceiving _mess, AnimalRacingData _animalRacingData){
            // byte result_index : vị trí về nhất (nếu -1 : lỗi server gold + không đọc tiếp)
            // byte[9][80] : data mô tả cách chạy của 9 animal cược
            // short[9] : 9 tỷ lệ cược mới
            // byte caseCheck
            // ***Xét trường hợp caseCheck
            // caseCheck=69 : không có ai đặt cược
            // caseCheck=-99: lỗi ServerGold
            // caseCheck=1 : xử lý tiền thành công ⟶ đọc tiếp
            //     byte numberPlayer
            //     for(numberPlayer){
            //         short sessionid
            //         byte casePlayer
            //         ***Xét trường hợp casePlayer
            //         casePlayer=-55 : lỗi databasePlayer
            //         casePlayer=-88 : lỗi không đủ tiền ⟶ đọc tiếp
            //             long gold_Limit
            //             long GOLD
            //         casePlayer=1 : xử lý tiền thành công
            //             long gold_Limit
            //             long gold_Win
            //             long goldAdd
            //             long GOLD
            //             int achievement : dựa vào ⟶ gold_Win
            //     }
         
            animalWin = (AnimalRacing_AnimalController.AnimalType) _mess.readByte();

            listAnimalRunData = new sbyte[9][];
            for (int i = 0; i < 9; i++){
                listAnimalRunData[i] = new sbyte[80]; // chạy 8s, 0.1s cập nhật 1 lần
                for (int j = 0; j < 80; j++){
                    listAnimalRunData[i][j] = _mess.readByte();
                }
            }

            newListCurrentScore = new List<short>();
            for (int i = 0; i < 9; i++){
                newListCurrentScore.Add(_mess.readShort());
            }
            newListGlobalBet = new List<long>();
            for (int i = 0; i < 9; i++){
                newListGlobalBet.Add(0);
            }
            newListMyBet = new List<long>();
            for (int i = 0; i < 9; i++){
                newListMyBet.Add(0);
            }

            listGoldUpdate = new List<GoldUpdateData>();
            valueCheck = _mess.readByte();
            switch(valueCheck){
            case 1: // cập nhật thành công
                sbyte _numberPlayer = _mess.readByte();
                GoldUpdateData _tmpGoldUpdate = null;
                for(int i = 0; i < _numberPlayer; i++){
                    _tmpGoldUpdate = new GoldUpdateData(_mess);
                    listGoldUpdate.Add(_tmpGoldUpdate);
                }
                break;
            case 69: // không có ai đặt cược
                #if TEST
                Debug.Log(">>> Không có ai đặt cược");
				#endif
                break;
            case -99: // lỗi ServerGold
                #if TEST
                Debug.Log(">>> Lỗi ServerGold");
				#endif
                break;
            default: 
                #if TEST
                Debug.Log(">>> Lỗi không xác định: " + valueCheck);
				#endif
                break;
            }
            
            nextTimeToShowResult = System.DateTime.Now.AddMilliseconds(_animalRacingData.duringTime);
        }
    }
    public List<AnimalRacing_Result_Data> processResultData;
    public AnimalRacing_Result_Data currentResultData{get;set;}

    public class GoldUpdateData{
        public short sessionId;
        public long GOLD;
        public long gold_Limit;
        public long betUnit;
        public long goldAdd; // hiện tại chưa biết xử lý biến này
        public long goldProcess; // dùng cho achievement
		public int achievement; // -> cập nhật lại achievement cho game (nếu goldProcess > 0 thì cập nhật win, = 0 thì cập nhật hòa, < 0 cập nhật thua)
        
        /// <summary>
        /// caseCheck:
        ///     - 1: xử lý tiền thành công
        ///     - -55: lỗi databasePlayer
        ///     - -88: lỗi không đủ tiền
        /// </summary>
        public sbyte caseCheck;

        public GoldUpdateData(MessageReceiving _mess){
            sessionId = _mess.readShort();
            caseCheck = _mess.readByte();
            switch(caseCheck){
            case 1: // xử lý tiền thành công
                gold_Limit = _mess.readLong();
				goldProcess = _mess.readLong();
				goldAdd = _mess.readLong();
				GOLD = _mess.readLong();
				achievement = _mess.readInt();
				betUnit = gold_Limit + goldProcess;
                #if TEST
                if(DataManager.instance.userData.sessionId == sessionId){
                    Debug.Log(">>> " + gold_Limit + "|" + goldProcess + "|" + goldAdd + "|" + GOLD + "|" + achievement + "|" + betUnit);
                }
				#endif
                break;
            case -55: // lỗi databasePlayer
                #if TEST
                Debug.Log(">>> lỗi databasePlayer: " + sessionId);
				#endif
                break;
            case -88: // lỗi không đủ tiền
                #if TEST
                Debug.Log(">>> lỗi không đủ tiền: " + sessionId);
				#endif
                gold_Limit = _mess.readLong();
                GOLD = _mess.readLong();
                break;
            default:
                #if TEST
                Debug.Log(">>> Lỗi không xác định: " + caseCheck + " - " + sessionId);
				#endif
                break;
            }
        }
    }

 	public int duringTime;
    public System.DateTime nextTimeToShowResult;
    public sbyte numberhistory;
    public List<UserDataInGame> listOtherPlayerData;
    public List<HistoryData> listHistoryData;
    public List<short> listCurrentScore; // tỉ lệ nhân hiện tại của 9 con thú
    public List<long> listMyBets;
    public List<long> listGlobalBets;
    public bool hasLoadTableInfo;

	public AnimalRacingData(){
        listHistoryData = new List<HistoryData>();
        listCurrentScore = new List<short>();
        listMyBets = new List<long>();
        listGlobalBets = new List<long>();
        listOtherPlayerData = new List<UserDataInGame>();

        processPlayerJoinGame = new List<PlayerJoinGame_Data>();
        processPlayerLeftGame = new List<PlayerLeftGame_Data>();
        processPlayerChatData = new List<PlayerChat_Data>();
		processPlayerAddGoldData = new List<PlayerAddGold_Data>();
        processPlayerSetParentData = new List<PlayerSetParent_Data>();

        processUpdateTableBet = new List<AnimalRacing_UpdateTableBet_Data>();
        processMeAddBet = new List<AnimalRacing_MeAddBet_Data>();
        processPlayerAddBet = new List<AnimalRacing_PlayerAddBet_Data>();
        processResultData = new List<AnimalRacing_Result_Data>();

        hasLoadTableInfo = false;
    }

	public void InitDataWhenGetTableInfo(MessageReceiving _mess){
        DataManager.instance.userData.AddNewTotalBetInGameInfo(IMiniGameInfo.Type.AnimalRacing);

        InitListOtherUserDataInGame(_mess);

		duringTime = _mess.readInt();
        int _timeLeft = _mess.readInt();
		nextTimeToShowResult = System.DateTime.Now.AddMilliseconds(_timeLeft);
        
        numberhistory = _mess.readByte();
        
        listHistoryData.Clear();
        HistoryData _tmpHistoryData = null;
        for (int i = 0; i < numberhistory; i++)
        {
            _tmpHistoryData = new HistoryData();
            _tmpHistoryData.animalType = (AnimalRacing_AnimalController.AnimalType) _mess.readByte();
            _tmpHistoryData.score = _mess.readShort();

            listHistoryData.Add(_tmpHistoryData);
        }

        listCurrentScore.Clear();
        for (int i = 0; i < 9; i++){
            listCurrentScore.Add(_mess.readShort());
        }

        listGlobalBets.Clear();
        for (int i = 0; i < 9; i++){
            listGlobalBets.Add(_mess.readLong());
        }

        listMyBets.Clear();
        for (int i = 0; i < 9; i++){
            listMyBets.Add(0);
        }
        hasLoadTableInfo = true;
	}

    public void InitListOtherUserDataInGame(MessageReceiving _mess){
        listOtherPlayerData = new List<UserDataInGame>();
		UserDataInGame _usedata = null;
		sbyte _maxViewer = _mess.readByte();
		for(int i = 0; i < (int)_maxViewer; i++){
			sbyte _caseCheck = _mess.readByte(); //(nếu giá trị -1 thì không đọc data dưới --> tiếp tục vòng for)
			if(_caseCheck >= 0){
                short _sessionId = _mess.readShort();
				_usedata = new UserDataInGame(_mess, _sessionId, (sbyte) i);
				if(_usedata.sessionId != DataManager.instance.userData.sessionId){
                    _usedata.AddNewTotalBetInGameInfo(IMiniGameInfo.Type.AnimalRacing);
					listOtherPlayerData.Add(_usedata);
				}else{
                    DataManager.instance.userData.CastToUserDataInGame().index = (sbyte) i;

                    AchievementDetail _achievementDetail = DataManager.instance.achievementData.GetAchievementDetail(IMiniGameInfo.Type.AnimalRacing);
                    if(_achievementDetail != null){
                        _achievementDetail.countWin = _usedata.win;
                        _achievementDetail.countDraw = _usedata.tie;
                        _achievementDetail.countLose = _usedata.lose;
                        // Debug.Log(_usedata.win + " - " + _usedata.tie + " - " + _usedata.lose);
                    }else{
                        Debug.LogError(">>> _achievementDetail is null");
                    }
                }
			}else{
				_usedata = new UserDataInGame();
				listOtherPlayerData.Add(_usedata);
			}
			// Debug.LogError(_usedata.sessionId + " - " + DataManager.instance.userData.sessionId);
		}
	}

    public void SetUpUserJoinGame(MessageReceiving _mess){
        PlayerJoinGame_Data _data = new PlayerJoinGame_Data(_mess);
        sbyte _myIndexChair = DataManager.instance.userData.CastToUserDataInGame().index;
        if(_data.viewerId == _myIndexChair){
            #if TEST
            Debug.LogError(">>> Chỗ này tao đang ngồi: " + _data.viewerId);
            #endif
        }else if(_data.viewerId > _myIndexChair){
            _data.viewerId --;
            _data.userData.index = _data.viewerId;
        }
        processPlayerJoinGame.Add(_data);
    }
    
    public void SetUpUserLeftGame(MessageReceiving _mess){
        PlayerLeftGame_Data _data = new PlayerLeftGame_Data(_mess);
		processPlayerLeftGame.Add(_data);
    }

    public void SetDataUpdateTableBet(MessageReceiving _mess){
        AnimalRacing_UpdateTableBet_Data _data = new AnimalRacing_UpdateTableBet_Data(_mess);
		processUpdateTableBet.Add(_data);
    }

    public void SetDataMeAddBet(MessageReceiving _mess){
        AnimalRacing_MeAddBet_Data _data = new AnimalRacing_MeAddBet_Data(_mess);
		processMeAddBet.Add(_data);
    }

    public void SetDataPlayerAddBet(MessageReceiving _mess){
        AnimalRacing_PlayerAddBet_Data _data = new AnimalRacing_PlayerAddBet_Data(_mess);
		processPlayerAddBet.Add(_data);
    }

    public void SetDataWhenShowResult(MessageReceiving _mess){
        AnimalRacing_Result_Data _data = new AnimalRacing_Result_Data(_mess, this);
		processResultData.Add(_data);
    }

    public void SetPlayerChatData(MessageReceiving _mess){
		PlayerChat_Data _data = new PlayerChat_Data(_mess);
		processPlayerChatData.Add(_data);
	}

	public void SetPlayerAddGoldData(MessageReceiving _mess){
		PlayerAddGold_Data _data = new PlayerAddGold_Data(_mess);
		processPlayerAddGoldData.Add(_data);
	}

    public void SetDataWhenSetParent(MessageReceiving _mess){
		PlayerSetParent_Data _data = new PlayerSetParent_Data(_mess);
		processPlayerSetParentData.Add(_data);
	}

    public void CheckListHistoryAgain(){
		int _limit = 20;
		if(listHistoryData.Count > _limit){
			int _tmpDelta = listHistoryData.Count - _limit;
			for(int i = 0; i < _tmpDelta; i ++){
				listHistoryData.RemoveAt(listHistoryData.Count - 1);
			}
		}
	}
}

