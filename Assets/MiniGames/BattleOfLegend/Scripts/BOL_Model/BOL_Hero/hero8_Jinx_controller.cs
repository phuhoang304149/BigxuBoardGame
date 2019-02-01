using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hero8_Jinx_controller : BOL_Hero_Controler {
	[Header("only hero")]
	public GameObject bullet1;
	public GameObject bullet2;
	public GameObject hand;
	public override void InitData() {
		base.InitData();
		if (chairPositions == Constant.CHAIR_LEFT) {
		} else if (chairPositions == Constant.CHAIR_RIGHT) {
		}
	}
	public override void Attack1() {
		base.Attack1();
		animatorHero.SetTrigger(Constant.attack1);
		Delay(0.8f, () => {
			Delay(0, () => { _attack1(0); });
			Delay(0.1f, () => { _attack1(1); });
			Delay(0.2f, () => { _attack1(0); });
			Delay(0.3f, () => { _attack1(1); });
			Delay(0.4f, () => { _attack1(0); });
			Delay(0.5f, () => {
				Auto_SelfDestruction_Object_Pool(ListFXSkill[0].gameObject, vector_competitor, 2f);
				ResetData();
			});
		});
	}
	public void _attack1(int posImg) {
		GameObject hero_bullet = CreateObjectPool(bulletPrefab, new Vector3(bullet2.transform.position.x, bullet2.transform.position.y - posImg / 1.8f));
		hero_bullet.GetComponent<SpriteRenderer>().sprite = listBullet[posImg];
		hero_bullet.transform.eulerAngles = Vector3.zero;
		LeanTween.move(hero_bullet,
	  new Vector3(vector_competitor.x, vector_competitor.y)
		, 0.2f).setOnComplete(() => {
			SelfDestruction_Object_Pool(hero_bullet);

		});
	}
	public override void Attack2() {
		base.Attack2();
		animatorHero.SetTrigger(Constant.attack2);
		Delay(1.1f, () => {
			GameObject hero_bullet = CreateObjectPool(bulletPrefab,
			new Vector3(bullet1.transform.position.x, bullet1.transform.position.y - 0.5f)
			);
			hero_bullet.GetComponent<SpriteRenderer>().sprite = listBullet[2];
			LTBezierPath ltPath = new LTBezierPath(new Vector3[] { new Vector3( bullet1.transform.position.x, bullet1.transform.position.y-0.5f),
			 new Vector3( 0, bullet1.transform.position.y+0.5f),
			 new Vector3( 0, bullet1.transform.position.y+0.5f),
			  vector_competitor });
			LeanTween.move(hero_bullet, ltPath.pts, 0.5f).setOnComplete(() => {
				hero_bullet.transform.eulerAngles = Vector3.zero;
				SelfDestruction_Object_Pool(hero_bullet);
				Auto_SelfDestruction_Object_Pool(ListFXSkill[0].gameObject, vector_competitor, 1f);
			});
			LeanTween.rotateZ(hero_bullet, 0, 0.25f).setOnComplete(() => {
				LeanTween.rotateZ(hero_bullet, -36, 0.25f);
				Delay(0.5f, ResetData);
			});
			if (chairPositions == Constant.CHAIR_LEFT) {
				hero_bullet.transform.eulerAngles = new Vector3(0, 0, 15);
			} else if (chairPositions == Constant.CHAIR_RIGHT) {
				hero_bullet.transform.eulerAngles = new Vector3(0, 180, 15);
			}
		});
	}
	public override void Attack_Q() {
		base.Attack_Q();
		animatorHero.SetTrigger(Constant.attackQ);
		Delay(1, () => {
			GameObject hero_bullet = CreateObjectPool(bulletPrefab, hand.transform.position);
			hero_bullet.transform.eulerAngles = Vector3.zero;
			hero_bullet.GetComponent<SpriteRenderer>().sprite = listBullet[3];
			LeanTween.move(hero_bullet, vector_competitor, 0.5f).setOnComplete(() => {
				SelfDestruction_Object_Pool(hero_bullet);
				Auto_SelfDestruction_Object_Pool(ListFXSkill[1].gameObject, vector_competitor, 3f);
				Delay(0.5f, ResetData);
			});
		});
	}
	public override void Attack_W() {
		base.Attack_W();
		animatorHero.SetTrigger(Constant.attackW);
		Delay(0.5f, () => {
			GameObject hero_bullet = CreateObjectPool(bulletPrefab, hand.transform.position);
			hero_bullet.GetComponent<SpriteRenderer>().sprite = listBullet[4];
			LTBezierPath ltPath = new LTBezierPath(new Vector3[] { bullet2.transform.position,
			new Vector3(0,bullet2.transform.position.y+ 1, 0),
			new Vector3(0,bullet2.transform.position.y+ 1, 0),
			vector_competitor });
			LeanTween.move(hero_bullet, ltPath.pts, 1).setOnComplete(() => {
				SelfDestruction_Object_Pool(hero_bullet);
				Auto_SelfDestruction_Object_Pool(ListFXSkill[0].gameObject, vector_competitor, 3);
				Delay(0.5f, ResetData);
			});
		});
	}
	public override void Attack_E() {
		base.Attack_E();
		animatorHero.SetTrigger(Constant.attackE);
		Delay(1.1f, () => {
			GameObject bullet = CreateObjectPool(bulletPrefab, new Vector3(bullet1.transform.position.x, bullet1.transform.position.y - 0.5f));
			bullet.GetComponent<SpriteRenderer>().sprite = listBullet[5];
		int id=	LeanTween.rotateAround(bullet, Vector3.left, 360, 1).setRepeat(-1).id;
			LeanTween.move(bullet, vector_competitor, 0.5f).setOnComplete(() => {
				SelfDestruction_Object_Pool(bullet);
				Auto_SelfDestruction_Object_Pool(listAnimationBullet[1].gameObject, vector_competitor, 2);
				//Auto_SelfDestruction_Object_Pool(ListFXSkill[4].gameObject, vector_competitor, 2, 4);
				Delay(4, ResetData);
				LeanTween.cancel(bullet, id);
			});
		});
	}
	public override void ResetData() {
		base.ResetData();
	}
}
