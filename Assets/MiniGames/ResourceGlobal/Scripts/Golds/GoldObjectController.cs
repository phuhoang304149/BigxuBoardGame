using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GoldObjectController : MySimplePoolObjectController {

	[SerializeField] SortingGroup mySortingGroup;

	float ratioScale = 1f;

	public override void ResetData(){
		StopAllCoroutines();
		LeanTween.cancel(gameObject);
		mySortingGroup.sortingLayerName = MyConstant.SORTINGLAYERNAME_DEFAULT;
		mySortingGroup.sortingOrder = 0;
		ratioScale = 1f;
	}

	public void InitData(MySortingLayerInfo _sortingLayerInfo, float _ratioScale){
		mySortingGroup.sortingLayerName = _sortingLayerInfo.layerName.ToString();
		mySortingGroup.sortingOrder = _sortingLayerInfo.layerOrderId;
		ratioScale = _ratioScale;
		transform.localScale = Vector3.one * ratioScale;
	}

	public Coroutine MoveAndSelfDestruction(Vector2 _pos, float _timeMove, LeanTweenType _leanTweenType, System.Action _onFinished = null){
		return StartCoroutine(DoActionMoveAndSelfDestruction(_pos, _timeMove, _leanTweenType, _onFinished));
	}

	IEnumerator DoActionMoveAndSelfDestruction(Vector2 _pos, float _timeMove, LeanTweenType _leanTweenType, System.Action _onFinished = null){
		bool _isFinished = false;
		LeanTween.move(gameObject, _pos, _timeMove).setOnComplete(()=>{
			_isFinished = true;
		}).setEase(_leanTweenType).setDelay(0.2f);
		yield return new WaitUntil(()=>_isFinished);
		if(_onFinished != null){
			_onFinished();
		}
		SelfDestruction();
	}
}
