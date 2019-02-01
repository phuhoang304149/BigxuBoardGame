using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class DailyRewardData {

	public List<RewardDetail> listRewards;

	public sbyte lastDayLogin;
	public sbyte currentDayLogin;
	public long timeCountDownToReceiveReward;
	public long timeCountDownToReset;
	public long lastTimeRecieved;
	public long nextTimeRecieved;
	public DateTime timeToGetReward;
	public DateTime timeToReset;

	public bool hadCheckSv{
		get{
			return _hadCheckSv;
		}set{
			_hadCheckSv = value;
		}
	}
	private bool _hadCheckSv;

	public bool isInitialized;

	public DailyRewardData(){}

	public void InitData(){
		listRewards = new List<RewardDetail>();
		lastDayLogin = -1;
		currentDayLogin = 0;
		timeCountDownToReceiveReward = 0;
		timeCountDownToReset = 0;
		lastTimeRecieved = 0;
		nextTimeRecieved = 0;
		timeToGetReward = DateTime.Now;
		timeToReset = DateTime.Now;
		isInitialized = true;
	}

	public void CheckWhenLogin(){
		if(listRewards == null){
			listRewards = new List<RewardDetail>();
		}
		if(timeToGetReward == DateTime.MinValue){
			timeToGetReward = DateTime.Now;
		}
		if(timeToReset == DateTime.MinValue){
			timeToReset = DateTime.Now;
		}
	}

	public void ResetLoginData(){
		lastDayLogin = -1;
		currentDayLogin = 0;
		timeCountDownToReceiveReward = 0;
		timeCountDownToReset = 0;
		lastTimeRecieved = 0;
		nextTimeRecieved = 0;
		timeToGetReward = DateTime.Now;
		timeToReset = DateTime.Now;
	}

	public void RecieveReward(MessageReceiving _messageReceiving, System.Action<sbyte> _onRecieveCaseCheck = null){
		string _debug = string.Empty;
		sbyte _caseCheck = _messageReceiving.readByte ();
		_debug += _caseCheck + "|";
		switch(_caseCheck){
		case -1: // tài khoản không tồn tại
			#if TEST
			Debug.LogError (">>> Tài khoản không tồn tại");
			#endif
			break;
		case -2: // chưa đến lúc nhận thưởng
			#if TEST
			Debug.LogError (">>> Chưa đến lúc nhận thưởng. Cập nhật lại!");
			#endif
			currentDayLogin = _messageReceiving.readByte();
			lastTimeRecieved = MyConstant.currentTimeMilliseconds;
			timeCountDownToReceiveReward = _messageReceiving.readLong();
			nextTimeRecieved = lastTimeRecieved + timeCountDownToReceiveReward;
			timeToGetReward = DateTime.Now.AddMilliseconds(timeCountDownToReceiveReward);

			timeCountDownToReset = _messageReceiving.readLong();
			timeToReset = DateTime.Now.AddMilliseconds(timeCountDownToReset);
			
			_debug += timeCountDownToReceiveReward + "|" + timeCountDownToReset + "|" + currentDayLogin;
			break;
		case 1: // nhận thưởng thành công
			#if TEST
			Debug.Log (">>> Nhận thưởng thành công");
			#endif
			lastDayLogin = currentDayLogin;
			currentDayLogin = _messageReceiving.readByte();
			long _goldAdd = _messageReceiving.readLong();
			long _goldResult = _messageReceiving.readLong();
			DataManager.instance.userData.gold = _goldResult;
			lastTimeRecieved = MyConstant.currentTimeMilliseconds;
			timeCountDownToReceiveReward = _messageReceiving.readLong();
			nextTimeRecieved = lastTimeRecieved + timeCountDownToReceiveReward;
			timeToGetReward = System.DateTime.Now.AddMilliseconds(timeCountDownToReceiveReward);

			timeCountDownToReset = _messageReceiving.readLong();
			timeToReset = DateTime.Now.AddMilliseconds(timeCountDownToReset);
			
			_debug += timeCountDownToReceiveReward + "|" + timeCountDownToReset + "|" + currentDayLogin + "|" + _goldAdd + "|" + _goldResult;
			
			RewardDetail _rewardDetail = new RewardDetail(IItemInfo.ItemType.Gold, _goldAdd);
			PopupManager.Instance.CreatePopupReward(_rewardDetail);
			break;
		default:
			#if TEST
			Debug.LogError("Không có casecheck này: " + _caseCheck);
			#endif
			break;
		}
		if(_onRecieveCaseCheck != null){
			_onRecieveCaseCheck(_caseCheck);
		}

		#if TEST
		Debug.Log (">>> GetGoldDaily Response : " + _debug);
		#endif
	}

	public bool CanRecieveReward(){
		if(timeToGetReward < DateTime.Now){
			return true;
		}
		return false;
	}

	public bool CanReset(){
		if(currentDayLogin > 0 && timeToReset < DateTime.Now){
			return true;
		}
		return false;
	}

	public DailyRewardData ShallowCopy()
    {
       DailyRewardData other = (DailyRewardData) this.MemberwiseClone();
       return other;
    }
}
