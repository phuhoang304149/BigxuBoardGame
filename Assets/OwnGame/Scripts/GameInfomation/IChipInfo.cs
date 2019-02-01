using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewChipInfo", menuName="GameInfo/ChipInfo")]
public class IChipInfo : ScriptableObject {

	public enum ChipType{
		_1, _2, _5, _10, _20, _50, _100, _200, _500,
		_1K, _2K, _5K, _10K, _20K, _50K, _100K, _200K, _500K,
		_1M, _2M, _5M, _10M, _20M, _50M, _100M, _200M, _500M
	}
	public ChipType chipType;
	public Sprite mySprite;
	public long value;
}

public class ChipDetail{
	public IChipInfo chipInfo;
	public bool isFocusing;
	public bool isDisable;
	public int index;

	public ChipDetail(IChipInfo _chipInfo, int _index){
		chipInfo = _chipInfo;
		index = _index;
	}

	public bool IsEqual(ChipDetail _other){
		if(chipInfo.chipType == _other.chipInfo.chipType){
			return true;
		}
		return false;
	}
}