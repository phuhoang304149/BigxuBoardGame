using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryServerData {

	public List<TemporaryServerDetail> listDetail;
    public bool isInitialized;

    public TemporaryServerData(){
        listDetail = new List<TemporaryServerDetail>();
    }

    /// <summary>
    /// InitDefaultData : Gọi Init khi không lấy được dữ liệu ban đầu của SV
    /// </summary>
    public void InitData(){
        if(listDetail == null){
            listDetail = new List<TemporaryServerDetail>();
        }else{
            listDetail.Clear();
        }
        isInitialized = true;
	}
}

public class TemporaryServerDetail {
	public int roomId;
    public long version;
    public short gameId;
    public int subServerId;

    public TemporaryServerDetail(){}
	public TemporaryServerDetail(MessageReceiving _messageReceiving){
        roomId = _messageReceiving.readInt();
        version = _messageReceiving.readLong();
        gameId = _messageReceiving.readShort();
        subServerId = _messageReceiving.readInt();
    }
}
