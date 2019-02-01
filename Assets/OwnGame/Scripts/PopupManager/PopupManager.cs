using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PopupManager : MonoBehaviour {

	[System.Serializable] public class Popup_AudioInfo{
		public AudioClip sfx_Popup;
		public AudioClip sfx_PopupReward;
	}
	
	[SerializeField] CanvasGroup myCanvasGroup;
	[SerializeField] Canvas myCanvas;
	[SerializeField] GameObject pool;

	[Header("Prefabs")]
	[SerializeField] PopupPlayerInfoController popupPlayerInfoPrefab;
	[SerializeField] PopupInfoController popupInfoPrefab;
	[SerializeField] ToastController toastPrefab;
	[SerializeField] PopupMessageController popupMessagePrefab;
	[SerializeField] PopupDialogController popupDialogPrefab;
	[SerializeField] PopupRewardController popupRewardPrefab;
	[SerializeField] PopupCreateTableController popupCreateTablePrefab;
	[SerializeField] PopupJoinTableController popupJoinTablePrefab;
	[SerializeField] PopupConfirmInviteFriendController popupConfirmInviteFriendPrefab;
	[SerializeField] PopupInviteFriendSucessfulController popupInviteFriendSuccessfulPrefab;
	[SerializeField] PopupChangePassController popupChangePasswordPrefab;
	[SerializeField] PopupVerifyEmailController popupVerifyEmailPrefab;
	[SerializeField] PopupForgotPasswordController popupForgotPasswordPrefab;
	[SerializeField] PopupRemindRatingController popupRemindRatingPrefab;
//	public PopupLoginController popupLoginPrefab;

	[Header("Audio Info")]
	public Popup_AudioInfo myInfoAudio;

	List<ToastController> toasts;
	List<IPopupController> popupsActive;

    static PopupManager ins;
	public static PopupManager Instance{
		get{
			return ins;
		}
	}

	void Awake() {
		if (ins != null && ins != this) { 
			Destroy(this.gameObject);
			return;
		}
		ins = this;
		DontDestroyOnLoad (this.gameObject);

		myCanvasGroup.alpha = 1f;
		myCanvasGroup.blocksRaycasts = false;

		if(popupsActive == null){
			popupsActive = new List<IPopupController>();
		}
		if(toasts == null){
			toasts = new List<ToastController>();
		}
	}

	// void Start(){
	// 	StartCoroutine(DoActionTest());
	// }

	// IEnumerator DoActionTest(){
	// 	while(true){
	// 		yield return Yielders.Get(0.1f);
	// 		CreateToast("Toast!");
	// 	}
	// }

	/**
	 * AddPopupActive : add các popup đang active vào list
	 * */
	void AddPopupActive(IPopupController _popup){
		_popup.transform.SetAsLastSibling();
		popupsActive.Add(_popup);
	}

	/**
	 * RemovePopupActive : remove các popup đang active ra khỏi list
	 * */
	public void RemovePopupActive(IPopupController _popup){
		if(popupsActive.Count == 0){
			return;
		}
		popupsActive.Remove(_popup);
		_popup.SelfDestruction();
		if (popupsActive.Count == 0) {
			myCanvasGroup.alpha = 0f;
			myCanvasGroup.blocksRaycasts = false;
		}
	}

	public PopupInfoController CreatePopupInfo(string _textInfo){
		if(myCanvas.worldCamera == null && CoreGameManager.instance.currentSceneManager != null){
			myCanvas.worldCamera = CoreGameManager.instance.currentSceneManager.cameraForConsumableScreen.mainCamera;
		}else{
			myCanvas.worldCamera = Camera.main;
		}

		myCanvasGroup.alpha = 1f;
		myCanvasGroup.blocksRaycasts = true;
		
		PopupInfoController _tmpPopup = LeanPool.Spawn(popupInfoPrefab, Vector3.zero, Quaternion.identity, pool.transform).GetComponent<PopupInfoController>();
		_tmpPopup.Init(_textInfo, () =>{
			RemovePopupActive(_tmpPopup);
		});
		MyAudioManager.instance.PlaySfx(PopupManager.Instance.myInfoAudio.sfx_Popup);
		AddPopupActive(_tmpPopup);

		return _tmpPopup;
	}

	public PopupPlayerInfoController CreatePopupPlayerInfo(UserDataInGame _userDataInGame){
		if(myCanvas.worldCamera == null && CoreGameManager.instance.currentSceneManager != null){
			myCanvas.worldCamera = CoreGameManager.instance.currentSceneManager.cameraForConsumableScreen.mainCamera;
		}else{
			myCanvas.worldCamera = Camera.main;
		}

		myCanvasGroup.alpha = 1f;
		myCanvasGroup.blocksRaycasts = true;

		PopupPlayerInfoController _tmpPopup = LeanPool.Spawn(popupPlayerInfoPrefab, Vector3.zero, Quaternion.identity, pool.transform).GetComponent<PopupPlayerInfoController>();
		_tmpPopup.Init(_userDataInGame, () =>{
			RemovePopupActive(_tmpPopup);
		});
		MyAudioManager.instance.PlaySfx(PopupManager.Instance.myInfoAudio.sfx_Popup);
		AddPopupActive(_tmpPopup);

		return _tmpPopup;
	}

	public void CreateToast(string _textInfo){
		CreateToast(_textInfo, ToastController.posDefault, ToastController.colorTextDefault, ToastController.maxSizeWDefault);
	}

	public void CreateToast(string _textInfo, Vector3 _pos, Color _colorText, float _maxSizeW){
		if(myCanvas.worldCamera == null && CoreGameManager.instance.currentSceneManager != null){
			myCanvas.worldCamera = CoreGameManager.instance.currentSceneManager.cameraForConsumableScreen.mainCamera;
		}else{
			myCanvas.worldCamera = Camera.main;
		}

		myCanvasGroup.alpha = 1f;
		myCanvasGroup.blocksRaycasts = true;

		ToastController _tmpPopup = LeanPool.Spawn(toastPrefab, Vector3.zero, Quaternion.identity, pool.transform).GetComponent<ToastController>();
		_tmpPopup.transform.position = _pos;
		_tmpPopup.Init(_textInfo, _colorText, _maxSizeW, () =>{
			if(toasts != null && toasts.Count > 0){
				toasts.Remove(_tmpPopup);
			}
			RemovePopupActive(_tmpPopup);
		});
		MyAudioManager.instance.PlaySfx(PopupManager.Instance.myInfoAudio.sfx_Popup);
		if(toasts == null){
			toasts = new List<ToastController>();
		}
		for(int i = 0; i < toasts.Count; i++){
			toasts[i].SetSpeedUp();
		}
		toasts.Add(_tmpPopup);
		AddPopupActive(_tmpPopup);
	}

	public PopupMessageController CreatePopupMessage(string _textTitle, string _textMessage, string _errorCode, string _textSubmitButton, System.Action _onSubmit = null){
		if(myCanvas.worldCamera == null && CoreGameManager.instance.currentSceneManager != null){
			myCanvas.worldCamera = CoreGameManager.instance.currentSceneManager.cameraForConsumableScreen.mainCamera;
		}else{
			myCanvas.worldCamera = Camera.main;
		}

		myCanvasGroup.alpha = 1f;
		myCanvasGroup.blocksRaycasts = true;

		PopupMessageController _tmpPopup = LeanPool.Spawn(popupMessagePrefab.transform, Vector3.zero, Quaternion.identity, pool.transform).GetComponent<PopupMessageController>();
		_tmpPopup.Init(_textTitle, _textMessage, _errorCode, _textSubmitButton, _onSubmit, () =>{
			RemovePopupActive(_tmpPopup);
		});
		MyAudioManager.instance.PlaySfx(PopupManager.Instance.myInfoAudio.sfx_Popup);
		AddPopupActive(_tmpPopup);

		return _tmpPopup;
	}

	public PopupDialogController CreatePopupDialog(string _textTitle, string _textMessage, string _errorCode, string _textSubmitButton, string _textCancleButton, System.Action _onSubmit, System.Action _onCanel){
		if(myCanvas.worldCamera == null && CoreGameManager.instance.currentSceneManager != null){
			myCanvas.worldCamera = CoreGameManager.instance.currentSceneManager.cameraForConsumableScreen.mainCamera;
		}else{
			myCanvas.worldCamera = Camera.main;
		}

		myCanvasGroup.alpha = 1f;
		myCanvasGroup.blocksRaycasts = true;

		PopupDialogController _tmpPopup = LeanPool.Spawn(popupDialogPrefab.transform, Vector3.zero, Quaternion.identity, pool.transform).GetComponent<PopupDialogController>();
		_tmpPopup.Init(_textTitle, _textMessage, _errorCode, _textSubmitButton, _textCancleButton, _onSubmit, _onCanel, () =>{
			RemovePopupActive(_tmpPopup);
		});
		MyAudioManager.instance.PlaySfx(PopupManager.Instance.myInfoAudio.sfx_Popup);
		AddPopupActive(_tmpPopup);
		
		return _tmpPopup;
	}

	public PopupConfirmInviteFriendController CreatePopupConfirmInviteFriend(UserData _userData, string _textSubmitButton, string _textCancleButton, System.Action _onSubmit, System.Action _onCanel){
		if(myCanvas.worldCamera == null && CoreGameManager.instance.currentSceneManager != null){
			myCanvas.worldCamera = CoreGameManager.instance.currentSceneManager.cameraForConsumableScreen.mainCamera;
		}else{
			myCanvas.worldCamera = Camera.main;
		}

		myCanvasGroup.alpha = 1f;
		myCanvasGroup.blocksRaycasts = true;

		PopupConfirmInviteFriendController _tmpPopup = LeanPool.Spawn(popupConfirmInviteFriendPrefab.transform, Vector3.zero, Quaternion.identity, pool.transform).GetComponent<PopupConfirmInviteFriendController>();
		_tmpPopup.InitData(_userData, _textSubmitButton, _textCancleButton, _onSubmit, _onCanel, () =>{
			RemovePopupActive(_tmpPopup);
		});
		MyAudioManager.instance.PlaySfx(PopupManager.Instance.myInfoAudio.sfx_Popup);
		AddPopupActive(_tmpPopup);

		return _tmpPopup;
	}

	public PopupInviteFriendSucessfulController CreatePopupInviteFriendSuccessful(UserData _childInfo, UserData _parentInfo, long _goldBonus, string _textSubmitButton, System.Action _onSubmit){
		if(myCanvas.worldCamera == null && CoreGameManager.instance.currentSceneManager != null){
			myCanvas.worldCamera = CoreGameManager.instance.currentSceneManager.cameraForConsumableScreen.mainCamera;
		}else{
			myCanvas.worldCamera = Camera.main;
		}

		myCanvasGroup.alpha = 1f;
		myCanvasGroup.blocksRaycasts = true;

		PopupInviteFriendSucessfulController _tmpPopup = LeanPool.Spawn(popupInviteFriendSuccessfulPrefab.transform, Vector3.zero, Quaternion.identity, pool.transform).GetComponent<PopupInviteFriendSucessfulController>();
		_tmpPopup.InitData(_childInfo, _parentInfo, _goldBonus, _textSubmitButton, _onSubmit, () =>{
			RemovePopupActive(_tmpPopup);
		});
		MyAudioManager.instance.PlaySfx(PopupManager.Instance.myInfoAudio.sfx_Popup);
		AddPopupActive(_tmpPopup);

		return _tmpPopup;
	}

	public Coroutine CreatePopupReward(List<RewardDetail> _listRewardDetail){
		return StartCoroutine(DoActionCreatePopupReward(_listRewardDetail));
	}

	IEnumerator DoActionCreatePopupReward(List<RewardDetail> _listRewardDetail){
		for(int i = 0; i < _listRewardDetail.Count; i++){
			yield return CreatePopupReward(_listRewardDetail[i]);
		}
	}

	public Coroutine CreatePopupReward(RewardDetail _rewardDetail){
		if(myCanvas.worldCamera == null && CoreGameManager.instance.currentSceneManager != null){
			myCanvas.worldCamera = CoreGameManager.instance.currentSceneManager.cameraForConsumableScreen.mainCamera;
		}else{
			myCanvas.worldCamera = Camera.main;
		}

		myCanvasGroup.alpha = 1f;
		myCanvasGroup.blocksRaycasts = true;
		
		return StartCoroutine(DoActionCreatePopupReward(_rewardDetail));
	}

	IEnumerator DoActionCreatePopupReward(RewardDetail _rewardDetail){
		bool _isFinished = false;
		PopupRewardController _tmpPopup = LeanPool.Spawn(popupRewardPrefab.transform, Vector3.zero, Quaternion.identity, pool.transform).GetComponent<PopupRewardController>();
		_tmpPopup.Init(_rewardDetail, () =>{
			RemovePopupActive(_tmpPopup);
			_isFinished = true;
		});
		MyAudioManager.instance.PlaySfx(PopupManager.Instance.myInfoAudio.sfx_PopupReward);
		AddPopupActive(_tmpPopup);
		yield return new WaitUntil(()=>_isFinished);
	}

	public PopupCreateTableController CreatePopupCreateTable(System.Action<string> _onSubmit, System.Action _onCancel){
		if(myCanvas.worldCamera == null && CoreGameManager.instance.currentSceneManager != null){
			myCanvas.worldCamera = CoreGameManager.instance.currentSceneManager.cameraForConsumableScreen.mainCamera;
		}else{
			myCanvas.worldCamera = Camera.main;
		}

		myCanvasGroup.alpha = 1f;
		myCanvasGroup.blocksRaycasts = true;

		PopupCreateTableController _tmpPopup = LeanPool.Spawn(popupCreateTablePrefab.transform, Vector3.zero, Quaternion.identity, pool.transform).GetComponent<PopupCreateTableController>();
		_tmpPopup.Init(_onSubmit, _onCancel, () =>{
			RemovePopupActive(_tmpPopup);
		});
		MyAudioManager.instance.PlaySfx(PopupManager.Instance.myInfoAudio.sfx_Popup);
		AddPopupActive(_tmpPopup);

		return _tmpPopup;
	}

	public PopupJoinTableController CreatePopupJoinTable(string _defaultId, System.Action<string, string> _onSubmit, System.Action _onCancel){
		if(myCanvas.worldCamera == null && CoreGameManager.instance.currentSceneManager != null){
			myCanvas.worldCamera = CoreGameManager.instance.currentSceneManager.cameraForConsumableScreen.mainCamera;
		}else{
			myCanvas.worldCamera = Camera.main;
		}

		myCanvasGroup.alpha = 1f;
		myCanvasGroup.blocksRaycasts = true;

		PopupJoinTableController _tmpPopup = LeanPool.Spawn(popupJoinTablePrefab.transform, Vector3.zero, Quaternion.identity, pool.transform).GetComponent<PopupJoinTableController>();
		_tmpPopup.Init(_defaultId, _onSubmit, _onCancel, () =>{
			RemovePopupActive(_tmpPopup);
		});
		MyAudioManager.instance.PlaySfx(PopupManager.Instance.myInfoAudio.sfx_Popup);
		AddPopupActive(_tmpPopup);

		return _tmpPopup;
	}

	public PopupChangePassController CreatePopupChangePassword(System.Action<string, string> _onSubmit, System.Action _onCancel){
		if(myCanvas.worldCamera == null && CoreGameManager.instance.currentSceneManager != null){
			myCanvas.worldCamera = CoreGameManager.instance.currentSceneManager.cameraForConsumableScreen.mainCamera;
		}else{
			myCanvas.worldCamera = Camera.main;
		}

		myCanvasGroup.alpha = 1f;
		myCanvasGroup.blocksRaycasts = true;

		PopupChangePassController _tmpPopup = LeanPool.Spawn(popupChangePasswordPrefab.transform, Vector3.zero, Quaternion.identity, pool.transform).GetComponent<PopupChangePassController>();
		_tmpPopup.Init(_onSubmit, _onCancel, () =>{
			RemovePopupActive(_tmpPopup);
		});
		MyAudioManager.instance.PlaySfx(PopupManager.Instance.myInfoAudio.sfx_Popup);
		AddPopupActive(_tmpPopup);

		return _tmpPopup;
	}

	public PopupVerifyEmailController CreatePopupVerifyEmail(System.Action<string> _onSubmit, System.Action _onCancel){
		if(myCanvas.worldCamera == null && CoreGameManager.instance.currentSceneManager != null){
			myCanvas.worldCamera = CoreGameManager.instance.currentSceneManager.cameraForConsumableScreen.mainCamera;
		}else{
			myCanvas.worldCamera = Camera.main;
		}

		myCanvasGroup.alpha = 1f;
		myCanvasGroup.blocksRaycasts = true;

		PopupVerifyEmailController _tmpPopup = LeanPool.Spawn(popupVerifyEmailPrefab.transform, Vector3.zero, Quaternion.identity, pool.transform).GetComponent<PopupVerifyEmailController>();
		_tmpPopup.Init(_onSubmit, _onCancel, () =>{
			RemovePopupActive(_tmpPopup);
		});
		MyAudioManager.instance.PlaySfx(PopupManager.Instance.myInfoAudio.sfx_Popup);
		AddPopupActive(_tmpPopup);

		return _tmpPopup;
	}

	public PopupForgotPasswordController CreatePopupForgotPassword(System.Action<string, string, string> _onSubmit, System.Action _onCancel){
		if(myCanvas.worldCamera == null && CoreGameManager.instance.currentSceneManager != null){
			myCanvas.worldCamera = CoreGameManager.instance.currentSceneManager.cameraForConsumableScreen.mainCamera;
		}else{
			myCanvas.worldCamera = Camera.main;
		}

		myCanvasGroup.alpha = 1f;
		myCanvasGroup.blocksRaycasts = true;

		PopupForgotPasswordController _tmpPopup = LeanPool.Spawn(popupForgotPasswordPrefab.transform, Vector3.zero, Quaternion.identity, pool.transform).GetComponent<PopupForgotPasswordController>();
		_tmpPopup.Init(_onSubmit, _onCancel, () =>{
			RemovePopupActive(_tmpPopup);
		});
		MyAudioManager.instance.PlaySfx(PopupManager.Instance.myInfoAudio.sfx_Popup);
		AddPopupActive(_tmpPopup);

		return _tmpPopup;
	}

	public PopupRemindRatingController CreatePopupRemindRating(System.Action _onSubmit, System.Action _onCanel){
		if(myCanvas.worldCamera == null && CoreGameManager.instance.currentSceneManager != null){
			myCanvas.worldCamera = CoreGameManager.instance.currentSceneManager.cameraForConsumableScreen.mainCamera;
		}else{
			myCanvas.worldCamera = Camera.main;
		}

		myCanvasGroup.alpha = 1f;
		myCanvasGroup.blocksRaycasts = true;
		
		PopupRemindRatingController _tmpPopup = LeanPool.Spawn(popupRemindRatingPrefab, Vector3.zero, Quaternion.identity, pool.transform).GetComponent<PopupRemindRatingController>();
		_tmpPopup.Init(_onSubmit, _onCanel, () =>{
			RemovePopupActive(_tmpPopup);
		});
		MyAudioManager.instance.PlaySfx(PopupManager.Instance.myInfoAudio.sfx_Popup);
		AddPopupActive(_tmpPopup);

		return _tmpPopup;
	}

	#region Test
	public void TestCreateToast(){
		CreateToast("Toast", ToastController.posDefault, ToastController.colorTextDefault, ToastController.maxSizeWDefault);
	}

	public void TestCreatePopupInfo(){
		CreatePopupInfo("Popup Info!");
	}

	public void TestCreatePopupMessage(){
		CreatePopupMessage("Title", "Message", string.Empty, "Close");
	}

	public void TestCreatePopupDialog(){
		CreatePopupDialog("Title", "Message", string.Empty, "Yes", "No", () =>{
			Debug.Log("Submit!");
		}, null);
	}

	public void TestCreatePopupConfirmInviteFriend(){
		CreatePopupConfirmInviteFriend(DataManager.instance.userData, "Yes", "No", null, null);
	}

	public void TestCreatePopupInviteFriendSuccessful(){
		CreatePopupInviteFriendSuccessful(DataManager.instance.userData, DataManager.instance.userData, 5000, "Ok", null);
	}

	public void TestCreatePopupChangePassword(){
		CreatePopupChangePassword(null, null);
	}

	public void TestCreatePopupVerifyEmail(){
		CreatePopupVerifyEmail(null, null);
	}

	public void TestCreatePopupForgotPassword(){
		CreatePopupForgotPassword(null, null);
	}

	public void TestCreatePopupRemindRating(){
		CreatePopupRemindRating(null, null);
	}

//	[ContextMenu ("Test Create Popup Reward1")]
//	public void TestCreatePopupDialog1(){
//		CreatePopupReward (new StackableItemData (ItemInfo.ItemID.Shard_Pettomon_00, 10));
//	}
//
//	[ContextMenu ("Test Create Popup Reward3")]
//	public void TestCreatePopupDialog3(){
//		// Generate
//		EquipmentItemData result = new EquipmentItemData ();
//		result.infoId = ItemInfo.ItemID.Equipment_Jewelry_00;
//		EquipmentItemStatData primaryStat = new EquipmentItemStatData ();
//		primaryStat.id = EquipmentItemStatId.IncreaseMountCriticalRate;
//		primaryStat.value = 5;
//		result.primaryStat = primaryStat;
//		result.rarity = Rarity.Common;
//		result.substats = new List<EquipmentItemStatData> ();
//
//		CreatePopupReward (result);
//	}
		
	#endregion

	public void UnActiveAllPopups(){
		if(popupsActive != null && popupsActive.Count > 0){
			for(int i = 0; i < popupsActive.Count; i++){
				popupsActive[i].SelfDestruction();
				popupsActive.RemoveAt(i);
				i--;
			}
			popupsActive.Clear ();
		}
		if(toasts != null && toasts.Count > 0){
			toasts.Clear();
		}

		myCanvasGroup.alpha = 0f;
		myCanvasGroup.blocksRaycasts = false;
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(PopupManager))]
public class PopupManager_Editor : Editor
{
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		PopupManager myScript = (PopupManager) target;
		
