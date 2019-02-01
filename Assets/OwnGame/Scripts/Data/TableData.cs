using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TableData {
	public int numberPlaying;
	public List<TableDetail> listTableDetail;
	public TableDetail currentTableDetail{
		get{
			return _currentTableDetail;
		}
		set{
			_currentTableDetail = value;
		}
	}
	private TableDetail _currentTableDetail;
    public sbyte maxPlayer;
    public sbyte maxViewer;

    public TableData (){
		listTableDetail = new List<TableDetail> ();
	}

	public void AddNewTableDetail(TableDetail _tableDetail){
		listTableDetail.Add (_tableDetail);
	}

	// public List<TableDetail> GetListTableFull(){
	// 	listTableFull = new List<TableDetail>();
	// 	for(int i = 0; i < listTableDetail.Count; i++){
	// 		if(listTableDetail[i].countPlaying >= maxPlayer || listTableDetail[i].countViewer >= maxViewer){
	// 			listTableFull.Add(listTableDetail[i]);
	// 		}
	// 	}
	// 	return listTableFull;
	// }

	public bool CanTableBeEnable(TableDetail _tableDetail){
		if(_tableDetail.countViewer >= maxViewer
			|| _tableDetail.isLockByPass){
			return false;
		}
		return true;
	}

	public TableData ShallowCopy()
    {
       TableData other = (TableData) this.MemberwiseClone();
       return other;
    }

	public void CheckWhenLogin(){
		if(listTableDetail == null){
			listTableDetail = new List<TableDetail>();
		}
	}
}

[System.Serializable]
public class TableDetail{
	public short tableId;
	public bool isLockByPass;
	public sbyte status;
	public long bet;
	public sbyte countViewer;
    public short countPlaying;

    public TableDetail(){}
	
}