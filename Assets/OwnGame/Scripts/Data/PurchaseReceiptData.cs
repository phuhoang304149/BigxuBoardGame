using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
* PurchaseReceiptData: class lưu trữ các hóa đơn
*	- Khi hóa đơn thanh toán thành công thì sẽ giữ lại đến khi login lần sau sẽ xóa
*	- Khi hóa đơn thanh toán không thành công sẽ lưu lại để người chơi bấm thanh toán lại
**/
[System.Serializable] public class PurchaseReceiptData {

	public List<PurchaseReceiptDetail> listPurchaseReceiptDetail;
	public bool isInitialized;
	public PurchaseReceiptData(){}

	public void InitData(){
		listPurchaseReceiptDetail = new List<PurchaseReceiptDetail> ();
		isInitialized = true;
	}

	public void AddNewPurchaseReceiptDetail(PurchaseReceiptDetail _detail){
		if(listPurchaseReceiptDetail == null){
			listPurchaseReceiptDetail = new List<PurchaseReceiptDetail>();
		}
		listPurchaseReceiptDetail.Add (_detail);
	}

	public void CheckWhenLogin(){
		if(listPurchaseReceiptDetail == null){
			listPurchaseReceiptDetail = new List<PurchaseReceiptDetail>();
		}else{
			for(int i = 0; i < listPurchaseReceiptDetail.Count; i++){
				if(listPurchaseReceiptDetail[i].isDone){
					listPurchaseReceiptDetail.RemoveAt(i);
					i--;
					continue;
				}
			}
			#if TEST
			Debug.Log(">>> Còn tồn đọng " + listPurchaseReceiptDetail.Count + " hóa đơn.");
			#endif
		}
	}
}

[System.Serializable] public class PurchaseReceiptDetail{
	public byte screenPurchase;
	public string transactionId;
	public string productId;
	public string tokenPurchase;
	public System.DateTime purchaseTime;
	public bool isDone;
	public PurchaseReceiptDetail(byte _screenPurchase, string _transactionId, string _productId, string _tokenPurchase, System.DateTime _purchaseTime){
		screenPurchase = _screenPurchase;
		transactionId = _transactionId;
		productId = _productId;
		tokenPurchase = _tokenPurchase;
		purchaseTime = _purchaseTime;
		isDone = false;
	}

	public void SendMessageToServer(SubServerDetail _serverDetail, System.Action<List<RewardDetail>> _onSucceed, System.Action _onFinished, bool _sendInSilent = false){
		if(isDone){
			return;
		}
		#if UNITY_ANDROID
		OneHitAPI.IAP_Android(_serverDetail, screenPurchase, productId, tokenPurchase, (_messageReceiving, _error)=>{
			if(_messageReceiving != null){
				sbyte _caseCheck = _messageReceiving.readByte();
				if(_caseCheck == 1 || _caseCheck < -100){
					if(_caseCheck == 1){
						long _goldAdd = _messageReceiving.readLong();
						DataManager.instance.userData.gold = _messageReceiving.readLong();
						if(_onSucceed != null){
							IAPProductDetail _productDetail = DataManager.instance.IAPProductData.GetProductDetail(productId);
							if(_productDetail == null){
								#if TEST
								Debug.LogError(">>> productDetail is null in data: " + productId);
								#endif
							}

							List<RewardDetail> _tmplistRewardDetail = new List<RewardDetail>();
							RewardDetail _rewardDetail = new RewardDetail(IItemInfo.ItemType.Gold, _goldAdd);
							_tmplistRewardDetail.Add(_rewardDetail);

							_onSucceed(_tmplistRewardDetail);
						}
					}else{
						#if TEST
						Debug.LogError(">>> IAP_Android Trường hợp check case = " + _caseCheck);
						#endif
					}
					isDone = true;
				}else{
					// #if TEST
					// Debug.LogError(">>> IAP_Android can not Purchase: " + _caseCheck);
					// #endif
					// private static final byte STATUS_PURCHASE_OK=1;
					// private static final byte STATUS_REFUND=-122;
					// private static final byte STATUS_ERROR_PURCHASE_PROCESSED_DaXuLyRoi=-125;
					// private static final byte STATUS_DATABASE_ERROR_1=-21;
					// private static final byte STATUS_VERIFY_PURCHASE_GOOGLE_API_ERROR=-22;
					// private static final byte STATUS_ERROR_GOLD_MAIN=-23;
					// private static final byte STATUS_VALIDATE_ERROR=-24;
					// private static final byte STATUS_PLAYER_ERROR=-25;
					if(_caseCheck == -22){
						string _tmp = _messageReceiving.readString();
						#if TEST
						Debug.LogError(">>> Error IAP_Android: STATUS_VERIFY_PURCHASE_GOOGLE_API_ERROR - " + _tmp);
						#endif
					}
					if(!_sendInSilent){
						PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kError)
							, MyLocalize.GetString("Error/IAP_CanNotPurchase")
							, _caseCheck.ToString()
							, MyLocalize.GetString(MyLocalize.kOk));
					}else{
						#if TEST
						Debug.LogError(">>> Error IAP_Android: " + MyLocalize.GetString("Error/IAP_CanNotPurchase") + " - " + _caseCheck);
						#endif
					}
				}
			}else{
				// #if TEST
				// Debug.LogError(">>> IAP_Android Error Code: " + _error);
				// #endif
				/* 1/ Thông báo : lỗi network --> show nut refresh
				 * 2/ Thông báo : hông được clear dữ liệu + nếu tắt ứng dụng ngay lúc này thì : lần kết nối sau sẽ tự động cộng tiền khi có kết nối với server
				 */
				if(!_sendInSilent){
					PopupManager.Instance.CreatePopupDialog(MyLocalize.GetString(MyLocalize.kError)
						, MyLocalize.GetString("Error/IAP_PurchaseUnsuccessful")
						, _error.ToString()
						, MyLocalize.GetString(MyLocalize.kTryAgain)
						, MyLocalize.GetString(MyLocalize.kOk)
						, ()=>{SendMessageToServer(_serverDetail, _onSucceed, _onFinished);}
						, ()=>{
							// IAP_Warning_CloseConnectToSv
							PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kWarning)
							, MyLocalize.GetString("System/IAP_Warning_CloseConnectToSv")
							, string.Empty
							, MyLocalize.GetString(MyLocalize.kOk));
						});
				}else{
					#if TEST
					Debug.LogError(">>> Error IAP_Android: " + MyLocalize.GetString("Error/IAP_PurchaseUnsuccessful") + " - " + _error);
					#endif
				}
			}
			if(_onFinished != null){
				_onFinished();
			}
		});
		#elif UNITY_IOS
		//TODO: Chưa làm cho IOS
		Debug.Log("Chưa làm");
		#endif
	}
}