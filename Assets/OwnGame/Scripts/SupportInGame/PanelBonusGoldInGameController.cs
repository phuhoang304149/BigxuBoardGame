using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;

public class PanelBonusGoldInGameController : MySimplePoolObjectController {

	[SerializeField] Text txtGold;
	[SerializeField] Color colorBonus;
	[SerializeField] Color colorDevide;

	private void Awake() {
		ResetData();
	}
	public override void ResetData(){
		txtGold.text = string.Empty;
		Color _c = txtGold.color;
		_c.a = 0f;
		txtGold.color = _c;
	}

	public void Show(long _goldAdd){
		Vector3 _pos = transform.position;
		_pos.y -= 0.1f;
		transform.position = _pos;

		if(_goldAdd >= 0){
			txtGold.text = "+" + MyConstant.GetMoneyString(_goldAdd, 9999);
			txtGold.color = colorBonus;
		}else{
			_goldAdd = _goldAdd * (-1);
			txtGold.text = "-" + MyConstant.GetMoneyString(_goldAdd, 9999);
			txtGold.color = colorDevide;
		}
		
		Color _c = txtGold.color;
		LeanTween.value(gameObject, 0f, 1f, 0.2f).setOnUpdate((_value)=>{
			_c = txtGold.color;
			_c.a = _value;
			txtGold.color = _c;
		}).setOnComplete(()=>{
			LeanTween.value(gameObject, 1f, 0f, 0.2f).setOnUpdate((_value)=>{
				_c = txtGold.color;
				_c.a = _value;
				txtGold.color = _c;
			}).setDelay(1.7f);
		});
		
		_pos.y += 0.4f;
		LeanTween.move(gameObject, _pos, 2f).setOnComplete(()=>{
			SelfDestruction();
		});
	}
}
