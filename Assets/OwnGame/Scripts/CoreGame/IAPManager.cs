using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
using UnityEngine.SceneManagement;

public class IAPManager : MonoBehaviour, IStoreListener {

	public IStoreController m_StoreController;          // The Unity Purchasing system.
	public IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.
	public IAppleExtensions m_AppleExtensions;
	public static event Action onInitSuccess;
	public static event Action onInitFailed;
	public static event Action<string> onPurchaseSuccess;
	public static event Action<PurchaseFailureReason> onPurchaseFailed;
	public static event Action onPurchaseDeferred;// Only in IOS

	bool silentInit;//Default is false
	string purchasingProductId;

	//	public List<string> gunPackages = new List<string> ();
	//	public List<IAPPackInfo> cashPackages = new List<IAPPackInfo> ();

	#region Singleton
	//Support singleton
	public static IAPManager instance{
		get{
			return ins;
		}
	}
	private static IAPManager ins;

	void Awake() 
	{
		if (ins != null && ins != this) { 
			Destroy(this.gameObject); 
			return;
		}
		ins = this;
		DontDestroyOnLoad (this.gameObject);
	}
	#endregion

	/// <param name="_silentInit">If set to <c>false</c> LoadingCanvas will be showed and an Dialog will be display if failed.</param>
	public void InitializePurchasing (bool _silentInit) 
	{
		if (_silentInit && IsInitialized()){
			return;
		}
		silentInit = _silentInit;
		Invoke ("OnInitializeTimeOut", 15f);
		var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

		for(int i = 0; i < DataManager.instance.IAPProductData.listProductDetail.Count; i++){
			builder.AddProduct(DataManager.instance.IAPProductData.listProductDetail[i].productId, ProductType.Consumable);
		}

		if(!_silentInit) LoadingCanvasController.instance.Show(-1f, true);
		UnityPurchasing.Initialize(this, builder);
	}

	public bool IsInitialized()
	{
		// Only say we are initialized if both the Purchasing references are set.
		return m_StoreController != null && m_StoreExtensionProvider != null;
	}

	public void InitiatePurchase (string _productId, System.Action _onInitiatePurchaseSuccess = null) {
		purchasingProductId = _productId;
		if (!IsInitialized ()) {
			InitializePurchasing (false);
			return;
		}

		if (m_StoreController.products.WithID (_productId) == null) {
			PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
				, MyLocalize.GetString("Error/IAP_CanNotFindPackage")
				, string.Empty
				, MyLocalize.GetString(MyLocalize.kOk));
			return;
		}

		m_StoreController.InitiatePurchase (_productId);

