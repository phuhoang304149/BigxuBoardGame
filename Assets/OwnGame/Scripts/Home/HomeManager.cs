using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class HomeManager : IMySceneManager
{
    public override Type mySceneType{
		get{
			return Type.Home;
		}
	}

    public enum State
    {
        GetDataFromSplashScreen,
        LogOutAndBackToLoginScreen,
        BackFromGamePlayToChooseTableScreen,
        Unknown
    }
    public static State myCurrentState { get; set; }

    public static HomeManager instance;
    public AnnouncementController announcement;

    [Header("Place Holder")]
    public Transform mainScreenHolder;
    public Transform subScreenHolder;
    [Header("List Screens")]
    public ListScreenInHome listScreens;
    [Header("Audio Info")]
    public Home_AudioInfo myAudioInfo;

    public UIHomeScreenController currentScreen { get; set; }
    public static bool hasShowTopAndBottomBar = false;
    public static bool getGoldAndGemInfoAgain = false;
    public static bool getEmailInfoAgain = false;
    public static bool showAnnouncement = false;

    public Home_CallbackManager myCallbackManager;
    
    System.Action onTestSubFinshed;

    void Awake()
    {
        instance = this;
        CoreGameManager.instance.currentSceneManager = instance;
    }

    [ContextMenu("AAAAA")]
    public void AAAAA()
    {
        List<int> a = new List<int>();
        a.Add(0);
        a.Add(3);
        a.Add(2);
        a.Add(1);
        a.Add(4);
        a.Sort(delegate (int _soSau, int _soTruoc) // y.CompareTo(x) = -1 là dịch về phía sau
        {
            // -1 là dịch về phía trước
            if(_soSau > _soTruoc){
                return -1;
            }else if(_soSau == _soTruoc){
                return 0;
            }else{
                return 1;
            }
            // Debug.Log(y + " - " + x + " - " + y.CompareTo(x));
            // return y.CompareTo(x);
        });

        List<int> b = new List<int>();
        for (int i = 0; i < a.Count; i++)
        {
            b.Add(a[i]);
        }
        b.Add(5);
        b.Sort(delegate (int x, int y)
        {
            Debug.Log(x + " - " + y + " - " + x.CompareTo(y));
            return x.CompareTo(y);
        });

        string _tmpA = string.Empty;
        for (int i = 0; i < a.Count; i++)
        {
            _tmpA += a[i] + "|";
        }
        Debug.Log(">>> " + _tmpA);

        string _tmpB = string.Empty;
        for (int i = 0; i < b.Count; i++)
        {
            _tmpB += b[i] + "|";
        }
        Debug.Log(">>> " + _tmpB);
    }
    [ContextMenu("BBBBB")]
    public void BBBBB(){
        // Debug.Log(System.DateTime.UtcNow + " - " + System.DateTime.Now);
        // long aaa = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
        // Debug.Log(new System.DateTime(MyConstant.currentTimeMilliseconds+aaa, System.DateTimeKind.Utc).ToString()+"------>"+aaa);
        // Debug.Log(new System.DateTime(MyConstant.currentTimeMilliseconds-aaa, System.DateTimeKind.Utc).ToString()+"------>"+aaa);
        // Debug.Log(new System.DateTime(aaa-MyConstant.currentTimeMilliseconds, System.DateTimeKind.Utc).ToString()+"------>"+aaa);
        System.DateTime start = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
		System.DateTime date = start.AddMilliseconds(MyConstant.currentTimeMilliseconds).ToLocalTime();
        Debug.Log(date.ToString());
        // UserDataInGame b = DataManager.instance.userData.CastToUserDataInGame();
        // Debug.Log(b.nameShowInGame);

        System.TimeSpan date2 = System.DateTime.Now.AddMinutes(1).Subtract(System.DateTime.Now);
        Debug.Log(date2.TotalMilliseconds);
    }

    [ContextMenu("CCCCC")]
    public void CCCCC(){
        System.DateTime _d = System.DateTime.Now.AddDays(1);
        System.TimeSpan _s = _d - System.DateTime.Now;
        Debug.Log("<color=blue>" + _s.TotalHours + " - " + _s.TotalSeconds + "</color>");
        double _a = 5d;
        int _t = 2;
        double _delta = _a / _t;
        if(_delta < 2){
            Debug.Log("hehehe");
        } else if(_delta > 2){
            Debug.Log("hihihi");
        } else {
            Debug.Log("hahaha");
        }
        Debug.Log(_delta);
    }
    [ContextMenu("DDDDD")]
    public void DDDD(){
        Debug.Log(string.Format("{0:00}", 500));
        Debug.Log(string.Format("{0:00}", 5));

        string _dateTimeNow = System.DateTime.Now.ToString();
        System.DateTime _myDate = System.DateTime.Parse(_dateTimeNow);
        Debug.Log(">>> " + _dateTimeNow + " -- " + _myDate.Hour + " -- " + _myDate.Minute + " -- " + _myDate.Second);
    }

    #region Flow Game 
    void Start()
    {
        HideAllScreen();
        StartCoroutine(DoActionRun());
    }

    IEnumerator DoActionRun()
    {
        yield return StartCoroutine(DoActionFirstInit());
        canShowScene = true;
        yield return StartCoroutine(DoActionShow());
        MyAudioManager.instance.PlayMusic(myAudioInfo.bgm);
    }

    void HideAllScreen(){
        
    }

    IEnumerator DoActionFirstInit(){
        myCallbackManager = new Home_CallbackManager();
        
        if (myCurrentState != State.GetDataFromSplashScreen){
            yield break;
        }
        
        DataManager.instance.subServerData.CheckWhenLogin();
        DataManager.instance.miniGameData.CheckWhenLogin();
        DataManager.instance.achievementData.CheckWhenLogin();
        DataManager.instance.leaderboardData.CheckWhenLogin();
        DataManager.instance.dailyRewardData.CheckWhenLogin();
        DataManager.instance.IAPProductData.CheckWhenLogin();
        DataManager.instance.installAppData.CheckWhenLogin();
        DataManager.instance.purchaseReceiptData.CheckWhenLogin();
        DataManager.instance.subsidyData.CheckWhenLogin();

        yield return null;
    }

    IEnumerator DoActionShow(){
        if (myCurrentState == State.GetDataFromSplashScreen){
            currentScreen = listScreens.splashScreen;
            // StartCoroutine(DoActionGetFirstDataOnAndroid());

            // #if UNITY_ANDROID
            // StartCoroutine(DoActionGetFirstDataOnAndroid());
            // #elif UNITY_IOS
            // StartCoroutine(DoActionGetFirstDataOnIOS());
            // #else
            // StartCoroutine(DoActionGetFirstDataOnAndroid());
            // #endif
        }
        else if (myCurrentState == State.BackFromGamePlayToChooseTableScreen){
            currentScreen = listScreens.chooseTableScreen;
            if (DataManager.instance.miniGameData.currentMiniGameDetail == null) {
                Debug.LogError ("currentMiniGameDetail is NULL");
            } else {
                DataManager.instance.miniGameData.currentMiniGameDetail.SortListServerDetailAgain();
            }
        }
        else if (myCurrentState == State.LogOutAndBackToLoginScreen){
            currentScreen = listScreens.loginScreen;
        }
        else{
            currentScreen = listScreens.chooseGameScreen;
        }

        currentScreen.InitData();
        currentScreen.Show();
        currentScreen.LateInitData();
        if (myCurrentState != State.GetDataFromSplashScreen){
            if(!DataManager.instance.getFirstDataSuccessfull){
                HomeManager.instance.TryToGetFirstData();
            }
        }
        myCurrentState = State.Unknown;
        yield return null;
    }

    public Coroutine TryToGetFirstData(){
        return StartCoroutine(DoActionTryToGetFirstData());
    }

    IEnumerator DoActionTryToGetFirstData(){
        while(!DataManager.instance.getFirstDataSuccessfull){
            yield return Yielders.Get(5f);
            yield return StartCoroutine(HomeManager.instance.DoActionGetFirstDataOnAndroid());
        }
    }

    public IEnumerator DoActionGetFirstDataOnAndroid(){
        #if TEST
        Debug.Log(">>> Get Sub Server Detail!");
		#endif
        bool _isFinished = false;
        yield return Yielders.EndOfFrame;
        OneHitAPI.GetSplashData_Android((short)DataManager.instance.currentLanguage, Application.identifier, (_mess, _error) =>
        {
            if (_mess != null) {
                long _featureVersion = _mess.readLong(); // nhỏ hơn giá trị này --> thông báo update
                #if TEST
                Debug.Log(">>> Version: " + _featureVersion + " - " + MyConstant.featureVersionCore);
				#endif
                if (MyConstant.featureVersionCore < _featureVersion){
                    DataManager.instance.haveNewVersion = true;
                }
                DataManager.instance.packageNameUpdate = _mess.readString(); // ==> setup vào myconstance.linkApp
                DataManager.instance.announcement = _mess.readString();

                // ------------- GoldDaily ------------- //
                InitGoldDailyData(_mess);
                // ------------------------------------- //

                // ------------- List IP For Onehit ------------- //
                InitListIPForOnehit(_mess);
                // ---------------------------------------------- //

                // ------------- List Sub Server ------------- //
                InitListSubServer(_mess);
                // ------------------------------------------ //

                // ------------- List IAP Product ------------- //
                InitIAPProductData(_mess);
                // ------------------------------------------ //
                DataManager.instance.getFirstDataSuccessfull = true;
            } else {
                #if TEST
                Debug.LogError(">>> TestGetSplashData_Android Error Code: " + _error);
                #endif
            }
            _isFinished = true;
        });

        yield return new WaitUntil(() => _isFinished);
    }

    public IEnumerator DoActionGetFirstDataOnIOS(){
        yield return null;
        Debug.LogError("Chưa làm");
    }

    // public Coroutine SetCheckAfterGetFirstData(){
    //     return StartCoroutine(DoActionCheckAfterGetFirstData());
    // }

    // IEnumerator DoActionCheckAfterGetFirstData(){
    //     IAPManager.instance.CheckWhenLogin();
    //     yield return Yielders.Get(1f);
    //     ShowAnnouncement();
    // }

    /*/IEnumerator DoActionGetFirstData(){
        #if TEST
        Debug.Log(">>> Get Sub Server Detail!");
		#endif
        bool _isFinished = false;
        bool _canTestSubSv = false;

        OneHitAPI.GetSubServerDetailAndData((_messageReceiving, _error) =>
        {
            if (_messageReceiving != null)
            {
                long _featureVersion = _messageReceiving.readLong(); // nhỏ hơn giá trị này --> thông báo update
				#if TEST
                Debug.Log(">>> Version: " + _featureVersion + " - " + MyConstant.featureVersionCore);
				#endif
                if (MyConstant.featureVersionCore < _featureVersion)
                {
                    DataManager.instance.haveNewVersion = true;
                }
                DataManager.instance.announcement = _messageReceiving.readString();

                // ------------- GoldDaily ------------- //
                sbyte _numberGoldDaily = _messageReceiving.readByte();
                List<RewardDetail> _tmpListRewards = new List<RewardDetail>();
                for (int i = 0; i < _numberGoldDaily; i++)
                {
                    long _goldDaily = _messageReceiving.readLong();
                    RewardDetail _reward = new RewardDetail(IItemInfo.ItemType.Gold, _goldDaily);
                    _tmpListRewards.Add(_reward);
                }
                DataManager.instance.dailyRewardData.listRewards = _tmpListRewards;
                // ------------------------------------- //

                // ------------- Leaderboard ------------- //
                long _currentMilisecondTimeUpdateTop = _messageReceiving.readLong();
                sbyte _numberTop = _messageReceiving.readByte();
                List<UserData> _listTopGold = new List<UserData>();
                for(int i = 0; i < _numberTop; i ++){
                    UserData _userData = new UserData();
                    _userData.InitData();
                    _userData.GetMoreUserData(_messageReceiving);
                    _listTopGold.Add(_userData);
                }
                System.DateTime _start = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
		        DataManager.instance.leaderboardData.lastTimeUpdateTop = _start.AddMilliseconds(_currentMilisecondTimeUpdateTop).ToLocalTime();
                DataManager.instance.leaderboardData.topGold = _listTopGold;
                DataManager.instance.leaderboardData.SortListTopGoldAgain();
                // --------------------------------------- //

                // ------------- List Sub Server ------------- //
                short _numberSubServer = _messageReceiving.readShort();
                List<SubServerDetail> _tmpListSubSv = new List<SubServerDetail>();
                if (_numberSubServer > 0){
                    for (int i = 0; i < _numberSubServer; i++){
                        SubServerDetail _tmpSubSvDetail = new SubServerDetail(_messageReceiving);
                        _tmpListSubSv.Add(_tmpSubSvDetail);
                    }
                }
                DataManager.instance.subServerData.LoadDataFromSv(_tmpListSubSv);
                // --------------------------------------- //

                // ------------- RoomDetail ------------- //
                List<TemporaryServerDetail> _tmpListTemporaryServerDetail = new List<TemporaryServerDetail>();
                short _numberRoomDetail = _messageReceiving.readShort();
                for(int i = 0; i < _numberRoomDetail; i ++){
                    TemporaryServerDetail _tmpServerDetail = new TemporaryServerDetail(_messageReceiving);
                    _tmpListTemporaryServerDetail.Add(_tmpServerDetail);
                }
                DataManager.instance.temporaryServerData.listDetail = _tmpListTemporaryServerDetail;

                for(int i = 0; i < DataManager.instance.miniGameData.listMiniGameDetail.Count; i++){
                    MiniGameDetail _gameDetail = DataManager.instance.miniGameData.listMiniGameDetail[i];
                    if(!_gameDetail.myInfo.isSubGame){
                        // _gameDetail.SetUpRoomDataAgain();
                    }
                }
                // --------------------------------------- //
                _canTestSubSv = true;
            }
            else
            {
                #if TEST
                Debug.LogError(">>> GetSubServerDetailAndData Error Code: " + _error);
                #endif
                
                // for(int i = 0; i < DataManager.instance.miniGameData.listMiniGameDetail.Count; i++){
                //     MiniGameDetail _gameDetail = DataManager.instance.miniGameData.listMiniGameDetail[i];
                //     if(!_gameDetail.myInfo.isSubGame
                //         && _gameDetail.roomData.listRoomServerDetail.Count == 0){
                //         _gameDetail.SetUpRoomDataAgain();
                //     }
                // }
            }
            _isFinished = true;
        });
        yield return new WaitUntil(() => _isFinished);

        if(_canTestSubSv){
            NetworkGlobal.instance.TestSub(DataManager.instance.subServerData.listSubServerDetail, ()=>{
                DataManager.instance.subServerData.SortListSubServerAgain();
                if(HomeManager.instance != null){
                    if (!string.IsNullOrEmpty(DataManager.instance.announcement)){
                        List<string> _listAnnouncement = new List<string>();
                        _listAnnouncement.Add(DataManager.instance.announcement);
                        HomeManager.instance.announcement.InitData(_listAnnouncement);
                        HomeManager.instance.announcement.Show();
                    }
                    if (DataManager.instance.userData.userId != 0){
                        HomeManager.instance.LoadDataGoldGemFromServer();
                    }
                }
                IAPManager.instance.CheckWhenLogin();
            });
        }else{
            DataManager.instance.subServerData.SortListSubServerAgain();
            if(CoreGameManager.instance.currentSceneManager != null
                && CoreGameManager.instance.currentSceneManager.mySceneType == IMySceneManager.Type.Home){
                if (!string.IsNullOrEmpty(DataManager.instance.announcement)){
                    List<string> _listAnnouncement = new List<string>();
                    _listAnnouncement.Add(DataManager.instance.announcement);
                    announcement.InitData(_listAnnouncement);
                    announcement.Show();
                }
            }
        }
    }*/

    void InitGoldDailyData(MessageReceiving _mess){
        sbyte _numberGoldDaily = _mess.readByte();
        List<RewardDetail> _tmpListRewards = new List<RewardDetail>();
        for (int i = 0; i < _numberGoldDaily; i++)
        {
            long _goldDaily = _mess.readLong();
            RewardDetail _reward = new RewardDetail(IItemInfo.ItemType.Gold, _goldDaily);
            _tmpListRewards.Add(_reward);
        }
        DataManager.instance.dailyRewardData.listRewards = _tmpListRewards;
    }

    void InitListIPForOnehit(MessageReceiving _mess){
        short _numberServerOnehit = _mess.readShort();
        List<IpDetail> _newListIpDetailForOnehit = new List<IpDetail>();
        IpDetail _tmpIpDetail = null;
        for(int i = 0; i < _numberServerOnehit; i ++){
            _tmpIpDetail = new IpDetail();
            _tmpIpDetail.ipId = _mess.readInt();
            _tmpIpDetail.ip = _mess.readString();
            _tmpIpDetail.port_onehit = _mess.readInt();
            _newListIpDetailForOnehit.Add(_tmpIpDetail);
        }
        DataManager.instance.subServerData.LoadListIpForOneHitDataFromSv(_newListIpDetailForOnehit);
    }

    void InitListSubServer(MessageReceiving _mess){
        short _numberSubServer = _mess.readShort();
        List<SubServerDetail> _tmpListSubSv = new List<SubServerDetail>();
        if (_numberSubServer > 0){
            SubServerDetail _tmpSubSvDetail = null;
            for (int i = 0; i < _numberSubServer; i++){
                _tmpSubSvDetail = new SubServerDetail(_mess);
                _tmpListSubSv.Add(_tmpSubSvDetail);
            }
        }
        DataManager.instance.subServerData.LoadSubServerDataFromSv(_tmpListSubSv);
    }

    void InitIAPProductData(MessageReceiving _mess){
        short _numberAndroidGoldPrice = _mess.readShort();
        List<IAPProductDetail> _tmpListProductDetail = new List<IAPProductDetail>();
        if(_numberAndroidGoldPrice > 0){
            for(int i = 0; i < _numberAndroidGoldPrice; i++){
                IAPProductDetail _productDetail = new IAPProductDetail(_mess);
                _tmpListProductDetail.Add(_productDetail);
            }
        }
        DataManager.instance.IAPProductData.LoadSubServerDataFromSv(_tmpListProductDetail);

        IAPManager.instance.InitializePurchasing(true);
    }

    public void LoadDataGoldGemFromServer(){
        OneHitAPI.GetGoldGemById(DataManager.instance.userData.databaseId, DataManager.instance.userData.userId,
        (_messageReceiving, _error) => {
            if (_messageReceiving != null){
                DataManager.instance.userData.gold = _messageReceiving.readLong();
                #if TEST
                Debug.Log(">>>Recieving Data GetGoldGemById: Gold: " + DataManager.instance.userData.gold);
                #endif
                if(HomeManager.instance != null){
                    if(HomeManager.instance.myCallbackManager != null
                        && HomeManager.instance.myCallbackManager.onLoadDataGoldGemFinished != null){
                        HomeManager.instance.myCallbackManager.onLoadDataGoldGemFinished();
                    }
                    HomeManager.instance.CheckShowRemindRating();
                }
                HomeManager.getGoldAndGemInfoAgain = false;
            }else{
                #if TEST
                Debug.LogError("Lỗi GetGoldGemById. Error code: " + _error);
                #endif
            }
        });
    }

    public void LoadEmailInfoFromServer(){
        if(DataManager.instance.userData.databaseId != UserData.DatabaseType.DATABASEID_BIGXU
			|| !string.IsNullOrEmpty(DataManager.instance.userData.emailShow)){
            getEmailInfoAgain = false;
            return;
        }

        OneHitAPI.BigxuAccount_GetEmailSecurity((_messageReceiving, _error)=>{
            if(_messageReceiving != null){
                sbyte _caseCheck = _messageReceiving.readByte();
                if(_caseCheck == 1){
                    string _emailShow = _messageReceiving.readString();
                    DataManager.instance.userData.emailShow = _emailShow;

                    #if TEST
                    Debug.Log(">>> GetEmailSecurity thành công: " + _emailShow);
                    #endif

                    if(HomeManager.instance != null){
                        if(HomeManager.instance.myCallbackManager != null
                            && HomeManager.instance.myCallbackManager.onLoadEmailInfoFinished != null){
                            HomeManager.instance.myCallbackManager.onLoadEmailInfoFinished();
                        }
                    }

                    HomeManager.getEmailInfoAgain = false;
                }else{
                    #if TEST
                    Debug.LogError (">>> GetEmailSecurity thất bại (0): " + _caseCheck);
                    #endif
                }
            }else{
                #if TEST
                Debug.LogError (">>> GetEmailSecurity thất bại (1): " + _error);
                #endif
            }
        });
    }

    public void ShowAnnouncement(){
        if (!string.IsNullOrEmpty(DataManager.instance.announcement)){
            List<string> _listAnnouncement = new List<string>();
            _listAnnouncement.Add(DataManager.instance.announcement);
            HomeManager.instance.announcement.InitData(_listAnnouncement);
            HomeManager.instance.announcement.Show();
        }
    }

    public void CheckShowRemindRating(){
        if(SceneLoaderManager.instance.currentState == SceneLoaderManager.State.Show){
            return;
        }
        if(DataManager.instance.hadRatingBefore){
            // Debug.LogError("1111111");
			return;
		}
        if(DataManager.instance.remindRating_NextTimeCanShowPopup > System.DateTime.Now){
            // Debug.LogError("2222222");
            return;
        }
        if(DataManager.instance.remindRating_countTimePressOkOnPopup >= 2){
            // Debug.LogError("33333333");
            return;
        }
        if(DataManager.instance.remindRating_countTimeShowPopup >= 5){
            // Debug.LogError("4444444");
            return;
        }
        if(DataManager.instance.remindRating_GoldUserCatched < 1000){
            // Debug.LogError("5555555");
            return;
        }
        double _tmp = DataManager.instance.userData.gold / DataManager.instance.remindRating_GoldUserCatched;
        if(_tmp < 2){
            // Debug.LogError("6666666");
            return;
        }
        PopupManager.Instance.CreatePopupRemindRating(
            ()=>{
                DataManager.instance.remindRating_countTimePressOkOnPopup++;
                DataManager.instance.remindRating_countTimeShowPopup++;
                DataManager.instance.remindRating_NextTimeCanShowPopup = System.DateTime.Now.AddSeconds(10);
                if(DataManager.instance.remindRating_countTimePressOkOnPopup >= 2){
                    DataManager.instance.hadRatingBefore = true;
                }
                Application.OpenURL(MyConstant.linkApp);
            }
            ,()=>{
                DataManager.instance.remindRating_countTimeShowPopup++;
                DataManager.instance.remindRating_NextTimeCanShowPopup = System.DateTime.Now.AddSeconds(10);
            }
        );
    }
    #endregion

    #region Change Screen
    public void ChangeScreen(UIHomeScreenController.UIType _typeScreen, bool _hideOldSubScreen = true)
    {
        if (_typeScreen == UIHomeScreenController.UIType.Unknown)
        {
#if TEST
            Debug.LogError("Chưa Setup typeScreen!");
#endif
            return;
        }
        UIHomeScreenController _tmpScreen = GetScreen(_typeScreen);
        if (_tmpScreen == null)
        {
            return;
        }
        if (_tmpScreen.currentState != UIHomeScreenController.State.Show)
        {
            _tmpScreen.InitData();
        }
        else
        {
            _tmpScreen.RefreshData();
        }
        if (_tmpScreen.isSubScreen)
        {
            if (_tmpScreen.myLastType == UIHomeScreenController.UIType.Unknown)
            {
                _tmpScreen.myLastType = currentScreen.myType;
            }
            if (currentScreen.isSubScreen && _hideOldSubScreen)
            {
                currentScreen.Hide();
            }
            currentScreen = _tmpScreen;
        }
        else
        {
            currentScreen.Hide();
            currentScreen = _tmpScreen;
        }
        currentScreen.transform.SetAsLastSibling();
        if (currentScreen.currentState != UIHomeScreenController.State.Show)
        {
            currentScreen.Show();
            currentScreen.LateInitData();
        }
    }

    UIHomeScreenController GetScreen(UIHomeScreenController.UIType _typeScreen)
    {
        switch (_typeScreen)
        {
            case UIHomeScreenController.UIType.LoginScreen:
                return listScreens.loginScreen;
            case UIHomeScreenController.UIType.RegisterScreen:
                return listScreens.registerScreen;
            case UIHomeScreenController.UIType.ChooseGame:
                return listScreens.chooseGameScreen;
            case UIHomeScreenController.UIType.ChooseTable:
                return listScreens.chooseTableScreen;
            case UIHomeScreenController.UIType.SettingScreen:
                return listScreens.settingScreen;
            case UIHomeScreenController.UIType.UserDetail:
                return listScreens.userDetailScreen;
            case UIHomeScreenController.UIType.Leaderboard:
                return listScreens.leaderboardScreen;
            case UIHomeScreenController.UIType.GetGold:
                return listScreens.getGoldScreen;
            case UIHomeScreenController.UIType.SubGame:
                return listScreens.subGame;
            case UIHomeScreenController.UIType.LuckyWheel:
                return listScreens.luckyWheel;
        }
        Debug.LogError("NULL Screen: " + _typeScreen.ToString());
        return null;
    }
    #endregion

    /*public IEnumerator DoActionAskForUpdateVersion()
    {
        bool _tmpFinished = false;
        if (DataManager.instance.forcedUpdate)
        {
            PopupManager.Instance.CreatePopupDialog(MyLocalize.GetString(MyLocalize.kWarning), MyLocalize.GetString(MyLocalize.kForcedUpdate), string.Empty, MyLocalize.GetString(MyLocalize.kQuit), MyLocalize.GetString(MyLocalize.kUpdate), () =>
            {
                //TODO : xử lý khi bấm nút update
				Application.OpenURL(MyConstant.linkApp);
            }, () =>
            {
                Application.Quit();
            });
            yield break;
        }
        else if (DataManager.instance.haveNewVersion)
        {
            PopupManager.Instance.CreatePopupMessage(MyLocalize.GetString(MyLocalize.kWarning), MyLocalize.GetString(MyLocalize.kHasNewVersion), string.Empty, MyLocalize.GetString(MyLocalize.kOk), () =>
            {
                DataManager.instance.haveNewVersion = false;
                _tmpFinished = true;
            });
        }
        else
        {
            _tmpFinished = true;
        }
        yield return new WaitUntil(() => _tmpFinished);
    }*/

    /*public IEnumerator DoActionGetListServersByGameId(System.Action _onFinished = null)
    {
        long _tmpTime = CoreGameManager.instance.gameInfomation.otherInfo.timeToGetListServerByGameId;
        bool _isFinished = false;
        for (int i = 0; i < DataManager.instance.miniGameData.listMiniGameDetail.Count; i++)
        {   
            if(!DataManager.instance.miniGameData.listMiniGameDetail[i].myInfo.isSubGame
                && !DataManager.instance.miniGameData.listMiniGameDetail[i].hadLoadFirstData){
                if (!DataManager.instance.miniGameData.listMiniGameDetail[i].isChecking
                    && (DataManager.instance.miniGameData.listMiniGameDetail[i].lastTimeGetData == -1 || MyConstant.currentTimeMilliseconds - DataManager.instance.miniGameData.listMiniGameDetail[i].lastTimeGetData >= _tmpTime))
                {
                    _isFinished = false;
                    GetListServerByGameId(DataManager.instance.miniGameData.listMiniGameDetail[i], () =>
                    {
                        _isFinished = true;
                    });
                    yield return new WaitUntil(() => _isFinished);
                    yield return Yielders.Get(1f);
                }
            }
        }
		if(_onFinished != null){
			_onFinished();
		}
    }*/

    // public void GetListServerByGameId(MiniGameDetail _gameDetail, System.Action _onFinished = null)
    // {
// 		#if TEST
// 		Debug.Log (">>> GetListServerByGameId " + _gameDetail.gameType);
// 		#endif
//         List<RoomServerDetail> _listRoomDetail = null;
//         RoomServerDetail _roomDetail = null;
//         _gameDetail.isChecking = true;
//         OneHitAPI.GetListServerByGameId(_gameDetail.myInfo.gameId, (_messageReceiving, _errorCode) =>
//         {
//             if (_messageReceiving != null)
//             {
//                 short _numberServer = _messageReceiving.readShort();
//                 if (_listRoomDetail == null)
//                 {
//                     _listRoomDetail = new List<RoomServerDetail>();
//                 }
//                 else
//                 {
//                     _listRoomDetail.Clear();
//                 }
//                 _roomDetail = null;

//                 for (int j = 0; j < _numberServer; j++)
//                 {
//                     _roomDetail = new RoomServerDetail(_messageReceiving);
//                     _listRoomDetail.Add(_roomDetail);
//                 }
                
//                 if(_listRoomDetail != null && _listRoomDetail.Count > 0){
//                     _gameDetail.roomData.listRoomServerDetail = _listRoomDetail;
//                     _gameDetail.SortListRoomAgain();
//                     _gameDetail.lastTimeGetData = MyConstant.currentTimeMilliseconds;

//                     _gameDetail.hadLoadFirstData = true;
//                 }
//             }
//             else
//             {
// #if TEST
//                 Debug.LogError(">>> GetListServerByGameId : " + _gameDetail.myInfo.gameId + " - errorCode: " + _errorCode);
// #endif
//             }
//             _gameDetail.isChecking = false;
//             if (_onFinished != null)
//             {
//                 _onFinished();
//             }
//         });

    // }

    private void OnDestroy() {
        instance = null;
    }
}

