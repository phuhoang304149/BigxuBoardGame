using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Lean.Pool;

public class hero1_nami_controller : BOL_Hero_Controler {
	public GameObject ObjectMe;
	public Vector3 vectorMe;
	public GameObject begin_bullet;
	public float timeDelaySkill2 = 5f;
	Vector3 vectorBegin_attack1;
	Vector3 vectorBegin_attack2;
	Vector3 vectorBegin_attackE;

	float time_during_Attack2 = 10;

	public override void InitData() {
		base.InitData();
		if (chairPositions == Constant.CHAIR_LEFT) {
			vectorBegin_attack1 = new Vector3(-2.7f, 0.5f, 0);
			// ObjectMe = BOL_Skill_Controller.instance._tmpLeft;
			vectorMe = ObjectMe.transform.position;
			vectorBegin_attackE = new Vector3(-3, 0, 0);
		} else if (chairPositions == Constant.CHAIR_RIGHT) {
			vectorBegin_attack1 = new Vector3(2.7f, 0.5f, 0);
			// ObjectMe = BOL_Skill_Controller.instance._tmpRight;
			vectorMe = ObjectMe.transform.position;
			vectorBegin_attackE = new Vector3(3, 0, 0);
		}
	}
	public override void Attack1() {
		base.Attack1();
		animatorHero.SetTrigger(Constant.attack1);
		Delay(0.7f, () => {
			GameObject hero_bullet = CreateObjectPool(listAnimationBullet[0].gameObject, begin_bullet.transform.position);
			LeanTween.move(hero_bullet, vector_competitor, 0.9f).setOnComplete(() => {
				SelfDestruction_Object_Pool(hero_bullet);
				Auto_SelfDestruction_Object_Pool(ListFXSkill[0].gameObject, vector_competitor, 3f);
				Delay(0.5f, ResetData);
			});
		});
	}
	public override void Attack2() {
		base.Attack2();
		animatorHero.SetTrigger(Constant.attack2);
		Delay(1, () => {
			GameObject animationSkill = CreateObjectPool(ListFXSkill[1].gameObject, ObjectMe.transform.position);
			animationSkill.transform.eulerAngles = new Vector3(90, 0, 0);
			SelfDestruction_Object_Pool(animationSkill, timeDelaySkill2);
			Delay(3, ResetData);
		});

	}
	public override void Attack_Q() {
		base.Attack_Q();
		animatorHero.SetTrigger(Constant.attackQ);
		Delay(1, () => {
			GameObject hero_bullet = CreateObjectPool(bulletPrefab, vectorBegin_attack1);
			GameObject animation_bullet = CreateObjectPool(listAnimationBullet[0], Vector3.zero, hero_bullet.transform);
			hero_bullet.GetComponent<SpriteRenderer>().sprite = listBullet[0];
			vectorBegin_attack1 = begin_bullet.transform.position;
			hero_bullet.transform.position = vectorBegin_attack1;
			LTBezierPath ltPath = new LTBezierPath(new Vector3[] { vectorBegin_attack1,
			Vector3.zero+new Vector3(0,vectorBegin_attack1.y+1),
			 Vector3.zero+new Vector3(0,vectorBegin_attack1.y+1) ,
			 vector_competitor });
			LeanTween.move(hero_bullet, ltPath.pts, 0.5f).setOnComplete(() => {
				SelfDestruction_Object_Pool(hero_bullet);
				SelfDestruction_Object_Pool(animation_bullet);
				Auto_SelfDestruction_Object_Pool(ListFXSkill[0].gameObject, vector_competitor, 0.5f);
				Delay(0.5f, ResetData);
			});
		});
	}
	public override void Attack_W() {
		base.Attack_W();
		animatorHero.SetTrigger(Constant.attackW);
		Delay(0.7f, () => {
			GameObject hero_bullet = CreateObjectPool(listAnimationBullet[1].gameObject, vectorBegin_attack1);
			vectorBegin_attack1 = begin_bullet.transform.position;
			hero_bullet.transform.position = vectorBegin_attack1;
			LTBezierPath ltPath = new LTBezierPath(new Vector3[] { vectorBegin_attack1,
			Vector3.zero+new Vector3(0,vectorBegin_attack1.y+1),
			Vector3.zero+new Vector3(0,vectorBegin_attack1.y+1),
			vector_competitor });
			LeanTween.move(hero_bullet, ltPath.pts, 1f).setOnComplete(() => {
				SelfDestruction_Object_Pool(hero_bullet);
				Auto_SelfDestruction_Object_Pool(ListFXSkill[2].gameObject, vector_competitor, 1f);
				Delay(0.5f, ResetData);
			});
		});

	}
	public override void Attack_E() {
		base.Attack_E();
		animatorHero.SetTrigger(Constant.attackE);
		//GameObject hero_bullet = CreateObjectPool(listAnimationBullet[2].gameObject, vectorBegin_attack1);
		//vectorBegin_attack1 = begin_bullet.transform.position;
		//hero_bullet.transform.position = vector_competitor;
		//_AttackE(0.5f, new Vector3(0, -2, 0), 1.7f);
		//_AttackE(0.6f, new Vector3(0, -1, 0), 1.3f);
		//_AttackE(0.7f, new Vector3(0, 0, 0), 0.9f);
		//_AttackE(0.8f, new Vector3(0, 1, 0), 0.5f);
		//_AttackE(0.9f, new Vector3(0, 2, 0), 0.1f);
		Auto_SelfDestruction_Object_Pool(listAnimationBullet[2].gameObject, vector_competitor, 2, 4);
		Delay(3, ResetData);
	}
	public override void ResetData() {
		base.ResetData();
	}

	#region OFFLINE
	public override void Attack_1_Offline(Vector3 vectorHero) {
		animatorHero.SetTrigger(Constant.attack1);
		Delay(0.7f, () => {
			GameObject hero_bullet = CreateObjectPool(listAnimationBullet[0].gameObject, begin_bullet.transform.position);
			LeanTween.move(hero_bullet, vectorHero, 0.6f).setOnComplete(() => {
				SelfDestruction_Object_Pool(hero_bullet);
				Auto_SelfDestruction_Object_Pool(ListFXSkill[0].gameObject, vectorHero, 1f);
			});
		});
	}
	#endregion

}
