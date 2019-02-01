using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Poker_History_OptionInfo_Controller : MySimplePoolObjectController {

	[SerializeField] RawImage imgAvatar;
	[SerializeField] Image imgIconAcc;
	[SerializeField] Text txtNameShow;
	[SerializeField] Text txtGoldWin;
	[SerializeField] Text txtWinner;
	[SerializeField] List<PanelCardDetailController> ownCards;
	[SerializeField] List<PanelCardDetailController> highlightCards;
	[SerializeField] Transform panelShadow;

	[Header("Text Status")]
	[SerializeField] Text txtStatus;
	[SerializeField] Color colorTxtStatus_Normal;
	[SerializeField] Color colorTxtStatus_Fold;

	[Header("Background")]
	[SerializeField] Image imgBg;
	[SerializeField] Sprite bg_Normal;
	[SerializeField] Sprite bg_Highlight;
	PokerGamePlayData.Poker_PlayerPlayingData data;

	public override void ResetData(){
        for(int i = 0; i < ownCards.Count; i++){
			ownCards[i].ResetData();
		}
		imgAvatar.texture = CoreGameManager.instance.gameInfomation.otherInfo.avatarDefault;
		panelShadow.gameObject.SetActive(false);
		imgBg.sprite = bg_Normal;
		txtStatus.color = colorTxtStatus_Normal;
		txtGoldWin.text = string.Empty;

		if(data != null){
			data = null;
		}
		for(int i = 0; i < ownCards.Count; i ++){
			ownCards[i].ResetData();
		}
		for(int i = 0; i < highlightCards.Count; i ++){
			highlightCards[i].ResetData();
		}
    }

	public void InitData(PokerGamePlayData.Poker_PlayerPlayingData _data, bool _showPanelHighlight){
		data = _data;
		bool _showPanelShadow = false;
		string _status = string.Empty;
		if(data.typeCardResult != PokerGamePlayData.TypeCardResult.UNKNOWN){
			_status = PokerGamePlayData.GetStringTypeCardResult(data.typeCardResult);
		}else if(data.currentState == PokerGamePlayData.Poker_PlayerPlayingData.State.STATEPOKER_FOLD){
			_status = "FOLD";
			_showPanelShadow = true;
		}
		
		Sprite _iconDatabaseID = _data.userData.GetIconDatabaseID();
		if(_iconDatabaseID != null){
			imgIconAcc.gameObject.SetActive(true);
			imgIconAcc.sprite = _iconDatabaseID;
		}else{
			imgIconAcc.gameObject.SetActive(false);
		}

		txtNameShow.text = MyConstant.ConvertString(_data.userData.nameShowInGame , 15);
		if(data.goldWinOrReturn > 0){
			txtGoldWin.text = "+"+MyConstant.GetMoneyString(data.goldWinOrReturn, 9999);
		}else{
			txtGoldWin.text = string.Empty;
		}

		txtStatus.text = _status;
		if(_showPanelShadow){
			txtStatus.color = colorTxtStatus_Fold;
		}else{
			txtStatus.color = colorTxtStatus_Normal;

			ICardInfo _cardInfo = null;
			for(int i = 0; i < data.ownCards.Count; i ++){
				if(data.ownCards[i] < 0){
					continue;
				}
				_cardInfo = Poker_GamePlay_Manager.instance.GetCardInfo(data.ownCards[i]);
				if(_cardInfo == null){
					Debug.LogError(">>> cardInfo is null : " + i + " - " + data.ownCards[i]);
					continue;
				}
				ownCards[i].ShowNow(_cardInfo);
				ownCards[i].ResizeAgain();
			}
			for(int i = 0; i < data.highLightCardsResult.Count; i ++){
				if(data.highLightCardsResult[i] < 0){
					continue;
				}
				_cardInfo = Poker_GamePlay_Manager.instance.GetCardInfo(data.highLightCardsResult[i]);
				if(_cardInfo == null){
					Debug.LogError(">>> cardInfo is null : " + i + " - " + data.highLightCardsResult[i]);
					continue;
				}
				highlightCards[i].ShowNow(_cardInfo);
				highlightCards[i].ResizeAgain();
			}
		}

		data.userData.LoadAvatar(this, imgAvatar.rectTransform.rect.width, imgAvatar.rectTransform.rect.height, (_avatar) => {
			try{
				if(_avatar != null){
					imgAvatar.texture = _avatar;
				}
			}catch{}
		});
		
		panelShadow.gameObject.SetActive(_showPanelShadow);
		if(_showPanelHighlight){
			imgBg.sprite = bg_Highlight;
			txtWinner.gameObject.SetActive(true);
		}else{
			imgBg.sprite = bg_Normal;
			txtWinner.gameObject.SetActive(false);
		}
	}
}
