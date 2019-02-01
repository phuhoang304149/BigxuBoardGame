using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelListChipDetailController : MonoBehaviour {

	public LoopScrollRect mainScrollRect;
	public List<IChipInfo> listChipInfo;
	public List<ChipDetail> listChipDetail{get;set;}

	public ChipDetail currentChip{get;set;}

	[ContextMenu("Test Sort List Chip Info")]
    void TestSortListChipInfo()
    {
        if (listChipInfo == null)
        {
            Debug.LogError("listChipInfo is null");
            return;
        }

        listChipInfo.Sort(delegate (IChipInfo x, IChipInfo y)
        {
            Debug.Log(x.value + " - " + y.value + " - " + x.value.CompareTo(y.value));
            return x.value.CompareTo(y.value);
        });
    }

	private void Awake() {
		currentChip = null;

		listChipDetail = new List<ChipDetail>();
		for(int i = 0; i < listChipInfo.Count; i++){
			ChipDetail _tmp = new ChipDetail(listChipInfo[i], i);
			listChipDetail.Add(_tmp);
		}

		SetCurrentChip();
	}

	public void InitData(){
		mainScrollRect.totalCount = listChipInfo.Count;
		if(currentChip != null){
			// mainScrollRect.SrollToCell(Mathf.Clamp(currentChip.index, 0, listChipDetail.Count - 1), 10000f);
        	mainScrollRect.RefillCells(Mathf.Clamp(currentChip.index - 2, 0, listChipDetail.Count - 1));
		}else{
			mainScrollRect.RefillCells();
		}
	}

	public void SetCurrentChip(){
		long _goldPrefer = 0;
		long _goldView = DataManager.instance.userData.GetGoldView();
		if(_goldView < 10){
			if(_goldView == 0){
				_goldPrefer=0;
			}else{
				_goldPrefer=1;
			}
		}else{
			_goldPrefer = MyConstant.GetGoldPrefer(_goldView/10);
		}

		for(int i = 0; i < listChipDetail.Count; i++){
			if(listChipDetail[i].chipInfo.value <= _goldView){
				listChipDetail[i].isDisable = false;
			}else{
				listChipDetail[i].isDisable = true;
			}
			if(listChipDetail[i].chipInfo.value <= _goldPrefer){
				currentChip = listChipDetail[i];
			}
		}

		if(currentChip != null){
			currentChip.isFocusing = true;
		}

	}

	public void SetFocusChipAgain(){
		RefreshListChips();
		long _goldView = DataManager.instance.userData.GetGoldView();
		if(currentChip == null){
			SetCurrentChip();
			if(currentChip == null){
				return;
			}
		}else{
			if(currentChip.chipInfo.value <= _goldView){
				return;
			}

			ChipDetail _tmpChip = null;
			for(int i = 0; i < listChipDetail.Count; i++){
				if(listChipDetail[i].chipInfo.value <= _goldView){
					_tmpChip = listChipDetail[i];
				}
			}
			
			if(currentChip != null){
				currentChip.isFocusing = false;
			}
			if(_tmpChip != null){
				_tmpChip.isFocusing = true;
				currentChip = _tmpChip;
			}else{
				currentChip = null;
			}
		}
		
		if(currentChip != null){
			mainScrollRect.SrollToCell(Mathf.Clamp(currentChip.index, 0, listChipDetail.Count - 1), 10000f);
		}else{
			mainScrollRect.SrollToCell(0, 10000f);
		}
	}

	public void RefreshListChips(){
		long _goldView = DataManager.instance.userData.GetGoldView();
		for(int i = 0; i < listChipDetail.Count; i++){
			if(listChipDetail[i].chipInfo.value <= _goldView){
				listChipDetail[i].isDisable = false;
			}else{
				listChipDetail[i].isDisable = true;
			}
		}
	}

	public void OnSelected(ChipDetail _chipDetail){
		if(currentChip == null){
			currentChip = _chipDetail;
			currentChip.isFocusing = true;
		}else if(!currentChip.IsEqual(_chipDetail)){
			currentChip.isFocusing = false;
			currentChip = _chipDetail;
			currentChip.isFocusing = true;
		}
	}

	public void SelfDestruction(){
		mainScrollRect.ClearCells();
	}
}
