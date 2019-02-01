using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetGoldScreen_PanelInviteFriend_Controller : MySimplePanelController {

	public enum State{
		Hide, Show
	}
	public State currentState{ get; set;}

	[SerializeField] CanvasGroup myCanvasGroup;
	[SerializeField] Transform panelFocusScreen;

	[Header("List Panels")]
	[SerializeField] Transform panelTip;
	[SerializeField] Image imgTip;
	[SerializeField] GetGoldScreen_PanelInviteFriend_PanelMyInfo_Controller panelMyInfo;
	[SerializeField] GetGoldScreen_PanelInviteFriend_PanelInputCode_Controller panelInputCode;

	[Header("Variable")]
	[SerializeField] UserData parentInfo;

	public override void ResetData(){
		currentState = State.Hide;
		myCanvasGroup.alpha = 0f;
		myCanvasGroup.blocksRaycasts = false;

		panelFocusScreen.gameObject.SetActive(false);
		panelInputCode.ResetData();
		panelMyInfo.ResetData();
		imgTip.sprite = null;

		parentInfo = new UserData();
	}

	public override void InitData (System.Action _onFinished = null){
		if(HomeManager.instance != null){
			if(DataManager.instance.parentUserData.isInitialized){
				panelMyInfo.InitData();
				panelMyInfo.Show();
				panelTip.gameObject.SetActive(false);
			}else{
				panelTip.gameObject.SetActive(true);
				var _obj = GameInformation.instance.tipInviteFriend.Load();
				if(_obj != null){
					imgTip.sprite = (Sprite) _obj;
				}
			}
		}else{
			panelTip.gameObject.SetActive(false);
			if(DataManager.instance.parentUserData.isInitialized){
				panelMyInfo.InitData();
				panelMyInfo.Show();
            }else{
				panelInputCode.InitData(SendMessageSearchParent);
				panelInputCode.Show();
            }
		}
	}

	public override void RefreshData(){
		panelFocusScreen.gameObject.SetActive(false);
		panelInputCode.ResetData();
		panelMyInfo.ResetData();

		if(HomeManager.instance != null){
			panelTip.gameObject.SetActive(true);
			var _obj = GameInformation.instance.tipInviteFriend.Load();
			if(_obj != null){
				imgTip.sprite = (Sprite) _obj;
			}
		}else{
			panelTip.gameObject.SetActive(false);
			if(DataManager.instance.parentUserData.isInitialized){
				panelMyInfo.InitData();
				panelMyInfo.Show();
            }else{
				panelInputCode.InitData(SendMessageSearchParent);
				panelInputCode.Show();
            }
		}
	}

	public override Coroutine Show (){
		currentState = State.Show;
		myCanvasGroup.alpha = 1f;
		myCanvasGroup.blocksRaycasts = true;
		panelFocusScreen.gameObject.SetActive(true);
		return null;
	}

	public override Coroutine Hide (){
		ResetData();
		return null;
	}

	public void SendMessageSearchParent(short _parentSessionId){
		// panelConfirm.InitData(parentInfo);
		// panelConfirm.Show();

		SubServerDetail _serverDetail = GetGoldScreenController.instance.GetServerDetail();
		OneHitAPI.InviteFriend_SearchParentInfo (_serverDetail, _parentSessionId, (_messageReceiving, _error) => {
				if(_messageReceiving != null){
					sbyte _caseCheck = _messageReceiving.readByte();
					if(_caseCheck == 1){
						// byte databaseid
						// long userid
						// byte avatarid
						// String nameShow
						// ***nếu databaseid là facebook thì đọc thêm kiểu long : facebookid --> lấy avatar
						// ***nếu databaseid là google thì đọc thêm kiểu string : link_icon

						UserData.DatabaseType _databaseId = (UserData.DatabaseType)_messageReceiving.readByte();
						long _userid = _messageReceiving.readLong();
						sbyte _avatarid = _messageReceiving.readByte();
						string _nameShow = _messageReceiving.readString();
						long _facebookId = -1;
						if(_databaseId == UserData.DatabaseType.DATABASEID_FACEBOOK){
							_facebookId = _messageReceiving.readLong();
						}else if(_databaseId == UserData.DatabaseType.DATABASEID_GOOGLE){
							string _linkIcon = _messageReceiving.readString();
						}else{
							#if TEST
							Debug.LogError("DatabaseID khác : " + _databaseId);
							#endif
						}

						parentInfo = new UserData(); 
						parentInfo.InitData();
						parentInfo.sessionId = _parentSessionId;
						parentInfo.databaseId = (UserData.DatabaseType) _databaseId;
						parentInfo.userId = _userid;
						parentInfo.avatarid = _avatarid;
						parentInfo.nameShowInGame = _nameShow;
						parentInfo.facebookId = _facebookId;
						
						PopupManager.Instance.CreatePopupConfirmInviteFriend(parentInfo
							, MyLocalize.GetString(MyLocalize.kYes)
							, MyLocalize.GetString(MyLocalize.kNo)
							, ()=>{
								if(parentInfo.sessionId == DataManager.instance.userData.sessionId){
									PopupManager.Instance.CreateToast(MyLocalize.GetString("InviteFriend/InvalidCode"));
								}else{
									GlobalRealTimeSendingAPI.SendMessageSetParent(parentInfo.sessionId);
								}
							}, null);
					}else{
						parentInfo = new UserData();
						PopupManager.Instance.CreateToast(MyLocalize.GetString("InviteFriend/CouldNotFound"));
					}
				}else{
					#if TEST
					Debug.LogError("InviteFriend_SearchParentInfo Error: " + _error);
					#endif
				}
			}
		);
	}
}
