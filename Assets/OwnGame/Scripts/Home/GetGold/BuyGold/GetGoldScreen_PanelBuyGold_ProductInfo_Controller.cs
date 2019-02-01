using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;

public class GetGoldScreen_PanelBuyGold_ProductInfo_Controller : MySimplePoolObjectController {

	[SerializeField] Text txtProductTitle;
	[SerializeField] Text txtCurrentQuantity;
	[SerializeField] Text txtOldQuantity;
	[SerializeField] Transform panelContainTxtPercentBonus;
	[SerializeField] Text txtPercentBonus;
	[SerializeField] Transform panelContainTxtTimeRemain;
	[SerializeField] Text txtTimeRemain;
	[SerializeField] Text txtPrice;
	[SerializeField] Transform panelShadow;
	System.Action<string> onBuyClicked;

	bool available;

	IAPProductDetail productDetail;
	System.DateTime timeCanPressBuyGold;
	IEnumerator actionCountDown;

	private void Awake() {
		ResetData();
	}

	public override void ResetData(){
		onBuyClicked = null;
		txtProductTitle.text = "";
		txtCurrentQuantity.text = "0";
		txtPrice.text = "Price: ";
		productDetail = null;
		available = false;
		panelShadow.gameObject.SetActive(true);

		if(actionCountDown != null){
			StopCoroutine(actionCountDown);
			actionCountDown = null;
		}
	}

	public void InitData(Product _product, IAPProductDetail _IAPProductDetail, System.Action<string> _onBuyClicked){
		productDetail = _IAPProductDetail;

		if(_product != null){
			if(string.IsNullOrEmpty(_product.metadata.localizedPriceString)){
				txtPrice.text = "Price: ";
				available = false;
			}else{
				txtPrice.text = _product.metadata.localizedPriceString;
				available = true;
			}
		}else{
			txtPrice.text = "Price: ";
			available = false;
		}
		txtProductTitle.text = productDetail.discount_title;

		if(available){
			panelShadow.gameObject.SetActive(false);
		}else{
			panelShadow.gameObject.SetActive(true);
		}

		RefreshData();
		onBuyClicked = _onBuyClicked;

		timeCanPressBuyGold = System.DateTime.Now;
	}

	void RefreshData(){
		if(System.DateTime.Now < productDetail.discount_time_finish){ // còn event
			txtCurrentQuantity.text = "+" + MyConstant.GetMoneyString(productDetail.discount_gold, 999999);
			
			txtOldQuantity.gameObject.SetActive(true);
			txtOldQuantity.text = "+" + MyConstant.GetMoneyString(productDetail.goldBuy, 999999);

			panelContainTxtPercentBonus.gameObject.SetActive(true);
			int _percentBonus = Mathf.CeilToInt((productDetail.discount_gold - productDetail.goldBuy) / productDetail.goldBuy) * 100;
			if(_percentBonus < 0){
				_percentBonus = 0;
			}
			txtPercentBonus.text = "+" + _percentBonus + "%";

			panelContainTxtTimeRemain.gameObject.SetActive(true);
			txtTimeRemain.text = "00:00:00";

			actionCountDown = DoActionCountDown();
			StartCoroutine(actionCountDown);
		}else{
			txtCurrentQuantity.text = "+" + MyConstant.GetMoneyString(productDetail.goldBuy, 999999);
			txtOldQuantity.gameObject.SetActive(false);
			panelContainTxtPercentBonus.gameObject.SetActive(false);
			panelContainTxtTimeRemain.gameObject.SetActive(false);
		}
	}

	IEnumerator DoActionCountDown(){
		System.TimeSpan _tmpTime;
		while(productDetail.discount_time_finish > System.DateTime.Now){
			_tmpTime = productDetail.discount_time_finish - System.DateTime.Now;
			txtTimeRemain.text = string.Format("{0}d " +"{1:00}:{2:00}:{3:00}", _tmpTime.Days , _tmpTime.Hours, _tmpTime.Minutes, _tmpTime.Seconds);
			yield return Yielders.Get(1f);
		}
		actionCountDown = null;
		RefreshData();
	}

	#region On Button Clicked
	public void OnButtonBuyClicked(){
		if(!GetGoldScreenController.instance.canTouch){
			return;
		}
		if(timeCanPressBuyGold > System.DateTime.Now){
			return;
		}
		timeCanPressBuyGold = System.DateTime.Now.AddSeconds(1f);

		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);

		if(available){
			if(onBuyClicked != null){
				onBuyClicked(productDetail.productId);
			}
		}else{
			PopupManager.Instance.CreateToast (MyLocalize.GetString("Global/CommingSoon"));
		}
	}
	#endregion
}
