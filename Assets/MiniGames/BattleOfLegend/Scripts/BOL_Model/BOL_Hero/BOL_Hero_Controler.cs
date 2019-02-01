using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using UnityEngine.Serialization;
using System;

public class BOL_Hero_Controler : MonoBehaviour {
	[Header(">>>>>>Hero controller<<<<<<")]
	public GameObject ParentBody;
	public GameObject myBody;
	public Animator animatorHero;
	public GameObject bulletPrefab;
	public Vector3 vectorMyBody;
	public Vector3 vector;
	public Vector3 vector_competitor;
	public Vector3 vector_competitor_parent;
	public Vector3 vector_position_ememy;

	public AudioClip sfx_Attack1;
	public AudioClip sfx_Attack2;
	public AudioClip sfx_AttackQ;
	public AudioClip sfx_AttackW;
	public AudioClip sfx_AttackE;
	public AudioClip sfx_Attack_Special;

	public AudioClip sfx_Start;

	/// <summary>
	/// sprites bullet
	/// </summary>
	public List<Sprite> listBullet;
	/// <summary>
	/// particle sau khi chạm
	/// </summary>
	public List<ParticleSystem> ListFXSkill;
	/// <summary>
	/// animation theo sau bullet
	/// </summary>
	public List<GameObject> listAnimationBullet;
	public int chairPositions;
	public virtual void InitData() {
		// vector_competitor = BOL_SetupGame.instance.heroEnemy.transform.position;
		vectorMyBody = myBody.transform.position;
		if (chairPositions == Constant.CHAIR_LEFT) {
			vector = new Vector3(0, 0, 0);
		} else if (chairPositions == Constant.CHAIR_RIGHT) {
			vector = new Vector3(0, 180, 0);
		}
	}
	public virtual void RefreshData() { }
	public virtual void ResetData() {
		animatorHero.speed = 1f;
		animatorHero.SetTrigger(Constant.idle);
		animatorHero.Update(0.1f);
		LeanTween.delayedCall(1f, () => {
			BOL_PlaySkill_Controller.instance.isFinish = true;
		});

	}
	public virtual void DestroyData() {
		LeanPool.Despawn(gameObject);
		
	}
	public virtual void Attack1() {
		UpdatePositionCompetitor();
	}
	public virtual void Attack2() {
		UpdatePositionCompetitor();
	}
	public virtual void Attack_Q() {
		UpdatePositionCompetitor();
	}
	public virtual void Attack_W() {
		UpdatePositionCompetitor();
	}
	public virtual void Attack_E() {
		UpdatePositionCompetitor();
	}
	public virtual void Attack_Spell() {
		UpdatePositionCompetitor();
	}
	public GameObject CreateObjectPool(GameObject prefab, Vector3 position, Transform parent = null) {
		if (parent != null) {
			return LeanPool.Spawn(prefab, position, Quaternion.Euler(vector), parent);
		} else return LeanPool.Spawn(prefab, position, Quaternion.Euler(vector));
	}
	public void SelfDestruction_Object_Pool(GameObject objectPool) {
		if (objectPool == null || !objectPool.activeSelf) {
			return;
		}
		// LeanPool.Despawn(objectPool);
		Destroy(objectPool);
	}
	public void SelfDestruction_Object_Pool(GameObject objectPool, float timeDespawn) {
		if (objectPool == null || !objectPool.activeSelf) {
			return;
		}
		_Auto_SelfDestruction_Object_Pool(objectPool, timeDespawn);
	}
	public void Auto_SelfDestruction_Object_Pool(GameObject prefab, Vector3 position, float timeDestroy) {
		GameObject fx = LeanPool.Spawn(prefab);
		fx.transform.position = position;
		fx.transform.eulerAngles = vector;
		if (fx.GetComponent<ParticleSystem>() != null) {
			fx.GetComponent<ParticleSystem>().Play();
		}
		LeanTween.delayedCall(timeDestroy, () => {
			// LeanPool.Despawn(fx);
			Destroy(fx);
		});

		//StartCoroutine(_Auto_SelfDestruction_Object_Pool(fx, timeDestroy));
	}
	public void Auto_SelfDestruction_Object_Pool(GameObject prefab, Vector3 position, float timeCreate, float timeDestroy) {
		DelayCreateAndDespaw(prefab, position, vector, timeCreate, timeDestroy);
	}
	public void Auto_SelfDestruction_Object_Pool(GameObject prefab, Vector3 position, Vector3 angle, float timeCreate, float timeDestroy) {
		DelayCreateAndDespaw(prefab, position, angle, timeCreate, timeDestroy);
	}
	public void Delay(float time, Action method) {
		StartCoroutine(_Delay(time, method));
	}
	public void DelayObject(GameObject game, float time) {
		DelayActiveObject(game, time);
	}
	void DelayCreateAndDespaw(GameObject prefab, Vector3 position, Vector3 angle, float timeCreate, float timeDestroy) {
		LeanTween.delayedCall(timeCreate, () => {
			GameObject fx = LeanPool.Spawn(prefab, position, Quaternion.Euler(vector));
			fx.transform.eulerAngles = angle;
			if (fx.GetComponent<ParticleSystem>() != null) {
				fx.GetComponent<ParticleSystem>().Play();
			}
			LeanTween.delayedCall(timeDestroy, () => {
				// LeanPool.Despawn(fx);
				Destroy(fx);
			});
		});

	}
	void _Auto_SelfDestruction_Object_Pool(GameObject objectprefab, float time) {
		LeanTween.delayedCall(time, () => {
			// LeanPool.Despawn(objectprefab);
				Destroy(objectprefab);
		});

	}
	void DelayActiveObject(GameObject objectActive, float time) {
		objectActive.SetActive(false);
		LeanTween.delayedCall(time, () => {
			objectActive.SetActive(true);
		});

	}
	IEnumerator _Delay(float time, Action method) {
		yield return Yielders.Get(time);
		method();
	}

