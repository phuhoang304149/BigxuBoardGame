using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hero11_Lux_controller : BOL_Hero_Controler {
	public GameObject ObjectMe;
	Vector3 vectorMe;
	public GameObject BeginBullet;
	public GameObject BeginBulletE;
	public GameObject LegObject;
	public override void InitData() {
		base.InitData();
		if (chairPositions == Constant.CHAIR_LEFT) {
			vectorMe = ObjectMe.transform.GetChild(0).transform.position;
		} else if (chairPositions == Constant.CHAIR_RIGHT) {
			vectorMe = ObjectMe.transform.GetChild(0).transform.position;
		}
	}
	public override void Attack1() {
		base.Attack1();
		animatorHero.SetTrigger(Constant.attack1);
		Delay(1, () => {
			GameObject hero_bullet = CreateObjectPool(bulletPrefab, BeginBullet.transform.position);
			hero_bullet.GetComponent<SpriteRenderer>().sprite = listBullet[0];
			GameObject animation_hero_bullet = CreateObjectPool(listAnimationBullet[0].gameObject, Vector3.zero, hero_bullet.transform);
			animation_hero_bullet.transform.localScale = Vector3.one * 2;
			LeanTween.move(hero_bullet, vector_competitor, 0.7f).setOnComplete(() => {
				SelfDestruction_Object_Pool(hero_bullet);
				SelfDestruction_Object_Pool(animation_hero_bullet);
				Auto_SelfDestruction_Object_Pool(ListFXSkill[0].gameObject, vector_competitor, 2);
				Delay(0.5f, ResetData);
			});
		});
	}

	public override void Attack2() {
		base.Attack2();
		animatorHero.SetTrigger(Constant.attack2);
		Auto_SelfDestruction_Object_Pool(ListFXSkill[1].gameObject, ObjectMe.transform.position, 1.8f, 10f);
		Delay(2.8f, ResetData);
	}
	public override void Attack_Q() {
		base.Attack_Q();
		animatorHero.SetTrigger(Constant.attackQ);
		Delay(1f, () => {
			GameObject hero_bullet = CreateObjectPool(bulletPrefab, BeginBullet.transform.position);
			hero_bullet.GetComponent<SpriteRenderer>().sprite = listBullet[0];
			LeanTween.move(hero_bullet, vector_competitor, 0.5f).setOnComplete(() => {
				SelfDestruction_Object_Pool(hero_bullet);
				Auto_SelfDestruction_Object_Pool(ListFXSkill[2].gameObject, vector_competitor + new Vector3(0, 0.5f, 0), 10f);
				Delay(1, ResetData);
			});
		});
	}
	public override void Attack_W() {
		base.Attack_W();
		animatorHero.SetTrigger(Constant.attackW);
		Delay(1.5f, () => {
			GameObject hero_bullet = CreateObjectPool(bulletPrefab, BeginBullet.transform.position);
			hero_bullet.GetComponent<SpriteRenderer>().sprite = listBullet[0];
			hero_bullet.transform.position = BeginBullet.transform.position;
			GameObject animation_hero_bullet = CreateObjectPool(listAnimationBullet[1].gameObject, Vector3.zero, hero_bullet.transform);
			LeanTween.move(hero_bullet, vector_competitor, 1f).setOnComplete(() => {
				SelfDestruction_Object_Pool(hero_bullet);
				SelfDestruction_Object_Pool(animation_hero_bullet);
				GameObject fxBullet = CreateObjectPool(ListFXSkill[3].gameObject, new Vector3(vector_competitor.x, LegObject.transform.position.y - 0.2f, 0));
				fxBullet.transform.eulerAngles = new Vector3(-65, 0, 0);
				SelfDestruction_Object_Pool(fxBullet, 1.5f);
				Delay(1, () => {
					GameObject fxSkill = CreateObjectPool(ListFXSkill[4].gameObject, vector_competitor);
					Delay(0.5f, ResetData);
				});
			});
		});
	}
	public override void Attack_E() {
		base.Attack_E();
		animatorHero.SetTrigger(Constant.attackE);
		Delay(1.5f, () => {
			GameObject herobullet = CreateObjectPool(listAnimationBullet[3].gameObject, BeginBulletE.transform.position);
			herobullet.transform.eulerAngles = new Vector3(0, 90, 0);
			herobullet.GetComponent<ParticleSystem>().Play();
            
			Delay(1f,()=>{
				SelfDestruction_Object_Pool(herobullet);
            });
		});
		Delay(3, ResetData);
	}

	public override void ResetData() {
		base.ResetData();
	}
}
