using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyLocalize {

	// ------------------------------------------------------ //
	public const string kOk = "Global/Ok";
	public const string kCancel = "Global/Cancel";
	public const string kClose = "Global/Close";
	public const string kQuit = "Global/Quit";
	public const string kUpdate = "Global/Update";
	public const string kYes = "Global/Yes";
	public const string kNo = "Global/No";
	public const string kError = "Global/Error";
	public const string kWarning = "Global/Warning";
	public const string kMessage = "Global/Message";
	public const string kSkip = "Global/Skip";
	public const string kTryAgain = "Global/Try Again";
	// ------------------------------------------------------ //
	

	// ------------------------------------------------------ //
	public const string kForcedUpdate = "System/ForcedUpdate";
	public const string kHasNewVersion = "System/HasNewVersion";
	public const string kAskForQuit = "System/AskForQuit";
	// ------------------------------------------------------ //

	// ------------------------------------------------------ //
	public const string kPassIsInvalid_00 = "RegisAndLogin/PassIsInvalid_00";
	public const string kUserNameIsInvalid = "RegisAndLogin/UserNameIsInvalid";
	public const string kUsernameAlreadyExists = "RegisAndLogin/UsernameAlreadyExists";
	public const string kPassIsInvalid_01 = "RegisAndLogin/PassIsInvalid_01";
	public const string kPassIsTooLong = "RegisAndLogin/PassIsTooLong";
	// ------------------------------------------------------ //

	// ------------------------------------------------------ //
	public const string kConnectionError = "Error/ConnectionError";
	public const string kSystemError = "Error/SystemError";
	public const string kUnauthorizedError = "Error/UnauthorizedError";
	// ------------------------------------------------------ //

	public const string kServerFull = "ChooseTable/SeverFull";
	public const string kTableFull = "ChooseTable/TableFull";
	public const string kRoomIsNotAvailable = "ChooseTable/RoomIsNotAvailable";

	static ILocalizeInfo currentLocalizeInfo;
	
	public static void InitData(){
		switch(DataManager.instance.currentLanguage){
		case ILocalizeInfo.Language.EN:
			currentLocalizeInfo =  GameInformation.instance.myListLocalizeInfo.localize_En;
			break;
		default:
			Debug.LogError("Cần init thêm ngôn ngữ: " + DataManager.instance.currentLanguage.ToString());
			break;
		}
	}

	public static string GetString(string _key){
		if(currentLocalizeInfo == null){
			Debug.LogError("currentLocalizeInfo is null");
			return string.Empty;
		}
		if(currentLocalizeInfo.store.dictionary.ContainsKey(_key)){
			return currentLocalizeInfo.store.dictionary[_key];
		}
		return string.Empty;
	}
}
