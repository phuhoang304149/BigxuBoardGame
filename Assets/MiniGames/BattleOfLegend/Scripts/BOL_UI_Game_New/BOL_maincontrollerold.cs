using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using UnityEngine.Serialization;
using System;
using UnityEditor;

public class BOL_maincontrollerold : MonoBehaviour {
	public static BOL_maincontrollerold instance {
		get {
			return ins;
		}
	}
	public static BOL_maincontrollerold ins;
	public List<BOL_HeroInfo> listHero;
	public string[] TypeStringPath = {
		"hero1_yasuo",
		"hero2_Ashe",
		"hero3_Leesin",
	   "hero4_Jhin",
	   "hero5_Zed",
	   "hero6_Talon",
	   "hero7_Jinx",
	   "hero8_Tristana",
	   "hero9_Lux",
	   "hero10_Nami",
	   "hero11_Kindred"
	};
	public const int WIN = 1;
	public const int LOSE = 0;

	public enum StateScene {
		unknow,
		waiting,
		playing,
		finish,

		showheroandskill,
		choicehero,
		showplayer,


		win,
		lose,
		tie

	}
	public StateScene stateScene {
		get {
			return StateScene.waiting;
		}
	}

	public StateScene stateWaiting {
		get {
			return StateScene.showplayer;
		}
	}

	public StateScene stateFinish {
		get {
			return StateScene.unknow;
		}
	}

	public Transform mainScreeenHolder;
	public BOL_ListStateScene listStateScene;
	public BOL_MySceneMain _BOL_ShowHeroAndSkill;
	public BOL_MySceneMain _BOL_ChoiceHero;
	public BOL_MySceneMain _BOL_ShowPlayer;
	public BOL_MySceneMain _BOL_PlayBattle;
	public BOL_MySceneMain _BOL_ShowFinishWin;
	public BOL_MySceneMain _BOL_ShowFinishLose;


	public GameObject chairLeft;
	public GameObject _chairLeftSpawn;
	public BOL_Hero_Controler _hero_left;
	public GameObject chairRight;
	public GameObject _chairRightSpawn;
	public BOL_Hero_Controler _hero_right;
	[Header("Choice Hero")]
	public MySimplePoolManager _myCVChoiceHero = new MySimplePoolManager(1);
	public DelayedAsset MainChoice;
	public GameObject _mainChoice;
	public BOL_ChoiceHero bOL_ChoiceHero;
	public int ChairPosition;
	[Header("ShowHeroAndButton")]
	public DelayedAsset ShowheroAndButton;
	public GameObject _showHeroAndButton;
	BOL_PlaySkill_Controller bOL_PlaySkill_Controller;
	[Header("Battle")]
	public MySimplePoolManager listBattle;
	public DelayedAsset MainBattle;
	public GameObject prefabInfo;
	public GameObject panelplayerLeft;
	public BOL_Battle_PlayerInGame _panelplayerLeft;
	public GameObject panelplayerRight;
	public BOL_Battle_PlayerInGame _panelplayerRight;
	[Header("Finish Game")]
	public GameObject objecFinish;
	public DelayedAsset objectWin;
	public DelayedAsset objectLose;
	[Header("ShowPlayerInGame")]
	public MySimplePoolManager _myCVShowPlayerInGame;
	public DelayedAsset objectShowHero;
	public GameObject _objectShowPlayer;
	public BOL_ShowPlayer_Manager _bolShowplayer;

