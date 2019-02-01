using System;
using System.IO;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;

public class NetworkGlobal : MonoBehaviour
{
	private static NetworkGlobal ins;
	public static NetworkGlobal instance { get { return ins; } }
   
	void Awake (){
        if (ins != null && ins != this) { 
			Destroy(this.gameObject); 
			return;
		}
		ins = this;
		DontDestroyOnLoad (this.gameObject);
		listProcess = null;
		instanceRealTime = null;
    }

    #region RealTime
    /// <summary>
    /// Phần này thuộc real-time
    /// </summary>

    public IActionProcessMessage[] listProcess;
	public void SetProcessRealTime (short cmd, Action<MessageReceiving> onReceiveMessage){
		if (listProcess == null) {
			listProcess = new IActionProcessMessage[1];
			listProcess [0] = new IActionProcessMessage();
			listProcess [0].cmd = cmd;
			listProcess [0].functionProcess = onReceiveMessage;
		} else {
			int numberProcess = listProcess.Length;
			for (int i = 0; i < numberProcess; i++)
				if (listProcess [i].cmd == cmd) {
					listProcess [i].functionProcess = onReceiveMessage;
					return;
				}

            IActionProcessMessage[] list = new IActionProcessMessage[numberProcess + 1];
			for (int i = 0; i < numberProcess; i++)
				list [i] = listProcess [i];

			list [numberProcess] = new IActionProcessMessage();
			list [numberProcess].cmd = cmd;
			list [numberProcess].functionProcess = onReceiveMessage;
            listProcess = list;

        }
	}

    public RealTimeGame instanceRealTime;

    public void RunRealTime (SubServerDetail _subServerDeail, Action<int> onCreateConnectionError, Action onCreateConnectionSuccess, Action onDisconnect, Action onServerFull){
        if (instanceRealTime != null) {
            instanceRealTime.onDisconnect = null;
            instanceRealTime.closeConnection();
        }

        instanceRealTime = new RealTimeGame(_subServerDeail, instance);
        instanceRealTime.onConnectSuccess = onCreateConnectionSuccess;
        instanceRealTime.onNetworkError = onCreateConnectionError;
        instanceRealTime.onDisconnect = onDisconnect;
        instanceRealTime.onServerFull = onServerFull;
        instanceRealTime.Start();
    }
    
	public void SendMessageRealTime(MessageSending messageRealTime){
		if (messageRealTime == null || instanceRealTime==null)
			return;
        #if TEST
        Debug.Log("Client send "+messageRealTime.getCMDName());
        #endif
        byte[] dataMessage = messageRealTime.getBytesArray();
        int lengDataMessage = dataMessage.Length;
        byte[] dataSending = new byte[lengDataMessage + 4];
        for (int i = 0; i < lengDataMessage; i++)
            dataSending[i + 4] = (byte)(dataMessage[i] ^ instanceRealTime.validateCode);
        dataSending[0] = (byte)(lengDataMessage >> 24);
        dataSending[1] = (byte)(lengDataMessage >> 16);
        dataSending[2] = (byte)(lengDataMessage >> 8);
        dataSending[3] = (byte)lengDataMessage;
        instanceRealTime.sendThread(dataSending);
    }
    
    public void reconnect() {if (instanceRealTime != null)StartCoroutine(instanceRealTime.ieReconnect());}
	// public void CloseRealTime (){if (instanceRealTime != null) {instanceRealTime.onDisconnect = null;instanceRealTime.closeConnection();}}
    public void StopRealTime (){
        if (instanceRealTime != null) {
            instanceRealTime.onDisconnect = null;
            SendMessageRealTime(new MessageSending(-4));
            instanceRealTime.Stop();
        }
    }
    #endregion

    #region One Hit
    public void StartOnehit(MessageSending _messageSending, SubServerDetail _serverDetail, Action<MessageReceiving, int> _onFinished){
        OneHitGame clientOnehit = new OneHitGame(_serverDetail, _messageSending);
        clientOnehit.onNetworkError = (n) => { 
            if(_onFinished != null){
                _onFinished(null,n); 
            }
        };
        if(_onFinished != null){
            clientOnehit.onReceiveMessage = (messageReceiving) => { 
                if(_onFinished != null){
                    _onFinished(messageReceiving,-1); 
                }
            };
        }
        StartCoroutine(clientOnehit.runNetwork());
    }

    public void StartOnehit(MessageSending _messageSending, Action<MessageReceiving, int> _onFinished){
        OneHitGame clientOnehit = new OneHitGame(_messageSending);
        clientOnehit.onNetworkError = (n) => { 
            if(_onFinished != null){
                _onFinished(null,n); 
            }
        };
        if(_onFinished != null){
            clientOnehit.onReceiveMessage = (messageReceiving) => { 
                if(_onFinished != null){
                    _onFinished(messageReceiving,-1); 
                }
            };
        }
        StartCoroutine(clientOnehit.runNetwork());
    }
	#endregion
}