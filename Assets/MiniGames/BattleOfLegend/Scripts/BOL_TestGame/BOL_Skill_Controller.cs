using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Lean.Pool;

public class BOL_Skill_Controller : MonoBehaviour {
	public static BOL_Skill_Controller instance {
		get {
			return ins;
		}
	}
	static BOL_Skill_Controller ins;
	private void Awake() {
		ins = this;
	}
	public const string move = "Move";
	Animator Animator_Left, Animator_Right;

	public Vector3 vectorBodyHeroLeft, vectorBodyHeroRight;

	public bool isStartGame = false;
	string[] listAnim = { "Idle", "Attack1", "Attack2", "AttackQ", "AttackW", "Attack_E" };
	public BOL_Hero_Controler _tmpHero_left;
	public BOL_Hero_Controler _tmpHero_right;
	public int heroleftchoice, herorightchoice;
	public List<GameObject> listHero;
	public GameObject _tmpLeft, _tmpRight;
	public Vector3 left_positon, right_position;
	public Button left_button1Choice;
	public Button right_button1Choice;
	public Camera mycamera;
	public GameObject objecMain;
	public GameObject objectSub;

	private void Start() {
		float ratio = (1.2f / 2.81203f) * mycamera.orthographicSize;
		objecMain.transform.localScale = Vector3.one * ratio;
		objectSub.GetComponent<RectTransform>().localScale = Vector3.one * ((0.8f / 2.81203f) * mycamera.orthographicSize);
		left_positon = _tmpLeft.transform.position;
		right_position = _tmpRight.transform.position;
		left_button1Choice.onClick.AddListener(() => {
			SelfDestruction_hero(_tmpLeft);
			_tmpLeft = CreateHero(listHero[heroleftchoice], Constant.CHAIR_LEFT);
			_tmpHero_left = _tmpLeft.transform.GetChild(0).GetComponent<BOL_Hero_Controler>();
			BOL_PlaySkill_Controller.instance._Hero_left_ControlSkill = _tmpHero_left;
			//_heroType_left = BOL_Hero_Controler.HeroType.hero1;
			//_tmpHero_left = GetHero(_heroType_left);
			_tmpHero_left.chairPositions = 2;
			Animator_Left = _tmpHero_left.animatorHero;
			_tmpHero_left.InitData();
		});
		right_button1Choice.onClick.AddListener(() => {
			SelfDestruction_hero(_tmpRight);
			_tmpRight = CreateHero(listHero[herorightchoice], Constant.CHAIR_RIGHT);
			_tmpHero_right = _tmpRight.transform.GetChild(0).GetComponent<BOL_Hero_Controler>();
			BOL_PlaySkill_Controller.instance._Hero_right_ControlSkill = _tmpHero_right;
			//_heroType_right = BOL_Hero_Controler.HeroType.hero1;
			//_tmpHero_right = GetHero(_heroType_right);
			_tmpHero_right.chairPositions = 4;
			Animator_Right = _tmpHero_right.animatorHero;
			_tmpHero_right.InitData();
		});
		StartCoroutine(DoActionInit());
	}
	public static void SelfDestruction() {
		ins = null;
	}
	IEnumerator DoActionInit() {
		yield return new WaitUntil(() => isStartGame);
#if TEST
		Debug.Log(isStartGame);
#endif

	}
	public void PlaySkill(int chairID, int skill) {
		switch (chairID) {
			case (int)Constant.CHAIR_LEFT:
				Animator_Left.SetInteger(move, skill);
				StartCoroutine(Reset_Skill(Animator_Left));
				break;
			case (int)Constant.CHAIR_RIGHT:
				Animator_Right.SetInteger(move, skill);
				StartCoroutine(Reset_Skill(Animator_Right));
				break;
		}
	}
	IEnumerator Reset_Skill(Animator animator) {
		yield return Yielders.Get(0.5f);
		animator.SetInteger(move, 0);
	}
	public GameObject CreateHero(GameObject prefab_hero, int postion_chair) {
		if (postion_chair == Constant.CHAIR_LEFT) {
			GameObject objecthero = LeanPool.Spawn(prefab_hero, left_positon, Quaternion.identity);
			objecthero.transform.localScale = Vector3.one * 0.8f;
			return objecthero;
		} else {
			GameObject objecthero = LeanPool.Spawn(prefab_hero, right_position, Quaternion.Euler(new Vector3(0, 180, 0)));
			objecthero.transform.localScale = Vector3.one * 0.8f;
			return objecthero;
		}
	}
	public void SelfDestruction_hero(GameObject hero) {
		if (hero == null || !hero.activeSelf) {
			return;
		}
		LeanPool.Despawn(hero);
	}
}
