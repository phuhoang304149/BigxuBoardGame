using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using Lean.Pool;

public class BOL_Battle_Screen : MonoBehaviour {
	public static BOL_Battle_Screen instance {
		get {
			return ins;
		}
	}
	static BOL_Battle_Screen ins;
	[Header("Control Cammera")]
	public GameObject ObjectBOLBattle;
	public Camera mainCamera;
	public GameObject matrix;
	public GameObject _heroPlayer, _heroComp;
	GameObject herotmp1, herotmp2;
	public GameObject playerHeroPrefab, compHeroPrefab;
	public BOL_Hero_Controler playerControl, compControl;
	public bool finishInitData;
	public void Awake() {
		ins = this;
		float ratio = (1.5f / 2.812202f) * mainCamera.orthographicSize;
		matrix.transform.localScale = Vector3.one * ratio;

	}
	public void InitData() {
		herotmp1 = CreateHeroInGame(_heroPlayer, playerHeroPrefab, ObjectBOLBattle.transform);
		herotmp2 = CreateHeroInGame(_heroComp, compHeroPrefab, ObjectBOLBattle.transform);
		playerControl = herotmp1.transform.GetChild(0).GetComponent<BOL_Hero_Controler>();
		compControl = herotmp2.transform.GetChild(0).GetComponent<BOL_Hero_Controler>();
		finishInitData = true;
	}
    public void SelfDestruction(){
		LeanPool.Despawn(herotmp1);
		LeanPool.Despawn(herotmp2);
		playerControl = null;
		compControl = null;
    }
	public void Attack1() {
		playerControl.Attack_1_Offline(_heroComp.transform.position);
		Debugs.LogBlue("countPiece1Break" + MainControlArcade.instance.countPiece1Break);
		Debugs.LogBlue(" base health" + ArcadeGame.instance._baseHealthPlayer);
		if (MainControlArcade.instance.countPiece1Break >= 3) {
			int healthDefeat = MainControlArcade.instance.countPiece1Break * 100;
			//compControl.Attack_1_Offline();
			ArcadeGame.instance.StartTween(
			ArcadeGame.instance.TextShowHealthComp,
			ArcadeGame.instance._baseHealthComp,
			ArcadeGame.instance._baseHealthComp - healthDefeat
			);
			ArcadeGame.instance._baseHealthComp -= healthDefeat;
			MainControlArcade.instance.countPiece1Break = 0;
		}
	}
	public void Attack2() {
		playerControl.Attack_1_Offline(_heroComp.transform.position);
		if (MainControlArcade.instance.countPiece2Break >= 3) {
			int healthDefeat = MainControlArcade.instance.countPiece2Break * 100;
			//compControl.Attack_1_Offline();
			ArcadeGame.instance.StartTween(
			ArcadeGame.instance.TextShowHealthComp,
			ArcadeGame.instance._baseHealthComp,
			ArcadeGame.instance._baseHealthComp - healthDefeat
			);
			ArcadeGame.instance._baseHealthComp -= healthDefeat;
			MainControlArcade.instance.countPiece2Break = 0;
		}
	}
	public void Attack3() {
		playerControl.Attack_1_Offline(_heroComp.transform.position);
		if (MainControlArcade.instance.countPiece3break >= 3) {
			int healthDefeat = MainControlArcade.instance.countPiece3break * 100;
			//compControl.Attack_1_Offline();
			ArcadeGame.instance.StartTween(
			ArcadeGame.instance.TextShowHealthComp,
			ArcadeGame.instance._baseHealthComp,
			ArcadeGame.instance._baseHealthComp - healthDefeat
			);
			ArcadeGame.instance._baseHealthComp -= healthDefeat;
			MainControlArcade.instance.countPiece3break = 0;
		}
	}
	public void Attack4() {
		playerControl.Attack_1_Offline(_heroComp.transform.position);
		if (MainControlArcade.instance.countPiece4break >= 3) {
			int healthDefeat = MainControlArcade.instance.countPiece4break * 100;
			//compControl.Attack_1_Offline();
			ArcadeGame.instance.StartTween(
			ArcadeGame.instance.TextShowHealthComp,
			ArcadeGame.instance._baseHealthComp,
			ArcadeGame.instance._baseHealthComp - healthDefeat
			);
			ArcadeGame.instance._baseHealthComp -= healthDefeat;
			MainControlArcade.instance.countPiece4break = 0;
		}
	}
	public void Attack5() {
		playerControl.Attack_1_Offline(_heroComp.transform.position);
		if (MainControlArcade.instance.countPiece5break >= 3) {
			int healthDefeat = MainControlArcade.instance.countPiece5break * 100;
			//compControl.Attack_1_Offline();
			ArcadeGame.instance.StartTween(
			ArcadeGame.instance.TextShowHealthComp,
			ArcadeGame.instance._baseHealthComp,
			ArcadeGame.instance._baseHealthComp - healthDefeat
			);
			ArcadeGame.instance._baseHealthComp -= healthDefeat;
			MainControlArcade.instance.countPiece5break = 0;
		}
	}
	public void Attack6() {
		playerControl.Attack_1_Offline(_heroComp.transform.position);
		if (MainControlArcade.instance.countPiece6break >= 3) {
			int healthDefeat = MainControlArcade.instance.countPiece6break * 100;
			//compControl.Attack_1_Offline();
			ArcadeGame.instance.StartTween(
			ArcadeGame.instance.TextShowHealthComp,
			ArcadeGame.instance._baseHealthComp,
			ArcadeGame.instance._baseHealthComp - healthDefeat
			);
			ArcadeGame.instance._baseHealthComp -= healthDefeat;
			MainControlArcade.instance.countPiece6break = 0;
		}
	}

	public void CompAttack1() {
		MainControlArcade.instance.countObjectBreak = 0;
		compControl.Attack_1_Offline(_heroPlayer.transform.position);
		ArcadeGame.instance.StartTween(
		ArcadeGame.instance.TextShowHealth,
		ArcadeGame.instance._baseHealthPlayer,
		ArcadeGame.instance._baseHealthPlayer - 200
		);
		ArcadeGame.instance._baseHealthPlayer -= 200;

	}
	public void CompAttack2() {
	}
	public void CompAttack3() {
	}
	public void CompAttack4() {
	}
	public void CompAttack5() {
	}
	GameObject CreateHeroInGame(GameObject objectGame, GameObject objectPrefab, Transform parent = null) {
		Vector3 pos = objectGame.transform.position;
		Vector3 scale = objectGame.transform.localScale;
		Debugs.LogRed("pos" + pos + "scale" + scale);
		if (parent != null) {
			GameObject objecttmp = LeanPool.Spawn(objectPrefab, pos, Quaternion.identity, parent);
			objecttmp.transform.localScale = scale;
			return objecttmp;
		} else {
			GameObject objecttmp = LeanPool.Spawn(objectPrefab, pos, Quaternion.identity);
			objecttmp.transform.localScale = scale;
			return objecttmp;
		}
	}
	
}
