using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] public class UnoGamePlayData : MyGamePlayData{

	public const sbyte RED_SKIP = 10;
	public const sbyte GREEN_SKIP = 23;
	public const sbyte BLUE_SKIP = 36;
	public const sbyte YELLOW_SKIP = 49;
	public const sbyte RED_REVERSE = 11;
	public const sbyte GREEN_REVERSE = 24;
	public const sbyte BLUE_REVERSE = 37;
	public const sbyte YELLOW_REVERSE = 50;
	public const sbyte RED_DRAW2 = 12;
	public const sbyte GREEN_DRAW2 = 25;
	public const sbyte BLUE_DRAW2 = 38;
	public const sbyte YELLOW_DRAW2 = 51;
	public const sbyte WILD_COLOR_RED = 52;
	public const sbyte WILD_COLOR_GREEN	= 53;
	public const sbyte WILD_COLOR_BLUE = 54;
	public const sbyte WILD_COLOR_YELLOW = 55;
	public const sbyte WILD_DRAW4_RED = 56;
	public const sbyte WILD_DRAW4_GREEN	= 57;
	public const sbyte WILD_DRAW4_BLUE = 58;
	public const sbyte WILD_DRAW4_YELLOW = 59;
	public const sbyte WILD_COLOR = 60;
	public const sbyte WILD_DRAW4 = 61;

    public enum GameState{
		STATUS_WAIT_FOR_PLAYER = 0,
		STATUS_PLAYING = 1,
		STATUS_FINISHGAME = 2
	}
    public GameState currentGameState;

    public enum TurnDirection{
        ClockWise,
        CounterClockWise
    }
    public TurnDirection turnDirection;

	public enum BackgroundColor{
		Red, Green, Blue, Yellow
	}
	public BackgroundColor currentColor;

	#region Inner Class
    [System.Serializable] public class Uno_PlayerPlayingData {
        public UserDataInGame userData;
        public sbyte indexChair;
        public List<sbyte> ownCards;
		public long totalBet;
        public bool isMe;
		public bool hasCalledUno;
		public int totalPoint;

        public Uno_PlayerPlayingData (){
			ownCards = new List<sbyte>();
		}

        public Uno_PlayerPlayingData (MessageReceiving _mess, List<short> _listSessionIdGlobalPlayer){
			short _sessionId = _mess.readShort();
			indexChair = _mess.readByte();
			sbyte _numberCardInHand = _mess.readByte();

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
			// Debug.LogError(userData.nameShowInGame + " _numberCardInHand : " + _numberCardInHand);
			ownCards = new List<sbyte>();
			for(int i = 0; i < _numberCardInHand; i++){
				ownCards.Add(-1);
			}
		}
    }

	public class Uno_StartGame_Data {
		public List<Uno_PlayerPlayingData> listPlayerPlaying;
		public List<short> listSessionIdPlaying;
		public TurnDirection turnDirection;
		public long currentBet;
		public long totalBet;
		public System.DateTime nextTimeToChangeCircle;
		public System.DateTime nextTimeToStopGame;

		public Uno_StartGame_Data(MessageReceiving _mess, UnoGamePlayData _unoGamePlayData){
			turnDirection = TurnDirection.ClockWise;
			sbyte _numCard = 7;
			sbyte[] _myCards = new sbyte[_numCard];
			for(int i = 0; i < _myCards.Length; i ++){
				_myCards[i] = _mess.readByte();
			}
			currentBet = _mess.readLong();
			sbyte _circleLength = _mess.readByte();

			totalBet = 0;
			listPlayerPlaying = new List<Uno_PlayerPlayingData>();
			listSessionIdPlaying = new List<short>();
			Uno_PlayerPlayingData _tmpPlayerPlaying = null;
			for(int i = 0; i < _circleLength; i++){
				_tmpPlayerPlaying = new Uno_PlayerPlayingData();
				short _sessionId = _mess.readShort();
				_tmpPlayerPlaying.indexChair = _mess.readByte();
				_tmpPlayerPlaying.userData = new UserDataInGame(_mess, _sessionId, -1);

				if(_unoGamePlayData.CheckIfIsMe(_sessionId)){
					_tmpPlayerPlaying.isMe = true;
					for(int j = 0; j < _numCard; j ++){
						_tmpPlayerPlaying.ownCards.Add(_myCards[j]);
					}
				}else{
					_tmpPlayerPlaying.isMe = false;
					for(int j = 0; j < _numCard; j ++){
						_tmpPlayerPlaying.ownCards.Add(-1);
					}
				}

				_tmpPlayerPlaying.totalBet = currentBet;
				totalBet += currentBet;

				listPlayerPlaying.Add(_tmpPlayerPlaying);
				listSessionIdPlaying.Add(_sessionId);
			}

			long _timeLeftToChangeCircle = _unoGamePlayData.timeDurringChangeCircle;
			nextTimeToChangeCircle = System.DateTime.Now.AddMilliseconds(_timeLeftToChangeCircle);

			long _timeLeftToStopGame = _unoGamePlayData.timeDurringPlayGame;
			nextTimeToStopGame = System.DateTime.Now.AddMilliseconds(_timeLeftToStopGame);
			// Debug.Log(">>> " + timeLeftToChangeCircle + " -- " + nextTimeToChangeCircle);
		}
	}
	public List<Uno_StartGame_Data> processStartGameData;

	public class Uno_GameReady_Data {
		public System.DateTime nextTimeToStartGame;
		public Uno_GameReady_Data(MessageReceiving _mess){
			int _timeCountDown = _mess.readInt();
			nextTimeToStartGame = System.DateTime.Now.AddMilliseconds(_timeCountDown);
		}
	}
	public List<Uno_GameReady_Data> processGameReadyData;

	public class Uno_Player_ChangeTurn_Data {
		
		public sbyte nextCircleIndex; // turn set đến người này
		public System.DateTime nextTimeToChangeCircle;
		public Uno_Player_ChangeTurn_Data(MessageReceiving _mess, UnoGamePlayData _unoGamePlayData){
			nextCircleIndex = _mess.readByte();
			int _timeLeftToChangeCircle = _unoGamePlayData.timeDurringChangeCircle;
			nextTimeToChangeCircle = System.DateTime.Now.AddMilliseconds(_timeLeftToChangeCircle);
		}
	}
	public List<Uno_Player_ChangeTurn_Data> processPlayerChangeTurnData;

	public class Uno_Player_PutCard_Data {
		public sbyte indexCircle;
		public sbyte cardValue;
		public sbyte clientIndexCard;
		public sbyte sumCardGet; // tổng số lá bài cộng dồn
		public TurnDirection turnDirection;
		public BackgroundColor bgColor;
		public sbyte countCard;

		public Uno_Player_PutCard_Data(MessageReceiving _mess, UnoGamePlayData _unoGamePlayData){
			indexCircle = _mess.readByte();
			cardValue = _mess.readByte();
			clientIndexCard = _mess.readByte();
			sumCardGet = _mess.readByte();
			bool _tmpDirection = _mess.readBoolean();
			if(_tmpDirection){
				turnDirection = TurnDirection.ClockWise;
			}else{
				turnDirection = TurnDirection.CounterClockWise;
			}

			bgColor = _unoGamePlayData.GetBackgroundColor(cardValue);

			countCard = _mess.readByte();
			// #if TEST
			// Debug.Log(">>> (PutCard) PlayerPlaying " + indexCircle +" còn " + countCard + " lá bài");
			// #endif
		}
	}
	public List<Uno_Player_PutCard_Data> processPlayerPutCardData;

	public class Uno_Me_GetCard_Data {
		public List<sbyte> myCardsValue;
		public sbyte countCard;
		public sbyte numberCardGet;

		public Uno_Me_GetCard_Data(MessageReceiving _mess){
			myCardsValue = new List<sbyte>();
			numberCardGet = _mess.readByte();
			for(int i = 0; i < numberCardGet; i ++){
				sbyte _cardValue = _mess.readByte();
				myCardsValue.Add(_cardValue);
			}
			countCard = _mess.readByte();
			// #if TEST
			// Debug.Log(">>> (MeGetCard) Mình còn " + _countCard + " lá bài, (" + _numberCardGet + ")");
			// #endif
		}
	}
	public List<Uno_Me_GetCard_Data> processMeGetCard;

	public class Uno_OtherPlayer_GetCard_Data {
		public sbyte indexCircle;
		public List<sbyte> cardsValue;
		public sbyte countCard;
		public sbyte numberCardGet;

		public Uno_OtherPlayer_GetCard_Data(MessageReceiving _mess){
			indexCircle = _mess.readByte();
			numberCardGet = _mess.readByte();
			cardsValue = new List<sbyte>();
			for(int i = 0; i < numberCardGet; i ++){
				cardsValue.Add(-1);
			}
			countCard = _mess.readByte();
			// #if TEST
			// Debug.Log(">>> (OtherPlayerGetCard) PlayerPlaying " + indexCircle +" còn " + _countCard + " lá bài, (" + _sumCardGet + ")");
			// #endif
		}
	}
	public List<Uno_OtherPlayer_GetCard_Data> processOtherPlayerGetCard;

	public class Uno_Player_CallUno_Data {
		public sbyte indexCircle; // người chơi này gọi uno
		public Uno_Player_CallUno_Data(MessageReceiving _mess){
			indexCircle = _mess.readByte();
		}
	}
	public List<Uno_Player_CallUno_Data> processPlayerCallUnoData;

	public class Uno_Player_AtkUno_Data {
		public sbyte indexAttack; // vị trí tấn công
   		public sbyte indexBeAttacked; // vị trí bị tấn công --> phải rút 2 lá
		public Uno_Player_AtkUno_Data(MessageReceiving _mess){
			indexAttack = _mess.readByte();
			indexBeAttacked = _mess.readByte();
		}
	}
	public List<Uno_Player_AtkUno_Data> processPlayerAtkUnoData;

	public class Uno_OtherPlayer_AtkUno_Me_Data {
		public sbyte indexPlayingAttackMe;
		public List<sbyte> cardsDraw;
		public Uno_OtherPlayer_AtkUno_Me_Data(MessageReceiving _mess){
			indexPlayingAttackMe = _mess.readByte();
			cardsDraw = new List<sbyte>();
			for(int i = 0; i < 2; i++){
				sbyte _card = _mess.readByte();
				cardsDraw.Add(_card);
			}
		}
	}
	public List<Uno_OtherPlayer_AtkUno_Me_Data> processOtherPlayerAtkUnoMeData;

	[System.Serializable] public class Uno_FinishGame_Data {
		public long id;

		public enum Reason{
			TimeOut = 1,
			NoGlobalCards = 2,
			PlayerWin = 3,
			OnePlayerInTable = 4
		}
		public Reason reasonFinish;

		[System.Serializable] public class Player_Data{
			public sbyte indexCircle;
			public long goldLast;
			public int achievementWinUpdate;
			public int achievementLoseUpdate;
			public List<sbyte> ownCards;
			public int totalPoint;
			public bool isWin;

			public Player_Data(MessageReceiving _mess, bool _isWin){
				isWin = _isWin;
				indexCircle = _mess.readByte();
				goldLast = _mess.readLong();
				if(_isWin){
					achievementWinUpdate = _mess.readInt();
					achievementLoseUpdate = -1;
				}else{
					achievementWinUpdate = -1;
					achievementLoseUpdate = _mess.readInt();
				}
				ownCards = new List<sbyte>();
				sbyte _numberCard = _mess.readByte();
				if(_numberCard < 0){
					#if TEST
					Debug.LogError(">>> Bug Server (Player_Data " + _isWin + "): " + _numberCard);
					#endif
				}
				totalPoint = 0;
				for(sbyte i = 0; i < _numberCard; i ++){
					sbyte _cardValue = _mess.readByte();
					ownCards.Add(_cardValue);
					if(_cardValue >= 52){
						totalPoint += 50;
					}else{
						int _tmpPoint = _cardValue % 13;
						if(_tmpPoint >= 10){
							_tmpPoint = 10;
						}
						totalPoint += _tmpPoint;
					}
				}
				// Debug.Log(">>>> " + indexCircle + " -- " + totalPoint);
			}
		}

		public long goldWin;
		public List<Player_Data> listPlayersData;

		public Uno_FinishGame_Data(MessageReceiving _mess, UnoGamePlayData _unoGamePlayData){
			id = MyConstant.currentTimeMilliseconds;
			reasonFinish = (Reason) _mess.readByte();
			goldWin = _mess.readLong();
			sbyte _numberWin = _mess.readByte();
			if(_numberWin <= 0){
				#if TEST
				Debug.LogError(">>> Bug Server FinishGame (0): " + _numberWin);
				#endif
			}
			listPlayersData = new List<Player_Data>();
			for(sbyte i = 0; i < _numberWin; i ++){
				Player_Data _player = new Player_Data(_mess, true);
				listPlayersData.Add(_player);
			}

			sbyte _numberLose = _mess.readByte();
			if(_numberLose <= 0){
				#if TEST
				Debug.LogError(">>> Bug Server FinishGame (1): " + _numberLose);
				#endif
			}
			List<Player_Data> _listLoser = new List<Player_Data>();
			for(sbyte i = 0; i < _numberLose; i ++){
				Player_Data _player = new Player_Data(_mess, false);
				_listLoser.Add(_player);
			}
			_listLoser.Sort(delegate (Player_Data _x, Player_Data _y)
			{
				if(_x.totalPoint > _y.totalPoint){
					return 1;
				}else if(_x.totalPoint == _y.totalPoint){
					return 0;
				}else{
					return -1;
				}
			});
			for(int i = 0; i < _listLoser.Count; i ++){
				listPlayersData.Add(_listLoser[i]);
			}
			if(listPlayersData.Count > _unoGamePlayData.listPlayerPlayingData.Count){
				#if TEST
				Debug.LogError(">>> Bug Server FinishGame (2): " + listPlayersData.Count + " - " + _unoGamePlayData.listPlayerPlayingData.Count);
				#endif
			}
		}
	}
	public List<Uno_FinishGame_Data> processFinishGameData;
	#endregion

    public List<UserDataInGame> listGlobalPlayerData;
	public List<Uno_PlayerPlayingData> listPlayerPlayingData; // list danh sách user đang chơi
    public List<short> listSessionIdGlobalPlayer;
	public List<short> listSessionIdOnChair;
	public List<short> listSessionIdPlaying;

    public long totalBet;
    public long betDefault;

	/// <summary>
	/// currentBet: giá trị bet của mỗi người khi start bàn, có thể nhỏ hơn betDefault vì người chơi có thể chơi minigame hết tiền trong khi đang ngồi đợi.
	/// </summary>
	public long currentBet; 

    public sbyte numberChairs;
    public System.DateTime nextTimeToStartGame;
    public System.DateTime nextTimeToChangeCircle;
	public int timeDurringChangeCircle;  // tính theo milisecond
	public sbyte currentCircle;
	public int indexCircleBeSkipped; // index bị skip bài
    
	public List<sbyte> globalCards;
	public System.DateTime nextTimeToStopGame;
	public int timeDurringPlayGame;
    public sbyte lastCardPut; // lá vừa đánh ra trên bàn
	public sbyte sumCardGet; // (tổng số lá bài cộng dồn)

    public bool hasLoadTableInfo;

	public UnoGamePlayData(){
        listGlobalPlayerData = new List<UserDataInGame>();
		listPlayerPlayingData = new List<Uno_PlayerPlayingData>();
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
		
		processGameReadyData = new List<Uno_GameReady_Data>();
		processStartGameData = new List<Uno_StartGame_Data>();
		processPlayerPutCardData = new List<Uno_Player_PutCard_Data>();
		processPlayerChangeTurnData = new List<Uno_Player_ChangeTurn_Data>();
		processMeGetCard = new List<Uno_Me_GetCard_Data>(); 
		processOtherPlayerGetCard = new List<Uno_OtherPlayer_GetCard_Data>(); 
		processPlayerCallUnoData = new List<Uno_Player_CallUno_Data>(); 
		processPlayerAtkUnoData = new List<Uno_Player_AtkUno_Data>();
		processOtherPlayerAtkUnoMeData = new List<Uno_OtherPlayer_AtkUno_Me_Data>();
		processFinishGameData = new List<Uno_FinishGame_Data>();

		nextTimeToStartGame = System.DateTime.Now;
		nextTimeToStopGame = System.DateTime.Now;
		nextTimeToChangeCircle = System.DateTime.Now;

		currentColor = BackgroundColor.Yellow;

		indexCircleBeSkipped = -1;

		hasLoadTableInfo = false;
    }

	public void ResetAtNewGame(){
		currentCircle = 0;
		indexCircleBeSkipped = -1;
		lastCardPut = -1;
		sumCardGet = 0;
		totalBet = 0;
		if(globalCards == null){
			globalCards = new List<sbyte>();
		}else{
			globalCards.Clear();
		}
		if(listPlayerPlayingData == null){
			listPlayerPlayingData = new List<Uno_PlayerPlayingData>();
		}else{
			listPlayerPlayingData.Clear();
		}
		if(listSessionIdPlaying == null){
			listSessionIdPlaying = new List<short>();
		}else{
			listSessionIdPlaying.Clear();
		}
	}

    public void InitDataWhenGetTableInfo(MessageReceiving _mess){
        InitListOtherUserDataInGame(_mess);
		InitListOnChair(_mess);

        totalBet = 0;
		sumCardGet = 0;
        betDefault = _mess.readLong();
		currentBet = _mess.readLong();
        timeDurringChangeCircle = _mess.readInt();
		timeDurringPlayGame = _mess.readInt();
        currentGameState = (GameState) _mess.readByte();

		indexCircleBeSkipped = -1;
		
        if(currentGameState == GameState.STATUS_WAIT_FOR_PLAYER){
            long _timeLeftToStartGame = _mess.readLong(); // thời gian đếm ngược để start game (nếu dương)
            if(_timeLeftToStartGame > 0){
                nextTimeToStartGame = System.DateTime.Now.AddMilliseconds(_timeLeftToStartGame);
            }else{
                nextTimeToStartGame = System.DateTime.Now;
            }
			lastCardPut = -1;
			currentColor = GetBackgroundColor(lastCardPut);
			nextTimeToStopGame = System.DateTime.MinValue;
        }else{
			nextTimeToStartGame = System.DateTime.Now;

            long _timeLeftToStopGame = _mess.readLong();
			nextTimeToStopGame = System.DateTime.Now.AddMilliseconds(_timeLeftToStopGame);
			
            lastCardPut = _mess.readByte();
			globalCards.Add(lastCardPut);

            bool _direction = _mess.readBoolean();
            turnDirection = _direction ? TurnDirection.ClockWise : TurnDirection.CounterClockWise;

            currentCircle = _mess.readByte();
			sbyte _circleLength = _mess.readByte();
			for(int i = 0; i < _circleLength; i ++){
				Uno_PlayerPlayingData _tmpData = new Uno_PlayerPlayingData(_mess, listSessionIdGlobalPlayer);
				_tmpData.totalBet = currentBet;
				listPlayerPlayingData.Add(_tmpData);
				listSessionIdPlaying.Add(_tmpData.userData.sessionId);
				totalBet += currentBet;
			}
            long _timeLeftToChangeCircle = _mess.readLong();
			nextTimeToChangeCircle = System.DateTime.Now.AddMilliseconds(_timeLeftToChangeCircle);

			currentColor = GetBackgroundColor(lastCardPut);
        }
        hasLoadTableInfo = true;
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

	public int GetTotalPlayerInGame(){
		int _total = 0;
		if(listSessionIdGlobalPlayer == null || listSessionIdGlobalPlayer.Count == 0){
			return _total;
		}
		for(int i = 0; i < listSessionIdGlobalPlayer.Count; i++){
			if(listSessionIdGlobalPlayer[i] >= 0){
				_total ++;
			}
		}
		return _total;
	}

	public int GetTotalRealPlayerOnChair(){
		int _total = 0;
		if(listSessionIdOnChair == null || listSessionIdOnChair.Count == 0){
			return _total;
		}
		for(int i = 0; i < listSessionIdOnChair.Count; i++){
			if(listSessionIdOnChair[i] >= 0){
				_total ++;
			}
		}
		return _total;
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
					AchievementDetail _achievementDetail = DataManager.instance.achievementData.GetAchievementDetail(IMiniGameInfo.Type.Uno);
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

    public void InitListOnChair(MessageReceiving _mess){
		numberChairs = _mess.readByte();
		if(numberChairs != 4){
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

	public void SetUpUserJoinGame(MessageReceiving _mess){
		PlayerJoinGame_Data _data = new PlayerJoinGame_Data(_mess);
		processPlayerJoinGame.Add(_data);
    }

    public void SetUpUserLeftGame(MessageReceiving _mess){
		PlayerLeftGame_Data _data = new PlayerLeftGame_Data(_mess);
		processPlayerLeftGame.Add(_data);
    }

	public void SetUpPlayerSitDown(MessageReceiving _mess){
		PlayerSitDown_Data _data = new PlayerSitDown_Data(_mess);
		processPlayerSitDown.Add(_data);
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

	public void SetUpPlayerStandUp(MessageReceiving _mess){
		PlayerStandUp_Data _data = new PlayerStandUp_Data(_mess);
		processPlayerStandUp.Add(_data);
		
		// short _sessionId = _mess.readShort();
		// sbyte _tmpIndexChair = _mess.readByte();
		// UserDataInGame _userData = null;
		// sbyte _indexChair = -1;
		// bool _playerIsPlaying = false;
		// if(_sessionId < 0){
		// 	#if TEST
        //     Debug.LogError(">>> sessionId nhảm : " + _sessionId);
        //     #endif
		// 	if(_onResult != null){
		// 		_onResult(_userData, _indexChair, _playerIsPlaying);
		// 	}
		// 	return;
		// }
		
		// if(listSessionIdOnChair == null || listSessionIdOnChair.Count == 0){
		// 	#if TEST
        //     Debug.LogError(">>> listSessionIdOnChair is NULL");
        //     #endif
		// 	if(_onResult != null){
		// 		_onResult(_userData, _indexChair, _playerIsPlaying);
		// 	}
		// 	return;
		// }
		
		// int _tmpIndex = listSessionIdPlaying.IndexOf(_sessionId);
		// if(_tmpIndex >= 0 && listSessionIdOnChair[listPlayerPlayingData[_tmpIndex].indexChair] >= 0){ 
		// 	_userData = listPlayerPlayingData[_tmpIndex].userData;
		// 	_indexChair = listPlayerPlayingData[_tmpIndex].indexChair;
		// 	listSessionIdOnChair[_indexChair] = -1;
		// 	_playerIsPlaying = true;
		// 	if(_indexChair != _tmpIndexChair){
		// 		#if TEST
		// 		Debug.LogError(">>> Dữ liệu không đồng bộ giữa client và server: " + _userData.nameShowInGame + " | " + _indexChair + " | " + _tmpIndexChair);
		// 		#endif
		// 	}
		// 	#if TEST
        //     Debug.Log(">>> " + _userData.nameShowInGame + " đang chơi và đứng dậy tại ghế " + _indexChair);
        //     #endif
		// 	if(_onResult != null){
		// 		_onResult(_userData, _indexChair, _playerIsPlaying);
		// 	}
		// 	return;
		// }

		// _tmpIndex = listSessionIdOnChair.IndexOf(_sessionId);
		// if(_tmpIndex < 0){
		// 	#if TEST
        //     Debug.LogError(">>> Không tìm thấy (1)");
        //     #endif
		// 	if(_onResult != null){
		// 		_onResult(_userData, _indexChair, _playerIsPlaying);
		// 	}
		// 	return;
		// }
		// if(_tmpIndex >= listSessionIdOnChair.Count){
		// 	#if TEST
        //     Debug.LogError(">>> Không tìm thấy (2)");
        //     #endif
		// 	if(_onResult != null){
		// 		_onResult(_userData, _indexChair, _playerIsPlaying);
		// 	}
		// 	return;
		// }
		// if(listSessionIdOnChair[_tmpIndex] < 0){
		// 	#if TEST
        //     Debug.LogError(">>> Không tìm thấy (3)");
        //     #endif
		// 	if(_onResult != null){
		// 		_onResult(_userData, _indexChair, _playerIsPlaying);
		// 	}
		// 	return;
		// }

		// _userData = GetUserDataInGameFromListGlobal(_sessionId);
		// if(_userData == null){
		// 	if(_onResult != null){
		// 		_onResult(_userData, _indexChair, _playerIsPlaying);
		// 	}
		// 	return;
		// }
		// _indexChair = (sbyte) _tmpIndex;
		// listSessionIdOnChair[_indexChair] = -1;
		// if(_indexChair != _tmpIndexChair){
		// 	#if TEST
		// 	Debug.LogError(">>> Dữ liệu không đồng bộ giữa client và server: " + _userData.nameShowInGame + " | " + _indexChair + " | " + _tmpIndexChair);
		// 	#endif
		// }
		
		// if(_onResult != null){
		// 	_onResult(_userData, _indexChair, _playerIsPlaying);
		// }
	}

	public void SetDataWhenReady(MessageReceiving _mess){
		Uno_GameReady_Data _data = new Uno_GameReady_Data(_mess);
		processGameReadyData.Add(_data);
	}

	public void SetDataWhenStartGame(MessageReceiving _mess){
		Uno_StartGame_Data _data = new Uno_StartGame_Data(_mess, this); 
		processStartGameData.Add(_data);
	}

	public void SetDataWhenPlayerPutCard(MessageReceiving _mess){
		Uno_Player_PutCard_Data _data = new Uno_Player_PutCard_Data(_mess, this);
		processPlayerPutCardData.Add(_data);
	}

	public void SetDataWhenPlayerChangeTurn(MessageReceiving _mess){
		Uno_Player_ChangeTurn_Data _data = new Uno_Player_ChangeTurn_Data(_mess, this);
		processPlayerChangeTurnData.Add(_data);
	}

	public void SetDataWhenMeGetCard(MessageReceiving _mess){
		Uno_Me_GetCard_Data _data = new Uno_Me_GetCard_Data(_mess);
		processMeGetCard.Add(_data);
	}

	public void SetDataWhenOtherPlayerGetCard(MessageReceiving _mess){
		Uno_OtherPlayer_GetCard_Data _data = new Uno_OtherPlayer_GetCard_Data(_mess);
		processOtherPlayerGetCard.Add(_data);
	}

	public void SetDataWhenPlayerCallUno(MessageReceiving _mess){
		Uno_Player_CallUno_Data _data = new Uno_Player_CallUno_Data(_mess);
		processPlayerCallUnoData.Add(_data);
	}

	public void SetDataWhenPlayerAtkUno(MessageReceiving _mess){
		Uno_Player_AtkUno_Data _data = new Uno_Player_AtkUno_Data(_mess);
		processPlayerAtkUnoData.Add(_data);
	}

	public void SetDataWhenOtherPlayerAtkUnoMe(MessageReceiving _mess){
		Uno_OtherPlayer_AtkUno_Me_Data _data = new Uno_OtherPlayer_AtkUno_Me_Data(_mess);
		processOtherPlayerAtkUnoMeData.Add(_data);
	}

	public void SetDataWhenFinishGame(MessageReceiving _mess){
		Uno_FinishGame_Data _data = new Uno_FinishGame_Data(_mess, this);
		processFinishGameData.Add(_data);
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
	#region Check Logic
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

	public bool CheckIsMyTurn(){
		if(currentGameState != GameState.STATUS_PLAYING){
			return false;
		}
		if(!CheckIsPlaying(DataManager.instance.userData.sessionId)){
			return false;
		}
		if(listPlayerPlayingData == null || listPlayerPlayingData.Count == 0
			|| currentCircle < 0 || currentCircle >= listPlayerPlayingData.Count){
			return false;
		}
		if(!listPlayerPlayingData[currentCircle].isMe){
			return false;
		}

		return true;
	}

	public bool IsWildCardColor(sbyte _cardValue){
		if(_cardValue == WILD_COLOR_RED || _cardValue == WILD_COLOR_GREEN || _cardValue == WILD_COLOR_BLUE || _cardValue == WILD_COLOR_YELLOW){
			return true;
		}
		return false;
	}

	public bool IsWildCardDraw(sbyte _cardValue){
		if(_cardValue == WILD_DRAW4_RED || _cardValue == WILD_DRAW4_GREEN || _cardValue == WILD_DRAW4_BLUE || _cardValue == WILD_DRAW4_YELLOW){
			return true;
		}
		return false;
	}

	public bool IsCardDraw(sbyte _cardValue){
		if(_cardValue == WILD_DRAW4_RED || _cardValue == WILD_DRAW4_GREEN || _cardValue == WILD_DRAW4_BLUE || _cardValue == WILD_DRAW4_YELLOW
			|| _cardValue == RED_DRAW2 || _cardValue == GREEN_DRAW2 || _cardValue == BLUE_DRAW2 || _cardValue == YELLOW_DRAW2){
			return true;
		}
		return false;
	}

	public BackgroundColor GetBackgroundColor(sbyte _cardValue){
		if(_cardValue == -1){
			return BackgroundColor.Yellow;
		}else if(_cardValue <= 12 || _cardValue == WILD_COLOR_RED || _cardValue == WILD_DRAW4_RED){
			return BackgroundColor.Red;
		}else if(_cardValue <= 25 || _cardValue == WILD_COLOR_GREEN || _cardValue == WILD_DRAW4_GREEN){
			return BackgroundColor.Green;
		}else if(_cardValue <= 38 || _cardValue == WILD_COLOR_BLUE || _cardValue == WILD_DRAW4_BLUE){
			return BackgroundColor.Blue;
		}else{
			return BackgroundColor.Yellow;
		}
	}

	public bool IsReverseCard(sbyte _cardValue){
		if(_cardValue == RED_REVERSE || _cardValue == GREEN_REVERSE || _cardValue == BLUE_REVERSE || _cardValue == YELLOW_REVERSE){
			return true;
		}
		return false;
	}

	public bool IsSkipCard(sbyte _cardValue){
		if(_cardValue == RED_SKIP || _cardValue == GREEN_SKIP || _cardValue == BLUE_SKIP || _cardValue == YELLOW_SKIP){
			return true;
		}
		return false;
	}

	private bool CompareWild(sbyte _cardValue) {
		if(_cardValue < 13){
			return lastCardPut == WILD_COLOR_RED || lastCardPut == WILD_DRAW4_RED;
		}else if(_cardValue < 26){
			return lastCardPut == WILD_COLOR_GREEN || lastCardPut == WILD_DRAW4_GREEN;
		}else if(_cardValue < 39){
			return lastCardPut == WILD_COLOR_BLUE || lastCardPut == WILD_DRAW4_BLUE;
		}else{
			return lastCardPut == WILD_COLOR_YELLOW || lastCardPut == WILD_DRAW4_YELLOW;
		}
	}
	public bool CheckCanPutCard(sbyte _cardValue) {//cardValue=-1 --> kết thúc turn
		if(currentGameState != GameState.STATUS_PLAYING){
			#if TEST
			Debug.LogError("BUG Logic CheckCanPutCard (0)");
			#endif
			return false;
		}
		if (_cardValue < 0) {/* Sai value */
			#if TEST
			Debug.LogError("BUG Logic CheckCanPutCard (1)");
			#endif
			return false;
		}
		if(lastCardPut < 0){ // chưa đánh lá nào hết
			return true;
		}
		
		if(sumCardGet > 0) {
			if((lastCardPut == RED_DRAW2 || lastCardPut == GREEN_DRAW2 || lastCardPut == BLUE_DRAW2 || lastCardPut == YELLOW_DRAW2) 
				&& (_cardValue == RED_DRAW2 || _cardValue == GREEN_DRAW2 || _cardValue == BLUE_DRAW2 || _cardValue == YELLOW_DRAW2)){
				return true;
			}else if((lastCardPut == WILD_DRAW4_RED || lastCardPut == WILD_DRAW4_GREEN || lastCardPut == WILD_DRAW4_BLUE || lastCardPut == WILD_DRAW4_YELLOW) 
				&& _cardValue == WILD_DRAW4) {
				return true;
			}else{
				return false;
			}
		}else {
			if (_cardValue < 52){// Đánh thường
				if(CompareWild(_cardValue)){
					return true;
				}
				if((_cardValue/13)==(lastCardPut/13) || ((_cardValue%13)==(lastCardPut%13) && lastCardPut<52)){
					return true;
				}else{
					return false;
				}
			}else if(_cardValue<60){
				return false;
			}else{
				return true;
			}
		}
	}
	#endregion
}
