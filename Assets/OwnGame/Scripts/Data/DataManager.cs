using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using BayatGames.SaveGameFree;
using BayatGames.SaveGameFree.Serializers;

[System.Serializable]
public class DataManager 
{

	public static DataManager instance {
		get {
			return ins;
		}
	}

	private static DataManager ins;

	public DataManager(){}

	#region Init / Save / Load / Clear
	public ISaveGameSerializer activeSerializer
	{
		get
		{
			if ( m_ActiveSerializer == null )
			{
				m_ActiveSerializer = new SaveGameJsonSerializer ();
			}
			return m_ActiveSerializer;
		}
	}
	private ISaveGameSerializer m_ActiveSerializer;

	private bool encode = false;
	private string encodePassword = "bigxuonline";

	public static void Init(){
		if (ins != null) {
			#if TEST
			Debug.LogError("Đã Init DataManager rồi");
			#endif
			return;
		}
		// --- Clear Data --- //
//		QuickSaveRoot.Delete (MyConstant.rootSaveName);
		// ------------------ //
		if (SaveGame.Exists (MyConstant.rootSaveName)) {
			#if TEST
			Debug.Log (">>> Load DataManager <<<");
			#endif
			LoadData ();
		} else {
			#if TEST
			Debug.Log (">>> Create New DataManager <<<");
			#endif
			SaveData ();
		}
	}

	public static void SaveData(){
		if (ins == null) {
			ins = new DataManager ();
		}
		if(ins.versionData != ins.save_versionData){ // khi save lỗi
			Debug.LogError("DataManager version: " + ins.versionData + " - " + ins.save_versionData);
			return;
		}
		SaveGame.Save<DataManager> (MyConstant.rootSaveName, ins, ins.activeSerializer);
		if(ins.versionData == 0){
			ins.versionData = 1;
			ins.save_versionData = 1;
		}
		
		#if TEST
		string _json = JsonUtility.ToJson(ins);
		Debug.Log (">>> Save Data : " + _json);
		#endif
	}

	public static void LoadData(){
		if (ins == null) {
			ins = new DataManager ();
		}

		ins = SaveGame.Load<DataManager> (
				MyConstant.rootSaveName,
				new DataManager (),
				ins.activeSerializer);

		if(ins.versionData != ins.save_versionData){
			ins.versionData = ins.save_versionData;
		}
		
		#if TEST
		string _json = JsonUtility.ToJson(ins);
		Debug.Log (">>> Load Data : " + _json);
		#endif
	}

	public static void ClearData(){
		ins = new DataManager ();
		SaveGame.Delete(MyConstant.rootSaveName);
		ins.versionData = 0;
		ins.save_versionData = -1;
		SaveData ();
	}
	#endregion

	#region Serializable Data
	///<summary>
	/// save_versionData : biến tạo ra để tham số tới versionData dùng để backup data khi save lỗi
	///		- -1: chưa tạo biến 
	///		- 0: mới tạo biến
	///		- 1: đã tạo biến
	///</summary>
	public int save_versionData{
		get{
			if(_save_versionData == -1){
				_save_versionData = PlayerPrefs.GetInt(MyConstant.save_kVersionDataName, 0);
				if(_save_versionData == -1){
					_save_versionData = 0;
				}
				PlayerPrefs.SetInt(MyConstant.save_kVersionDataName, _save_versionData);
			}
			return _save_versionData;
		}set{
			_save_versionData = value;
			PlayerPrefs.SetInt(MyConstant.save_kVersionDataName, _save_versionData);
		}
	}
	private int _save_versionData = -1;

	public int versionData = 0;

	public UserData userData {
		get {
			if (_userData == null) {
				#if TEST
				Debug.Log ("--- Create New UserData! ---");
				#endif
				_userData = new UserData ();
				_userData.InitData ();
			} else {
				if (!_userData.isInitialized) {
					_userData.InitData ();
				}
			}
			return _userData;
		}
		set {
			_userData = value;
		}
	}
	[SerializeField] private UserData _userData;

	public UserData parentUserData {
		get {
			if(_parentUserData == null){
				_parentUserData = new UserData ();
			}
			return _parentUserData;
		}
		set {
			_parentUserData = value;
		}
	}
	[SerializeField] private UserData _parentUserData;

	public AchievementData achievementData {
		get {
			if (_achievementData == null) {
				#if TEST
				Debug.Log ("--- Create New AchievementData! ---");
				#endif
				_achievementData = new AchievementData ();
				_achievementData.InitData ();
			} else {
				if (!_achievementData.isInitialized) {
					_achievementData.InitData ();
				} 
			}
			return _achievementData;
		}
		set {
			_achievementData = value;
		}
	}
	[SerializeField] private AchievementData _achievementData;

