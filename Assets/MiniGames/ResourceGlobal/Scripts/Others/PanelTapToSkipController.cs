using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelTapToSkipController : MonoBehaviour {

	[SerializeField] CanvasGroup myCanvasGroup;

	public System.Action onTap;

	private void Awake() {
		Hide();
	}

	public void Show(){
		myCanvasGroup.alpha = 1f;
		myCanvasGroup.blocksRaycasts = true;
	}

	public void Hide(){
		myCanvasGroup.alpha = 0f;
		myCanvasGroup.blocksRaycasts = false;
		onTap = null;
	}

	public void OnTapToSkip(){
		if(onTap != null){
			onTap ();
			onTap = null;
		}
	}
}
