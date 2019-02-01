using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable] public class InstallAppData {
    public List<InstallAppDetail> listAppDone;
    public List<InstallAppDetail> listCurrentAppDetail;
    public DateTime nextTimeToGetDataFromSever;

	public bool isInitialized;

    public InstallAppData(){}

    public void InitData(){
        listAppDone = new List<InstallAppDetail>();
        listCurrentAppDetail = new List<InstallAppDetail>();
        nextTimeToGetDataFromSever = DateTime.Now;
        isInitialized = true;
    }

    public void CheckWhenLogin(){
		if(listAppDone == null){
			listAppDone = new List<InstallAppDetail>();
		}
        if(listCurrentAppDetail == null){
            listCurrentAppDetail = new List<InstallAppDetail>();
        }
        if(nextTimeToGetDataFromSever == DateTime.MinValue){
            nextTimeToGetDataFromSever = DateTime.Now;
        }
	}
}

[System.Serializable] public class InstallAppDetail {
	public enum State{
		None,
		Checking,
		Done
	}
	public State currentState;
    public string packageName;
    public bool isRating;
	public string textTitle;
	public string textDescription;
    public string textkeySearch;
    public string linkIcon;
    public long timeKeep;
    public string linkReport;
	public DateTime timeToGetReward;
    public RewardDetail reward;
    public MessageReceiving messRecieveReward{
		get{
			return _messRecieveReward;
		}set{
			_messRecieveReward = value;
		}
	}
	[NonSerialized] private MessageReceiving _messRecieveReward;

    public Texture2D myIcon{ 
		get{
			return _myIcon;
		}set{
			_myIcon = value;
		}
	}
	[NonSerialized] private Texture2D _myIcon;

    public InstallAppDetail myOriginalDetail{
        get{
            return _myOriginalDetail;
        }set{
            _myOriginalDetail = value;
        }
    }
    [NonSerialized] private InstallAppDetail _myOriginalDetail;

	public InstallAppDetail(MessageReceiving _mess){
        int _id = _mess.readInt();
        packageName = _mess.readString();
        textkeySearch = _mess.readString();
        int count_install = _mess.readInt();
        int sum_install = _mess.readInt();
        int money_pay_to_user = _mess.readInt();
        isRating = _mess.readBoolean();
        textTitle = _mess.readString();
        textDescription = _mess.readString();
        linkIcon = _mess.readString();
        timeKeep = _mess.readLong();
        linkReport = _mess.readString();

        currentState = State.None;
        timeToGetReward = DateTime.MinValue;
        reward = new RewardDetail(IItemInfo.ItemType.Gold, money_pay_to_user);
        messRecieveReward = null;
	}
    public InstallAppDetail ShallowCopy()
    {
        InstallAppDetail other = (InstallAppDetail) this.MemberwiseClone();
        other.myOriginalDetail = this;
        return other;
    }

    public bool IsEqual(InstallAppDetail _other){
        if(packageName.Equals(_other.packageName)){
            return true;
        }
        return false;
    }

    public bool CanRecieveReward(){
		if(currentState == State.Checking && DateTime.Now >= timeToGetReward){
			return true;
		}
		return false;
	}

    public void RecieveReward(System.Action _onRecieveRewardSuccessfully, System.Action _onClearData){
        LoadingCanvasController.instance.Show(1f);
        OneHitAPI.Forward_Bonus_AndroidInstall_Commit(packageName, (_messageReceiving, _error)=>{
			LoadingCanvasController.instance.Hide();
			if(_messageReceiving != null){
                sbyte _caseCheck = _messageReceiving.readByte(); // (nếu 1 thì đọc tiếp)
                bool _tmpFlag = false;
                if(_caseCheck == 1){
                    int _goldAdd = _messageReceiving.readInt();
                    long _goldResult = _messageReceiving.readLong();

                    DataManager.instance.userData.gold = _goldResult;
                    
                    RewardDetail _reward = new RewardDetail(IItemInfo.ItemType.Gold, _goldAdd);
                    PopupManager.Instance.CreatePopupReward(_reward);

                    if(_onRecieveRewardSuccessfully != null){
                        _onRecieveRewardSuccessfully();
                    }

                    _tmpFlag = true;
                }else{
                    switch(_caseCheck){
                    case -1: // nhiệm vụ không còn tồn tại (clear data)
                        PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kWarning)
                            , MyLocalize.GetString("Error/InstallApp_Error_1")
                            , _caseCheck.ToString()
                            , MyLocalize.GetString(MyLocalize.kOk));
                         _tmpFlag = true;
                        break;
                    case -2: // lỗi không load được database tracking cài đặt
                        PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kWarning)
                            , MyLocalize.GetString("Error/InstallApp_Error_2")
                            , _caseCheck.ToString()
                            , MyLocalize.GetString(MyLocalize.kOk));
                        break;
                    case -3: // đã nhận thưởng rồi (clear data)
                        PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kWarning)
                            , MyLocalize.GetString("Error/InstallApp_Error_3")
                            , _caseCheck.ToString()
                            , MyLocalize.GetString(MyLocalize.kOk));
                         _tmpFlag = true;
                        break;
                    case -4: // nhiệm vụ hết hạn (clear data)
                        PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kWarning)
                            , MyLocalize.GetString("Error/InstallApp_Error_4")
                            , _caseCheck.ToString()
                            , MyLocalize.GetString(MyLocalize.kOk));
                         _tmpFlag = true;
                        break;
                    case -5: // không lấy được thông tin người chơi (user này chưa đăng ký)
                        PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kWarning)
                            , MyLocalize.GetString("Error/InstallApp_Error_5")
                            , _caseCheck.ToString()
                            , MyLocalize.GetString(MyLocalize.kOk));
                        break;
                    default:
                        #if TEST
                        Debug.LogError("Forward_Bonus_AndroidInstall_Commit other error from sever: " + _caseCheck);
                        #endif
                        break;
                    }
                }
                if(_tmpFlag){
                    currentState = State.Done;
                    if(myOriginalDetail != null){
                        myOriginalDetail.currentState = State.Done;
                    }
                    if(_onClearData != null){
                        _onClearData();
                    }
                }
			}else{
				#if TEST
				Debug.LogError("RecieveReward InstallApp Reward error: " + _error);
				#endif
			}
		});
    }

    public void StartChecking(){
        if(currentState != State.None){
            return;
        }
        currentState = State.Checking;
        timeToGetReward = DateTime.Now.AddMilliseconds(timeKeep);
    }
}