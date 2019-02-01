using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ArcadeGame : MonoBehaviour {

	public static ArcadeGame instance {
		get {
			return ins;
		}
	}
	static ArcadeGame ins;
	public BOL_HeroInfo heroInfoPlayer;
	public BOL_HeroInfo heroInfoComp;
	public GameObject healthPlayer;
	public GameObject manaPlayer;
	public GameObject healthComp;

	public TextMesh TextShowHealth;
	public TextMesh TextShowMana;
	public TextMesh TextShowHealthComp;
	public float _baseHealthPlayer;
	public float _baseManaPlayer;
	public float _baseHealthComp;
	public float _baseHightHealthPlayer;
	public float _baseHightHealthComp;
	public float _baseHightManaPlayer;

	bool tobecontinue;

	void Awake() {
		ins = this;
	}
	void Start() {
		StartCoroutine(FirstInitData());
		_baseHightHealthPlayer = healthPlayer.transform.localScale.y;
		_baseHightManaPlayer = manaPlayer.transform.localScale.y;
	}
	IEnumerator FirstInitData() {
		yield return new WaitUntil(() => BOL_Battle_Screen.instance.finishInitData);
		BOL_Battle_Screen.instance.finishInitData = false;
		DoActionInit();
	}
	void DoActionInit() {
		_baseHealthPlayer = heroInfoPlayer.baseHp;
		_baseManaPlayer = heroInfoPlayer.baseMana;
		_baseHealthComp = heroInfoComp.baseHp;
		TextShowHealth.text = _baseHealthPlayer.ToString();
		TextShowMana.text = _baseManaPlayer.ToString();
		TextShowHealthComp.text = _baseHealthComp.ToString();
	}
	public void ResetData() {
		heroInfoPlayer = null;
		heroInfoComp = null;
		_baseHealthComp = 0;
		_baseHealthPlayer = 0;
		_baseManaPlayer = 0;
	}
	public void SelfDestruction() {
		ins = null;
		ResetData();
	}
	private void OnDestroy() {
		ins = null;
	}
	//IEnumerator TweenText() {
	//	while (_baseHealthPlayer>0) {
	//		tobecontinue = false;
	//		StartTween(TextShowHealth, _baseHealthPlayer, _baseHealthPlayer - 200);
	//		Debugs.LogRed(_baseHealthPlayer);
    //        _baseHealthPlayer -= 200;
	//		yield return new WaitUntil(() => tobecontinue);
	//	}
	//}
	IEnumerator DoActionTweenText(TextMesh text, float value1st, float value2nd) {
		LeanTween.scale(text.gameObject, new Vector3(2.2f,2.2f,1), 0.3f);
		float rate = value1st - value2nd;
		float i = 0;
		while (i < rate) {
			yield return Yielders.Get(0.01f);
			i+=(rate/10);
			value1st -= (rate/10);
			text.text = value1st.ToString();
		}
        LeanTween.scale(text.gameObject, new Vector3(1.7f,1.7f,1), 0.3f).setOnComplete(()=>{
        tobecontinue = true;
        });
	}
	IEnumerator _DoActionTweenText;
	public void StartTween(TextMesh text, float value1st, float value2nd) {
		//if (_DoActionTweenText != null) {
		//	StopCoroutine(_DoActionTweenText);
		//	_DoActionTweenText = null;
		//}
		//_DoActionTweenText = DoActionTweenText(text, value1st, value2nd);
        //StartCoroutine(_DoActionTweenText);
		StartCoroutine(DoActionTweenText(text, value1st, value2nd));
	}
	void StopTween() {
		if (_DoActionTweenText != null) {
			StopCoroutine(_DoActionTweenText);
			_DoActionTweenText = null;
		}
	}
    
    
}
