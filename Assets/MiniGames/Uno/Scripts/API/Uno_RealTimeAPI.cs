using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Uno_RealTimeAPI {

	public static Uno_RealTimeAPI instance {
        get {
            if (ins == null) {
				ins = new Uno_RealTimeAPI();
            }
            return ins;
        }
    }
	static Uno_RealTimeAPI ins;

	MessageSending messageSendingSetBet;
	MessageSending messageSendingSitDown;
	MessageSending messageSendingStandUp;
	MessageSending messageSendingPutCard;
	MessageSending messageSendingGetCard;
	MessageSending messageSendingSkipTurn;
	MessageSending messageSendingCallUno;
	MessageSending messageSendingAtkUno;
	MessageSending messageSendingChat;
	MessageSending messageSendingTestFinishGame;

	public static void SelfDestruction(){
		ins = null;
	}

	public Uno_RealTimeAPI(){}

	public void SendMessageSitDown(byte _indexChair){
		if (messageSendingSitDown == null) {
			messageSendingSitDown = new MessageSending (CMD_REALTIME.C_GAMEPLAY_SITDOWN);
		} else {
			messageSendingSitDown.ClearData ();
		}

		messageSendingSitDown.writeByte (_indexChair);

		#if TEST
		string _tmp = string.Empty;
		_tmp += _indexChair;
		Debug.Log(">>>CMD SitDown : " + messageSendingSitDown.getCMD() + "|" + _tmp);
		#endif

		NetworkGlobal.instance.SendMessageRealTime (messageSendingSitDown);
	}

	public void SendMessageStandUp(){
		if (messageSendingStandUp == null) {
			messageSendingStandUp = new MessageSending (CMD_REALTIME.C_GAMEPLAY_STANDUP);
		}else{
			messageSendingStandUp.ClearData ();
		}

		#if TEST
		Debug.Log(">>>CMD StandUp : " + messageSendingStandUp.getCMD());
		#endif

		NetworkGlobal.instance.SendMessageRealTime (messageSendingStandUp);
	}

	public void SendMessagePutCard(int _cardValue, int _indexOfMyCards){
		if (messageSendingPutCard == null) {
			messageSendingPutCard = new MessageSending (CMD_REALTIME.C_GAMEPLAY_PUT_CARD);
		}else{
			messageSendingPutCard.ClearData ();
		}
		messageSendingPutCard.writeByte((byte) _cardValue);
		messageSendingPutCard.writeByte((byte) _indexOfMyCards);

		#if TEST
		string _tmp = string.Empty;
		_tmp += _cardValue;
		Debug.Log(">>>CMD Put Card : " + messageSendingPutCard.getCMD() + "|" + _tmp);
		#endif

		NetworkGlobal.instance.SendMessageRealTime (messageSendingPutCard);
	}

	public void SendMessageGetCard(){
		if (messageSendingGetCard == null) {
			messageSendingGetCard = new MessageSending (CMD_REALTIME.C_GAMEPLAY_GET_CARD);
		}else{
			messageSendingGetCard.ClearData ();
		}

		#if TEST
		Debug.Log(">>>CMD GetCard : " + messageSendingGetCard.getCMD());
		#endif

		NetworkGlobal.instance.SendMessageRealTime (messageSendingGetCard);
	}

	public void SendMessageSkipTurn(){
		if (messageSendingSkipTurn == null) {
			messageSendingSkipTurn = new MessageSending (CMD_REALTIME.C_GAMEPLAY_END_TURN);
		}else{
			messageSendingSkipTurn.ClearData ();
		}

		#if TEST
		Debug.Log(">>>CMD SkipTurn : " + messageSendingSkipTurn.getCMD());
		#endif

		NetworkGlobal.instance.SendMessageRealTime (messageSendingSkipTurn);
	}

	public void SendMessageCallUno(){
		if (messageSendingCallUno == null) {
			messageSendingCallUno = new MessageSending (CMD_REALTIME.C_GAMEPLAY_CALL_WIN);
		}else{
			messageSendingCallUno.ClearData ();
		}

		#if TEST
		Debug.Log(">>>CMD Call Uno : " + messageSendingCallUno.getCMD());
		#endif

		NetworkGlobal.instance.SendMessageRealTime (messageSendingCallUno);
	}

	public void SendMessageAtkUno(int _indexPlaying){
		if (messageSendingAtkUno == null) {
			messageSendingAtkUno = new MessageSending (CMD_REALTIME.C_GAMEPLAY_ATTACK_WIN);
		}else{
			messageSendingAtkUno.ClearData ();
		}
		messageSendingAtkUno.writeByte((byte) _indexPlaying);

		#if TEST
		Debug.Log(">>>CMD Atk Uno : " + messageSendingAtkUno.getCMD() + " - " + _indexPlaying);
		#endif

		NetworkGlobal.instance.SendMessageRealTime (messageSendingAtkUno);
	}

	public void TestFinishGame(byte _reason){
		if (messageSendingTestFinishGame == null) {
			messageSendingTestFinishGame = new MessageSending (CMD_REALTIME.C_TEST);
		}else{
			messageSendingTestFinishGame.ClearData ();
		}
		messageSendingTestFinishGame.writeByte((byte) _reason);

		#if TEST
		Debug.Log(">>>CMD Test Finish Game : " + messageSendingTestFinishGame.getCMD() + " - " + _reason);
		#endif

		NetworkGlobal.instance.SendMessageRealTime (messageSendingTestFinishGame);
	}

	public void SendMessageChat(string _message){
		if (messageSendingChat == null) {
			messageSendingChat = new MessageSending (CMD_REALTIME.C_GAMEPLAY_CHAT_IN_TABLE);
		} else {
			messageSendingChat.ClearData ();
		}

		messageSendingChat.writeString (_message);

		string _tmp = string.Empty;
		_tmp += _message;

		#if TEST
		Debug.Log(">>>CMD Chat : " + messageSendingChat.getCMD() + "|" + _tmp);
		#endif

		NetworkGlobal.instance.SendMessageRealTime (messageSendingChat);
	}
}