public class Home_CallbackManager{
    public System.Action onLoadDataGoldGemFinished;
    public System.Action onLoadEmailInfoFinished;
	public System.Action onDestructAllObject;
}

[System.Serializable] public class ListScreenInHome
{   
    [Header("Prefabs")]
    public GameObject splashScreenPrefab;
    public UIHomeScreenController splashScreen{
        get{
            if(_splashScreen == null){
                _splashScreen = LeanPool.Spawn(splashScreenPrefab, Vector3.zero, Quaternion.identity, HomeManager.instance.mainScreenHolder).GetComponent<UIHomeScreenController>();
            }  
            return _splashScreen;
        }
    }
    private UIHomeScreenController _splashScreen;

    public GameObject loginScreenPrefab;
    public UIHomeScreenController loginScreen{
        get{
            if(_loginScreen == null){
                _loginScreen = LeanPool.Spawn(loginScreenPrefab, Vector3.zero, Quaternion.identity, HomeManager.instance.mainScreenHolder).GetComponent<UIHomeScreenController>();
            }  
            return _loginScreen;
        }
    }
    private UIHomeScreenController _loginScreen;

    public GameObject registerScreenPrefab;
    public UIHomeScreenController registerScreen{
        get{
            if(_registerScreen == null){
                _registerScreen = LeanPool.Spawn(registerScreenPrefab, Vector3.zero, Quaternion.identity, HomeManager.instance.mainScreenHolder).GetComponent<UIHomeScreenController>();
            }  
            return _registerScreen;
        }
    }
    private UIHomeScreenController _registerScreen;

    public GameObject chooseGameScreenPrefab;
    public UIHomeScreenController chooseGameScreen{
        get{
            if(_chooseGameScreen == null){
                _chooseGameScreen = LeanPool.Spawn(chooseGameScreenPrefab, Vector3.zero, Quaternion.identity, HomeManager.instance.mainScreenHolder).GetComponent<UIHomeScreenController>();
            }  
            return _chooseGameScreen;
        }
    }
    private UIHomeScreenController _chooseGameScreen;

    public UIHomeScreenController settingScreen{
        get{
            return SettingScreenController.instance;
        }
    }

    public GameObject chooseTableScreenPrefab;
    public UIHomeScreenController chooseTableScreen{
        get{
            if(_chooseTableScreen == null){
                _chooseTableScreen = LeanPool.Spawn(chooseTableScreenPrefab, Vector3.zero, Quaternion.identity, HomeManager.instance.mainScreenHolder).GetComponent<UIHomeScreenController>();
            }  
            return _chooseTableScreen;
        }
    }
    private UIHomeScreenController _chooseTableScreen;

    public GameObject userDetailScreenPrefab;
    public UIHomeScreenController userDetailScreen{
        get{
            if(_userDetailScreen == null){
                _userDetailScreen = LeanPool.Spawn(userDetailScreenPrefab, Vector3.zero, Quaternion.identity, HomeManager.instance.subScreenHolder).GetComponent<UIHomeScreenController>();
            }  
            return _userDetailScreen;
        }
    }
    private UIHomeScreenController _userDetailScreen;

    public UIHomeScreenController getGoldScreen{
        get{
            return GetGoldScreenController.instance;
        }
    }

    public GameObject leaderboardScreenPrefab;
    public UIHomeScreenController leaderboardScreen{
        get{
            if(_leaderboardScreen == null){
                _leaderboardScreen = LeanPool.Spawn(leaderboardScreenPrefab, Vector3.zero, Quaternion.identity, HomeManager.instance.subScreenHolder).GetComponent<UIHomeScreenController>();
            }  
            return _leaderboardScreen;
        }
    }
    private UIHomeScreenController _leaderboardScreen;

    public UIHomeScreenController subGame{
        get{
            return ChooseSubGameScreenController.instance;
        }
    }

    public GameObject luckyWheelPrefab;
    public UIHomeScreenController luckyWheel{
        get{
            if(_luckyWheel == null){
                _luckyWheel = LeanPool.Spawn(luckyWheelPrefab, Vector3.zero, Quaternion.identity, HomeManager.instance.subScreenHolder).GetComponent<UIHomeScreenController>();
            }  
            return _luckyWheel;
        }
    }
    private UIHomeScreenController _luckyWheel;
}

[System.Serializable] public class Home_AudioInfo{

    [Header("Playback")]
    public AudioClip bgm;
}
