using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class hero3_tristana_controller : BOL_Hero_Controler {
	public GameObject BulletBegin;
	public GameObject ObjectMe;
	public Vector3 vectorMe;
	public Vector3 vectorBegin;
	public Vector3 vectorBulletAttack2 = new Vector3(-3.3f, -0.15f, 0);
	public Vector3 vectorRotationBulletAttack2;
	public Vector3 rorateSkillQ;
	public Vector3 vectorBulletAttack_Q = new Vector3(-3.7f, -1.1f, 0);
	public override void InitData() {
		base.InitData();
		if (chairPositions == Constant.CHAIR_LEFT) {
			vectorBegin = new Vector3(-3.3f, -0.9f, 0);
			vectorMe = ObjectMe.transform.position;
			vectorRotationBulletAttack2 = new Vector3(0, 0, -10);
			rorateSkillQ = new Vector3(0, 90, 0);
		} else if (chairPositions == Constant.CHAIR_RIGHT) {
			vectorBegin = new Vector3(3.3f, -0.9f, 0);
			vectorMe = ObjectMe.transform.position;
			vectorRotationBulletAttack2 = new Vector3(0, 180, -10);
			rorateSkillQ = new Vector3(0, -90, 0);
		}
	}
	public override void Attack1() {
		base.Attack1();
		animatorHero.SetTrigger(Constant.attack1);
		animatorHero.SetTrigger(Constant.idle);
		GameObject hero_bullet = CreateObjectPool(ListFXSkill[0].gameObject, BulletBegin.transform.position);
		DelayObject(hero_bullet, 1.1f);
		LeanTween.alpha(hero_bullet, 1, 0.01f).setOnComplete(() => {
			LeanTween.move(hero_bullet, vector_competitor, 0.4f).setOnComplete(() => {
				SelfDestruction_Object_Pool(hero_bullet);
				Auto_SelfDestruction_Object_Pool(listAnimationBullet[0], vector_competitor, 1);
				ResetData();
			});
		}).setDelay(1.1f);
	}
	public override void Attack2() {
		base.Attack2();
		animatorHero.SetTrigger(Constant.attack2);
		animatorHero.SetTrigger(Constant.idle);
		HeroAttack2(2.2f, new Vector3(0, 0.1f, 0));
		HeroAttack2(1.9f, new Vector3(0, 0.3f, 0));
		HeroAttack2(1.8f, new Vector3(0, 0.4f, 0));
		HeroAttack2(1.5f, new Vector3(0, 0.7f, 0));
		HeroAttack2(1.4f, new Vector3(0, 0.9f, 0));
		HeroAttack2(1.2f, new Vector3(0, 0, 0));
		HeroAttack2(1.3f, new Vector3(0, -0.1f, 0));
		HeroAttack2(1.6f, new Vector3(0, -0.3f, 0));
		HeroAttack2(2.0f, new Vector3(0, -0.5f, 0));
		HeroAttack2(2.1f, new Vector3(0, -0.7f, 0));
		HeroAttack2(2.3f, new Vector3(0, -0.9f, 0), ResetData);

	}
	public override void Attack_Q() {
		base.Attack_Q();
		animatorHero.SetTrigger(Constant.attackQ);
		animatorHero.SetTrigger(Constant.idle);
		GameObject bullet = CreateObjectPool(bulletPrefab, BulletBegin.transform.position);
		bullet.GetComponent<SpriteRenderer>().sprite = listBullet[0];
		DelayObject(bullet, 0.8f);
		LeanTween.alpha(bullet, 1, 0.01f).setOnComplete(() => {
			bullet.transform.position = BulletBegin.transform.position;
			GameObject fxbullet = CreateObjectPool(listAnimationBullet[1].gameObject, Vector3.zero, bullet.transform);
			LTBezierPath ltPath = new LTBezierPath(new Vector3[] { BulletBegin.transform.position,
			new Vector3(0, BulletBegin.transform.position.y+1.5f, 0),
			new Vector3(0,  BulletBegin.transform.position.y+1.5f, 0),
			vector_competitor });
			LeanTween.move(bullet, ltPath.pts, 0.8f).setOnComplete(() => {
				SelfDestruction_Object_Pool(bullet);
				SelfDestruction_Object_Pool(fxbullet);
				Auto_SelfDestruction_Object_Pool(listAnimationBullet[2].gameObject, vector_competitor, 1);
				Auto_SelfDestruction_Object_Pool(listAnimationBullet[0].gameObject, vector_competitor, 1f, 0.7f);
				ResetData();
			});
		}).setDelay(0.8f);
	}
	public override void Attack_W() {
		base.Attack_W();
		animatorHero.SetTrigger(Constant.attackW);
		animatorHero.SetTrigger(Constant.idle);
		LTBezierPath ltPath = new LTBezierPath(new Vector3[] { ObjectMe.transform.position,
		new Vector3(0,ObjectMe.transform.position.y+ 1.5f, 0),
		new Vector3(0,ObjectMe.transform.position.y+ 1.5f, 0),
		vector_competitor
		});
		LeanTween.move(ObjectMe, ltPath.pts, 0.9f).setOnComplete(() => {
			ltPath = new LTBezierPath(new Vector3[] { ObjectMe.transform.position,
			new Vector3(0,ObjectMe.transform.position.y+1.5f, 0),
			new Vector3(0, ObjectMe.transform.position.y+1.5f, 0),
			new Vector3(-vector_competitor.x, vector_competitor.y) });
			LeanTween.move(ObjectMe, ltPath.pts, 0.7f).setOnComplete(() => {
				ResetData();
			});
		}).setDelay(0.3f);
	}
	public override void Attack_E() {
		base.Attack_E();
		animatorHero.SetTrigger(Constant.attackE);
		animatorHero.SetTrigger(Constant.idle);
		GameObject bullet = CreateObjectPool(listAnimationBullet[1].gameObject, BulletBegin.transform.position);
		bullet.transform.eulerAngles = new Vector3(0, 90, 0);
		GameObject fxskill = CreateObjectPool(ListFXSkill[0].gameObject, vector_competitor);
		fxskill.transform.localScale = new Vector3(2, 2);
		DelayObject(bullet, 1);
		DelayObject(fxskill, 1);
		LeanTween.alpha(bullet, 1, 0.01f).setOnComplete(() => {
			fxskill.transform.localScale = Vector3.one;
			SelfDestruction_Object_Pool(bullet);
			SelfDestruction_Object_Pool(fxskill);
			ResetData();
		}).setDelay(3);
	}
	public void HeroAttack2(float timeDelay, Vector3 vc, Action method = null) {
		GameObject hero_bullet = CreateObjectPool(ListFXSkill[0].gameObject, BulletBegin.transform.position);
		DelayObject(hero_bullet, timeDelay);
		LeanTween.scale(hero_bullet, new Vector3(1, 1, 1), 0.1f).setOnComplete(() => {
			hero_bullet.transform.position = BulletBegin.transform.position + vc;
			LeanTween.move(hero_bullet, vector_competitor, 0.2f).setOnComplete(() => {
				SelfDestruction_Object_Pool(hero_bullet);
				Auto_SelfDestruction_Object_Pool(listAnimationBullet[0].gameObject, vector_competitor, 0.5f);
				if (method != null) {
					method();
					method = null;
				}
			});
		}).setDelay(timeDelay - 0.1f);
		UpdatePositionCompetitor();
	}
	public override void ResetData() {
		base.ResetData();
	}
}
