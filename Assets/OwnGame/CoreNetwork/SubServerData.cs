using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable] public class SubServerData {
	public List<SubServerDetail> listSubServerDetail;
    public List<IpDetail> listIpForOneHit;
	public bool isInitialized;

	public SubServerData(){}

	public void InitData(){
        SetListSubServerDetailDefaultData();
        SetListIpForOneHitDefaultData();

		isInitialized = true;
	}

    void SetListSubServerDetailDefaultData(){
        listSubServerDetail = new List<SubServerDetail>();

        SubServerDetail _subServerDetail = new SubServerDetail();
        _subServerDetail.subServerId = 0;
        _subServerDetail.subServerName = "Global";
		_subServerDetail.version = 20181122;
        _subServerDetail.countryCode = "VN";

		IpDetail _tmpIpDetail = new IpDetail();
        _tmpIpDetail.ipId = -1;
        _tmpIpDetail.ipType = IpDetail.Type.IPv4;
		_tmpIpDetail.ip = "subv4.bigxuonline.com";
		_tmpIpDetail.port_onehit = 2598;
		_tmpIpDetail.port_realtime = 2589;
		_tmpIpDetail.port_test = 2704;
		_subServerDetail.listIpDetail.Add(_tmpIpDetail);

        _tmpIpDetail = new IpDetail();
        _tmpIpDetail.ipId = -2;
        _tmpIpDetail.ipType = IpDetail.Type.IPv6;
		_tmpIpDetail.ip = "subv6.bigxuonline.com";
		_tmpIpDetail.port_onehit = 2598;
		_tmpIpDetail.port_realtime = 2589;
		_tmpIpDetail.port_test = 2704;
        _subServerDetail.listIpDetail.Add(_tmpIpDetail);

        _tmpIpDetail = new IpDetail();
        _tmpIpDetail.ipId = -3;
        _tmpIpDetail.ipType = IpDetail.Type.IPv4;
		_tmpIpDetail.ip = "subv4.battleoflegend.com";
		_tmpIpDetail.port_onehit = 2598;
		_tmpIpDetail.port_realtime = 2589;
		_tmpIpDetail.port_test = 2704;
        _subServerDetail.listIpDetail.Add(_tmpIpDetail);

        _tmpIpDetail = new IpDetail();
        _tmpIpDetail.ipId = -4;
        _tmpIpDetail.ipType = IpDetail.Type.IPv6;
		_tmpIpDetail.ip = "subv6.battleoflegend.com";
		_tmpIpDetail.port_onehit = 2598;
		_tmpIpDetail.port_realtime = 2589;
		_tmpIpDetail.port_test = 2704;
        _subServerDetail.listIpDetail.Add(_tmpIpDetail);

        listSubServerDetail.Add(_subServerDetail);
    }

    void SetListIpForOneHitDefaultData(){
        listIpForOneHit = new List<IpDetail>();
        IpDetail _tmpIpDetail = null;

        _tmpIpDetail = new IpDetail();
        _tmpIpDetail.ipId = -1;
        _tmpIpDetail.ipType = IpDetail.Type.IPv4;
        _tmpIpDetail.ip = "subv4.bigxuonline.com";
        _tmpIpDetail.port_onehit = 2598;
        listIpForOneHit.Add(_tmpIpDetail);

        _tmpIpDetail = new IpDetail();
        _tmpIpDetail.ipId = -2;
        _tmpIpDetail.ipType = IpDetail.Type.IPv6;
        _tmpIpDetail.ip = "subv6.bigxuonline.com";
        _tmpIpDetail.port_onehit = 2598;
        listIpForOneHit.Add(_tmpIpDetail);

        _tmpIpDetail = new IpDetail();
        _tmpIpDetail.ipId = -3;
        _tmpIpDetail.ipType = IpDetail.Type.IPv4;
        _tmpIpDetail.ip = "subv4.battleoflegend.com";
        _tmpIpDetail.port_onehit = 2598;
        listIpForOneHit.Add(_tmpIpDetail);

        _tmpIpDetail = new IpDetail();
        _tmpIpDetail.ipId = -4;
        _tmpIpDetail.ipType = IpDetail.Type.IPv6;
        _tmpIpDetail.ip = "subv6.battleoflegend.com";
        _tmpIpDetail.port_onehit = 2598;
        listIpForOneHit.Add(_tmpIpDetail);
    }

    public void LoadSubServerDataFromSv(List<SubServerDetail> _newListSub){
        if(_newListSub == null || _newListSub.Count == 0){
            #if TEST
            Debug.LogError(">>> _newListSub is null or count = 0");
            #endif
            return;
        }

        // --- Merge dữ liệu mới và cũ --- //
        for(int i = 0; i < _newListSub.Count; i++){
            for(int j = 0; j < listSubServerDetail.Count;j++){
                if(listSubServerDetail[j].IsEqual(_newListSub[i])){
                    _newListSub[i].InitDataAgain(listSubServerDetail[j]);
                    break;
                }
            }
        }
        
        listSubServerDetail = _newListSub;
    }

    public void LoadListIpForOneHitDataFromSv(List<IpDetail> _newListIpDetail){
        if(_newListIpDetail == null || _newListIpDetail.Count == 0){
            #if TEST
            Debug.LogError(">>> _newListIpDetail is null or count = 0");
            #endif
            return;
        }

        // --- Merge dữ liệu mới và cũ --- //
        for(int i = 0; i < _newListIpDetail.Count; i++){
            for(int j = 0; j < listIpForOneHit.Count;j++){
                if(listIpForOneHit[j].IsEqual(_newListIpDetail[i])){
                    _newListIpDetail[i].InitDataAgain(listIpForOneHit[j]);
                    break;
                }
            }
        }

        IpDetail _tmpIpDetail = new IpDetail();
        _tmpIpDetail.ipId = -1;
        _tmpIpDetail.ipType = IpDetail.Type.IPv4;
        _tmpIpDetail.ip = "subv4.bigxuonline.com";
        _tmpIpDetail.port_onehit = 2598;
        _tmpIpDetail.beingError = true;
        _newListIpDetail.Add(_tmpIpDetail);

        _tmpIpDetail = new IpDetail();
        _tmpIpDetail.ipId = -2;
        _tmpIpDetail.ipType = IpDetail.Type.IPv6;
        _tmpIpDetail.ip = "subv6.bigxuonline.com";
        _tmpIpDetail.port_onehit = 2598;
        _tmpIpDetail.beingError = true;
        _newListIpDetail.Add(_tmpIpDetail);

        _tmpIpDetail = new IpDetail();
        _tmpIpDetail.ipId = -3;
        _tmpIpDetail.ipType = IpDetail.Type.IPv4;
        _tmpIpDetail.ip = "subv4.battleoflegend.com";
        _tmpIpDetail.port_onehit = 2598;
        _tmpIpDetail.beingError = true;
        _newListIpDetail.Add(_tmpIpDetail);

        _tmpIpDetail = new IpDetail();
        _tmpIpDetail.ipId = -4;
        _tmpIpDetail.ipType = IpDetail.Type.IPv6;
        _tmpIpDetail.ip = "subv6.battleoflegend.com";
        _tmpIpDetail.port_onehit = 2598;
        _tmpIpDetail.beingError = true;
        _newListIpDetail.Add(_tmpIpDetail);
        
        listIpForOneHit = _newListIpDetail;
    }

    public void CheckWhenLogin(){
        if(listSubServerDetail == null || listSubServerDetail.Count == 0){
            SetListSubServerDetailDefaultData();
        }else{
            for(int i = 0; i < listSubServerDetail.Count; i++){
                listSubServerDetail[i].CheckWhenLogin();
            }
        }
        if(listIpForOneHit == null || listIpForOneHit.Count == 0){
            SetListIpForOneHitDefaultData();
        }else{
            for(int i = 0; i < listIpForOneHit.Count; i++){
                if(listIpForOneHit[i].ipId < 0){
                    listIpForOneHit[i].beingError = true;
                }
            }
        }
    }

    public SubServerDetail GetSubServerDetail(int _subServerId){
        if(listSubServerDetail == null || listSubServerDetail.Count == 0){
            #if TEST
            Debug.LogError(">>> BUG Logic: listSubServerDetail is NULL");
            #endif
            return null;
        }
        for(int i = 0; i < listSubServerDetail.Count; i++){
            if(listSubServerDetail[i].subServerId == _subServerId){
                return listSubServerDetail[i];
            }
        }
        return null;
    }
}

