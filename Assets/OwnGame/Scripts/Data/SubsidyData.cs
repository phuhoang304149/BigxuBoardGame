using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable] public class SubsidyData {

	public DateTime timeToGetReward;
	public sbyte countGetSubsidy;
	public bool isInitialized;

	public bool hadCheckSv{
		get{
			return _hadCheckSv;
		}set{
			_hadCheckSv = value;
		}
	}
	private bool _hadCheckSv;

	public SubsidyData(){}

	public void InitData(){
		timeToGetReward = DateTime.Now;
		isInitialized = true;
	}

	public bool CanRecieveReward(){
		if(timeToGetReward < DateTime.Now){
			return true;
		}
		return false;
	}

	public void ResetLoginData(){
		timeToGetReward = DateTime.Now;
	}

	public void CheckWhenLogin(){
		if(timeToGetReward == DateTime.MinValue){
			timeToGetReward = DateTime.Now;
		}
	}

	public void RecieveReward(MessageReceiving _messageReceiving, System.Action<sbyte> _onRecieveCaseCheck = null){
		// byte caseValue
		// Xét trường hợp : caseValue
		// -1 : tài khoảng không tồn tại
		// -2 : chưa đến lúc trợ cấp ⟶ đọc tiếp kiểu long : thời gian countDown nhận trợ cấp 
		// -7 : số gold đang lớn hơn gold add ⟶ đọc tiếp goldAdd kiểu long
		// 1 hoặc 2 : trợ cấp thành công ⟶ đọc tiếp dữ liệu sau
		// 		long gold_add : số gold được cộng
		// 		long gold_result : số gold sau khi cộng
		// 		long timeCountDown : thời gian reset trợ cấp
		// 		byte countSubsidy : số trợ cấp đã nhận trong ngày
		
		string _debug = string.Empty;
		sbyte _caseCheck = _messageReceiving.readByte ();
		long _timeCountDownToReceiveReward = 0;
		long _goldAdd = 0;
		long _GOLD = 0;
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
			_timeCountDownToReceiveReward = _messageReceiving.readLong();
			Debug.LogError(">>>> " + _timeCountDownToReceiveReward);
			if(_timeCountDownToReceiveReward > 86400000){
				#if TEST
				Debug.LogError (">>> Bug tràn số : " + _timeCountDownToReceiveReward);
				#endif
				_timeCountDownToReceiveReward = 86400000;
			}
			timeToGetReward = DateTime.Now.AddMilliseconds(_timeCountDownToReceiveReward);
			_debug += _timeCountDownToReceiveReward + "|" + timeToGetReward;
			break;
		case -7: // số gold đang lớn hơn gold add ⟶ đọc tiếp goldAdd kiểu long
			_goldAdd = _messageReceiving.readLong(); // số gold được cộng
			_GOLD = _messageReceiving.readLong();
			_debug += _goldAdd + "|" + _GOLD;
			PopupManager.Instance.CreatePopupMessage(
					MyLocalize.GetString(MyLocalize.kWarning)
					, string.Format(MyLocalize.GetString("System/Message_Subsidy_UnqualifiedClaim"), _goldAdd)
					, "-7"
					, MyLocalize.GetString(MyLocalize.kOk)
					, null);
			break;
		case 1: // nhận thưởng thành công
		case 2:
			#if TEST
			Debug.Log (">>> Nhận thưởng thành công");
			#endif

			countGetSubsidy = _messageReceiving.readByte();	// số trợ cấp đã nhận trong ngày
			_timeCountDownToReceiveReward = _messageReceiving.readLong(); // thời gian reset trợ cấp
			if(_timeCountDownToReceiveReward > 86400000){
				#if TEST
				Debug.LogError (">>> Bug tràn số : " + _timeCountDownToReceiveReward);
				#endif
				_timeCountDownToReceiveReward = 86400000;
			}
			timeToGetReward = System.DateTime.Now.AddMilliseconds(_timeCountDownToReceiveReward);

			_goldAdd = _messageReceiving.readLong(); // số gold được cộng
			_GOLD = _messageReceiving.readLong(); // số gold sau khi cộng

			DataManager.instance.userData.gold = _GOLD; 
			
			_debug += _timeCountDownToReceiveReward + "|" + timeToGetReward + "|" + countGetSubsidy + "|" + _goldAdd + "|" + _GOLD;
			
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
		Debug.Log (">>> GetSubsidy Response : " + _debug);
		#endif
	}
}
