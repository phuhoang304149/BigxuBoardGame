using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poker_RealTimeAPI {

	public static Poker_RealTimeAPI instance {
        get {
            if (ins == null) {
				ins = new Poker_RealTimeAPI();
            }
            return ins;
        }
    }
	static Poker_RealTimeAPI ins;

	MessageSending messageSendingSetBet;
	MessageSending messageSendingSitDown;
	MessageSending messageSendingStandUp;
	MessageSending messageSendingChat;

	public static void SelfDestruction(){
		ins = null;
	}

	public Poker_RealTimeAPI(){}

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
		} else {
			messageSendingStandUp.ClearData ();
		}

		#if TEST
		Debug.Log(">>>CMD StandUp : " + messageSendingStandUp.getCMD());
		#endif

		NetworkGlobal.instance.SendMessageRealTime (messageSendingStandUp);
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

	public void SendMessageSetBet(long _goldBet){
		if (messageSendingSetBet == null) {
			messageSendingSetBet = new MessageSending (CMD_REALTIME.C_GAMEPLAY_SETBET);
		} else {
			messageSendingSetBet.ClearData ();
		}

		messageSendingSetBet.writeLong (_goldBet);

		#if TEST
		string _tmp = string.Empty;
		_tmp += _goldBet;
		Debug.Log(">>>CMD SetBet : " + messageSendingSetBet.getCMD() + "|" + _tmp);
		#endif

		NetworkGlobal.instance.SendMessageRealTime (messageSendingSetBet);
	}
}