[Serializable] public class SubServerDetail {
	public int subServerId;
	public string subServerName;
	public long version;
	public string countryCode;
	public List<IpDetail> listIpDetail;
	public List<RoomDetail> listRoomDetail;
    public bool beingError;
    public long countConnectionError;
    public long timeDelay{
        get{
            long _tmpTimeDelay = -1;
            if(beingError){
                return _tmpTimeDelay;
            }
            for(int i = 0; i < listIpDetail.Count; i++){
                if(listIpDetail[i].beingError){
                    continue;
                }
                if(listIpDetail[i].timeDelay == -1){
                    continue;
                }
                if(_tmpTimeDelay == -1){
                    _tmpTimeDelay = listIpDetail[i].timeDelay;
                }else{
                    if(listIpDetail[i].timeDelay < _tmpTimeDelay){
                        _tmpTimeDelay = listIpDetail[i].timeDelay;
                    }
                }
            }
            return _tmpTimeDelay;
        }
    }

	public SubServerDetail(){
		listIpDetail = new List<IpDetail>();
		listRoomDetail = new List<RoomDetail>();
	}

	public SubServerDetail(MessageReceiving _mess){
		listIpDetail = new List<IpDetail>();
		listRoomDetail = new List<RoomDetail>();

        subServerId = _mess.readInt();
		version = _mess.readLong();
        subServerName = _mess.readString();
        countryCode = _mess.readString();

		short _numberRoomDetail = _mess.readShort();
        RoomDetail _tmpNewRoomDetail = null;
        for(int i = 0; i < _numberRoomDetail; i++){
            _tmpNewRoomDetail = new RoomDetail(_mess);
            listRoomDetail.Add(_tmpNewRoomDetail);
        }

        short _numberIpDetail = _mess.readShort();
        IpDetail _tmpIpDetail = null;
        for(int i = 0; i < _numberIpDetail; i++){
            _tmpIpDetail = new IpDetail(_mess);
            listIpDetail.Add(_tmpIpDetail);
        }
    }

    public void InitDataAgain(SubServerDetail _otherServer){
        beingError = _otherServer.beingError;
        countConnectionError = _otherServer.countConnectionError;
        for(int i = 0; i < listIpDetail.Count; i++){
            for(int j = 0; j < _otherServer.listIpDetail.Count;j++){
                if(listIpDetail[i].IsEqual(_otherServer.listIpDetail[j])){
                    listIpDetail[i].InitDataAgain(_otherServer.listIpDetail[j]);
                    break;
                }
            }
        }
    }

    public void CheckWhenLogin(){
        if(listIpDetail == null){
            listIpDetail = new List<IpDetail>();
        }
        if(listRoomDetail == null){
            listRoomDetail = new List<RoomDetail>();
        }
    }

    public bool IsContainMiniGame(MiniGameDetail _gameDetail){
        if(_gameDetail.myInfo.isSubGame){
            return true;
        }
        if(listRoomDetail == null || listRoomDetail.Count == 0){
            return false;
        }
        for(int i = 0; i < listRoomDetail.Count; i ++){
            if(_gameDetail.myInfo.gameId == listRoomDetail[i].gameId){
                return true;
            }
        }
        return false;
    }

    public bool IsEqual(SubServerDetail _other){
        if(subServerId == _other.subServerId){
            return true;
        }
        return false;
    }
}

