using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BOL_FlashingSprite : MonoBehaviour {
	public enum SubState {
		None, Hit
	}
	SubState subState;
	public Color colorsss;
	public List<Renderer> myBodyPartRenderers;
	protected Color currentColor;
	protected float currentflashAmount = 0f;
	private MaterialPropertyBlock _propBlock;
    public void Initdata(){
		currentColor = colorsss;
        _propBlock = new MaterialPropertyBlock();
        subState = SubState.None;
        ResetBodyColor();
    }
    public void StartFlash(){
    if(_doAction== null){
			_doAction = DoAction();
			StartCoroutine(_doAction);
    }
    }
    public void StopFlash(){
    if(_doAction!= null){
			StopCoroutine(_doAction);
			_doAction = null;
    }
    }
	IEnumerator _doAction;
    
	IEnumerator DoAction() {
		while (true) {
			yield return Yielders.Get(1f);
			SetUpEffectHitToBody();
		}
	}
	protected void ResetBodyColor() {
		currentColor = colorsss;
		currentflashAmount = 0f;
	}
	public void SetUpEffectHitToBody() {
		if (subState != SubState.Hit) {
			subState = SubState.Hit;
			for (int i = 0; i < myBodyPartRenderers.Count; i++) {
				myBodyPartRenderers[i].GetPropertyBlock(_propBlock);
				_propBlock.SetFloat("_FlashAmount", 0.5f);
				myBodyPartRenderers[i].SetPropertyBlock(_propBlock);
			}
			StartCoroutine(DoActionHit(0.05f));
		}
	}
	public IEnumerator DoActionHit(float _time) {
		yield return Yielders.Get(_time);
		subState = SubState.None;
		ResetPiecesOfBody();
	}
	protected void ResetPiecesOfBody() {
		for (int i = 0; i < myBodyPartRenderers.Count; i++) {
			myBodyPartRenderers[i].GetPropertyBlock(_propBlock);
			// Assign our new value.
			_propBlock.SetColor("_FlashColor", currentColor);
			_propBlock.SetFloat("_FlashAmount", currentflashAmount);
			// Apply the edited values to the renderer.
			myBodyPartRenderers[i].SetPropertyBlock(_propBlock);
		}
	}
}
//#if UNITY_EDITOR
//[CustomEditor(typeof(BOL_FlashingSprite))]
//public class Test_SetColor : Editor {
//    public float values = 40;
//    public string Accept;
//    public override void OnInspectorGUI() {
//        base.OnInspectorGUI();
//        BOL_FlashingSprite myScript = (BOL_FlashingSprite)target;
//		//EditorGUILayout.ColorField(myScript.colorsss);
//    }
//}
//#endif
