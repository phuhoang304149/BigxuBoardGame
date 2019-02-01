using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetGoldScreen_PanelBuyGold_PanelHistory_OptionInfo_Controller : MySimplePoolObjectController {

	[SerializeField] Text txtTransactionID;
	[SerializeField] Text txtTimePurchase;
	[SerializeField] Button btnReSend;
	[SerializeField] Transform panelSendSuccess;
	PurchaseReceiptDetail purchaseReceiptDetail;
	public System.DateTime timeCanPressReSend;

	public void InitData(PurchaseReceiptDetail _purchaseReceiptDetail){
		purchaseReceiptDetail = _purchaseReceiptDetail;
		
		txtTransactionID.text = purchaseReceiptDetail.transactionId;
		txtTimePurchase.text = string.Format("{0:00}/{1:00}/{2} - {3:00}:{4:00}", purchaseReceiptDetail.purchaseTime.Day, purchaseReceiptDetail.purchaseTime.Month, purchaseReceiptDetail.purchaseTime.Year, purchaseReceiptDetail.purchaseTime.Hour, purchaseReceiptDetail.purchaseTime.Minute);
		if(purchaseReceiptDetail.isDone){
			btnReSend.gameObject.SetActive(false);
			panelSendSuccess.gameObject.SetActive(true);
		}else{
			btnReSend.gameObject.SetActive(true);
			panelSendSuccess.gameObject.SetActive(false);
		}

		timeCanPressReSend = System.DateTime.Now;
	}

	public void OnBtnReSendClicked(){
		if(timeCanPressReSend > System.DateTime.Now){
			return;
		}
		timeCanPressReSend = System.DateTime.Now.AddSeconds(0.5f);
		
		SubServerDetail _serverDetail = GetGoldScreenController.instance.GetServerDetail();
		LoadingCanvasController.instance.Show(-1, true);
		purchaseReceiptDetail.SendMessageToServer(_serverDetail,
		(_listRewarDetails)=>{
			PopupManager.Instance.CreatePopupReward(_listRewarDetails);
			GetGoldScreenController.instance.RefreshMyGoldInfo(false);

			if(HomeManager.instance != null && HomeManager.instance.myCallbackManager != null
				&& HomeManager.instance.myCallbackManager.onLoadDataGoldGemFinished != null){
				HomeManager.instance.myCallbackManager.onLoadDataGoldGemFinished();
			}
		},
		()=>{
			LoadingCanvasController.instance.Hide();
		});
	}
}
