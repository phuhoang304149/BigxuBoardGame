using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour {

	public static bool canEnable{
		get{ 
			#if TEST
			return true;
			#endif
			return false;
		}
	}

	public static DebugManager instance{
		get{
			return ins;
		}
	}
	private static DebugManager ins;

	public DataManager dataManager;

	void Awake() {
		if (canEnable) {
			if (ins != null && ins != this) {
				Destroy (this.gameObject);
				return;
			}
			ins = this;
			DontDestroyOnLoad (this.gameObject);
			return;
		}
		Destroy(this.gameObject);
	}

	void Start(){
		dataManager = DataManager.instance;
	}
}
