using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MySimplePoolManager {

	public List<MySimplePoolObjectController> listObjects;
	int maxLength;

	public MySimplePoolManager(int _maxLength = -1){
		if(_maxLength == 0){
			Debug.LogError("BUG nhỗn làm");
		}
		listObjects = new List<MySimplePoolObjectController>();
		maxLength = _maxLength;
	}

	public void AddObject(MySimplePoolObjectController _newObject){
		listObjects.Add(_newObject);
		_newObject.onSelfDestruction = (_o)=>{
			listObjects.Remove(_o);
		};
		if(maxLength != -1){
			if(listObjects.Count > maxLength){
				listObjects[0].onSelfDestruction = null;
				listObjects[0].SelfDestruction();
				listObjects.RemoveAt(0);
			}
		}
	}

	public void ClearAllObjectsNow(){
		if(listObjects.Count == 0){
			return;
		}
		for(int i = 0; i < listObjects.Count; i++){
			listObjects[i].onSelfDestruction = null;
			listObjects[i].SelfDestruction();
		}
		listObjects.Clear();
	}
}
