using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCameraController : MonoBehaviour {

	public Camera mainCamera;
	public Vector2 sizeOfCamera = Vector2.zero;

	void Awake(){
		// if (ins != null && ins != this) { Destroy(this.gameObject);}
		// ins = this;
		// DontDestroyOnLoad (this.gameObject);
		
		SetResizeCameraAgain();
	}

	void SetResizeCameraAgain(){
		mainCamera.orthographicSize = (((float) Screen.height / (float) Screen.width) / 2f) * 10f;
		sizeOfCamera.y = mainCamera.orthographicSize * 2f;
		sizeOfCamera.x = (mainCamera.aspect * mainCamera.orthographicSize) * 2f;
	}
}