	void Awake() {
		ins = this;
	}
	private void Start() {
		InitData(StateScene.waiting, StateScene.showplayer);
	}
	#region old
	// show hero and button skill;  
	public void InitDataShowHeroAndButton() {
#if TEST
		Debug.LogWarning(Debugs.ColorString("InitDataShowHeroAndButton", Color.red));
#endif
		if (_showHeroAndButton == null) {
			_showHeroAndButton = SpawnShowHero(); ;
			bOL_PlaySkill_Controller = _showHeroAndButton.GetComponent<BOL_PlaySkill_Controller>();
			bOL_PlaySkill_Controller.InitData();
		}
		//else {
		//  bOL_PlaySkill_Controller = _showHeroAndButton.GetComponent<BOL_PlaySkill_Controller>();
		//  bOL_PlaySkill_Controller.Initdata();
		//}
		chairLeft = GameObject.FindWithTag("PlayerLeft");
		chairRight = GameObject.FindWithTag("PlayerRight");
		bOL_PlaySkill_Controller.SetActivePanel(false);
	}
	public void ResetShowHeroAndButton() {
#if TEST
		Debug.LogWarning("ResetShowHeroAndButton");
#endif

		if (_chairLeftSpawn != null) {
			DespawnGameObject(_chairLeftSpawn);
		}
		_hero_left = null;
		if (_chairRightSpawn != null) {
			DespawnGameObject(_chairRightSpawn);
		}
		_hero_right = null;
		bOL_PlaySkill_Controller.ResetData();
		bOL_PlaySkill_Controller.InitData();

	}
	// show panel choice hero
	public void InitDataChoice() {
#if TEST
		Debug.LogWarning(Debugs.ColorString("InitDataChoice", Color.red));
#endif

		if (_mainChoice == null) {
			_mainChoice = SpawnMainChoice();
			bOL_ChoiceHero = _mainChoice.GetComponent<BOL_ChoiceHero>();
		} else {
			bOL_ChoiceHero = _mainChoice.GetComponent<BOL_ChoiceHero>();
		}
		Piece_Control piece = _mainChoice.GetComponent<Piece_Control>();
		_myCVChoiceHero.AddObject(piece);
		bOL_ChoiceHero.InitData();
		bOL_ChoiceHero.ResetData();
		if (panelplayerLeft != null) {
			DespawnGameObject(panelplayerLeft);
			_panelplayerLeft = null;
		}
		if (panelplayerRight != null) {
			DespawnGameObject(panelplayerRight);
			_panelplayerRight = null;
		}
		if (_objectShowPlayer == null) {
			bOL_ChoiceHero.SetActiveChoiceHero(true);
		} else {
			bOL_ChoiceHero.SetActiveChoiceHero(false);
		}
	}
	public void ResetChoiceHero(bool isDetroy = false) {
#if TEST
		Debug.LogWarning(Debugs.ColorString("ResetChoiceHero", Color.red));
#endif

		if (isDetroy) {
			Destroy(_mainChoice);
			_mainChoice = null;
			bOL_ChoiceHero = null;
			_myCVChoiceHero.ClearAllObjectsNow();
		} else {
			bOL_ChoiceHero.ResetData();
		}
	}
	// show matrix battle
	public void InitDataBattle() {
#if TEST
		Debug.LogWarning(Debugs.ColorString("InitDataBattle", Color.red));
#endif

		listBattle = new MySimplePoolManager();
		float ratio = 1.4f / 2.81203f;
		Debugs.LogRed("init battle");
		panelplayerLeft = SpawnHeroBattle(Constant.CHAIR_LEFT);
		_panelplayerLeft = panelplayerLeft.GetComponent<BOL_Battle_PlayerInGame>();
		//_panelplayerLeft.InitFirstData(1);
		Piece_Control pieceleft = _panelplayerLeft.GetComponent<Piece_Control>();
		panelplayerRight = SpawnHeroBattle(Constant.CHAIR_RIGHT);
		_panelplayerRight = panelplayerRight.GetComponent<BOL_Battle_PlayerInGame>();
		//_panelplayerRight.InitFirstData(-1);
		Piece_Control pieceRight = _panelplayerRight.GetComponent<Piece_Control>();
		listBattle.AddObject(pieceleft);
		listBattle.AddObject(pieceRight);
		ResetChoiceHero(true);
	}
	public void ResetBattle() {
#if TEST
		Debug.LogWarning(Debugs.ColorString("ResetBattle", Color.red));
#endif
		Destroy(panelplayerLeft);
		Destroy(panelplayerRight);
		panelplayerLeft = null;
		panelplayerRight = null;
		_panelplayerLeft = null;
		_panelplayerRight = null;

	}
	// show finish
	public void InitDataFInish(int styleFinishGame) {
#if TEST
		Debug.LogWarning(Debugs.ColorString("InitDataFInish", Color.red));
#endif

		if (styleFinishGame != Constant.CHAIR_VIEWER) {
			objecFinish = SpawnFinishGame(styleFinishGame);
			SetupFinishGame(objecFinish);
		}
		Delay(3, () => {
			ResetFinishGame();
			ResetBattle();
			Delay(1, () => {
				InitDataShowHeroAndButton();
				//BOL_Main_Controller.instance.SpawnHeroWhenChoice(chairId, characterid);
				//BOL_Main_Controller.instance.SpawnHeroWhenChoice(chairId, characterid);
				//InitDataShowPlayer();
			});
		});
	}
	public void ResetFinishGame() {
#if TEST
		Debug.LogWarning(Debugs.ColorString("ResetFinishGame", Color.red));
#endif

		if (objecFinish != null) {
			DespawnGameObject(objecFinish);
			objecFinish = null;
		}
	}