		if(_onInitiatePurchaseSuccess != null){
			_onInitiatePurchaseSuccess();
		}
	}

	public void RestoreTransactions ()
	{
		LoadingCanvasController.instance.Show (-1f, true);
		m_AppleExtensions.RestoreTransactions( (bool success) => {
			LoadingCanvasController.instance.Hide ();
			if (!success) {
				PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
					, MyLocalize.GetString("Error/IAP_FailedToRestore")
					, string.Empty
					, MyLocalize.GetString(MyLocalize.kOk));
			}
		});
	}

	public Product GetProductInfo(string _productId){
		if (IsInitialized ()) {
			Product _product = m_StoreController.products.WithID (_productId);
			return _product;
		}else {
			return null;
		}
	}

	#region IStoreListener
	public void OnInitializeTimeOut ()
	{
		OnInitializeFailed (InitializationFailureReason.AppNotKnown);
	}

	public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		Debug.Log ("IAP - Initialized");
		CancelInvoke ("OnInitializeTimeOut");
		m_StoreController = controller;
		m_StoreExtensionProvider = extensions;
		m_AppleExtensions = extensions.GetExtension<IAppleExtensions> ();
		m_AppleExtensions.RegisterPurchaseDeferredListener(OnDeferred);
		
		if(!silentInit) LoadingCanvasController.instance.Hide ();
		silentInit = false;
		if (!string.IsNullOrEmpty(purchasingProductId)) {
			InitiatePurchase (purchasingProductId, ()=>{
				purchasingProductId = string.Empty;
			});
		}

		if (onInitSuccess != null)
			onInitSuccess ();
	}

	public void OnInitializeFailed(InitializationFailureReason error)
	{
		CancelInvoke ("OnInitializeTimeOut");
		// Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
		Debug.Log("IAP - InitializeFailed InitializationFailureReason:" + error);
		if (!silentInit) {
			LoadingCanvasController.instance.Hide ();
			PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
				, MyLocalize.GetString("Error/IAP_CanNotConnectToAppStore")
				, string.Empty
				, MyLocalize.GetString(MyLocalize.kOk));
		}
		silentInit = false;

		if (onInitFailed != null)
			onInitFailed ();
	}

	private void OnDeferred(Product item)
	{
		Debug.Log("IAP - Purchase deferred: " + item.definition.id);
		LoadingCanvasController.instance.Hide ();
	}

	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args) 
	{
		Debug.Log("Purchase OK: " + args.purchasedProduct.definition.id);
        Debug.Log("Receipt: " + args.purchasedProduct.receipt);
		// var unifiedReceipt = JsonUtility.FromJson<UnifiedReceipt>(args.purchasedProduct.receipt);
		// if (unifiedReceipt != null && !string.IsNullOrEmpty(unifiedReceipt.Payload))
		// {
		// 	var purchaseReceipt = JsonUtility.FromJson<UnityChannelPurchaseReceipt>(unifiedReceipt.Payload);
		// 	Debug.LogFormat(
		// 		"UnityChannel receipt: storeSpecificId = {0}, transactionId = {1}, orderQueryToken = {2}",
		// 		unifiedReceipt.Store, unifiedReceipt.TransactionID, purchaseReceipt.orderQueryToken);
		// }

		//Cheat for Editor
		#if UNITY_EDITOR
		purchasingProductId = string.Empty;
		SubServerDetail _serverDetail = GetGoldScreenController.instance.GetServerDetail();
		LoadingCanvasController.instance.Show(-1f, true);
		OneHitAPI.TESTIAP(_serverDetail, (_messageReceiving, _error)=>{
			LoadingCanvasController.instance.Hide();
			if(_messageReceiving != null){
				bool _isSuccess = _messageReceiving.readBoolean();
				if(_isSuccess){
					DataManager.instance.userData.gold = _messageReceiving.readLong();
					GetGoldScreenController.instance.RefreshMyGoldInfo(false);
				}else{
					#if TEST
					Debug.LogError("TESTIAP false");
					#endif
				}
			}else{
				#if TEST
				Debug.LogError("Error code: " + _error);
				#endif
			}
		});
		// ApplyIAPPackage (args.purchasedProduct.definition.id);
		return PurchaseProcessingResult.Complete;
		#endif

		#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
		CrossPlatformValidator validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
		string receipt = args.purchasedProduct.receipt;
		try {
			// On Google Play, result has a single product ID.
			// On Apple stores, receipts contain multiple products.
			var result = validator.Validate(receipt);
			foreach (IPurchaseReceipt productReceipt in result) {
				ApplyIAPPackage (productReceipt.productID, productReceipt);
			}
				
		} catch (IAPSecurityException e) {
			PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
				, MyLocalize.GetString("Error/IAP_FailedToVerifyReceipt")
				, string.Empty
				, MyLocalize.GetString(MyLocalize.kOk));
			if (onPurchaseFailed != null) onPurchaseFailed (PurchaseFailureReason.SignatureInvalid);
		}
		#endif

		purchasingProductId = string.Empty;
		return PurchaseProcessingResult.Complete;
	}

	void ApplyIAPPackage (string _productId, IPurchaseReceipt _productReceipt = null) {
		Debug.Log(">>> ApplyIAPPackage " + _productId);
		if(_productReceipt == null){
			Debug.LogError(">>> _productReceipt is null");
			return;
		}
		string _tokenPurchase = string.Empty;
		GooglePlayReceipt _google = _productReceipt as GooglePlayReceipt;
		if (_google !=null) {
			_tokenPurchase = _google.purchaseToken;
			// Debug.Log(google.purchaseState);
			// Debug.Log(google.purchaseToken);
		}
		AppleInAppPurchaseReceipt _apple = _productReceipt as AppleInAppPurchaseReceipt;
		if (_apple != null) {
			// Debug.Log(_apple.originalTransactionIdentifier);
			// Debug.Log(_apple.subscriptionExpirationDate);
			// Debug.Log(_apple.cancellationDate);
			// Debug.Log(_apple.quantity);
			_tokenPurchase = _google.purchaseToken;
		}

		if(!string.IsNullOrEmpty(_tokenPurchase)){
			byte _screenPurchase = (byte) IMySceneManager.Type.Home;
			if(CoreGameManager.instance.currentSceneManager != null){
				_screenPurchase = (byte) CoreGameManager.instance.currentSceneManager.mySceneType;
			}
			
			PurchaseReceiptDetail _purchaseReceiptDetail = new PurchaseReceiptDetail(_screenPurchase, _productReceipt.transactionID, _productId, _tokenPurchase, _productReceipt.purchaseDate);
			DataManager.instance.purchaseReceiptData.AddNewPurchaseReceiptDetail(_purchaseReceiptDetail);
			
			SubServerDetail _serverDetail = GetGoldScreenController.instance.GetServerDetail();
			LoadingCanvasController.instance.Show(-1f, true);
			_purchaseReceiptDetail.SendMessageToServer(_serverDetail,
				(_listRewarDetails)=>{
					PopupManager.Instance.CreatePopupReward(_listRewarDetails);
					GetGoldScreenController.instance.RefreshMyGoldInfo(false);
					// StartCoroutine(CreatePopUpRewards(_listRewarDetails, null));
					if(GetGoldScreenController.instance.currentState == UIHomeScreenController.State.Show
						&& GetGoldScreenController.instance.currentTab == GetGoldScreenController.Tab.BuyGold){
						((GetGoldScreen_PanelBuyGold_Controller) GetGoldScreenController.instance.currentPanel).SetActiveIconWarningHasNewPurchase(true);
					}

					if(HomeManager.instance != null && HomeManager.instance.myCallbackManager != null
						&& HomeManager.instance.myCallbackManager.onLoadDataGoldGemFinished != null){
						HomeManager.instance.myCallbackManager.onLoadDataGoldGemFinished();
					}
				},
				()=>{
					LoadingCanvasController.instance.Hide();
				});
		}else{
			Debug.LogError(">>> _tokenPurchase is null");
		}
	}

	public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
	{
		Debug.Log("Purchase failed: " + product.definition.id);
        Debug.Log(failureReason);
		if (onPurchaseFailed != null) onPurchaseFailed (failureReason);
	}
	#endregion

	// IEnumerator CreatePopUpRewards (List<RewardDetail> _rewardDetails, System.Action _onFinished) {

		//TODO: Create Popup reward
		// if (diamond > 0) {
		// 	yield return PopupManager.Instance.CreatePopupReward (new StackableItemData (ItemInfo.ItemID.diamond, diamond));
		// }
		// if (gold > 0) {
		// 	yield return PopupManager.Instance.CreatePopupReward (new StackableItemData (ItemInfo.ItemID.gold, gold));
		// }
	// 	yield return null;
	// 	if (_onFinished != null) {
	// 		_onFinished ();
	// 	}
	// }

	public Coroutine CheckWhenLogin(){
		int _countReceiptError = 0;
		for(int i = 0; i < DataManager.instance.purchaseReceiptData.listPurchaseReceiptDetail.Count; i++){
			if(!DataManager.instance.purchaseReceiptData.listPurchaseReceiptDetail[i].isDone){
				_countReceiptError++;
			}
		}
		#if TEST
		Debug.Log(">>> Còn tồn đọng " + _countReceiptError + " hóa đơn.");
		#endif
		return null;
		// return StartCoroutine(DoActionCheckWhenLogin());
	}

	public IEnumerator DoActionCheckWhenLogin(){
		if(DataManager.instance.purchaseReceiptData.listPurchaseReceiptDetail == null
			|| DataManager.instance.purchaseReceiptData.listPurchaseReceiptDetail.Count == 0){
			yield break;
		}
		#if TEST
		Debug.Log(">>> Còn tồn đọng " + DataManager.instance.purchaseReceiptData.listPurchaseReceiptDetail.Count + " hóa đơn.");
		#endif

		float _timeDelay = 0f;
		for(int i = 0; i < DataManager.instance.purchaseReceiptData.listPurchaseReceiptDetail.Count; i++){
			if(DataManager.instance.purchaseReceiptData.listPurchaseReceiptDetail[i].isDone){
				DataManager.instance.purchaseReceiptData.listPurchaseReceiptDetail.RemoveAt(i);
				i--;
				continue;
			}
			StartCoroutine(WaitAndSendPurchareToSeverWhenLogin(DataManager.instance.purchaseReceiptData.listPurchaseReceiptDetail[i], _timeDelay));
			_timeDelay ++;
		}
	}

	IEnumerator WaitAndSendPurchareToSeverWhenLogin(PurchaseReceiptDetail _purchaseReceiptDetail, float _timeDelay){
		yield return Yielders.Get(_timeDelay);
		_purchaseReceiptDetail.SendMessageToServer(null, null, null, true);
	}
}
