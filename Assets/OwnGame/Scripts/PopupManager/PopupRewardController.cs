using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupRewardController : IPopupController {

	[SerializeField] Text textTitle;
	[SerializeField] Text textRewardQuantity;
	[SerializeField] Image imgReward;
	[SerializeField] Text textSubmitButton;
	RewardDetail rewardDetail;

	public void Init(RewardDetail _rewardDetail, System.Action _onClose = null){
		rewardDetail = _rewardDetail;
		imgReward.sprite = rewardDetail.itemInfo.icon;
		textRewardQuantity.text = "+" + MyConstant.GetMoneyString(_rewardDetail.quantity, 999999);
		textSubmitButton.text = MyLocalize.GetString("Global/Claim");
		textTitle.text = MyLocalize.GetString("Global/Congratulations");
		onClose = _onClose;
		CoreGameManager.instance.RegisterNewCallbackPressBackKey (Close);

		Show();
	}

	public override void Close (){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
		
		Hide(()=>{
			if(onClose != null){
				onClose.Invoke();
				onClose = null;
			}
			CoreGameManager.instance.RemoveCurrentCallbackPressBackKey (Close);
			SelfDestruction();
		});
	}
}
