using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetGoldScreen_PanelInviteFriend_PanelInputCode_Controller : MonoBehaviour {

	[SerializeField] CanvasGroup myCanvasGroup;
	[SerializeField] Text txtPlsInputInviteCode;
	[SerializeField] Text txtMyInviteCode;
	[SerializeField] InputField fieldInputInviteCode;

	System.DateTime timeCanPressSearchParent;
	System.Action<short> onBtnSearchedClicked;

	public void ResetData(){
		myCanvasGroup.alpha = 0f;
		myCanvasGroup.blocksRaycasts = false;
		
		fieldInputInviteCode.text = "";
		timeCanPressSearchParent = System.DateTime.Now;
	}

	public void InitData(System.Action<short> _onBtnSearchedClicked){
		onBtnSearchedClicked = _onBtnSearchedClicked;
		txtPlsInputInviteCode.text = MyLocalize.GetString("InviteFriend/PlsInputYourParentCode");
		txtMyInviteCode.text = string.Format(MyLocalize.GetString("InviteFriend/YourInvitationCode"), DataManager.instance.userData.sessionId);
	}

	public void Show(){
		myCanvasGroup.alpha = 1f;
		myCanvasGroup.blocksRaycasts = true;
	}

	public void Hide(){
		myCanvasGroup.alpha = 0f;
		myCanvasGroup.blocksRaycasts = false;
		ResetData();
	}

	#region On Button Clicked
	public void OnValueInputEndEdit(){
		if(string.IsNullOrEmpty(fieldInputInviteCode.text)){
			return;
		}
		int _parentSessionId = int.Parse(fieldInputInviteCode.text);
		if(_parentSessionId > 32767){
			_parentSessionId = 32767;
			fieldInputInviteCode.text = "32767";
		}
	}

	public void OnButtonSearchParentClicked(){
		if(string.IsNullOrEmpty(fieldInputInviteCode.text)){
			return;
		}
		int _parentSessionId = int.Parse(fieldInputInviteCode.text);
		if(_parentSessionId > 32767){
			#if TEST
			Debug.LogError(">>> Bug Over flow");
			#endif
			return;
		}

		if(timeCanPressSearchParent > System.DateTime.Now){
			return;
		}
		timeCanPressSearchParent = System.DateTime.Now.AddSeconds(0.5f);

		// if(_parentSessionId == DataManager.instance.userData.sessionId){
		// 	PopupManager.Instance.CreateToast(MyLocalize.GetString("InviteFriend/YouCouldNotInvitedByYourself"));
		// 	return;
		// }

		if(onBtnSearchedClicked != null){
			onBtnSearchedClicked((short) _parentSessionId);
		}
	}
	#endregion
}
