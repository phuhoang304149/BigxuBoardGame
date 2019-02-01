using System;
using System.IO;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OneHitGame {
    private const int BUFFER_SENDING = 8192;
    private const long TIME_OUT = 3689;
    private const float  TIME_SLEEP = 0.01f;

    private TcpClient tcpClient;
    private NetworkStream networkStream;

    public long timeStart;

    public Action<MessageReceiving> onReceiveMessage;
    public Action<int> onNetworkError;

    private MessageSending messageSending;
    private List<IpDetail> listIpConnect;
    private IpDetail currentIPDetail;

    string serverName;

    public OneHitGame(MessageSending _messageSending) {
        serverName = "listIpForOneHit";
        listIpConnect = new List<IpDetail>();
        List<IpDetail> _newListIpConnect = new List<IpDetail>();
        for(int i = 0; i < DataManager.instance.subServerData.listIpForOneHit.Count; i++){
            _newListIpConnect.Add(DataManager.instance.subServerData.listIpForOneHit[i]);
        }
        _newListIpConnect.Sort(delegate (IpDetail _ipDetailSau, IpDetail _ipDetailTruoc) // y.CompareTo(x) = -1 là dịch về phía sau
        {
            // -1 là dịch về phía trước
            if(_ipDetailTruoc.timeDelay == -1){
                return 1;
            }

            if(_ipDetailSau.timeDelay > _ipDetailTruoc.timeDelay){
                return 1;
            }else if(_ipDetailSau.timeDelay == _ipDetailTruoc.timeDelay){
                if(_ipDetailSau.countConnectionError > _ipDetailTruoc.countConnectionError){
                    return 1;
                }else if(_ipDetailSau.countConnectionError == _ipDetailTruoc.countConnectionError){
                    return 0;
                }else{
                    return -1;
                }
            }else{
                return -1;
            }
        });
        for(int i = 0; i < _newListIpConnect.Count; i++){
            if(!_newListIpConnect[i].beingError){
                // Debug.Log(">>> Khong Loi: " + _newListIpConnect[i][i].ip + ":" + _newListIpConnect[i].port_onehit);
                listIpConnect.Add(_newListIpConnect[i]);
            }
        }
        for(int i = 0; i < _newListIpConnect.Count; i++){
            if(_newListIpConnect[i].beingError){
                // Debug.Log(">>> Loi: " + _newListIpConnect[i][i].ip + ":" + _newListIpConnect[i].port_onehit);
                listIpConnect.Add(_newListIpConnect[i]);
            }
        }
        messageSending = _messageSending;
    }

    public OneHitGame(SubServerDetail _subServerDeail, MessageSending _messageSending) {
        // #if TEST
        // Debug.Log(">>> Kết nối SubServerDetail: " + _subServerDeail.subServerName);
        // #endif
        serverName = _subServerDeail.subServerName;
        listIpConnect = new List<IpDetail>();
        List<IpDetail> _newListIpConnect = new List<IpDetail>();
        for(int i = 0; i < _subServerDeail.listIpDetail.Count; i++){
            _newListIpConnect.Add(_subServerDeail.listIpDetail[i]);
        }
        _newListIpConnect.Sort(delegate (IpDetail _ipDetailSau, IpDetail _ipDetailTruoc) // y.CompareTo(x) = -1 là dịch về phía sau
        {
            // -1 là dịch về phía trước
            if(_ipDetailTruoc.timeDelay == -1){
                return 1;
            }

            if(_ipDetailSau.timeDelay > _ipDetailTruoc.timeDelay){
                return 1;
            }else if(_ipDetailSau.timeDelay == _ipDetailTruoc.timeDelay){
                if(_ipDetailSau.countConnectionError > _ipDetailTruoc.countConnectionError){
                    return 1;
                }else if(_ipDetailSau.countConnectionError == _ipDetailTruoc.countConnectionError){
                    return 0;
                }else{
                    return -1;
                }
            }else{
                return -1;
            }
        });

        for(int i = 0; i < _newListIpConnect.Count; i++){
            if(!_newListIpConnect[i].beingError){
                // Debug.Log(">>> Khong Loi: " + _newListIpConnect[i][i].ip + ":" + _newListIpConnect[i].port_onehit);
                listIpConnect.Add(_newListIpConnect[i]);
            }
        }
        for(int i = 0; i < _newListIpConnect.Count; i++){
            if(_newListIpConnect[i].beingError){
                // Debug.Log(">>> Loi: " + _newListIpConnect[i][i].ip + ":" + _newListIpConnect[i].port_onehit);
                listIpConnect.Add(_newListIpConnect[i]);
            }
        }
        messageSending = _messageSending;
    }

    public IEnumerator runNetwork() {
        timeStart = currentTimeMillis;
        tcpClient = null;
        networkStream = null;
        long timeOut;
        
        int numberIpConnect = listIpConnect.Count;
        for (int i = 0; i < numberIpConnect; i++) {
            currentIPDetail = listIpConnect[i];
            if (networkStream != null) networkStream.Close();
            if (tcpClient != null) tcpClient.Close();
            if(currentIPDetail.ipType == IpDetail.Type.IPv6)
                tcpClient = new TcpClient(AddressFamily.InterNetworkV6);
            else
                tcpClient = new TcpClient();
            // Debug.Log(">>> " + currentIP + ":" + currentPort);

            try {
                //tcpClient.ConnectAsync(currentIPDetail.ip, currentIPDetail.port_onehit);
                tcpClient.BeginConnect(currentIPDetail.ip, currentIPDetail.port_onehit,null,null);
            } catch (Exception s) {
                currentIPDetail.beingError = true;
                currentIPDetail.countConnectionError ++;
                currentIPDetail = null;
                tcpClient.Close();
                continue;
            }
            timeOut=currentTimeMillis+1258;
            while(currentTimeMillis < timeOut){
                if(tcpClient.Connected){
                    break;
                } else{
                    yield return Yielders.Get(TIME_SLEEP);
                }
            }
            if(tcpClient.Connected){
                networkStream = tcpClient.GetStream();
                break;
            }else{
                currentIPDetail.beingError = true;
                currentIPDetail.countConnectionError ++;
                currentIPDetail = null;
                tcpClient.Close();
            }
        }
        if (currentIPDetail == null) {
            #if TEST
            Debug.LogError("Lỗi tạo kết nối đến server " + serverName);
            for (int i = 0; i < numberIpConnect; i++){
                Debug.LogError("---->"+ listIpConnect[i].ip+":"+ listIpConnect[i].port_onehit);
            }
            #endif
            CleanAndStopNetwork(1);
            yield break;
        }

        long _startTime = currentTimeMillis;
        
        timeOut = currentTimeMillis + TIME_OUT;
        while (currentTimeMillis < timeOut)
            if (tcpClient.Available < 8)
                yield return Yielders.Get(TIME_SLEEP);
            else
                break;

        if (currentTimeMillis > timeOut) {//Lỗi mạng : kết nối được nhưng lỗi tín hiệu
            CleanAndStopNetwork(10);
            yield break;
        }


        byte[] validateData = new byte[8];
        networkStream.Read(validateData, 0, validateData.Length);
        byte validateCode = validateData[2];

        byte[] dataMessage = messageSending.getBytesArray();
        byte[] dataSending = new byte[dataMessage.Length + 7 + 4];
        dataSending[0] = (byte)(validateData[0] ^ validateCode);
        dataSending[1] = (byte)(validateData[1] ^ validateCode);
        dataSending[2] = (byte)(validateData[3] ^ validateCode);
        dataSending[3] = (byte)(validateData[4] ^ validateCode);
        dataSending[4] = (byte)(validateData[5] ^ validateCode);
        dataSending[5] = (byte)(validateData[6] ^ validateCode);
        dataSending[6] = (byte)(validateData[7] ^ validateCode);
        int lengthData = dataMessage.Length;
        int l = lengthData;
        dataSending[7] = (byte)(l >> 24);
        dataSending[8] = (byte)(l >> 16);
        dataSending[9] = (byte)(l >> 8);
        dataSending[10] = (byte)l;
        for (int i = 0; i < lengthData; i++)
            dataSending[i + 11] = (byte)(dataMessage[i] ^ validateCode);

        /*Nếu dữ liệu trên 8k cần code lại chỗ này*/
        if (dataSending.Length > BUFFER_SENDING) { Debug.LogError("ERROR ONEHIT : cần code lại phần send dữ liệu quá 8k"); yield break;}
        try {
            networkStream.Write(dataSending, 0, dataSending.Length);
            timeOut = currentTimeMillis + TIME_OUT;
        } catch (IOException e) {
            CleanAndStopNetwork(11);
            yield break;
        }

        /*Nếu không setup hàm nhận thì đóng kết nối luôn*/
        if (onReceiveMessage == null) { yield return new WaitForSeconds(5); networkStream.Close(); tcpClient.Close(); yield break; }

        while (currentTimeMillis < timeOut)
            if (tcpClient.Available > 4)
                break;
            else
                yield return Yielders.Get(TIME_SLEEP);


        if (currentTimeMillis >= timeOut) {//Gởi data không hợp lệ bị server ngắt kết nối và không trả về gì
#if TEST
            Debug.LogError("Lỗi gởi sai dữ liệu Onehit(" + currentIPDetail.ip + ":" + currentIPDetail.port_onehit + ")➡ " + CMD_ONEHIT.getCMDName(messageSending.getCMD()) + " " + messageSending.getBytesArray().Length + " byte");
#endif
            CleanAndStopNetwork(12);
            yield break;
        }

        timeOut=currentTimeMillis+8952;
        byte[] dataReceive = new byte[4];
        networkStream.Read(dataReceive, 0, dataReceive.Length);

        int lengthDataReceive = (dataReceive[0] << 24) + (dataReceive[1] << 16) + (dataReceive[2] << 8) + (dataReceive[3] << 0);
        dataReceive = new byte[lengthDataReceive];

        if (lengthDataReceive + 4 <= BUFFER_SENDING) {
            while (currentTimeMillis < timeOut)
                if (tcpClient.Available < lengthDataReceive)
                    yield return Yielders.Get(TIME_SLEEP);
                else
                    break;
            if (currentTimeMillis >= timeOut || tcpClient.Available < lengthDataReceive) {//Gởi data không hợp lệ bị server ngắt kết nối và không trả về gì
#if TEST
                Debug.LogError("Lỗi network nhận Onehit(" + currentIPDetail.ip + ":" + currentIPDetail.port_onehit + ")➡ " + CMD_ONEHIT.getCMDName(messageSending.getCMD()) + " " + messageSending.getBytesArray().Length + " byte");
#endif
                CleanAndStopNetwork(13);
                yield break;
            } else {
                networkStream.Read(dataReceive, 0, lengthDataReceive);
            }
        } else {
            while (currentTimeMillis < timeOut)
                if (tcpClient.Available < BUFFER_SENDING - 4)
                    yield return Yielders.Get(TIME_SLEEP);
                else
                    break;
            networkStream.Read(dataReceive, 0, BUFFER_SENDING - 4);
            networkStream.WriteByte(1);
            int count = BUFFER_SENDING - 4;
            while (count < lengthDataReceive && currentTimeMillis < timeOut) {
                if (count + BUFFER_SENDING < lengthDataReceive) {
                    if (tcpClient.Available >= BUFFER_SENDING) {
                        networkStream.Read(dataReceive, count, BUFFER_SENDING);
                        count = count + BUFFER_SENDING;
                        timeOut = currentTimeMillis + TIME_OUT;
                        networkStream.WriteByte(1);//Nhận được là gởi 1 đi
                    } else
                        yield return Yielders.Get(TIME_SLEEP);
                } else {
                    if (tcpClient.Available >= lengthDataReceive - count) {
                        networkStream.Read(dataReceive, count, lengthDataReceive - count);
                        networkStream.WriteByte(1);
                        count = lengthDataReceive;
                        break;
                    } else
                        yield return Yielders.Get(TIME_SLEEP);
                }
            }
            if (count < lengthDataReceive || currentTimeMillis >= timeOut) {
#if TEST
                Debug.LogError("Lỗi network nhận Onehit(" + currentIPDetail.ip + ":" + currentIPDetail.port_onehit + ")➡ " + CMD_ONEHIT.getCMDName(messageSending.getCMD()) + " " + messageSending.getBytesArray().Length + " byte");
#endif
                CleanAndStopNetwork(14);
                yield break;
            }
        }

        for (int i = 0; i < dataReceive.Length; i++)
            dataReceive[i] = (byte)(dataReceive[i] ^ validateCode);
        MessageReceiving messageReceiving = new MessageReceiving(dataReceive);
        if (messageReceiving.getCMD() == -7 || messageReceiving.getCMD()==-17) {
#if TEST
        if (messageReceiving.getCMD() == -7)
            Debug.LogError("Lỗi network không tìm được CMD Onehit(" + currentIPDetail.ip + ":" + currentIPDetail.port_onehit + ")➡ " + CMD_ONEHIT.getCMDName(messageSending.getCMD()) + " " + messageSending.getBytesArray().Length + " byte");
        else
            Debug.LogError("Lỗi Foward CMD Onehit(" + currentIPDetail.ip + ":" + currentIPDetail.port_onehit + ")➡ " + CMD_ONEHIT.getCMDName(messageSending.getCMD()) + " " + messageSending.getBytesArray().Length + " byte");
#endif
            CleanAndStopNetwork(15);
            yield break;
        } else {
            onReceiveMessage(messageReceiving);
#if TEST
            if (messageReceiving.avaiable() > 0)
                Debug.LogError("Lỗi chưa đọc hết CMD Onehit(" + currentIPDetail.ip + ":" + currentIPDetail.port_onehit + ")➡ " + CMD_ONEHIT.getCMDName(messageSending.getCMD()) + " " + messageSending.getBytesArray().Length + " byte");
            else
                Debug.LogWarning(currentIPDetail.ip + ":" + currentIPDetail.port_onehit + "  --->" + CMD_ONEHIT.getCMDName(messageSending.getCMD()) + " Send(" + messageSending.getBytesArray().Length + ")➡Receive(" + messageReceiving.lengthReceive() + ")" + (currentTimeMillis - timeStart) + " Giây");
#endif
        }

        CleanAndStopNetwork(-1, currentTimeMillis - _startTime);
        yield break;
    }


    private void CleanAndStopNetwork(int errorCore = -1, long _timeDelay = 0) {
        if (errorCore != -1) {
            if(currentIPDetail != null){
                currentIPDetail.beingError = true;
                currentIPDetail.countConnectionError ++;
            }
            if(onNetworkError != null){
                onNetworkError(errorCore);
            }
        }else{
            if(currentIPDetail != null){
                currentIPDetail.beingError = false;
                currentIPDetail.timeDelay = _timeDelay;
            }
        }
        if (networkStream != null) networkStream.Close();
        if(tcpClient != null) tcpClient.Close();

        listIpConnect = null;
        currentIPDetail = null;
    }
    
    private long currentTimeMillis { get { return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds; } }
}
