using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Poker_TEST : MonoBehaviour {
	public Poker_GamePlay_Manager poker_GamePlay_Manager;
	public List<CardDetail> listCardDetail;
	public List<sbyte> ownCards;
	public List<sbyte> globalCards;

	[Header("CHEAT")]
	public List<ICardInfo.CardType> cheatCard_Global;
	public List<ICardInfo.CardType> cheatCard_Player_00;
	public List<ICardInfo.CardType> cheatCard_Player_01;
	public List<ICardInfo.CardType> cheatCard_Player_02;
	public List<ICardInfo.CardType> cheatCard_Player_03;
	public List<ICardInfo.CardType> cheatCard_Player_04;
	public List<ICardInfo.CardType> cheatCard_Player_05;
	public List<ICardInfo.CardType> cheatCard_Player_06;
	public List<ICardInfo.CardType> cheatCard_Player_07;
	public List<ICardInfo.CardType> cheatCard_Player_08;

//		2   3   4   5   6   7   8   9   10  Q   J   K   A
//--------------------------------------------------------		
//		0   1   2   3   4   5   6   7   8   9   10  11  12
//		13  14  15  16  17  18  19  20  21  22  23  24  25
//		26  27  28  29  30  31  32  33  34  35  36  37  38
//		39  40  41  42  43  44  45  46  47  48  49  50  51

 	[ContextMenu("AAA")]
    public void CCCCC(){
		for(int i = 0; i < poker_GamePlay_Manager.listCardDetail.Count; i++){
			listCardDetail.Add(poker_GamePlay_Manager.listCardDetail[i]);
		}
    }
	public void CheckBai(){
		PokerGamePlayData.CheckResultCard(ownCards, globalCards, (_typeCard, _cardHightLight)=>{
			string _tmp = "";
			for(int i = 0; i < _cardHightLight.Count; i++){
				ICardInfo _info = GetCardInfo(_cardHightLight[i]);
				_tmp += _info.cardType + "|";
			}
			Debug.Log(">>> Type Card: " + _typeCard.ToString() + " : " + _tmp);
		});
	}
	public void GetPercentTypeCard(){
		long _xxx = MyConstant.currentTimeMilliseconds;
		PokerGamePlayData.GetPercentTypeCard(ownCards, globalCards, (_percent)=>{
			string _tmp = "";
			for(int i = 0; i < _percent.Length; i++){
				_tmp += ((PokerGamePlayData.TypeCardResult) i + 1).ToString() + ": " + _percent[i] + "% |";
			}
			Debug.Log(">>> Get Percent Type Card: " + _tmp);
			Debug.Log("Time elapsed: " + (MyConstant.currentTimeMilliseconds - _xxx));
		});
		
	}

	public ICardInfo GetCardInfo(int _value){
		if(listCardDetail == null || listCardDetail.Count == 0){
			return null;
		}
		for(int i = 0; i < listCardDetail.Count; i ++){
			if(listCardDetail[i].cardId == _value){
				return listCardDetail[i].cardInfo;
			}
		}
		return null;
	}
	public CardDetail GetCardDetail(ICardInfo.CardType _cardType){
		if(listCardDetail == null || listCardDetail.Count == 0){
			return null;
		}
		for(int i = 0; i < listCardDetail.Count; i ++){
			// Debug.Log(listCardDetail[i].cardInfo.cardType + " - " + _cardType);
			if(listCardDetail[i].cardInfo.cardType == _cardType){
				return listCardDetail[i];
			}
		}
		return null;
	}

	public void CheatBai(){
		MessageSending _messageSending = new MessageSending (CMD_REALTIME.C_GAMEPLAY_ADMIN_CHEAT);

		for(int i = 0; i < cheatCard_Global.Count; i++){
			CardDetail _cardDetail = GetCardDetail(cheatCard_Global[i]);
			_messageSending.writeByte ((byte)_cardDetail.cardId);
		}
		for(int i = 0; i < cheatCard_Player_00.Count; i++){
			CardDetail _cardDetail = GetCardDetail(cheatCard_Player_00[i]);
			_messageSending.writeByte ((byte)_cardDetail.cardId);
		}
		for(int i = 0; i < cheatCard_Player_01.Count; i++){
			CardDetail _cardDetail = GetCardDetail(cheatCard_Player_01[i]);
			_messageSending.writeByte ((byte)_cardDetail.cardId);
		}
		for(int i = 0; i < cheatCard_Player_02.Count; i++){
			CardDetail _cardDetail = GetCardDetail(cheatCard_Player_02[i]);
			_messageSending.writeByte ((byte)_cardDetail.cardId);
		}
		for(int i = 0; i < cheatCard_Player_03.Count; i++){
			CardDetail _cardDetail = GetCardDetail(cheatCard_Player_03[i]);
			_messageSending.writeByte ((byte)_cardDetail.cardId);
		}
		for(int i = 0; i < cheatCard_Player_04.Count; i++){
			CardDetail _cardDetail = GetCardDetail(cheatCard_Player_04[i]);
			_messageSending.writeByte ((byte)_cardDetail.cardId);
		}
		for(int i = 0; i < cheatCard_Player_05.Count; i++){
			CardDetail _cardDetail = GetCardDetail(cheatCard_Player_05[i]);
			_messageSending.writeByte ((byte)_cardDetail.cardId);
		}
		for(int i = 0; i < cheatCard_Player_06.Count; i++){
			CardDetail _cardDetail = GetCardDetail(cheatCard_Player_06[i]);
			_messageSending.writeByte ((byte)_cardDetail.cardId);
		}
		for(int i = 0; i < cheatCard_Player_07.Count; i++){
			CardDetail _cardDetail = GetCardDetail(cheatCard_Player_07[i]);
			_messageSending.writeByte ((byte)_cardDetail.cardId);
		}
		for(int i = 0; i < cheatCard_Player_08.Count; i++){
			CardDetail _cardDetail = GetCardDetail(cheatCard_Player_08[i]);
			_messageSending.writeByte ((byte)_cardDetail.cardId);
		}

		NetworkGlobal.instance.SendMessageRealTime (_messageSending);
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(Poker_TEST))]
public class Poker_TEST_Editor : Editor
{
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		Poker_TEST myScript = (Poker_TEST) target;
		
		GUILayout.Space(30);
		GUILayout.Label(">>> For Test <<<");

		if (GUILayout.Button ("Check bài")) {
			myScript.CheckBai();
		}
		if (GUILayout.Button ("Get Percent Type Card")) {
			myScript.GetPercentTypeCard();
		}
		if (GUILayout.Button ("Cheat bài")) {
			myScript.CheatBai();
		}
	}
}
#endif