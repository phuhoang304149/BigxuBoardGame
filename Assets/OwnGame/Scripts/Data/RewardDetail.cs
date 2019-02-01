using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RewardDetail {
	public IItemInfo itemInfo{
		get{
			if(_itemInfo == null){
				_itemInfo = GameInformation.instance.GetItemInfo(itemType);
			}
			return _itemInfo;
		}
	}
	IItemInfo _itemInfo;

	public IItemInfo.ItemType itemType;
	public long quantity;

	public RewardDetail(){}

	public RewardDetail (IItemInfo.ItemType _itemType, long _quantity) {
		itemType = _itemType;
		quantity = _quantity;
		_itemInfo = GameInformation.instance.GetItemInfo(itemType);
	}

	// public void RecieveReward(){
	// 	switch(itemType){
	// 	case IItemInfo.ItemType.Gold:
	// 		DataManager.instance.userData.gold += quantity;
	// 		break;
	// 	}
	// }
}