		GUILayout.Space(30);
		GUILayout.Label(">>> For Test <<<");

		if (GUILayout.Button ("Test Create Toast")) {
			myScript.TestCreateToast();
		}

		if (GUILayout.Button ("Test Create Popup Info")) {
			myScript.TestCreatePopupInfo();
		}

		if (GUILayout.Button ("Test Create Popup Message")) {
			myScript.TestCreatePopupMessage();
		}

		if (GUILayout.Button ("Test Create Popup Dialog")) {
			myScript.TestCreatePopupDialog();
		}

		if (GUILayout.Button ("Test Create Popup Confirm Invite Friend")) {
			myScript.TestCreatePopupConfirmInviteFriend();
		}

		if (GUILayout.Button ("Test Create Popup Invite Friend Successful")) {
			myScript.TestCreatePopupInviteFriendSuccessful();
		}

		if (GUILayout.Button ("Test Create Popup Change Password")) {
			myScript.TestCreatePopupChangePassword();
		}

		if (GUILayout.Button ("Test Create Popup Verify Email")) {
			myScript.TestCreatePopupVerifyEmail();
		}
		
		if (GUILayout.Button ("Test Create Popup Forgot Pass")) {
			myScript.TestCreatePopupForgotPassword();
		}

		if (GUILayout.Button ("Test Create Popup Remind Rating")) {
			myScript.TestCreatePopupRemindRating();
		}

		if (GUILayout.Button ("UnActive All Popups")) {
			myScript.UnActiveAllPopups();
		}
	}
}
#endif