[Serializable] public class RoomDetail{
	public int roomId;
	public string roomName; // ví dụ : Poker, Uno, ...
	public long versionRoom;
	public short gameId;
    public short numberTable;

    public RoomDetail(MessageReceiving _mess){
        roomId = _mess.readInt();
        roomName = _mess.readString();
        versionRoom = _mess.readLong();
        gameId = _mess.readShort();
        numberTable = _mess.readShort();
    }
}

[Serializable] public class IpDetail {
    public enum Type{
        IPv4, IPv6
    }
    public Type ipType;
    public int ipId;
	public string ip;
    public int port_onehit;
    public int port_realtime;
    public int port_test;
    public bool beingError;
    public long countConnectionError;
    public long timeDelay;

    public IpDetail(){
        beingError = false;
        countConnectionError = 0;
        timeDelay = -1;
    }

    public IpDetail(MessageReceiving _mess){
        ipId = _mess.readInt();
        ip = _mess.readString();
        port_onehit = _mess.readInt();
        port_realtime = _mess.readInt();
        port_test = _mess.readInt();

        if(ip.Contains(":")||ip.Contains("v6")){
            ipType = Type.IPv6;
        }else{
            ipType = Type.IPv4;
        }

        beingError = false;
        countConnectionError = 0;
        timeDelay = -1;
    }

    public void InitDataAgain(IpDetail _other){
        beingError = _other.beingError;
        countConnectionError = _other.countConnectionError;
        timeDelay = _other.timeDelay;
    }

    public bool IsEqual(IpDetail _other){
        if(ipId == _other.ipId){
            return true;
        }
        return false;
    }
}