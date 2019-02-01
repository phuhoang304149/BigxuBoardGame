using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGrayScale : MonoBehaviour {

	public List<Renderer> myBodyPartRenderers;
	private MaterialPropertyBlock _propBlock;

	void Awake(){
		_propBlock = new MaterialPropertyBlock();
	}

	[ContextMenu("TestOn")]
	public void TestOn(){
		for (int i = 0; i < myBodyPartRenderers.Count; i++) {
			myBodyPartRenderers[i].GetPropertyBlock(_propBlock);
			_propBlock.SetFloat("_EffectAmount", 1f);
			myBodyPartRenderers[i].SetPropertyBlock(_propBlock);
		}
	}

	[ContextMenu("TestOff")]
	public void TestOff(){
		for (int i = 0; i < myBodyPartRenderers.Count; i++) {
			myBodyPartRenderers[i].GetPropertyBlock(_propBlock);
			_propBlock.SetFloat("_EffectAmount", 0f);
			myBodyPartRenderers[i].SetPropertyBlock(_propBlock);
		}
	}
}
