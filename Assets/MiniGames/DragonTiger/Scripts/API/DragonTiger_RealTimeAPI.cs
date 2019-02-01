using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonTiger_RealTimeAPI {

	public static DragonTiger_RealTimeAPI instance {
        get {
            if (ins == null) {
				ins = new DragonTiger_RealTimeAPI();
            }
            return ins;
        }
    }
	static DragonTiger_RealTimeAPI ins;

	MessageSending messageSendingAddBet;
	MessageSending messageSendingChat;

	public static void SelfDestruction(){
		ins = null;
	}

	public DragonTiger_RealTimeAPI(){}

	public void SendMessageAddBet(byte _indexBet, short _indexChip, long _goldAdd){
		if (messageSendingAddBet == null) {
			messageSendingAddBet = new MessageSending (CMD_REALTIME.C_MINIGAME_LONGHO_ADDBET);
		} else {
			messageSendingAddBet.ClearData ();
		}

		messageSendingAddBet.writeByte (_indexBet);
		messageSendingAddBet.writeshort (_indexChip);
		messageSendingAddBet.writeLong (_goldAdd);

		string _tmp = string.Empty;
		_tmp += _indexBet + "|" + _goldAdd + "|" + _indexChip;

		#if TEST
		Debug.Log(">>>CMD AddBet : " + messageSendingAddBet.getCMD() + "|" + _tmp);
		#endif

		NetworkGlobal.instance.SendMessageRealTime (messageSendingAddBet);
	}

	public void SendMessageChat(string _message){
		if (messageSendingChat == null) {
			messageSendingChat = new MessageSending (CMD_REALTIME.C_MINIGAME_LONGHO_CHAT_ALL);
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
