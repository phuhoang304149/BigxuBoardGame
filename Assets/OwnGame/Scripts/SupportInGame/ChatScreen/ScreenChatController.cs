using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;
using EmojiUI;

public class ScreenChatController : MonoBehaviour
{
    public class ChatDetail{
        public bool isMe;
        public UserDataInGame userData;
        public string strMess;

        public ChatDetail(UserDataInGame _userData, string _strMess){
            userData = _userData;
            strMess = _strMess;
        }
    }

    public enum State{
        Show, Hide
    }
    public State currentState{get;set;}
    [SerializeField] CanvasGroup myCanvasGroup;
    [SerializeField] Canvas myCanvas;
    [SerializeField] RectTransform mainContent;
    [SerializeField] Transform panelChatContent;
    [SerializeField] EmojiTouchScreenInputField chatInput;

    [Header("Setting")]
    [SerializeField] float widthMainContent;
    [SerializeField] float widthBtnClose;
    [SerializeField] float timeTweenMainContent;
    [SerializeField] int maxChatContent;

    [Header("Prefab")]
    public GameObject panelChatDetailPrefab_others;
    public GameObject panelChatDetailPrefab_me;

    List<ChatDetail> listData;
    List<PanelChatDetailController> listPanelChatDetail;
    LTDescr myTweenMainContent, myTweenCanvasGroup;

    public System.Action<string> onSendMessage;
    public System.Action onHasNewMessage;
    public System.Action onStartShow;
    public System.Action onStartHide;

    void Awake()
    {
        listData = new List<ChatDetail>();
        listPanelChatDetail = new List<PanelChatDetailController>();

        Vector2 _pos = mainContent.offsetMin;
        _pos.x = 0 - widthMainContent - widthBtnClose;
        mainContent.offsetMin = _pos;

        _pos = mainContent.offsetMax;
        _pos.x = 0 - widthBtnClose;
        mainContent.offsetMax = _pos;

        chatInput.onEndEdit.AddListener(OnEndEditInputField);

        ResetData();
    }

    [ContextMenu("ResetData")]
    void ResetData()
    {
        currentState = State.Hide;
        LeanTween.cancel(gameObject);
        LeanTween.cancel(mainContent.gameObject);
        LeanTween.cancel(myCanvasGroup.gameObject);
        
        myTweenMainContent = null;
        myTweenCanvasGroup = null;

        myCanvasGroup.alpha = 0f;
        myCanvasGroup.blocksRaycasts = false;
        chatInput.text = string.Empty;

        if(listPanelChatDetail != null){
            for(int i = 0; i < listPanelChatDetail.Count; i++){
                listPanelChatDetail[i].onSelfDestruction = null;
                listPanelChatDetail[i].SelfDestruction();
            }
            listPanelChatDetail.Clear();
        }
    }

    public void SetSortingLayerInfoAgain(MySortingLayerInfo _sortingLayerInfo){
		myCanvas.sortingLayerName = _sortingLayerInfo.layerName.ToString();
		myCanvas.sortingOrder = _sortingLayerInfo.layerOrderId;
	}

    public void AddMessage(UserDataInGame _userData, string _strMess){
        if(listData == null){
            listData = new List<ChatDetail>();
        }
        if(listPanelChatDetail == null){
            listPanelChatDetail = new List<PanelChatDetailController>();
        }

        ChatDetail _tmpChatDetail = new ChatDetail(_userData, _strMess);

        if(_userData.IsEqual(DataManager.instance.userData.userId, DataManager.instance.userData.databaseId)){
             _tmpChatDetail.isMe = true;
        }else if(_userData.sessionId == DataManager.instance.userData.sessionId){
            _tmpChatDetail.isMe = true;
        }

        listData.Insert(0,_tmpChatDetail);
        
        if(listData.Count > maxChatContent && listData.Count > 0){
            listData.RemoveAt(listData.Count - 1);
        }

        if(currentState == State.Show){
            if(listPanelChatDetail.Count > maxChatContent && listPanelChatDetail.Count > 0){
                listPanelChatDetail[listPanelChatDetail.Count - 1].SelfDestruction();
            }
            AddPanelDetail(_tmpChatDetail);
        }else{
            if(onHasNewMessage != null){
                onHasNewMessage();
            }
        }
    }