	public void UpdatePositionCompetitor() {
		if (chairPositions == Constant.CHAIR_LEFT) {
			if (BOL_Main_Controller.ins != null) {
				vector_competitor_parent = BOL_Main_Controller.instance._chairRightSpawn.transform.position;
				vector_competitor = BOL_Main_Controller.instance._chairRightSpawn.transform.GetChild(0).transform.position;// release
				vector_position_ememy = vector_competitor - new Vector3(1, 0);
			}
		} else if (chairPositions == Constant.CHAIR_RIGHT) {
			if (BOL_Main_Controller.ins != null) {
				vector_competitor_parent = BOL_Main_Controller.instance._chairLeftSpawn.transform.position;
				vector_competitor = BOL_Main_Controller.instance._chairLeftSpawn.transform.GetChild(0).transform.position;//release
				vector_position_ememy = vector_competitor + new Vector3(1, 0);
			}
		}
	}

	public virtual void RotateObject(GameObject gameObjects) {

		if (vector_competitor.x < 0 && gameObjects.transform.localScale.x > 0) {
			gameObjects.transform.localScale = new Vector3(gameObjects.transform.localScale.x * -1, gameObjects.transform.localScale.y);
		} else if (vector_competitor.x > 0 && gameObjects.transform.localScale.x < 0) {
			gameObjects.transform.localScale = new Vector3(gameObjects.transform.localScale.x * -1, gameObjects.transform.localScale.y);
		}
	}


	#region OFFLINE
	public virtual void InitDataOffline() {
		vectorMyBody = myBody.transform.position;
		vector = new Vector3(0, 0, 0);
	}
	public virtual void Attack_1_Offline(Vector3 vectorhero) {
		UpdatePositionOffline();
	}
	public virtual void Attack_2_Offline() {
		UpdatePositionOffline();
	}
	public virtual void Attack_Q_Offline() {
		UpdatePositionOffline();
	}
	public virtual void Attack_W_Offline() {
		UpdatePositionOffline();
	}
	public virtual void Attack_E_Offline() {
		UpdatePositionOffline();
	}

	void UpdatePositionOffline() {
		vector_competitor = BOL_Battle_Screen.instance._heroComp.transform.position;
	}
	public void UpdatePosPlayer() {
		vector_competitor = BOL_Battle_Screen.instance._heroPlayer.transform.position;
	}

	#endregion
}
