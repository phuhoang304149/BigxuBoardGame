using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] public class IAPProductData {

	public List<IAPProductDetail> listProductDetail;
	public bool isInitialized;

	public IAPProductData(){}

	public void InitData(){
		SetDefaultData();
		isInitialized = true;
	}

	public void SetDefaultData(){
		// '1', '1_usd', '200000', 'a', '111', '11'
		// '2', '2_usd', '450000', 'b', '222', '12'
		// '5', '5_usd', '1350000', 'c', '333', '13'
		// '10', '10_usd', '3000000', 'd', '444', '114'
		// '20', '20_usd', '8000000', 'e', '555', '15'
		// '50', '50_usd', '20000000', 'f', '666', '16'
		// '100', '100_usd', '50000000', 'g', '777', '17'
		// '200', '200_usd', '150000000', 'h', '888', '18'
		// '400', '400_usd', '590000000', 'i', '999', '19'

		listProductDetail = new List<IAPProductDetail>();
		listProductDetail.Add(new IAPProductDetail(1, "1_usd", 200000));
		listProductDetail.Add(new IAPProductDetail(2, "2_usd", 450000));
		listProductDetail.Add(new IAPProductDetail(5, "5_usd", 1350000));
		listProductDetail.Add(new IAPProductDetail(10, "10_usd", 3000000));
		listProductDetail.Add(new IAPProductDetail(20, "20_usd", 8000000));
		listProductDetail.Add(new IAPProductDetail(50, "50_usd", 20000000));
		listProductDetail.Add(new IAPProductDetail(100, "100_usd", 50000000));
		listProductDetail.Add(new IAPProductDetail(200, "200_usd", 150000000));
		listProductDetail.Add(new IAPProductDetail(400, "400_usd", 590000000));
	}

	public void CheckWhenLogin(){
		if(listProductDetail == null){
			SetDefaultData();
		}

		for(int i = 0; i < listProductDetail.Count; i++){
			listProductDetail[i].CheckWhenLogin();
		}
	}

	public void LoadSubServerDataFromSv(List<IAPProductDetail> _newListProductDetail){
		if(_newListProductDetail == null || _newListProductDetail.Count == 0){
            #if TEST
            Debug.LogError(">>> _newListProductId is null or count = 0");
            #endif
            return;
        }
		listProductDetail = _newListProductDetail;
    }

	public IAPProductDetail GetProductDetail(string _productId){
		if(listProductDetail == null || listProductDetail.Count == 0){
			return null;
		}
		for(int i = 0; i < listProductDetail.Count; i++){
			if(listProductDetail[i].productId.Equals(_productId)){
				return listProductDetail[i];
			}
		}
		return null;
	}
}

[System.Serializable] public class IAPProductDetail {
	public int id;
	public string productId;
	public long goldBuy;
	public string discount_title;
	public System.DateTime discount_time_finish;
	public long discount_gold;

	public IAPProductDetail(){}

	public IAPProductDetail(MessageReceiving _mess){
		id = _mess.readInt();
		productId = _mess.readString();
		goldBuy = _mess.readLong();
		discount_title = _mess.readString();

		long _deltaTimeDiscount = _mess.readLong();
		System.DateTime _start = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
		long _currentMillisecondsDiscountFinish = MyConstant.currentTimeMilliseconds + _deltaTimeDiscount;
		discount_time_finish = _start.AddMilliseconds(_currentMillisecondsDiscountFinish).ToLocalTime();

		discount_gold = _mess.readLong();
	}

	public IAPProductDetail(int _id, string _productId, long _goldBuy){
		id = _id;
		productId = _productId;
		goldBuy = _goldBuy;
		discount_time_finish = System.DateTime.Now;
	}

	public void CheckWhenLogin(){
		if(discount_time_finish == System.DateTime.MinValue){
			discount_time_finish = System.DateTime.Now;
		}
	}
}