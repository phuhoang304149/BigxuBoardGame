using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] public class PokerGamePlayData : MyGamePlayData {
	
	public enum TypeCardResult{
		UNKNOWN = 0,
		TYPECARD_HIGH_CARD = 1,
		TYPECARD_PAIR = 2,
		TYPECARD_TWO_PAIR = 3,
		TYPECARD_THREE_OF_A_KIND = 4,
		TYPECARD_STRAIGHT = 5,
		TYPECARD_FLUSH = 6,
		TYPECARD_FULL_HOUSE = 7,
		TYPECARD_FOUR_OF_A_KIND = 8,
		TYPECARD_STRAIGHT_FLUSH = 9
	}

	public enum GameState{
		STATUS_WAIT_FOR_PLAYER = 0,
		STATUS_TURN_1_CHIA_BAI_NGUOI_CHOI = 110,
		STATUS_TURN_2_CHIA_3 = 111,
		STATUS_TURN_3_CHIA_4 = 112,
		STATUS_TURN_4_CHIA_5 = 113,
		STATUS_FINISHGAME = 114
	}
	public GameState currentGameState;

	// public const sbyte STATUS_WAIT_FOR_PLAYER = 0; 
	// public const sbyte STATUS_TURN_1_CHIA_BAI_NGUOI_CHOI = 110;
	// public const sbyte STATUS_TURN_2_CHIA_3 = 111;
	// public const sbyte STATUS_TURN_3_CHIA_4 = 112;
	// public const sbyte STATUS_TURN_4_CHIA_5 = 113;

	// public const sbyte STATEPOKER_WAITING = 0;
	// public const sbyte STATEPOKER_PLAYING = 99;
	// public const sbyte STATEPOKER_FOLD = 100;
	// public const sbyte STATEPOKER_CHECKING = 101;
	// public const sbyte STATEPOKER_RAISE = 102;
	// public const sbyte STATEPOKER_ALLIN = 103;

	[System.Serializable] public class Poker_HistoryData{
		public long id;
		public List<sbyte> globalCards;
		public List<sbyte> circleIndexWin;
		public List<Poker_PlayerPlayingData> listPlayerPlayingData;
		
		public Poker_HistoryData(){
			id = MyConstant.currentTimeMilliseconds;
			globalCards = new List<sbyte>();
			circleIndexWin = new List<sbyte>();
			listPlayerPlayingData = new List<Poker_PlayerPlayingData>();
		}
	}
	public Poker_HistoryData historyData{get;set;}

	[System.Serializable] public class Poker_PlayerPlayingData{
		public enum State{
			None = 99,
			STATEPOKER_FOLD = 100,
			STATEPOKER_CHECKING = 101,
			STATEPOKER_CALL  = 102,
			STATEPOKER_RAISE = 103,
			STATEPOKER_ALLIN = 104
		}

		public UserDataInGame userData;
		public State currentState;
		public sbyte indexChair;
		public long turnBet;
		public long totalBet;
		public long goldWinOrReturn;
		public List<sbyte> ownCards;
		public List<sbyte> highLightCardsResult;
		public TypeCardResult typeCardResult;
		public bool isMe;

		public Poker_PlayerPlayingData (){
			currentState = State.None;
			ownCards = new List<sbyte>();
			ownCards.Add(-1);
			ownCards.Add(-1);

			typeCardResult = TypeCardResult.UNKNOWN;
			highLightCardsResult = new List<sbyte>();
		}

		public Poker_PlayerPlayingData (MessageReceiving _mess, List<short> _listSessionIdGlobalPlayer){
			short _sessionId = _mess.readShort();
			indexChair = _mess.readByte();
			currentState = (State) _mess.readByte();
			turnBet = _mess.readLong();
			totalBet = _mess.readLong();

			sbyte _indexGlobal = (sbyte) _listSessionIdGlobalPlayer.IndexOf(_sessionId);
			userData = new UserDataInGame(_mess, _sessionId, _indexGlobal);
			if(_indexGlobal < 0){
				#if TEST
				Debug.Log(userData.nameShowInGame + " đã rời bàn trước đó");
				#endif
			}
			if(_sessionId == DataManager.instance.userData.sessionId){
				isMe = true;
			}else{
				isMe = false;
			}

			ownCards = new List<sbyte>();
			ownCards.Add(-1);
			ownCards.Add(-1);

			typeCardResult = TypeCardResult.UNKNOWN;
			highLightCardsResult = new List<sbyte>();
		}
	}

	public class Poker_GameReady_Data{
		public System.DateTime nextTimeToStartGame;
		public Poker_GameReady_Data(MessageReceiving _mess){
			long _timeCountDown = _mess.readLong();
			nextTimeToStartGame = System.DateTime.Now.AddMilliseconds(_timeCountDown);
		}
	}
	public List<Poker_GameReady_Data> processGameReadyData;

	public class Poker_StartGame_Data {
		public sbyte ownCard_00;
		public sbyte ownCard_01;
		public long betDefault;
		public sbyte circleCurrent;
		public sbyte circleLength;
		public List<Poker_PlayerPlayingData> listPlayerPlaying;
		public List<short> listSessionIdPlaying;
		public long totalBet;
		public System.DateTime nextTimeToChangeCircle;
		public Poker_StartGame_Data(MessageReceiving _mess, PokerGamePlayData _pokerGamePlayData){
			ownCard_00 = _mess.readByte();
			ownCard_01 = _mess.readByte();
			betDefault = _mess.readLong();
			circleCurrent = _mess.readByte();
			circleLength = _mess.readByte();
			totalBet = 0;
			
			listPlayerPlaying = new List<Poker_PlayerPlayingData>();
			listSessionIdPlaying = new List<short>();
			Poker_PlayerPlayingData _tmpPlayerPlaying = null;
			for(int i = 0; i < circleLength; i ++){
				_tmpPlayerPlaying = new Poker_PlayerPlayingData();
				short _sessionId = _mess.readShort();
				_tmpPlayerPlaying.indexChair = _mess.readByte();
				_tmpPlayerPlaying.userData = new UserDataInGame(_mess, _sessionId, -1);

				if(_pokerGamePlayData.CheckIfIsMe(_sessionId)){
					_tmpPlayerPlaying.isMe = true;
					_tmpPlayerPlaying.ownCards[0] = ownCard_00;
					_tmpPlayerPlaying.ownCards[1] = ownCard_01;
				}else{
					_tmpPlayerPlaying.isMe = false;
				}
				
				if(i == 0){
					_tmpPlayerPlaying.turnBet = betDefault / 2;
				}else if(i == 1){
					_tmpPlayerPlaying.turnBet = betDefault;
				}else{
					_tmpPlayerPlaying.turnBet = 0;
				}
				_tmpPlayerPlaying.totalBet += _tmpPlayerPlaying.turnBet;
				totalBet += _tmpPlayerPlaying.totalBet;

				listPlayerPlaying.Add(_tmpPlayerPlaying);
				listSessionIdPlaying.Add(_sessionId);
			}

			nextTimeToChangeCircle = System.DateTime.Now.AddMilliseconds(_pokerGamePlayData.timeDurringChangeCircle);
		}
	}
	public List<Poker_StartGame_Data> processStartGameData;

	public class Poker_PlayerChangeTurn{
		public sbyte lastPlayer_IndexCircle;
		public Poker_PlayerPlayingData.State lastPlayer_CurrentState;
		public long lastPlayer_CircleBet;
		public long lastPlayer_TotalBet;
		public long lastPlayer_GoldRemain;
		public long currentMaxBet;
		public sbyte currentPlayer_IndexCircle;
		public GameState currentGameState;
		public List<sbyte> globalCards;
		public System.DateTime nextTimeToChangeCircle;
		public Poker_PlayerChangeTurn(MessageReceiving _mess, PokerGamePlayData _pokerGamePlayData){
			// byte indexPlayingBuffer : người vừa mới cược
			// byte stateBetting : trạng thái người vừa mới cược
			// long circleBet : mức cược vừa mới bỏ ra của người chơi
			// long goldKeepBeting : tổng cược của thằng vừa mới cược
			// long GOLD : số gold của người chơi
			// long maxBet : số tiền cược lớn nhất hiện tại của bàn
			// byte indexofNewTurn : turn sẽ đến người này (time trong tableinfo)
			// byte status
			// ***Nếu status = STATUS_TURN_2_CHIA_3 thì đọc byte[3]
			// ***Nếu status = STATUS_TURN_3_CHIA_4 thì đọc byte[4]
			// ***Nếu status = STATUS_TURN_4_CHIA_5 thì đọc byte[5]

			lastPlayer_IndexCircle = _mess.readByte();
			lastPlayer_CurrentState = (Poker_PlayerPlayingData.State) _mess.readByte();
			lastPlayer_CircleBet = _mess.readLong();
			// Debug.LogError(">>>>>> " + playerChangeTurnData.lastPlayer_IndexCircle + " - " + playerChangeTurnData.lastPlayer_CurrentState + " (" + _tmphaha + ") - " + playerChangeTurnData.lastPlayer_CircleBet);
			if(lastPlayer_CircleBet < 0){
				Debug.LogError("BUG logic circleBet: " + lastPlayer_CircleBet);
			}
			lastPlayer_TotalBet = _mess.readLong();
			lastPlayer_GoldRemain = _mess.readLong();
			currentMaxBet = _mess.readLong();
			currentPlayer_IndexCircle = _mess.readByte();
			currentGameState = (GameState) _mess.readByte();
			int _tmpNum = 0;
			if(currentGameState == GameState.STATUS_TURN_2_CHIA_3){
				_tmpNum = 3;
			}else if(currentGameState == GameState.STATUS_TURN_3_CHIA_4){
				_tmpNum = 4;
			}else if(currentGameState == GameState.STATUS_TURN_4_CHIA_5){
				_tmpNum = 5;
			}
			globalCards = new List<sbyte>();
			for(int i = 0; i < _tmpNum; i++){
				globalCards.Add(_mess.readByte());
			}
			
			nextTimeToChangeCircle = System.DateTime.Now.AddMilliseconds(_pokerGamePlayData.timeDurringChangeCircle);
		}
	}
	public List<Poker_PlayerChangeTurn> processPlayerChangeTurnData;

	public class Poker_FinishGame{
		// byte numberWin : số người chiến thắng
		// for(numberWin){
		// 	byte circleIndexWin
		// 	long goldWin
		// 	int achievement_win
		// }
		// byte[5] lá bài trên bàn 
		// byte circleLength
		// for(circleLength){
		// 	byte card1
		// 	btye card2
		// 	byte stateBetting
		// 	long circleBet
		// 	long turnBet
		// 	long goldKeepBeting
		// 	long GOLD
		// 	int achievementLose
		// }

		public sbyte lastPlayer_IndexCircle;

		public class Poker_FinishGame_PlayerWin{
			public sbyte circleIndexWin;
			public long goldWin;
			public int achievementWin;
		}
		public List<Poker_FinishGame_PlayerWin> listPlayerWin;
		public List<sbyte> listFullGlobalCards;

		public class Poker_FinishGame_TemporaryPlayer{
			public sbyte card1;
			public sbyte card2;
			public sbyte currentState;
			public long circleBet;
			public long turnBet;
			public long totalBet;
			public long goldWinOrReturn; // số gold thắng hoặc gold dư (all-in lệch cược)
			public long goldRemain;
			public int achievementLose;
		}
		public List<Poker_FinishGame_TemporaryPlayer> listTemporaryPlayer;

		public Poker_FinishGame(MessageReceiving _mess, PokerGamePlayData _pokerGamePlayData){
			listPlayerWin = new List<Poker_FinishGame.Poker_FinishGame_PlayerWin>();
			sbyte _numberWin = _mess.readByte();
			for(int i = 0; i < _numberWin; i ++){
				Poker_FinishGame.Poker_FinishGame_PlayerWin _tmp = new Poker_FinishGame.Poker_FinishGame_PlayerWin();
				_tmp.circleIndexWin = _mess.readByte();
				_tmp.goldWin = _mess.readLong();
				_tmp.achievementWin = _mess.readInt();
				listPlayerWin.Add(_tmp);
			}

			listFullGlobalCards = new List<sbyte>();
			for(int i = 0; i < 5; i++){
				sbyte _card = _mess.readByte();
				listFullGlobalCards.Add(_card);
				if(i < _pokerGamePlayData.globalCards.Count){
					if(_pokerGamePlayData.globalCards[i] != _card){
						#if TEST
						Debug.LogError(">>> Trả sai globalCards : " + _pokerGamePlayData.globalCards[i] + " - " + _card);
						#endif
					}
				}
			}

			listTemporaryPlayer = new List<Poker_FinishGame.Poker_FinishGame_TemporaryPlayer>();
			Poker_FinishGame.Poker_FinishGame_TemporaryPlayer _temporaryPlayer = null;
			for(int i = 0; i < _pokerGamePlayData.listPlayerPlayingData.Count; i++){
				_temporaryPlayer = new Poker_FinishGame.Poker_FinishGame_TemporaryPlayer();
				_temporaryPlayer.card1 = _mess.readByte();
				_temporaryPlayer.card2 = _mess.readByte();
				if(_pokerGamePlayData.listPlayerPlayingData[i].isMe){
					if(_temporaryPlayer.card1 != _pokerGamePlayData.listPlayerPlayingData[i].ownCards[0]){
						#if TEST
						Debug.LogError(">>> Trả sai owncards 0: " + _pokerGamePlayData.listPlayerPlayingData[i].ownCards[0] + " - " + _temporaryPlayer.card1);
						#endif
					}
					if(_temporaryPlayer.card2 != _pokerGamePlayData.listPlayerPlayingData[i].ownCards[1]){
						#if TEST
						Debug.LogError(">>> Trả sai owncards 1: " + _pokerGamePlayData.listPlayerPlayingData[i].ownCards[1] + " - " + _temporaryPlayer.card2);
						#endif
					}
				}
				
				_temporaryPlayer.currentState = _mess.readByte();
				_temporaryPlayer.circleBet = _mess.readLong();
				_temporaryPlayer.turnBet = _mess.readLong();
				_temporaryPlayer.totalBet = _mess.readLong();
				_temporaryPlayer.goldWinOrReturn = _mess.readLong();
				_temporaryPlayer.goldRemain = _mess.readLong();
				_temporaryPlayer.achievementLose = _mess.readInt();
				listTemporaryPlayer.Add(_temporaryPlayer);

				if(_pokerGamePlayData.currentCircle == i){
					// Debugs.LogGreen(">>> Finish: " + listPlayerPlayingData[i].turnBet + " | " + listPlayerPlayingData[i].totalBet);
					lastPlayer_IndexCircle = _pokerGamePlayData.currentCircle;
				}
			}
		}
	}
	public List<Poker_FinishGame> processFinishGameData;

	public List<UserDataInGame> listGlobalPlayerData;
	public List<Poker_PlayerPlayingData> listPlayerPlayingData; // list danh sách user đang chơi
	public List<short> listSessionIdGlobalPlayer;
	public List<short> listSessionIdOnChair;
	public List<short> listSessionIdPlaying;

	
	public long totalBet;
	public long betDefault;
	public sbyte numberChairs;
    public System.DateTime nextTimeToStartGame;
    public System.DateTime nextTimeToChangeCircle;
	public int timeDurringChangeCircle;  // tính theo milisecond
	public sbyte currentCircle;

	public List<sbyte> globalCards;
	
	public long maxBet; // số tiền cược lớn nhất hiện tại của bàn
	public int totalRaiseInTurn{get;set;}

	public bool hasLoadTableInfo;

	public PokerGamePlayData(){
		listGlobalPlayerData = new List<UserDataInGame>();
		listPlayerPlayingData = new List<Poker_PlayerPlayingData>();
		listSessionIdGlobalPlayer = new List<short>();
		listSessionIdOnChair = new List<short>();
		listSessionIdPlaying = new List<short>();
		globalCards = new List<sbyte>();
		
		processPlayerJoinGame = new List<PlayerJoinGame_Data>();
		processPlayerLeftGame = new List<PlayerLeftGame_Data>();
		processPlayerSitDown = new List<PlayerSitDown_Data>();
		processMeSitDownFail = new List<MeSitDownFail_Data>();
		processPlayerStandUp = new List<PlayerStandUp_Data>();
		processPlayerChatData = new List<PlayerChat_Data>();
		processPlayerAddGoldData = new List<PlayerAddGold_Data>();
		processPlayerSetParentData = new List<PlayerSetParent_Data>();
		
		processGameReadyData = new List<Poker_GameReady_Data>();
		processStartGameData = new List<Poker_StartGame_Data>();
		processPlayerChangeTurnData = new List<Poker_PlayerChangeTurn>();
		processFinishGameData = new List<Poker_FinishGame>();

		nextTimeToStartGame = System.DateTime.Now;
		nextTimeToChangeCircle = System.DateTime.Now;

		hasLoadTableInfo = false;
	}

	public bool CheckIfIsMe(short _sessionId){
		if(_sessionId == DataManager.instance.userData.sessionId){
			return true;
		}
		return false;
	}

	public bool CheckIsPlaying(short _sessionId){
		if(!listSessionIdPlaying.Contains(_sessionId)){
			return false;
		}
		int _index = listSessionIdPlaying.IndexOf(_sessionId);
		if(_index < 0){
			return false;
		}
		int _indexChair = listPlayerPlayingData[_index].indexChair;
		if(listSessionIdOnChair[_indexChair] != _sessionId){
			return false;
		}

		return true;
	}

	public void UpdateGoldAgain(short _sessionId, long _goldRemain){
		if(CheckIfIsMe(_sessionId)){
			DataManager.instance.userData.gold = _goldRemain;
		}
		int _indexGlobal = listSessionIdGlobalPlayer.IndexOf(_sessionId);
		if(_indexGlobal >= 0){
			listGlobalPlayerData[_indexGlobal].gold = _goldRemain;
		}
	}

	public void InitDataWhenGetTableInfo(MessageReceiving _mess){
		InitListOtherUserDataInGame(_mess);
		InitListOnChair(_mess);

		betDefault = _mess.readLong();
		timeDurringChangeCircle = _mess.readInt();
		currentGameState = (GameState) _mess.readByte();
		// ***Nếu status = STATUS_TURN_2_CHIA_3 thì đọc byte[3]
		// ***Nếu status = STATUS_TURN_3_CHIA_4 thì đọc byte[4]
		// ***Nếu status = STATUS_TURN_4_CHIA_5 thì đọc byte[5]
		int _tmpNum = 0;
		if(currentGameState == GameState.STATUS_TURN_2_CHIA_3){
			_tmpNum = 3;
		}else if(currentGameState == GameState.STATUS_TURN_3_CHIA_4){
			_tmpNum = 4;
		}else if(currentGameState == GameState.STATUS_TURN_4_CHIA_5){
			_tmpNum = 5;
		}
		globalCards = new List<sbyte>();
		for(int i = 0; i < _tmpNum; i++){
			globalCards.Add(_mess.readByte());
		}
		totalBet = 0;
		maxBet = 0;

		if(currentGameState == GameState.STATUS_WAIT_FOR_PLAYER){
			long _timeLeftToStartGame = _mess.readLong(); // thời gian đếm ngược để start game (nếu dương)
			if(_timeLeftToStartGame > 0){
				nextTimeToStartGame = System.DateTime.Now.AddMilliseconds(_timeLeftToStartGame);
			}else{
				nextTimeToStartGame = System.DateTime.Now;
			}
		}else{
			nextTimeToStartGame = System.DateTime.Now;

			currentCircle = _mess.readByte();
			long _timeLeftToChangeCircle = _mess.readLong();
			nextTimeToChangeCircle = System.DateTime.Now.AddMilliseconds(_timeLeftToChangeCircle);
			sbyte _circleLength = _mess.readByte();
			listPlayerPlayingData = new List<Poker_PlayerPlayingData>();
			listSessionIdPlaying = new List<short>();
			for(int i = 0; i < _circleLength; i ++){
				// i = 0 : Small Blind, i = 1 : Big Blind
				Poker_PlayerPlayingData _tmpData = new Poker_PlayerPlayingData(_mess, listSessionIdGlobalPlayer);
				listPlayerPlayingData.Add(_tmpData);
				listSessionIdPlaying.Add(_tmpData.userData.sessionId);

				totalBet += _tmpData.totalBet;
				if(maxBet < _tmpData.totalBet){
					maxBet = _tmpData.totalBet;
				}
			}
		}
		hasLoadTableInfo = true;
	}

	public UserDataInGame GetUserDataInGameFromListGlobal(short _sessionId){
		if(_sessionId < 0){
			return null;
		}
		if(listGlobalPlayerData == null || listGlobalPlayerData.Count == 0){
			#if TEST
            Debug.LogError(">>> listGlobalPlayerData is NULL");
            #endif
			return null;
		}
		if(listSessionIdGlobalPlayer == null || listSessionIdGlobalPlayer.Count == 0){
			#if TEST
            Debug.LogError(">>> listSessionIdGlobalPlayer is NULL");
            #endif
			return null;
		}
		int _index = listSessionIdGlobalPlayer.IndexOf(_sessionId);
		if(_index < 0){
			#if TEST
            Debug.LogError(">>> Không tìm thấy (1)");
            #endif
			return null;
		}
		if(_index >= listGlobalPlayerData.Count){
			#if TEST
            Debug.LogError(">>> Không tìm thấy (2)");
            #endif
			return null;
		}
		if(listGlobalPlayerData[_index] == null){
			#if TEST
            Debug.LogError(">>> Không tìm thấy (3)");
            #endif
			return null;
		}
		return listGlobalPlayerData[_index];
	}

	public void InitListOtherUserDataInGame(MessageReceiving _mess){
        listGlobalPlayerData = new List<UserDataInGame>();
		listSessionIdGlobalPlayer = new List<short>();
		UserDataInGame _userdata = null;
		sbyte _maxViewer = _mess.readByte();
		for(int i = 0; i < (int)_maxViewer; i++){
			sbyte _caseCheck = _mess.readByte(); //(nếu giá trị -1 thì không đọc data dưới --> tiếp tục vòng for)
			if(_caseCheck != -1){
				short _sessionId = _mess.readShort();
				_userdata = new UserDataInGame(_mess, _sessionId, (sbyte) i);
				if(CheckIfIsMe(_userdata.sessionId)){
					int _countWin = _userdata.win;
					int _countTie = _userdata.tie;
					int _countLose = _userdata.lose;
					_userdata = DataManager.instance.userData.CastToUserDataInGame();
					_userdata.index = (sbyte) i;
					_userdata.win = _countWin;
					_userdata.tie = _countTie;
					_userdata.lose = _countLose;
					AchievementDetail _achievementDetail = DataManager.instance.achievementData.GetAchievementDetail(IMiniGameInfo.Type.Poker);
					if(_achievementDetail != null){
						_achievementDetail.countWin = _userdata.win;
						_achievementDetail.countDraw = _userdata.tie;
						_achievementDetail.countLose = _userdata.lose;
						// Debug.Log(_usedata.win + " - " + _usedata.tie + " - " + _usedata.lose);
					}else{
						Debug.LogError(">>> _achievementDetail is null");
					}
				}
				listGlobalPlayerData.Add(_userdata);
				listSessionIdGlobalPlayer.Add(_sessionId);
			}else{
				_userdata = new UserDataInGame();
				listGlobalPlayerData.Add(_userdata);
				listSessionIdGlobalPlayer.Add(-1);
			}
			// Debug.LogError(_usedata.sessionId + " - " + DataManager.instance.userData.sessionId);
		}
	}

	public void SetUpUserJoinGame(MessageReceiving _mess){
		PlayerJoinGame_Data _data = new PlayerJoinGame_Data(_mess);
		processPlayerJoinGame.Add(_data);
    }
    public void SetUpUserLeftGame(MessageReceiving _mess){
		PlayerLeftGame_Data _data = new PlayerLeftGame_Data(_mess);
		processPlayerLeftGame.Add(_data);
    }

	public void InitListOnChair(MessageReceiving _mess){
		numberChairs = _mess.readByte();
		if(numberChairs != 9){
			#if TEST
			Debug.LogError("Sai dữ liệu ghế: " + numberChairs);
			#endif
		}
		listSessionIdOnChair = new List<short>();
		for(int i = 0; i < numberChairs; i ++){
			sbyte _checkCase = _mess.readByte(); // nếu = -1 : vị trí trống, còn lại thì đọc tiếp
			if(_checkCase == -1){
				listSessionIdOnChair.Add(-1);
			}else{
				short _sessionId = _mess.readShort();
				listSessionIdOnChair.Add(_sessionId);
			}
		}
	}

	public void SetUpPlayerSitDown(MessageReceiving _mess){
		PlayerSitDown_Data _data = new PlayerSitDown_Data(_mess);
		processPlayerSitDown.Add(_data);
	}

	public void SetUpPlayerStandUp(MessageReceiving _mess){
		PlayerStandUp_Data _data = new PlayerStandUp_Data(_mess);
		processPlayerStandUp.Add(_data);
	}

	public void SetUpMeSitDownFail(MessageReceiving _mess){
		MeSitDownFail_Data _data = new MeSitDownFail_Data(_mess);
		processMeSitDownFail.Add(_data);

		if(!_data.isSuccess){
			if(_data.myGold < betDefault){
				PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kWarning)
					, string.Format(MyLocalize.GetString("System/Message_PlayGame_NotEnoughMoney"), MyConstant.GetMoneyString(betDefault, 9999))
					, string.Empty
					, MyLocalize.GetString(MyLocalize.kOk)
					, null);
			}else{
				PopupManager.Instance.CreateToast(MyLocalize.GetString("Error/GamePlay_CanNotSitDown"));
			}
		}
	}

	public void SetDataWhenReady(MessageReceiving _mess){
		Poker_GameReady_Data _data = new Poker_GameReady_Data(_mess);
		processGameReadyData.Add(_data);
	}

	public void SetDataWhenStartGame(MessageReceiving _mess){
		Poker_StartGame_Data _data = new Poker_StartGame_Data(_mess, this);
		processStartGameData.Add(_data);
	}

	public void SetDataWhenSetParent(MessageReceiving _mess){
		PlayerSetParent_Data _data = new PlayerSetParent_Data(_mess);
		processPlayerSetParentData.Add(_data);
	}

	public void SetDataWhenChangeTurn(MessageReceiving _mess){
		Poker_PlayerChangeTurn _data = new Poker_PlayerChangeTurn(_mess, this);
		processPlayerChangeTurnData.Add(_data);

		// playerChangeTurnData = new Poker_PlayerChangeTurn();
		// playerChangeTurnData.lastPlayer_IndexCircle = _mess.readByte();
		// playerChangeTurnData.lastPlayer_CurrentState = (Poker_PlayerPlayingData.State) _mess.readByte();
		// playerChangeTurnData.lastPlayer_CircleBet = _mess.readLong();
		// // Debug.LogError(">>>>>> " + playerChangeTurnData.lastPlayer_IndexCircle + " - " + playerChangeTurnData.lastPlayer_CurrentState + " (" + _tmphaha + ") - " + playerChangeTurnData.lastPlayer_CircleBet);
		// if(playerChangeTurnData.lastPlayer_CircleBet < 0){
		// 	Debug.LogError("BUG logic circleBet: " + playerChangeTurnData.lastPlayer_CircleBet);
		// }
		// playerChangeTurnData.lastPlayer_TotalBet = _mess.readLong();
		// playerChangeTurnData.lastPlayer_GoldRemain = _mess.readLong();
		// playerChangeTurnData.currentMaxBet = _mess.readLong();
		// playerChangeTurnData.currentPlayer_IndexCircle = _mess.readByte();
		// playerChangeTurnData.currentGameState = (GameState) _mess.readByte();
		// playerChangeTurnData.hasNewTurn = false;
		// playerChangeTurnData.hasNewCircle = false;
		// if((sbyte) playerChangeTurnData.currentGameState > (sbyte) currentGameState){
		// 	playerChangeTurnData.hasNewTurn = true;
		// }else if((sbyte) playerChangeTurnData.currentGameState < (sbyte) currentGameState){
		// 	#if TEST
		// 	Debug.LogError("BUG logic _newGameState: " + playerChangeTurnData.currentGameState + " (" + (int)playerChangeTurnData.currentGameState+")" + " | " + currentGameState + " (" + (int)currentGameState+")");
		// 	#endif
		// }else{
		// 	if(playerChangeTurnData.lastPlayer_CurrentState == Poker_PlayerPlayingData.State.STATEPOKER_RAISE
		// 		|| playerChangeTurnData.lastPlayer_CurrentState == Poker_PlayerPlayingData.State.STATEPOKER_ALLIN){
		// 		if(playerChangeTurnData.lastPlayer_TotalBet > maxBet){
		// 			totalRaiseInTurn ++;
		// 			playerChangeTurnData.hasNewCircle = true;
		// 		}
		// 	}
		// }
		// int _tmpNum = 0;
		// if(playerChangeTurnData.currentGameState == GameState.STATUS_TURN_2_CHIA_3){
		// 	_tmpNum = 3;
		// }else if(playerChangeTurnData.currentGameState == GameState.STATUS_TURN_3_CHIA_4){
		// 	_tmpNum = 4;
		// }else if(playerChangeTurnData.currentGameState == GameState.STATUS_TURN_4_CHIA_5){
		// 	_tmpNum = 5;
		// }
		// playerChangeTurnData.globalCards = new List<sbyte>();
		// for(int i = 0; i < _tmpNum; i++){
		// 	playerChangeTurnData.globalCards.Add(_mess.readByte());
		// }
		// playerChangeTurnData.newGlobalCards = new List<sbyte>();
		// if(playerChangeTurnData.hasNewTurn){
		// 	for(int i = globalCards.Count; i < _tmpNum; i++){
		// 		playerChangeTurnData.newGlobalCards.Add(playerChangeTurnData.globalCards[i]);
		// 	}
		// }

		// // --- Set dữ liệu player change turn vào cho dữ liệu tổng --- //
		// listPlayerPlayingData[playerChangeTurnData.lastPlayer_IndexCircle].currentState = playerChangeTurnData.lastPlayer_CurrentState;
		// globalCards = playerChangeTurnData.globalCards;

		// listPlayerPlayingData[playerChangeTurnData.lastPlayer_IndexCircle].userData.gold = playerChangeTurnData.lastPlayer_GoldRemain;
		// listPlayerPlayingData[playerChangeTurnData.lastPlayer_IndexCircle].turnBet += playerChangeTurnData.lastPlayer_CircleBet;
		// listPlayerPlayingData[playerChangeTurnData.lastPlayer_IndexCircle].totalBet = playerChangeTurnData.lastPlayer_TotalBet;
		// totalBet += playerChangeTurnData.lastPlayer_CircleBet;
		
		// // if(listPlayerPlayingData[playerChangeTurnData.lastPlayer_IndexCircle].isMe){
		// // 	Debugs.LogGreen(">>> Change turn" + listPlayerPlayingData[playerChangeTurnData.lastPlayer_IndexCircle].userData.gold + " | " + listPlayerPlayingData[playerChangeTurnData.lastPlayer_IndexCircle].turnBet + " | " + listPlayerPlayingData[playerChangeTurnData.lastPlayer_IndexCircle].totalBet + " | " + totalBet);
		// // }
		
		// if(playerChangeTurnData.hasNewCircle){
		// 	for(int i = 0; i < listPlayerPlayingData.Count; i ++){
		// 		if(i != playerChangeTurnData.lastPlayer_IndexCircle){
		// 			if(listPlayerPlayingData[i].currentState != Poker_PlayerPlayingData.State.STATEPOKER_FOLD
		// 				&& listPlayerPlayingData[i].currentState != Poker_PlayerPlayingData.State.STATEPOKER_ALLIN){
		// 				listPlayerPlayingData[i].currentState = Poker_PlayerPlayingData.State.None;
		// 			}
		// 		}
		// 	}
		// }

		// UpdateGoldAgain(listPlayerPlayingData[playerChangeTurnData.lastPlayer_IndexCircle].userData.sessionId, playerChangeTurnData.lastPlayer_GoldRemain);
		// maxBet = playerChangeTurnData.currentMaxBet;
		// currentCircle = playerChangeTurnData.currentPlayer_IndexCircle;
		// currentGameState = playerChangeTurnData.currentGameState;
		// timeLeftToChangeCircle = timeDurringChangeCircle;
		// nextTimeToChangeCircle = System.DateTime.Now.AddMilliseconds(timeLeftToChangeCircle);
	}

	public void ResetNewTurn(){
		for(int i = 0; i < listPlayerPlayingData.Count; i++){
			if(listPlayerPlayingData[i].currentState != Poker_PlayerPlayingData.State.STATEPOKER_FOLD
				&& listPlayerPlayingData[i].currentState != Poker_PlayerPlayingData.State.STATEPOKER_ALLIN){
				listPlayerPlayingData[i].currentState = Poker_PlayerPlayingData.State.None;
			}
			listPlayerPlayingData[i].turnBet = 0;
		}
		totalRaiseInTurn = 0;
	}

	public void SetDataWhenFinishGame(MessageReceiving _mess){
		Poker_FinishGame _data = new Poker_FinishGame(_mess, this);
		processFinishGameData.Add(_data);

		// finishGameData = new Poker_FinishGame();

		// finishGameData.listPlayerWin = new List<Poker_FinishGame.Poker_FinishGame_PlayerWin>();
		// sbyte _numberWin = _mess.readByte();
		// for(int i = 0; i < _numberWin; i ++){
		// 	Poker_FinishGame.Poker_FinishGame_PlayerWin _tmp = new Poker_FinishGame.Poker_FinishGame_PlayerWin();
		// 	_tmp.circleIndexWin = _mess.readByte();
		// 	_tmp.goldWin = _mess.readLong();
		// 	_tmp.achievementWin = _mess.readInt();
		// 	finishGameData.listPlayerWin.Add(_tmp);
		// }
		// finishGameData.newGlobalCards = new List<sbyte>(); 
		// finishGameData.listFullGlobalCards = new List<sbyte>();
		// for(int i = 0; i < 5; i++){
		// 	sbyte _card = _mess.readByte();
		// 	finishGameData.listFullGlobalCards.Add(_card);
		// 	if(i < globalCards.Count){
		// 		if(globalCards[i] != _card){
		// 			#if TEST
		// 			Debug.LogError(">>> Trả sai globalCards : " + globalCards[i] + " - " + _card);
		// 			#endif
		// 		}
		// 	}
		// }

		// finishGameData.listTemporaryPlayer = new List<Poker_FinishGame.Poker_FinishGame_TemporaryPlayer>();
		// Poker_FinishGame.Poker_FinishGame_TemporaryPlayer _temporaryPlayer = null;
		// for(int i = 0; i < listPlayerPlayingData.Count; i++){
		// 	_temporaryPlayer = new Poker_FinishGame.Poker_FinishGame_TemporaryPlayer();
		// 	_temporaryPlayer.card1 = _mess.readByte();
		// 	_temporaryPlayer.card2 = _mess.readByte();
		// 	if(listPlayerPlayingData[i].isMe){
		// 		if(_temporaryPlayer.card1 != listPlayerPlayingData[i].ownCards[0]){
		// 			#if TEST
		// 			Debug.LogError(">>> Trả sai owncards : " + listPlayerPlayingData[i].ownCards[0] + " - " + _temporaryPlayer.card1);
		// 			#endif
		// 		}
		// 		if(_temporaryPlayer.card2 != listPlayerPlayingData[i].ownCards[1]){
		// 			#if TEST
		// 			Debug.LogError(">>> Trả sai owncards : " + listPlayerPlayingData[i].ownCards[01] + " - " + _temporaryPlayer.card2);
		// 			#endif
		// 		}
		// 	}
			
		// 	_temporaryPlayer.currentState = _mess.readByte();
		// 	_temporaryPlayer.circleBet = _mess.readLong();
		// 	_temporaryPlayer.turnBet = _mess.readLong();
		// 	_temporaryPlayer.totalBet = _mess.readLong();
		// 	_temporaryPlayer.goldWinOrReturn = _mess.readLong();
		// 	_temporaryPlayer.goldRemain = _mess.readLong();
		// 	_temporaryPlayer.achievementLose = _mess.readInt();
		// 	finishGameData.listTemporaryPlayer.Add(_temporaryPlayer);

		// 	// --- Cập nhật 1 vài thông số vào dữ liệu thật --- //
		// 	listPlayerPlayingData[i].turnBet = _temporaryPlayer.turnBet;
		// 	listPlayerPlayingData[i].totalBet = _temporaryPlayer.totalBet;
		// 	listPlayerPlayingData[i].goldWinOrReturn = _temporaryPlayer.goldWinOrReturn;
		// 	if(currentCircle == i){
		// 		// Debugs.LogGreen(">>> Finish: " + listPlayerPlayingData[i].turnBet + " | " + listPlayerPlayingData[i].totalBet);
		// 		finishGameData.lastPlayer_IndexCircle = currentCircle;
		// 		listPlayerPlayingData[i].currentState = (Poker_PlayerPlayingData.State)_temporaryPlayer.currentState;
		// 		listPlayerPlayingData[i].userData.gold -= _temporaryPlayer.circleBet;
		// 		UpdateGoldAgain(listPlayerPlayingData[i].userData.sessionId, listPlayerPlayingData[i].userData.gold);
		// 	}

		// 	if(listPlayerPlayingData[i].currentState != Poker_PlayerPlayingData.State.STATEPOKER_FOLD){
		// 		listPlayerPlayingData[i].ownCards[0] = _temporaryPlayer.card1;
		// 		listPlayerPlayingData[i].ownCards[1] = _temporaryPlayer.card2;
		// 	}
		// }

		// // -- Check trường hợp chia xong 5 lá và tất cả đều fold, chỉ có 1 người trên bàn
		// int _tmpCountFold = 0;
		// for(int i = 0; i < listPlayerPlayingData.Count; i++){
		// 	if(listPlayerPlayingData[i].currentState == Poker_PlayerPlayingData.State.STATEPOKER_FOLD){
		// 		_tmpCountFold ++;
		// 		continue;
		// 	}
		// }
		// finishGameData.canShowCards = false;
		// if(_tmpCountFold != listPlayerPlayingData.Count - 1){
		// 	finishGameData.canShowCards = true;
		// 	if(globalCards.Count != finishGameData.listFullGlobalCards.Count){
		// 		for(int i = globalCards.Count; i < finishGameData.listFullGlobalCards.Count; i++){
		// 			finishGameData.newGlobalCards.Add(finishGameData.listFullGlobalCards[i]);
		// 		}
		// 	}
		// 	globalCards = finishGameData.listFullGlobalCards;

		// 	for(int i = 0; i < listPlayerPlayingData.Count; i++){
		// 		if(listPlayerPlayingData[i].currentState != Poker_PlayerPlayingData.State.STATEPOKER_FOLD){
		// 			CheckResultCard(listPlayerPlayingData[i].ownCards, globalCards, (_typeCardResult, _cardHighLight)=>{
		// 				listPlayerPlayingData[i].typeCardResult = _typeCardResult;
		// 				listPlayerPlayingData[i].highLightCardsResult = _cardHighLight;
		// 			});
		// 		}
		// 	}

		// 	historyData = new Poker_HistoryData();
		// 	for(int i = 0; i < finishGameData.listPlayerWin.Count; i ++){
		// 		historyData.circleIndexWin.Add(finishGameData.listPlayerWin[i].circleIndexWin);
		// 	}
		// 	for(int i = 0; i < globalCards.Count; i++){
		// 		historyData.globalCards.Add(globalCards[i]);
		// 	}
		// 	for(int i = 0; i < listPlayerPlayingData.Count; i++){
		// 		historyData.listPlayerPlayingData.Add(listPlayerPlayingData[i]);
		// 	}
		// }
	}

	public void SetPlayerChatData(MessageReceiving _mess){
		PlayerChat_Data _data = new PlayerChat_Data(_mess);
		processPlayerChatData.Add(_data);
	}

	public void SetPlayerAddGoldData(MessageReceiving _mess){
		PlayerAddGold_Data _data = new PlayerAddGold_Data(_mess);
		processPlayerAddGoldData.Add(_data);
	}

	public static void GetPercentTypeCard(List<sbyte> _ownCards, List<sbyte> _globalCards, System.Action<float[]> _result){
		if(_globalCards.Count < 3 || _ownCards.Count != 2 || _ownCards[0] < 0 || _ownCards[1] < 0){
			if(_result != null){
				_result(null);
			}
			return;
		}

		float[] _tmpPercentProcess = new float[9];
		_tmpPercentProcess[0] = 0f;
		_tmpPercentProcess[1] = 0f;
		_tmpPercentProcess[2] = 0f;
		_tmpPercentProcess[3] = 0f;
		_tmpPercentProcess[4] = 0f;
		_tmpPercentProcess[5] = 0f;
		_tmpPercentProcess[6] = 0f;
		_tmpPercentProcess[7] = 0f;
		_tmpPercentProcess[8] = 0f;

		short[] _listCount = new short[9];
		_listCount[0] = 0;
		_listCount[1] = 0;
		_listCount[2] = 0;
		_listCount[3] = 0;
		_listCount[4] = 0;
		_listCount[5] = 0;
		_listCount[6] = 0;
		_listCount[7] = 0;
		_listCount[8] = 0;

		short _total = 0; 

		if(_globalCards.Count == 3){
			for(int i = 0; i < 52; i++){
				if(!_ownCards.Contains((sbyte) i) && !_globalCards.Contains((sbyte) i)){
					for(int j = i + 1; j < 52; j++){
						if(!_ownCards.Contains((sbyte) j) && !_globalCards.Contains((sbyte) j)){
							List<sbyte> _newGlobalCards = new List<sbyte>();
							for(int m = 0; m < _globalCards.Count; m ++){
								_newGlobalCards.Add(_globalCards[m]);
							}
							_newGlobalCards.Add((sbyte) i);
							_newGlobalCards.Add((sbyte) j);

							_listCount[((short) GetTypeCardResult(_ownCards, _newGlobalCards)) - 1] ++;
							_total ++;
						}
					}
				}
			}
		}else if(_globalCards.Count == 4){
			for(int i = 0; i < 52; i++){
				if(!_ownCards.Contains((sbyte) i) && !_globalCards.Contains((sbyte) i)){
					List<sbyte> _newGlobalCards = new List<sbyte>();
					for(int m = 0; m < _globalCards.Count; m ++){
						_newGlobalCards.Add(_globalCards[m]);
					}
					_newGlobalCards.Add((sbyte) i);

					_listCount[((short) GetTypeCardResult(_ownCards, _newGlobalCards)) - 1] ++;
					_total ++;
				}
			}
		}else{
			_listCount[((short) GetTypeCardResult(_ownCards, _globalCards)) - 1] ++;
			_total ++;
		}

		for(int i = 0 ; i < 9; i ++){
			_tmpPercentProcess[i] = (float) (_listCount[i]) / (float) _total * 100f;
		}
		if(_result != null){
			_result(_tmpPercentProcess);
		}
	}

	public static TypeCardResult GetTypeCardResult(List<sbyte> _ownCard, List<sbyte> _globalCard) {		
		List<sbyte> _listCards = new List<sbyte>();
		_listCards.Add(_ownCard[0]);
		_listCards.Add(_ownCard[1]);
		for(int i = 0; i < _globalCard.Count; i++){
			_listCards.Add(_globalCard[i]);
		}

		_listCards.Sort(delegate (sbyte x, sbyte y) // xếp tăng dần
        {
            // Debug.Log(x + " - " + y + " - " + x.CompareTo(y));
            return x.CompareTo(y);
        });

		// string _tmp2 = "";
		// for(int i = 0; i < _listCards.Count; i++){
		// 	_tmp2 += _listCards[i] + "|";
		// }
		// Debug.Log(">>> listCards: " + _tmp2);
		
		///////1.Thùng phá sảnh
		if(_listCards[2]==_listCards[3]-1) {//Chung 23
			if(_listCards[3]==_listCards[4]-1 && _listCards[4]==_listCards[5]-1) {//23456
				if(_listCards[5]==_listCards[6]-1) {
					if(_listCards[2]%13<9) {
						return TypeCardResult.TYPECARD_STRAIGHT_FLUSH;
					}
				}
				
				if(_listCards[2]==_listCards[6]-12) {
					if(_listCards[2]%13==0) {
						return TypeCardResult.TYPECARD_STRAIGHT_FLUSH;
					}
				}
			}
			
			if(_listCards[1]==_listCards[2]-1 && _listCards[3]==_listCards[4]-1) {//1234
				if(_listCards[4]==_listCards[5]-1) {//12345
					if(_listCards[1]%13<9) {
						return TypeCardResult.TYPECARD_STRAIGHT_FLUSH;
					}
				}
				
				if(_listCards[1]%13==0){
					if(_listCards[1]==_listCards[5]-12){ //12345
						return TypeCardResult.TYPECARD_STRAIGHT_FLUSH;
					}
					if(_listCards[1]==_listCards[6]-12){ //12346
						return TypeCardResult.TYPECARD_STRAIGHT_FLUSH;
					}
				}
			}
			
			if(_listCards[0]==_listCards[1]-1 && _listCards[1]==_listCards[2]-1) {
				if(_listCards[3]==_listCards[4]-1) {//01234
					if(_listCards[0]%13<9) {
						return TypeCardResult.TYPECARD_STRAIGHT_FLUSH;
					}
				}

				if(_listCards[0]%13==0) {
					if(_listCards[0]==_listCards[4]-12){ //01234
						return TypeCardResult.TYPECARD_STRAIGHT_FLUSH;
					}
					if(_listCards[0]==_listCards[5]-12){ //01235
						return TypeCardResult.TYPECARD_STRAIGHT_FLUSH;
					}
					if(_listCards[0]==_listCards[6]-12){ //01236
						return TypeCardResult.TYPECARD_STRAIGHT_FLUSH;
					}
				}
			}
		}
//		2   3   4   5   6   7   8   9   10  J   Q   K   A
//--------------------------------------------------------		
//		0   1   2   3   4   5   6   7   8   9   10  11  12
//		13  14  15  16  17  18  19  20  21  22  23  24  25
//		26  27  28  29  30  31  32  33  34  35  36  37  38
//		39  40  41  42  43  44  45  46  47  48  49  50  51
		sbyte[] vCards = new sbyte[13];
		vCards[0]=0;vCards[1]=0;vCards[2]=0;vCards[3]=0;vCards[4]=0;vCards[5]=0;
		vCards[6]=0;vCards[7]=0;vCards[8]=0;vCards[9]=0;vCards[10]=0;vCards[11]=0;vCards[12]=0;

		vCards[_ownCard[0]%13]++;
		vCards[_ownCard[1]%13]++;
		vCards[_globalCard[0]%13]++;
		vCards[_globalCard[1]%13]++;
		vCards[_globalCard[2]%13]++;
		vCards[_globalCard[3]%13]++;
		vCards[_globalCard[4]%13]++;

		// string _tmp = "";
		// for(int i = 0; i < vCards.Length; i++){
		// 	_tmp += vCards[i] + "|";
		// }
		// Debug.Log(">>> vCards: " + _tmp);

		///////2.Tứ quý
		List<sbyte> _tmpListCards = new List<sbyte>();

		for(sbyte i=12;i>=0;i--){
			if(vCards[i]==4) {
				return TypeCardResult.TYPECARD_FOUR_OF_A_KIND;
			}
		}

		///////3. Cù lũ		
		for(sbyte i=12;i>=0;i--){
			if(vCards[i]==3){
				for(sbyte j=12; j>=0;j--){
					if(vCards[j]>1 && j!=i) {
						return TypeCardResult.TYPECARD_FULL_HOUSE;
					}
				}
			}
		}
		///////4. Thùng --> trên bàn ghép tối đa được 1 cái thùng --> so sánh những lá trong cái thùng
		byte _count;
		sbyte _valueMax;
		for(sbyte chatBai=0;chatBai<4;chatBai++) {
			_count=0;
			_valueMax=-1;
			for(sbyte i=0;i<7;i++){
				if(_listCards[i]/13==chatBai) {
					_count++;
					if(_valueMax<_listCards[i])
						_valueMax=_listCards[i];
				}
			}
			if(_count>4) {
				return TypeCardResult.TYPECARD_FLUSH;
			}
		}
		
		///////5. Sảnh
		sbyte _indexStraiBegin=-1;
		sbyte _indexStraiFinish=-1;
		for(sbyte i=0;i<13;i++){
			if(vCards[i]==0) {
				_indexStraiBegin=-1;
				_indexStraiFinish=-1;
			}else {
				if(_indexStraiBegin == -1) {
					_indexStraiBegin = i;
				}
				_indexStraiFinish = i;
				if(_indexStraiFinish -_indexStraiBegin > 3){
					return TypeCardResult.TYPECARD_STRAIGHT;
				}
			}
		}
		if(vCards[0]>0 && vCards[1]>0 && vCards[2]>0 && vCards[3]>0 && vCards[12]>0) {
			return TypeCardResult.TYPECARD_STRAIGHT;
		}
		
		///////6. Sám cô
		for(sbyte i=12;i>=0;i--){
			if(vCards[i]==3) {
				return TypeCardResult.TYPECARD_THREE_OF_A_KIND;
			}
		}
		
		///////7. Đôi + Thú
		for(sbyte i=12;i>=0;i--){
			if(vCards[i]>1) {
				for(sbyte j=(sbyte) (i-1);j>=0;j--){
					if(vCards[j]>1) {
						return TypeCardResult.TYPECARD_TWO_PAIR;
					}
				}
				return TypeCardResult.TYPECARD_PAIR;
			}
		}
		///////9. Mậu thầu
		return TypeCardResult.TYPECARD_HIGH_CARD;
	}

	public static void CheckResultCard(List<sbyte> _ownCard, List<sbyte> _globalCard, System.Action<TypeCardResult, List<sbyte>> _onResult) {
		TypeCardResult _typeCard = TypeCardResult.TYPECARD_HIGH_CARD;
		List<sbyte> _cardHighlight = new List<sbyte>();
		
		List<sbyte> _listCards = new List<sbyte>();
		_listCards.Add(_ownCard[0]);
		_listCards.Add(_ownCard[1]);
		for(int i = 0; i < _globalCard.Count; i++){
			_listCards.Add(_globalCard[i]);
		}

		_listCards.Sort(delegate (sbyte x, sbyte y) // xếp tăng dần
        {
            // Debug.Log(x + " - " + y + " - " + x.CompareTo(y));
            return x.CompareTo(y);
        });

		// string _tmp2 = "";
		// for(int i = 0; i < _listCards.Count; i++){
		// 	_tmp2 += _listCards[i] + "|";
		// }
		// Debug.Log(">>> listCards: " + _tmp2);
		
		///////1.Thùng phá sảnh
		if(_listCards[2]==_listCards[3]-1) {//Chung 23
			if(_listCards[3]==_listCards[4]-1 && _listCards[4]==_listCards[5]-1) {//23456
				if(_listCards[5]==_listCards[6]-1) {
					if(_listCards[2]%13<9) {
						_typeCard = TypeCardResult.TYPECARD_STRAIGHT_FLUSH;
						_cardHighlight.Clear();
						_cardHighlight.Add(_listCards[2]);
						_cardHighlight.Add(_listCards[3]);
						_cardHighlight.Add(_listCards[4]);
						_cardHighlight.Add(_listCards[5]);
						_cardHighlight.Add(_listCards[6]);
						if(_onResult != null){
							_onResult(_typeCard, _cardHighlight);
						}
						return;
					}
				}
				
				if(_listCards[2]==_listCards[6]-12) {
					if(_listCards[2]%13==0) {
						_typeCard = TypeCardResult.TYPECARD_STRAIGHT_FLUSH;
						_cardHighlight.Clear();
						_cardHighlight.Add(_listCards[2]);
						_cardHighlight.Add(_listCards[3]);
						_cardHighlight.Add(_listCards[4]);
						_cardHighlight.Add(_listCards[5]);
						_cardHighlight.Add(_listCards[6]);
						if(_onResult != null){
							_onResult(_typeCard, _cardHighlight);
						}
						return;
					}
				}
			}
			
			if(_listCards[1]==_listCards[2]-1 && _listCards[3]==_listCards[4]-1) {//1234
				if(_listCards[4]==_listCards[5]-1) {//12345
					if(_listCards[1]%13<9) {
						_typeCard = TypeCardResult.TYPECARD_STRAIGHT_FLUSH;
						_cardHighlight.Clear();
						_cardHighlight.Add(_listCards[1]);
						_cardHighlight.Add(_listCards[2]);
						_cardHighlight.Add(_listCards[3]);
						_cardHighlight.Add(_listCards[4]);
						_cardHighlight.Add(_listCards[5]);
						if(_onResult != null){
							_onResult(_typeCard, _cardHighlight);
						}
						return;
					}
				}
				
				if(_listCards[1]%13==0){
					if(_listCards[1]==_listCards[5]-12){ //12345
						_typeCard = TypeCardResult.TYPECARD_STRAIGHT_FLUSH;
						_cardHighlight.Clear();
						_cardHighlight.Add(_listCards[1]);
						_cardHighlight.Add(_listCards[2]);
						_cardHighlight.Add(_listCards[3]);
						_cardHighlight.Add(_listCards[4]);
						_cardHighlight.Add(_listCards[5]);
						if(_onResult != null){
							_onResult(_typeCard, _cardHighlight);
						}
						return;
					}
					if(_listCards[1]==_listCards[6]-12){ //12346
						_typeCard = TypeCardResult.TYPECARD_STRAIGHT_FLUSH;
						_cardHighlight.Clear();
						_cardHighlight.Add(_listCards[1]);
						_cardHighlight.Add(_listCards[2]);
						_cardHighlight.Add(_listCards[3]);
						_cardHighlight.Add(_listCards[4]);
						_cardHighlight.Add(_listCards[6]);
						if(_onResult != null){
							_onResult(_typeCard, _cardHighlight);
						}
						return;
					}
				}
			}
			
			if(_listCards[0]==_listCards[1]-1 && _listCards[1]==_listCards[2]-1) {
				if(_listCards[3]==_listCards[4]-1) {//01234
					if(_listCards[0]%13<9) {
						_typeCard = TypeCardResult.TYPECARD_STRAIGHT_FLUSH;
						_cardHighlight.Clear();
						_cardHighlight.Add(_listCards[0]);
						_cardHighlight.Add(_listCards[1]);
						_cardHighlight.Add(_listCards[2]);
						_cardHighlight.Add(_listCards[3]);
						_cardHighlight.Add(_listCards[4]);
						if(_onResult != null){
							_onResult(_typeCard, _cardHighlight);
						}
						return;
					}
				}

				if(_listCards[0]%13==0) {
					if(_listCards[0]==_listCards[4]-12){ //01234
						_typeCard = TypeCardResult.TYPECARD_STRAIGHT_FLUSH;
						_cardHighlight.Clear();
						_cardHighlight.Add(_listCards[0]);
						_cardHighlight.Add(_listCards[1]);
						_cardHighlight.Add(_listCards[2]);
						_cardHighlight.Add(_listCards[3]);
						_cardHighlight.Add(_listCards[4]);
						if(_onResult != null){
							_onResult(_typeCard, _cardHighlight);
						}
						return;
					}
					if(_listCards[0]==_listCards[5]-12){ //01235
						_typeCard = TypeCardResult.TYPECARD_STRAIGHT_FLUSH;
						_cardHighlight.Clear();
						_cardHighlight.Add(_listCards[0]);
						_cardHighlight.Add(_listCards[1]);
						_cardHighlight.Add(_listCards[2]);
						_cardHighlight.Add(_listCards[3]);
						_cardHighlight.Add(_listCards[5]);
						if(_onResult != null){
							_onResult(_typeCard, _cardHighlight);
						}
						return;
					}
					if(_listCards[0]==_listCards[6]-12){ //01236
						_typeCard = TypeCardResult.TYPECARD_STRAIGHT_FLUSH;
						_cardHighlight.Clear();
						_cardHighlight.Add(_listCards[0]);
						_cardHighlight.Add(_listCards[1]);
						_cardHighlight.Add(_listCards[2]);
						_cardHighlight.Add(_listCards[3]);
						_cardHighlight.Add(_listCards[6]);
						if(_onResult != null){
							_onResult(_typeCard, _cardHighlight);
						}
						return;
					}
				}
			}
		}
//		2   3   4   5   6   7   8   9   10  J   Q   K   A
//--------------------------------------------------------		
//		0   1   2   3   4   5   6   7   8   9   10  11  12
//		13  14  15  16  17  18  19  20  21  22  23  24  25
//		26  27  28  29  30  31  32  33  34  35  36  37  38
//		39  40  41  42  43  44  45  46  47  48  49  50  51
		sbyte[] vCards = new sbyte[13];
		vCards[0]=0;vCards[1]=0;vCards[2]=0;vCards[3]=0;vCards[4]=0;vCards[5]=0;
		vCards[6]=0;vCards[7]=0;vCards[8]=0;vCards[9]=0;vCards[10]=0;vCards[11]=0;vCards[12]=0;

		vCards[_ownCard[0]%13]++;
		vCards[_ownCard[1]%13]++;
		vCards[_globalCard[0]%13]++;
		vCards[_globalCard[1]%13]++;
		vCards[_globalCard[2]%13]++;
		vCards[_globalCard[3]%13]++;
		vCards[_globalCard[4]%13]++;

		string _tmp = "";
		for(int i = 0; i < vCards.Length; i++){
			_tmp += vCards[i] + "|";
		}
		Debug.Log(">>> vCards: " + _tmp);

		///////2.Tứ quý
		List<sbyte> _tmpListCards = new List<sbyte>();

		for(sbyte i=12;i>=0;i--){
			if(vCards[i]==4) {
				_typeCard = TypeCardResult.TYPECARD_FOUR_OF_A_KIND;
				_cardHighlight.Clear();
				_tmpListCards.Clear();
				for(int p = 0; p < _listCards.Count; p++){
					_tmpListCards.Add(_listCards[p]);
				}
				for(sbyte j = 0; j < _tmpListCards.Count; j++){
					if(_tmpListCards[j] % 13 == i){
						_cardHighlight.Add(_tmpListCards[j]);
						_tmpListCards.RemoveAt(j);
						j--;
						continue;
					}
				}
				if(_cardHighlight.Count != 4){
					#if TEST
					Debug.LogError(">>> Sai Logic check tứ quý (1): (length) " + _cardHighlight.Count);
					#endif
				}
				if(_cardHighlight.Count != 5){
					if(_cardHighlight.Count > 5){
						while(_cardHighlight.Count > 5){
							_cardHighlight.RemoveAt(0);
						}
					}else{
						for(sbyte l = 12; l >= 0; l--){
							if(vCards[l]>=1) {
								for(int k = 0; k < _tmpListCards.Count; k++){
									sbyte _tmpValue = (sbyte)(_tmpListCards[k] % 13); 
									if(_tmpValue == l){
										_cardHighlight.Add(_tmpListCards[k]);
										_tmpListCards.RemoveAt(k);
										break;
									}
								}
								if(_cardHighlight.Count == 5){
									break;
								}
							}
						}
					}
				}
				if(_onResult != null){
					_onResult(_typeCard, _cardHighlight);
				}
				return;
			}
		}

		///////3. Cù lũ		
		for(sbyte i=12;i>=0;i--){
			if(vCards[i]==3){
				for(sbyte j=12; j>=0;j--){
					if(vCards[j]>1 && j!=i) {
						_typeCard = TypeCardResult.TYPECARD_FULL_HOUSE;
						_cardHighlight.Clear();
						_tmpListCards.Clear();
						for(int p = 0; p < _listCards.Count; p++){
							_tmpListCards.Add(_listCards[p]);
						}
						for(sbyte k = 0; k < _tmpListCards.Count; k++){
							if(_tmpListCards[k] % 13 == i || _tmpListCards[k] % 13 == j){
								_cardHighlight.Add(_tmpListCards[k]);
								_tmpListCards.RemoveAt(k);
								k--;
								continue;
							}
						}
						if(_cardHighlight.Count != 5){
							if(_cardHighlight.Count > 5){
								while(_cardHighlight.Count > 5){
									_cardHighlight.RemoveAt(0);
								}
							}else{
								#if TEST
								Debug.LogError(">>> Sai Logic check Cù lũ (1): (length) " + _cardHighlight.Count);
								#endif
							}
						}
						if(_onResult != null){
							_onResult(_typeCard, _cardHighlight);
						}
						return;
					}
				}
			}
		}
		///////4. Thùng --> trên bàn ghép tối đa được 1 cái thùng --> so sánh những lá trong cái thùng
		byte _count;
		sbyte _valueMax;
		for(sbyte chatBai=0;chatBai<4;chatBai++) {
			_count=0;
			_valueMax=-1;
			for(sbyte i=0;i<7;i++){
				if(_listCards[i]/13==chatBai) {
					_count++;
					if(_valueMax<_listCards[i])
						_valueMax=_listCards[i];
				}
			}
			if(_count>4) {
				_typeCard = TypeCardResult.TYPECARD_FLUSH;
//				point=_valueMax;//2 lá trên tay + 5 lá trên bàn chỉ có thể tạo được tối đa 1 cái thùng --> trong 1 trận tối đa có 1 cái thùng
				_cardHighlight.Clear();
				_tmpListCards.Clear();
				for(int p = 0; p < _listCards.Count; p++){
					_tmpListCards.Add(_listCards[p]);
				}
				for(sbyte k = 0; k < _tmpListCards.Count; k++){
					if(_tmpListCards[k] / 13 == chatBai){
						_cardHighlight.Add(_tmpListCards[k]);
						_tmpListCards.RemoveAt(k);
						k--;
						continue;
					}
				}
				if(_cardHighlight.Count != 5){
					if(_cardHighlight.Count > 5){
						while(_cardHighlight.Count > 5){
							_cardHighlight.RemoveAt(0);
						}
					}else{
						#if TEST
						Debug.LogError(">>> Sai Logic check Thùng (1): (length) " + _cardHighlight.Count);
						#endif
					}
				}
				if(_onResult != null){
					_onResult(_typeCard, _cardHighlight);
				}
				return;
			}
		}
		
		///////5. Sảnh
		sbyte _indexStraiBegin=-1;
		sbyte _indexStraiFinish=-1;
		for(sbyte i=0;i<13;i++){
			if(vCards[i]==0) {
				if(_indexStraiFinish-_indexStraiBegin > 3) {
					_typeCard = TypeCardResult.TYPECARD_STRAIGHT;
					_cardHighlight.Clear();
					_tmpListCards.Clear();
					for(int p = 0; p < _listCards.Count; p++){
						_tmpListCards.Add(_listCards[p]);
					}
					for(int m = _indexStraiFinish - 4; m <= _indexStraiFinish; m ++){
						for(int n = 0; n < _tmpListCards.Count; n++){
							sbyte _tmpValue = (sbyte)(_tmpListCards[n] % 13);
							if(_tmpValue == m){
								_cardHighlight.Add(_tmpListCards[n]);
								_tmpListCards.RemoveAt(n);
								break;
							}
						}
					}
					// for(int k = 0; k < _tmpListCards.Count; k++){
					// 	sbyte _tmpValue = (sbyte)(_tmpListCards[k] % 13);
					// 	if(_tmpValue == _indexStraiFinish
					// 		|| _tmpValue == _indexStraiFinish - 1
					// 		|| _tmpValue == _indexStraiFinish - 2
					// 		|| _tmpValue == _indexStraiFinish - 3
					// 		|| _tmpValue == _indexStraiFinish - 4){
					// 			Debug.Log(">>>> _tmpValue: " + _tmpValue + " | " + _tmpListCards[k]);
					// 		_cardHighlight.Add(_tmpListCards[k]);
					// 		_tmpListCards.RemoveAt(k);
					// 		k--;
					// 		continue;
					// 	}
					// }
					if(_cardHighlight.Count < 5){
						#if TEST
						Debug.LogError(">>> Sai Logic check Sảnh (1): (length) " + _cardHighlight.Count);
						#endif
					} else if(_cardHighlight.Count > 5){
						while(_cardHighlight.Count > 5){
							_cardHighlight.RemoveAt(0);
						}
					}
					if(_onResult != null){
						_onResult(_typeCard, _cardHighlight);
					}
					return;
				}
				_indexStraiBegin=-1;
				_indexStraiFinish=-1;
			}else {
				if(_indexStraiBegin==-1) {
					_indexStraiBegin=i;
				}
				_indexStraiFinish=i;
			}
		}
		if(_indexStraiFinish-_indexStraiBegin > 3) {
			_typeCard = TypeCardResult.TYPECARD_STRAIGHT;
			_cardHighlight.Clear();
			_tmpListCards.Clear();
			for(int p = 0; p < _listCards.Count; p++){
				_tmpListCards.Add(_listCards[p]);
			}
			for(int m = _indexStraiFinish - 4; m <= _indexStraiFinish; m ++){
				for(int n = 0; n < _tmpListCards.Count; n++){
					sbyte _tmpValue = (sbyte)(_tmpListCards[n] % 13);
					if(_tmpValue == m){
						_cardHighlight.Add(_tmpListCards[n]);
						_tmpListCards.RemoveAt(n);
						break;
					}
				}
			}
			// for(int k = 0; k < _tmpListCards.Count; k++){
			// 	sbyte _tmpValue = (sbyte)(_tmpListCards[k] % 13); 
			// 	if(_tmpValue== _indexStraiFinish
			// 		|| _tmpValue == _indexStraiFinish - 1
			// 		|| _tmpValue == _indexStraiFinish - 2
			// 		|| _tmpValue == _indexStraiFinish - 3
			// 		|| _tmpValue == _indexStraiFinish - 4){
			// 		_cardHighlight.Add(_tmpListCards[k]);
			// 		_tmpListCards.RemoveAt(k);
			// 		k--;
			// 		continue;
			// 	}
			// }
			if(_cardHighlight.Count < 5){
				#if TEST
				Debug.LogError(">>> Sai Logic check Sảnh (2): (length) " + _cardHighlight.Count);
				#endif
			}else if(_cardHighlight.Count > 5){
				while(_cardHighlight.Count > 5){
					_cardHighlight.RemoveAt(0);
				}
			}
			if(_onResult != null){
				_onResult(_typeCard, _cardHighlight);
			}
			return;
		}
		if(vCards[0]>0 && vCards[1]>0 && vCards[2]>0 && vCards[3]>0 && vCards[12]>0) {
			_typeCard = TypeCardResult.TYPECARD_STRAIGHT;
			_cardHighlight.Clear();
			_tmpListCards.Clear();
			for(int p = 0; p < _listCards.Count; p++){
				_tmpListCards.Add(_listCards[p]);
			}

			List<int> _tmpVCardNeed = new List<int>();
			_tmpVCardNeed.Add(12);
			_tmpVCardNeed.Add(0);
			_tmpVCardNeed.Add(1);
			_tmpVCardNeed.Add(2);
			_tmpVCardNeed.Add(3);

			for(int m = 0; m < _tmpVCardNeed.Count; m ++){
				for(int n = 0; n < _tmpListCards.Count; n++){
					sbyte _tmpValue = (sbyte)(_tmpListCards[n] % 13);
					if(_tmpValue == _tmpVCardNeed[m]){
						_cardHighlight.Add(_tmpListCards[n]);
						_tmpListCards.RemoveAt(n);
						break;
					}
				}
			}

			// for(int k = 0; k < _tmpListCards.Count; k++){
			// 	sbyte _tmpValue = (sbyte)(_tmpListCards[k] % 13); 
			// 	if(_tmpValue == 0
			// 		|| _tmpValue == 1
			// 		|| _tmpValue == 2
			// 		|| _tmpValue == 3
			// 		|| _tmpValue == 12){
			// 		_cardHighlight.Add(_tmpListCards[k]);
			// 		_tmpListCards.RemoveAt(k);
			// 		k--;
			// 		continue;
			// 	}
			// }
			if(_cardHighlight.Count < 5){
				#if TEST
				Debug.LogError(">>> Sai Logic check Sảnh (3): (length) " + _cardHighlight.Count);
				#endif
			} else if(_cardHighlight.Count > 5){
				while(_cardHighlight.Count > 5){
					_cardHighlight.RemoveAt(0);
				}
			}
			if(_onResult != null){
				_onResult(_typeCard, _cardHighlight);
			}
			return;
		}
		
		///////6. Sám cô
		for(sbyte i=12;i>=0;i--){
			if(vCards[i]==3) {
				_typeCard = TypeCardResult.TYPECARD_THREE_OF_A_KIND;
				_cardHighlight.Clear();
				_tmpListCards.Clear();
				for(int p = 0; p < _listCards.Count; p++){
					_tmpListCards.Add(_listCards[p]);
				}
				for(int k = 0; k < _tmpListCards.Count; k++){
					if(_tmpListCards[k] % 13 == i){
						_cardHighlight.Add(_tmpListCards[k]);
						_tmpListCards.RemoveAt(k);
						k--;
						continue;
					}
				}
				if(_cardHighlight.Count != 3){
					#if TEST
					Debug.LogError(">>> Sai Logic check Xám cô (1): (length) " + _cardHighlight.Count);
					#endif
				}
				if(_cardHighlight.Count != 5){
					if(_cardHighlight.Count > 5){
						while(_cardHighlight.Count > 5){
							_cardHighlight.RemoveAt(0);
						}
					}else{
						for(sbyte l = 12; l >= 0; l--){
							if(vCards[l]==1) {
								for(int k = 0; k < _tmpListCards.Count; k++){
									if(_tmpListCards[k] % 13 == l){
										_cardHighlight.Add(_tmpListCards[k]);
										_tmpListCards.RemoveAt(k);
										break;
									}
								}
								if(_cardHighlight.Count == 5){
									break;
								}
							}
						}
					}
				}
				if(_cardHighlight.Count != 5){
					#if TEST
					Debug.LogError(">>> Sai Logic check Xám cô (2): (length) " + _cardHighlight.Count);
					#endif
				}
				if(_onResult != null){
					_onResult(_typeCard, _cardHighlight);
				}
				return;
			}
		}
		
		///////7. Đôi + Thú
		for(sbyte i=12;i>=0;i--){
			if(vCards[i]>1) {
				for(sbyte j=(sbyte) (i-1);j>=0;j--){
					if(vCards[j]>1) {
						_typeCard = TypeCardResult.TYPECARD_TWO_PAIR;
						_cardHighlight.Clear();
						_tmpListCards.Clear();
						for(int p = 0; p < _listCards.Count; p++){
							_tmpListCards.Add(_listCards[p]);
						}
						for(int k = 0; k < _tmpListCards.Count; k++){
							if(_tmpListCards[k] % 13 == i){
								_cardHighlight.Add(_tmpListCards[k]);
								_tmpListCards.RemoveAt(k);
								k--;
								continue;
							}
						}
						for(int k = 0; k < _tmpListCards.Count; k++){
							if(_tmpListCards[k] % 13 == j){
								_cardHighlight.Add(_tmpListCards[k]);
								_tmpListCards.RemoveAt(k);
								k--;
								continue;
							}
						}
						if(_cardHighlight.Count != 5){
							if(_cardHighlight.Count > 5){
								while(_cardHighlight.Count > 5){
									_cardHighlight.RemoveAt(0);
								}
							}else{
								for(sbyte l = 12; l >= 0; l--){
									if(vCards[l]==1) {
										for(int k = 0; k < _tmpListCards.Count; k++){
											sbyte _tmpValue = (sbyte)(_tmpListCards[k] % 13); 
											if(_tmpValue == l){
												_cardHighlight.Add(_tmpListCards[k]);
												_tmpListCards.RemoveAt(k);
												break;
											}
										}
										if(_cardHighlight.Count == 5){
											break;
										}
									}
								}
							}
						}
						if(_cardHighlight.Count != 5){
							#if TEST
							Debug.LogError(">>> Sai Logic 2 đôi: (length) " + _cardHighlight.Count);
							#endif
						}
						if(_onResult != null){
							_onResult(_typeCard, _cardHighlight);
						}
						return;
					}
				}
				_typeCard = TypeCardResult.TYPECARD_PAIR;
				_cardHighlight.Clear();
				_tmpListCards.Clear();
				for(int p = 0; p < _listCards.Count; p++){
					_tmpListCards.Add(_listCards[p]);
				}
				for(int k = 0; k < _tmpListCards.Count; k++){
					if(_tmpListCards[k] % 13 == i){
						_cardHighlight.Add(_tmpListCards[k]);
						_tmpListCards.RemoveAt(k);
						k--;
						continue;
					}
				}
				if(_cardHighlight.Count != 5){
					if(_cardHighlight.Count > 5){
						while(_cardHighlight.Count > 5){
							_cardHighlight.RemoveAt(0);
						}
					}else{
						for(sbyte l = 12; l >= 0; l--){
							if(vCards[l]==1) {
								for(int k = 0; k < _tmpListCards.Count; k++){
									sbyte _tmpValue = (sbyte)(_tmpListCards[k] % 13); 
									if(_tmpValue == l){
										_cardHighlight.Add(_tmpListCards[k]);
										_tmpListCards.RemoveAt(k);
										break;
									}
								}
								if(_cardHighlight.Count == 5){
									break;
								}
							}
						}
					}
				}
				if(_cardHighlight.Count != 5){
					#if TEST
					Debug.LogError(">>> Sai Logic đôi: (length) " + _cardHighlight.Count);
					#endif
				}
				if(_onResult != null){
					_onResult(_typeCard, _cardHighlight);
				}
				return;
			}
		}
		///////9. Mậu thầu
		_typeCard = TypeCardResult.TYPECARD_HIGH_CARD;
		_cardHighlight.Clear();
		_tmpListCards.Clear();
		for(int p = 0; p < _listCards.Count; p++){
			_tmpListCards.Add(_listCards[p]);
		}

		for(sbyte l = 12; l >= 0; l--){
			if(vCards[l]==1) {
				for(int k = 0; k < _tmpListCards.Count; k++){
					sbyte _tmpValue = (sbyte)(_tmpListCards[k] % 13); 
					if(_tmpValue == l){
						_cardHighlight.Add(_tmpListCards[k]);
						_tmpListCards.RemoveAt(k);
						break;
					}
				}
				if(_cardHighlight.Count == 5){
					break;
				}
			}
		}
		if(_cardHighlight.Count != 5){
			#if TEST
			Debug.LogError(">>> Sai Logic check Mậu thầu (1): (length) " + _cardHighlight.Count);
			#endif
		}
		if(_onResult != null){
			_onResult(_typeCard, _cardHighlight);
		}
		return;
	}

	public static string GetStringTypeCardResult(TypeCardResult _typeCard){
		string _str = string.Empty;
		switch (_typeCard){
		case TypeCardResult.TYPECARD_HIGH_CARD: 
			_str = "HIGH CARD";
			break;
		case TypeCardResult.TYPECARD_PAIR: 
			_str = "PAIR";
			break;
		case TypeCardResult.TYPECARD_TWO_PAIR: 
			_str = "TWO PAIR";
			break;
		case TypeCardResult.TYPECARD_THREE_OF_A_KIND: 
			_str = "THREE OF A KIND";
			break;
		case TypeCardResult.TYPECARD_STRAIGHT: 
			_str = "STRAIGHT";
			break;
		case TypeCardResult.TYPECARD_FLUSH: 
			_str = "FLUSH";
			break;
		case TypeCardResult.TYPECARD_FULL_HOUSE: 
			_str = "FULL HOUSE";
			break;
		case TypeCardResult.TYPECARD_FOUR_OF_A_KIND: 
			_str = "FOUR OF A KIND";
			break;
		case TypeCardResult.TYPECARD_STRAIGHT_FLUSH: 
			_str = "STRAIGHT FLUSH";
			break;
		}
		return _str;
	}
}