	public GameObject SpawnHeroBattle(int positon) {
		float ratitoscaele = BOL_Manager.instance.mainCamera.mainCamera.orthographicSize / 2.81203f;
		GameObject gameObjects = LeanPool.Spawn((GameObject)MainBattle.Load(), Vector3.zero, Quaternion.identity);
		gameObjects.transform.localScale = gameObjects.transform.localScale * ratitoscaele;
		if (positon == Constant.CHAIR_RIGHT) {
			gameObjects.transform.localScale = new Vector3(gameObjects.transform.localScale.x * (-1), gameObjects.transform.localScale.y);
		}
		return gameObjects;
	}
	public GameObject SpawnMainChoice() {
		GameObject gameObjects = LeanPool.Spawn((GameObject)MainChoice.Load());
		return gameObjects;
	}
	public GameObject SpawnShowHero() {
		GameObject gameObjects = LeanPool.Spawn((GameObject)ShowheroAndButton.Load());
		return gameObjects;
	}
	public GameObject SpawnShowPlayer() {
		GameObject gameObjects = LeanPool.Spawn((GameObject)objectShowHero.Load());
		return gameObjects;
	}
	public GameObject SpawnFinishGame(int styleFinish) {
		if (styleFinish == WIN) {
			GameObject gameObjects = LeanPool.Spawn((GameObject)objectWin.Load());
			return gameObjects;

		} else {
			GameObject gameObjects = LeanPool.Spawn((GameObject)objectLose.Load());
			return gameObjects;
		}
	}
	#endregion
	public void SetupFinishGame(GameObject objectfinish) {
		GameObject obFinish = LeanPool.Spawn(objectfinish, Vector3.zero, Quaternion.identity);
		LeanTween.rotateZ(obFinish.transform.GetChild(0).gameObject, 720, 5);
		LeanTween.rotateZ(obFinish.transform.GetChild(1).gameObject, 720, 5);
		LeanTween.scale(obFinish.transform.GetChild(2).gameObject, Vector3.one, 1);
		LeanTween.scale(obFinish.transform.GetChild(3).gameObject, Vector3.one * 0.8f, 1).setDelay(0.3f).setOnComplete(() => {
			LeanTween.scale(obFinish.transform.GetChild(3).gameObject, Vector3.one, 1).setOnComplete(() => {
				StartCoroutine(_Delay(4, () => {
					obFinish.transform.GetChild(0).transform.localPosition = Vector3.zero;
					obFinish.transform.GetChild(1).transform.localPosition = Vector3.zero;
					obFinish.transform.GetChild(2).transform.localScale = Vector3.zero;
					obFinish.transform.GetChild(3).transform.localScale = Vector3.zero; ;
					LeanPool.Despawn(obFinish);
				}));
			});
		});
	}
	public void DespawnGameObject(GameObject objecDespawn) {
		if (objecDespawn != null) {
			LeanPool.Despawn(objecDespawn);
		}
	}
	public void SpawnHeroWhenChoice(int chairID, int heroID) {
		Debug.LogWarning("heroID" + heroID);
		if (chairID == Constant.CHAIR_LEFT) {
			if (_chairLeftSpawn != null) {
				DespawnGameObject(_chairLeftSpawn);
				_chairLeftSpawn = null;
			}
			if (listHero[heroID].heroPrefab == null) {
				GameObject abc = Resources.Load("HeroPrefab/" + TypeStringPath[heroID]) as GameObject;
				listHero[heroID].heroPrefab = abc;
			}
			_chairLeftSpawn = LeanPool.Spawn(listHero[heroID].heroPrefab, chairLeft.transform.localPosition, Quaternion.identity);
			_chairLeftSpawn.transform.localScale = new Vector3(0.6f, 0.6f);
			_hero_left = _chairLeftSpawn.transform.GetChild(0).GetComponent<BOL_Hero_Controler>();
			_hero_left.chairPositions = Constant.CHAIR_LEFT;
			BOL_PlaySkill_Controller.instance._Hero_left_ControlSkill = _hero_left;
		} else if (chairID == Constant.CHAIR_RIGHT) {
			if (_chairRightSpawn != null) {
				DespawnGameObject(_chairRightSpawn);
				_chairRightSpawn = null;
			}
			if (listHero[heroID].heroPrefab == null) {
				GameObject abc = Resources.Load("HeroPrefab/" + TypeStringPath[heroID]) as GameObject;
				listHero[heroID].heroPrefab = abc;
			}
			_chairRightSpawn = LeanPool.Spawn(listHero[heroID].heroPrefab, chairRight.transform.localPosition, Quaternion.Euler(new Vector3(0, 180)));
			_chairRightSpawn.transform.localScale = new Vector3(0.6f, 0.6f);
			_hero_right = _chairRightSpawn.transform.GetChild(0).GetComponent<BOL_Hero_Controler>();
			_hero_right.chairPositions = Constant.CHAIR_RIGHT;
			BOL_PlaySkill_Controller.instance._Hero_right_ControlSkill = _hero_right;

		} else {
			//if (_chairRightSpawn != null) {
			//  DespawnGameObject(_chairRightSpawn);
			//  _chairRightSpawn = null;
			//}
			//if (_chairLeftSpawn != null) {
			//  DespawnGameObject(_chairLeftSpawn);
			//  _chairLeftSpawn = null;
			//}
		}
	}
	IEnumerator LoadHeroAfterLoadScene() {
		for (int i = 0; i < listHero.Count; i++) {
			var request = Resources.LoadAsync("HeroPrefab/" + TypeStringPath[i], typeof(GameObject));
			yield return request;
			//listHero[i].heroPrefab = Resources.Load("HeroPrefab/" + TypeStringPath[i]) as GameObject;
		}

	}
	public void OnDestroy() {
		ins = null;
	}
	public void onDestruction() {
		ins = null;
	}
	public void Delay(float time, Action method) {
		StartCoroutine(_Delay(time, method));
	}
	IEnumerator _Delay(float time, Action method) {
		yield return Yielders.Get(time);
		method();
	}


