using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anima2D;
using Lean.Pool;

public class hero2_talon_controller : BOL_Hero_Controler {

	[Header("only hero")]
	public GameObject objectHand;
	public Vector3 vector1 = new Vector3(2, 2, 0);
	public Vector3 vector2 = new Vector3(-2, 2, 0);
	public Vector3[] listvector = new Vector3[30];
	// Use this for initialization
	public override void InitData() {
		base.InitData();

	}
	public override void Attack1() {
		base.Attack1();
		//Addpositon();
		animatorHero.SetTrigger(Constant.attack1);
		animatorHero.SetTrigger(Constant.dash2);
		vectorMyBody = myBody.transform.position;
		LTBezierPath ltPath = new LTBezierPath(
		new Vector3[] { myBody.transform.position,
		vector1,
		vector2,
		vector_position_ememy});
		LeanTween.move(myBody, ltPath, 1).setOnComplete(() => {
			Auto_SelfDestruction_Object_Pool(ListFXSkill[0].gameObject, vector_competitor, 2f);
			//GameObject fxattack = LeanPool.Spawn();
			ltPath = new LTBezierPath(
			new Vector3[] { myBody.transform.position,
			vector2,
			vector1,
			new Vector3(vectorMyBody.x,vectorMyBody.y) });
			LeanTween.move(myBody, ltPath.pts, 1).setOnComplete(() => {
				ResetData();
			}).setDelay(0.5f);
		});
	}
	public override void Attack2() {
		base.Attack2();
		animatorHero.SetTrigger(Constant.dash1);
		animatorHero.SetTrigger(Constant.attack2);
		vectorMyBody = myBody.transform.position;
		//LTBezierPath ltPath = new LTBezierPath(
		//new Vector3[] { myBody.transform.position,
		//new Vector3(1.3f, myBody.transform.position.y+2.5f, 0),
		//new Vector3(-1.3f, myBody.transform.position.y+2.5f, 0),
		//vector_position_ememy});
		LTBezierPath ltPath = new LTBezierPath(
		 new Vector3[] {
		 myBody.transform.position,
		vector1,
		vector2,
		 vector_position_ememy});
		LeanTween.move(myBody, ltPath.pts, 1).setOnComplete(() => {
			Auto_SelfDestruction_Object_Pool(ListFXSkill[0].gameObject, vector_competitor, 1f);
			animatorHero.SetTrigger(Constant.dash2);
			ltPath = new LTBezierPath(
			new Vector3[] { myBody.transform.position,
			vector2,
			vector1,
			new Vector3(vectorMyBody.x,vectorMyBody.y) });
			LeanTween.move(myBody, ltPath.pts, 1).setOnComplete(() => {
				ResetData();
			}).setDelay(0.6f);
		}).setDelay(0.3f);
	}
	public override void Attack_Q() {
		base.Attack_Q();
		animatorHero.SetTrigger(Constant.attackQ);
		animatorHero.SetTrigger(Constant.idle);
		GameObject hero_bullet = CreateObjectPool(bulletPrefab, objectHand.transform.position);
		hero_bullet.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
		hero_bullet.GetComponent<SpriteRenderer>().sprite = listBullet[0];
		DelayObject(hero_bullet, 1.2f);
		LeanTween.alpha(hero_bullet, 1, 0.1f).setOnComplete(() => {
			hero_bullet.transform.position = objectHand.transform.position;
			GameObject animation_bullet = CreateObjectPool(listAnimationBullet[0], Vector3.zero, hero_bullet.transform);
			animation_bullet.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
			LeanTween.move(hero_bullet, vector_competitor, 0.5f).setOnComplete(() => {
				hero_bullet.GetComponent<SpriteRenderer>().sprite = listBullet[1];
				hero_bullet.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
				LeanTween.move(hero_bullet, vector_competitor, 0.3f);
				LeanTween.rotateZ(hero_bullet, 720, 0.4f).setRepeat(5).setOnComplete(() => {
					Auto_SelfDestruction_Object_Pool(ListFXSkill[0].gameObject, vector_competitor, 1f);
					LeanTween.rotateZ(hero_bullet, -720, 0.4f).setRepeat(5);
					LeanTween.move(hero_bullet, myBody.transform.position, 0.7f).setOnComplete(() => {
						SelfDestruction_Object_Pool(hero_bullet);
						SelfDestruction_Object_Pool(animation_bullet);
						ResetData();
					});
				});
			});
		}).setDelay(1f);
	}
	public override void Attack_W() {
		base.Attack_W();
		animatorHero.SetTrigger(Constant.dash1);
		animatorHero.SetTrigger(Constant.attackW);
		vectorMyBody = myBody.transform.position;
		LTBezierPath ltPath = new LTBezierPath(
		new Vector3[] { myBody.transform.position,
		vector1,
		vector2,
		vector_position_ememy});
		LeanTween.move(myBody, ltPath.pts, 0.8f).setOnComplete(() => {
			Auto_SelfDestruction_Object_Pool(ListFXSkill[0].gameObject, vector_competitor, 0.5f, 1f);
			animatorHero.SetTrigger(Constant.dash2);
			animatorHero.SetTrigger(Constant.idle);
			ltPath = new LTBezierPath(
			new Vector3[] { myBody.transform.position,
			vector2,
			vector1,
			new Vector3(vectorMyBody.x,vectorMyBody.y) });
			LeanTween.move(myBody, ltPath.pts, 1).setOnComplete(() => {
				ResetData();
			}).setDelay(1.04f);
		});
	}
	public override void Attack_E() {
		base.Attack_E();
		animatorHero.SetTrigger(Constant.attackE);
		vectorMyBody = myBody.transform.position;
		LeanTween.move(myBody, new Vector3(0, myBody.transform.position.y), 1.09f).setOnComplete(() => {
			LTBezierPath ltPath = new LTBezierPath(new Vector3[] { myBody.transform.position,
			vector1,
    		vector2,
			vector_position_ememy });
			LeanTween.move(myBody, ltPath.pts, 0.5f).setOnComplete(() => {
				Auto_SelfDestruction_Object_Pool(ListFXSkill[0].gameObject, vector_competitor, 1f);
				animatorHero.SetTrigger(Constant.dash2);
				animatorHero.SetTrigger(Constant.idle);
				ltPath = new LTBezierPath(new Vector3[] { myBody.transform.position,
				vector2,
				vector1,
				new Vector3(vectorMyBody.x, vectorMyBody.y) });
				LeanTween.move(myBody, ltPath.pts, 1).setOnComplete(() => {
					ResetData();
				}).setDelay(0.3f);
			});
		}).setDelay(0.2f);
	}
	public override void ResetData() {
		base.ResetData();
	}
	public override void RotateObject(GameObject gameObjects) {
		base.RotateObject(gameObjects);
	}
}