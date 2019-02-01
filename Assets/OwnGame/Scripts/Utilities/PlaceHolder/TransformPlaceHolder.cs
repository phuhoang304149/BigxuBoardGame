using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] public class TransformPlaceHolder {
	public Vector3 position;
	public Quaternion rotation;
	public Vector3 localScale;

	public TransformPlaceHolder(Transform _transform){
		position = _transform.position;
		rotation = _transform.rotation;
		localScale = _transform.localScale;
	}
}
