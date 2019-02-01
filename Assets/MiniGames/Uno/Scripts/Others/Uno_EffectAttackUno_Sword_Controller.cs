using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Uno_EffectAttackUno_Sword_Controller : MySimplePoolObjectController {

	[SerializeField] SpriteRenderer mySpriteRenderer;

	void Awake(){
		ResetData();
	}

	public override void ResetData(){
		LeanTween.cancel(gameObject);
		Color _c = mySpriteRenderer.color;
		_c.a = 0;
		mySpriteRenderer.color = _c;
	}

	public Coroutine SetUpMove(Vector3 _posTo){
 		Vector3 _dir = _posTo - this.transform.position;
		_dir.Normalize ();
		float zAngle = Mathf.Atan2 (_dir.y, _dir.x) * Mathf.Rad2Deg - 90;
		Quaternion desiredRot = Quaternion.Euler (0,0,zAngle);
		transform.rotation = desiredRot;
		return StartCoroutine(DoActionMove(_posTo));
	}

	IEnumerator DoActionMove(Vector3 _posTo){
		bool _isFinished = false;
		LeanTween.alpha(mySpriteRenderer.gameObject, 1f, 0.1f).setOnComplete(()=>{
			_isFinished = true;
		});
		yield return new WaitUntil(()=>_isFinished);

		_isFinished = false;
		LeanTween.move(gameObject, _posTo, 0.4f).setEase(LeanTweenType.easeInBack).setOnComplete(()=>{
			_isFinished = true;
		});
		yield return new WaitUntil(()=>_isFinished);
		
		SelfDestruction();
	}
}
