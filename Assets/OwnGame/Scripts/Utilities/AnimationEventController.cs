using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class AnimationEventController : MonoBehaviour {
	public Events events;

	public void OnFinished(){
		if(events != null
			&& events.onFnished != null){
			events.onFnished.Invoke();
		}
	}

	[System.Serializable] 
	public class Events{
		public UnityEvent onFnished;
	}
}
