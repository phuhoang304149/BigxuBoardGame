using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimalRacing_Utilities {

	public static bool CanAddBet(this AnimalRacing_GamePlay_Manager _gamePlayManager){
		if (_gamePlayManager.animalRacingData == null)
		{
#if TEST
			Debug.LogError(">>> animalRacingData is NULL");
#endif
			return false;
		}

		if (_gamePlayManager.currentState != AnimalRacing_GamePlay_Manager.State.Bet)
		{
#if TEST
			Debug.LogError(">>> Chưa được cược");
#endif
			return false;
		}

		if(_gamePlayManager.animalRacingData.nextTimeToShowResult < System.DateTime.Now.AddSeconds(2.5f)
            || _gamePlayManager.animalRacingData.currentResultData != null){
#if TEST
			Debug.LogError(">>> Hết thời gian đặt cược");
#endif
			return false;
		}
		
		return true;
	}
	public static bool CanShowPlayerAddBet(this AnimalRacing_GamePlay_Manager _gamePlayManager)
	{
		if (_gamePlayManager.currentState != AnimalRacing_GamePlay_Manager.State.Bet)
		{
#if TEST
			Debug.LogError(">>> Không ở trong trang đặt cược nên ko setup");
#endif
			return false;
		}
		return true;
	}

	public static void ShowPopupChat(this AnimalRacing_GamePlay_Manager _gamePlayManager, short _sessionId, string _strMess){
        if (_gamePlayManager.betManager.myCurrentState != AnimalRacing_Bet_Manager.State.Show){
            return;
        }
        if(_sessionId == DataManager.instance.userData.sessionId){
            return;
        }
        for(int i = 0; i < _gamePlayManager.betManager.listOtherPlayerInfo.Count; i++){
            if(_gamePlayManager.betManager.listOtherPlayerInfo[i].data != null
                && _gamePlayManager.betManager.listOtherPlayerInfo[i].data.IsEqual(_sessionId)){
                PanelPlayerInfoInGameController _panelPlayerInfo = _gamePlayManager.betManager.listOtherPlayerInfo[i];
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
                break;
            }
        }
    }

	public static bool CanPlayMusicAndSfx(this AnimalRacing_GamePlay_Manager _gamePlayManager){
		if(!_gamePlayManager.canShowScene){
			return false;
		}
		if(DataManager.instance.miniGameData.currentSubGameDetail != null){
			return false;
		}
		return true;
	}
}
