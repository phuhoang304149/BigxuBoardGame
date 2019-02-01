using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class BOL_UI_Offline : MonoBehaviour {

	public CanvasGroup canvasScene;
	public enum UIType {
		unknow,
		home,
		campaign,
		arcade,
		survival,
		timed_mode,
		tutorial,
	}
	public virtual UIType mySceneType {
		get {
			return UIType.home;
		}
	}

	public UIType myLastType { get; set; }
    
	public virtual void InitData() { }
	public virtual void ResetData() { 
    
    
    }
	public virtual void RefreshData() { }
	public virtual void Show() {
		gameObject.SetActive(true);
		canvasScene.alpha = 1;
		canvasScene.interactable = true;
		canvasScene.blocksRaycasts = true;
	}
	public virtual void Hide() {
		gameObject.SetActive(false);
		canvasScene.alpha = 0;
		canvasScene.interactable = false;
		canvasScene.blocksRaycasts = false;
		StopAllCoroutines();
	
	}
	public virtual void SelfDestruction() {
		LeanPool.Despawn(gameObject);
	}
}
