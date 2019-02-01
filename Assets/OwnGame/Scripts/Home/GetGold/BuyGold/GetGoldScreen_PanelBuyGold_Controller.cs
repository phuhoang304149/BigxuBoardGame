using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using Lean.Pool;

public class GetGoldScreen_PanelBuyGold_Controller : MySimplePanelController {

	[SerializeField] CanvasGroup myCanvasGroup;
	[SerializeField] Text txtEmpty;
	[SerializeField] Transform panelLoading;
	[SerializeField] Transform productContent;
	[SerializeField] Transform panelFocusScreen;
	[SerializeField] Transform iconWarningHasNewPurchase;
	[SerializeField] GetGoldScreen_PanelBuyGold_PanelHistory_Controller panelPurchaseHistory;
	
	[Header("Prefab")]
	[SerializeField] GameObject prefabPanelProduct;

	List<GetGoldScreen_PanelBuyGold_ProductInfo_Controller> listPanelProduct;
	IEnumerator actionWaitToActiveButtons;
	public System.DateTime timeCanPressBuyGold;

	public override void ResetData(){
		StopAllCoroutines();

		actionWaitToActiveButtons = null;

		myCanvasGroup.alpha = 0f;
		myCanvasGroup.blocksRaycasts = false;

		panelLoading.gameObject.SetActive(false);
		txtEmpty.gameObject.SetActive(false);
		panelFocusScreen.gameObject.SetActive(false);
		SetActiveIconWarningHasNewPurchase(false);

		panelPurchaseHistory.ResetData();

		if(listPanelProduct != null && listPanelProduct.Count > 0){
			for(int i = 0; i < listPanelProduct.Count; i ++){
				listPanelProduct[i].SelfDestruction();
			}
			listPanelProduct.Clear();
		}
	}

	public override void InitData (System.Action _onFinished = null){
		if (!IAPManager.instance.IsInitialized ()) {
			IAPManager.instance.InitializePurchasing (false);
		}
		if(listPanelProduct == null){
			listPanelProduct = new List<GetGoldScreen_PanelBuyGold_ProductInfo_Controller>();
		}
		if(DataManager.instance.IAPProductData.listProductDetail.Count == 0){
			txtEmpty.gameObject.SetActive(true);
		}else{
			txtEmpty.gameObject.SetActive(false);
			GetGoldScreen_PanelBuyGold_ProductInfo_Controller _tmpPanel = null;
			for(int i = 0; i < DataManager.instance.IAPProductData.listProductDetail.Count; i++){
				_tmpPanel = LeanPool.Spawn(prefabPanelProduct, Vector3.zero, Quaternion.identity, productContent).GetComponent<GetGoldScreen_PanelBuyGold_ProductInfo_Controller>();
				listPanelProduct.Add(_tmpPanel);
			}
		}
		
		
		RefreshData();
	}

	public override void RefreshData (){
		bool _flag = false;
		for(int i = 0; i < DataManager.instance.purchaseReceiptData.listPurchaseReceiptDetail.Count; i++){
			if(!DataManager.instance.purchaseReceiptData.listPurchaseReceiptDetail[i].isDone){
				_flag = true;
				break;
			}
		}
		SetActiveIconWarningHasNewPurchase(_flag);
	}

	public override Coroutine Show (){
		myCanvasGroup.alpha = 1f;
		myCanvasGroup.blocksRaycasts = true;
		panelFocusScreen.gameObject.SetActive(true);
		timeCanPressBuyGold = System.DateTime.Now;

		if(actionWaitToActiveButtons == null){
			actionWaitToActiveButtons = DoActionWaitToActiveButtons();
			StartCoroutine(actionWaitToActiveButtons);
		}
		return null;
	}

	IEnumerator DoActionWaitToActiveButtons(){
		if(listPanelProduct.Count == 0){
			yield break;
		}
		if(!IAPManager.instance.IsInitialized ()){
			panelLoading.gameObject.SetActive(true);
			yield return new WaitUntil(()=>IAPManager.instance.IsInitialized ());
			panelLoading.gameObject.SetActive(false);
		}

		Product _product = null;
		for(int i = 0; i < DataManager.instance.IAPProductData.listProductDetail.Count; i++){
			_product = IAPManager.instance.GetProductInfo(DataManager.instance.IAPProductData.listProductDetail[i].productId);
			listPanelProduct[i].InitData(_product, DataManager.instance.IAPProductData.listProductDetail[i], OnBuyProduct);
		}

		actionWaitToActiveButtons = null;
	}

	public override Coroutine Hide (){
		ResetData();
		return null;
	}

	public void SetActiveIconWarningHasNewPurchase(bool _flag){
		iconWarningHasNewPurchase.gameObject.SetActive(_flag);
	}

	#region On Button Clicked
	public void OnBuyProduct(string _productId){
		if(timeCanPressBuyGold > System.DateTime.Now){
			return;
		}

		timeCanPressBuyGold = System.DateTime.Now.AddSeconds(0.5f);

		if(string.IsNullOrEmpty(_productId)){
			return;
		}
		IAPManager.instance.InitiatePurchase (_productId);
	}

	public void OnOpenPanelPurchaseHistory(){
		panelPurchaseHistory.InitData();
		panelPurchaseHistory.Show();
	}
	#endregion
}
