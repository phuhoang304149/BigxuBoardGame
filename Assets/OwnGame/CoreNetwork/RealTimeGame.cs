using System;
using System.IO;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealTimeGame {
    public const long TIME_OUT = 16860;
    public const long TIME_PING = 1000;
    public const long TIME_RECONNECT = 3000;
    public const float TIME_SLEEP = 0.01f;

    private TcpClient tcpClient;
    private NetworkStream networkStream;

    private IEnumerator actionRunNetwork;
    public MonoBehaviour monoBehaviour;
    public short sessionId;
    public long timeStart;
    private List<IpDetail> listIpConnect;

    public RealTimeGame(SubServerDetail _subServerDeail, MonoBehaviour mono) {
        listIpConnect = _subServerDeail.listIpDetail;
        monoBehaviour = mono;
        timeStart = currentTimeMillis;
        sessionId = -1;

        isProcessReceive = true;
        listWait = null;
        isRelease = false;
        lockSend = new System.Object();
        dataServer = new byte[4];

        onConnectSuccess = null;
        onServerFull = null;
        onReconnect = null;
        onDisconnect = null;
        onNetworkError = null;
        lockSend = new System.Object();

        actionRunNetwork = runNetwork();
    }

    private void onReceive(byte[] data) {
        if(!isRunning) return;
        IActionProcessMessage[] listProcess = NetworkGlobal.instance.listProcess;
        MessageReceiving messageReceiving = new MessageReceiving(data);
        if(messageReceiving.getCMD() == -4){
            monoBehaviour.StartCoroutine(release());
            return;
        }
        int BUFFER_PROCESS = listProcess.Length;
        for (int i = 0; i < BUFFER_PROCESS; i++)
            if (listProcess[i].cmd == messageReceiving.getCMD()) {
                listProcess[i].functionProcess(messageReceiving);
                if (messageReceiving.avaiable() > 0)
                    Debug.Log("**********Chua doc het CMD : " + messageReceiving.getCMDName());
                else
                    Debug.Log("**********Receive CMD : " + messageReceiving.getCMDName());
                return;
            }
        Debug.Log("**********Chưa setup CMD : " + messageReceiving.getCMDName());
    }

    public Action onConnectSuccess,onServerFull,onReconnect,onDisconnect;
    public Action<int> onNetworkError;
    public void closeConnection() { isRunning = false; }
    public void Start() {monoBehaviour.StartCoroutine(actionRunNetwork);}

    public void Stop(){monoBehaviour.StopCoroutine(actionRunNetwork);monoBehaviour.StartCoroutine(release());}

    public byte validateCode = 0;
    public byte[] validateData = new byte[7];
    public bool isRunning;
    public long timeStop;
    public long nextTimeReconnect;
    public long nextTimePing;

    private IpDetail currentIPDetail;
    private System.Object lockSend;
    public void sendThread(byte[] data) {
        lock (lockSend) {
            try {
                networkStream.Write(data, 0, data.Length);
                nextTimePing = currentTimeMillis + TIME_PING;
                timeStop = currentTimeMillis + TIME_OUT;
                if (data.Length == 4) networkStream.Flush();
                return;
            } catch (Exception e) {}
        }
        actionReconnect();
    }

    private IEnumerator runNetwork() {
        timeStart = currentTimeMillis;
        tcpClient = null;
        networkStream = null;
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
                //tcpClient.ConnectAsync(currentIPDetail.ip, currentIPDetail.port_realtime);
                tcpClient.BeginConnect(currentIPDetail.ip, currentIPDetail.port_realtime, null, null);
            } catch (Exception s) {
                currentIPDetail.beingError = true;
                currentIPDetail.countConnectionError ++;
                currentIPDetail = null;
                tcpClient.Close();
                continue;
            }
            timeStop=currentTimeMillis+1258;
            while(currentTimeMillis < timeStop){
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
            // try {
            //     if (tcpClient.ConnectAsync(currentIPDetail.ip, currentIPDetail.port_realtime).Wait(258)) {
            //         networkStream = tcpClient.GetStream();
            //         break;
            //     } else {
            //         #if TEST
            //         Debug.Log("Lỗi tạo kết nối đến server: " + currentIPDetail.ip + ":" + currentIPDetail.port_realtime);
            //         #endif
            //         currentIPDetail = null;
            //     }
            // } catch (Exception s) {
            //     #if TEST
            //     Debug.Log("Lỗi tạo kết nối đến server: " + currentIPDetail.ip + ":" + currentIPDetail.port_realtime);
            //     #endif
            //     currentIPDetail = null;
            // }
        }

        if (currentIPDetail == null) {
            if (onNetworkError != null) onNetworkError(1);
            if (onDisconnect != null) onDisconnect();
             yield break;
        }
        
        timeStop = currentTimeMillis + 1268;
        while (currentTimeMillis < timeStop)
            if (tcpClient.Available < 8)
                yield return Yielders.Get(TIME_SLEEP);
            else
                break;

        isRelease = false;
        if (currentTimeMillis >= timeStop) { monoBehaviour.StartCoroutine(release()); if (onNetworkError != null) onNetworkError(2); yield break; }//Rớt mạng thì đóng kết nối

        byte[] temp = new byte[8];
        try { networkStream.Read(temp, 0, temp.Length); } catch (Exception s) { monoBehaviour.StartCoroutine(release()); if (onNetworkError != null) onNetworkError(3);yield break;/*Lỗi bị server đóng kết nối*/}

        validateCode = temp[4];
        validateData = new byte[7];
        validateData[0] = (byte)(temp[0] ^ validateCode);
        validateData[1] = (byte)(temp[1] ^ validateCode);
        validateData[2] = (byte)(temp[2] ^ validateCode);
        validateData[3] = (byte)(temp[3] ^ validateCode);
        validateData[4] = (byte)(temp[5] ^ validateCode);
        validateData[5] = (byte)(temp[6] ^ validateCode);
        validateData[6] = (byte)(temp[7] ^ validateCode);


        sessionId = -1;
        byte[] dataHeader = new byte[9];
        for (int i = 0; i < 7; i++)
            dataHeader[i] = validateData[i];
        dataHeader[7] = (byte)((int)((uint)sessionId >> 8) & 0xFF);
        dataHeader[8] = (byte)((int)((uint)sessionId >> 0) & 0xFF);

        
        try { networkStream.Write(dataHeader, 0, dataHeader.Length); } catch (Exception s) { monoBehaviour.StartCoroutine(release()); if (onNetworkError != null) onNetworkError(4);yield break;/*Lỗi mạng*/}

        while (currentTimeMillis < timeStop) if (tcpClient.Available < 2) yield return Yielders.Get(TIME_SLEEP); else break;/*Chờ lấy sessionId*/
        if (currentTimeMillis >= timeStop) { monoBehaviour.StartCoroutine(release()); if (onNetworkError != null) onNetworkError(5); yield break;/*Bị server chặn hack handshark hoặc lỗi mạng*/ }

        temp = new byte[2];
        try { networkStream.Read(temp, 0, temp.Length); } catch (Exception s) { monoBehaviour.StartCoroutine(release()); if (onNetworkError != null) onNetworkError(6); yield break;/*Lỗi bị server đóng kết nối*/}

        int t1 = temp[0];
        int t2 = temp[1];
        sessionId = (short)((t1 << 8) + (t2 << 0));//SessionId chỉ được gán ở đây
        if (sessionId == -1) {
            if (onServerFull != null) onServerFull();
            monoBehaviour.StartCoroutine(release());
        } else if (onConnectSuccess != null) onConnectSuccess();

        /*
         * Main loop
         * 
        */
        

        int t3, t4;
        int dataLength;
        byte[] dataReceive;
        isRunning = true;
        nextTimePing = currentTimeMillis + TIME_PING;
        nextTimeReconnect = currentTimeMillis + TIME_RECONNECT;
        timeStop = currentTimeMillis + TIME_OUT;
        byte[] ping = new byte[] { 0, 0, 0, 1 };

        while (currentTimeMillis < timeStop && isRunning) {
            /*Xử lý những gói tin ngừng xử lý*/
            if (isProcessReceive && listWait != null) { for (int kkk = 0; kkk < listWait.Count; kkk++) { onReceive(listWait[kkk].data); if (timeDelayListWait > 0) yield return Yielders.Get(timeDelayListWait); } listWait = null; }

            if (tcpClient.Available > 8) {
                temp = new byte[4];
                networkStream.Read(temp, 0, 4);
                t1 = temp[0];
                t2 = temp[1];
                t3 = temp[2];
                t4 = temp[3];
                dataLength = (t1 << 24) + (t2 << 16) + (t3 << 8) + (t4 << 0);

                networkStream.Read(temp, 0, 4);
                dataServer = temp;/*Tránh trường hợp bất đồng bộ*/

                if (dataLength < 2 || 8192< dataLength) {//Trường hợp lỗi
                    actionReconnect();
                    yield return Yielders.Get(TIME_SLEEP);
                    continue;
                } else {
                    dataReceive = new byte[dataLength];
                    while (currentTimeMillis < timeStop && isRunning) if (tcpClient.Available < dataLength) yield return Yielders.Get(TIME_SLEEP); else break;

                    try { networkStream.Read(dataReceive, 0, dataReceive.Length); } catch (Exception eee) { actionReconnect(); continue; }
                    for (int i = 0; i < dataLength; i++)
                        dataReceive[i] = (byte)(dataReceive[i] ^ validateCode);

                    if (isProcessReceive && listWait == null)
                        onReceive(dataReceive);
                    else
                        listWait.Add(new byteArrayTemp(dataReceive));
                }
            } else yield return Yielders.Get(TIME_SLEEP);

            if (currentTimeMillis > nextTimePing) sendThread(ping);
        }
        monoBehaviour.StartCoroutine(release());
        yield break;
    }

    private bool isRelease;
    private IEnumerator release() {
        isRunning = false;
        if (isRelease == false)
            isRelease = true;
        else
            yield break;
        if (onDisconnect!=null) onDisconnect();
        yield return Yielders.Get(3);
        networkStream.Close();
        tcpClient.Close();
        yield break;
    }

    private void actionReconnect() {if (currentTimeMillis < nextTimeReconnect) return;nextTimeReconnect = currentTimeMillis + 1268;monoBehaviour.StartCoroutine(ieReconnect());}
    private byte[] dataServer;
    public IEnumerator ieReconnect() {
		long l;
        TcpClient tcp;
        if(currentIPDetail.ipType == IpDetail.Type.IPv6)
            tcp = new TcpClient(AddressFamily.InterNetworkV6);
        else
            tcp = new TcpClient();
        NetworkStream nStream = null;
        Debug.Log("Thực hiện reconnect đến " + currentIPDetail.ip + ":" + currentIPDetail.port_realtime);

		try {
            //tcp.ConnectAsync(currentIPDetail.ip, currentIPDetail.port_realtime);
            tcp.BeginConnect(currentIPDetail.ip, currentIPDetail.port_realtime, null, null);
        } catch (Exception s) {
            tcp.Close();
            yield break;
        }
		l=currentTimeMillis + 589;
		while(currentTimeMillis < l){
			if(tcp.Connected){
				break;
			} else{
				yield return Yielders.Get(TIME_SLEEP);
			}
		}
		if(tcp.Connected){
			nStream = tcp.GetStream();
		}else{
			if (nStream != null) 
				nStream.Close();
			tcp.Close();
			yield break;
		}

//        try {
//            if (tcp.ConnectAsync(currentIPDetail.ip, currentIPDetail.port_realtime).Wait(1000))
//                nStream = tcp.GetStream();
//            else {
//                if (nStream != null) nStream.Close(); tcp.Close();
//                yield break;
//            }
//       } catch (Exception s) {
//            if (nStream != null) nStream.Close(); tcp.Close();
//            yield break;
//        }

        l = currentTimeMillis + 1000;
        /*Chờ 8 byte*/
        while (currentTimeMillis < l) if (tcp.Available < 8) yield return Yielders.Get(TIME_SLEEP); else break;
        if (currentTimeMillis >= l) { nStream.Close(); tcp.Close(); yield break; }

        byte[] handshake = new byte[8];
        nStream.Read(handshake, 0, handshake.Length);
        byte vCode = handshake[4];

        byte[] reconnectData = new byte[20];
        reconnectData[0] = (byte)(handshake[0] ^ vCode);
        reconnectData[1] = (byte)(handshake[1] ^ vCode);
        reconnectData[2] = (byte)(handshake[2] ^ vCode);
        reconnectData[3] = (byte)(handshake[3] ^ vCode);
        reconnectData[4] = (byte)(handshake[5] ^ vCode);
        reconnectData[5] = (byte)(handshake[6] ^ vCode);
        reconnectData[6] = (byte)(handshake[7] ^ vCode);

        short ssidTemp = sessionId;
        reconnectData[7] = (byte)((int)((uint)ssidTemp >> 8) & 0xFF);
        reconnectData[8] = (byte)((int)((uint)ssidTemp >> 0) & 0xFF);

        reconnectData[9] = validateData[0];
        reconnectData[10] = validateData[1];
        reconnectData[11] = validateData[2];
        reconnectData[12] = validateData[3];
        reconnectData[13] = validateData[4];
        reconnectData[14] = validateData[5];
        reconnectData[15] = validateData[6];

        byte[] dataSV = dataServer;
        reconnectData[16] = dataSV[0];
        reconnectData[17] = dataSV[1];
        reconnectData[18] = dataSV[2];
        reconnectData[19] = dataSV[3];

        while (currentTimeMillis < l) {
            try { nStream.Write(reconnectData, 0, 20); break; } catch (Exception e) { }
            yield return Yielders.Get(TIME_SLEEP);
        }
        if (currentTimeMillis >= l) { nStream.Close(); tcp.Close(); yield break; }

        /*Chờ 1 byte*/
        while (currentTimeMillis < l) if (tcp.Available < 1) yield return Yielders.Get(TIME_SLEEP); else break;
        if (currentTimeMillis >= l) { nStream.Close(); tcp.Close(); yield break; }

        while (currentTimeMillis < l) {
            try { nStream.Read(reconnectData, 0, 1); break; } catch (Exception e) { }
            yield return Yielders.Get(TIME_SLEEP);
        }

        if (reconnectData[0] == 1) {
            if (onReconnect != null) onReconnect();
            TcpClient tcpClose = tcpClient;
            NetworkStream nsClose = networkStream;

            tcpClient = tcp;
            networkStream = nStream;

            tcpClose.Close();
            nsClose.Close();

            nextTimePing = currentTimeMillis + TIME_PING;
            timeStop = currentTimeMillis + TIME_OUT;
            Debug.Log("Kết nối lại thành công");
        } else {
            nStream.Close(); tcp.Close();
            Debug.Log("Kết nối lại thất bại");
            closeConnection();
        }
        yield break;
    }

    private bool isProcessReceive;
    private List<byteArrayTemp> listWait;
    public float timeDelayListWait;
    public void ResumeReceiveMessage(float timeSleepPerMessageWait = 0) { isProcessReceive = true; timeDelayListWait = timeSleepPerMessageWait; }
    public void PauseReceiveMessage() { if (listWait == null) listWait = new List<byteArrayTemp>(); isProcessReceive = false; }
    public class byteArrayTemp { public byte[] data; public byteArrayTemp(byte[] datas) { data = datas; } }

    private long currentTimeMillis { get { return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds; } }
}
