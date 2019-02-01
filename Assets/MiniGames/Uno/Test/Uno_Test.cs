using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Uno_Test : MonoBehaviour {

	public Uno_GamePlay_Manager uno_GamePlay_Manager;
	public List<PanelCardUnoDetailController> cardsUnoNeedToCheckDetail;
	public UnoGamePlayData.Uno_FinishGame_Data.Reason testReasonFinishGame;

	PanelCardUnoDetailController currentCardUno;
	IEnumerator actionTestCheckBaiUno;

	void Start(){
		
	}

	public void JoinBan(){
		StartCoroutine(DoActionJoinBan());
	}

	IEnumerator DoActionJoinBan(){
		UserDataInGame _userData = DataManager.instance.userData.CastToUserDataInGame();
		yield return null;
		Uno_PlayerGroup _myPlayerGroup = null;
		for(int i = 0; i < uno_GamePlay_Manager.listPlayerGroup.Count; i++){
			_myPlayerGroup = uno_GamePlay_Manager.listPlayerGroup[i];
			if(!_myPlayerGroup.isInitialized){
				_myPlayerGroup.InitData(_userData);
				LeanTween.scale(_myPlayerGroup.panelPlayerInfo.gameObject, Vector3.one * uno_GamePlay_Manager.UIManager.listPlaceHolderPanelPlayerInfo_Wating[_myPlayerGroup.realIndex].ratioScale, 0.2f)
					.setEase(LeanTweenType.easeOutBack);
				yield return Yielders.Get(0.2f);
			}
		}
	}

	public void LeftBan(){
		StartCoroutine(DoActionLeftBan());
	}

	IEnumerator DoActionLeftBan(){
		Uno_PlayerGroup _myPlayerGroup = null;
		for(int i = 0; i < uno_GamePlay_Manager.listPlayerGroup.Count; i++){
			_myPlayerGroup = uno_GamePlay_Manager.listPlayerGroup[i];
			if(_myPlayerGroup.isInitialized
				&& !_myPlayerGroup.isMe){
				_myPlayerGroup.HideAndClear();
				LeanTween.scale(_myPlayerGroup.panelPlayerInfo.gameObject, Vector3.one, 0.2f)
					.setEase(LeanTweenType.easeOutBack);
				yield return Yielders.Get(0.2f);
			}
		}
	}

	public void DealCard(){
		Uno_PlayerGroup _myPlayerGroup = null;
		for(int i = 0; i < uno_GamePlay_Manager.listPlayerGroup.Count; i++){
			_myPlayerGroup = uno_GamePlay_Manager.listPlayerGroup[i];
			if(_myPlayerGroup.isInitialized){
				uno_GamePlay_Manager.UIManager.DealPlayerCard(_myPlayerGroup, 0, 0.1f, null);
			}
		}
	}

	void OnFocusCard(PanelCardUnoDetailController _cardUnoDetail){
		if(currentCardUno != null){
			currentCardUno.MoveLocal(Vector2.zero, 0.2f, LeanTweenType.easeOutBack);
		}
		if(currentCardUno != _cardUnoDetail){
			_cardUnoDetail.MoveLocal(Vector2.up * 50f, 0.2f, LeanTweenType.easeOutBack);
			currentCardUno = _cardUnoDetail;
		}else{
			currentCardUno = null;
		}
	}

	public void TestCheckBaiUno(){
		if(cardsUnoNeedToCheckDetail == null || cardsUnoNeedToCheckDetail.Count == 0){
			return;
		}
		if(actionTestCheckBaiUno != null){
			StopCoroutine(actionTestCheckBaiUno);
		}
		actionTestCheckBaiUno = DoActionTestCheckBaiUno();
		StartCoroutine(actionTestCheckBaiUno);
	}
	
	IEnumerator DoActionTestCheckBaiUno(){
		while(true){
			for(int i = 0; i < uno_GamePlay_Manager.listCardDetail.Count; i ++){
				for(int j = 0; j < cardsUnoNeedToCheckDetail.Count; j++){
					cardsUnoNeedToCheckDetail[j].ShowNow(uno_GamePlay_Manager.listCardDetail[i].cardInfo, uno_GamePlay_Manager.listCardDetail[i].cardId);
				}
				yield return Yielders.Get(1f);
			}
			yield return null;
		}
	}

	public void CheckMyCardCanPutOrNot(){
		// if(uno_GamePlay_Manager.myPlayerGroup == null){
		// 	return;
		// }
		// for(int i = 0; i < uno_GamePlay_Manager.myPlayerGroup.myCards.Count; i++){
		// 	if(i % 2 == 0){
		// 		uno_GamePlay_Manager.myPlayerGroup.myCards[i].SetCanPut(true);
		// 		uno_GamePlay_Manager.myPlayerGroup.myCards[i].MoveLocal(Vector2.up * 50f, 0.2f, LeanTweenType.easeOutBack);
		// 	}else{
		// 		uno_GamePlay_Manager.myPlayerGroup.myCards[i].SetCanPut(false);
		// 		uno_GamePlay_Manager.myPlayerGroup.myCards[i].MoveLocal(Vector2.zero, 0.2f, LeanTweenType.easeOutBack);
		// 	}
		// }
	}

	public void TestFinishGame(){
		Uno_RealTimeAPI.instance.TestFinishGame((byte) testReasonFinishGame);
	}
	
	IEnumerator DoActionTestFinishGame(){
		if(uno_GamePlay_Manager.myPlayerGroup == null){
			yield break;
		}
		Uno_RealTimeAPI.instance.SendMessageStandUp();
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(Uno_Test))]
public class Uno_Test_Editor : Editor
{
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		Uno_Test myScript = (Uno_Test) target;
		
		GUILayout.Space(30);
		GUILayout.Label(">>> For Test <<<");

		if (GUILayout.Button ("Test Check Bai Uno")) {
			myScript.TestCheckBaiUno();
		}
		if (GUILayout.Button ("Test Join Ban")) {
			myScript.JoinBan();
		}
		if (GUILayout.Button ("Test Left Ban")) {
			myScript.LeftBan();
		}
		if (GUILayout.Button ("Test Move To Pos Playing")) {
			myScript.uno_GamePlay_Manager.UIManager.MoveAllToPosPlaying(false);
		}
		if (GUILayout.Button ("Chia Bai")) {
			myScript.DealCard();
		}
		if (GUILayout.Button ("Check Card Can Put Or Not")) {
			myScript.CheckMyCardCanPutOrNot();
		}
		if (GUILayout.Button ("Test Finish Game")) {
			myScript.TestFinishGame();
		}
	}
}
#endif