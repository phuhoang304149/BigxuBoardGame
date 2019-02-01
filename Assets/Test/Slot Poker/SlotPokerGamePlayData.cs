using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotPokerGamePlayData : MyGamePlayData {

//		2   3   4   5   6   7   8   9   10  J   Q   K   A
//--------------------------------------------------------		
//		0   1   2   3   4   5   6   7   8   9   10  11  12
//		13  14  15  16  17  18  19  20  21  22  23  24  25
//		26  27  28  29  30  31  32  33  34  35  36  37  38
//		39  40  41  42  43  44  45  46  47  48  49  50  51

	public class ResultCardType{
		public PokerGamePlayData.TypeCardResult typeCard; 
		public List<List<sbyte>> listTypecards;
		public ResultCardType(){
			listTypecards = new List<List<sbyte>>();
		}
	}

	public List<ResultCardType> listResultCardType;
	public int totalCase;

	List<CardDetail> listCardDetail;

	public SlotPokerGamePlayData(){
		// listResultCardType = new List<ResultCardType>();

		// for(int i = 0; i < 9; i ++){
		// 	ResultCardType _tmp = new ResultCardType();
		// 	_tmp.typeCard = (PokerGamePlayData.TypeCardResult) (i + 1);
		// 	listResultCardType.Add(_tmp);
		// }

		// totalCase = 0;
		// for(int i = 0; i < 52; i ++){
		// 	for(int j = i+1; j < 52; j ++){
		// 		for(int m = j+1; m < 52; m ++){
		// 			for(int n = m+1; n < 52; n ++){
		// 				for(int p = n+1; p < 52; p ++){
		// 					List<sbyte> _cards = new List<sbyte>();
		// 					_cards.Add((sbyte) i);
		// 					_cards.Add((sbyte) j);
		// 					_cards.Add((sbyte) m);
		// 					_cards.Add((sbyte) n);
		// 					_cards.Add((sbyte) p);
		// 					int _tmpType = (int) GetTypeCardResult(_cards);
		// 					listResultCardType[_tmpType -1].listTypecards.Add(_cards);
		// 					totalCase ++;
		// 				}
		// 			}
		// 		}
		// 	}
		// }
		// Debug.LogError("totalCase: " + totalCase);
		// for(int i = 0; i < listResultCardType.Count; i++){
		// 	Debug.LogError(listResultCardType[i].typeCard.ToString() + " - " + listResultCardType[i].listTypecards.Count);
		// }
	}
	
	public void GetListResultCardType(List<CardDetail> _listCardDetail, List<sbyte> _currentCards){
		if(listResultCardType == null){
			listResultCardType = new List<ResultCardType>();
		}else{
			listResultCardType.Clear();
		}

		for(int i = 0; i < 9; i ++){
			ResultCardType _tmp = new ResultCardType();
			_tmp.typeCard = (PokerGamePlayData.TypeCardResult) (i+1);
			listResultCardType.Add(_tmp);
		}

		if(_currentCards.Count == 5){
			totalCase = 1;
			int _tmpType = (int) GetTypeCardResult(_currentCards);
			listResultCardType[_tmpType -1].listTypecards.Add(_currentCards);
			return;
		}

		totalCase = 40*(5 - _currentCards.Count);
		int _count = 0;
		while(_count < totalCase){
			List<sbyte> _listPoolIndex = new List<sbyte>();
			for(int i = 0; i < _listCardDetail.Count; i ++){
				_listPoolIndex.Add((sbyte) i);
			}
			List<sbyte> _newCurrentCard = new List<sbyte>();
			for(int i = 0; i < _currentCards.Count; i++){
				_newCurrentCard.Add(_currentCards[i]);
			}

			while(_newCurrentCard.Count < 5){
				int _index = Random.Range(0, _listPoolIndex.Count);
				int _cardValue = _listCardDetail[_listPoolIndex[_index]].cardId;
				_newCurrentCard.Add((sbyte)_cardValue);
				_listPoolIndex.RemoveAt(_index);
			}

			int _tmpType = (int) GetTypeCardResult(_newCurrentCard);
			listResultCardType[_tmpType -1].listTypecards.Add(_newCurrentCard);
			_count ++;
		}
	}

	public IEnumerator FirstTime_GetCardsResult_HIGH_CARD(System.Action<List<sbyte>> _onFinished){
		List<sbyte> _cards = new List<sbyte>();

		List<sbyte> _tmpListPool = new List<sbyte>();
		// --- Lấy ra 3 lá còn lại random không giống nhau --- //
		long _currentTime = MyConstant.currentTimeMilliseconds;
		long _timeOut = 60;
		while(true){
			if(MyConstant.currentTimeMilliseconds - _currentTime >= _timeOut){
				yield return Yielders.FixedUpdate;
				_currentTime = MyConstant.currentTimeMilliseconds;
			}

			_cards.Clear();

			List<int> _listTypeCard = new List<int>();
			for(int i = 0; i < 13; i ++){
				_listTypeCard.Add(i);
			}
			
			for(int i = 0; i < 5; i++){
				int _type = _listTypeCard[Random.Range(0, _listTypeCard.Count)];
				_listTypeCard.Remove(_type);
				_tmpListPool.Clear();
				for(int j = 0; j < 4; j ++){
					sbyte _cardValue = (sbyte) (_type + (j * 13));
					_tmpListPool.Add(_cardValue);
				}

				for(int j = 0; j < 1; j ++){
					sbyte _cardValue = _tmpListPool[Random.Range(0, _tmpListPool.Count)];
					_cards.Add(_cardValue);
					_tmpListPool.Remove(_cardValue);
				}
				_tmpListPool.Clear();
			}

			if(GetTypeCardResult(_cards) == PokerGamePlayData.TypeCardResult.TYPECARD_HIGH_CARD){
				break;
			}
		}

		if(_onFinished != null){
			_onFinished(_cards);
		}
	}

	public IEnumerator FirstTime_GetCardsResult_PAIR(System.Action<List<sbyte>> _onFinished){
		List<sbyte> _cards = new List<sbyte>();

		List<int> _listTypeCard = new List<int>();
		for(int i = 0; i < 13; i ++){
			_listTypeCard.Add(i);
		}

		// --- Lấy ra 2 lá giống nhau trước --- //
		int _type = _listTypeCard[Random.Range(0, _listTypeCard.Count)];
		_listTypeCard.Remove(_type);
		List<sbyte> _tmpListPool = new List<sbyte>();
		for(int i = 0; i < 4; i ++){
			sbyte _cardValue = (sbyte) (_type + (i * 13));
			_tmpListPool.Add(_cardValue);
		}

		for(int i = 0; i < 2; i ++){
			sbyte _cardValue = _tmpListPool[Random.Range(0, _tmpListPool.Count)];
			_cards.Add(_cardValue);
			_tmpListPool.Remove(_cardValue);
		}
		_tmpListPool.Clear();

		// --- Lấy ra 3 lá còn lại random không giống nhau --- //
		long _currentTime = MyConstant.currentTimeMilliseconds;
		long _timeOut = 60;
		while(true){
			if(MyConstant.currentTimeMilliseconds - _currentTime >= _timeOut){
				yield return Yielders.FixedUpdate;
				_currentTime = MyConstant.currentTimeMilliseconds;
			}

			List<sbyte> _newCards = new List<sbyte>();
			for(int i = 0; i < _cards.Count; i++){
				_newCards.Add(_cards[i]);
			}

			List<int> _newListTypeCard = new List<int>();
			for(int i = 0; i < _listTypeCard.Count; i++){
				_newListTypeCard.Add(_listTypeCard[i]);
			}
			
			for(int i = 0; i < 3; i++){
				_type = _listTypeCard[Random.Range(0, _newListTypeCard.Count)];
				_newListTypeCard.Remove(_type);
				_tmpListPool.Clear();
				for(int j = 0; j < 4; j ++){
					sbyte _cardValue = (sbyte) (_type + (j * 13));
					_tmpListPool.Add(_cardValue);
				}

				for(int j = 0; j < 1; j ++){
					sbyte _cardValue = _tmpListPool[Random.Range(0, _tmpListPool.Count)];
					_newCards.Add(_cardValue);
					_tmpListPool.Remove(_cardValue);
				}
				_tmpListPool.Clear();
			}

			if(GetTypeCardResult(_newCards) == PokerGamePlayData.TypeCardResult.TYPECARD_PAIR){
				_cards = _newCards;
				break;
			}
		}

		if(_onFinished != null){
			_onFinished(_cards);
		}
	}

	public IEnumerator FirstTime_GetCardsResult_TWO_PAIR(System.Action<List<sbyte>> _onFinished){
		List<sbyte> _cards = new List<sbyte>();

		List<int> _listTypeCard = new List<int>();
		for(int i = 0; i < 13; i ++){
			_listTypeCard.Add(i);
		}

		// --- Lấy ra 2 lá giống nhau trước --- //
		int _type = _listTypeCard[Random.Range(0, _listTypeCard.Count)];
		_listTypeCard.Remove(_type);
		List<sbyte> _tmpListPool = new List<sbyte>();
		for(int i = 0; i < 4; i ++){
			sbyte _tmp = (sbyte) (_type + (i * 13));
			_tmpListPool.Add((sbyte) _tmp);
		}

		for(int i = 0; i < 2; i ++){
			sbyte _cardValue = (sbyte) _tmpListPool[Random.Range(0, _tmpListPool.Count)];
			_cards.Add(_cardValue);
			_tmpListPool.Remove(_cardValue);
		}
		_tmpListPool.Clear();

		// -- Lấy tiếp 2 lá giống nhau -- //
		_type = _listTypeCard[Random.Range(0, _listTypeCard.Count)];
		_listTypeCard.Remove(_type);
		_tmpListPool = new List<sbyte>();
		for(int i = 0; i < 4; i ++){
			sbyte _tmp = (sbyte) (_type + (i * 13));
			_tmpListPool.Add((sbyte) _tmp);
		}
		for(int i = 0; i < 2; i ++){
			sbyte _cardValue = (sbyte) _tmpListPool[Random.Range(0, _tmpListPool.Count)];
			_cards.Add(_cardValue);
			_tmpListPool.Remove(_cardValue);
		}
		_tmpListPool.Clear();

		// -- Lấy lá cuối random -- //
		_type = _listTypeCard[Random.Range(0, _listTypeCard.Count)];
		_listTypeCard.Remove(_type);
		_tmpListPool = new List<sbyte>();
		for(int i = 0; i < 4; i ++){
			sbyte _tmp = (sbyte) (_type + (i * 13));
			_tmpListPool.Add((sbyte) _tmp);
		}

		for(int i = 0; i < 1; i ++){
			sbyte _cardValue = (sbyte) _tmpListPool[Random.Range(0, _tmpListPool.Count)];
			_cards.Add(_cardValue);
			_tmpListPool.Remove(_cardValue);
		}
		_tmpListPool.Clear();
		
		
		if(_onFinished != null){
			_onFinished(_cards);
		}
		yield break;
	}

	public IEnumerator FirstTime_GetCardsResult_THREE_OF_A_KIND(System.Action<List<sbyte>> _onFinished){
		List<sbyte> _cards = new List<sbyte>();

		List<sbyte> _listGlobalPoolCards = new List<sbyte>();
		for(int i = 0; i < 52; i ++){
			_listGlobalPoolCards.Add((sbyte) i);
		}

		List<int> _listTypeCard = new List<int>();
		for(int i = 0; i < 13; i ++){
			_listTypeCard.Add(i);
		}

		// --- Lấy ra 3 lá giống nhau trước --- //
		int _type = _listTypeCard[Random.Range(0, _listTypeCard.Count)];
		_listTypeCard.Remove(_type);
		List<sbyte> _tmpListPool = new List<sbyte>();
		for(int i = 0; i < 4; i ++){
			sbyte _cardValue = (sbyte) (_type + (i * 13));
			_tmpListPool.Add(_cardValue);
			_listGlobalPoolCards.Remove(_cardValue);
		}

		for(int i = 0; i < 3; i ++){
			sbyte _cardValue = (sbyte) _tmpListPool[Random.Range(0, _tmpListPool.Count)];
			_cards.Add(_cardValue);
			_tmpListPool.Remove(_cardValue);
		}
		_tmpListPool.Clear();

		// --- Lấy 2 lá còn lại random --- //
		long _currentTime = MyConstant.currentTimeMilliseconds;
		long _timeOut = 60;
		while(true){
			if(MyConstant.currentTimeMilliseconds - _currentTime >= _timeOut){
				yield return Yielders.FixedUpdate;
				_currentTime = MyConstant.currentTimeMilliseconds;
			}

			List<sbyte> _newListGlobalPoolCards = new List<sbyte>();
			for(int i = 0; i < _listGlobalPoolCards.Count; i ++){
				_newListGlobalPoolCards.Add(_listGlobalPoolCards[i]);
			}

			List<sbyte> _newCards = new List<sbyte>();
			for(int i = 0; i < _cards.Count; i++){
				_newCards.Add(_cards[i]);
			}
			
			for(int i = 0; i < 2; i++){
				sbyte _cardValue = _newListGlobalPoolCards[Random.Range(0, _newListGlobalPoolCards.Count)];
				_newCards.Add(_cardValue);
				_newListGlobalPoolCards.Remove(_cardValue);
			}

			if(GetTypeCardResult(_newCards) == PokerGamePlayData.TypeCardResult.TYPECARD_THREE_OF_A_KIND){
				_cards = _newCards;
				break;
			}
		}
		if(_onFinished != null){
			_onFinished(_cards);
		}
		yield break;
	}

	public IEnumerator FirstTime_GetCardsResult_STRAIGHT(System.Action<List<sbyte>> _onFinished){
		List<sbyte> _cards = new List<sbyte>();
		
		List<sbyte> _listGlobalPoolCards = new List<sbyte>();
		for(int i = 0; i < 52; i ++){
			_listGlobalPoolCards.Add((sbyte) i);
		}
		
		if(Random.Range(0, 10) == 0){ // trường hợp A2345
			int _kind = Random.Range(0, 4);
			sbyte _typeCardBegin = (sbyte) (12 + (13*_kind));

			long _currentTime = MyConstant.currentTimeMilliseconds;
			long _timeOut = 60;
			while(true){
				if(MyConstant.currentTimeMilliseconds - _currentTime >= _timeOut){
					yield return Yielders.FixedUpdate;
					_currentTime = MyConstant.currentTimeMilliseconds;
				}
				
				_cards.Clear();
				_cards.Add(_typeCardBegin);

				for(int i = 0; i < 4; i ++){
					_kind = Random.Range(0, 4);
					sbyte _cardValue = (sbyte) (i + 13 * _kind);
					_cards.Add(_cardValue);
				}

				if(GetTypeCardResult(_cards) == PokerGamePlayData.TypeCardResult.TYPECARD_STRAIGHT){
					break;
				}
			}
		}else{
			sbyte _typeCardBegin = (sbyte) Random.Range(0, 9);

			long _currentTime = MyConstant.currentTimeMilliseconds;
			long _timeOut = 60;
			while(true){
				if(MyConstant.currentTimeMilliseconds - _currentTime >= _timeOut){
					yield return Yielders.FixedUpdate;
					_currentTime = MyConstant.currentTimeMilliseconds;
				}

				_cards.Clear();

				for(int i = _typeCardBegin; i < (_typeCardBegin + 5); i ++){
					int _kind = Random.Range(0, 4);
					sbyte _cardValue = (sbyte) (i + (13*_kind));
					_cards.Add(_cardValue);
				}

				if(GetTypeCardResult(_cards) == PokerGamePlayData.TypeCardResult.TYPECARD_STRAIGHT){
					break;
				}
			}
		}

		if(_onFinished != null){
			_onFinished(_cards);
		}
	}

	public IEnumerator FirstTime_GetCardsResult_FLUSH(System.Action<List<sbyte>> _onFinished){
		List<sbyte> _cards = new List<sbyte>();

		List<int> _listKindCard = new List<int>();
		for(int i = 0; i < 4; i ++){
			_listKindCard.Add(i);
		}

		int _kind = _listKindCard[Random.Range(0, _listKindCard.Count)];
		_listKindCard.Remove(_kind);

		long _currentTime = MyConstant.currentTimeMilliseconds;
		long _timeOut = 60;
		while(true){
			if(MyConstant.currentTimeMilliseconds - _currentTime >= _timeOut){
				yield return Yielders.FixedUpdate;
				_currentTime = MyConstant.currentTimeMilliseconds;
			}
			_cards.Clear();

			List<int> _listTypeCard = new List<int>();
			for(int i = 0; i < 13; i ++){
				_listTypeCard.Add(i);
			}

			for(int i = 0; i < 5; i++){
				int _type = _listTypeCard[Random.Range(0, _listTypeCard.Count)];
				_listTypeCard.Remove(_type);
				sbyte _cardValue = (sbyte) (_type + (_kind * 13));
				_cards.Add(_cardValue);
			}
			if(GetTypeCardResult(_cards) == PokerGamePlayData.TypeCardResult.TYPECARD_FLUSH){
				break;
			}
		}

		if(_onFinished != null){
			_onFinished(_cards);
		}
	}

	public IEnumerator FirstTime_GetCardsResult_FULL_HOUSE(System.Action<List<sbyte>> _onFinished){
		List<sbyte> _cards = new List<sbyte>();

		List<int> _listTypeCard = new List<int>();
		for(int i = 0; i < 13; i ++){
			_listTypeCard.Add(i);
		}

		// --- Lấy ra 3 lá giống nhau trước --- //
		int _type = _listTypeCard[Random.Range(0, _listTypeCard.Count)];
		_listTypeCard.Remove(_type);
		List<sbyte> _tmpListPool = new List<sbyte>();
		for(int i = 0; i < 4; i ++){
			sbyte _tmp = (sbyte) (_type + (i * 13));
			_tmpListPool.Add((sbyte) _tmp);
		}

		for(int i = 0; i < 3; i ++){
			sbyte _cardValue = (sbyte) _tmpListPool[Random.Range(0, _tmpListPool.Count)];
			_cards.Add(_cardValue);
			_tmpListPool.Remove(_cardValue);
		}
		_tmpListPool.Clear();

		// -- Lấy tiếp 2 lá giống nhau -- //
		_type = _listTypeCard[Random.Range(0, _listTypeCard.Count)];
		_listTypeCard.Remove(_type);
		_tmpListPool = new List<sbyte>();
		for(int i = 0; i < 4; i ++){
			sbyte _tmp = (sbyte) (_type + (i * 13));
			_tmpListPool.Add((sbyte) _tmp);
		}
		for(int i = 0; i < 2; i ++){
			sbyte _cardValue = (sbyte) _tmpListPool[Random.Range(0, _tmpListPool.Count)];
			_cards.Add(_cardValue);
			_tmpListPool.Remove(_cardValue);
		}
		_tmpListPool.Clear();
		
		if(_onFinished != null){
			_onFinished(_cards);
		}
		yield break;
	}

	public IEnumerator FirstTime_GetCardsResult_FOUR_OF_A_KIND(System.Action<List<sbyte>> _onFinished){
		List<sbyte> _cards = new List<sbyte>();
		List<sbyte> _listPoolCards = new List<sbyte>();
		for(int i = 0; i < 52; i ++){
			_listPoolCards.Add((sbyte) i);
		}

		int _type = _listPoolCards[Random.Range(0, 13)];
		for(int i = 0; i < 4; i ++){
			sbyte _tmp = (sbyte) (_type + (i * 13));
			_cards.Add((sbyte) _tmp);
			_listPoolCards.Remove(_tmp);
		}

		sbyte _valueLastCard = _listPoolCards[Random.Range(0, _listPoolCards.Count)];
		_cards.Add(_valueLastCard);

		if(_onFinished != null){
			_onFinished(_cards);
		}
		yield break;
	}

	public IEnumerator FirstTime_GetCardsResult_STRAIGHT_FLUSH(System.Action<List<sbyte>> _onFinished){
		List<sbyte> _cards = new List<sbyte>();

		sbyte _valueCardBegin = 0;
		if(Random.Range(0, 10) == 0){ // trường hợp A2345
			int _kind = Random.Range(0, 4);
			_valueCardBegin = (sbyte) (12 + (13*_kind));
			_cards.Add(_valueCardBegin);
			for(int i = _valueCardBegin - 12; i < (_valueCardBegin - 12 + 4); i ++){
				sbyte _cardValue = (sbyte) i;
				_cards.Add(_cardValue);
			}
		}else{
			_valueCardBegin = (sbyte) Random.Range(0, 9);
			int _kind = Random.Range(0, 4);
			_valueCardBegin = (sbyte) (_valueCardBegin + (13*_kind));
			for(int i = _valueCardBegin; i < (_valueCardBegin + 5); i ++){
				sbyte _cardValue = (sbyte) i;
				_cards.Add(_cardValue);
			}
		}

		if(_onFinished != null){
			_onFinished(_cards);
		}
		yield break;
	}

	public PokerGamePlayData.TypeCardResult GetTypeCardResult(List<sbyte> _cards) {		
		if(_cards.Count != 5){
			#if TEST
			Debug.LogError(">>> Không đủ 5 lá bài");
			#endif
		}
		List<sbyte> _listCards = new List<sbyte>();
		for(int i = 0; i < _cards.Count; i++){
			_listCards.Add(_cards[i]);
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
			if(_listCards[0]==_listCards[1]-1 && _listCards[1]==_listCards[2]-1) {
				if(_listCards[3]==_listCards[4]-1) {//01234
					if(_listCards[0]%13<9) {
						return PokerGamePlayData.TypeCardResult.TYPECARD_STRAIGHT_FLUSH;
					}
				}

				if(_listCards[0]%13==0) {
					if(_listCards[0]==_listCards[4]-12){ //01234
						return PokerGamePlayData.TypeCardResult.TYPECARD_STRAIGHT_FLUSH;
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

		vCards[_cards[0]%13]++;
		vCards[_cards[1]%13]++;
		vCards[_cards[2]%13]++;
		vCards[_cards[3]%13]++;
		vCards[_cards[4]%13]++;

		// string _tmp = "";
		// for(int i = 0; i < vCards.Length; i++){
		// 	_tmp += vCards[i] + "|";
		// }
		// Debug.Log(">>> vCards: " + _tmp);

		///////2.Tứ quý
		List<sbyte> _tmpListCards = new List<sbyte>();

		for(sbyte i=12;i>=0;i--){
			if(vCards[i]==4) {
				return PokerGamePlayData.TypeCardResult.TYPECARD_FOUR_OF_A_KIND;
			}
		}

		///////3. Cù lũ		
		for(sbyte i=12;i>=0;i--){
			if(vCards[i]==3){
				for(sbyte j=12; j>=0;j--){
					if(vCards[j]>1 && j!=i) {
						return PokerGamePlayData.TypeCardResult.TYPECARD_FULL_HOUSE;
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
			for(sbyte i=0;i<5;i++){
				if(_listCards[i]/13==chatBai) {
					_count++;
					if(_valueMax<_listCards[i])
						_valueMax=_listCards[i];
				}
			}
			if(_count>4) {
				return PokerGamePlayData.TypeCardResult.TYPECARD_FLUSH;
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
					return PokerGamePlayData.TypeCardResult.TYPECARD_STRAIGHT;
				}
			}
		}
		if(vCards[0]>0 && vCards[1]>0 && vCards[2]>0 && vCards[3]>0 && vCards[12]>0) {
			return PokerGamePlayData.TypeCardResult.TYPECARD_STRAIGHT;
		}
		
		///////6. Sám cô
		for(sbyte i=12;i>=0;i--){
			if(vCards[i]==3) {
				return PokerGamePlayData.TypeCardResult.TYPECARD_THREE_OF_A_KIND;
			}
		}
		
		///////7. Đôi + Thú
		for(sbyte i=12;i>=0;i--){
			if(vCards[i]>1) {
				for(sbyte j=(sbyte) (i-1);j>=0;j--){
					if(vCards[j]>1) {
						return PokerGamePlayData.TypeCardResult.TYPECARD_TWO_PAIR;
					}
				}
				return PokerGamePlayData.TypeCardResult.TYPECARD_PAIR;
			}
		}
		///////9. Mậu thầu
		return PokerGamePlayData.TypeCardResult.TYPECARD_HIGH_CARD;
	}

	// public void GetPercentTypeCard(sbyte[] _cards, System.Action<float[]> _result){
	// 	float[] _tmpPercentProcess = new float[9];
	// 	_tmpPercentProcess[0] = 0f;
	// 	_tmpPercentProcess[1] = 0f;
	// 	_tmpPercentProcess[2] = 0f;
	// 	_tmpPercentProcess[3] = 0f;
	// 	_tmpPercentProcess[4] = 0f;
	// 	_tmpPercentProcess[5] = 0f;
	// 	_tmpPercentProcess[6] = 0f;
	// 	_tmpPercentProcess[7] = 0f;
	// 	_tmpPercentProcess[8] = 0f;

	// 	short[] _listCount = new short[9];
	// 	_listCount[0] = 0;
	// 	_listCount[1] = 0;
	// 	_listCount[2] = 0;
	// 	_listCount[3] = 0;
	// 	_listCount[4] = 0;
	// 	_listCount[5] = 0;
	// 	_listCount[6] = 0;
	// 	_listCount[7] = 0;
	// 	_listCount[8] = 0;

	// 	short _total = 0; 

	// 	if(_cards.Length == 0){

	// 	}
		

	// 	for(int i = 0; i < 52; i++){
	// 		if(!_cards.Contains((sbyte) i)){
	// 			for(int j = i + 1; j < 52; j++){
	// 				if(!_cards.Contains((sbyte) j)){
	// 					List<sbyte> _newGlobalCards = new List<sbyte>();
	// 					for(int m = 0; m < _cards.Count; m ++){
	// 						_newGlobalCards.Add(_cards[m]);
	// 					}
	// 					_newGlobalCards.Add((sbyte) i);
	// 					_newGlobalCards.Add((sbyte) j);

	// 					_listCount[((short) GetTypeCardResult(_ownCards, _newGlobalCards)) - 1] ++;
	// 					_total ++;
	// 				}
	// 			}
	// 		}
	// 	}

	// 	for(int i = 0 ; i < 9; i ++){
	// 		_tmpPercentProcess[i] = (float) (_listCount[i]) / (float) _total * 100f;
	// 	}
	// 	if(_result != null){
	// 		_result(_tmpPercentProcess);
	// 	}
	// }

}
