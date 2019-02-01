using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CoreGameManager : MonoBehaviour
{
    public static CoreGameManager instance
    {
        get
        {
            return ins;
        }
    }
    private static CoreGameManager ins;

    public GameInformation gameInfomation;

    public IMySceneManager currentSceneManager { get; set; }
    public ISubGamePlayManager currentSubGamePlay { get; set; }

    bool isPaused = false;

    [Header("For Test")]
    public bool giaLapNgatKetNoi;
    public bool giaLapMangChapChon;
    public float giaLapTyLeRotMang;

    void Awake()
    {
        if (ins != null && ins != this)
        {
            Destroy(this.gameObject);
            return;
        }
        ins = this;
        DontDestroyOnLoad(this.gameObject);

        Init();
    }

    void Start()
    {
#if UNITY_ANDROID
		StartCoroutine(DoActionCheckPressBackKey());
#endif
    }

    void Init()
    {
        DataManager.Init();
        FacebookAPI.Init();
        MyLocalize.InitData();
        LeanTween.init();
        #if UNITY_ANDROID
        onKeyBackClicked = new List<System.Action>();
        #endif
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    public void SetUpOutRoomAndBackToChooseTableScreen()
    {
        if (HomeManager.instance != null)
        {
            return;
        }
        NetworkGlobal.instance.StopRealTime();
        HomeManager.showAnnouncement = true;
        HomeManager.getGoldAndGemInfoAgain = true;
        if(DataManager.instance.miniGameData.currentSubGameDetail != null){
			DataManager.instance.miniGameData.currentSubGameDetail = null;
		}
        HomeManager.myCurrentState = HomeManager.State.BackFromGamePlayToChooseTableScreen;
        SceneLoaderManager.instance.LoadScene(MyConstant.SCENE_HOME);
    }

    public void SetUpOutRoomFromSubGamePlayAndBackToChooseGameScreen()
    {
        if (HomeManager.instance != null)
        {
            return;
        }
        NetworkGlobal.instance.StopRealTime();
        HomeManager.showAnnouncement = true;
        HomeManager.getGoldAndGemInfoAgain = true;
        if(DataManager.instance.miniGameData.currentSubGameDetail != null){
			DataManager.instance.miniGameData.currentSubGameDetail = null;
		}
        SceneLoaderManager.instance.LoadScene(MyConstant.SCENE_HOME);
    }

    void OnApplicationQuit()
    {
        DataManager.SaveData();
    }

    void OnApplicationFocus(bool _hasFocus)
    {
        isPaused = !_hasFocus;
        if (isPaused)
        {
            DataManager.SaveData();
        }
        // Debug.Log(">>> OnApplicationFocus: " + isPaused);
    }

    void OnApplicationPause(bool _pauseStatus)
    {
        isPaused = _pauseStatus;
        // Debug.Log(">>> OnApplicationPause: " + isPaused);
    }

    [ContextMenu("Clear All Data")]
    public void ClearAllData()
    {
        DataManager.ClearData();
        PlayerPrefs.DeleteAll();
        if(FacebookAPI.IsLoggedIn()){
            FacebookAPI.LogOut();
        }
    }

    #region Press back key
    public List<System.Action> onKeyBackClicked;
    bool canPressButtonEscape = true;
    System.DateTime nextTimeToPressBackKey;

    IEnumerator DoActionCheckPressBackKey()
    {
        nextTimeToPressBackKey = System.DateTime.Now;
        while (true)
        {
            yield return null;
            if (canPressButtonEscape)
            {
                if (System.DateTime.Now >= nextTimeToPressBackKey)
                {
                    if (Input.GetKeyUp(KeyCode.Escape))
                    {
                        if (onKeyBackClicked != null
                            && onKeyBackClicked.Count > 0
                            && LoadingCanvasController.instance.currentState == LoadingCanvasController.State.Hide)
                        {
                            canPressButtonEscape = false;
                            try
                            {
                                onKeyBackClicked[onKeyBackClicked.Count - 1]();
                            }
                            catch (System.Exception e)
                            {
#if TEST
									Debug.LogError(SceneManager.GetActiveScene().name + ": " + e.StackTrace);
#endif
                                RemoveCurrentCallbackPressBackKey();
                            }
                            yield return new WaitForSecondsRealtime(0.2f);
                            canPressButtonEscape = true;
                        }
                    }
                }
            }
        }
    }

    public void RegisterNewCallbackPressBackKey(System.Action _onKeyBackClicked, float _timeDelay = 1f)
    {
#if UNITY_ANDROID
		nextTimeToPressBackKey = System.DateTime.Now.AddMilliseconds((double)(_timeDelay*1000f));
		onKeyBackClicked.Add(_onKeyBackClicked);
#endif
    }

    public void RemoveCurrentCallbackPressBackKey(System.Action _onKeyBackClicked = null)
    {
#if UNITY_ANDROID
		if(onKeyBackClicked == null || onKeyBackClicked.Count <= 0){
			return;
		}
		if (_onKeyBackClicked != null) {
			onKeyBackClicked.Remove (_onKeyBackClicked);
		}
		else {
			onKeyBackClicked.RemoveAt(onKeyBackClicked.Count - 1);
		}
#endif
    }

    public void ClearAllCallbackPressBackKey()
    {
#if UNITY_ANDROID
		nextTimeToPressBackKey = System.DateTime.Now;
		if(onKeyBackClicked != null){
			onKeyBackClicked.Clear();
		}
#endif
    }
    #endregion

    #region Utilities
    public void DoVibrate()
    {
        if (!SystemInfo.supportsVibration)
        {
            return;
        }
        if (DataManager.instance.vibrationStatus != 1)
        {
            return;
        }
#if UNITY_ANDROID || UNITY_IOS
		Handheld.Vibrate();
#endif
    }

    public IEnumerator DoActionLoadAvatar(string _uri, float _w, float _h, System.Action<Texture2D> _onFinished){
        Texture2D _texture = null;
        if(string.IsNullOrEmpty(_uri)){
            if(_onFinished != null){
                _onFinished(_texture);
            }
            yield break;
        }
        // string _uri = "https://graph.facebook.com/" + _uri + "/picture?width=" + _w + "&height=" + _h;
        // Debug.Log(_uri);
        bool _isFinished = false;
        yield return StartCoroutine(MyConstant.DownloadIcon(_uri, (_t)=>{
            _texture = _t;
            _isFinished = true;
        }));
        yield return new WaitUntil(()=>_isFinished);
        if(_onFinished != null){
            _onFinished(_texture);
        }
    }
    #endregion
}

#if UNITY_EDITOR
[CustomEditor(typeof(CoreGameManager))]
public class CoreGameManager_Editor : Editor
{
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		CoreGameManager myScript = (CoreGameManager) target;
		
		GUILayout.Space(30);
		GUILayout.Label(">>> For Test <<<");

		if (GUILayout.Button ("Clear All Data")) {
			myScript.ClearAllData();
		}
	}
}
#endif