	void InitData(StateScene state, StateScene statusState = StateScene.unknow) {
		switch (state) {
			case StateScene.waiting:
				ActionWaiting(statusState);
				break;
			case StateScene.playing: break;
			case StateScene.finish: break;
		}
	}
	void ActionWaiting(StateScene stateStatus) {
		switch (stateStatus) {
			case StateScene.showheroandskill:
				_BOL_ShowHeroAndSkill = listStateScene.BOL_ShowHeroAndSkill;
				_BOL_ShowHeroAndSkill.Hide();
				break;
			case StateScene.showplayer:
				//_BOL_ShowPlayer = listStateScene.BOL_ShowPlayer;
				_BOL_ChoiceHero = listStateScene.BOL_ChoiceHero;
				_BOL_ShowPlayer.InitData();
				_BOL_ChoiceHero.Hide();
				break;
			case StateScene.choicehero:
				//_BOL_ShowPlayer = listStateScene.BOL_ShowPlayer;  
				_BOL_ChoiceHero = listStateScene.BOL_ChoiceHero;
				_BOL_ShowPlayer.SelfDestruction();
				_BOL_ChoiceHero.Show();
				break;

		}
	}
	void ActionFinish(StateScene state) {
		switch (state) {
			case StateScene.win: break;
			case StateScene.lose: break;
			case StateScene.tie: break;
			default:
#if TEST
				Debug.Log(Debugs.ColorString("không show cái gì cả", Color.red));
#endif
				break;
		}
	}
	public BOL_MySceneMain GetScreen(BOL_MySceneMain.UIScene _typeScreen) {
		switch (_typeScreen) {
			case BOL_MySceneMain.UIScene.ShowHeroAndSkill:
				return listStateScene.BOL_ShowHeroAndSkill;

		}
		Debug.LogError("NULL Screen: " + _typeScreen.ToString());
		return null;
	}
}
