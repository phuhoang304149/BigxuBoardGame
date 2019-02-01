using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class MySimplePoolObjectController : MonoBehaviour {

	[SerializeField] bool autoSelfDestruction;
	[SerializeField] float timeAutoSelfDestruction;

	public System.Action<MySimplePoolObjectController> onSelfDestruction;

	public virtual void ResetData(){}
	
	protected bool hasSpawned;
	
	protected IEnumerator DoActionAutoSelfDestruction(){
		yield return Yielders.Get(timeAutoSelfDestruction);
		SelfDestruction();
	}

	public virtual void SelfDestruction(){
		if(!hasSpawned){
			return;
		}
//		Debug.Log ("SelfDestruction");
		StopAllActionNow();
		if(onSelfDestruction != null){
			onSelfDestruction(this);
			onSelfDestruction = null;
		}
		LeanPool.Despawn(gameObject);
	}

	public virtual void StopAllActionNow(){
		StopAllCoroutines();
		LeanTween.cancel(gameObject);
	}

	protected void OnSpawn(){
		hasSpawned = true;
		if(autoSelfDestruction){
			StartCoroutine(DoActionAutoSelfDestruction());
		}
	}

	protected void OnDespawn(){
		hasSpawned = false;
		ResetData ();
	}

	protected void OnDestroy(){
		StopAllCoroutines();
	}
}
