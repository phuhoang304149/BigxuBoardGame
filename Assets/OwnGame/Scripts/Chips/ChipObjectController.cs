using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Lean.Pool;

/**
** ChipObjectController: chip dạng gameobject để quăng ra màn hình khi đặt cược
**/
public class ChipObjectController : MySimplePoolObjectController {

	[SerializeField] IChipInfo.ChipType chipType{get;set;}
	[SerializeField] SortingGroup mySortingGroup;
	[SerializeField] SpriteRenderer mySprite;

	System.Action<int> onMoveFinished;
	int indexSaved;
	float ratioScale = 1f;

	private void Awake() {
		ResetData();
	}
	public override void ResetData(){
		Color _c = mySprite.color;
		_c.a = 0f;
		mySprite.color = _c;
		
		ratioScale = 1f;

		onMoveFinished = null;
	}

	public void SetData(IChipInfo _chipInfo, MySortingLayerInfo _sortingLayerInfo, float _ratioScale){
		mySortingGroup.sortingLayerName = _sortingLayerInfo.layerName.ToString();
		mySortingGroup.sortingOrder = _sortingLayerInfo.layerOrderId;
		ratioScale = _ratioScale;
		transform.localScale = Vector3.one * ratioScale;
		
		chipType = _chipInfo.chipType;
		mySprite.sprite = _chipInfo.mySprite;
	}

	public void RegisCallbackJustMoveFinished(int _index, System.Action<int> _onMoveFinished){
		indexSaved = _index;
		onMoveFinished = _onMoveFinished;
	}

	public void SetUpMoveToTableBet(Vector3 _pos){
		Color _c = mySprite.color;

		LeanTween.value(gameObject, 0f, 1f, 0.1f).setOnUpdate((_value)=>{
			_c = mySprite.color;
			_c.a = _value;
			mySprite.color = _c;
		});

		LeanTween.move(gameObject, _pos, 0.2f).setOnComplete(()=>{
			if(onMoveFinished != null){
				onMoveFinished(indexSaved);
			}
			LeanTween.value(gameObject, 1f, 0f, 0.5f).setOnUpdate((_value)=>{
				_c = mySprite.color;
				_c.a = _value;
				mySprite.color = _c;
			}).setOnComplete(()=>{
				SelfDestruction();
			}).setDelay(2f);
		});
	}

	public void SetUpMoveToPlayer(Vector3 _pos){
		Color _c = mySprite.color;

		LeanTween.value(gameObject, 0f, 1f, 0.1f).setOnUpdate((_value)=>{
			_c = mySprite.color;
			_c.a = _value;
			mySprite.color = _c;
		});

		LeanTween.move(gameObject, _pos, 0.4f).setEase(LeanTweenType.easeInBack).setOnComplete(()=>{
			if(onMoveFinished != null){
				onMoveFinished(indexSaved);
			}
			LeanTween.value(gameObject, 1f, 0f, 0.5f).setOnUpdate((_value)=>{
				_c = mySprite.color;
				_c.a = _value;
				mySprite.color = _c;
			}).setOnComplete(()=>{
				SelfDestruction();
			});
		}).setDelay(Random.Range(0.5f, 1f));
	}
}
