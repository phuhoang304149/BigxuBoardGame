using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class hero9_Kindred_controller : BOL_Hero_Controler {
	[Header(">>>>>>Private hero only<<<<<<")]
	public GameObject ObjectMe;
	Vector3 vectorMe;
	public GameObject BeginBullet;
	public GameObject LegObject;
	public GameObject wolfObject;
	public override void InitData() {
		base.InitData();
		if (chairPositions == Constant.CHAIR_LEFT) {
			vectorMe = ObjectMe.transform.position;
		} else if (chairPositions == Constant.CHAIR_RIGHT) {
			vectorMe = ObjectMe.transform.position;
		}
	}
	public override void Attack1() {
		base.Attack1();
		animatorHero.SetTrigger(Constant.attack1);
		Delay(0.8f, () => {
			GameObject bullet = CreateObjectPool(bulletPrefab, BeginBullet.transform.position);
			bullet.GetComponent<SpriteRenderer>().sprite = listBullet[0];
			bullet.transform.position = BeginBullet.transform.position;
			LeanTween.move(bullet, new Vector3(vector_competitor.x, BeginBullet.transform.position.y, 0), 0.3f).setOnComplete(() => {
				SelfDestruction_Object_Pool(bullet);
				Auto_SelfDestruction_Object_Pool(ListFXSkill[0].gameObject, vector_competitor, 0.2f, 1.5f);
				Delay(0.5f, ResetData);
			});
		});
	}
	public override void Attack2() {
		base.Attack2();
		animatorHero.SetTrigger(Constant.attack1);
		Delay(0.8f, () => {
			GameObject bullet = CreateObjectPool(bulletPrefab, BeginBullet.transform.position);
			bullet.GetComponent<SpriteRenderer>().sprite = listBullet[0];
			bullet.transform.position = BeginBullet.transform.position;
			LeanTween.move(bullet, new Vector3(vector_competitor.x, BeginBullet.transform.position.y, 0), 0.5f).setOnComplete(() => {
				SelfDestruction_Object_Pool(bullet);
				Auto_SelfDestruction_Object_Pool(ListFXSkill[0].gameObject, vector_competitor, 0.2f, 1.5f);
				Delay(0.5f, () => {
					animatorHero.SetTrigger(Constant.attack2);
					Delay(0.1f, () => {
						bullet = CreateObjectPool(bulletPrefab, wolfObject.transform.position);
						bullet.transform.localScale = new Vector3(2, 2, 2);
						bullet.GetComponent<SpriteRenderer>().sprite = listBullet[2];
						bullet.transform.eulerAngles = new Vector3(0, 0, -10);
						LeanTween.move(bullet, vector_competitor, 0.5f).setOnComplete(() => {
							SelfDestruction_Object_Pool(bullet);
							Auto_SelfDestruction_Object_Pool(ListFXSkill[1].gameObject, new Vector3(vector_competitor.x, vector_competitor.y - 0.5f), 0.2f, 1.5f);
							animatorHero.SetTrigger(Constant.idle);
							Delay(0.5f, ResetData);
						});
					});
				});
			});
		});
	}
	public override void Attack_Q() {
		base.Attack_Q();
		animatorHero.SetTrigger(Constant.attackQ);

		Delay(1.1f, () => {
			AttackQ(new Vector3(0, 0, 20), 2f);
			AttackQ(new Vector3(0, 0, 0), 0);
			AttackQ(new Vector3(0, 0, -20), -2f);
		});
		Delay(1.5f, ResetData);
	}
	public override void Attack_W() {
		base.Attack_W();
		animatorHero.SetTrigger(Constant.attackW);
		Auto_SelfDestruction_Object_Pool(ListFXSkill[2].gameObject, LegObject.transform.position - new Vector3(0, 0.2f, 0), 0.5f, 2f);
		Delay(2, () => {
			GameObject bullet = CreateObjectPool(bulletPrefab, wolfObject.transform.position);
			bullet.transform.localScale = new Vector3(2, 2, 2);
			bullet.GetComponent<SpriteRenderer>().sprite = listBullet[2];
			bullet.transform.eulerAngles = new Vector3(0, 0, -10);
			LeanTween.move(bullet, vector_competitor, 0.5f).setOnComplete(() => {
				SelfDestruction_Object_Pool(bullet);
				Auto_SelfDestruction_Object_Pool(ListFXSkill[4].gameObject, vector_competitor, 1.5f);
				Delay(0.5f, ResetData);
			});
		});
	}
	public override void Attack_E() {
		base.Attack_E();
		animatorHero.SetTrigger(Constant.attackE);
		Auto_SelfDestruction_Object_Pool(ListFXSkill[3].gameObject, LegObject.transform.position - new Vector3(0, 0.2f, 0), 2.5f);
		Delay(1.8f, () => {
			GameObject bullet = CreateObjectPool(bulletPrefab, BeginBullet.transform.position);
			bullet.GetComponent<SpriteRenderer>().sprite = listBullet[1];
			LeanTween.move(bullet, vector_competitor, 0.5f).setOnComplete(() => {
				SelfDestruction_Object_Pool(bullet);
				Auto_SelfDestruction_Object_Pool(ListFXSkill[1].gameObject, vector_competitor, 1.5f);
				Delay(0.5f, ResetData);
			});
		});

	}

	void AttackQ(Vector3 rotation, float hight) {
		GameObject bullet = CreateObjectPool(bulletPrefab, BeginBullet.transform.position);
		bullet.transform.localScale = new Vector3(1, 1, 1);
		bullet.transform.eulerAngles = rotation;
		bullet.transform.position = BeginBullet.transform.position;
		bullet.GetComponent<SpriteRenderer>().sprite = listBullet[0];
		LTBezierPath ltPath = new LTBezierPath(new Vector3[] {
			BeginBullet.transform.position,
			new Vector3(0,BeginBullet.transform.position.y+hight,BeginBullet.transform.position.z),
			new Vector3(0,BeginBullet.transform.position.y+hight,BeginBullet.transform.position.z),
			vector_competitor-new Vector3(hight/10,0,0)
			});
		LeanTween.move(bullet, ltPath.pts, 0.5f).setOnComplete(() => {
			SelfDestruction_Object_Pool(bullet);
			Auto_SelfDestruction_Object_Pool(ListFXSkill[0].gameObject, vector_competitor, 0.2f, 1.5f);
		});
		LeanTween.rotateZ(bullet, 0, 0.25f).setOnComplete(() => {
			LeanTween.rotateZ(bullet, -rotation.z, 0.25f);
		});
	}
}
