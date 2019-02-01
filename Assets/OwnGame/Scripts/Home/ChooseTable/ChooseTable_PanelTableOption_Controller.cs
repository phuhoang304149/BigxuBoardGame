using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseTable_PanelTableOption_Controller : MonoBehaviour {

    [SerializeField] GameObject panelTableId;
    [SerializeField] Text txt_TableId;
	[SerializeField] Text txt_Bet;
	[SerializeField] GameObject iconHavePassword;
    [SerializeField] GameObject iconBet;
    [SerializeField] GameObject panelLock;
	[SerializeField] CanvasGroup canvasGroupListChairs;
	[SerializeField] List<Image> listChairHolder;

	TableDetail tableDetail;

	ChooseTableScreenController chooseTableScreen;

	public void InitData(ChooseTableScreenController _chooseTableScreen,TableDetail _tableDetail){
		chooseTableScreen = _chooseTableScreen;
		
		tableDetail = _tableDetail;
		txt_TableId.text = string.Empty;
		txt_Bet.text = string.Empty;
		iconHavePassword.SetActive(false);
		if (tableDetail == null) {
			panelLock.SetActive (true);
            iconBet.SetActive(false);
            panelTableId.SetActive(false);
			canvasGroupListChairs.alpha = 0f;
        } else {
			if(tableDetail.isLockByPass){
				iconHavePassword.SetActive(true);
			}
			panelLock.SetActive (false);
            iconBet.SetActive(true);
            panelTableId.SetActive(true);
            txt_TableId.text = string.Format("{0:00}", tableDetail.tableId);
			txt_Bet.text = MyConstant.GetMoneyString(tableDetail.bet, 9999);
			canvasGroupListChairs.gameObject.SetActive(true);
			canvasGroupListChairs.alpha = 1f;
			int _numPlayerOnTable = tableDetail.countPlaying;
			switch(DataManager.instance.miniGameData.currentMiniGameDetail.myInfo.gameType){
			case IMiniGameInfo.Type.AnimalRacing:
				_numPlayerOnTable = tableDetail.countViewer;
				break;
			}
			Color _c = Color.white;
			for(int i = 0; i < listChairHolder.Count; i ++){
				if(i < _numPlayerOnTable){
					_c = listChairHolder[i].color;
					_c.a = 1f;
					listChairHolder[i].color = _c;
					int _indexIcon = Random.Range(0, GameInformation.instance.otherInfo.listChooseTableIcon.Count);
					listChairHolder[i].sprite = GameInformation.instance.otherInfo.listChooseTableIcon[_indexIcon];
				}else{
					_c = listChairHolder[i].color;
					_c.a = 0f;
					listChairHolder[i].color = _c;
				}
			}
		}
	}

	public void OnButtonSelectClicked(){
		MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_Click);
		
		if(chooseTableScreen.timeCanPressSelectServerOrTable > System.DateTime.Now){
			return;
		}
		chooseTableScreen.timeCanPressSelectServerOrTable = System.DateTime.Now.AddSeconds(chooseTableScreen.timeDelayToPressSelectServerOrTable);

		if (panelLock.activeSelf || tableDetail == null) {
			PopupManager.Instance.CreateToast ("Table is not available");
			return;
		}
		#if TEST
		Debug.Log (">>> Chọn bàn: " + tableDetail.tableId);
		#endif

		chooseTableScreen.OnChooseTable(tableDetail);
	}
}
