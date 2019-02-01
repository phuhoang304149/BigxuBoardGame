using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWhiteFlashSprite : MonoBehaviour {

	public enum SubState{
		None, Hit
	}
	SubState subState;

	public List<Renderer> myBodyPartRenderers;
	protected Color currentColor = Color.white;
	protected float currentflashAmount = 0f;
	private MaterialPropertyBlock _propBlock;

	void Awake(){
		_propBlock = new MaterialPropertyBlock();
		subState = SubState.None;
		ResetBodyColor();
	}

	IEnumerator Start() {
		while(true){
			yield return Yielders.Get(0.5f);
			SetUpEffectHitToBody();
		}
	}

	protected void ResetBodyColor ()
	{
		currentColor = Color.white;
		currentflashAmount = 0f;
	}

	public void SetUpEffectHitToBody(){
	if(subState != SubState.Hit){
		subState = SubState.Hit;
		for (int i = 0; i < myBodyPartRenderers.Count; i++) {
			myBodyPartRenderers[i].GetPropertyBlock(_propBlock);
			_propBlock.SetFloat("_FlashAmount", 0.5f);
			myBodyPartRenderers[i].SetPropertyBlock(_propBlock);
		}

		StartCoroutine(DoActionHit(0.05f));
	}
}

	public IEnumerator DoActionHit(float _time){
		yield return Yielders.Get(_time);
		subState = SubState.None;
		ResetPiecesOfBody();
	}

	protected void ResetPiecesOfBody(){
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