    public void AddMessage(short _sessionId, string _strMess, List<UserDataInGame> _listOtherPlayerData){
        if(listData == null){
            listData = new List<ChatDetail>();
        }
        if(listPanelChatDetail == null){
            listPanelChatDetail = new List<PanelChatDetailController>();
        }

        ChatDetail _tmpChatDetail = null;

        if(_sessionId == DataManager.instance.userData.sessionId){
            _tmpChatDetail = new ChatDetail(DataManager.instance.userData.CastToUserDataInGame(), _strMess);
            _tmpChatDetail.isMe = true;
        }else{
            for(int i = 0; i < _listOtherPlayerData.Count; i ++){
                if(_listOtherPlayerData[i].IsEqual(_sessionId)){
                    _tmpChatDetail = new ChatDetail(_listOtherPlayerData[i].ShallowCopy(), _strMess);
                    break;
                }
            }
        }

        if(_tmpChatDetail == null){
            return;
        }
        
        listData.Insert(0,_tmpChatDetail);
        
        if(listData.Count > maxChatContent && listData.Count > 0){
            listData.RemoveAt(listData.Count - 1);
        }

        if(currentState == State.Show){
            if(listPanelChatDetail.Count > maxChatContent && listPanelChatDetail.Count > 0){
                listPanelChatDetail[listPanelChatDetail.Count - 1].SelfDestruction();
            }
            AddPanelDetail(_tmpChatDetail);
        }else{
            if(onHasNewMessage != null){
                onHasNewMessage();
            }
        }
    }

    void AddPanelDetail(ChatDetail _chatDetail){
        GameObject _prefab = null;
        if(_chatDetail.isMe){
            _prefab = panelChatDetailPrefab_me;
        }else{
            _prefab = panelChatDetailPrefab_others;
        }
        PanelChatDetailController _tmpPanelChatDetail = ((GameObject) LeanPool.Spawn(_prefab, Vector3.zero, Quaternion.identity, panelChatContent)).GetComponent<PanelChatDetailController>();
        _tmpPanelChatDetail.InitData(_chatDetail.userData, _chatDetail.strMess, (_p)=>{
            listPanelChatDetail.Remove((PanelChatDetailController)_p);
        });
        _tmpPanelChatDetail.Show();
        listPanelChatDetail.Insert(0, _tmpPanelChatDetail);
        _tmpPanelChatDetail.transform.SetAsLastSibling();
    }

    [ContextMenu("Show")]
    public void Show()
    {
        if(myCanvas.worldCamera == null && CoreGameManager.instance.currentSceneManager != null){
			myCanvas.worldCamera = CoreGameManager.instance.currentSceneManager.cameraForConsumableScreen.mainCamera;
		}else{
			myCanvas.worldCamera = Camera.main;
		}

        currentState = State.Show;
        MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_TogglePanel);

        for(int i = listData.Count - 1; i >= 0 && i < listData.Count; i --){
            AddPanelDetail(listData[i]);
        }

        myCanvasGroup.blocksRaycasts = true;

        if(onStartShow != null){
            onStartShow();
        }

        if (myTweenCanvasGroup != null)
        {
            LeanTween.cancel(myCanvasGroup.gameObject, myTweenCanvasGroup.uniqueId);
            myTweenCanvasGroup = null;
        }
        myTweenCanvasGroup = LeanTween.alphaCanvas(myCanvasGroup, 1f, 0.1f).setOnComplete(() => { myTweenCanvasGroup = null; });

        if (myTweenMainContent != null)
        {
            LeanTween.cancel(mainContent.gameObject);
            myTweenMainContent = null;
        }
        myTweenMainContent = LeanTween.moveX(mainContent, 0f, timeTweenMainContent).setEase(LeanTweenType.easeOutSine).setOnComplete(() => { myTweenMainContent = null; });
    }

    [ContextMenu("Hide")]
    public void Hide()
    {
        currentState = State.Hide;
        MyAudioManager.instance.PlaySfx(GameInformation.instance.globalAudioInfo.sfx_TogglePanel);

        if(onStartHide != null){
            onStartHide();
        }

        if (myTweenCanvasGroup != null)
        {
            LeanTween.cancel(myCanvasGroup.gameObject, myTweenCanvasGroup.uniqueId);
            myTweenCanvasGroup = null;
        }
        myTweenCanvasGroup = LeanTween.alphaCanvas(myCanvasGroup, 0f, timeTweenMainContent).setOnComplete(() => { 
            myTweenCanvasGroup = null; 
        });

        if (myTweenMainContent != null)
        {
            LeanTween.cancel(mainContent.gameObject);
            myTweenMainContent = null;
        }
        myTweenMainContent = LeanTween.moveX(mainContent, 0 - widthMainContent - widthBtnClose, timeTweenMainContent).setEase(LeanTweenType.easeOutSine).setOnComplete(() =>
        {
            myTweenMainContent = null;
            ResetData();
        });
    }

    public void OnEndEditInputField(string _arg0){
        if(!string.IsNullOrEmpty(_arg0)){
            if(onSendMessage != null){
                onSendMessage(_arg0);
            }
        }
        chatInput.text = string.Empty;
    }

    public void OnButtonSendChat(){
        if(!string.IsNullOrEmpty(chatInput.text)){
            if(onSendMessage != null){
                onSendMessage(chatInput.text);
            }
        }
        chatInput.text = string.Empty;
    }

    public void SelfDestruction(){
        if(listPanelChatDetail != null && listPanelChatDetail.Count > 0){
            for(int i = 0; i < listPanelChatDetail.Count; i++){
                listPanelChatDetail[i].onSelfDestruction = null;
                listPanelChatDetail[i].SelfDestruction();
            }
            listPanelChatDetail.Clear();
        }
    }
    
}