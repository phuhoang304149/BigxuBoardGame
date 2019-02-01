using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Uno_Utilities {

	public static CardUnoInfo GetCardInfo(this Uno_GamePlay_Manager _gamePlayManager,  int _value){
		if(_gamePlayManager.listCardDetail == null || _gamePlayManager.listCardDetail.Count == 0){
			return null;
		}
		for(int i = 0; i < _gamePlayManager.listCardDetail.Count; i ++){
			if(_gamePlayManager.listCardDetail[i].cardId == _value){
				return _gamePlayManager.listCardDetail[i].cardInfo;
			}
		}
		return null;
	}

	public static CardUnoInfo GetCardInfo(this Uno_GamePlay_Manager _gamePlayManager, CardUnoInfo.CardType _cardType){
		if(_gamePlayManager.listCardDetail == null || _gamePlayManager.listCardDetail.Count == 0){
			return null;
		}
		for(int i = 0; i < _gamePlayManager.listCardDetail.Count; i ++){
			if(_gamePlayManager.listCardDetail[i].cardInfo.cardType == _cardType){
				return _gamePlayManager.listCardDetail[i].cardInfo;
			}
		}
		return null;
	}

	public static void ShowPopupChat(this Uno_GamePlay_Manager _gamePlayManager, short _sessionId, string _strMess){
		if(!_gamePlayManager.unoGamePlayData.listSessionIdGlobalPlayer.Contains(_sessionId)){
			return;
		}
		if(!_gamePlayManager.unoGamePlayData.listSessionIdOnChair.Contains(_sessionId)){
			return;
		}
		int _indexChair = _gamePlayManager.unoGamePlayData.listSessionIdOnChair.IndexOf(_sessionId);
		if(_indexChair < 0){
			return;
		}
		if(!_gamePlayManager.listPlayerGroup[_indexChair].isInitialized){
			return;
		}
		PanelPlayerInfoInGameController _panelPlayerInfo = _gamePlayManager.listPlayerGroup[_indexChair].panelPlayerInfo;
		if(_gamePlayManager.listPlayerGroup[_indexChair].viewIndex == 0){
			_panelPlayerInfo.popupChatPosType = PopupChatManager.PopupChatPosType.Bottom;
		}else if(_gamePlayManager.listPlayerGroup[_indexChair].viewIndex == 1){
			_panelPlayerInfo.popupChatPosType = PopupChatManager.PopupChatPosType.Left;
		}else if(_gamePlayManager.listPlayerGroup[_indexChair].viewIndex == 2){
			_panelPlayerInfo.popupChatPosType = PopupChatManager.PopupChatPosType.Top;
		}else{
			_panelPlayerInfo.popupChatPosType = PopupChatManager.PopupChatPosType.Right;
		}
		Vector3 _pos =  _panelPlayerInfo.imgAvatar.transform.position;
		switch(_panelPlayerInfo.popupChatPosType){
		case PopupChatManager.PopupChatPosType.Top:
			_pos = _panelPlayerInfo.popupChat_PlaceHolder_Top.position;
			break;
		case PopupChatManager.PopupChatPosType.Bottom:
			_pos = _panelPlayerInfo.popupChat_PlaceHolder_Bottom.position;
			break;
		case PopupChatManager.PopupChatPosType.Left:
			_pos = _panelPlayerInfo.popupChat_PlaceHolder_Left.position;
			break;
		case PopupChatManager.PopupChatPosType.Right:
			_pos = _panelPlayerInfo.popupChat_PlaceHolder_Right.position;
			break;
		default:
			Debug.LogError("Cần thêm vào PopupChatPosType: " + _panelPlayerInfo.popupChatPosType.ToString());
			break;
		}
		if(_gamePlayManager.CanPlayMusicAndSfx()){
			MyAudioManager.instance.PlaySfx(_gamePlayManager.myAudioInfo.sfx_PopupChat);
		}
		PopupChatController _popupChat = _gamePlayManager.popupChatManager.CreatePopupChat(_panelPlayerInfo.popupChatPosType, _strMess, _pos);
		_panelPlayerInfo.AddPopUpChat(_popupChat);
    }

	public static bool CanPlayMusicAndSfx(this Uno_GamePlay_Manager _gamePlayManager){
		if(!_gamePlayManager.canShowScene){
			return false;
		}
		if(DataManager.instance.miniGameData.currentSubGameDetail != null){
			return false;
		}
		return true;
	}
}
