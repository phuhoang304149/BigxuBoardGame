using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable] public class Poker_PlayerGroup {

	public Transform panelContainerPlayerInfo;
	public PlaceHolderPanelOtherPlayerInfo placeHolderPanelPlayerInfo;
	public PanelPlayerInfoInGameController panelPlayerInfo{get;set;}
	public Poker_Panel_TxtPlayerStatus_Controller myPanelStatus;
	public Poker_PanelPlayerBetInfo_Controller myPanelBet;
	[SerializeField] Button buttonSitDown;
	[SerializeField] List<Transform> cardCoverHolders;
	[SerializeField] List<Transform> cardOpenHolders;
	[SerializeField] List<Transform> ownCardHolders;
	public List<TransformPlaceHolder> cardCoverHoldersCatched{get;set;}
	public List<TransformPlaceHolder> cardOpenHoldersCatched{get;set;}
	public List<TransformPlaceHolder> ownCardHoldersCatched{get;set;}
	public MySimplePoolManager ownCardPoolManager;

	[Header("Prefabs")]
	public GameObject panelPlayerInfoPrefab;
	
	public bool isInitialized{get;set;}
	public bool isMe{
		get{
			if(userData == null){
				return false;
			}
			if(userData.sessionId != DataManager.instance.userData.sessionId){
				return false;
			}
			return true;
		}
	}
	public UserDataInGame userData{
		get{
			return panelPlayerInfo.data;
		}
	}

	public void InitFirst(){
		isInitialized = false;
		CatchPlaceHolder();
		ownCardPoolManager = new MySimplePoolManager();
	}

	void CatchPlaceHolder(){
		cardCoverHoldersCatched = new List<TransformPlaceHolder>();
		for(int i = 0; i < cardCoverHolders.Count; i ++) {
			TransformPlaceHolder _placeHolderCatched = new TransformPlaceHolder(cardCoverHolders[i]);
			cardCoverHoldersCatched.Add(_placeHolderCatched);
		}
		cardCoverHolders.Clear();

		cardOpenHoldersCatched = new List<TransformPlaceHolder>();
		for(int i = 0; i < cardOpenHolders.Count; i ++) {
			TransformPlaceHolder _placeHolderCatched = new TransformPlaceHolder(cardOpenHolders[i]);
			cardOpenHoldersCatched.Add(_placeHolderCatched);
		}
		cardOpenHolders.Clear();

		ownCardHoldersCatched = new List<TransformPlaceHolder>();
		for(int i = 0; i < ownCardHolders.Count; i ++) {
			TransformPlaceHolder _placeHolderCatched = new TransformPlaceHolder(ownCardHolders[i]);
			ownCardHoldersCatched.Add(_placeHolderCatched);
		}
		ownCardHolders.Clear();

		panelPlayerInfo = GameObject.Instantiate(panelPlayerInfoPrefab, panelContainerPlayerInfo.transform, false).GetComponent<PanelPlayerInfoInGameController>();
		panelPlayerInfo.transform.position = placeHolderPanelPlayerInfo.transform.position;
		panelPlayerInfo.transform.localScale = Vector3.one * placeHolderPanelPlayerInfo.ratioScale;
		panelPlayerInfo.popupChatPosType = placeHolderPanelPlayerInfo.popupChatPosType;
	}

	public void InitData(UserDataInGame _userData){
		panelPlayerInfo.InitData(_userData);
		panelPlayerInfo.SetShadow(false);
		panelPlayerInfo.Show();
		myPanelStatus.Hide();
		myPanelBet.Hide();
		ownCardPoolManager.ClearAllObjectsNow();
		isInitialized = true;
	}

	public void InitAsIncognito(UserDataInGame _userData){
		panelPlayerInfo.InitAsIncognito(_userData);
		panelPlayerInfo.SetShadow(true);
		panelPlayerInfo.Show();
		myPanelStatus.Hide();
		myPanelBet.Hide();
		ownCardPoolManager.ClearAllObjectsNow();
		isInitialized = true;
	}

	public void ShowPlayerState(PokerGamePlayData.Poker_PlayerPlayingData.State _state, bool _isNow = false){
		string _content = string.Empty;
		switch(_state){
		case PokerGamePlayData.Poker_PlayerPlayingData.State.STATEPOKER_FOLD:
			_content = "FOLD";
			break;
		case PokerGamePlayData.Poker_PlayerPlayingData.State.STATEPOKER_CHECKING:
			_content = "CHECK";
			break;
		case PokerGamePlayData.Poker_PlayerPlayingData.State.STATEPOKER_CALL:
			_content = "CALL";
			break;
		case PokerGamePlayData.Poker_PlayerPlayingData.State.STATEPOKER_RAISE:
			_content = "RAISE";
			break;
		case PokerGamePlayData.Poker_PlayerPlayingData.State.STATEPOKER_ALLIN:
			_content = "ALL-IN";
			break;
		case PokerGamePlayData.Poker_PlayerPlayingData.State.None:
			myPanelStatus.Hide();
			panelPlayerInfo.SetShadow(false);
			return;
		default:
			#if TEST
			Debug.LogError("Chả có trạng thái gì: " +_state + "("+ (int) _state+")");
			#endif
			break;
		}

		if(!string.IsNullOrEmpty(_content)){
			myPanelStatus.Show(_content, _isNow);
			panelPlayerInfo.SetShadow(true);
		}else{
			myPanelStatus.Hide();
			panelPlayerInfo.SetShadow(false);
		}
	}

	public void HideAndClear(){
		panelPlayerInfo.Hide();
		myPanelBet.Hide();
		myPanelStatus.Hide();
		ClearAllCards();
		isInitialized = false;
	}

	public void ClearAllCards(){
		ownCardPoolManager.ClearAllObjectsNow();
	}

	public void ShowButtonSitDown(){
		buttonSitDown.gameObject.SetActive(true);
	}

	public void HideButtonSitDown(){
		buttonSitDown.gameObject.SetActive(false);
	}
}