	public DailyRewardData dailyRewardData {
		get {
			if (_dailyRewardData == null) {
				#if TEST
				Debug.Log ("--- Create New DailyRewardData! ---");
				#endif
				_dailyRewardData = new DailyRewardData ();
				_dailyRewardData.InitData ();
			} else {
				if (!_dailyRewardData.isInitialized) {
					_dailyRewardData.InitData ();
				}
			}
			return _dailyRewardData;
		}
		set {
			_dailyRewardData = value;
		}
	}
	[SerializeField] private DailyRewardData _dailyRewardData;

	public SubsidyData subsidyData {
		get {
			if (_subsidyData == null) {
				#if TEST
				Debug.Log ("--- Create New SubsidyData! ---");
				#endif
				_subsidyData = new SubsidyData ();
				_subsidyData.InitData ();
			} else {
				if (!_subsidyData.isInitialized) {
					_subsidyData.InitData ();
				}
			}
			return _subsidyData;
		}
		set {
			_subsidyData = value;
		}
	}
	[SerializeField] private SubsidyData _subsidyData;

	public IAPProductData IAPProductData {
		get {
			if (_IAPProductData == null) {
				#if TEST
				Debug.Log ("--- Create New IAPProductData! ---");
				#endif
				_IAPProductData = new IAPProductData ();
				_IAPProductData.InitData ();
			} else {
				if (!_IAPProductData.isInitialized) {
					_IAPProductData.InitData ();
				}
			}
			return _IAPProductData;
		}
		set {
			_IAPProductData = value;
		}
	}
	[SerializeField] private IAPProductData _IAPProductData;

	public int sfxStatus{ // trạng thái tắt bật âm thanh hiệu ứng (0: tắt, 1 : bật)
		get{
			if(_sfxStatus == -1){
				_sfxStatus = PlayerPrefs.GetInt(MyConstant.save_kSfxName, 1);
			}
			return _sfxStatus;
		}set{
			_sfxStatus = value;
			PlayerPrefs.SetInt(MyConstant.save_kSfxName,_sfxStatus);
		}
	} 
	private int _sfxStatus = -1;

	public int musicStatus{ // trạng thái tắt bật nhạc nền (0: tắt, 1 : bật)
		get{
			if(_musicStatus == -1){
				_musicStatus = PlayerPrefs.GetInt(MyConstant.save_kMusicName, 1);
			}
			return _musicStatus;
		}set{
			_musicStatus = value;
			PlayerPrefs.SetInt(MyConstant.save_kMusicName, _musicStatus);
		}
	} 
	private int _musicStatus = -1;

	public int vibrationStatus{ // trạng thái tắt bật hiệu ứng rung (0: tắt, 1 : bật)
		get{
			if(_vibrationStatus == -1){
				_vibrationStatus = PlayerPrefs.GetInt(MyConstant.save_kVibrationName, 1);
			}
			return _vibrationStatus;
		}set{
			_vibrationStatus = value;
			PlayerPrefs.SetInt(MyConstant.save_kVibrationName, _vibrationStatus);
		}
	} 
	private int _vibrationStatus = -1;

	public MiniGameData miniGameData{
		get{
			if (_miniGameData == null) {
				#if TEST
				Debug.Log ("--- Create New MiniGameData! ---");
				#endif
				_miniGameData = new MiniGameData ();
				_miniGameData.InitData();
			} else {
				if (!_miniGameData.isInitialized) {
					_miniGameData.InitData ();
				}
			}
			return _miniGameData;
		}set{
			_miniGameData = value;
		}
	}
	[SerializeField] private MiniGameData _miniGameData;

	public ILocalizeInfo.Language currentLanguage = ILocalizeInfo.Language.EN;

	public PurchaseReceiptData purchaseReceiptData {
		get {
			if (_purchaseReceiptData == null) {
				#if TEST
				Debug.Log ("--- Create New PurchaseReceiptData! ---");
				#endif
				_purchaseReceiptData = new PurchaseReceiptData ();
				_purchaseReceiptData.InitData ();
			} else {
				if (!_purchaseReceiptData.isInitialized) {
					_purchaseReceiptData.InitData ();
				}
			}
			return _purchaseReceiptData;
		}
		set {
			_purchaseReceiptData = value;
		}
	}
	[SerializeField] private PurchaseReceiptData _purchaseReceiptData;

	public InstallAppData installAppData {
		get {
			if (_installAppData == null) {
				#if TEST
				Debug.Log ("--- Create New InstallAppData! ---");
				#endif
				_installAppData = new InstallAppData ();
				_installAppData.InitData ();
			} else {
				if (!_installAppData.isInitialized) {
					_installAppData.InitData ();
				}
			}
			return _installAppData;
		}
		set {
			_installAppData = value;
		}
	}
	[SerializeField] private InstallAppData _installAppData;

