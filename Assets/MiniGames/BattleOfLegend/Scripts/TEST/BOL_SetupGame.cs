using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;
public class BOL_SetupGame : MonoBehaviour {
	public static BOL_SetupGame instance {
		get {
			return ins;
		}
	}
	static BOL_SetupGame ins;
	public BOL_SetupGame() { }
	[Header("Hero left")]
	public Text infoLeft;
	public GameObject hero_left;
	public BOL_Hero_Controler _hero_left;
	public GameObject ParentLeft;
	[Header("Hero right")]
	public Text infoRight;
	public GameObject hero_right;
	public BOL_Hero_Controler _hero_right;
	public GameObject ParentRight;

	[Header("Control game")]
	public int chairPosition;
	public List<DelayedAsset> ListHeroPrefab;
	GameObject heroTemp;
	public bool isStartGame;
	public GameObject prefabInfo;
	public GameObject prefab_win;
	public GameObject prefab_lose;
	[Header("Object scale screen")]
	public Camera mainCamera;
	public GameObject MainChoiceHero;
	public float ratioScreen;
	public sbyte STATE_STATUS_PLAYER;
	private void Awake() {
		ins = this;
	}
	void Start() {
		STATE_STATUS_PLAYER = (sbyte)Constant.CHAIR_LEAVE;
		ratioScreen = (1.2f / 2.81203f) * mainCamera.orthographicSize;
	}
	public void InitData() {
		ParentLeft = GameObject.FindWithTag("PlayerLeft");
		ParentRight = GameObject.FindWithTag("PlayerRight");
	}


	public GameObject CreateHeros(int chairPosition, int position, List<DelayedAsset> lstHero) {
		GameObject heroPos = (GameObject)lstHero[position].Load();
		if (chairPosition == Constant.CHAIR_LEFT) {
			GameObject herospawn = LeanPool.Spawn(heroPos, ParentLeft.transform.localPosition, Quaternion.identity);
			herospawn.transform.localScale = Vector3.one * 0.6f;
			return herospawn;
		} else {
			GameObject herospawn = LeanPool.Spawn(heroPos, ParentRight.transform.localPosition, Quaternion.Euler(new Vector3(0, 180, 0)));
			herospawn.transform.localScale = Vector3.one * 0.6f;
			return herospawn;
		}
	}
	public void SelfDestruction_hero(GameObject hero) {
		if (hero == null) {
			return;
		}
		LeanPool.Despawn(hero);
	}
	public void CreateHeroStartGame(int idleft, int idright) {
		SelfDestruction_hero(hero_left);
		SelfDestruction_hero(hero_right);
		hero_left = CreateHeros(Constant.CHAIR_LEFT, idleft, ListHeroPrefab);
		hero_right = CreateHeros(Constant.CHAIR_RIGHT, idright, ListHeroPrefab);
		_hero_left = hero_left.transform.GetChild(0).GetComponent<BOL_Hero_Controler>();
		_hero_left.chairPositions = Constant.CHAIR_LEFT;
		_hero_right = hero_right.transform.GetChild(0).GetComponent<BOL_Hero_Controler>();
		_hero_right.chairPositions = Constant.CHAIR_RIGHT;
		_hero_left.InitData();
		_hero_right.InitData();
		_hero_left.vector_competitor = _hero_right.myBody.transform.position;
		_hero_right.vector_competitor = _hero_left.myBody.transform.position;
		StartCoroutine(InitFirstDataStart());
	}
	public void DeSpawnHeroFinshgame() {
		SelfDestruction_hero(hero_left);
		SelfDestruction_hero(hero_right);
	}

	IEnumerator InitFirstDataStart() {
		yield return new WaitUntil(() => isStartGame);
		yield return Yielders.Get(1f);
#if TEST
		Debug.Log("start Game");
#endif
		_hero_left.InitData();
		_hero_right.InitData();
		_hero_left.vector_competitor = _hero_right.myBody.transform.position;
		_hero_right.vector_competitor = _hero_left.myBody.transform.position;
		BOL_PlaySkill_Controller.instance._Hero_left_ControlSkill = _hero_left;
		BOL_PlaySkill_Controller.instance._Hero_right_ControlSkill = _hero_right;

		//isStartGame = false;
	}
	IEnumerator DelayStartGame() {
		yield return Yielders.Get(30);
	}
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
	IEnumerator _Delay(float time, Action method) {
		yield return Yielders.Get(time);
		method();
	}
	void OnDestroy() {
		BolNetworkReceiving.SelfDestruction();
		ins = null;
	}
}
