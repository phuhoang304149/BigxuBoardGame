using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;
using Coffee.UIExtensions;

public class GetGoldScreen_PanelDailyLogin_OptionRewardInfo_Controller : MySimplePoolObjectController {
    public enum State{
        CanNotClaim,
        CanClaim,
        HadClaimed,
    }
    State currentState;

    [SerializeField] Image imgBg;
	public Text txtDay;
    public Image imgReward;
    [SerializeField] UIShiny imgRewardShiny;
    public Text txtQuantity;
    public GameObject panelLock;

    [Header("Setting")]
    [SerializeField] Color bgColorNormal;
    [SerializeField] Color bgColorFocusing;

    System.DateTime timeCanPress;
    RewardDetail rewardDetail;
    System.Action onRecieveReward;

    public override void ResetData(){
        currentState = State.CanNotClaim;
        panelLock.SetActive(false);
        SetUnFocus();
        onRecieveReward = null;
    }

    public void InitData(int _indexDay, RewardDetail _rewardDetail, System.Action _onRecieveReward){
        currentState = State.CanNotClaim;
        if(_indexDay < DataManager.instance.dailyRewardData.listRewards.Count){
            txtDay.text = MyLocalize.GetString("Global/Day") + " " + (_indexDay + 1);
        }else{
            txtDay.text = MyLocalize.GetString("Global/Day") + " " + (DataManager.instance.dailyRewardData.listRewards.Count) + "+";
        }
        
        rewardDetail = _rewardDetail;
        txtQuantity.text = "+" + MyConstant.GetMoneyString(rewardDetail.quantity, 9999);
        imgReward.sprite = _rewardDetail.itemInfo.icon;
        onRecieveReward = _onRecieveReward;

        timeCanPress = System.DateTime.Now;
    }

    public void RefreshData(State _state){
        currentState = _state;
        switch(currentState){
        case State.CanNotClaim:
        case State.CanClaim:
            panelLock.SetActive(false);
            break;
        case State.HadClaimed:
            panelLock.SetActive(true);
            break;
        }
    }

    public void SetFocus(){
        imgBg.color = bgColorFocusing;
        imgRewardShiny.Play();
    }
    
    public void SetUnFocus(){
        imgBg.color = bgColorNormal;
        imgRewardShiny.Stop();
        imgRewardShiny.effectFactor = 0f;
    }

	#region On Button Clicked
    public void OnButtonClaimClicked(){
        if(timeCanPress > System.DateTime.Now){
			return;
		}
        timeCanPress = System.DateTime.Now.AddSeconds(1f);

        MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);

        if(currentState == State.HadClaimed){
            return;
        }
        //TODO: làm effect nhận thưởng
        if(onRecieveReward != null){
            onRecieveReward();
        }
    }
    #endregion
}