	public LeaderboardData leaderboardData {
		get {
			if (_leaderboardData == null) {
				#if TEST
				Debug.Log ("--- Create New LeaderboardData! ---");
				#endif
				_leaderboardData = new LeaderboardData ();
				_leaderboardData.InitData ();
			} else {
				if (!_leaderboardData.isInitialized) {
					_leaderboardData.InitData ();
				}
			}
			return _leaderboardData;
		}
		set {
			_leaderboardData = value;
		}
	}
	[SerializeField] private LeaderboardData _leaderboardData;

	public BOL_GameData bol_GameData {
		get {
			if (_bol_GameData == null) {
				#if TEST
				Debug.Log ("--- Create New BOL_GameData! ---");
				#endif
				_bol_GameData = new BOL_GameData ();
				_bol_GameData.InitData ();
			} else {
				if (!_bol_GameData.isInitialized) {
					_bol_GameData.InitData ();
				}
			}
			return _bol_GameData;
		}
		set {
			_bol_GameData = value;
		}
	}
	[SerializeField] private BOL_GameData _bol_GameData;

	public SubServerData subServerData {
		get {
			if (_subServerData == null) {
				#if TEST
				Debug.Log ("--- Create New SubServerData! ---");
				#endif
				_subServerData = new SubServerData ();
				_subServerData.InitData ();
			} else {
				if (!_subServerData.isInitialized) {
					_subServerData.InitData ();
				}
			}
			return _subServerData;
		}
		set {
			_subServerData = value;
		}
	}
	[SerializeField] private SubServerData _subServerData;

	public string announcement;
	public string packageNameUpdate;
	public bool getFirstDataSuccessfull;

	public int remindRating_countTimeShowPopup{
		get{
			if(_remindRating_countTimeShowPopup == -1){
				_remindRating_countTimeShowPopup = PlayerPrefs.GetInt(MyConstant.save_kCountTimeShowPopupRemindRating, 0);
			}
			return _remindRating_countTimeShowPopup;
		}set{
			_remindRating_countTimeShowPopup = value;
 			PlayerPrefs.SetInt(MyConstant.save_kCountTimeShowPopupRemindRating, _remindRating_countTimeShowPopup);
		}
	}
	private int _remindRating_countTimeShowPopup = -1;

	public int remindRating_countTimePressOkOnPopup{
		get{
			if(_remindRating_countTimePressOkOnPopup == -1){
				_remindRating_countTimePressOkOnPopup = PlayerPrefs.GetInt(MyConstant.save_kCountTimePressOkOnPopupRating, 0);
			}
			return _remindRating_countTimePressOkOnPopup;
		}set{
			_remindRating_countTimePressOkOnPopup = value;
 			PlayerPrefs.SetInt(MyConstant.save_kCountTimePressOkOnPopupRating, _remindRating_countTimePressOkOnPopup);
		}
	}
	private int _remindRating_countTimePressOkOnPopup = -1;

	public DateTime remindRating_NextTimeCanShowPopup{
		get{
			if(string.IsNullOrEmpty(_remindRating_NextTimeCanShowPopup)){
				_remindRating_NextTimeCanShowPopup = PlayerPrefs.GetString(MyConstant.save_kNextTimeCanShowPopupRemindRating, DateTime.Now.ToString());
			}
			return DateTime.Parse(_remindRating_NextTimeCanShowPopup);
		}set{
			_remindRating_NextTimeCanShowPopup = value.ToString();
 			PlayerPrefs.SetString(MyConstant.save_kNextTimeCanShowPopupRemindRating, _remindRating_NextTimeCanShowPopup);
		}
	}
	private string _remindRating_NextTimeCanShowPopup = string.Empty;

	public bool hadRatingBefore{ // trạng thái đã rating app hay chưa (0: chưa, 1: rồi)
		get{
			if(_kHadRatingBefore == -1){
				_kHadRatingBefore = PlayerPrefs.GetInt(MyConstant.save_kRatingApp, 0);
			}
			return _kHadRatingBefore == 1;
		}set{
			if(value){
				_kHadRatingBefore = 1;
			}else{
				_kHadRatingBefore = 0;
			}
 			PlayerPrefs.SetInt(MyConstant.save_kRatingApp, _kHadRatingBefore);
		}
	} 
	private int _kHadRatingBefore = -1;
	#endregion

	#region Non Serializable Data
	public bool haveNewVersion{
		get{
			return _haveNewVersion;
		}set{
			_haveNewVersion = value;
		}
	}
	private bool _haveNewVersion;

	public long remindRating_GoldUserCatched{
		get{
			return _remindRating_GoldUserCatched;
		}set{
			_remindRating_GoldUserCatched = value;
		}
	}
	private long _remindRating_GoldUserCatched;
	#endregion
}
